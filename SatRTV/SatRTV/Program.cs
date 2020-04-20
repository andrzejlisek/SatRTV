/*
 * Created by SharpDevelop.
 * User: XXX
 * Date: 2020-03-23
 * Time: 11:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SatRTV
{
	class Program
	{
        static void Pause()
        {
            Console.Write("Press any key to continue");
            Console.ReadKey();
            Console.WriteLine();
        }
        
        public static void Main(string[] args)
        {
            AppCore Core = new AppCore();
            AppCoreSat[] AppCoreSat_ = new AppCoreSat[5];
            AppCoreNo AppCoreNo_ = Core.AppCoreNo_;
            AppCoreSat_[0] = null;
            AppCoreSat_[1] = Core.CoreKingOfSat;
            AppCoreSat_[2] = Core.CoreLyngSat;
            AppCoreSat_[3] = Core.CoreFlySat;
            AppCoreSat_[4] = Core.CoreSatBeams;
            AppCoreSat_[1].SetListFields(Core.ListTransFields, Core.ListChanFields);
            AppCoreSat_[2].SetListFields(Core.ListTransFields, Core.ListChanFields);
            AppCoreSat_[3].SetListFields(Core.ListTransFields, Core.ListChanFields);
            AppCoreSat_[4].SetListFields(Core.ListTransFields, Core.ListChanFields);

            bool Work = true;
            while (Work)
            {
                Console.Clear();
                for (int i = 0; i < Core.SatName.Count; i++)
                {
                    if (i < 10)
                    {
                        Console.Write(" ");
                    }
                    Console.Write(i.ToString());
                    if (Core.SatSelected[i])
                    {
                        Console.Write(" # ");
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                    Console.WriteLine(Core.SatName[i]);
                }
                Console.WriteLine();
                Console.WriteLine("Band 1 ( 3400MHz -  4200MHz) - " + (Core.Band1 ? "yes" : "no"));
                Console.WriteLine("Band 2 (10700MHz - 12750MHz) - " + (Core.Band2 ? "yes" : "no"));
                Console.WriteLine("Band 3 (18200MHz - 22200MHz) - " + (Core.Band3 ? "yes" : "no"));
                Console.WriteLine();
                if (Core.FTA)
                {
                    Console.WriteLine("FTA channels");
                }
                else
                {
                    Console.WriteLine("All channels");
                }
                if (Core.TransCh)
                {
                    Console.WriteLine("Transponders with channels");
                }
                else
                {
                    Console.WriteLine("All transponders");
                }
                Console.Write("Channel type: ");
                bool XC = false;
                if (Core.ChanFilter1) { Console.Write("R"); XC = true; }
                if (Core.ChanFilter2) { if (XC) { Console.Write(", "); XC = false; } Console.Write("TV"); XC = true; }
                if (Core.ChanFilter3) { if (XC) { Console.Write(", "); XC = false; } Console.Write("IMG"); XC = true; }
                if (Core.ChanFilter4) { if (XC) { Console.Write(", "); XC = false; } Console.Write("DATA"); }
                Console.WriteLine();

                string Cmd = Console.ReadLine();

                Cmd = Cmd.Trim();
                while (Cmd != Cmd.Replace("  ", " "))
                {
                    Cmd = Cmd.Replace("  ", " ");
                }
                string[] CmdX = Cmd.Split(' ');
                bool GoodCmd = false;
                if (CmdX.Length == 1)
                {
                    switch (CmdX[0].ToUpperInvariant())
                    {
                        case "FTA":
                            Core.FTA = !Core.FTA;
                            GoodCmd = true;
                            break;
                        case "TRANSCH":
                            Core.TransCh = !Core.TransCh;
                            GoodCmd = true;
                            break;
                        case "EXIT":
                            Work = false;
                            GoodCmd = true;
                            break;
                    }
                }
                if (CmdX.Length == 2)
                {
                    int CmdNum = AppCoreSat.ToInt(CmdX[1]);
                    switch (CmdX[0].ToUpperInvariant())
                    {
                        case "SELECT":
                            if ((CmdNum >= 0) && (CmdNum < Core.SatSelected.Count))
                            {
                                Core.SatSelected[CmdNum] = !Core.SatSelected[CmdNum];
                                GoodCmd = true;
                            }
                            break;
                        case "BAND":
                            if (CmdNum == 1)
                            {
                                Core.Band1 = !Core.Band1;
                                GoodCmd = true;
                            }
                            if (CmdNum == 2)
                            {
                                Core.Band2 = !Core.Band2;
                                GoodCmd = true;
                            }
                            if (CmdNum == 3)
                            {
                                Core.Band3 = !Core.Band3;
                                GoodCmd = true;
                            }
                            break;
                        case "TYPE":
                            string ChT = CmdX[1].ToUpperInvariant();
                            switch (ChT)
                            {
                                case "R": Core.ChanFilter1 = !Core.ChanFilter1; GoodCmd = true; break;
                                case "TV": Core.ChanFilter2 = !Core.ChanFilter2; GoodCmd = true; break;
                                case "IMG": Core.ChanFilter3 = !Core.ChanFilter3; GoodCmd = true; break;
                                case "DATA": Core.ChanFilter4 = !Core.ChanFilter4; GoodCmd = true; break;
                            }
                            break;
                        case "DOWNLOAD":
                            if ((CmdNum >= 1) && (CmdNum < AppCoreSat_.Length))
                            {
                                for (int i = 0; i < Core.SatName.Count; i++)
                                {
                                    if (Core.SatSelected[i])
                                    {
                                        Console.Write("Downloading " + i.ToString() + "...");
                                        AppCoreSat_[CmdNum].Download(i);
                                        Console.WriteLine("OK");
                                    }
                                }
                                Pause();
                                GoodCmd = true;
                            }
                            break;
                        case "PARSE":
                            if ((CmdNum >= 1) && (CmdNum < AppCoreSat_.Length))
                            {
                                for (int i = 0; i < Core.SatName.Count; i++)
                                {
                                    if (Core.SatSelected[i])
                                    {
                                        Console.Write("Parsing " + i.ToString() + "...");
                                        AppCoreSat_[CmdNum].Parse(i, true);
                                        Console.WriteLine("OK");
                                    }
                                }
                                Pause();
                                GoodCmd = true;
                            }
                            break;
                        case "LIST":
                            if ((CmdNum >= 1) && (CmdNum < AppCoreSat_.Length))
                            {
                                for (int i = 0; i < Core.SatName.Count; i++)
                                {
                                    if (Core.SatSelected[i])
                                    {
                                        Console.Write("Creating lists " + i.ToString() + "...");
                                        string ChanFilter = "|";
                                        if (Core.ChanFilter1) { ChanFilter += "R|"; }
                                        if (Core.ChanFilter2) { ChanFilter += "TV|"; }
                                        if (Core.ChanFilter3) { ChanFilter += "IMG|"; }
                                        if (Core.ChanFilter4) { ChanFilter += "DATA|"; }
                                        AppCoreSat_[CmdNum].CreateList(i, Core.FTA, Core.TransCh, ChanFilter, Core.Band1, Core.Band2, Core.Band3);
                                        Console.WriteLine("OK");
                                    }
                                }
                                Pause();
                                GoodCmd = true;
                            }
                            break;
                        case "TRANSIMG":
                            if ((CmdNum >= 1) && (CmdNum < AppCoreSat_.Length))
                            {
                                AppCoreSat_[CmdNum].CreateTransBitmap(Core.SatSelected, Core.Band1, Core.Band2, Core.Band3);
                                Pause();
                                GoodCmd = true;
                            }
                            break;
                        case "TRANSNO":
                            {
                                AppCoreNo_.TransNo(CmdX[1], Core.SatSelected);
                                Pause();
                                GoodCmd = true;
                            }
                            break;
                        case "CHANNO":
                            {
                                AppCoreNo_.ChanNo(CmdX[1], Core.SatSelected);
                                Pause();
                                GoodCmd = true;
                            }
                            break;
                    }
                }
                if (!GoodCmd)
                {
                    Console.WriteLine();
                    Console.WriteLine("S=1 - KingOfSat");
                    Console.WriteLine("S=2 - LyngSat");
                    Console.WriteLine("S=3 - FlySat");
                    Console.WriteLine("S=4 - SatBeams");
                    Console.WriteLine("SELECT N - Select or deselect satellite");
                    Console.WriteLine("BAND N - Select or deselect band");
                    Console.WriteLine("FTA - Toggle between FTA and all channels");
                    Console.WriteLine("TRANSCH - Toggle between transponder with channels and all transponders");
                    Console.WriteLine("TYPE R/TV/IMG/DATA - Include or exclude channel type");
                    Console.WriteLine("DOWNLOAD S - Download channel data from S");
                    Console.WriteLine("PARSE S - Parse downloaded data from S");
                    Console.WriteLine("LIST S - Create lists of transponder and channels from S");
                    Console.WriteLine("TRANSIMG S - Visualize transponder frequencies in picture file");
                    Console.WriteLine("TRANSNO S - Create numbered transponder list from multiple sources");
                    Console.WriteLine("CHANNO S - Create numbered channel list from multiple sources");
                    Console.WriteLine("EXIT - Exit application");
                    Console.WriteLine();
                    Pause();
                }
            }
            Console.Clear();
        }
	}
}