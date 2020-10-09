#define Camera

using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ASCOM.LxWebcam
{
    public partial class Camera
    {
        [ComVisible(false)]
        public class ComPort
        {
            public const string Idle       = "IDLE";
            public const string Exposure   = "EXPOSURE";
            public const string PulseGuide = "PULSEGUIDE";

            private SerialPort serialPort;

            public ComPort(string name)
            {
                this.Name = name;
                this.BaudRate = 9600;
                this.Parity = Parity.None;
                this.DataBits = 8;
                this.StopBits = StopBits.One;
                this.ReadTimeout = 1000;
                this.WriteTimeout = 1000;

                /////

                this.serialPort = null;
            }

            public string Name { get; set; }
            public int BaudRate { get; set; }
            public Parity Parity { get; set; }
            public int DataBits { get; set; }
            public StopBits StopBits { get; set; }
            public int ReadTimeout { get; set; }
            public int WriteTimeout { get; set; }

            public void Open()
            {
                lock (this)
                {
                    if (this.serialPort == null)
                    {
                        try
                        {
                            SerialPort serialPort = new SerialPort(this.Name, this.BaudRate, this.Parity, this.DataBits, this.StopBits)
                            {
                                ReadTimeout = this.ReadTimeout,
                                WriteTimeout = this.WriteTimeout
                            };

                            serialPort.Open(); this.serialPort = serialPort;

                            Response response = Response.ErrorUnknown;

                            for (int chances = 10; chances > 0; chances--)
                            {
                                response = this.Version(out string version);

                                if (response != Response.ErrorTimeout)
                                {
                                    break;
                                }
                            }

                            if (response != Response.OK)
                            {
                                throw new TimeoutException();
                            }
                        }
                        catch (Exception e)
                        {
                            if (this.serialPort != null)
                            {
                                try
                                {
                                    this.serialPort.Close();
                                }
                                catch
                                {

                                }
                                finally
                                {
                                    this.serialPort.Dispose();
                                    this.serialPort = null;
                                }
                            }

                            throw e;
                        }
                    }
                }
            }

            public void Close()
            {
                lock (this)
                {
                    if (this.serialPort != null)
                    {
                        try
                        {
                            this.serialPort.Close();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        finally
                        {
                            this.serialPort.Dispose();
                            this.serialPort = null;
                        }
                    }
                }
            }

            public Response Handshake(string command, string parameter = null)
            {
                string reply;

                if (!string.IsNullOrEmpty(parameter))
                {
                    command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", command, parameter);
                }

                lock (this)
                {
                    try
                    {
                        this.serialPort.WriteLine(command);

                        reply = this.serialPort.ReadLine();

                        if (string.IsNullOrEmpty(reply))
                        {
                            reply = string.Empty;
                        }
                        else
                        {
                            reply = reply.Trim();
                        }
                    }
                    catch (TimeoutException)
                    {
                        reply = "ERROR:TIMEOUT";
                    }
                    catch
                    {
                        reply = "ERROR:BROKEN";
                    }
                }

                Response response;

                switch (reply)
                {
                    case "OK":
                        response = Response.OK; break;
                    case "ERROR:COMMAND":
                        response = Response.ErrorCommand; break;
                    case "ERROR:PARAMETER":
                        response = Response.ErrorParameter; break;
                    case "ERROR:STATE":
                        response = Response.ErrorState; break;
                    case "ERROR:TIMEOUT":
                        response = Response.ErrorTimeout; break;
                    case "ERROR:BROKEN":
                        response = Response.ErrorBroken; break;
                    case "ERROR:UNKNOWN":
                    default:
                        response = Response.ErrorUnknown; break;
                }

                response.Command = command;
                response.Reply = reply;

                return response;
            }

            public Response Lx(out bool on)
            {
                on = false; Response response = this.Handshake("LX");

                if (response == Response.ErrorUnknown)
                {
                    if (response.Reply == "ON")
                    {
                        response.Assign(Response.OK); on = true;
                    }
                    else if (response.Reply == "OFF")
                    {
                        response.Assign(Response.OK); /*on = false;*/
                    }
                }

                return response;
            }

            public Response Lx(bool on)
            {
                return this.Handshake("LX", on ? "ON" : "OFF");
            }

            public Response Shutter(out bool open)
            {
                open = false; Response response = this.Handshake("SHUTTER");

                if (response == Response.ErrorUnknown)
                {
                    if (response.Reply == "OPEN")
                    {
                        response.Assign(Response.OK); open = true;
                    }
                    else if (response.Reply == "CLOSE")
                    {
                        response.Assign(Response.OK); /*open = false;*/
                    }
                }

                return response;
            }

            public Response Shutter(bool open)
            {
                return this.Handshake("SHUTTER", open ? "OPEN" : "CLOSE");
            }

            public Response Amp(out bool on)
            {
                on = false; Response response = this.Handshake("AMP");

                if (response == Response.ErrorUnknown)
                {
                    if (response.Reply == "ON")
                    {
                        response.Assign(Response.OK); on = true;
                    }
                    else if (response.Reply == "OFF")
                    {
                        response.Assign(Response.OK); /*on = false;*/
                    }
                }

                return response;
            }

            public Response Amp(bool on)
            {
                return this.Handshake("AMP", on ? "ON" : "OFF");
            }

            public Response Start(int duration)
            {
                return this.Handshake("START", duration.ToString());
            }

            public Response Stop()
            {
                return this.Handshake("STOP");
            }

            public Response East(int duration)
            {
                return this.Handshake("RA+", duration.ToString());
            }

            public Response North(int duration)
            {
                return this.Handshake("DEC+", duration.ToString());
            }

            public Response South(int duration)
            {
                return this.Handshake("DEC-", duration.ToString());
            }

            public Response West(int duration)
            {
                return this.Handshake("RA-", duration.ToString());
            }

            public Response Duration(string action, out int min, out int max)
            {
                min = max = 0; Response response = this.Handshake("DURATION", action);

                if (response == Response.ErrorUnknown)
                {
                    Match match = Regex.Match(response.Reply, "(?<Min>\\d+)\\s+(?<Max>\\d+)");

                    if (match.Success && int.TryParse(match.Groups["Min"].Value, out min) &&
                                         int.TryParse(match.Groups["Max"].Value, out max))
                    {
                        response.Assign(Response.OK);
                    }
                }

                return response;
            }

            public Response State(HashSet<string> states)
            {
                states.Clear(); Response response = this.Handshake("STATE");

                if (response == Response.ErrorUnknown)
                {
                    response.Assign(Response.OK);

                    foreach (string state in response.Reply.Split(' '))
                    {
                        states.Add(state);
                    }
                }

                return response;
            }

            public Response Version(out string version)
            {
                version = "?"; Response response = this.Handshake("VERSION");

                if (response == Response.ErrorUnknown)
                {
                    response.Assign(Response.OK); version = response.Reply;
                }

                return response;
            }

            [ComVisible(false)]
            public struct Response
            {
                public static readonly Response OK             = new Response("OK");
                public static readonly Response ErrorCommand   = new Response("ERROR:COMMAND");
                public static readonly Response ErrorParameter = new Response("ERROR:PARAMETER");
                public static readonly Response ErrorState     = new Response("ERROR:STATE");
                public static readonly Response ErrorTimeout   = new Response("ERROR:TIMEOUT");
                public static readonly Response ErrorBroken    = new Response("ERROR:BROKEN");
                public static readonly Response ErrorUnknown   = new Response("ERROR:UNKNOWN");

                private string response;

                public Response(string reponse)
                {
                    this.response = reponse;

                    this.Command = string.Empty;
                    this.Reply = string.Empty;
                }

                public void Assign(Response response)
                {
                    this.response = response.response;
                }

                public string Command { get; set; }
                public string Reply { get; set; }

                public override bool Equals(object obj)
                {
                    if (obj == null || !(obj is Response))
                    {
                        return false;
                    }

                    return this.response == ((Response)obj).response;
                }

                public override int GetHashCode()
                {
                    return this.response.GetHashCode();
                }
                public override string ToString()
                {
                    return string.Format(CultureInfo.InvariantCulture, "[{0}] [{1}]", this.Command, this.Reply);
                }

                public static bool operator ==(Response a, Response b)
                {
                    return a.Equals(b);
                }

                public static bool operator !=(Response a, Response b)
                {
                    return !a.Equals(b);
                }
            }
        }
    }
}
