//tabs=4
// --------------------------------------------------------------------------------
// ASCOM Camera Driver for LxWebcam
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM Camera interface version: 2
// Author:		Lung-Kai Cheng <lkcheng89@gmail.com>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//

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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASCOM.LxWebcam
{
    //
    // Your driver's DeviceID is ASCOM.LxWebcam.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.LxWebcam.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _LxWebcam from being created and used as the [default] interface
    //
    // TODO Replace the not implemented exceptions with code to implement the function or
    // throw the appropriate ASCOM exception.
    //

    /// <summary>
    /// ASCOM Camera Driver for LxWebcam.
    /// </summary>
    [Guid("68f32d94-e64a-4523-8955-52892c80cc85")]
    [ClassInterface(ClassInterfaceType.None)]
    public partial class Camera : ICameraV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal static string driverID = "ASCOM.LxWebcam.Camera";

        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        internal static string driverDescription = "LxWebcam";

        internal static string webcamProfileName = "Webcam"; // Constants used for Profile persistence
        internal static string webcamDefault = "Philips SPC 900NC PC Camera";
        internal static string mediaTypeProfileName = "MediaType";
        internal static string mediaTypeDefault = "";
        internal static string pixelSizeXProfileName = "PixelSizeX";
        internal static double pixelSizeXDefault = 5.6;
        internal static string pixelSizeYProfileName = "PixelSizeY";
        internal static double pixelSizeYDefault = 5.6;

        internal static string comPortProfileName = "ComPort";
        internal static string comPortDefault = "COM1";
        internal static string baudRateProfileName = "BaudRate";
        internal static int baudRateDefault = 9600;
        internal static string parityProfileName = "Parity";
        internal static Parity parityDefault = Parity.None;
        internal static string dataBitsProfileName = "DataBits";
        internal static int dataBitsDefault = 8;
        internal static string stopBitsProfileName = "StopBits";
        internal static StopBits stopBitsDefault = StopBits.One;
        internal static string readTimeoutProfileName = "ReadTimeout";
        internal static int readTimeoutDefault = 1000;
        internal static string writeTimeoutProfileName = "WriteTimeout";
        internal static int writeTimeoutDefault = 1000;

        internal static string traceLoggerProfileName = "TraceLogger";
        internal static bool traceLoggerDefault = false;

        internal static string webcamName;  // Variables to hold the currrent device configuration
        internal static VideoInfoHeader mediaType;
        internal static double pixelSizeX;
        internal static double pixelSizeY;

        internal static string comPortName;
        internal static int baudRate;
        internal static Parity parity;
        internal static int dataBits;
        internal static StopBits stopBits;
        internal static int readTimeout;
        internal static int writeTimeout;

        internal static TraceLogger traceLogger;

        internal Webcam webcam; // Initialise variables to hold values required for functionality tested by Conform
        internal ComPort comPort;

        internal int cameraXSize;
        internal int cameraYSize;
        internal int numX;
        internal int numY;
        internal int startX;
        internal int startY;
        internal double exposureMin;
        internal double exposureMax;
        internal DateTime exposureStart;
        internal DateTime exposureAbort;
        internal double exposureDuration;
        internal bool imageReady;
        internal int[,,] imageArray;
        internal CameraStates cameraState;
        internal int pulseGuideMin;
        internal int pulseGuideMax;
        internal Semaphore semaphore;
        internal bool threadRunning;
        internal Thread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="LxWebcam"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Camera()
        {
            traceLogger = new TraceLogger("", "LxWebcam");
            ReadProfile(); // Read device configuration from the ASCOM Profile store

            traceLogger.LogMessage("Camera", "Starting initialisation");

            webcam = null;
            comPort = null;

            cameraXSize = 0;
            cameraYSize = 0;
            numX = 0;
            numY = 0;
            startX = 0;
            startY = 0;
            exposureMin = double.NaN;
            exposureMax = double.NaN;
            exposureStart = DateTime.MinValue;
            exposureAbort = DateTime.MinValue;
            exposureDuration = double.NaN;
            imageReady = false;
            imageArray = null;
            cameraState = CameraStates.cameraIdle;
            pulseGuideMin = 0;
            pulseGuideMax = 0;
            semaphore = null;
            threadRunning = false;
            thread = null;

            traceLogger.LogMessage("Camera", "Completed initialisation");
        }

        //
        // PUBLIC COM INTERFACE ICameraV2 IMPLEMENTATION
        //

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public void SetupDialog()
        {
            if (this.webcam != null && this.comPort != null)
            {
                System.Windows.Forms.MessageBox.Show("Already connected, just press OK");
            }

            using (SetupDialogForm F = new SetupDialogForm())
            {
                var result = F.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    WriteProfile(); // Persist device configuration values to the ASCOM Profile store
                }
            }
        }

        public ArrayList SupportedActions
        {
            get
            {
                LogMessage("SupportedActions_get", "[ ]");
                return new ArrayList();
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            LogMessage("Action", "ActionNotImplementedException");
            throw new ASCOM.ActionNotImplementedException(actionName);
        }

        public void CommandBlind(string command, bool raw)
        {
            LogMessage("CommandBlind", "MethodNotImplementedException");
            throw new ASCOM.MethodNotImplementedException("CommandBlind");
        }

        public bool CommandBool(string command, bool raw)
        {
            LogMessage("CommandBool", "MethodNotImplementedException");
            throw new ASCOM.MethodNotImplementedException("CommandBool");
        }

        public string CommandString(string command, bool raw)
        {
            LogMessage("CommandString", "MethodNotImplementedException");
            throw new ASCOM.MethodNotImplementedException("CommandString");
        }

        public void Dispose()
        {
            traceLogger.Enabled = false;
            traceLogger.Dispose();
            traceLogger = null;
        }

        public bool Connected
        {
            get
            {
                bool connected = (this.webcam != null && this.comPort != null);
                LogMessage("Connected_get", connected.ToString(CultureInfo.InvariantCulture));
                return connected;
            }
            set
            {
                LogMessage("Connected_set", value.ToString(CultureInfo.InvariantCulture));

                if (value)
                {
                    bool connected = false;

                    do
                    {
                        if (this.webcam != null && this.comPort != null)
                        {
                            connected = true; break;
                        }

                        this.webcam = new Webcam(webcamName)
                        {
                            MediaType = mediaType,
                        };

                        try
                        {
                            this.webcam.Open();
                        }
                        catch (COMException e)
                        {
                            LogMessage("Connected_set", "[{0}] [{1}:0x{2:X08}][{3}]", webcamName, e.GetType(), e.ErrorCode, e.Message);
                            break;
                        }
                        catch (Exception e)
                        {
                            LogMessage("Connected_set", "[{0}] [{1}][{2}]", webcamName, e.GetType(), e.Message);
                            break;
                        }

                        this.cameraXSize = this.webcam.Width;
                        this.cameraYSize = this.webcam.Height;
                        this.numX = this.webcam.Width;
                        this.numY = this.webcam.Height;

                        this.comPort = new ComPort(comPortName)
                        {
                            BaudRate = baudRate,
                            Parity = parity,
                            DataBits = dataBits,
                            StopBits = stopBits,
                            ReadTimeout = readTimeout,
                            WriteTimeout = writeTimeout,
                        };

                        try
                        {
                            this.comPort.Open();
                        }
                        catch (Exception e)
                        {
                            LogMessage("Connected_set", "[{0}] [{1}][{2}]", comPortName, e.GetType(), e.Message);
                            break;
                        }

                        ComPort.Response response = this.comPort.Lx(true);
                        LogMessage("Connected_set", response.ToString());

                        if (response != ComPort.Response.OK)
                        {
                            break;
                        }

                        response = this.comPort.Amp(false);
                        LogMessage("Connected_set", response.ToString());

                        if (response != ComPort.Response.OK)
                        {
                            break;
                        }

                        response = this.comPort.Duration(ComPort.Exposure, out int min, out int max);
                        LogMessage("Connected_set", response.ToString());

                        if (response != ComPort.Response.OK)
                        {
                            break;
                        }

                        this.exposureMin = min / 1000.0;
                        this.exposureMax = max / 1000.0;

                        response = comPort.Duration(ComPort.PulseGuide, out pulseGuideMin, out pulseGuideMax);
                        LogMessage("Connected_set", response.ToString());

                        if (response != ComPort.Response.OK)
                        {
                            break;
                        }

                        this.semaphore = new Semaphore(0, 1);
                        this.threadRunning = true;
                        this.thread = new Thread(ExposureThread);
                        this.thread.Start();

                        connected = true;
                    }
                    while (false);

                    if (!connected)
                    {
                        if (this.webcam != null)
                        {
                            try
                            {
                                this.webcam.Close();
                            }
                            catch
                            {

                            }
                            finally
                            {
                                this.webcam = null;
                            }
                        }

                        if (this.comPort != null)
                        {
                            ComPort.Response response = comPort.Lx(false);
                            LogMessage("Connected_set", response.ToString());

                            response = comPort.Amp(true);
                            LogMessage("Connected_set", response.ToString());

                            try
                            {
                                this.comPort.Close();
                            }
                            catch
                            {

                            }
                            finally
                            {
                                this.comPort = null;
                            }
                        }

                        this.semaphore = null;
                        this.threadRunning = false;
                        this.thread = null;
                    }
                }
                else
                {
                    this.threadRunning = false;

                    if (this.thread != null)
                    {
                        if (this.semaphore != null)
                        {
                            try
                            {
                                this.semaphore.Release();
                            }
                            catch
                            {

                            }
                            finally
                            {
                                this.semaphore = null;
                            }
                        }

                        try
                        {
                            this.thread.Join();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            this.thread = null;
                        }
                    }
                    else if (this.semaphore != null)
                    {
                        this.semaphore = null;
                    }

                    if (this.webcam != null)
                    {
                        try
                        {
                            this.webcam.Close();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            this.webcam = null;
                        }
                    }

                    if (this.comPort != null)
                    {
                        ComPort.Response response = comPort.Lx(false);
                        LogMessage("Connected_set", response.ToString());

                        response = comPort.Amp(true);
                        LogMessage("Connected_set", response.ToString());

                        try
                        {
                            this.comPort.Close();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            this.comPort = null;
                        }
                    }
                }
            }
        }

        public string Description
        {
            get
            {
                LogMessage("Description_get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverInfo = "LxWebcam Driver v" + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                LogMessage("DriverInfo_get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                LogMessage("DriverVersion_get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            get
            {
                LogMessage("InterfaceVersion_get", "2");
                return Convert.ToInt16("2");
            }
        }

        public string Name
        {
            get
            {
                string name = "LxWebcam";
                LogMessage("Name_get", name);
                return name;
            }
        }

        #endregion

        #region ICamera Implementation

        private void ExposureThread()
        {
            while (true)
            {
                bool success;

                try
                {
                    success = this.semaphore.WaitOne();
                }
                catch
                {
                    success = false;
                }

                if (!success || !this.threadRunning)
                {
                    cameraState = CameraStates.cameraIdle; break;
                }

                int startX = this.startX;
                int startY = this.startY;
                int numX = this.numX;
                int numY = this.numY;

                HashSet<string> states = new HashSet<string>();

                while (this.threadRunning)
                {
                    ComPort.Response response = this.comPort.State(states);
                    LogMessage("ExposureThread", response.ToString());

                    if (response != ComPort.Response.OK)
                    {
                        success = false; break;
                    }

                    if (!states.Contains(ComPort.Exposure))
                    {
                        break;
                    }

                    cameraState = CameraStates.cameraExposing; Thread.Sleep(1);
                }

                if (!this.threadRunning)
                {
                    this.cameraState = CameraStates.cameraIdle; break;
                }

                if (success && this.exposureStart > this.exposureAbort)
                {
                    this.cameraState = CameraStates.cameraReading;
                    this.imageReady = this.webcam.Capture(out this.imageArray, startX, startY, numX, numY);
                }

                this.cameraState = CameraStates.cameraIdle;
            }
        }

        public void AbortExposure()
        {
            this.CheckConnected("AbortExposure");

            if (this.cameraState != CameraStates.cameraIdle)
            {
                this.exposureAbort = DateTime.Now;

                ComPort.Response response = this.comPort.Stop();
                LogMessage("AbortExposure", response.ToString());

                if (response.Reply != "OK")
                {
                    throw new InvalidOperationException("AbortExposure");
                }
            }
        }

        public short BayerOffsetX
        {
            get
            {
                short bayerOffsetX = 0;
                LogMessage("BayerOffsetX_get", bayerOffsetX.ToString(CultureInfo.InvariantCulture));
                return bayerOffsetX;
            }
        }

        public short BayerOffsetY
        {
            get
            {
                short bayerOffsetY = 0;
                LogMessage("BayerOffsetY_get", bayerOffsetY.ToString(CultureInfo.InvariantCulture));
                return bayerOffsetY;
            }
        }

        public short BinX
        {
            get
            {
                short binX = 1;
                LogMessage("BinX_get", binX.ToString(CultureInfo.InvariantCulture));
                return binX;
            }
            set
            {
                LogMessage("BinX_set", value.ToString(CultureInfo.InvariantCulture));
                if (value != 1) throw new ASCOM.InvalidValueException("BinX", value.ToString(CultureInfo.InvariantCulture), "1");
            }
        }

        public short BinY
        {
            get
            {
                short binY = 1;
                LogMessage("BinY_get", binY.ToString(CultureInfo.InvariantCulture));
                return binY;
            }
            set
            {
                LogMessage("BinY_set", value.ToString(CultureInfo.InvariantCulture));
                if (value != 1) throw new ASCOM.InvalidValueException("BinY", value.ToString(CultureInfo.InvariantCulture), "1");
            }
        }

        public double CCDTemperature
        {
            get
            {
                LogMessage("CCDTemperature_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("CCDTemperature", false);
            }
        }

        public CameraStates CameraState
        {
            get
            {
                this.CheckConnected("CameraState_get");

                LogMessage("CameraState_get", this.cameraState.ToString());
                return this.cameraState;
            }
        }

        public int CameraXSize
        {
            get
            {
                this.CheckConnected("CameraXSize_get");

                LogMessage("CameraXSize_get", this.cameraXSize.ToString(CultureInfo.InvariantCulture));
                return this.cameraXSize;
            }
        }

        public int CameraYSize
        {
            get
            {
                this.CheckConnected("CameraYSize_get");

                LogMessage("CameraYSize_get", this.cameraYSize.ToString(CultureInfo.InvariantCulture));
                return this.cameraYSize;
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                LogMessage("CanAbortExposure_get", true.ToString(CultureInfo.InvariantCulture));
                return true;
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                LogMessage("CanAsymmetricBin_get", false.ToString(CultureInfo.InvariantCulture));
                return false;
            }
        }

        public bool CanFastReadout
        {
            get
            {
                LogMessage("CanFastReadout_get", false.ToString(CultureInfo.InvariantCulture));
                return false;
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                LogMessage("CanGetCoolerPower_get", false.ToString(CultureInfo.InvariantCulture));
                return false;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                LogMessage("CanPulseGuide_get", true.ToString(CultureInfo.InvariantCulture));
                return true;
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                LogMessage("CanSetCCDTemperature_get", false.ToString(CultureInfo.InvariantCulture));
                return false;
            }
        }

        public bool CanStopExposure
        {
            get
            {
                LogMessage("CanStopExposure_get", true.ToString(CultureInfo.InvariantCulture));
                return true;
            }
        }

        public bool CoolerOn
        {
            get
            {
                LogMessage("CoolerOn_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", false);
            }
            set
            {
                LogMessage("CoolerOn_set", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", true);
            }
        }

        public double CoolerPower
        {
            get
            {
                LogMessage("CoolerPower_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("CoolerPower", false);
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                LogMessage("ElectronsPerADU_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("ElectronsPerADU", false);
            }
        }

        public double ExposureMax
        {
            get
            {
                this.CheckConnected("ExposureMax_get");

                LogMessage("ExposureMax_get", this.exposureMax.ToString(CultureInfo.InvariantCulture));
                return this.exposureMax;
            }
        }

        public double ExposureMin
        {
            get
            {
                this.CheckConnected("ExposureMin_get");

                LogMessage("ExposureMin_get", this.exposureMin.ToString(CultureInfo.InvariantCulture));
                return this.exposureMin;
            }
        }

        public double ExposureResolution
        {
            get
            {
                this.CheckConnected("ExposureResolution_get");

                double resolution = 0.001;
                LogMessage("ExposureResolution_get", resolution.ToString(CultureInfo.InvariantCulture));
                return resolution;
            }
        }

        public bool FastReadout
        {
            get
            {
                LogMessage("FastReadout_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", false);
            }
            set
            {
                LogMessage("FastReadout_set", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", true);
            }
        }

        public double FullWellCapacity
        {
            get
            {
                LogMessage("FullWellCapacity_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("FullWellCapacity", false);
            }
        }

        public short Gain
        {
            get
            {
                LogMessage("Gain_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("Gain", false);
            }
            set
            {
                LogMessage("Gain_set", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("Gain", true);
            }
        }

        public short GainMax
        {
            get
            {
                LogMessage("GainMax_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("GainMax", false);
            }
        }

        public short GainMin
        {
            get
            {
                LogMessage("GainMin_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("GainMin", true);
            }
        }

        public ArrayList Gains
        {
            get
            {
                LogMessage("Gains_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("Gains", true);
            }
        }

        public bool HasShutter
        {
            get
            {
                LogMessage("HasShutter_get", false.ToString(CultureInfo.InvariantCulture));
                return false;
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                LogMessage("HeatSinkTemperature_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("HeatSinkTemperature", false);
            }
        }

        public object ImageArray
        {
            get
            {
                CheckConnected("ImageArray_get");

                if (!this.imageReady || this.imageArray == null)
                {
                    LogMessage("ImageArray_get", "InvalidOperationException");
                    throw new ASCOM.InvalidOperationException();
                }

                return this.imageArray;
            }
        }

        public object ImageArrayVariant
        {
            get
            {
                CheckConnected("ImageArrayVariant_get");

                if (!this.imageReady || this.imageArray == null)
                {
                    LogMessage("ImageArrayVariant_get", "InvalidOperationException");
                    throw new ASCOM.InvalidOperationException();
                }

                object[,,] imageArrayVariant = new object[this.numX, this.numY, 3];

                for (int x = 0; x < this.numX; x++)
                {
                    for (int y = 0; y < this.numY; y++)
                    {
                        imageArrayVariant[x, y, 0] = this.imageArray[x, y, 0];
                        imageArrayVariant[x, y, 1] = this.imageArray[x, y, 1];
                        imageArrayVariant[x, y, 2] = this.imageArray[x, y, 2];
                    }
                }

                return imageArrayVariant;
            }
        }

        public bool ImageReady
        {
            get
            {
                CheckConnected("ImageReady_get");

                LogMessage("ImageReady_get", this.imageReady.ToString(CultureInfo.InvariantCulture));
                return imageReady;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                CheckConnected("IsPulseGuiding_get");

                HashSet<string> states = new HashSet<string>();
                ComPort.Response response = this.comPort.State(states);
                LogMessage("IsPulseGuiding_get", response.ToString());

                if (response != ComPort.Response.OK)
                {
                    throw new InvalidOperationException("IsPulseGuiding_get");
                }

                return states.Contains(ComPort.PulseGuide);
            }
        }

        public double LastExposureDuration
        {
            get
            {
                if (!this.imageReady)
                {
                    LogMessage("LastExposureDuration_get", "InvalidOperationException");
                    throw new ASCOM.InvalidOperationException("LastExposureStartTime_get");
                }

                LogMessage("LastExposureDuration_get", this.exposureDuration.ToString(CultureInfo.InvariantCulture));
                return this.exposureDuration;
            }
        }

        public string LastExposureStartTime
        {
            get
            {
                if (!this.imageReady)
                {
                    LogMessage("LastExposureStartTime_get", "InvalidOperationException");
                    throw new ASCOM.InvalidOperationException("LastExposureStartTime_get");
                }

                string exposureStart = this.exposureStart.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                LogMessage("LastExposureStartTime_get", exposureStart);
                return exposureStart;
            }
        }

        public int MaxADU
        {
            get
            {
                int maxADU = 255;
                LogMessage("MaxADU_get", maxADU.ToString(CultureInfo.InvariantCulture));
                return maxADU;
            }
        }

        public short MaxBinX
        {
            get
            {
                short maxBinX = 1;
                LogMessage("MaxBinX_get", maxBinX.ToString(CultureInfo.InvariantCulture));
                return maxBinX;
            }
        }

        public short MaxBinY
        {
            get
            {
                short maxBinY = 1;
                LogMessage("MaxBinY_get", maxBinY.ToString(CultureInfo.InvariantCulture));
                return maxBinY;
            }
        }

        public int NumX
        {
            get
            {
                LogMessage("NumX_get", this.numX.ToString(CultureInfo.InvariantCulture));
                return this.numX;
            }
            set
            {
                this.numX = value;
                LogMessage("NumX_set", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public int NumY
        {
            get
            {
                LogMessage("NumY_get", this.numY.ToString(CultureInfo.InvariantCulture));
                return this.numY;
            }
            set
            {
                this.numY = value;
                LogMessage("NumY_set", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public short PercentCompleted
        {
            get
            {
                if (this.exposureStart == DateTime.MinValue || double.IsNaN(this.exposureDuration))
                {
                    return 0;
                }

                short percentCompleted = (short)(Math.Min(Math.Max((DateTime.Now - this.exposureStart).TotalSeconds / this.exposureDuration, 0), 1) * 100);
                LogMessage("PercentCompleted_get", percentCompleted.ToString(CultureInfo.InvariantCulture));
                return percentCompleted;
            }
        }

        public double PixelSizeX
        {
            get
            {
                LogMessage("PixelSizeX_get", pixelSizeX.ToString(CultureInfo.InvariantCulture));
                return pixelSizeX;
            }
        }

        public double PixelSizeY
        {
            get
            {
                LogMessage("PixelSizeY_get", pixelSizeY.ToString(CultureInfo.InvariantCulture));
                return pixelSizeY;
            }
        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            this.CheckConnected("PulseGuide");

            if (Duration < this.pulseGuideMin || this.pulseGuideMax < Duration)
            {
                throw new InvalidValueException("PulseGuide", Duration.ToString(CultureInfo.InvariantCulture), pulseGuideMin.ToString(CultureInfo.InvariantCulture), pulseGuideMax.ToString(CultureInfo.InvariantCulture));
            }

            ComPort.Response response;

            switch (Direction)
            {
                case GuideDirections.guideEast:
                    response = this.comPort.East(Duration); break;
                case GuideDirections.guideNorth:
                    response = this.comPort.North(Duration); break;
                case GuideDirections.guideSouth:
                    response = this.comPort.South(Duration); break;
                case GuideDirections.guideWest:
                    response = this.comPort.West(Duration); break;
                default:
                    throw new InvalidValueException("PulseGuide", Direction.ToString(), "guideEast,guideNorth,guideSouth,guideWest");
            }

            LogMessage("PulseGuide", response.ToString());

            if (response != ComPort.Response.OK)
            {
                throw new InvalidOperationException("PulseGuide");
            }

            HashSet<string> states = new HashSet<string>();

            while (true)
            {
                response = this.comPort.State(states);
                LogMessage("PulseGuide", response.ToString());

                if (response != ComPort.Response.OK)
                {
                    throw new InvalidOperationException("PulseGuide");
                }

                if (!states.Contains(ComPort.PulseGuide))
                {
                    break;
                }

                Thread.Sleep(1);
            }
        }

        public short ReadoutMode
        {
            get
            {
                short readoutMode = 0;
                LogMessage("ReadoutMode_get", readoutMode.ToString(CultureInfo.InvariantCulture));
                return readoutMode;
            }
            set
            {
                LogMessage("ReadoutMode_set", value.ToString(CultureInfo.InvariantCulture));
                if (value != 0) throw new ASCOM.InvalidValueException("ReadoutMode_set", value.ToString(CultureInfo.InvariantCulture), "0");
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                ArrayList readoutModes = new ArrayList() { "Normal" };
                LogMessage("ReadoutModes_get", "[ Normal ]");
                return readoutModes;
            }
        }

        public string SensorName
        {
            get
            {
                string sensorName = "ICX098BQ";
                LogMessage("SensorName_get", sensorName);
                return sensorName;
            }
        }

        public SensorType SensorType
        {
            get
            {
                SensorType sensorType = SensorType.Color;
                LogMessage("SensorType_get", sensorType.ToString());
                return sensorType;
            }
        }

        public double SetCCDTemperature
        {
            get
            {
                LogMessage("SetCCDTemperature_get", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", false);
            }
            set
            {
                LogMessage("SetCCDTemperature_set", "PropertyNotImplementedException");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", true);
            }
        }

        public void StartExposure(double Duration, bool Light)
        {
            this.CheckConnected("StartExposure");

            if (Duration < this.exposureMin || this.exposureMax < Duration)
            {
                if (Duration != 0)
                {
                    throw new InvalidValueException("StartExposure", Duration.ToString(CultureInfo.InvariantCulture), exposureMin.ToString(CultureInfo.InvariantCulture), exposureMax.ToString(CultureInfo.InvariantCulture));
                }

                Duration = exposureMin;
            }

            if (this.startX < 0 || this.cameraXSize <= this.startX)
            {
                throw new InvalidValueException("StartExposure", this.startX.ToString(CultureInfo.InvariantCulture), "0", (this.cameraXSize - 1).ToString(CultureInfo.InvariantCulture));
            }

            if (this.startY < 0 || this.cameraYSize <= this.startY)
            {
                throw new InvalidValueException("StartExposure", this.startY.ToString(CultureInfo.InvariantCulture), "0", (this.cameraYSize - 1).ToString(CultureInfo.InvariantCulture));
            }

            if (this.numX < 1 || this.cameraXSize - this.startX < this.numX)
            {
                throw new InvalidValueException("StartExposure", this.numX.ToString(CultureInfo.InvariantCulture), "1", (this.cameraXSize - this.startX).ToString(CultureInfo.InvariantCulture));
            }

            if (this.numY < 1 || this.cameraYSize - this.startY < this.numY)
            {
                throw new InvalidValueException("StartExposure", this.numY.ToString(CultureInfo.InvariantCulture), "1", (this.cameraYSize - this.startY).ToString(CultureInfo.InvariantCulture));
            }

            ComPort.Response response = this.comPort.Start((int)(Duration * 1000 + 0.5));
            LogMessage("StartExposure", response.ToString());

            if (response.Reply != "OK")
            {
                throw new InvalidOperationException("StartExposure");
            }

            this.exposureDuration = Duration;
            this.exposureStart = DateTime.Now;

            this.imageReady = false;
            this.imageArray = null;
            this.cameraState = CameraStates.cameraWaiting;

            try
            {
                this.semaphore.Release();
            }
            catch
            {

            }
        }

        public int StartX
        {
            get
            {
                LogMessage("StartX_get", this.startX.ToString(CultureInfo.InvariantCulture));
                return this.startX;
            }
            set
            {
                this.startX = value;
                LogMessage("StartX_set", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public int StartY
        {
            get
            {
                LogMessage("StartY_get", this.startY.ToString(CultureInfo.InvariantCulture));
                return this.startY;
            }
            set
            {
                this.startY = value;
                LogMessage("StartY_set", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void StopExposure()
        {
            this.CheckConnected("StopExposure");

            if (this.cameraState != CameraStates.cameraIdle)
            {
                ComPort.Response response = this.comPort.Stop();
                LogMessage("StopExposure", response.ToString());

                if (response.Reply != "OK")
                {
                    throw new InvalidOperationException("StopExposure");
                }
            }
        }

        #endregion

        #region Private properties and methods
        // here are some useful properties and methods that can be used as required
        // to help with driver development

        #region ASCOM Registration

        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        /// <summary>
        /// Register or unregister the driver with the ASCOM Platform.
        /// This is harmless if the driver is already registered/unregistered.
        /// </summary>
        /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
        private static void RegUnregASCOM(bool bRegister)
        {
            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "Camera";
                if (bRegister)
                {
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
                }
            }
        }

        /// <summary>
        /// This function registers the driver with the ASCOM Chooser and
        /// is called automatically whenever this class is registered for COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is successfully built.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
        /// </remarks>
        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        /// <summary>
        /// This function unregisters the driver from the ASCOM Chooser and
        /// is called automatically whenever this class is unregistered from COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is cleaned or prior to rebuilding.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
        /// </remarks>
        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }

        #endregion

        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        private void CheckConnected(string message)
        {
            if (this.webcam == null || this.comPort == null)
            {
                throw new ASCOM.NotConnectedException(message);
            }
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        private void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Camera";

                webcamName = driverProfile.GetValue(driverID, webcamProfileName, string.Empty, webcamDefault);
                mediaType = Webcam.Parse(driverProfile.GetValue(driverID, mediaTypeProfileName, string.Empty, mediaTypeDefault));

                if (!double.TryParse(driverProfile.GetValue(driverID, pixelSizeXProfileName, string.Empty, string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out pixelSizeX))
                {
                    pixelSizeX = pixelSizeXDefault;
                }

                if (!double.TryParse(driverProfile.GetValue(driverID, pixelSizeYProfileName, string.Empty, string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out pixelSizeY))
                {
                    pixelSizeY = pixelSizeYDefault;
                }

                comPortName = driverProfile.GetValue(driverID, comPortProfileName, string.Empty, comPortDefault);

                if (!int.TryParse(driverProfile.GetValue(driverID, baudRateProfileName, string.Empty, string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out baudRate))
                {
                    baudRate = baudRateDefault;
                }

                if (!Enum.TryParse(driverProfile.GetValue(driverID, parityProfileName, string.Empty, string.Empty), out parity))
                {
                    parity = parityDefault;
                }

                if (!int.TryParse(driverProfile.GetValue(driverID, dataBitsProfileName, string.Empty, string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out dataBits))
                {
                    dataBits = dataBitsDefault;
                }

                if (!Enum.TryParse(driverProfile.GetValue(driverID, stopBitsProfileName, string.Empty, string.Empty), out stopBits))
                {
                    stopBits = stopBitsDefault;
                }

                if (!int.TryParse(driverProfile.GetValue(driverID, readTimeoutProfileName, string.Empty, string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out readTimeout))
                {
                    readTimeout = readTimeoutDefault;
                }

                if (!int.TryParse(driverProfile.GetValue(driverID, writeTimeoutProfileName, string.Empty, string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out writeTimeout))
                {
                    writeTimeout = writeTimeoutDefault;
                }

                if (!bool.TryParse(driverProfile.GetValue(driverID, traceLoggerProfileName, string.Empty, string.Empty), out bool traceLoggerEnabled))
                {
                    traceLoggerEnabled = traceLoggerDefault;
                }

                traceLogger.Enabled = traceLoggerEnabled;
            }
        }

        /// <summary>
        /// Write the device configuration to the  ASCOM  Profile store
        /// </summary>
        private void WriteProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Camera";

                driverProfile.WriteValue(driverID, webcamProfileName, webcamName);
                driverProfile.WriteValue(driverID, mediaTypeProfileName, Webcam.ToString(mediaType));
                driverProfile.WriteValue(driverID, pixelSizeXProfileName, pixelSizeX.ToString(CultureInfo.InvariantCulture));
                driverProfile.WriteValue(driverID, pixelSizeYProfileName, pixelSizeY.ToString(CultureInfo.InvariantCulture));

                driverProfile.WriteValue(driverID, comPortProfileName, comPortName);
                driverProfile.WriteValue(driverID, baudRateProfileName, baudRate.ToString(CultureInfo.InvariantCulture));
                driverProfile.WriteValue(driverID, parityProfileName, parity.ToString());
                driverProfile.WriteValue(driverID, dataBitsProfileName, dataBits.ToString(CultureInfo.InvariantCulture));
                driverProfile.WriteValue(driverID, stopBitsProfileName, stopBits.ToString());
                driverProfile.WriteValue(driverID, readTimeoutProfileName, readTimeout.ToString(CultureInfo.InvariantCulture));
                driverProfile.WriteValue(driverID, writeTimeoutProfileName, writeTimeout.ToString(CultureInfo.InvariantCulture));

                driverProfile.WriteValue(driverID, traceLoggerProfileName, traceLogger.Enabled.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Log helper function that takes formatted strings and arguments
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        private static void LogMessage(string identifier, string message, params object[] args)
        {
            string msg = string.Format(CultureInfo.InvariantCulture, message, args);
            traceLogger.LogMessage(identifier, msg);
        }
        #endregion
    }
}
