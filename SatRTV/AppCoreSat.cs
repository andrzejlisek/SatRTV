using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SatRTV
{
    class AppCoreSat
    {
        private string PolOrder = "HVRL";

        public static int ToInt(string S)
        {
            int N = 0;
            if (int.TryParse(S, out N))
            {
                return N;
            }
            else
            {
                return 0;
            }
        }

        public static string ClearInt(string S)
        {
            string X = "";
            if (S.Length == 0)
            {
                return "0";
            }
            for (int i = 0; i < S.Length; i++)
            {
                if ((S[i] >= '0') && (S[i] <= '9'))
                {
                    X = X + S[i].ToString();
                }
            }
            return X;
        }

        public string TempDir;
        public List<string> SatAddr = new List<string>();

        public string DataFileName(int N)
        {
            return TempDir + "Data" + N.ToString() + ".html";
        }

        public string TransFileName(int N)
        {
            return TempDir + "TransData" + N.ToString() + ".txt";
        }

        public string ChanFileName(int N)
        {
            return TempDir + "ChanData" + N.ToString() + ".txt";
        }

        public string BeamFileName(int N)
        {
            return TempDir + "Beam" + N.ToString() + ".txt";
        }

        public string TransListFileName(int N)
        {
            return TempDir + "TransList" + N.ToString() + ".txt";
        }

        public string ChanListFileName(int N)
        {
            return TempDir + "ChanList" + N.ToString() + ".txt";
        }

        public string TransImgFileName(int N)
        {
            return TempDir + "TransFreq" + N.ToString() + ".png";
        }

        /// <summary>
        /// Download HTML data
        /// </summary>
        /// <param name="I"></param>
        public void Download(int I)
        {
            Directory.CreateDirectory(TempDir);
            File.Delete(DataFileName(I));
            if (SatAddr[I] == "")
            {
                return;
            }
            WebClient WC = new WebClient();
            //WC.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");
            //WC.Headers.Add("Accept-Language", "en-US,en-GB;q=0.7,en;q=0.3");
            FileStream HFS_ = new FileStream(DataFileName(I), FileMode.Create, FileAccess.Write);
            byte[] RAWX = WC.DownloadData(SatAddr[I]);
            HFS_.Write(RAWX, 0, RAWX.Length);
            HFS_.Close();
        }

        /// <summary>
        /// Base parse function - in each subclass this function is overrided
        /// </summary>
        /// <param name="I"></param>
        /// <param name="TypeByPID"></param>
        public virtual void Parse(int I, bool TypeByPID)
        {
        }

        /// <summary>
        /// Separator character
        /// </summary>
        public string Separator = "|";

        /// <summary>
        /// Convert text string to text to tabbed file
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        protected string Prepare(string X)
        {
            int T = X.IndexOf("&#");
            while (T >= 0)
            {
                int TT = X.IndexOf(";", T);
                string CoX = X.Substring(T, TT - T + 1);
                string Co = X.Substring(T + 2, TT - T - 2);

                char Chr = (char)int.Parse(Co);
                Chr.ToString();

                X = X.Replace(CoX, Chr.ToString());

                T = X.IndexOf("&#");
            }

            X = X.Replace(Separator, " ");

            if (X.Contains(Separator))
            {
                throw new Exception("XXX");
            }
            return X.Replace("\t", " ").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("&nbsp;", " ").Trim();
        }


        public void SetListFields(string[] FieldsT, string[] FieldsC)
        {
            ListTransFields = FieldsT;
            ListChanFields = FieldsC;
        }

        public string[] ListTransFields = null;
        public string[] ListChanFields = null;


        /// <summary>
        /// Create ordered list of transponders and channels based on parsed data
        /// </summary>
        /// <param name="I"></param>
        /// <param name="FTA"></param>
        /// <param name="TypeFilter"></param>
        /// <param name="Band1"></param>
        /// <param name="Band2"></param>
        /// <param name="Band3"></param>
        public void CreateList(int I, bool FTA, bool TransCh, string TypeFilter, bool Band1, bool Band2, bool Band3)
        {
            // Load beam list
            BeamList.Clear();
            FileStream BLS = new FileStream(BeamFileName(I), FileMode.Open, FileAccess.Read);
            StreamReader BLR = new StreamReader(BLS);
            string BLX = BLR.ReadLine();
            while (BLX != null)
            {
                BeamListAdd(BLX);
                BLX = BLR.ReadLine();
            }
            BLR.Close();
            BLS.Close();
            BeamList.Sort();

            // Open parsed channel file
            List<object[]> ChanList = new List<object[]>();
            FileStream FS1 = new FileStream(ChanFileName(I), FileMode.Open, FileAccess.Read);
            StreamReader SR1 = new StreamReader(FS1);

            // Ommit header
            SR1.ReadLine();

            // Set fields varying between sources
            int IdxSID = 0;
            int IdxFTA = 0;
            int IdxLang = 0;
            if (this is AppCoreSat_1KingOfSat)
            {
                IdxSID = 10;
                IdxFTA = 17;
                IdxLang = 12;
            }
            if (this is AppCoreSat_2LyngSat)
            {
                IdxSID = 8;
                IdxFTA = 6;
                IdxLang = 10;
            }
            if (this is AppCoreSat_3FlySat)
            {
                IdxSID = 9;
                IdxFTA = 6;
                IdxLang = 8;
            }

            // Load file into list
            string Buf = SR1.ReadLine();
            while (Buf != null)
            {
                object[] Tab = new object[11];
                string[] Raw = Buf.Split('\t');

                // Frequency
                Tab[0] = (int)Math.Round((AppCore.String2Double(Raw[0])));

                // Polarization
                Tab[1] = Raw[1].ToUpperInvariant();

                // Beam
                Tab[2] = Raw[3];

                // SID
                if ((Raw[IdxSID] == "") || (Raw[IdxSID] == "-") || (Raw[IdxSID] == "NO-SID"))
                {
                    Raw[IdxSID] = "0";
                }
                Tab[3] = Raw[IdxSID];

                // Type
                Tab[4] = Raw[4];

                // Name
                Tab[5] = Raw[5];

                // Lang
                Tab[6] = Raw[IdxLang];

                // FTA
                Tab[7] = Raw[IdxFTA];

                // SortKey
                Tab[10] = GetSortKey((int)Tab[0], (string)Tab[1], (string)Tab[2], (string)Tab[3]);

                bool Good = TypeFilter.Contains("|" + Tab[4] + "|");
                if (FTA && (((string)Tab[6]) != "Yes"))
                {
                    Good = false;
                }

                if (!IsFreqAllowed((int)Tab[0], Band1, Band2, Band3))
                {
                    Good = false;
                }

                if (!BeamList.Contains((string)Tab[2]))
                {
                    Good = false;
                }

                if (Good)
                {
                    ChanList.Add(Tab);
                }

                Buf = SR1.ReadLine();
            }

            // Close channel parsed file
            SR1.Close();
            FS1.Close();

            // Sort channel list by frequency, polarization, beam and SID
            int ChanListCount = ChanList.Count;
            for (int i = 0; i < ChanListCount; i++)
            {
                for (int ii = 0; ii < ChanListCount; ii++)
                {
                    if (((long)ChanList[ii][10]) > ((long)ChanList[i][10]))
                    {
                        object[] Temp = ChanList[ii];
                        ChanList[ii] = ChanList[i];
                        ChanList[i] = Temp;
                    }
                }
            }


            // Create channel list file
            File.Delete(ChanListFileName(I));
            FileStream FS2 = new FileStream(ChanListFileName(I), FileMode.Create, FileAccess.Write);
            StreamWriter SR2 = new StreamWriter(FS2);

            // Write header
            for (int i = 0; i < ListChanFields.Length; i++)
            {
                if (i > 0)
                {
                    SR2.Write("\t");
                }
                SR2.Write(ListChanFields[i]);
            }
            SR2.WriteLine();

            // Write channel list
            for (int i = 0; i < ChanList.Count; i++)
            {
                object[] Tab = ChanList[i];
                double Freq = ((int)Tab[0]);

                int FreqI = (int)Math.Round(Freq);
                Tab[0] = FreqI.ToString();

                string LangX = (string)Tab[6];
                string LangXX = "";
                bool InLang = false;
                for (int ii = 0; ii < LangX.Length; ii++)
                {
                    if (LangX[ii] == '[')
                    {
                        InLang = true;
                        LangXX = LangXX + "[";
                    }
                    else
                    {
                        if (LangX[ii] == ']')
                        {
                            InLang = false;
                            LangXX = LangXX + "]";
                        }
                        else
                        {
                            if (InLang)
                            {
                                LangXX = LangXX + LangX[ii].ToString();
                            }
                        }
                    }
                }

                LangXX = LangXX.ToLowerInvariant().Replace("[---]", "").Replace("][", "[").Replace("]", "[");
                string[] LangXX_ = LangXX.Split('[');
                List<string> LangXX__ = new List<string>();
                for (int ii = 0; ii < LangXX_.Length; ii++)
                {
                    if (!LangXX__.Contains(LangXX_[ii]))
                    {
                        if (LangXX_[ii] != "")
                        {
                            LangXX__.Add(LangXX_[ii]);
                        }
                    }
                }

                LangXX__.Sort();
                string LangXX___ = "";
                for (int ii = 0; ii < LangXX__.Count; ii++)
                {
                    LangXX___ = LangXX___ + "[" + LangXX__[ii] + "]";
                }
                Tab[6] = LangXX___;

                for (int ii = 0; ii < ListChanFields.Length; ii++)
                {
                    if (ii > 0)
                    {
                        SR2.Write("\t");
                    }
                    switch (ListChanFields[ii])
                    {
                        case "Freq": SR2.Write((string)Tab[0]); break;
                        case "Pol": SR2.Write((string)Tab[1]); break;
                        case "Beam": SR2.Write((string)Tab[2]); break;
                        case "SID": SR2.Write((string)Tab[3]); break;
                        case "Type": SR2.Write((string)Tab[4]); break;
                        case "Name": SR2.Write((string)Tab[5]); break;
                        case "Lang": SR2.Write((string)Tab[6]); break;
                        case "FTA": SR2.Write((string)Tab[7]); break;
                    }
                }
                SR2.WriteLine();
            }

            // Close channel list file
            SR2.Close();
            FS2.Close();


            // Open parsed transponder file
            List<object[]> TransList = new List<object[]>();
            FS1 = new FileStream(TransFileName(I), FileMode.Open, FileAccess.Read);
            SR1 = new StreamReader(FS1);

            // Ommit header
            SR1.ReadLine();

            // Set fields varying between sources
            int IdxFreq = 0;
            int IdxPol = 0;
            int IdxSR = 0;
            int IdxTxp = 0;
            int IdxBeam = 0;
            if (this is AppCoreSat_1KingOfSat)
            {
                IdxFreq = 1;
                IdxPol = 2;
                IdxSR = 7;
                IdxTxp = 3;
                IdxBeam = 4;
            }
            if (this is AppCoreSat_2LyngSat)
            {
                IdxFreq = 0;
                IdxPol = 1;
                IdxSR = 7;
                IdxTxp = 2;
                IdxBeam = 3;
            }
            if (this is AppCoreSat_3FlySat)
            {
                IdxFreq = 3;
                IdxPol = 4;
                IdxSR = 6;
                IdxTxp = 0;
                IdxBeam = 10;
            }

            // Load file into list
            Buf = SR1.ReadLine();
            while (Buf != null)
            {
                object[] Tab = new object[20];
                string[] Raw = Buf.Split('\t');

                // Frequency
                Tab[0] = (int)Math.Round((AppCore.String2Double(Raw[IdxFreq])));
                if (((int)Tab[0]) < 0)
                {
                    throw new Exception("Negative frequency [" + Tab[0].ToString() + "]");
                }

                // Polarization
                Tab[1] = Raw[IdxPol].ToUpperInvariant();

                // SR/FEC
                Tab[2] = Raw[IdxSR].Replace("-", " ");

                // Txp
                Tab[4] = Raw[IdxTxp];

                // Beam
                Tab[5] = Raw[IdxBeam];

                // Sort key
                Tab[6] = GetSortKey((int)Tab[0], (string)Tab[1], (string)Tab[5], "");

                bool Good = true;

                if (!IsFreqAllowed((int)Tab[0], Band1, Band2, Band3))
                {
                    Good = false;
                }

                if (!BeamList.Contains((string)Tab[5]))
                {
                    Good = false;
                }

                if (Good)
                {
                    TransList.Add(Tab);
                }


                Buf = SR1.ReadLine();
            }

            // Close parsed transponder file
            SR1.Close();
            FS1.Close();

            // Merge repeated transponders - the same frequency, the same polarization and the same beam means the same transponder
            int TransListCount = TransList.Count;
            for (int i = 0; i < TransListCount; i++)
            {
                for (int j = (i + 1); j < TransListCount; j++)
                {
                    if ((((int)TransList[i][0]) == ((int)TransList[j][0])) && (((string)TransList[i][1]) == ((string)TransList[j][1])) && (((string)TransList[i][5]) == ((string)TransList[j][5])))
                    {
                        string SR_I = (string)TransList[i][2];
                        string SR_J = (string)TransList[j][2];
                        if (SR_J != "")
                        {
                            if (SR_I != "")
                            {
                                TransList[i][2] = SR_I + Separator + SR_J;
                            }
                            else
                            {
                                TransList[i][2] = SR_J;
                            }
                        }

                        for (int k = 4; k <= 4; k++)
                        {
                            SR_I = (string)TransList[i][k];
                            SR_J = (string)TransList[j][k];
                            if (SR_J != "")
                            {
                                if ((SR_I != "") && (!("|" + SR_I + "|").Contains("|" + SR_J + "|")))
                                {
                                    TransList[i][k] = SR_I + Separator + SR_J;
                                }
                                else
                                {
                                    TransList[i][k] = SR_J;
                                }
                            }
                        }

                        TransList[j][0] = -1;
                        TransList[j][2] = "";
                        TransList[j][4] = "";
                    }
                }
            }

            for (int i = (TransListCount - 1); i >= 0; i--)
            {
                if (((int)TransList[i][0]) < 0)
                {
                    TransList.RemoveAt(i);
                    TransListCount--;
                }
            }

            // Sort transponder list by frequency and polarization
            for (int i = 0; i < TransListCount; i++)
            {
                for (int ii = 0; ii < TransListCount; ii++)
                {
                    if (((long)TransList[ii][6]) > ((long)TransList[i][6]))
                    {
                        object[] Temp = TransList[ii];
                        TransList[ii] = TransList[i];
                        TransList[i] = Temp;
                    }
                }
            }


            // Create transponder list file
            File.Delete(TransListFileName(I));
            FS2 = new FileStream(TransListFileName(I), FileMode.Create, FileAccess.Write);
            SR2 = new StreamWriter(FS2);

            // Write header
            for (int i = 0; i < ListTransFields.Length; i++)
            {
                if (i > 0)
                {
                    SR2.Write("\t");
                }
                SR2.Write(ListTransFields[i]);
            }
            SR2.WriteLine();

            // Write transponder list
            for (int i = 0; i < TransList.Count; i++)
            {
                int Count_R = 0;
                int Count_TV = 0;
                int Count_IMG = 0;
                int Count_DATA = 0;
                int Count_TOTAL = 0;
                object[] Tab = TransList[i];
                double Freq = ((int)Tab[0]);

                int FreqI = (int)Math.Round(Freq);
                string FreqS = FreqI.ToString();

                // Counting items by type
                for (int ii = 0; ii < ChanList.Count; ii++)
                {
                    if ((((string)ChanList[ii][0]) == FreqS) && (((string)ChanList[ii][1]) == ((string)Tab[1])) && (((string)ChanList[ii][2]) == ((string)Tab[5])))
                    {
                        string T = (string)ChanList[ii][4];
                        switch (T)
                        {
                            case "R":
                                Count_R++;
                                break;
                            case "TV":
                                Count_TV++;
                                break;
                            case "IMG":
                                Count_IMG++;
                                break;
                            case "DATA":
                                Count_DATA++;
                                break;
                        }
                        Count_TOTAL++;
                    }
                }


                if ((!TransCh) || (Count_TOTAL > 0))
                {
                    Tab[0] = FreqI.ToString();

                    string[] FEC = ((string)Tab[2]).Split('|');
                    List<string> FEC_1 = new List<string>();
                    List<string> FEC_2 = new List<string>();
                    string FEC_1_;
                    string FEC_2_;
                    for (int ii = 0; ii < FEC.Length; ii++)
                    {
                        FEC_1_ = "";
                        FEC_2_ = "";
                        if (FEC[ii].Contains(" "))
                        {
                            int TempI = FEC[ii].IndexOf(' ');
                            FEC_1_ = FEC[ii].Substring(0, TempI);
                            FEC_2_ = FEC[ii].Substring(TempI + 1);
                        }
                        else
                        {
                            FEC_1_ = FEC[ii];
                        }
                        if (!FEC_1.Contains(FEC_1_))
                        {
                            if (FEC_1_.Length > 0)
                            {
                                FEC_1.Add(FEC_1_);
                            }
                        }
                        if (!FEC_2.Contains(FEC_2_))
                        {
                            if (FEC_2_.Length > 0)
                            {
                                FEC_2.Add(FEC_2_);
                            }
                        }
                    }

                    FEC_1.Sort();
                    FEC_1_ = "";
                    for (int ii = 0; ii < FEC_1.Count; ii++)
                    {
                        if (ii > 0)
                        {
                            FEC_1_ = FEC_1_ + Separator;
                        }
                        FEC_1_ = FEC_1_ + FEC_1[ii];
                    }
                    Tab[2] = FEC_1_;

                    FEC_2.Sort();
                    FEC_2_ = "";
                    for (int ii = 0; ii < FEC_2.Count; ii++)
                    {
                        if (ii > 0)
                        {
                            FEC_2_ = FEC_2_ + Separator;
                        }
                        FEC_2_ = FEC_2_ + FEC_2[ii];
                    }
                    Tab[3] = FEC_2_;

                    // R|TV|IMG|DATA|TOTAL
                    Tab[11] = Count_R.ToString();
                    Tab[12] = Count_TV.ToString();
                    Tab[13] = Count_IMG.ToString();
                    Tab[14] = Count_DATA.ToString();
                    Tab[15] = Count_TOTAL.ToString();

                    for (int ii = 0; ii < ListTransFields.Length; ii++)
                    {
                        if (ii > 0)
                        {
                            SR2.Write("\t");
                        }
                        switch (ListTransFields[ii])
                        {
                            case "Freq": SR2.Write((string)Tab[0]); break;
                            case "Pol": SR2.Write((string)Tab[1]); break;
                            case "SR": SR2.Write((string)Tab[2]); break;
                            case "FEC": SR2.Write((string)Tab[3]); break;
                            case "Txp": SR2.Write((string)Tab[4]); break;
                            case "Beam": SR2.Write((string)Tab[5]); break;
                            case "R": SR2.Write((string)Tab[11]); break;
                            case "TV": SR2.Write((string)Tab[12]); break;
                            case "IMG": SR2.Write((string)Tab[13]); break;
                            case "DATA": SR2.Write((string)Tab[14]); break;
                            case "TOTAL": SR2.Write((string)Tab[15]); break;
                        }
                    }
                    SR2.WriteLine();
                }
            }

            // Close transponder list file
            SR2.Close();
            FS2.Close();
        }


        /// <summary>
        /// Text list from HTML created by CreateInfo
        /// </summary>
        protected List<string> InfoText = new List<string>();

        /// <summary>
        /// Color list from HTML created by CreateInfo
        /// </summary>
        protected List<string> InfoColor = new List<string>();

        /// <summary>
        /// List of table row stan numbers to correct errors
        /// </summary>
        public List<Dictionary<int, int>> SpanChange = new List<Dictionary<int, int>>();

        bool PrepareInfoExtend = true;
        protected bool PrepareInfoFlySat = false;

        /// <summary>
        /// Prepare information based on HTML tags
        /// </summary>
        /// <param name="N"></param>
        protected void PrepareInfo(HtmlAgilityPack.HtmlNode N)
        {
            InfoText.Clear();
            InfoColor.Clear();


            for (int i = 0; i < N.ChildNodes.Count; i++)
            {
                if (N.ChildNodes[i].Name == "font")
                {
                    for (int ii = 0; ii < N.ChildNodes[i].ChildNodes.Count; ii++)
                    {
                        if ((N.ChildNodes[i].ChildNodes[ii].Name == "font") || (PrepareInfoFlySat && (N.ChildNodes[i].ChildNodes[ii].Name == "a")))
                        {
                            if (PrepareInfoExtend)
                            {
                                for (int iii = 0; iii < N.ChildNodes[i].ChildNodes[ii].ChildNodes.Count; iii++)
                                {
                                    if (N.ChildNodes[i].ChildNodes[ii].ChildNodes[iii].Name == "#text")
                                    {
                                        if (N.ChildNodes[i].ChildNodes[ii].Attributes.Contains("color"))
                                        {
                                            InfoColor.Add(N.ChildNodes[i].ChildNodes[ii].Attributes["color"].Value);
                                        }
                                        else
                                        {
                                            InfoColor.Add("");
                                        }
                                        InfoText.Add(Prepare(N.ChildNodes[i].ChildNodes[ii].ChildNodes[iii].InnerText));
                                    }
                                }
                            }
                            else
                            {
                                if (N.ChildNodes[i].ChildNodes[ii].Attributes.Contains("color"))
                                {
                                    InfoColor.Add(N.ChildNodes[i].ChildNodes[ii].Attributes["color"].Value);
                                }
                                else
                                {
                                    InfoColor.Add("");
                                }
                                InfoText.Add(Prepare(N.ChildNodes[i].ChildNodes[ii].InnerText));
                            }
                        }
                        if (N.ChildNodes[i].ChildNodes[ii].Name == "#text")
                        {
                            if (PrepareInfoFlySat && (N.ChildNodes[i].Attributes.Contains("color")))
                            {
                                InfoColor.Add(N.ChildNodes[i].Attributes["color"].Value);
                            }
                            else
                            {
                                InfoColor.Add("");
                            }
                            InfoText.Add(Prepare(N.ChildNodes[i].ChildNodes[ii].InnerText));
                        }
                        if (N.ChildNodes[i].ChildNodes[ii].Name == "b")
                        {
                            if (PrepareInfoFlySat)
                            {
                                for (int iii = 0; iii < N.ChildNodes[i].ChildNodes[ii].ChildNodes.Count; iii++)
                                {
                                    if (N.ChildNodes[i].ChildNodes[ii].ChildNodes[iii].Name == "#text")
                                    {
                                        if (N.ChildNodes[i].ChildNodes[ii].Attributes.Contains("color"))
                                        {
                                            InfoColor.Add(N.ChildNodes[i].ChildNodes[ii].Attributes["color"].Value);
                                        }
                                        else
                                        {
                                            InfoColor.Add("");
                                        }
                                        InfoText.Add(Prepare(N.ChildNodes[i].ChildNodes[ii].ChildNodes[iii].InnerText));
                                    }
                                }
                            }
                            else
                            {
                                InfoColor.Add("");
                                InfoText.Add(Prepare(N.ChildNodes[i].ChildNodes[ii].InnerText));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create language list from source data
        /// </summary>
        /// <returns></returns>
        protected string LangList()
        {
            string LangList_ = "";
            for (int ii = 0; ii < InfoText.Count; ii++)
            {
                int XLetter = 0;
                int XDigit = 0;
                int XSpace = 0;
                InfoText[ii] = InfoText[ii].Trim();
                InfoText[ii] = InfoText[ii].Replace("  ", " ");
                InfoText[ii] = InfoText[ii].Replace("  ", " ");

                for (int XII = 0; XII < InfoText[ii].Length; XII++)
                {

                    if (InfoText[ii][XII] == ' ')
                    {
                        XSpace++;
                    }

                    char C = InfoText[ii][XII];
                    if (((C >= '0') && (C <= '9')) || (C == '.'))
                    {
                        XDigit++;
                    }
                    else
                    {
                        if (((C >= 'a') && (C <= 'z')) || ((C >= 'A') && (C <= 'Z')) || (C == '-') || (C == '/'))
                        {
                            XLetter++;
                        }
                        else
                        {
                            if (C != ' ')
                            {
                                throw new Exception("XXX");
                            }
                        }
                    }
                }
                if (XSpace == 1)
                {
                    if (LangList_ != "")
                    {
                        LangList_ += Separator;
                    }
                    InfoText[ii] = InfoText[ii].Replace(' ', '[') + "]";
                    InfoText[ii] = InfoText[ii].Replace("/", "][");
                    LangList_ = LangList_ + InfoText[ii];
                }
                else
                {
                    if (XSpace > 1)
                    {
                        throw new Exception("XXX");
                    }
                    else
                    {
                        if ((XDigit > 0) | (XLetter > 0))
                        {
                            if ((XDigit > 0) & (XLetter == 0))
                            {
                                if (LangList_ != "")
                                {
                                    LangList_ += Separator;
                                }
                                LangList_ = LangList_ + InfoText[ii];
                            }
                            else
                            {
                                if ((XDigit == 0) & (XLetter > 0))
                                {
                                    LangList_ = LangList_ + "[" + InfoText[ii].Replace("/", "][") + "]";
                                }
                                else
                                {
                                    throw new Exception("XXX");
                                }
                            }
                        }
                    }
                }
            }
            return LangList_;
        }

        /// <summary>
        /// Get sorting key, which is a number created from frequency, polarization and SID
        /// </summary>
        /// <param name="Freq"></param>
        /// <param name="Pol"></param>
        /// <param name="Beam"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        long GetSortKey(int Freq, string Pol, string Beam, string SID)
        {
            long Freq_ = Freq;
            long SID_ = int.Parse(ClearInt(SID));
            Freq_ = Freq_ * 1000000000L;
            if (SID_ >= 1000000L)
            {
                throw new Exception("Unsupported SID [" + SID.ToString() + "]");
            }
            long Beam_ = BeamList.Contains(Beam) ? (BeamList.IndexOf(Beam)) : 0;
            Beam_ = Beam_ * 1000000L;

            Pol = Pol.ToUpperInvariant();
            if ((Pol.Length != 1) || (!PolOrder.Contains(Pol)))
            {
                throw new Exception("Unsupported polarization [" + Pol + "]");
            }
            long Pol_ = PolOrder.IndexOf(Pol[0]);

            Pol_ = Pol_ * 100000000L;
            return (Freq_) + Pol_ + Beam_ + SID_;
        }

        /// <summary>
        /// Check if frequency is allowed according selected bands
        /// </summary>
        /// <param name="Freq"></param>
        /// <param name="Band1"></param>
        /// <param name="Band2"></param>
        /// <param name="Band3"></param>
        /// <returns></returns>
        bool IsFreqAllowed(double Freq, bool Band1, bool Band2, bool Band3)
        {
            if (Freq == 20185)
            {
                Freq.ToString();
            }

            if (Freq < 0)
            {
                return false;
            }

            bool FreqAllowed = false;
            bool FreqOutOfBand = true;
            if ((Freq >= 3400) && (Freq <= 4200))
            {
                FreqOutOfBand = false;
                if (Band1)
                {
                    FreqAllowed = true;
                }
            }
            if ((Freq >= 10700) && (Freq <= 12750))
            {
                FreqOutOfBand = false;
                if (Band2)
                {
                    FreqAllowed = true;
                }
            }
            if ((Freq >= 18200) && (Freq <= 22200))
            {
                FreqOutOfBand = false;
                if (Band3)
                {
                    FreqAllowed = true;
                }
            }

            if (FreqOutOfBand)
            {
                throw new Exception("Frequency out of band: " + Freq.ToString());
            }

            return FreqAllowed;
        }

        /// <summary>
        /// Creating bitmaps showing transponder overlapping between satelites
        /// </summary>
        /// <param name="Band1"></param>
        /// <param name="Band2"></param>
        /// <param name="Band3"></param>
        public void CreateTransBitmap(List<bool> Selected, bool Band1, bool Band2, bool Band3)
        {
            File.Delete(TransImgFileName(1));
            File.Delete(TransImgFileName(2));
            File.Delete(TransImgFileName(3));
            if (Band1)
            {
                CreateTransBitmapWork(Selected, TransImgFileName(1), 3400, 4200, 1);
            }
            if (Band2)
            {
                CreateTransBitmapWork(Selected, TransImgFileName(2), 10700, 12750, 1);
            }
            if (Band3)
            {
                CreateTransBitmapWork(Selected, TransImgFileName(3), 18200, 22200, 1);
            }
        }

        /// <summary>
        /// Creating bitmap showing transponder overlapping for one band
        /// </summary>
        /// <param name="Selected"></param>
        /// <param name="FileName"></param>
        /// <param name="FreqMin"></param>
        /// <param name="FreqMax"></param>
        /// <param name="Resolution"></param>
        private void CreateTransBitmapWork(List<bool> Selected, string FileName, int FreqMin, int FreqMax, int Resolution)
        {
            double Resolution_ = Resolution;
            List<int> SatList = new List<int>();
            for (int i = 0; i < Selected.Count; i++)
            {
                if (Selected[i])
                {
                    SatList.Add(i);
                }
            }

            int BmpW = ((FreqMax - FreqMin) / Resolution) + 1;
            int BmpH = SatList.Count;

            // Create bitmap file
            Bitmap Bmp = new Bitmap(BmpW, BmpH, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics Bmp_ = Graphics.FromImage(Bmp);
            Bmp_.FillRectangle(Brushes.Black, 0, 0, BmpW, BmpH);

            for (int i = 0; i < SatList.Count; i++)
            {
                // Open transponder list file
                List<object[]> TransList = new List<object[]>();
                FileStream FS1 = new FileStream(TransListFileName(SatList[i]), FileMode.Open, FileAccess.Read);
                StreamReader SR1 = new StreamReader(FS1);

                // Ommit header
                SR1.ReadLine();

                // Load file into list
                string Buf = SR1.ReadLine();
                while (Buf != null)
                {
                    string[] Raw = Buf.Split('\t');

                    // Frequency
                    int FreqX = int.Parse(Raw[0]);
                    int Freq = (int)Math.Round((((double)FreqX) - (double)FreqMin) / Resolution_);

                    // Polarization
                    string Pol = Raw[1].ToUpperInvariant();

                    // Put pixel in bitmap if transponder frequency is within analyzed band
                    if ((FreqX >= FreqMin) && (FreqX <= FreqMax))
                    {
                        if (Freq < 0)
                        {
                            Freq = 0;
                        }
                        if (Freq >= BmpW)
                        {
                            Freq = BmpW - 1;
                        }
                        Color C1;
                        Color C2;
                        C1 = Bmp.GetPixel(Freq, i);
                        switch (Pol)
                        {
                            case "H":
                            case "R":
                                C2 = Color.FromArgb(255, C1.G, C1.B);
                                Bmp.SetPixel(Freq, i, C2);
                                break;
                            case "V":
                            case "L":
                                C2 = Color.FromArgb(C1.R, 255, C1.B);
                                Bmp.SetPixel(Freq, i, C2);
                                break;
                            default:
                                throw new Exception("Unsupported polarization [" + Pol + "]");
                        }
                    }

                    Buf = SR1.ReadLine();
                }

                // Close transponder list file
                SR1.Close();
                FS1.Close();
            }

            // Save bitmap file
            Bmp.Save(FileName);
        }


        protected List<string> BeamList = new List<string>();


        protected void BeamListAdd(string X)
        {
            if (!BeamList.Contains(X))
            {
                BeamList.Add(X);
            }
        }

        protected void BeamListWriteFile(int I)
        {
            BeamList.Sort();
            File.Delete(BeamFileName(I));
            FileStream FS = new FileStream(BeamFileName(I), FileMode.CreateNew, FileAccess.Write);
            StreamWriter FSW = new StreamWriter(FS);
            for (int i = 0; i < BeamList.Count; i++)
            {
                FSW.WriteLine(BeamList[i]);
            }
            FSW.Close();
            FS.Close();
        }
    }
}
