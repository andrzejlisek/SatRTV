﻿/*
 * Created by SharpDevelop.
 * User: XXX
 * Date: 2020-03-23
 * Time: 11:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SatRTV
{
	/// <summary>
	/// Description of AppCore.
	/// </summary>
	public class AppCore
	{
        public static double String2Double(string S)
        {
            return double.Parse(S, CultureInfo.InvariantCulture);
        }

        public static string ApplicationDirectory()
        {
            string AppDir = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
            if (AppDir[AppDir.Length - 1] != Path.DirectorySeparatorChar)
            {
                AppDir = AppDir + Path.DirectorySeparatorChar;
            }
            return AppDir;
        }

        public AppCoreSat_1KingOfSat CoreKingOfSat;
        public AppCoreSat_2LyngSat CoreLyngSat;
        public AppCoreSat_3FlySat CoreFlySat;
        public AppCoreSat_4SatBeams CoreSatBeams;
        public AppCoreNo AppCoreNo_;
        public int SatCount;
        public List<string> SatName = new List<string>();
        public List<bool> SatSelected = new List<bool>();
        public bool Band1 = true;
        public bool Band2 = true;
        public bool Band3 = true;

        public bool FTA = false;
        public bool TransCh = false;
        public bool ChanFilter1 = true;
        public bool ChanFilter2 = true;
        public bool ChanFilter3 = true;
        public bool ChanFilter4 = true;

        public string[] ListTransFields = null;
        public string[] ListChanFields = null;

        public AppCore()
        {
            CoreKingOfSat = new AppCoreSat_1KingOfSat();
            CoreLyngSat = new AppCoreSat_2LyngSat();
            CoreFlySat = new AppCoreSat_3FlySat();
            CoreSatBeams = new AppCoreSat_4SatBeams();
            AppCoreNo_ = new AppCoreNo();
            ConfigFile CF = new ConfigFile();
            CF.FileLoad(ApplicationDirectory() + "Config.txt");
            CoreKingOfSat.TempDir = ApplicationDirectory() + "Data1" + Path.DirectorySeparatorChar;
            CoreLyngSat.TempDir = ApplicationDirectory() + "Data2" + Path.DirectorySeparatorChar;
            CoreFlySat.TempDir = ApplicationDirectory() + "Data3" + Path.DirectorySeparatorChar;
            CoreSatBeams.TempDir = ApplicationDirectory() + "Data4" + Path.DirectorySeparatorChar;
            AppCoreNo_.TempDir = ApplicationDirectory();

            CF.ParamGet("SetBand1", ref Band1);
            CF.ParamGet("SetBand2", ref Band2);
            CF.ParamGet("SetBand3", ref Band3);

            CF.ParamGet("SetTypeR", ref ChanFilter1);
            CF.ParamGet("SetTypeTV", ref ChanFilter2);
            CF.ParamGet("SetTypeIMG", ref ChanFilter3);
            CF.ParamGet("SetTypeDATA", ref ChanFilter4);

            CF.ParamGet("SetFTA", ref FTA);
            CF.ParamGet("SetTransWithChan", ref TransCh);

            ListTransFields = CF.ParamGetS("SetTransFields").Split('|');
            ListChanFields = CF.ParamGetS("SetChanFields").Split('|');

            AppCoreNo_.TransNoFields = CF.ParamGetS("SetTransNoListFields").Split('|');
            AppCoreNo_.TransNoMode = CF.ParamGetS("SetTransNoListMode").Split('|');
            AppCoreNo_.ChanNoFields = CF.ParamGetS("SetChanNoListFields").Split('|');
            AppCoreNo_.ChanNoMode = CF.ParamGetS("SetChanNoListMode").Split('|');

            SatCount = 0;
            while (CF.ParamGetS("Sat" + SatCount.ToString() + "Name") != "")
            {
                SatCount++;
            }
            int SpanCount;
            bool SpanCountWork;
            for (int i = 0; i < SatCount; i++)
            {
                SatName.Add(CF.ParamGetS("Sat" + i.ToString() + "Name"));
                SatSelected.Add(CF.ParamGetB("Sat" + i.ToString() + "Selected"));
                CoreKingOfSat.SatAddr.Add(CF.ParamGetS("Sat" + i.ToString() + "KingOfSat"));
                CoreLyngSat.SatAddr.Add(CF.ParamGetS("Sat" + i.ToString() + "LyngSat"));
                CoreFlySat.SatAddr.Add(CF.ParamGetS("Sat" + i.ToString() + "FlySat"));
                CoreSatBeams.SatAddr.Add(CF.ParamGetS("Sat" + i.ToString() + "SatBeams"));
                CoreLyngSat.SpanChange.Add(new Dictionary<int, int>());
                CoreFlySat.SpanChange.Add(new Dictionary<int, int>());

                SpanCountWork = true;
                SpanCount = 0;
                while (SpanCountWork)
	            {
                    int XRow = CF.ParamGetI("Sat" + i.ToString() + "LyngSatRowSpan" + SpanCount.ToString() + "Row", -1);
                    int XVal = CF.ParamGetI("Sat" + i.ToString() + "LyngSatRowSpan" + SpanCount.ToString() + "Val", 0);
                    if (XRow >= 0)
                    {
                        CoreLyngSat.SpanChange[i].Add(XRow, XVal);
                        SpanCount++;
                    }
                    else
                    {
                        SpanCountWork = false;
                    }
	            }

                SpanCountWork = true;
                SpanCount = 0;
                while (SpanCountWork)
                {
                    int XRow = CF.ParamGetI("Sat" + i.ToString() + "FlySatRowSpan" + SpanCount.ToString() + "Row", -1);
                    int XVal = CF.ParamGetI("Sat" + i.ToString() + "FlySatRowSpan" + SpanCount.ToString() + "Val", 0);
                    if (XRow >= 0)
                    {
                        CoreFlySat.SpanChange[i].Add(XRow, XVal);
                        SpanCount++;
                    }
                    else
                    {
                        SpanCountWork = false;
                    }
                }
            }
        }
	}
}
