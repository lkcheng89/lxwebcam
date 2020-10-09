using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.LxWebcam;
using DirectShowLib;
using System.Globalization;
using System.Reflection;
using System.IO.Ports;

namespace ASCOM.LxWebcam
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form, IComparer<SetupDialogForm.DeviceItem>
    {
        public class DeviceItem
        {
            public DeviceItem(string name, bool exists = true)
            {
                this.Name = name;
                this.Exists = exists;
            }

            public string Name { get; set; }
            public bool Exists { get; set; }

            public override string ToString()
            {
                return this.Exists ? this.Name : string.Format(CultureInfo.InvariantCulture, "{0} (?)", this.Name);
            }
        }

        public class MediaTypeItem
        {
            public MediaTypeItem(VideoInfoHeader mediaType)
            {
                this.MediaType = mediaType;
            }

            public VideoInfoHeader MediaType { get; set; }

            public override string ToString()
            {
                BitmapInfoHeader bmiHeader = this.MediaType.BmiHeader;

                char c1 = (char)((bmiHeader.Compression >>  0) & (0xFF));
                char c2 = (char)((bmiHeader.Compression >>  8) & (0xFF));
                char c3 = (char)((bmiHeader.Compression >> 16) & (0xFF));
                char c4 = (char)((bmiHeader.Compression >> 24) & (0xFF));

                double fps = 10000000.0 / this.MediaType.AvgTimePerFrame;

                return string.Format(CultureInfo.InvariantCulture, "{0} x {1}, {2} bits, {3}{4}{5}{6}, {7:0.00} fps", bmiHeader.Width, bmiHeader.Height, bmiHeader.BitCount, c1, c2, c3, c4, fps);
            }
        }

        public SetupDialogForm()
        {
            InitializeComponent();
        }

        public int Compare(DeviceItem x, DeviceItem y)
        {
            return string.Compare(x.Name, y.Name);
        }

        private void SetupDialogForm_Load(object sender, EventArgs e)
        {
            List<DeviceItem> items = new List<DeviceItem>(); bool found = false;

            foreach (DsDevice device in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                if (string.IsNullOrEmpty(device.Name))
                {
                    continue;
                }

                items.Add(new DeviceItem(device.Name));

                if (device.Name == Camera.webcamName)
                {
                    found = true;
                }
            }

            if (!found)
            {
                items.Add(new DeviceItem(Camera.webcamName, false));
            }

            items.Sort(this);

            this.comboBoxWebcam.Items.Clear();
            this.comboBoxWebcam.Items.AddRange(items.ToArray());

            for (int index = 0; index < items.Count; index++)
            {
                DeviceItem item = items[index];

                if (item.Name == Camera.webcamName)
                {
                    this.comboBoxWebcam.SelectedIndex = index;
                }
            }

            this.textBoxPixelSizeX.Text = Camera.pixelSizeX.ToString();
            this.textBoxPixelSizeY.Text = Camera.pixelSizeY.ToString();

            /////

            items.Clear(); found = false;

            foreach (string name in SerialPort.GetPortNames())
            {
                items.Add(new DeviceItem(name));

                if (name == Camera.comPortName)
                {
                    found = true;
                }
            }

            if (!found)
            {
                items.Add(new DeviceItem(Camera.comPortName, false));
            }

            items.Sort(this);

            this.comboBoxComPort.Items.Clear();
            this.comboBoxComPort.Items.AddRange(items.ToArray());

            for (int index = 0; index < items.Count; index++)
            {
                DeviceItem item = items[index];

                if (item.Name == Camera.comPortName)
                {
                    this.comboBoxComPort.SelectedIndex = index;
                }
            }

            Util util = new Util();

            if (util.ServicePack > 0)
            {
                this.labelPlatformVersion.Text = string.Format(CultureInfo.InvariantCulture, "Platform Version: {0}.{1} SP{2}", util.MajorVersion, util.MinorVersion, util.ServicePack);
            }
            else
            {
                this.labelPlatformVersion.Text = string.Format(CultureInfo.InvariantCulture, "Platform Version: {0}.{1}", util.MajorVersion, util.MinorVersion);
            }

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.labelDriverVersion.Text = string.Format(CultureInfo.InvariantCulture, "Driver Version: {0}.{1}", version.Major, version.Minor);

            this.checkBoxTraceLogger.Checked = Camera.traceLogger.Enabled;
        }

        private void comboBoxWebcam_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeviceItem webcamItem = this.comboBoxWebcam.SelectedItem as DeviceItem;

            if (webcamItem != null && webcamItem.Exists)
            {
                Camera.Webcam webcam = new Camera.Webcam(webcamItem.Name);
                List<VideoInfoHeader> mediaTypes;

                try
                {
                    webcam.Open(false); mediaTypes = webcam.GetMediaTypes();
                }
                catch
                {
                    mediaTypes = null;
                }
                    
                this.comboBoxMediaType.Items.Clear();

                if (mediaTypes != null && mediaTypes.Count > 0)
                {
                    int selectedIndex = -1;

                    for (int index = 0; index < mediaTypes.Count; index++)
                    {
                        MediaTypeItem item = new MediaTypeItem(mediaTypes[index]);

                        this.comboBoxMediaType.Items.Add(item);

                        if (selectedIndex < 0)
                        {
                            string x = Camera.Webcam.ToString(Camera.mediaType);
                            string y = Camera.Webcam.ToString(item.MediaType);

                            if (x == y)
                            {
                                selectedIndex = index;
                            }
                        }
                    }

                    if (selectedIndex < 0)
                    {
                        selectedIndex = 0;
                    }

                    this.comboBoxMediaType.SelectedIndex = selectedIndex;
                    this.comboBoxMediaType.Enabled = true;

                    this.textBoxPixelSizeX.Enabled = true;
                    this.textBoxPixelSizeY.Enabled = true;
                    this.labelPixelSizeBy.Enabled = true;
                    this.labelPixelSizeUm.Enabled = true;
                }
                else
                {
                    this.comboBoxMediaType.Enabled = false;

                    this.textBoxPixelSizeX.Enabled = false;
                    this.textBoxPixelSizeY.Enabled = false;
                    this.labelPixelSizeBy.Enabled = false;
                    this.labelPixelSizeUm.Enabled = false;
                }

                try
                {
                    webcam.Close();
                }
                catch
                {

                }
            }
            else
            {
                this.comboBoxMediaType.Items.Clear();
                this.comboBoxMediaType.Enabled = false;

                this.textBoxPixelSizeX.Enabled = false;
                this.textBoxPixelSizeY.Enabled = false;
                this.labelPixelSizeBy.Enabled = false;
                this.labelPixelSizeUm.Enabled = false;
            }
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            DeviceItem comPortItem = this.comboBoxComPort.SelectedItem as DeviceItem;

            if (comPortItem == null)
            {
                MessageBox.Show(this, "No Com Port is available", "Com Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!comPortItem.Exists)
            {
                MessageBox.Show(this, string.Format("Com Port ({0}) doesn't exist", comPortItem.Name), "Com Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Camera.ComPort comPort = new Camera.ComPort(comPortItem.Name)
            {
                BaudRate = Camera.baudRate,
                Parity = Camera.parity,
                DataBits = Camera.dataBits,
                StopBits = Camera.stopBits,
                ReadTimeout = Camera.readTimeout,
                WriteTimeout = Camera.writeTimeout,
            };

            try
            {
                comPort.Open();
            }
            catch
            {
                MessageBox.Show(this, string.Format("Com Port ({0}) doesn't exist", comPortItem.Name), "Com Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Camera.ComPort.Response response = comPort.Lx(out bool lx);
            bool lxOK = (response == Camera.ComPort.Response.OK);

            response = comPort.Shutter(out bool shutter);
            bool shutterOK = (response == Camera.ComPort.Response.OK);

            response = comPort.Amp(out bool amp);
            bool ampOK = (response == Camera.ComPort.Response.OK);

            response = comPort.Version(out string version);
            bool versionOK = (response == Camera.ComPort.Response.OK);

            response = comPort.Duration(Camera.ComPort.Exposure, out int exposureMin, out int exposureMax);
            bool exposureOK = (response == Camera.ComPort.Response.OK);

            response = comPort.Duration(Camera.ComPort.PulseGuide, out int pulseGuideMin, out int pulseGuideMax);
            bool pulseGuideOK = (response == Camera.ComPort.Response.OK);

            this.checkBoxLx.Enabled = lxOK;
            this.checkBoxLx.Checked = lx;
            this.checkBoxShutter.Enabled = shutterOK;
            this.checkBoxShutter.Checked = shutter;
            this.checkBoxAmp.Enabled = ampOK;
            this.checkBoxAmp.Checked = amp;

            this.textBoxExposureMin.Enabled = exposureOK;
            this.textBoxExposureMin.Text = exposureMin.ToString();
            this.textBoxExposureMax.Enabled = exposureOK;
            this.textBoxExposureMax.Text = exposureMax.ToString();
            this.labelExposureDash.Enabled = exposureOK;
            this.labelExposureMS.Enabled = exposureOK;

            this.textBoxPulseGuideMin.Enabled = pulseGuideOK;
            this.textBoxPulseGuideMin.Text = pulseGuideMin.ToString();
            this.textBoxPulseGuideMax.Enabled = pulseGuideOK;
            this.textBoxPulseGuideMax.Text = pulseGuideMax.ToString();
            this.labelPulseGuideDash.Enabled = exposureOK;
            this.labelPulseGuideMS.Enabled = exposureOK;

            if (versionOK)
            {
                this.labelFirmwareVersion.Text = string.Format(CultureInfo.InvariantCulture, "Firmware Version: {0}", version);
            }
            else
            {
                this.labelFirmwareVersion.Text = "Firmware Version: --";
            }

            try
            {
                comPort.Close();
            }
            catch
            {

            }
        }

        private void buttonOK_Click(object sender, EventArgs e) // OK button event handler
        {
            DeviceItem webcamItem = this.comboBoxWebcam.SelectedItem as DeviceItem;

            if (webcamItem != null)
            {
                Camera.webcamName = webcamItem.Name;
            }

            MediaTypeItem mediaTypeItem = this.comboBoxMediaType.SelectedItem as MediaTypeItem;

            if (mediaTypeItem != null)
            {
                Camera.mediaType = mediaTypeItem.MediaType;
            }

            Camera.pixelSizeX = double.Parse(this.textBoxPixelSizeX.Text);
            Camera.pixelSizeY = double.Parse(this.textBoxPixelSizeY.Text);

            webcamItem = this.comboBoxComPort.SelectedItem as DeviceItem;

            if (webcamItem != null)
            {
                Camera.comPortName = webcamItem.Name;
            }

            Camera.traceLogger.Enabled = checkBoxTraceLogger.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
    }
}