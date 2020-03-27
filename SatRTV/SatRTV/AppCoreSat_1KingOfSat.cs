/*
 * Created by SharpDevelop.
 * User: XXX
 * Date: 2020-03-23
 * Time: 11:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace SatRTV
{
	/// <summary>
	/// Description of AppCoreSat_1KingOfSat.
	/// </summary>
	public class AppCoreSat_1KingOfSat : AppCoreSat
	{
        public override void Parse(int I, bool TypeByPID)
        {
            File.Delete(TransFileName(I));
            File.Delete(ChanFileName(I));
            if (!File.Exists(DataFileName(I)))
            {
                return;
            }

            FileStream FS1 = new FileStream(TransFileName(I), FileMode.CreateNew, FileAccess.Write);
            StreamWriter FS1W = new StreamWriter(FS1);
            FS1W.WriteLine("Satellite\tFrequence\tPol\tTxp\tBeam\tStandard\tModulation\tSR/FEC\tNetwork, bitrate\tNID\tTID\tRow");

            FileStream FS2 = new FileStream(ChanFileName(I), FileMode.CreateNew, FileAccess.Write);
            StreamWriter FS2W = new StreamWriter(FS2);
            FS2W.WriteLine("Freq\tPol\tSR\tBeam\tType\tName\tCountry\tCategory\tPackages\tEncryption\tSID\tVPID\tAudio\tPMT\tPCR\tTXT\tLast updated\tFTA\tRow");

            BeamList.Clear();

            HtmlAgilityPack.HtmlDocument HTMLDoc = new HtmlAgilityPack.HtmlDocument();
            FileStream HFS = new FileStream(DataFileName(I), FileMode.Open, FileAccess.Read);
            StreamReader HFSR = new StreamReader(HFS);
            string RAW = HFSR.ReadToEnd();
            HFSR.Close();
            HFS.Close();
            HTMLDoc.LoadHtml(RAW);
            for (int ii = 0; ii < HTMLDoc.DocumentNode.ChildNodes.Count; ii++)
            {
                if (HTMLDoc.DocumentNode.ChildNodes[ii].Name == "html")
                {
                    for (int iii = 0; iii < HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes.Count; iii++)
                    {
                        if (HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].Name == "body")
                        {
                            for (int iiii = 0; iiii < HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes.Count; iiii++)
                            {
                                HtmlAgilityPack.HtmlNode N = HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes[iiii];
                                if (N.Name == "div")
                                {
                                    if (N.Attributes["class"].Value == "w3-main")
                                    {
                                        bool WasHeader = false;
                                        string[] SatTrans = null;
                                        for (int _i = 0; _i < N.ChildNodes.Count; _i++)
                                        {
                                            if (N.ChildNodes[_i].Name == "center")
                                            {
                                                WasHeader = true;
                                            }
                                            if (WasHeader)
                                            {
                                                if (N.ChildNodes[_i].Name == "table")
                                                {
                                                    if (N.ChildNodes[_i].Attributes["class"] != null)
                                                    {
                                                        if (N.ChildNodes[_i].Attributes["class"].Value == "frq")
                                                        {
                                                            for (int _ii = 0; _ii < N.ChildNodes[_i].ChildNodes.Count; _ii++)
                                                            {
                                                                if (N.ChildNodes[_i].ChildNodes[_ii].Name == "tr")
                                                                {
                                                                    int RowNum = (_i * 1000000) + (_ii * 1000);
                                                                    int TdNum = 0;
                                                                    SatTrans = new string[14];
                                                                    for (int _iii = 0; _iii < N.ChildNodes[_i].ChildNodes[_ii].ChildNodes.Count; _iii++)
                                                                    {
                                                                        HtmlAgilityPack.HtmlNode NN = N.ChildNodes[_i].ChildNodes[_ii].ChildNodes[_iii];
                                                                        if (NN.Name == "td")
                                                                        {
                                                                            switch (TdNum)
                                                                            {
                                                                                case 0:
                                                                                    SatTrans[TdNum] = Prepare(NN.InnerText).Replace("&deg;", "");
                                                                                    if (SatTrans[TdNum].Contains("E") || SatTrans[TdNum].Contains("W"))
                                                                                    {
                                                                                        if (SatTrans[TdNum].Contains("E"))
                                                                                        {
                                                                                            SatTrans[TdNum] = SatTrans[TdNum].Replace("E", "");
                                                                                        }
                                                                                        if (SatTrans[TdNum].Contains("W"))
                                                                                        {
                                                                                            SatTrans[TdNum] = "-" + SatTrans[TdNum].Replace("W", "");
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (SatTrans[TdNum] != "Pos")
                                                                                        {
                                                                                            throw new Exception("XXX");
                                                                                        }
                                                                                    }
                                                                                    break;
                                                                                case 1:
                                                                                    if (NN.ChildNodes.Count > 1)
                                                                                    {
                                                                                        SatTrans[TdNum] = "";
                                                                                        for (int _iiii = 0; _iiii < NN.ChildNodes.Count; _iiii++)
                                                                                        {
                                                                                            if (NN.ChildNodes[_iiii].Name == "a")
                                                                                            {
                                                                                                SatTrans[TdNum] += NN.ChildNodes[_iiii].InnerText;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        SatTrans[TdNum] = Prepare(NN.InnerText).Replace("&deg;", "");
                                                                                    }
                                                                                    break;
                                                                                default:
                                                                                    SatTrans[TdNum] = Prepare(NN.InnerText);
                                                                                    if ((TdNum == 5) && (SatTrans[0] != "Pos"))
                                                                                    {
                                                                                        BeamListAdd(Prepare(SatTrans[TdNum]));
                                                                                    }
                                                                                    break;
                                                                            }
                                                                            TdNum++;
                                                                        }
                                                                    }

                                                                    SatTrans[13] = RowNum.ToString();

                                                                    if (SatTrans[0] != "Pos")
                                                                    {
                                                                        for (int _iii = 1; _iii < 12; _iii++)
                                                                        {
                                                                            if (_iii > 1)
                                                                            {
                                                                                FS1W.Write("\t");
                                                                            }
                                                                            FS1W.Write(SatTrans[_iii]);
                                                                        }
                                                                        FS1W.Write("\t");
                                                                        FS1W.Write(SatTrans[13]);
                                                                        FS1W.Write("\r\n");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (N.ChildNodes[_i].Name == "div")
                                                {
                                                    if (N.ChildNodes[_i].Attributes["id"] != null)
                                                    {
                                                        if (N.ChildNodes[_i].Attributes["id"].Value.StartsWith("m"))
                                                        {
                                                            for (int _ii = 0; _ii < N.ChildNodes[_i].ChildNodes.Count; _ii++)
                                                            {
                                                                HtmlAgilityPack.HtmlNode NN = N.ChildNodes[_i].ChildNodes[_ii];
                                                                if (NN.Name == "table")
                                                                {
                                                                    for (int _iii = 0; _iii < NN.ChildNodes.Count; _iii++)
                                                                    {
                                                                        if (NN.ChildNodes[_iii].Name == "tr")
                                                                        {
                                                                            int RowNum = (_i * 1000000) + (_ii * 1000) + _iii;

                                                                            int TdNum = 0;
                                                                            string[] SatChan = new string[21];
                                                                            for (int _iiii = 0; _iiii < NN.ChildNodes[_iii].ChildNodes.Count; _iiii++)
                                                                            {
                                                                                HtmlAgilityPack.HtmlNode NNN = NN.ChildNodes[_iii].ChildNodes[_iiii];
                                                                                if (NNN.Name == "td")
                                                                                {
                                                                                    switch (TdNum)
                                                                                    {
                                                                                        case 0:
                                                                                            break;
                                                                                        case 1: // Type
                                                                                            SatChan[TdNum] = "UNK";
                                                                                            for (int __i = 0; __i < NNN.ChildNodes.Count; __i++)
                                                                                            {
                                                                                                if (NNN.ChildNodes[__i].Name == "img")
                                                                                                {
                                                                                                    if (NNN.ChildNodes[__i].Attributes["src"].Value.Contains("data"))
                                                                                                    {
                                                                                                        SatChan[TdNum] = "DATA";
                                                                                                    }
                                                                                                    if (NNN.ChildNodes[__i].Attributes["src"].Value.Contains("radio"))
                                                                                                    {
                                                                                                        SatChan[TdNum] = "R";
                                                                                                    }
                                                                                                    if (NNN.ChildNodes[__i].Attributes["src"].Value.Contains("tv"))
                                                                                                    {
                                                                                                        SatChan[TdNum] = "TV";
                                                                                                    }
                                                                                                    if (NNN.ChildNodes[__i].Attributes["src"].Value.Contains("zap"))
                                                                                                    {
                                                                                                        SatChan[TdNum] = "TV";
                                                                                                    }
                                                                                                }
                                                                                                if (NNN.ChildNodes[__i].Name == "a")
                                                                                                {
                                                                                                    for (int ___i = 0; ___i < NNN.ChildNodes[__i].ChildNodes.Count; ___i++)
                                                                                                    {
                                                                                                        if (NNN.ChildNodes[__i].ChildNodes[___i].Attributes["src"].Value.Contains("data"))
                                                                                                        {
                                                                                                            SatChan[TdNum] = "DATA";
                                                                                                        }
                                                                                                        if (NNN.ChildNodes[__i].ChildNodes[___i].Attributes["src"].Value.Contains("radio"))
                                                                                                        {
                                                                                                            SatChan[TdNum] = "R";
                                                                                                        }
                                                                                                        if (NNN.ChildNodes[__i].ChildNodes[___i].Attributes["src"].Value.Contains("tv"))
                                                                                                        {
                                                                                                            SatChan[TdNum] = "TV";
                                                                                                        }
                                                                                                        if (NNN.ChildNodes[__i].ChildNodes[___i].Attributes["src"].Value.Contains("zap"))
                                                                                                        {
                                                                                                            SatChan[TdNum] = "TV";
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            break;
                                                                                        default:
                                                                                            SatChan[TdNum] = "";
                                                                                            for (int __i = 0; __i < NNN.ChildNodes.Count; __i++)
                                                                                            {
                                                                                                if ((NNN.ChildNodes[__i].Name == "a") || (NNN.ChildNodes[__i].Name == "#text") || (NNN.ChildNodes[__i].Name == "i"))
                                                                                                {
                                                                                                    bool Good = true;
                                                                                                    if (TdNum == 13)
                                                                                                    {
                                                                                                        if (Prepare(NNN.ChildNodes[__i].InnerText) == "+")
                                                                                                        {
                                                                                                            Good = false;
                                                                                                        }
                                                                                                    }

                                                                                                    if (Good)
                                                                                                    {
                                                                                                        string XXX = "";
                                                                                                        XXX = Prepare(NNN.ChildNodes[__i].InnerText);
                                                                                                        if (XXX != "")
                                                                                                        {
                                                                                                            if (SatChan[TdNum] != "")
                                                                                                            {
                                                                                                                SatChan[TdNum] += Separator;
                                                                                                            }
                                                                                                            SatChan[TdNum] += XXX;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            // Audio
                                                                                            if (TdNum == 9)
                                                                                            {
                                                                                                SatChan[TdNum] = SatChan[TdNum].Replace(" ", Separator);
                                                                                                string[] Lang = SatChan[TdNum].Split(Separator[0]);
                                                                                                SatChan[TdNum] = "";
                                                                                                for (int XI = 0; XI < Lang.Length; XI++)
                                                                                                {
                                                                                                    int XLetter = 0;
                                                                                                    int XDigit = 0;
                                                                                                    for (int XII = 0; XII < Lang[XI].Length; XII++)
                                                                                                    {
                                                                                                        char C = Lang[XI][XII];
                                                                                                        if ((C >= '0') && (C <= '9'))
                                                                                                        {
                                                                                                            XDigit++;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (((C >= 'a') && (C <= 'z')) || ((C >= 'A') && (C <= 'Z')) || (C == '-') || (C == '.'))
                                                                                                            {
                                                                                                                XLetter++;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                throw new Exception("XXX");
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                    if ((XDigit > 0) && (XLetter > 0))
                                                                                                    {
                                                                                                        //throw new Exception("XXX");
                                                                                                    }
                                                                                                    if ((XDigit > 0) || (XLetter > 0))
                                                                                                    {
                                                                                                        if ((XDigit > 0) && (XLetter == 0))
                                                                                                        {
                                                                                                            if (SatChan[TdNum] != "")
                                                                                                            {
                                                                                                                SatChan[TdNum] += Separator;
                                                                                                            }
                                                                                                            SatChan[TdNum] += Lang[XI];
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            SatChan[TdNum] += "[" + Lang[XI] + "]";
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            break;
                                                                                    }
                                                                                    TdNum++;
                                                                                }
                                                                            }
                                                                            if (TdNum >= 12)
                                                                            {
                                                                                if (TypeByPID)
                                                                                {
                                                                                    string ChannelType = "UNK";
                                                                                    if ((SatChan[8] == "") && (SatChan[9] == ""))
                                                                                    {
                                                                                        ChannelType = "DATA";
                                                                                    }
                                                                                    if ((SatChan[8] != "") && (SatChan[9] != ""))
                                                                                    {
                                                                                        ChannelType = "TV";
                                                                                    }
                                                                                    if ((SatChan[8] == "") && (SatChan[9] != ""))
                                                                                    {
                                                                                        ChannelType = "R";
                                                                                    }
                                                                                    if ((SatChan[8] != "") && (SatChan[9] == ""))
                                                                                    {
                                                                                        ChannelType = "IMG";
                                                                                    }
                                                                                    SatChan[1] = ChannelType;
                                                                                }

                                                                                SatChan[20] = RowNum.ToString();

                                                                                if (SatChan[12] != "TXT")
                                                                                {
                                                                                    //FS2W.Write(SatTrans[0]);
                                                                                    //FS2W.Write("\t");
                                                                                    FS2W.Write(SatTrans[2]);
                                                                                    FS2W.Write("\t");
                                                                                    FS2W.Write(SatTrans[3]);
                                                                                    FS2W.Write("\t");
                                                                                    FS2W.Write(SatTrans[8]);
                                                                                    FS2W.Write("\t");
                                                                                    FS2W.Write(SatTrans[5]);

                                                                                    for (int Xi = 1; Xi < TdNum; Xi++)
                                                                                    {
                                                                                        FS2W.Write("\t");
                                                                                        FS2W.Write(SatChan[Xi]);
                                                                                    }

                                                                                    if ((SatChan[6].ToLowerInvariant().Contains("clear")) || (SatChan[6].ToLowerInvariant().Contains("fta")))
                                                                                    {
                                                                                        FS2W.Write("\t");
                                                                                        FS2W.Write("Yes");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        FS2W.Write("\t");
                                                                                        FS2W.Write("No");
                                                                                    }


                                                                                    FS2W.Write("\t");
                                                                                    FS2W.Write(SatChan[20]);

                                                                                    FS2W.WriteLine();
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            FS1W.Close();
            FS1.Close();
            FS2W.Close();
            FS2.Close();
            BeamListWriteFile(I);
        }
	}
}
