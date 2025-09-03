# CVLog Serial Tool (Putty Chooser)

A Windows Forms app (.NET Framework) that lists all connected serial ports as buttons. Supports launching both PuTTY and RealTerm for serial port connections at 115200 baud. A refresh button updates the port list.

## Features
- **Automatic Detection**: Automatically finds PuTTY and RealTerm executables wherever they're installed
- **Dual Terminal Support**: Choose between PuTTY or RealTerm for each port
- **Port Description**: Shows device descriptions alongside port names
- **No Configuration Required**: Works out-of-the-box with standard installations

## Usage
- Install PuTTY and/or RealTerm (optional: the app will detect them automatically)
- Run the app
- Click "Open with PuTTY" or "Open with RealTerm" for any port
- Click "Refresh" to update the port list

## Automatic Detection
The app automatically searches for executables in these locations:
- System PATH
- Program Files directories
- Chocolatey installations
- Application directory
- Desktop (for portable versions)
- Common installation paths

## Manual Configuration (Optional)
If automatic detection fails, you can specify paths in the .config file:
```xml
<appSettings>
    <add key="PuttyPath" value="C:\path\to\putty.exe" />
    <add key="RealTermPath" value="C:\path\to\realterm.exe" />
</appSettings>
```

## Requirements
- .NET Framework 4.8 (Windows)
- PuTTY and/or RealTerm (automatically detected)

## How it works
- Uses `System.IO.Ports.SerialPort.GetPortNames()` to list ports
- Uses WMI to get device descriptions
- Automatically finds terminal programs using `ExecutableFinder` utility
- Launches PuTTY with `-serial` and `-sercfg 115200,8,n,1,N` arguments
- Launches RealTerm with port-specific configuration
