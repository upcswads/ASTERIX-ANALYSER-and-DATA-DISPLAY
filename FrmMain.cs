﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GMap.NET.WindowsForms;
using GMap.NET;
using GMap.NET.MapProviders;

namespace MulticastingUDP
{

    public partial class FormMain : Form
    {
        // Static Map Overlay
        GMapOverlay StaticOverlay;
        // Dynamic Map Overlay
        GMapOverlay DinamicOverlay;

        // Keep track of the last selected SSR code index
        int SSR_Filter_Last_Index = 0;

        // Define a lookup table for all possible SSR codes, well even more
        // then all possible but lets keep it simple.
        private bool[] SSR_Code_Lookup = new bool[7778];

        // Define the main listener thread
        Thread ListenForDataThread = new Thread(new ThreadStart(ASTERIX.ListenForData));

        public FormMain()
        {
            InitializeComponent();

            SystemAdaptationDataSet.InitializeData();

            // Here call constructor 
            // for each ASTERIX type
            CAT01.Intitialize();
            CAT02.Intitialize();
            CAT08.Intitialize();
            CAT34.Intitialize();
            CAT48.Intitialize();
            CAT62.Intitialize();
            CAT63.Intitialize();
            CAT65.Intitialize();
            CAT244.Intitialize();

            // Start the thread to listen for data
            ListenForDataThread.Start();

            // Set up progress bar marguee
            this.progressBar1.Step = 2;
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.MarqueeAnimationSpeed = 100; // 100msec
            this.progressBar1.Visible = false;
        }

        public static void Intitialize()
        {

        }

        // This is a timer driven method which will update the main 
        // display box with the currently received data
        private void DataUpdateTimer_Tick(object sender, EventArgs e)
        {

            int count = SharedData.DataBox.Items.Count;
            for (int i = 0; i < count; i++)
                listBoxManFrame.Items.Add(SharedData.DataBox.Items[i]);
            SharedData.DataBox.Items.Clear();

            this.labelActiveConnName.Text = SharedData.ConnName;

            string Port;
            if (SharedData.Current_Port == 0)
            {
                Port = "N/A";
                this.buttonStopRun.Enabled = false;
            }
            else
            {
                Port = SharedData.Current_Port.ToString();
                this.buttonStopRun.Enabled = true;
            }

            this.labelConnIpAndPort.Text = SharedData.CurrentMulticastAddress.ToString() + " : " + Port;
        }

        // Display menu box to enable users to set up connection(s)
        private void connectionSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings SettingDialog = new FrmSettings();
            SettingDialog.Visible = false;
            SettingDialog.Show(this);
            SettingDialog.Visible = true;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Initialize Map
            InitializeMap();
        }

        private void InitializeMap()
        {
            // Set system origin position
            gMapControl.Position = new PointLatLng(SystemAdaptationDataSet.SystemOriginPoint.Lat, SystemAdaptationDataSet.SystemOriginPoint.Lng);
            this.lblCenterLat.Text = gMapControl.Position.Lat.ToString();
            this.lblCenterLon.Text = gMapControl.Position.Lng.ToString();

            // Choose MAP provider and MAP mode
            gMapControl.MapProvider = GMapProviders.GoogleTerrainMap;
            gMapControl.Manager.Mode = AccessMode.ServerAndCache;
            gMapControl.EmptyMapBackground = Color.Gray;

            // Set MIN/MAX for the ZOOM function
            gMapControl.MinZoom = 0;
            gMapControl.MaxZoom = 20;
            // Default ZOOM
            gMapControl.Zoom = 8;
            this.lblZoomLevel.Text = gMapControl.Zoom.ToString();

            // Add overlays
            StaticOverlay = new GMapOverlay(gMapControl, "OverlayTwo");
            gMapControl.Overlays.Add(StaticOverlay);
            DinamicOverlay = new GMapOverlay(gMapControl, "OverlayOne");
            gMapControl.Overlays.Add(DinamicOverlay);

            this.label9.Text = "Current rate at: " + this.PlotandTrackDisplayUpdateTimer.Interval.ToString() + "ms";
            this.comboBox1.Text = "Plain";

            // Now build static display
            StaticDisplayBuilder.Build(ref StaticOverlay);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (SharedData.bool_Listen_for_Data == true)
            {
                SharedData.bool_Listen_for_Data = false;
                buttonStopRun.Text = "Stopped";
                this.progressBar1.Visible = false;
                this.detailedViewToolStripMenuItem.Enabled = true;
                this.toolsToolStripMenuItem.Enabled = true;
                this.dataBySSRCodeToolStripMenuItem.Enabled = true;
                this.googleEarthToolStripMenuItem.Enabled = true;
            }
            else
            {
                SharedData.bool_Listen_for_Data = true;
                buttonStopRun.Text = "Running";
                this.progressBar1.Visible = true;
                this.detailedViewToolStripMenuItem.Enabled = false;
                this.toolsToolStripMenuItem.Enabled = false;
                this.dataBySSRCodeToolStripMenuItem.Enabled = false;
                this.googleEarthToolStripMenuItem.Enabled = false;
            }

            HandlePlotDisplayEnabledChanged();
        }

        // This method will analyze received, determine what data items are present and then
        // display results in a user friendly window 
        private void cAT001DataItemPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT001;

            // Show the dialog
            FrmItemPresence.Show(this);

        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cAT034DataItemPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT034;

            // Show the dialog
            FrmItemPresence.Show(this);

        }

        private void cAT002DataItemPresenceToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT048;

            // Show the dialog
            FrmItemPresence.Show(this);

        }

        private void cAT002DataItemPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT002;

            // Show the dialog
            FrmItemPresence.Show(this);

        }

        private void cAT008DataItemPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT008;

            // Show the dialog
            FrmItemPresence.Show(this);

        }

        private void cAT062DataItemPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT062;

            // Show the dialog
            FrmItemPresence.Show(this);
        }

        private void cAT065DataItemPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT065;

            // Show the dialog
            FrmItemPresence.Show(this);
        }

        private void cAT063ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Define the box
            FrmDataItemPresence FrmItemPresence = new FrmDataItemPresence();

            // Set desired asterix category
            FrmItemPresence.CAT_Type_To_Analyze = SharedData.Supported_Asterix_CAT_Type.CAT063;

            // Show the dialog
            FrmItemPresence.Show(this);
        }

        private void cAT244DataItemPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Asterix Sniffer 1.3 by Amer Kapetanovic\nakapetanovic@gmail.com", "About");
        }

        private void resetDataBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool ListenigForForData = SharedData.bool_Listen_for_Data;
            if (SharedData.bool_Listen_for_Data == true)
            {
                SharedData.bool_Listen_for_Data = false;
                buttonStopRun.Text = "Stopped";
                this.progressBar1.Visible = false;
            }


            // Here reset the data buffer, this will empty data buffer.
            int NumOfItems = listBoxManFrame.Items.Count - 1;
            for (int Index = NumOfItems; Index >= 0; Index--)
                listBoxManFrame.Items.RemoveAt(Index);

            // Reset data buffer for each
            // category
            CAT01.Intitialize();
            CAT02.Intitialize();
            CAT08.Intitialize();
            CAT34.Intitialize();
            CAT48.Intitialize();
            CAT62.Intitialize();
            CAT63.Intitialize();
            CAT65.Intitialize();
            CAT244.Intitialize();

            if (ListenigForForData == true)
            {
                SharedData.bool_Listen_for_Data = true;
                buttonStopRun.Text = "Running";
                this.progressBar1.Visible = true;
            }
        }

        private void dataItem000MessageTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contourIdentifier040ContourIdentifierToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataSourceIdentifierToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataSourceIdentifierToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void CAT02MessageTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT02I000;
            MyDetailedView.Show();
        }

        private void messageTypeToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void timeofDayToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void timeOfDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT02I030;
            MyDetailedView.Show();
        }

        private void messageTypeToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void sectorNumberToolStripMenuItem1_Click(object sender, EventArgs e)
        {


        }

        private void targetReportDescriptorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT01I020;
            MyDetailedView.Show();
        }

        private void measuredPositionInPolarCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT01I040;
            MyDetailedView.Show();
        }

        private void mode3ACodeInOctalRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT01I070;
            MyDetailedView.Show();
        }

        private void targetReportDescriptorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT48I020;
            MyDetailedView.Show();
        }

        private void mode3ACodeInOctalRepresentationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT48I070;
            MyDetailedView.Show();
        }

        private void flightLevelInBinaryRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT48I090;
            MyDetailedView.Show();
        }

        private void modeCCodeInBinaryRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT01I090;
            MyDetailedView.Show();
        }

        private void measuredPositionInSlantPolarCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT48I040;
            MyDetailedView.Show();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EarthPlotExporter MyForm = new EarthPlotExporter();
            MyForm.TypeOfExporter = EarthPlotExporter.ExporterType.EarthPlot;
            MyForm.Show();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewDataBySSRCode MyForm = new ViewDataBySSRCode();
            MyForm.Show();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EarthPlotExporter MyForm = new EarthPlotExporter();
            MyForm.TypeOfExporter = EarthPlotExporter.ExporterType.GePath;
            MyForm.Show();
        }

        private void mode1CodeInOctalRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void radarPlotCharacteristicsToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void sectorNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT02I020;
            MyDetailedView.Show();
        }

        private void antennaRotationPeriodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT02I041;
            MyDetailedView.Show();
        }

        // This is a timer that is resposible for updating the data display
        private void PlotDisplayTimer_Tick(object sender, EventArgs e)
        {
            Update_PlotTrack_Data();
        }


        private void Update_PlotTrack_Data()
        {
            // First clear all the data from the previous cycle.
            if (DinamicOverlay.Markers.Count > 0)
            {
                DinamicOverlay.Markers.Clear();
            }

            // Now get the data since the last cycle and display it on the map
            DynamicDisplayBuilder DP = new DynamicDisplayBuilder();
            System.Collections.Generic.List<DynamicDisplayBuilder.TargetType> TargetList = new System.Collections.Generic.List<DynamicDisplayBuilder.TargetType>();

            // Here hanlde display od live data
            if (SharedData.bool_Listen_for_Data == true)
            {
                DynamicDisplayBuilder.GetDisplayData(false, out TargetList);

                foreach (DynamicDisplayBuilder.TargetType Target in TargetList)
                {
                    if (Passes_Check_For_Flight_Level_Filter(Target.ModeC))
                    {
                        // If SSR code filtering is to be applied 
                        if (this.checkBoxFilterBySSR.Enabled == true &&
                            this.textBoxSSRCode.Enabled == true &&
                            this.textBoxSSRCode.Text.Length == 4)
                        {
                            if (Target.ModeA == this.textBoxSSRCode.Text)
                            {
                                GMap.NET.WindowsForms.Markers.GMapMarkerCross MyMarker = new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(Target.Lat, Target.Lon));
                                MyMarker.ToolTipMode = MarkerTooltipMode.Always;

                                if (Target.ACID_Modes != null)
                                    MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ACID_Modes + "\n" + Target.ModeC;
                                else
                                    MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ModeC;

                                SetLabelAttributes(ref MyMarker);
                                DinamicOverlay.Markers.Add(MyMarker);
                            }
                        }
                        else // No SSR filter so just display all of them
                        {
                            GMap.NET.WindowsForms.Markers.GMapMarkerCross MyMarker = new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(Target.Lat, Target.Lon));
                            MyMarker.DisableRegionCheck = true;

                            MyMarker.ToolTipMode = MarkerTooltipMode.Always;

                            if (Target.ACID_Modes != null)
                                MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ACID_Modes + "\n" + Target.ModeC;
                            else
                                MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ModeC;

                            SetLabelAttributes(ref MyMarker);
                            DinamicOverlay.Markers.Add(MyMarker);
                        }
                    }
                }
            }
            else // Here handle display of passive display (buffered data)
            {
                DynamicDisplayBuilder.GetDisplayData(true, out TargetList);

                foreach (DynamicDisplayBuilder.TargetType Target in TargetList)
                {
                    if (Passes_Check_For_Flight_Level_Filter(Target.ModeC))
                    {

                        // If SSR code filtering is to be applied 
                        if (this.checkBoxFilterBySSR.Checked == true && (this.comboBoxSSRFilterBox.Items.Count > 0))
                        {
                            if (Target.ModeA == this.comboBoxSSRFilterBox.Items[SSR_Filter_Last_Index].ToString())
                            {
                                GMap.NET.WindowsForms.Markers.GMapMarkerCross MyMarker = new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(Target.Lat, Target.Lon));
                                MyMarker.ToolTipMode = MarkerTooltipMode.Always;

                                if (Target.ACID_Modes != null)
                                    MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ACID_Modes + "\n" + Target.ModeC;
                                else
                                    MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ModeC;

                                SetLabelAttributes(ref MyMarker);
                                DinamicOverlay.Markers.Add(MyMarker);
                            }
                        }
                        else // No filter so just display all of them
                        {
                            GMap.NET.WindowsForms.Markers.GMapMarkerCross MyMarker = new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(Target.Lat, Target.Lon));
                            MyMarker.ToolTipMode = MarkerTooltipMode.Always;
                            
                            if (Target.ACID_Modes != null)
                                MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ACID_Modes + "\n" + Target.ModeC;
                            else
                                MyMarker.ToolTipText = Target.ModeA + "\n" + Target.ModeC;

                            SetLabelAttributes(ref MyMarker);
                            DinamicOverlay.Markers.Add(MyMarker);
                        }
                    }
                }
            }
        }

        private void SetLabelAttributes(ref GMap.NET.WindowsForms.Markers.GMapMarkerCross Marker_In)
        {

            // label Font and Size
            FontFamily family = new FontFamily("Microsoft Sans Serif");
            Font font = new Font(family, 8,
            FontStyle.Bold | FontStyle.Regular);
            Marker_In.ToolTip.Font = font;

            // Label text color
            Marker_In.ToolTip.Foreground = Brushes.LimeGreen;

            // Symbol color
            Marker_In.Pen = new Pen(Brushes.WhiteSmoke);

            // Tool Tip border color
            Marker_In.ToolTip.Stroke = new Pen(Brushes.WhiteSmoke, 1);

            // Tool Tip Fill color
            Marker_In.ToolTip.Fill = Brushes.Transparent;
        }

        private bool Passes_Check_For_Flight_Level_Filter(string Flight_Level)
        {
            bool Result = true;

            if (this.checkBoxFLFilter.Checked)
            {
                double FL = double.Parse(Flight_Level);

                if (FL < (double)this.numericUpDownLower.Value || FL > (double)this.numericUpDownUpper.Value)
                    Result = false;
            }

            return Result;
        }

        private void tabPlotDisplay_Click(object sender, EventArgs e)
        {

        }

        private void checkEnableDisplay_CheckedChanged(object sender, EventArgs e)
        {
            HandlePlotDisplayEnabledChanged();
        }

        private void HandlePlotDisplayEnabledChanged()
        {
            if (this.checkEnableDisplay.Checked == true)
            {
                this.checkEnableDisplay.BackColor = Color.Green;
                this.groupBoxSSRFilter.Enabled = true;
                this.checkBoxFilterBySSR.Enabled = true;

                if (SharedData.bool_Listen_for_Data == true)
                {
                    this.checkEnableDisplay.Text = "Live Enabled";
                    this.groupBoxUpdateRate.Enabled = true;
                    this.groupBoxUpdateRate.Enabled = true;
                    this.textBoxUpdateRate.Enabled = true; ;
                }
                else
                {
                    this.checkEnableDisplay.Text = "Passive Enabled";
                    this.groupBoxUpdateRate.Enabled = false;
                    this.groupBoxUpdateRate.Enabled = false;
                    this.textBoxUpdateRate.Enabled = false;
                }

                // Start the timer
                this.PlotandTrackDisplayUpdateTimer.Enabled = true;
            }
            else
            {
                this.checkEnableDisplay.Text = "Disabled";
                this.checkEnableDisplay.BackColor = Color.Red;
                this.checkBoxFilterBySSR.BackColor = Color.Transparent;
                this.textBoxSSRCode.Enabled = false;
                this.textBoxUpdateRate.Enabled = false;
                this.groupBoxSSRFilter.Enabled = false;
                this.groupBoxUpdateRate.Enabled = false;
                this.checkBoxFilterBySSR.Enabled = false;
                this.checkBoxFilterBySSR.Checked = false;

                // Stop the timer
                this.PlotandTrackDisplayUpdateTimer.Enabled = false;

                // Clear the latest map display
                DinamicOverlay.Markers.Clear();
            }
        }

        private void textBoxUpdateRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            string allowedCharacterSet = "0123456789\b";    	   //Allowed character set

            if (allowedCharacterSet.Contains(e.KeyChar.ToString()))
            {

            }
            else if (e.KeyChar.ToString() == "\r")
            {
                e.Handled = true;

                int UpdateRateinMS = 4000;
                if (int.TryParse(this.textBoxUpdateRate.Text, out UpdateRateinMS) == true)
                {
                    if (UpdateRateinMS > 0 && UpdateRateinMS < 100001)
                    {
                        this.PlotandTrackDisplayUpdateTimer.Interval = UpdateRateinMS;
                        this.label9.Text = "Current rate at: " + this.PlotandTrackDisplayUpdateTimer.Interval.ToString() + "ms";
                        this.textBoxUpdateRate.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Please enter an integer in range of 1000 to 10000");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter an integer in range of 1000 to 10000");
                }
            }
            else
            {
                MessageBox.Show("Please enter an integer in range of 1000 to 10000");
            }
        }

        private void checkBoxFilterBySSR_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxFilterBySSR.Checked == true)
            {
                this.checkBoxFilterBySSR.Text = "Enabled";
                this.checkBoxFilterBySSR.BackColor = Color.Red;

                if (SharedData.bool_Listen_for_Data == true)
                {
                    this.comboBoxSSRFilterBox.Enabled = false;
                    this.textBoxSSRCode.Enabled = true;
                }
                else
                {
                    this.comboBoxSSRFilterBox.Enabled = true;
                    this.textBoxSSRCode.Enabled = false;
                }
            }
            else
            {
                this.checkBoxFilterBySSR.Text = "Disbaled";
                this.checkBoxFilterBySSR.BackColor = Color.Transparent;
                this.comboBoxSSRFilterBox.Enabled = false;
                this.textBoxSSRCode.Enabled = false;
            }
        }

        private void comboBoxSSRFilterBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Each time the form is opened reset code lookup
            // and then populate based on the latest received
            // data
            for (int I = 0; I < SSR_Code_Lookup.Length; I++)
                SSR_Code_Lookup[I] = false;

            // On load determine what SSR codes are present end populate the combo box
            if (MainASTERIXDataStorage.CAT01Message.Count > 0)
            {
                foreach (MainASTERIXDataStorage.CAT01Data Msg in MainASTERIXDataStorage.CAT01Message)
                {
                    if (Msg.I001DataItems[CAT01.ItemIDToIndex("070")].CurrentlyPresent == true)
                    {
                        CAT01I070Types.CAT01070Mode3UserData MyData = (CAT01I070Types.CAT01070Mode3UserData)Msg.I001DataItems[CAT01.ItemIDToIndex("070")].value;
                        int Result;
                        if (int.TryParse(MyData.Mode3A_Code, out Result) == true)
                            SSR_Code_Lookup[Result] = true;
                    }
                }
            }
            else if (MainASTERIXDataStorage.CAT48Message.Count > 0)
            {
                foreach (MainASTERIXDataStorage.CAT48Data Msg in MainASTERIXDataStorage.CAT48Message)
                {
                    if (Msg.I048DataItems[CAT48.ItemIDToIndex("070")].CurrentlyPresent == true)
                    {
                        CAT48I070Types.CAT48I070Mode3UserData MyData = (CAT48I070Types.CAT48I070Mode3UserData)Msg.I048DataItems[CAT48.ItemIDToIndex("070")].value;

                        int Result;
                        if (int.TryParse(MyData.Mode3A_Code, out Result) == true)
                            SSR_Code_Lookup[Result] = true;
                    }
                }
            }
            else
            {

            }

            this.comboBoxSSRFilterBox.Items.Clear();
            for (int I = 0; I < SSR_Code_Lookup.Length; I++)
            {
                if (SSR_Code_Lookup[I] == true)
                    this.comboBoxSSRFilterBox.Items.Add(I.ToString().PadLeft(4, '0'));
            }

            if (this.comboBoxSSRFilterBox.Items.Count > 0)
                this.comboBoxSSRFilterBox.SelectedIndex = 0;
        }

        private void comboBoxSSRFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SSR_Filter_Last_Index = this.comboBoxSSRFilterBox.SelectedIndex;
        }

        private void gMapControl_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            gMapControl.Zoom = gMapControl.Zoom + 1;
            this.lblZoomLevel.Text = gMapControl.Zoom.ToString();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            gMapControl.Zoom = gMapControl.Zoom - 1;
            this.lblZoomLevel.Text = gMapControl.Zoom.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (gMapControl.Zoom > 10)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat + 0.1, gMapControl.Position.Lng);
            else if (gMapControl.Zoom > 8)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat + 0.2, gMapControl.Position.Lng);
            else
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat + 0.5, gMapControl.Position.Lng);
            this.lblCenterLat.Text = gMapControl.Position.Lat.ToString();
            this.lblCenterLon.Text = gMapControl.Position.Lng.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (gMapControl.Zoom > 10)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat - 0.1, gMapControl.Position.Lng);
            else if (gMapControl.Zoom > 8)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat - 0.2, gMapControl.Position.Lng);
            else
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat - 0.5, gMapControl.Position.Lng);
            this.lblCenterLat.Text = gMapControl.Position.Lat.ToString();
            this.lblCenterLon.Text = gMapControl.Position.Lng.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (gMapControl.Zoom > 10)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat, gMapControl.Position.Lng - 0.1);
            else if (gMapControl.Zoom > 8)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat, gMapControl.Position.Lng - 0.2);
            else
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat, gMapControl.Position.Lng - 0.5);
            this.lblCenterLat.Text = gMapControl.Position.Lat.ToString();
            this.lblCenterLon.Text = gMapControl.Position.Lng.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (gMapControl.Zoom > 10)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat, gMapControl.Position.Lng + 0.1);
            if (gMapControl.Zoom > 8)
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat, gMapControl.Position.Lng + 0.2);
            else
                gMapControl.Position = new PointLatLng(gMapControl.Position.Lat, gMapControl.Position.Lng + 0.5);
            this.lblCenterLat.Text = gMapControl.Position.Lat.ToString();
            this.lblCenterLon.Text = gMapControl.Position.Lng.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            gMapControl.Position = new PointLatLng(44.05267, 17.6769);
            this.lblCenterLat.Text = gMapControl.Position.Lat.ToString();
            this.lblCenterLon.Text = gMapControl.Position.Lng.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.Text == "Google Plain")
            {
                gMapControl.MapProvider = GMapProviders.GoogleMap;
            }
            else if (this.comboBox1.Text == "Google Satellite")
            {
                gMapControl.MapProvider = GMapProviders.GoogleSatelliteMap;
            }
            else if (this.comboBox1.Text == "Google Terrain")
            {
                gMapControl.MapProvider = GMapProviders.GoogleTerrainMap;
            }
            else if (this.comboBox1.Text == "Google Hybrid")
            {
                gMapControl.MapProvider = GMapProviders.GoogleHybridMap;
            }
            else if (this.comboBox1.Text == "Custom Built")
            {
                gMapControl.MapProvider = GMapProviders.EmptyProvider;

            }


        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        // Update Mouse posistion on the control
        private void gMapControl_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void aircraftAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aircraftIdentificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDetailedView MyDetailedView = new FrmDetailedView();
            MyDetailedView.WhatToDisplay = FrmDetailedView.DisplayType.CAT48I240;
            MyDetailedView.Show();
        }

        private void colorDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayAttibutePicker ColorPickerForm = new DisplayAttibutePicker();
            ColorPickerForm.Show();
        }

        private void StaticDisplayTimer_Tick(object sender, EventArgs e)
        {
            if (DisplayAttributes.StaticDisplayBuildRequired)
            {
                // Always check for the change to the background color
                gMapControl.EmptyMapBackground = DisplayAttributes.GetDisplayAttribute(DisplayAttributes.DisplayItemsType.BackgroundColor).TextColor;

                // rebuild static display
                StaticOverlay.Markers.Clear();
                StaticOverlay.Routes.Clear();
                StaticOverlay.Polygons.Clear();
                StaticDisplayBuilder.Build(ref StaticOverlay);
                DisplayAttributes.StaticDisplayBuildRequired = false;
            }
        }

        private void gMapControl_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void gMapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DisplayRightClickOptions MyForm = new DisplayRightClickOptions();
                MyForm.StartPosition = FormStartPosition.Manual;
                MyForm.Location = new Point(e.X + 75, e.Y + 150);
                MyForm.Show();
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            this.tabMainTab.Size = new Size(this.Size.Width - 16, this.Size.Height - 90);

        }

        private void tabMainTab_SizeChanged(object sender, EventArgs e)
        {
            this.tabPlotDisplay.Size = new Size(this.tabMainTab.Size.Width - 8, this.tabMainTab.Size.Height - 26);
        }

        private void tabPlotDisplay_SizeChanged(object sender, EventArgs e)
        {
            this.gMapControl.Size = new Size(this.tabPlotDisplay.Size.Width - 147, this.tabPlotDisplay.Size.Height - 12);
            this.groupBoxConnection.Location = new Point(this.Size.Width - 408, 1);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxFLFilter.Checked)
            {
                this.checkBoxFLFilter.Text = "Enabled";
                this.checkBoxFLFilter.BackColor = Color.Red;
            }
            else
            {
                this.checkBoxFLFilter.Text = "Disabled";
                this.checkBoxFLFilter.BackColor = Color.Transparent;
            }
        }

        private void tabMainTab_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            FrmSettings SettingDialog = new FrmSettings();
            SettingDialog.Visible = false;
            SettingDialog.Show(this);
            SettingDialog.Visible = true;
        }
    }
}
