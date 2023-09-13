# DatalogicComHelper

`DatalogicComHelper` is a C# library designed to assist in communicating with Datalogic devices (tested with 320N). The library provides simple methods to start the phase mode and one-shot mode of the devices both synchronously and asynchronously, connecting via TCP/IP.

## Installation

**Via NuGet**:  
You can install the library via NuGet package manager by searching for `DatalogicComHelper` or using the following command:
```
Install-Package DatalogicComHelper
```

## Usage

First you need to configure the device using Datalogic software DL.CODE, setting IP Address, port, operation mode, type(s) of barcode to be read etc.

**Parameters**:
- **ipAddress**: The IP address of the Datalogic device you wish to connect to.
- **ipPort**: The port on the Datalogic device to which the connection should be established.
- **startCommandString**: The string command that triggers the device to start reading (set via DL.CODE).
- **stopCommandString (only for phase-mode)**: The string command that triggers the device to stop reading (set via DL.CODE).
- **timeoutMilliseconds**: Timeout in milliseconds waiting for a read response. Returns a TimeoutException if elapsed.

### Initialize the Service

```csharp
using DatalogicComHelper;
var service = new DatalogicService();
```

### Start Phase Mode

**Asynchronously**:
```csharp
string response = await service.StartPhaseModeAsync("192.168.1.100", 51236, "START", "STOP", 5000);
```

**Synchronously**:
```csharp
string response = service.StartPhaseMode("192.168.1.100", 51236 "START", "STOP", 5000);
```

### Start One-Shot Mode

**Asynchronously**:
```csharp
string response = await service.StartOneShotModeAsync("192.168.1.100", 51236, "START", 5000);
```

**Synchronously**:
```csharp
string response = service.StartOneShotMode("192.168.1.100", 51236, "START", 5000);
```

## Note: 

All methods send the start trigger and then wait for a response from the device undefinitely, there is no timeout. Timeout will be implemented in a future release.

## Error Handling

It's recommended to surround the method calls with try-catch blocks to handle any potential errors, especially when dealing with network operations:

```csharp
try
{
    string response = service.StartOneShotMode("192.168.1.100", 51236, "START", 5000);
    Console.WriteLine(response);
}
catch(Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## License

Distributed under the MIT License.