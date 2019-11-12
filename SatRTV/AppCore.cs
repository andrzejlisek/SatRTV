using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatRTV
{
    class AppCore
    {
        public static double String2Double(string S)
        {
            return double.Parse(S, CultureInfo.InvariantCulture);
        }

        public static string ApplicationDirectory()
        {
            string AppDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            if (AppDir[AppDir.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                AppDir = AppDir + System.IO.Path.DirectorySeparatorChar;
            }
            return AppDir;
        }



        public AppCoreSat_1KingOfSat CoreKingOfSat;
        public AppCoreSat_2LyngSat CoreLyngSat;
        public AppCoreSat_3FlySat CoreFlySat;
        public int SatCount;
        public List<string> SatName = new List<string>();
        public List<bool> SatSelected = new List<bool>();
        public bool Band1 = true;
        public bool Band2 = true;
        public bool Band3 = true;

        public AppCore()
        {
            CoreKingOfSat = new AppCoreSat_1KingOfSat();
            CoreLyngSat = new AppCoreSat_2LyngSat();
            CoreFlySat = new AppCoreSat_3FlySat();
            ConfigFile CF = new ConfigFile();
            CF.FileLoad(@"Config.txt");
            CoreKingOfSat.TempDir = ApplicationDirectory() + "Data1" + System.IO.Path.DirectorySeparatorChar;
            CoreLyngSat.TempDir = ApplicationDirectory() + "Data2" + System.IO.Path.DirectorySeparatorChar;
            CoreFlySat.TempDir = ApplicationDirectory() + "Data3" + System.IO.Path.DirectorySeparatorChar;


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
