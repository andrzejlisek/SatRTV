using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        static void Main(string[] args)
        {
            AppCore Core = new AppCore();
            AppCoreSat[] AppCoreSat_ = new AppCoreSat[4];
            AppCoreSat_[0] = null;
            AppCoreSat_[1] = Core.CoreKingOfSat;
            AppCoreSat_[2] = Core.CoreLyngSat;
            AppCoreSat_[3] = Core.CoreFlySat;
            bool FTA = false;
            bool TransCh = true;
            bool ChanFilter1 = true;
            bool ChanFilter2 = true;
            bool ChanFilter3 = true;
            bool ChanFilter4 = true;

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
                if (FTA)
                {
                    Console.WriteLine("FTA channels");
                }
                else
                {
                    Console.WriteLine("All channels");
                }
                if (TransCh)
                {
                    Console.WriteLine("Transponders with channels");
                }
                else
                {
                    Console.WriteLine("All transponders");
                }
                Console.Write("Channel type: ");
                bool XC = false;
                if (ChanFilter1) { Console.Write("R"); XC = true; }
                if (ChanFilter2) { if (XC) { Console.Write(", "); XC = false; } Console.Write("TV"); XC = true; }
                if (ChanFilter3) { if (XC) { Console.Write(", "); XC = false; } Console.Write("IMG"); XC = true; }
                if (ChanFilter4) { if (XC) { Console.Write(", "); XC = false; } Console.Write("DATA"); }
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
                            FTA = !FTA;
                            GoodCmd = true;
                            break;
                        case "TRANSCH":
                            TransCh = !TransCh;
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
                                case "R": ChanFilter1 = !ChanFilter1; GoodCmd = true; break;
                                case "TV": ChanFilter2 = !ChanFilter2; GoodCmd = true; break;
                                case "IMG": ChanFilter3 = !ChanFilter3; GoodCmd = true; break;
                                case "DATA": ChanFilter4 = !ChanFilter4; GoodCmd = true; break;
                            }
                            break;
                        case "DOWNLOAD":
                            if ((CmdNum >= 1) && (CmdNum <= 3))
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
                            if ((CmdNum >= 1) && (CmdNum <= 3))
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
                            if ((CmdNum >= 1) && (CmdNum <= 3))
                            {
                                for (int i = 0; i < Core.SatName.Count; i++)
                                {
                                    if (Core.SatSelected[i])
                                    {
                                        Console.Write("Creating lists " + i.ToString() + "...");
                                        string ChanFilter = "|";
                                        if (ChanFilter1) { ChanFilter += "R|"; }
                                        if (ChanFilter2) { ChanFilter += "TV|"; }
                                        if (ChanFilter3) { ChanFilter += "IMG|"; }
                                        if (ChanFilter4) { ChanFilter += "DATA|"; }
                                        AppCoreSat_[CmdNum].CreateList(i, FTA, TransCh, ChanFilter, Core.Band1, Core.Band2, Core.Band3);
                                        Console.WriteLine("OK");
                                    }
                                }
                                Pause();
                                GoodCmd = true;
                            }
                            break;
                        case "TRANSIMG":
                            if ((CmdNum >= 1) && (CmdNum <= 3))
                            {
                                AppCoreSat_[CmdNum].CreateTransBitmap(Core.SatSelected, Core.Band1, Core.Band2, Core.Band3);
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
                    Console.WriteLine("SELECT N - Select or deselect satellite");
                    Console.WriteLine("BAND N - Select or deselect band");
                    Console.WriteLine("FTA - Toggle between FTA and all channels");
                    Console.WriteLine("TRANSCH - Toggle between transponder with channels and all transponders");
                    Console.WriteLine("TYPE R/TV/IMG/DATA - Include or exclude channel type");
                    Console.WriteLine("DOWNLOAD S - Download channel data from S");
                    Console.WriteLine("PARSE S - Parse downloaded data from S");
                    Console.WriteLine("LIST S - Create lists of transponder and channels from S");
                    Console.WriteLine("TRANSIMG S - Visualize transponder frequencies in picture file");
                    Console.WriteLine("EXIT - Exit application");
                    Console.WriteLine();
                    Pause();
                }
            }
            Console.Clear();
        }
    }
}
