using System;
using System.Collections.Generic;
using System.IO;

namespace SatRTV
{
    public class Enigma
    {
        public int TransFrequencyDelta = 0;

        public class EnigmaConfig
        {
            public EnigmaConfig(string SatId_, string BouquetT_, string BouquetR_)
            {
                SatId = SatId_;
                BouquetT = BouquetT_;
                BouquetR = BouquetR_;
            }

            public string SatId;
            public string BouquetT;
            public string BouquetR;
        }

        public List<EnigmaConfig> EnigmaConfig_ = new List<EnigmaConfig>();


        class ItemTrans
        {
            public string[] Raw;
            public bool Include = false;
            public enum TransTypeDef { T, S_Ku, S_C, X };
            public TransTypeDef TransType = TransTypeDef.X;
            public int Freqency;
            public int SymbolRate;
            public enum PolarityDef { H, V, L, R, X }
            public PolarityDef Polarity = PolarityDef.X;
            public long SortKey;


            public ItemTrans(string[] Raw_)
            {
                Raw = Raw_;

                TransType = TransTypeDef.X;
                if (Raw[1].StartsWith("t "))
                {
                    TransType = TransTypeDef.T;
                    string[] Temp = Raw[1].Split(':');
                    if (Temp.Length != 12)
                    {
                        throw new Exception("Transponder error 2: " + Raw[0]);
                    }

                    // Transponder frequency
                    Freqency = int.Parse(Temp[0].Substring(2));
                }
                if (Raw[1].StartsWith("s "))
                {
                    TransType = TransTypeDef.S_Ku;
                    string[] Temp = Raw[1].Split(':');
                    if ((Temp.Length != 7) && (Temp.Length != 11) && (Temp.Length != 14))
                    {
                        throw new Exception("Transponder error 3: " + Raw[0]);
                    }

                    // Transponder frequency
                    Freqency = int.Parse(Temp[0].Substring(2));

                    // Transponder symbol rate
                    SymbolRate = int.Parse(Temp[1]);

                    // Transponder polarity as additional digit to frequency to force ordering
                    SortKey = Freqency * 2; 
                    if(Temp[2] == "0")
                    {
                        Polarity = PolarityDef.H;
                        SortKey = Freqency * 4 + 0;
                    }
                    if (Temp[2] == "1")
                    {
                        Polarity = PolarityDef.V;
                        SortKey = Freqency * 4 + 1;
                    }
                    if ((Temp[2] != "0") && (Temp[2] != "1"))
                    {
                        throw new Exception("Transponder error 4: " + Raw[0]);
                    }

                }
                if (Raw[1].StartsWith("c "))
                {
                    TransType = TransTypeDef.S_C;
                    string[] Temp = Raw[1].Split(':');
                    if (Temp.Length != 7)
                    {
                        throw new Exception("Transponder error 3: " + Raw[0]);
                    }

                    // Transponder frequency
                    Freqency = int.Parse(Temp[0].Substring(2));

                    // Transponder symbol rate
                    SymbolRate = int.Parse(Temp[1]);

                    // Transponder polarity as additional digit to frequency to force ordering
                    SortKey = Freqency * 2;
                    if (Temp[2] == "2")
                    {
                        Polarity = PolarityDef.L;
                        SortKey = Freqency * 4 + 2;
                    }
                    if (Temp[2] == "3")
                    {
                        Polarity = PolarityDef.R;
                        SortKey = Freqency * 4 + 3;
                    }
                    if ((Temp[2] != "2") && (Temp[2] != "3"))
                    {
                        throw new Exception("Transponder error 4: " + Raw[0]);
                    }

                }
                if (TransType == TransTypeDef.X)
                {
                    throw new Exception("Transponder error 1: " + Raw[0]);
                }
            }

            public void SetInclude(string[] SatId)
            {
                string[] Temp = Raw[1].Split(':');
                if (ArrayContains(SatId, Temp[4]))
                {
                    Include = true;
                }
                else
                {
                    Include = false;
                }
            }

            public void Print()
            {
                Console.WriteLine("Trans:" + Raw[0] + "|" + Raw[1] + "|" + Raw[2] + "| " + (Freqency / 1000).ToString() + " " + Polarity.ToString() + " " + (SymbolRate / 1000).ToString());
            }
        }

        class ItemChan
        {
            public string[] Raw;
            public int SID;
            public ItemTrans ItemTrans_ = null;
            public bool Include = false;
            public long SortKey;
            public string ServiceType;
            public int FreqSortKey;

            public ItemChan(string[] Raw_, List<ItemTrans> ItemTrans__)
            {
                Raw = Raw_;
                SID = int.Parse(Raw[0].Substring(0, 4), System.Globalization.NumberStyles.HexNumber);

                if (Raw[0].IndexOf(':') != 4)
                {
                    throw new Exception("Channel error 1: " + Raw[0]);
                }
                string TransId = Raw[0].Substring(5);
                ServiceType = Raw[0].Split(':')[4];


                // Search for transponder to get frequency
                int TransNum = -1;
                for (int i = 0; i < ItemTrans__.Count; i++)
                {
                    if (TransId.StartsWith(ItemTrans__[i].Raw[0] + ":"))
                    {
                        if (TransNum >= 0)
                        {
                            throw new Exception("Channel error 3: " + Raw[0]);
                        }
                        TransNum = i;
                    }
                }
                if (TransNum < 0)
                {
                    throw new Exception("Channel error 2: " + Raw[0]);
                }

                ItemTrans_ = ItemTrans__[TransNum];

                SortKey = ItemTrans_.SortKey * 1000000 + SID;
            }

            public void SetInclude()
            {
                Include = ItemTrans_.Include;
            }

            public void Print()
            {
                Console.WriteLine("Chan:" + Raw[0] + "|" + Raw[1] + "|" + ItemTrans_.Freqency + "|" + SID);
            }
        }

        List<ItemTrans> ListTrans = new List<ItemTrans>();
        List<ItemChan> ListChan = new List<ItemChan>();


        static bool ArrayContains(string[] Arr, string Val)
        {
            for (int i = 0; i < Arr.Length; i++)
            {
                if (Arr[i] == Val)
                {
                    return true;
                }
            }
            return false;
        }


        public void DatabaseSetInclude(string[] SatId)
        {
            for (int i = 0; i < ListTrans.Count; i++)
            {
                ListTrans[i].SetInclude(SatId);
            }
            for (int i = 0; i < ListChan.Count; i++)
            {
                ListChan[i].SetInclude();
            }
        }

        public void DatabaseLoad(string SatFileName)
        {
            ListTrans.Clear();
            ListChan.Clear();

            // Reading channels from database
            FileStream SatFS = new FileStream(SatFileName, FileMode.Open, FileAccess.Read);
            StreamReader SatSR = new StreamReader(SatFS);
            int I = 0;
            string Buf = "";
            while (Buf != null)
            {
                Buf = SatSR.ReadLine();
                if (Buf != null)
                {
                    if (I == 0)
                    {
                        if (Buf == "transponders")
                        {
                            I = 1;
                        }
                        if (Buf == "services")
                        {
                            I = 2;
                        }
                    }
                    else
                    {
                        if (Buf == "end")
                        {
                            I = 0;
                        }
                        if (I == 1)
                        {
                            string[] Data = new string[3];
                            Data[0] = Buf.Trim();
                            Data[1] = SatSR.ReadLine().Trim();
                            Data[2] = SatSR.ReadLine().Trim();
                            ListTrans.Add(new ItemTrans(Data));
                        }
                        if (I == 2)
                        {
                            string[] Data = new string[3];
                            Data[0] = Buf;
                            Data[1] = SatSR.ReadLine();
                            Data[2] = SatSR.ReadLine();
                            ListChan.Add(new ItemChan(Data, ListTrans));
                        }
                    }
                }
            }
            SatSR.Close();
            SatFS.Close();

        }

        public void TestPrint()
        {
            // Sort transponders
            List<int> ListTransSort = new List<int>();
            for (int i = 0; i < ListTrans.Count; i++)
            {
                ListTransSort.Add(i);
            }
            ListTransSort.Sort((a, b) => ListTrans[a].SortKey.CompareTo(ListTrans[b].SortKey));

            // Print transponders
            for (int i = 0; i < ListTransSort.Count; i++)
            {
                if (ListTrans[ListTransSort[i]].Include)
                {
                    //ListTrans[ListTransSort[i]].Print();
                }
            }

            // Sort channels
            List<int> ListChanSort = new List<int>();
            for (int i = 0; i < ListChan.Count; i++)
            {
                ListChanSort.Add(i);
            }
            ListChanSort.Sort((a, b) => ListChan[a].SortKey.CompareTo(ListChan[b].SortKey));

            // Print channels
            for (int i = 0; i < ListChanSort.Count; i++)
            {
                if (ListChan[ListChanSort[i]].Include)
                {
                    //ListChan[ListChanSort[i]].Print();
                }
            }

            // Print bouquet
            Console.WriteLine("{" + BouquetName + "}");
            for (int i = 0; i < ListBouquet.Count; i++)
            {
                //ListBouquet[i].Print();
            }
        }


        class ItemBouquet
        {
            public ItemChan ItemChan_;
            public ItemBouquet(ItemChan ItemChan__)
            {
                ItemChan_ = ItemChan__;
            }
            public ItemBouquet(string Raw_, List<ItemChan> ItemChan__)
            {
                string[] Temp = Raw_.Split(':');

                // 3 - PID
                // 6 - TransID 1
                // 4 - TransID 2
                // 5 - TransID 3
                string ItemId = Temp[3].ToLowerInvariant().PadLeft(4, '0');
                ItemId = ItemId + ":" + Temp[6].ToLowerInvariant().PadLeft(8, '0');
                ItemId = ItemId + ":" + Temp[4].ToLowerInvariant().PadLeft(4, '0');
                ItemId = ItemId + ":" + Temp[5].ToLowerInvariant().PadLeft(4, '0');

                int ChanId = -1;
                for (int i = 0; i < ItemChan__.Count; i++)
                {
                    if (ItemChan__[i].Raw[0].StartsWith(ItemId) && ItemChan__[i].Include)
                    {
                        if (ChanId >= 0)
                        {
                            throw new Exception("Bouquet error 2: " + Raw_);
                        }
                        ChanId = i;
                    }
                }
                if (ChanId <= 0)
                {
                    throw new Exception("Bouquet error 1: " + Raw_);
                }
                ItemChan_ = ItemChan__[ChanId];
            }

            public void Print()
            {
                ItemChan_.Print();
            }
        }
        List<ItemBouquet> ListBouquet = new List<ItemBouquet>();
        string BouquetName = "";
        string BouquetFile = "";

        public void BouquetClear()
        {
            ListBouquet.Clear();
        }

        public void BouquetLoad(string BouquetFileName, bool Loaditems)
        {
            BouquetFile = BouquetFileName;
            ListBouquet.Clear();
            FileStream BouFS = new FileStream(BouquetFileName, FileMode.Open, FileAccess.Read);
            StreamReader BouSR = new StreamReader(BouFS);
            string Buf = "";
            while (Buf != null)
            {
                Buf = BouSR.ReadLine();
                if (Buf != null)
                {
                    if (Buf.StartsWith("#NAME "))
                    {
                        BouquetName = Buf.Substring(6);
                    }
                    if (Buf.StartsWith("#SERVICE ") && Loaditems)
                    {
                        ListBouquet.Add(new ItemBouquet(Buf.Substring(9), ListChan));
                    }
                }
            }
            BouSR.Close();
            BouFS.Close();
        }

        public void BouquetSave()
        {
            FileStream BouFS = new FileStream(BouquetFile + ".txt", FileMode.Create, FileAccess.Write);
            StreamWriter BouSW = new StreamWriter(BouFS);
            BouSW.WriteLine("#NAME " + BouquetName);
            List<string> SavedItems = new List<string>();
            for (int i = 0; i < ListBouquet.Count; i++)
            {
                string Buf = "";
                Buf += "#SERVICE 1:0:";
                Buf += int.Parse(ListBouquet[i].ItemChan_.ServiceType).ToString("X");
                Buf += ":";
                Buf += ListBouquet[i].ItemChan_.SID.ToString("X");
                string[] Temp = ListBouquet[i].ItemChan_.ItemTrans_.Raw[0].Split(':');
                Buf += ":";
                while ((Temp[1].Length > 1) && (Temp[1].StartsWith("0")))
                {
                    Temp[1] = Temp[1].Substring(1);
                }
                Buf += Temp[1].ToUpperInvariant();
                Buf += ":";
                while ((Temp[2].Length > 1) && (Temp[2].StartsWith("0")))
                {
                    Temp[2] = Temp[2].Substring(1);
                }
                Buf += Temp[2].ToUpperInvariant();
                Buf += ":";
                while ((Temp[0].Length > 1) && (Temp[0].StartsWith("0")))
                {
                    Temp[0] = Temp[0].Substring(1);
                }
                Buf += Temp[0].ToUpperInvariant();
                Buf += ":0:0:0:";
                if (!SavedItems.Contains(Buf))
                {
                    BouSW.WriteLine(Buf);
                    SavedItems.Add(Buf);
                }
            }
            BouSW.Close();
            BouFS.Close();
        }

        public void BouquetSort()
        {
            ListBouquet.Sort((a, b) => a.ItemChan_.SortKey.CompareTo(b.ItemChan_.SortKey));
        }

        public bool BouquetCreateFromList(string ListFileName, bool TypeR, bool TypeTV, bool TypeIMG, bool TypeDATA)
        {
            FileStream ListFS = new FileStream(ListFileName, FileMode.Open, FileAccess.Read);
            StreamReader ListSR = new StreamReader(ListFS);
            string[] ListHeaders = ListSR.ReadLine().Split('\t');

            int ColumnFreq = -1;
            int ColumnPol = -1;
            int ColumnSID = -1;
            int ColumnType = -1;

            for (int i = 0; i < ListHeaders.Length; i++)
            {
                ListHeaders[i] = ListHeaders[i].ToUpperInvariant();
                switch (ListHeaders[i])
                {
                    case "FREQ": if (ColumnFreq < 0) { ColumnFreq = i; } break;
                    case "POL": if (ColumnPol < 0) { ColumnPol = i; } break;
                    case "SID": if (ColumnSID < 0) { ColumnSID = i; } break;
                    case "TYPE": if (ColumnType < 0) { ColumnType = i; } break;
                }
            }

            if ((ColumnFreq < 0) || (ColumnPol < 0) || (ColumnSID < 0) || (ColumnType < 0))
            {
                ListSR.Close();
                ListFS.Close();
                return false;
            }

            FileStream LogFS = new FileStream(BouquetFile + ".log", FileMode.Create, FileAccess.Write);
            StreamWriter LogFW = new StreamWriter(LogFS);
            LogFW.WriteLine("Freq\tPol\tSID\tType\tComment");

            string Buf = "";
            while (Buf != null)
            {
                Buf = ListSR.ReadLine();
                if (Buf != null)
                {
                    string[] ListData = Buf.Split('\t');
                    LogFW.Write(ListData[ColumnFreq]);
                    LogFW.Write("\t");
                    LogFW.Write(ListData[ColumnPol]);
                    LogFW.Write("\t");
                    LogFW.Write(ListData[ColumnSID]);
                    LogFW.Write("\t");
                    LogFW.Write(ListData[ColumnType]);
                    LogFW.Write("\t");

                    bool GoodType = false;
                    if (TypeR && (ListData[ColumnType] == "R")) { GoodType = true; }
                    if (TypeTV && (ListData[ColumnType] == "TV")) { GoodType = true; }
                    if (TypeIMG && (ListData[ColumnType] == "IMG")) { GoodType = true; }
                    if (TypeDATA && (ListData[ColumnType] == "DATA")) { GoodType = true; }

                    if (GoodType)
                    {
                        List<ItemChan> ChanTemp = new List<ItemChan>();
                        for (int i = 0; i < ListChan.Count; i++)
                        {
                            if (ListChan[i].Include)
                            {
                                if (ListChan[i].SID == int.Parse(ListData[ColumnSID]))
                                {
                                    int Freq1 = int.Parse(ListData[ColumnFreq]);
                                    int Freq2 = (ListChan[i].ItemTrans_.Freqency / 1000);
                                    if (Math.Abs(Freq1 - Freq2) <= TransFrequencyDelta)
                                    {
                                        if (ListData[ColumnPol] == ListChan[i].ItemTrans_.Polarity.ToString())
                                        {
                                            ListChan[i].FreqSortKey = Math.Abs(Freq1 - Freq2);
                                            ChanTemp.Add(ListChan[i]);
                                        }
                                    }
                                }
                            }
                        }
                        if (ChanTemp.Count < 1)
                        {
                            LogFW.Write("Channel not exists");
                        }
                        if (ChanTemp.Count > 1)
                        {
                            ChanTemp.Sort((a, b) => a.FreqSortKey.CompareTo(b.FreqSortKey));
                            if (ChanTemp[0].ItemTrans_.Freqency == ChanTemp[1].ItemTrans_.Freqency)
                            {
                                LogFW.Write("Ambiguous channel (" + ChanTemp.Count + ")");
                            }
                            else
                            {
                                while (ChanTemp.Count > 1)
                                {
                                    ChanTemp.RemoveAt(1);
                                }
                            }
                        }
                        if (ChanTemp.Count == 1)
                        {
                            LogFW.Write("Channel found: " + (ChanTemp[0].ItemTrans_.Freqency / 1000) + " " + ChanTemp[0].ItemTrans_.Polarity.ToString() + " " + ChanTemp[0].Raw[1]);
                            ListBouquet.Add(new ItemBouquet(ChanTemp[0]));
                        }
                    }
                    else
                    {
                        LogFW.Write("Type mismatch");
                    }
                    LogFW.WriteLine();

                }
            }


            LogFW.Close();
            LogFS.Close();


            ListSR.Close();
            ListFS.Close();
            return true;
        }
    }
}
