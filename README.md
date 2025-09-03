# Putty Chooser

A Windows Forms app (.NET Framework) that lists all connected serial ports as buttons. Clicking a button launches PuTTY for that port at 115200 baud. A refresh button updates the list.

## Usage
- Ensure `putty.exe` is in your PATH or in the same folder as this app.
- Run the app.
- Click a port button to open PuTTY at 115200 baud.
- Click "Refresh" to update the port list.

## Requirements
- .NET Framework (Windows)
- PuTTY (`putty.exe`)

## How it works
- Uses `System.IO.Ports.SerialPort.GetPortNames()` to list ports.
- Launches PuTTY with `-serial` and `-sercfg 115200,8,n,1,N` arguments.
