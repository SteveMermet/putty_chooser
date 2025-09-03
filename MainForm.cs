using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Management;

namespace CVLog_serial_tool
{
    public class MainForm : Form
    {
    private TableLayoutPanel portTable;
    private Button refreshButton;

        public MainForm()
        {
            Text = "Putty Serial Port Chooser";
            Width = 400;
            Height = 400;

            portTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 3,
                RowCount = 1,
                AutoSize = true
            };
            portTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            portTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            portTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            refreshButton = new Button
            {
                Text = "Refresh",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            refreshButton.Click += (s, e) => RefreshPorts();

            Controls.Add(portTable);
            Controls.Add(refreshButton);

            Load += (s, e) => RefreshPorts();
        }

        private void RefreshPorts()
        {
            portTable.Controls.Clear();
            portTable.RowStyles.Clear();
            var ports = SerialPort.GetPortNames().OrderBy(p => p).ToList();
            // Get port descriptions using WMI
            var portDescriptions = ports.ToDictionary(p => p, p => p);
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT DeviceID, Description FROM Win32_SerialPort"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string deviceId = obj["DeviceID"]?.ToString();
                        string desc = obj["Description"]?.ToString();
                        if (!string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(desc) && portDescriptions.ContainsKey(deviceId))
                        {
                            portDescriptions[deviceId] = $"{deviceId} - {desc}";
                        }
                    }
                }
            }
            catch { /* ignore WMI errors */ }
            int buttonHeight = 40;
            int refreshHeight = refreshButton.Height;
            int realtermHeight = 40; // Use fixed height for header
            int padding = 60; // for window chrome and spacing
            portTable.RowCount = ports.Count;
            for (int i = 0; i < ports.Count; i++)
            {
                var port = ports[i];
                var portLabel = new Label
                {
                    Text = portDescriptions[port],
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Fill,
                    Height = buttonHeight
                };
                var puttyBtn = new Button
                {
                    Text = "Open with PuTTY",
                    Tag = port,
                    Height = buttonHeight,
                    Dock = DockStyle.Fill
                };
                puttyBtn.Click += (s, e) => LaunchPutty((string)puttyBtn.Tag);

                var realtermBtn = new Button
                {
                    Text = "Open with RealTerm",
                    Tag = port,
                    Height = buttonHeight,
                    Dock = DockStyle.Fill
                };
                realtermBtn.Click += (s, e) => LaunchRealTermForPort((string)realtermBtn.Tag);

                portTable.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonHeight));
                portTable.Controls.Add(portLabel, 0, i);
                portTable.Controls.Add(puttyBtn, 1, i);
                portTable.Controls.Add(realtermBtn, 2, i);
            }
            // Dynamically set window height
            int totalHeight = ports.Count * buttonHeight + refreshHeight + realtermHeight + padding;
            int minHeight = 100;
            int maxHeight = 800;
            Height = Math.Max(minHeight, Math.Min(maxHeight, totalHeight));
        }

        private void LaunchRealTermForPort(string port)
        {
            var realtermPath = ConfigurationManager.AppSettings["RealTermPath"] ?? "C:\\Program Files (x86)\\BEL\\Realterm\\realterm.exe";
                // Strip 'COM' prefix if present
                string portNum = port.StartsWith("COM", StringComparison.OrdinalIgnoreCase) ? port.Substring(3) : port;
                var args = $"PORT={portNum} BAUD=115200 " +
                           "DISPLAY=1 " +
                           "SENDSTR=\"controlModem -p\" " +
                           "SENDSTR=\"controlModem -s\" " +
                           "SENDSTR=\"updateModem -s\" " +
                           "SENDSTR=\"updateModem\" " +
                           "SENDSTR=\"test\" " +
                           "CR=1 LF=1";
            try
            {
                Process.Start(realtermPath, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch RealTerm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LaunchPutty(string port)
        {   var puttyPath = ConfigurationManager.AppSettings["PuttyPath"] ?? "C:\\Users\\mermet\\Desktop\\putty.exe";
            var args = $"-serial {port} -sercfg 115200,8,n,1,N";
            try
            {
                Process.Start(puttyPath, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch PuTTY: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
