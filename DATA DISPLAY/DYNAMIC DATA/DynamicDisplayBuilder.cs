﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET.WindowsForms;
using GMap.NET;
using GMap.NET.MapProviders;
using System.Drawing;

namespace AsterixDisplayAnalyser
{
    class DynamicDisplayBuilder
    {
        public class TargetType
        {
            /// <summary>
            /// ////////////////////////////////////////////////
            /// Track Label Items
            /// </summary>
            public string ModeA;
            public string ModeC;
            public string ModeC_Previous_Cycle;
            public string ACID_Mode_S;
            public double Lat;
            public double Lon;
            /// <summary>
            /// ////////////////////////////////////////////////////
            /// Extended label items )Applcable to CAT48 and CAT62
            /// </summary>
            public string TAS = "N/A";
            public string IAS = "N/A";
            public string MACH = "N/A";
            public string M_HDG = "N/A";
            public string TRK = "N/A";
            /// <summary>
            /// ////////////////////////////////////////////////////
            /// Internal stuff
            /// </summary>
            public int TrackNumber = -1;
            public int TrackTerminateTreshold = Properties.Settings.Default.TrackCoast;
            // Image properties
            public GMapTargetandLabel MyMarker = new GMapTargetandLabel(new PointLatLng(0, 0));
        }

        // Keeps track of the data index from the last update of the 
        // display. Used in order to be able to extract only targets recived
        // since the last data update. 
        private static int LastDataIndex = 0;

        // This method is to be used to reset the data index when the main data buffer is reseted
        // and a new block of data is te be recived.
        public static int ResetGetDataIndex
        {
            get { return LastDataIndex; }
            set { LastDataIndex = 0; }
        }

        private static System.Collections.Generic.List<TargetType> CurrentTargetList = new System.Collections.Generic.List<TargetType>();
        private static System.Collections.Generic.List<TargetType> GlobalTargetList = new System.Collections.Generic.List<TargetType>();
        private static System.Collections.Generic.List<TargetType> PSRTargetList = new System.Collections.Generic.List<TargetType>();

        public static void UpdateCFL(int Index, string CFL_Value)
        {
            GlobalTargetList[Index].MyMarker.CFL_STRING = CFL_Value;
        }

        public static void Initialise()
        {
            GlobalTargetList.Clear();
            for (int I = 0; I < 65536; I++)
            {
                GlobalTargetList.Add(new TargetType());
            }
        }

        private static void UpdateGlobalList()
        {
            foreach (TargetType CurrentTarget in CurrentTargetList)
            {
                CurrentTarget.TrackTerminateTreshold = 0;
                if (CurrentTarget.TrackNumber != -1)
                {
                    GlobalTargetList[CurrentTarget.TrackNumber].ModeA = CurrentTarget.ModeA;
                    GlobalTargetList[CurrentTarget.TrackNumber].ModeC_Previous_Cycle = "";
                    if (GlobalTargetList[CurrentTarget.TrackNumber].ModeC != null)
                        GlobalTargetList[CurrentTarget.TrackNumber].ModeC_Previous_Cycle = "" + GlobalTargetList[CurrentTarget.TrackNumber].ModeC;
                    GlobalTargetList[CurrentTarget.TrackNumber].ModeC = CurrentTarget.ModeC;
                    GlobalTargetList[CurrentTarget.TrackNumber].ACID_Mode_S = CurrentTarget.ACID_Mode_S;
                    GlobalTargetList[CurrentTarget.TrackNumber].M_HDG = CurrentTarget.M_HDG;
                    GlobalTargetList[CurrentTarget.TrackNumber].IAS = CurrentTarget.IAS;
                    GlobalTargetList[CurrentTarget.TrackNumber].TRK = CurrentTarget.TRK;
                    GlobalTargetList[CurrentTarget.TrackNumber].MACH = CurrentTarget.MACH;
                    GlobalTargetList[CurrentTarget.TrackNumber].TAS = CurrentTarget.TAS;
                    GlobalTargetList[CurrentTarget.TrackNumber].Lat = CurrentTarget.Lat;
                    GlobalTargetList[CurrentTarget.TrackNumber].Lon = CurrentTarget.Lon;
                    GlobalTargetList[CurrentTarget.TrackNumber].TrackNumber = CurrentTarget.TrackNumber;
                    GlobalTargetList[CurrentTarget.TrackNumber].TrackTerminateTreshold = CurrentTarget.TrackTerminateTreshold;
                }
                else
                {
                    int ModeAIndex = int.Parse(CurrentTarget.ModeA.ToString());
                    GlobalTargetList[ModeAIndex].ModeA = CurrentTarget.ModeA;
                    GlobalTargetList[ModeAIndex].ModeC_Previous_Cycle = "";
                    if (GlobalTargetList[ModeAIndex].ModeC != null)
                        GlobalTargetList[ModeAIndex].ModeC_Previous_Cycle = "" + GlobalTargetList[ModeAIndex].ModeC;
                    GlobalTargetList[ModeAIndex].ModeC = CurrentTarget.ModeC;
                    GlobalTargetList[ModeAIndex].ACID_Mode_S = CurrentTarget.ACID_Mode_S;
                    GlobalTargetList[ModeAIndex].M_HDG = CurrentTarget.M_HDG;
                    GlobalTargetList[ModeAIndex].IAS = CurrentTarget.IAS;
                    GlobalTargetList[ModeAIndex].TRK = CurrentTarget.TRK;
                    GlobalTargetList[ModeAIndex].MACH = CurrentTarget.MACH;
                    GlobalTargetList[ModeAIndex].TAS = CurrentTarget.TAS;
                    GlobalTargetList[ModeAIndex].Lat = CurrentTarget.Lat;
                    GlobalTargetList[ModeAIndex].Lon = CurrentTarget.Lon;
                    GlobalTargetList[ModeAIndex].TrackNumber = ModeAIndex;
                    GlobalTargetList[ModeAIndex].TrackTerminateTreshold = CurrentTarget.TrackTerminateTreshold;
                }
            }

            CurrentTargetList.Clear();
            foreach (TargetType GlobalTarget in GlobalTargetList)
            {
                if (GlobalTarget.TrackTerminateTreshold < Properties.Settings.Default.TrackCoast)
                {
                    TargetType NewTarget = new TargetType();
                    GlobalTarget.TrackTerminateTreshold++;
                    NewTarget.ModeA = GlobalTarget.ModeA;
                    NewTarget.ModeC_Previous_Cycle = GlobalTarget.ModeC_Previous_Cycle;
                    NewTarget.ModeC = GlobalTarget.ModeC;
                    NewTarget.ACID_Mode_S = GlobalTarget.ACID_Mode_S;
                    NewTarget.TRK = GlobalTarget.TRK;
                    NewTarget.TAS = GlobalTarget.TAS;
                    NewTarget.MACH = GlobalTarget.MACH;
                    NewTarget.M_HDG = GlobalTarget.M_HDG;
                    NewTarget.IAS = GlobalTarget.IAS;                   
                    NewTarget.Lat = GlobalTarget.Lat;
                    NewTarget.Lon = GlobalTarget.Lon;
                    NewTarget.TrackNumber = GlobalTarget.TrackNumber;
                    NewTarget.TrackTerminateTreshold = GlobalTarget.TrackTerminateTreshold;
                    NewTarget.MyMarker = GlobalTarget.MyMarker;
                    CurrentTargetList.Add(NewTarget);
                }
                else
                {
                    if (GlobalTarget.MyMarker != null)
                        GlobalTarget.MyMarker.TerminateTarget();
                }
            }

            if (Properties.Settings.Default.DisplayPSR == true)
            {
                // Now append all the PSR tracks to the end of the display list
                foreach (TargetType PSRTgtList in PSRTargetList)
                {
                    TargetType NewTarget = new TargetType();
                    NewTarget.ModeC_Previous_Cycle = PSRTgtList.ModeC_Previous_Cycle;
                    NewTarget.Lat = PSRTgtList.Lat;
                    NewTarget.Lon = PSRTgtList.Lon;
                    NewTarget.TrackNumber = PSRTgtList.TrackNumber;
                    NewTarget.TrackTerminateTreshold = 0;
                    NewTarget.MyMarker = PSRTgtList.MyMarker;
                    CurrentTargetList.Add(NewTarget);
                }
            }
        }

        // Each time this method is called it will extract the targets recived since the
        // the method was last called. It returns a list of the targets in a user friendly
        // format (The TargetType)
        public static void GetDisplayData(bool Return_Buffered, out System.Collections.Generic.List<TargetType> TargetList)
        {
            // First remove all the previous data
            CurrentTargetList.Clear();
            PSRTargetList.Clear();

            if (Return_Buffered == true)
            {
                if (MainASTERIXDataStorage.CAT01Message.Count > 0)
                {
                    for (int Start_Idx = 0; Start_Idx < MainASTERIXDataStorage.CAT01Message.Count; Start_Idx++)
                    {
                        MainASTERIXDataStorage.CAT01Data Msg = MainASTERIXDataStorage.CAT01Message[Start_Idx];

                        // Get Target Descriptor
                        CAT01I020UserData MyCAT01I020UserData = (CAT01I020UserData)Msg.CAT01DataItems[CAT01.ItemIDToIndex("020")].value;
                        // Get Mode3A
                        CAT01I070Types.CAT01070Mode3UserData Mode3AData = (CAT01I070Types.CAT01070Mode3UserData)Msg.CAT01DataItems[CAT01.ItemIDToIndex("070")].value;
                        // Get Lat/Long
                        CAT01I040Types.CAT01I040MeasuredPosInPolarCoordinates LatLongData = (CAT01I040Types.CAT01I040MeasuredPosInPolarCoordinates)Msg.CAT01DataItems[CAT01.ItemIDToIndex("040")].value;
                        // Get Flight Level
                        CAT01I090Types.CAT01I090FlightLevelUserData FlightLevelData = (CAT01I090Types.CAT01I090FlightLevelUserData)Msg.CAT01DataItems[CAT01.ItemIDToIndex("090")].value;

                        TargetType Target = new TargetType();
                        if (MyCAT01I020UserData.Type_Of_Radar_Detection == CAT01I020Types.Radar_Detection_Type.Primary)
                        {
                            Target.ModeA = "PSR";
                            Target.ModeC = "";
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            Target.TrackTerminateTreshold = 0;
                            PSRTargetList.Add(Target);
                        }
                        else if ((MyCAT01I020UserData.Type_Of_Radar_Detection != CAT01I020Types.Radar_Detection_Type.No_Detection) && (MyCAT01I020UserData.Type_Of_Radar_Detection != CAT01I020Types.Radar_Detection_Type.Unknown_Data))
                        {
                            Target.ModeA = Mode3AData.Mode3A_Code;
                            Target.ModeC = FlightLevelData.FlightLevel.ToString();
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            CurrentTargetList.Add(Target);
                        }

                    }
                }
                else if (MainASTERIXDataStorage.CAT48Message.Count > 0)
                {

                    for (int Start_Idx = 0; Start_Idx < MainASTERIXDataStorage.CAT48Message.Count; Start_Idx++)
                    {

                        MainASTERIXDataStorage.CAT48Data Msg = MainASTERIXDataStorage.CAT48Message[Start_Idx];

                        // Get Target Descriptor
                        CAT48I020UserData MyCAT48I020UserData = (CAT48I020UserData)Msg.CAT48DataItems[CAT48.ItemIDToIndex("020")].value;
                        //
                        CAT48I070Types.CAT48I070Mode3UserData Mode3AData = (CAT48I070Types.CAT48I070Mode3UserData)Msg.CAT48DataItems[CAT48.ItemIDToIndex("070")].value;
                        // Get Lat/Long in decimal
                        CAT48I040Types.CAT48I040MeasuredPosInPolarCoordinates LatLongData = (CAT48I040Types.CAT48I040MeasuredPosInPolarCoordinates)Msg.CAT48DataItems[CAT48.ItemIDToIndex("040")].value;
                        // Get Flight Level
                        CAT48I090Types.CAT48I090FlightLevelUserData FlightLevelData = (CAT48I090Types.CAT48I090FlightLevelUserData)Msg.CAT48DataItems[CAT48.ItemIDToIndex("090")].value;
                        // Get ACID data for Mode-S
                        CAT48I240Types.CAT48I240ACID_Data ACID_Mode_S = (CAT48I240Types.CAT48I240ACID_Data)Msg.CAT48DataItems[CAT48.ItemIDToIndex("240")].value;

                        TargetType Target = new TargetType();
                        if ((MyCAT48I020UserData.Type_Of_Report == CAT48I020Types.Type_Of_Report_Type.Single_PSR) || (MyCAT48I020UserData.Type_Of_Report == CAT48I020Types.Type_Of_Report_Type.Mode_S_Roll_Call_PSR))
                        {
                            Target.ModeA = "PSR";
                            Target.ModeC = "";
                            Target.ACID_Mode_S = "";
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            Target.TrackTerminateTreshold = 0;
                            PSRTargetList.Add(Target);
                        }
                        else if ((MyCAT48I020UserData.Type_Of_Report != CAT48I020Types.Type_Of_Report_Type.No_Detection) &&
                        (MyCAT48I020UserData.Type_Of_Report != CAT48I020Types.Type_Of_Report_Type.Unknown_Data))
                        {
                            Target.ModeA = Mode3AData.Mode3A_Code;
                            Target.ModeC = FlightLevelData.FlightLevel.ToString();
                            if (ACID_Mode_S != null)
                            {
                                Target.ACID_Mode_S = ACID_Mode_S.ACID;
                            }
                            else
                            {
                                Target.ACID_Mode_S = "N/A";
                            }
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            CurrentTargetList.Add(Target);
                        }
                    }
                }
                else if (MainASTERIXDataStorage.CAT62Message.Count > 0)
                {

                    for (int Start_Idx = 0; Start_Idx < MainASTERIXDataStorage.CAT62Message.Count; Start_Idx++)
                    {
                        MainASTERIXDataStorage.CAT62Data Msg = MainASTERIXDataStorage.CAT62Message[Start_Idx];

                        CAT62I060Types.CAT62060Mode3UserData Mode3AData = (CAT62I060Types.CAT62060Mode3UserData)Msg.CAT62DataItems[CAT62.ItemIDToIndex("060")].value;
                        // Get Lat/Long in decimal
                        GeoCordSystemDegMinSecUtilities.LatLongClass LatLongData = (GeoCordSystemDegMinSecUtilities.LatLongClass)Msg.CAT62DataItems[CAT62.ItemIDToIndex("105")].value;
                        double FlightLevel = (double)Msg.CAT62DataItems[CAT62.ItemIDToIndex("136")].value;
                        CAT62I380Types.CAT62I380Data CAT62I380Data = (CAT62I380Types.CAT62I380Data)Msg.CAT62DataItems[CAT62.ItemIDToIndex("380")].value;

                        
                        TargetType Target = new TargetType();
                        Target.ModeA = Mode3AData.Mode3A_Code;
                        Target.ModeC = FlightLevel.ToString();
                        if (CAT62I380Data != null)
                        {
                            if (CAT62I380Data.ACID.Is_Valid)
                                Target.ACID_Mode_S = CAT62I380Data.ACID.ACID_String;
                            else
                                Target.ACID_Mode_S = "N/A";

                            if (CAT62I380Data.TAS.Is_Valid)
                                Target.TAS = CAT62I380Data.TAS.TAS.ToString();
                            else
                                Target.TAS = "N/A";

                            if (CAT62I380Data.IAS.Is_Valid)
                                Target.IAS = CAT62I380Data.IAS.IAS.ToString();
                            else
                                Target.IAS = "N/A";

                            if (CAT62I380Data.MACH.Is_Valid)
                                Target.MACH = CAT62I380Data.MACH.MACH.ToString();
                            else
                                Target.MACH = "N/A";

                            if (CAT62I380Data.M_HDG.Is_Valid)
                                Target.M_HDG = CAT62I380Data.M_HDG.M_HDG.ToString();
                            else
                                Target.M_HDG = "N/A";

                            if (CAT62I380Data.TRK.Is_Valid)
                                Target.TRK = CAT62I380Data.TRK.TRK.ToString();
                            else
                                Target.TRK = "N/A";
                        }

                        Target.Lat = LatLongData.GetLatLongDecimal().LatitudeDecimal;
                        Target.Lon = LatLongData.GetLatLongDecimal().LongitudeDecimal;
                        CurrentTargetList.Add(Target);
                    }
                }
            }
            else
            {
                if (MainASTERIXDataStorage.CAT01Message.Count > 0)
                {
                    for (int Start_Idx = LastDataIndex; Start_Idx < MainASTERIXDataStorage.CAT01Message.Count; Start_Idx++)
                    {
                        LastDataIndex++;
                        MainASTERIXDataStorage.CAT01Data Msg = MainASTERIXDataStorage.CAT01Message[Start_Idx];

                        // Get Target Descriptor
                        CAT01I020UserData MyCAT01I020UserData = (CAT01I020UserData)Msg.CAT01DataItems[CAT01.ItemIDToIndex("020")].value;
                        // Get Mode3A
                        CAT01I070Types.CAT01070Mode3UserData Mode3AData = (CAT01I070Types.CAT01070Mode3UserData)Msg.CAT01DataItems[CAT01.ItemIDToIndex("070")].value;
                        // Get Lat/Long
                        CAT01I040Types.CAT01I040MeasuredPosInPolarCoordinates LatLongData = (CAT01I040Types.CAT01I040MeasuredPosInPolarCoordinates)Msg.CAT01DataItems[CAT01.ItemIDToIndex("040")].value;
                        // Get Flight Level
                        CAT01I090Types.CAT01I090FlightLevelUserData FlightLevelData = (CAT01I090Types.CAT01I090FlightLevelUserData)Msg.CAT01DataItems[CAT01.ItemIDToIndex("090")].value;

                        TargetType Target = new TargetType();
                        if (MyCAT01I020UserData.Type_Of_Radar_Detection == CAT01I020Types.Radar_Detection_Type.Primary)
                        {
                            Target.ModeA = "PSR";
                            Target.ModeC = "";
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            Target.MyMarker.Position = new PointLatLng(Target.Lat, Target.Lon);
                            PSRTargetList.Add(Target);
                        }
                        else if ((MyCAT01I020UserData.Type_Of_Radar_Detection != CAT01I020Types.Radar_Detection_Type.No_Detection) && (MyCAT01I020UserData.Type_Of_Radar_Detection != CAT01I020Types.Radar_Detection_Type.Unknown_Data))
                        {
                            Target.ModeA = Mode3AData.Mode3A_Code;
                            Target.ModeC = FlightLevelData.FlightLevel.ToString();
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            CurrentTargetList.Add(Target);
                        }
                    }
                }
                else if (MainASTERIXDataStorage.CAT48Message.Count > 0)
                {

                    for (int Start_Idx = LastDataIndex; Start_Idx < MainASTERIXDataStorage.CAT48Message.Count; Start_Idx++)
                    {
                        LastDataIndex++;

                        MainASTERIXDataStorage.CAT48Data Msg = MainASTERIXDataStorage.CAT48Message[Start_Idx];

                        CAT48I020UserData MyCAT48I020UserData = (CAT48I020UserData)Msg.CAT48DataItems[CAT48.ItemIDToIndex("020")].value;
                        CAT48I070Types.CAT48I070Mode3UserData Mode3AData = (CAT48I070Types.CAT48I070Mode3UserData)Msg.CAT48DataItems[CAT48.ItemIDToIndex("070")].value;
                        // Get Lat/Long in decimal
                        CAT48I040Types.CAT48I040MeasuredPosInPolarCoordinates LatLongData = (CAT48I040Types.CAT48I040MeasuredPosInPolarCoordinates)Msg.CAT48DataItems[CAT48.ItemIDToIndex("040")].value;
                        // Get Flight Level
                        CAT48I090Types.CAT48I090FlightLevelUserData FlightLevelData = (CAT48I090Types.CAT48I090FlightLevelUserData)Msg.CAT48DataItems[CAT48.ItemIDToIndex("090")].value;
                        // Get ACID data for Mode-S
                        CAT48I240Types.CAT48I240ACID_Data ACID_Mode_S = (CAT48I240Types.CAT48I240ACID_Data)Msg.CAT48DataItems[CAT48.ItemIDToIndex("240")].value;

                        TargetType Target = new TargetType();
                        if ((MyCAT48I020UserData.Type_Of_Report == CAT48I020Types.Type_Of_Report_Type.Single_PSR) ||
                            (MyCAT48I020UserData.Type_Of_Report == CAT48I020Types.Type_Of_Report_Type.Mode_S_Roll_Call_PSR))
                        {
                            Target.ModeA = "PSR";
                            Target.ModeC = "";
                            Target.ACID_Mode_S = "";
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            PSRTargetList.Add(Target);
                        }
                        else if ((MyCAT48I020UserData.Type_Of_Report != CAT48I020Types.Type_Of_Report_Type.No_Detection) &&
                         (MyCAT48I020UserData.Type_Of_Report != CAT48I020Types.Type_Of_Report_Type.Unknown_Data))
                        {
                            Target.ModeA = Mode3AData.Mode3A_Code;
                            Target.ModeC = FlightLevelData.FlightLevel.ToString();
                            if (ACID_Mode_S != null)
                            {
                                Target.ACID_Mode_S = ACID_Mode_S.ACID;
                            }
                            else
                            {
                                Target.ACID_Mode_S = "N/A";
                            }
                            Target.Lat = LatLongData.LatLong.GetLatLongDecimal().LatitudeDecimal;
                            Target.Lon = LatLongData.LatLong.GetLatLongDecimal().LongitudeDecimal;
                            CurrentTargetList.Add(Target);
                        }

                    }

                }
                else if (MainASTERIXDataStorage.CAT62Message.Count > 0)
                {

                    for (int Start_Idx = LastDataIndex; Start_Idx < MainASTERIXDataStorage.CAT62Message.Count; Start_Idx++)
                    {
                        LastDataIndex++;

                        MainASTERIXDataStorage.CAT62Data Msg = MainASTERIXDataStorage.CAT62Message[Start_Idx];

                        CAT62I060Types.CAT62060Mode3UserData Mode3AData = (CAT62I060Types.CAT62060Mode3UserData)Msg.CAT62DataItems[CAT62.ItemIDToIndex("060")].value;
                        // Get Lat/Long in decimal
                        GeoCordSystemDegMinSecUtilities.LatLongClass LatLongData = (GeoCordSystemDegMinSecUtilities.LatLongClass)Msg.CAT62DataItems[CAT62.ItemIDToIndex("105")].value;
                        double FlightLevel = (double)Msg.CAT62DataItems[CAT62.ItemIDToIndex("136")].value;
                        CAT62I380Types.CAT62I380Data CAT62I380Data = (CAT62I380Types.CAT62I380Data)Msg.CAT62DataItems[CAT62.ItemIDToIndex("380")].value;

                        TargetType Target = new TargetType();
                        Target.ModeA = Mode3AData.Mode3A_Code;
                        Target.ModeC = FlightLevel.ToString();
                        if (CAT62I380Data != null)
                        {
                            if (CAT62I380Data.ACID.Is_Valid)
                                Target.ACID_Mode_S = CAT62I380Data.ACID.ACID_String;
                            else
                                Target.ACID_Mode_S = "N/A";

                            if (CAT62I380Data.TAS.Is_Valid)
                                Target.TAS = CAT62I380Data.TAS.TAS.ToString();
                            else
                                Target.TAS = "N/A";

                            if (CAT62I380Data.IAS.Is_Valid)
                                Target.IAS = CAT62I380Data.IAS.IAS.ToString();
                            else
                                Target.IAS = "N/A";

                            if (CAT62I380Data.MACH.Is_Valid)
                                Target.MACH = Math.Round(CAT62I380Data.MACH.MACH, 2).ToString();
                            else
                                Target.MACH = "N/A";

                            if (CAT62I380Data.M_HDG.Is_Valid)
                                Target.M_HDG = Math.Round(CAT62I380Data.M_HDG.M_HDG).ToString();
                            else
                                Target.M_HDG = "N/A";

                            if (CAT62I380Data.TRK.Is_Valid)
                                Target.TRK = Math.Round(CAT62I380Data.TRK.TRK).ToString();
                            else
                                Target.TRK = "N/A";
                        }

                        Target.Lat = LatLongData.GetLatLongDecimal().LatitudeDecimal;
                        Target.Lon = LatLongData.GetLatLongDecimal().LongitudeDecimal;
                        CurrentTargetList.Add(Target);
                    }

                }
            }

            if (Return_Buffered == false)
            {
                UpdateGlobalList();
            }
            TargetList = CurrentTargetList;
        }
    }
}
