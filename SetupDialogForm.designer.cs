namespace ASCOM.LxWebcam
{
    partial class SetupDialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.labelComPort = new System.Windows.Forms.Label();
            this.checkBoxTraceLogger = new System.Windows.Forms.CheckBox();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.labelWebcam = new System.Windows.Forms.Label();
            this.comboBoxWebcam = new System.Windows.Forms.ComboBox();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.groupBoxWebcam = new System.Windows.Forms.GroupBox();
            this.labelPixelSize = new System.Windows.Forms.Label();
            this.labelMediaType = new System.Windows.Forms.Label();
            this.comboBoxMediaType = new System.Windows.Forms.ComboBox();
            this.textBoxPixelSizeY = new System.Windows.Forms.TextBox();
            this.textBoxPixelSizeX = new System.Windows.Forms.TextBox();
            this.labelPixelSizeUm = new System.Windows.Forms.Label();
            this.labelPixelSizeBy = new System.Windows.Forms.Label();
            this.groupBoxComPort = new System.Windows.Forms.GroupBox();
            this.textBoxPulseGuideMax = new System.Windows.Forms.TextBox();
            this.textBoxExposureMax = new System.Windows.Forms.TextBox();
            this.textBoxPulseGuideMin = new System.Windows.Forms.TextBox();
            this.textBoxExposureMin = new System.Windows.Forms.TextBox();
            this.labelPulseGuide = new System.Windows.Forms.Label();
            this.labelPulseGuideDash = new System.Windows.Forms.Label();
            this.labelPulseGuideMS = new System.Windows.Forms.Label();
            this.labelExposureMS = new System.Windows.Forms.Label();
            this.labelExposureDash = new System.Windows.Forms.Label();
            this.labelExposure = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.checkBoxAmp = new System.Windows.Forms.CheckBox();
            this.checkBoxShutter = new System.Windows.Forms.CheckBox();
            this.checkBoxLx = new System.Windows.Forms.CheckBox();
            this.labelPlatformVersion = new System.Windows.Forms.Label();
            this.labelDriverVersion = new System.Windows.Forms.Label();
            this.labelFirmwareVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.groupBoxWebcam.SuspendLayout();
            this.groupBoxComPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(350, 274);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(59, 26);
            this.buttonOK.TabIndex = 32;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(350, 306);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(59, 27);
            this.buttonCancel.TabIndex = 33;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // picASCOM
            // 
            this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.LxWebcam.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(12, 274);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.Size = new System.Drawing.Size(48, 56);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picASCOM.TabIndex = 3;
            this.picASCOM.TabStop = false;
            this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
            this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
            // 
            // labelComPort
            // 
            this.labelComPort.AutoSize = true;
            this.labelComPort.Location = new System.Drawing.Point(6, 28);
            this.labelComPort.Name = "labelComPort";
            this.labelComPort.Size = new System.Drawing.Size(39, 14);
            this.labelComPort.TabIndex = 11;
            this.labelComPort.Text = "Name";
            // 
            // checkBoxTraceLogger
            // 
            this.checkBoxTraceLogger.AutoSize = true;
            this.checkBoxTraceLogger.Location = new System.Drawing.Point(252, 315);
            this.checkBoxTraceLogger.Name = "checkBoxTraceLogger";
            this.checkBoxTraceLogger.Size = new System.Drawing.Size(92, 18);
            this.checkBoxTraceLogger.TabIndex = 31;
            this.checkBoxTraceLogger.Text = "Trace Logger";
            this.checkBoxTraceLogger.UseVisualStyleBackColor = true;
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Location = new System.Drawing.Point(84, 25);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(162, 22);
            this.comboBoxComPort.TabIndex = 12;
            // 
            // labelWebcam
            // 
            this.labelWebcam.AutoSize = true;
            this.labelWebcam.Location = new System.Drawing.Point(6, 28);
            this.labelWebcam.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelWebcam.Name = "labelWebcam";
            this.labelWebcam.Size = new System.Drawing.Size(39, 14);
            this.labelWebcam.TabIndex = 1;
            this.labelWebcam.Text = "Name";
            // 
            // comboBoxWebcam
            // 
            this.comboBoxWebcam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWebcam.FormattingEnabled = true;
            this.comboBoxWebcam.Location = new System.Drawing.Point(84, 25);
            this.comboBoxWebcam.Name = "comboBoxWebcam";
            this.comboBoxWebcam.Size = new System.Drawing.Size(225, 22);
            this.comboBoxWebcam.TabIndex = 2;
            this.comboBoxWebcam.SelectedIndexChanged += new System.EventHandler(this.comboBoxWebcam_SelectedIndexChanged);
            // 
            // buttonCheck
            // 
            this.buttonCheck.Location = new System.Drawing.Point(250, 25);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(59, 23);
            this.buttonCheck.TabIndex = 13;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
            // 
            // groupBoxWebcam
            // 
            this.groupBoxWebcam.Controls.Add(this.labelPixelSize);
            this.groupBoxWebcam.Controls.Add(this.labelMediaType);
            this.groupBoxWebcam.Controls.Add(this.labelWebcam);
            this.groupBoxWebcam.Controls.Add(this.comboBoxMediaType);
            this.groupBoxWebcam.Controls.Add(this.textBoxPixelSizeY);
            this.groupBoxWebcam.Controls.Add(this.textBoxPixelSizeX);
            this.groupBoxWebcam.Controls.Add(this.comboBoxWebcam);
            this.groupBoxWebcam.Controls.Add(this.labelPixelSizeUm);
            this.groupBoxWebcam.Controls.Add(this.labelPixelSizeBy);
            this.groupBoxWebcam.Location = new System.Drawing.Point(12, 12);
            this.groupBoxWebcam.Name = "groupBoxWebcam";
            this.groupBoxWebcam.Size = new System.Drawing.Size(394, 115);
            this.groupBoxWebcam.TabIndex = 0;
            this.groupBoxWebcam.TabStop = false;
            this.groupBoxWebcam.Text = "Webcam";
            // 
            // labelPixelSize
            // 
            this.labelPixelSize.AutoSize = true;
            this.labelPixelSize.Location = new System.Drawing.Point(6, 84);
            this.labelPixelSize.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPixelSize.Name = "labelPixelSize";
            this.labelPixelSize.Size = new System.Drawing.Size(58, 14);
            this.labelPixelSize.TabIndex = 5;
            this.labelPixelSize.Text = "Pixel Size";
            // 
            // labelMediaType
            // 
            this.labelMediaType.AutoSize = true;
            this.labelMediaType.Location = new System.Drawing.Point(6, 56);
            this.labelMediaType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMediaType.Name = "labelMediaType";
            this.labelMediaType.Size = new System.Drawing.Size(69, 14);
            this.labelMediaType.TabIndex = 3;
            this.labelMediaType.Text = "Media Type";
            // 
            // comboBoxMediaType
            // 
            this.comboBoxMediaType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMediaType.FormattingEnabled = true;
            this.comboBoxMediaType.Location = new System.Drawing.Point(84, 53);
            this.comboBoxMediaType.Name = "comboBoxMediaType";
            this.comboBoxMediaType.Size = new System.Drawing.Size(225, 22);
            this.comboBoxMediaType.TabIndex = 4;
            // 
            // textBoxPixelSizeY
            // 
            this.textBoxPixelSizeY.Location = new System.Drawing.Point(175, 81);
            this.textBoxPixelSizeY.Name = "textBoxPixelSizeY";
            this.textBoxPixelSizeY.Size = new System.Drawing.Size(71, 22);
            this.textBoxPixelSizeY.TabIndex = 8;
            this.textBoxPixelSizeY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxPixelSizeX
            // 
            this.textBoxPixelSizeX.Location = new System.Drawing.Point(84, 81);
            this.textBoxPixelSizeX.Name = "textBoxPixelSizeX";
            this.textBoxPixelSizeX.Size = new System.Drawing.Size(71, 22);
            this.textBoxPixelSizeX.TabIndex = 6;
            this.textBoxPixelSizeX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelPixelSizeUm
            // 
            this.labelPixelSizeUm.AutoSize = true;
            this.labelPixelSizeUm.Enabled = false;
            this.labelPixelSizeUm.Location = new System.Drawing.Point(251, 84);
            this.labelPixelSizeUm.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPixelSizeUm.Name = "labelPixelSizeUm";
            this.labelPixelSizeUm.Size = new System.Drawing.Size(24, 14);
            this.labelPixelSizeUm.TabIndex = 9;
            this.labelPixelSizeUm.Text = "um";
            // 
            // labelPixelSizeBy
            // 
            this.labelPixelSizeBy.AutoSize = true;
            this.labelPixelSizeBy.Enabled = false;
            this.labelPixelSizeBy.Location = new System.Drawing.Point(157, 84);
            this.labelPixelSizeBy.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPixelSizeBy.Name = "labelPixelSizeBy";
            this.labelPixelSizeBy.Size = new System.Drawing.Size(18, 14);
            this.labelPixelSizeBy.TabIndex = 7;
            this.labelPixelSizeBy.Text = " x ";
            // 
            // groupBoxComPort
            // 
            this.groupBoxComPort.Controls.Add(this.textBoxPulseGuideMax);
            this.groupBoxComPort.Controls.Add(this.textBoxExposureMax);
            this.groupBoxComPort.Controls.Add(this.buttonCheck);
            this.groupBoxComPort.Controls.Add(this.textBoxPulseGuideMin);
            this.groupBoxComPort.Controls.Add(this.textBoxExposureMin);
            this.groupBoxComPort.Controls.Add(this.labelPulseGuide);
            this.groupBoxComPort.Controls.Add(this.comboBoxComPort);
            this.groupBoxComPort.Controls.Add(this.labelComPort);
            this.groupBoxComPort.Controls.Add(this.labelPulseGuideDash);
            this.groupBoxComPort.Controls.Add(this.labelPulseGuideMS);
            this.groupBoxComPort.Controls.Add(this.labelExposureMS);
            this.groupBoxComPort.Controls.Add(this.labelExposureDash);
            this.groupBoxComPort.Controls.Add(this.labelExposure);
            this.groupBoxComPort.Controls.Add(this.labelMode);
            this.groupBoxComPort.Controls.Add(this.checkBoxAmp);
            this.groupBoxComPort.Controls.Add(this.checkBoxShutter);
            this.groupBoxComPort.Controls.Add(this.checkBoxLx);
            this.groupBoxComPort.Location = new System.Drawing.Point(12, 131);
            this.groupBoxComPort.Name = "groupBoxComPort";
            this.groupBoxComPort.Size = new System.Drawing.Size(394, 136);
            this.groupBoxComPort.TabIndex = 10;
            this.groupBoxComPort.TabStop = false;
            this.groupBoxComPort.Text = "Com Port";
            // 
            // textBoxPulseGuideMax
            // 
            this.textBoxPulseGuideMax.Enabled = false;
            this.textBoxPulseGuideMax.Location = new System.Drawing.Point(175, 104);
            this.textBoxPulseGuideMax.Name = "textBoxPulseGuideMax";
            this.textBoxPulseGuideMax.ReadOnly = true;
            this.textBoxPulseGuideMax.Size = new System.Drawing.Size(71, 22);
            this.textBoxPulseGuideMax.TabIndex = 26;
            this.textBoxPulseGuideMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxExposureMax
            // 
            this.textBoxExposureMax.Enabled = false;
            this.textBoxExposureMax.Location = new System.Drawing.Point(175, 76);
            this.textBoxExposureMax.Name = "textBoxExposureMax";
            this.textBoxExposureMax.ReadOnly = true;
            this.textBoxExposureMax.Size = new System.Drawing.Size(71, 22);
            this.textBoxExposureMax.TabIndex = 21;
            this.textBoxExposureMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxPulseGuideMin
            // 
            this.textBoxPulseGuideMin.Enabled = false;
            this.textBoxPulseGuideMin.Location = new System.Drawing.Point(84, 104);
            this.textBoxPulseGuideMin.Name = "textBoxPulseGuideMin";
            this.textBoxPulseGuideMin.ReadOnly = true;
            this.textBoxPulseGuideMin.Size = new System.Drawing.Size(71, 22);
            this.textBoxPulseGuideMin.TabIndex = 24;
            this.textBoxPulseGuideMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxExposureMin
            // 
            this.textBoxExposureMin.Enabled = false;
            this.textBoxExposureMin.Location = new System.Drawing.Point(84, 76);
            this.textBoxExposureMin.Name = "textBoxExposureMin";
            this.textBoxExposureMin.ReadOnly = true;
            this.textBoxExposureMin.Size = new System.Drawing.Size(71, 22);
            this.textBoxExposureMin.TabIndex = 19;
            this.textBoxExposureMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelPulseGuide
            // 
            this.labelPulseGuide.AutoSize = true;
            this.labelPulseGuide.Location = new System.Drawing.Point(5, 107);
            this.labelPulseGuide.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPulseGuide.Name = "labelPulseGuide";
            this.labelPulseGuide.Size = new System.Drawing.Size(73, 14);
            this.labelPulseGuide.TabIndex = 23;
            this.labelPulseGuide.Text = "Pulse Guide";
            // 
            // labelPulseGuideDash
            // 
            this.labelPulseGuideDash.AutoSize = true;
            this.labelPulseGuideDash.Enabled = false;
            this.labelPulseGuideDash.Location = new System.Drawing.Point(157, 107);
            this.labelPulseGuideDash.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPulseGuideDash.Name = "labelPulseGuideDash";
            this.labelPulseGuideDash.Size = new System.Drawing.Size(17, 14);
            this.labelPulseGuideDash.TabIndex = 25;
            this.labelPulseGuideDash.Text = " - ";
            // 
            // labelPulseGuideMS
            // 
            this.labelPulseGuideMS.AutoSize = true;
            this.labelPulseGuideMS.Enabled = false;
            this.labelPulseGuideMS.Location = new System.Drawing.Point(251, 107);
            this.labelPulseGuideMS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPulseGuideMS.Name = "labelPulseGuideMS";
            this.labelPulseGuideMS.Size = new System.Drawing.Size(23, 14);
            this.labelPulseGuideMS.TabIndex = 27;
            this.labelPulseGuideMS.Text = "ms";
            // 
            // labelExposureMS
            // 
            this.labelExposureMS.AutoSize = true;
            this.labelExposureMS.Enabled = false;
            this.labelExposureMS.Location = new System.Drawing.Point(251, 79);
            this.labelExposureMS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelExposureMS.Name = "labelExposureMS";
            this.labelExposureMS.Size = new System.Drawing.Size(23, 14);
            this.labelExposureMS.TabIndex = 22;
            this.labelExposureMS.Text = "ms";
            // 
            // labelExposureDash
            // 
            this.labelExposureDash.AutoSize = true;
            this.labelExposureDash.Enabled = false;
            this.labelExposureDash.Location = new System.Drawing.Point(157, 79);
            this.labelExposureDash.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelExposureDash.Name = "labelExposureDash";
            this.labelExposureDash.Size = new System.Drawing.Size(17, 14);
            this.labelExposureDash.TabIndex = 20;
            this.labelExposureDash.Text = " - ";
            // 
            // labelExposure
            // 
            this.labelExposure.AutoSize = true;
            this.labelExposure.Location = new System.Drawing.Point(6, 79);
            this.labelExposure.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelExposure.Name = "labelExposure";
            this.labelExposure.Size = new System.Drawing.Size(56, 14);
            this.labelExposure.TabIndex = 18;
            this.labelExposure.Text = "Exposure";
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Location = new System.Drawing.Point(6, 53);
            this.labelMode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(38, 14);
            this.labelMode.TabIndex = 14;
            this.labelMode.Text = "Mode";
            // 
            // checkBoxAmp
            // 
            this.checkBoxAmp.AutoCheck = false;
            this.checkBoxAmp.AutoSize = true;
            this.checkBoxAmp.Enabled = false;
            this.checkBoxAmp.Location = new System.Drawing.Point(199, 52);
            this.checkBoxAmp.Name = "checkBoxAmp";
            this.checkBoxAmp.Size = new System.Drawing.Size(50, 18);
            this.checkBoxAmp.TabIndex = 17;
            this.checkBoxAmp.Text = "Amp";
            this.checkBoxAmp.UseVisualStyleBackColor = true;
            // 
            // checkBoxShutter
            // 
            this.checkBoxShutter.AutoCheck = false;
            this.checkBoxShutter.AutoSize = true;
            this.checkBoxShutter.Enabled = false;
            this.checkBoxShutter.Location = new System.Drawing.Point(127, 52);
            this.checkBoxShutter.Name = "checkBoxShutter";
            this.checkBoxShutter.Size = new System.Drawing.Size(65, 18);
            this.checkBoxShutter.TabIndex = 16;
            this.checkBoxShutter.Text = "Shutter";
            this.checkBoxShutter.UseVisualStyleBackColor = true;
            // 
            // checkBoxLx
            // 
            this.checkBoxLx.AutoCheck = false;
            this.checkBoxLx.AutoSize = true;
            this.checkBoxLx.Enabled = false;
            this.checkBoxLx.Location = new System.Drawing.Point(84, 52);
            this.checkBoxLx.Name = "checkBoxLx";
            this.checkBoxLx.Size = new System.Drawing.Size(37, 18);
            this.checkBoxLx.TabIndex = 15;
            this.checkBoxLx.Text = "LX";
            this.checkBoxLx.UseVisualStyleBackColor = true;
            // 
            // labelPlatformVersion
            // 
            this.labelPlatformVersion.AutoSize = true;
            this.labelPlatformVersion.Location = new System.Drawing.Point(66, 274);
            this.labelPlatformVersion.Name = "labelPlatformVersion";
            this.labelPlatformVersion.Size = new System.Drawing.Size(110, 14);
            this.labelPlatformVersion.TabIndex = 28;
            this.labelPlatformVersion.Text = "Platform Version: --";
            // 
            // labelDriverVersion
            // 
            this.labelDriverVersion.AutoSize = true;
            this.labelDriverVersion.Location = new System.Drawing.Point(66, 295);
            this.labelDriverVersion.Name = "labelDriverVersion";
            this.labelDriverVersion.Size = new System.Drawing.Size(97, 14);
            this.labelDriverVersion.TabIndex = 29;
            this.labelDriverVersion.Text = "Driver Version: --";
            // 
            // labelFirmwareVersion
            // 
            this.labelFirmwareVersion.AutoSize = true;
            this.labelFirmwareVersion.Location = new System.Drawing.Point(66, 316);
            this.labelFirmwareVersion.Name = "labelFirmwareVersion";
            this.labelFirmwareVersion.Size = new System.Drawing.Size(116, 14);
            this.labelFirmwareVersion.TabIndex = 30;
            this.labelFirmwareVersion.Text = "Firmware Version: --";
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 342);
            this.Controls.Add(this.labelFirmwareVersion);
            this.Controls.Add(this.labelDriverVersion);
            this.Controls.Add(this.labelPlatformVersion);
            this.Controls.Add(this.checkBoxTraceLogger);
            this.Controls.Add(this.groupBoxComPort);
            this.Controls.Add(this.groupBoxWebcam);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupDialogForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LxWebcam Setup";
            this.Load += new System.EventHandler(this.SetupDialogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.groupBoxWebcam.ResumeLayout(false);
            this.groupBoxWebcam.PerformLayout();
            this.groupBoxComPort.ResumeLayout(false);
            this.groupBoxComPort.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.PictureBox picASCOM;
        private System.Windows.Forms.Label labelComPort;
        private System.Windows.Forms.CheckBox checkBoxTraceLogger;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.Label labelWebcam;
        private System.Windows.Forms.ComboBox comboBoxWebcam;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.GroupBox groupBoxWebcam;
        private System.Windows.Forms.GroupBox groupBoxComPort;
        private System.Windows.Forms.Label labelPlatformVersion;
        private System.Windows.Forms.Label labelDriverVersion;
        private System.Windows.Forms.Label labelFirmwareVersion;
        private System.Windows.Forms.CheckBox checkBoxLx;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.CheckBox checkBoxAmp;
        private System.Windows.Forms.CheckBox checkBoxShutter;
        private System.Windows.Forms.TextBox textBoxPulseGuideMax;
        private System.Windows.Forms.TextBox textBoxExposureMax;
        private System.Windows.Forms.TextBox textBoxPulseGuideMin;
        private System.Windows.Forms.TextBox textBoxExposureMin;
        private System.Windows.Forms.Label labelPulseGuide;
        private System.Windows.Forms.Label labelPulseGuideDash;
        private System.Windows.Forms.Label labelExposureDash;
        private System.Windows.Forms.Label labelExposure;
        private System.Windows.Forms.Label labelPulseGuideMS;
        private System.Windows.Forms.Label labelExposureMS;
        private System.Windows.Forms.ComboBox comboBoxMediaType;
        private System.Windows.Forms.Label labelMediaType;
        private System.Windows.Forms.Label labelPixelSize;
        private System.Windows.Forms.TextBox textBoxPixelSizeX;
        private System.Windows.Forms.TextBox textBoxPixelSizeY;
        private System.Windows.Forms.Label labelPixelSizeUm;
        private System.Windows.Forms.Label labelPixelSizeBy;
    }
}