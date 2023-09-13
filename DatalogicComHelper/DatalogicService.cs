using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatalogicComHelper
{
    /// <summary>
    /// Represents a service for communicating with Datalogic devices.
    /// Provides synchronous and asynchronous methods to initiate 
    /// phase mode and one-shot mode and retrieve the responses.
    /// </summary>
    public class DatalogicService
    {

        /// <summary>
        /// Initiates the phase mode asynchronously and retrieves the response.
        /// </summary>
        /// <param name="ipAddress">The IP address to connect to.</param>
        /// <param name="ipPort">The port to connect to.</param>
        /// <param name="startCommandString">String that triggers the device to start reading.</param>
        /// <param name="stopCommandString">String that triggers the device to stop reading.</param>
        /// <param name="timeoutInMilliseconds">The duration in milliseconds to wait for a response before timing out.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response string.</returns>
        public async Task<string> StartPhaseModeAsync(string ipAddress, int ipPort, string startCommandString, string stopCommandString, int timeoutInMilliseconds)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, ipPort).ConfigureAwait(false);
                    var stream = client.GetStream();

                    var stopData = System.Text.Encoding.ASCII.GetBytes(stopCommandString);
                    var data = System.Text.Encoding.ASCII.GetBytes(startCommandString);

                    await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);

                    var bytes = new byte[client.ReceiveBufferSize];
                    int bytesRead;

                    var readTask = stream.ReadAsync(bytes, 0, client.ReceiveBufferSize);

                    var completedTask = await Task.WhenAny(readTask, Task.Delay(timeoutInMilliseconds)).ConfigureAwait(false);

                    if (completedTask == readTask)
                    {
                        bytesRead = await readTask;
                    }
                    else
                    {
                        await stream.WriteAsync(stopData, 0, stopData.Length).ConfigureAwait(false);
                        throw new TimeoutException("Reading from the stream has timed out.");
                    }

                    await stream.WriteAsync(stopData, 0, stopData.Length).ConfigureAwait(false);

                    return System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
                }
            }
            catch (SocketException se)
            {
                // It's good to maintain the original exception to preserve the stack trace and other details
                throw new Exception("TCP Socket Error", se);
            }
        }


        /// <summary>
        /// Initiates the phase mode synchronously and retrieves the response.
        /// </summary>
        /// <param name="ipAddress">The IP address to connect to.</param>
        /// <param name="ipPort">The port to connect to.</param>
        /// <param name="startCommandString">String that triggers the device to start reading.</param>
        /// <param name="stopCommandString">String that triggers the device to stop reading.</param>
        /// <param name="timeoutInMilliseconds">The duration in milliseconds to wait for a response before timing out.</param>
        /// <returns>The response string.</returns>
        public string StartPhaseMode(string ipAddress, int ipPort, string startCommandString, string stopCommandString, int timeoutInMilliseconds)
        {
            try
            {
                using (var client = new TcpClient(ipAddress, ipPort))
                {
                    var stream = client.GetStream();

                    var stopData = System.Text.Encoding.ASCII.GetBytes(stopCommandString);

                    var data = System.Text.Encoding.ASCII.GetBytes(startCommandString);
                    stream.Write(data, 0, data.Length);

                    var stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    var bytes = new byte[client.ReceiveBufferSize];
                    int bytesRead = 0;

                    while (stopwatch.ElapsedMilliseconds < timeoutInMilliseconds && bytesRead == 0)
                    {
                        if (stream.DataAvailable)
                        {
                            bytesRead = stream.Read(bytes, 0, client.ReceiveBufferSize);
                        }
                    }

                    stopwatch.Stop();

                    if (bytesRead == 0)
                    {
                        stream.Write(stopData, 0, stopData.Length);

                        throw new TimeoutException("Reading from the stream has timed out.");
                    }

                    var response = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

                    stream.Write(stopData, 0, stopData.Length);

                    return response;
                }
            }
            catch (SocketException se)
            {
                throw new Exception("TCP Socket Error: " + se.Message);
            }
        }

        /// <summary>
        /// Initiates the one-shot mode asynchronously and retrieves the response.
        /// </summary>
        /// <param name="ipAddress">The IP address to connect to.</param>
        /// <param name="ipPort">The port to connect to.</param>
        /// <param name="commandString">String that triggers the device to start reading.</param>
        /// <param name="timeoutInMilliseconds">The duration in milliseconds to wait for a response before timing out.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response string.</returns>
        public async Task<string> StartOneShotModeAsync(string ipAddress, int ipPort, string commandString, int timeoutInMilliseconds)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, ipPort).ConfigureAwait(false);
                    var stream = client.GetStream();

                    var data = System.Text.Encoding.ASCII.GetBytes(commandString);

                    await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);

                    var bytes = new byte[client.ReceiveBufferSize];
                    int bytesRead;

                    var readTask = stream.ReadAsync(bytes, 0, client.ReceiveBufferSize);

                    var completedTask = await Task.WhenAny(readTask, Task.Delay(timeoutInMilliseconds)).ConfigureAwait(false);

                    if (completedTask == readTask)
                    {
                        bytesRead = await readTask;
                    }
                    else
                    {
                        throw new TimeoutException("Reading from the stream has timed out.");
                    }

                    return System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
                }
            }
            catch (SocketException se)
            {
                throw new Exception("TCP Socket Error: " + se.Message);
            }
        }



        /// <summary>
        /// Initiates the one-shot mode synchronously and retrieves the response.
        /// </summary>
        /// <param name="ipAddress">The IP address to connect to.</param>
        /// <param name="ipPort">The port to connect to.</param>
        /// <param name="commandString">String that triggers the device to start reading.</param>
        /// <param name="timeoutInMilliseconds">The duration in milliseconds to wait for a response before timing out.</param> 
        /// <returns>The response string.</returns>
        public string StartOneShotMode(string ipAddress, int ipPort, string commandString, int timeoutInMilliseconds)
        {
            try
            {
                using (var client = new TcpClient(ipAddress, ipPort))
                {
                    var stream = client.GetStream();

                    var data = System.Text.Encoding.ASCII.GetBytes(commandString);
                    stream.Write(data, 0, data.Length);

                    var stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    var bytes = new byte[client.ReceiveBufferSize];
                    int bytesRead = 0;

                    while (stopwatch.ElapsedMilliseconds < timeoutInMilliseconds && bytesRead == 0)
                    {
                        if (stream.DataAvailable)
                        {
                            bytesRead = stream.Read(bytes, 0, client.ReceiveBufferSize);
                        }
                    }

                    stopwatch.Stop();

                    if (bytesRead == 0)
                    {
                        throw new TimeoutException("Reading from the stream has timed out.");
                    }

                    var response = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

                    return response;
                }
            }
            catch (SocketException se)
            {
                throw new Exception("TCP Socket Error: " + se.Message);
            }
        }

    }
}
