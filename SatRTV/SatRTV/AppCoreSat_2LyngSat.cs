/*
 * Created by SharpDevelop.
 * User: XXX
 * Date: 2020-03-23
 * Time: 11:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace SatRTV
{
	/// <summary>
	/// Description of AppCoreSat_2LyngSat.
	/// </summary>
	public class AppCoreSat_2LyngSat : AppCoreSat
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
            FS1W.WriteLine("Frequency\tPol\tTxp\tBeam\tEIRP (dbW)\tProvider\tSystem\tSR-FEC\tModulation\tONID-TID\tSource\tUpdated\tRow");

            FileStream FS2 = new FileStream(ChanFileName(I), FileMode.CreateNew, FileAccess.Write);
            StreamWriter FS2W = new StreamWriter(FS2);
            FS2W.WriteLine("Freq\tPol\tSR\tBeam\tType\tName\tFTA\tEncryption\tSID\tVPID\tAPID Lang\tRow");

            BeamList.Clear();

            HtmlAgilityPack.HtmlDocument HTMLDoc = new HtmlAgilityPack.HtmlDocument();
            FileStream HFS = new FileStream(DataFileName(I), FileMode.Open, FileAccess.Read);
            StreamReader HFSR = new StreamReader(HFS, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            string RAW = HFSR.ReadToEnd();
            HFSR.Close();
            HFS.Close();
            HTMLDoc.LoadHtml(RAW);

            RowN = 0;
            RowN0 = 0;
            RowX = 0;

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
                                if (HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes[iiii].Name == "div")
                                {
                                    HtmlAgilityPack.HtmlNode N = HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes[iiii];
                                    for (int i_ = 0; i_ < N.ChildNodes.Count; i_++)
                                    {
                                        if (N.ChildNodes[i_].Name == "table")
                                        {
                                            if (N.ChildNodes[i_].Attributes["class"].Value == "bigtable")
                                            {
                                                for (int ii_ = 0; ii_ < N.ChildNodes[i_].ChildNodes.Count; ii_++)
                                                {
                                                    if (N.ChildNodes[i_].ChildNodes[ii_].Name == "tr")
                                                    {
                                                        for (int iii_ = 0; iii_ < N.ChildNodes[i_].ChildNodes[ii_].ChildNodes.Count; iii_++)
                                                        {
                                                            if (N.ChildNodes[i_].ChildNodes[ii_].ChildNodes[iii_].Name == "td")
                                                            {
                                                                if (ToInt(N.ChildNodes[i_].ChildNodes[ii_].ChildNodes[iii_].Attributes["width"].Value) >= 500)
                                                                {
                                                                    HtmlAgilityPack.HtmlNode NN = N.ChildNodes[i_].ChildNodes[ii_].ChildNodes[iii_];
                                                                    for (int i__ = 0; i__ < NN.ChildNodes.Count; i__++)
                                                                    {
                                                                        if (NN.ChildNodes[i__].Name == "table")
                                                                        {
                                                                            if (NN.ChildNodes[i__].Attributes.Contains("width"))
                                                                            {
                                                                                if (ToInt(NN.ChildNodes[i__].Attributes["width"].Value) >= 500)
                                                                                {
                                                                                    ChannelTable(I, NN.ChildNodes[i__], ref FS1W, ref FS2W, TypeByPID);
                                                                                    RowX += 1000;
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

        bool ChannelFromFirstRow = true;
        bool ChannelFromOtherRows = true;

        int RowN = 0;
        int RowN0 = 0;
        int RowX = 0;

        private void ChannelTable(int SatN, HtmlAgilityPack.HtmlNode N, ref StreamWriter FS1W, ref StreamWriter FS2W, bool TypeByPID)
        {
            List<int> Rowlist = new List<int>();
            List<List<int>> RowlistTD = new List<List<int>>();
            for (int i = 0; i < N.ChildNodes.Count; i++)
            {
                if (N.ChildNodes[i].Name == "tr")
                {
                    Rowlist.Add(i);
                    List<int> RowlistTD_ = new List<int>();
                    for (int i_ = 0; i_ < N.ChildNodes[i].ChildNodes.Count; i_++)
                    {
                        if (N.ChildNodes[i].ChildNodes[i_].Name == "td")
                        {
                            RowlistTD_.Add(i_);
                        }
                    }
                    RowlistTD.Add(RowlistTD_);
                }
            }

            if (Rowlist.Count >= 4)
            {
                Rowlist.RemoveAt(Rowlist.Count - 1);
                RowlistTD.RemoveAt(RowlistTD.Count - 1);
                Rowlist.RemoveAt(0);
                Rowlist.RemoveAt(0);
                RowlistTD.RemoveAt(0);
                RowlistTD.RemoveAt(0);

                //FS1W.WriteLine("TABLE BEGIN");

                string[] SatTrans = null;
                string[] SatChan = null;


                int TransSpan = 0;
                bool FirstItem = false;
                for (int i = 0; i < Rowlist.Count; i++)
                {
                    HtmlAgilityPack.HtmlNode NN = N.ChildNodes[Rowlist[i]];
                    if (RowlistTD[i].Count >= 4)
                    {
                        string ChannelType = "";
                        SatChan = null;
                        int PrepareChannel = 0;
                        if (TransSpan == 0)
                        {
                            RowN0 = RowN;
                            RowN = i + RowX;

                            FirstItem = true;
                            SatTrans = new string[20];

                            TransSpan = 0;
                            if (NN.ChildNodes[RowlistTD[i][1]].Attributes.Contains("rowspan"))
                            {
                                TransSpan = ToInt(NN.ChildNodes[RowlistTD[i][1]].Attributes["rowspan"].Value);
                                TransSpan--;
                            }
                            if (SpanChange[SatN].ContainsKey(RowN))
                            {
                                TransSpan += SpanChange[SatN][RowN];
                            }

                            int SatTransI = 0;
                            int TDi = 1;
                            HtmlAgilityPack.HtmlNode NNN = NN.ChildNodes[RowlistTD[i][TDi]];

                            for (int i_ = 0; i_ < NNN.ChildNodes.Count; i_++)
                            {
                                if (NNN.ChildNodes[i_].Name == "font")
                                {
                                    int Trans0 = 0;
                                    for (int ii_ = 0; ii_ < NNN.ChildNodes[i_].ChildNodes.Count; ii_++)
                                    {
                                        if (NNN.ChildNodes[i_].ChildNodes[ii_].Name == "font")
                                        {
                                            for (int iii_ = 0; iii_ < NNN.ChildNodes[i_].ChildNodes[ii_].ChildNodes.Count; iii_++)
                                            {

                                                if (NNN.ChildNodes[i_].ChildNodes[ii_].ChildNodes[iii_].Name == "br")
                                                {
                                                    SatTransI++;
                                                    Trans0++;
                                                }
                                                else
                                                {
                                                    if (Trans0 == 0)
                                                    {
                                                        string Temp = NNN.ChildNodes[i_].ChildNodes[ii_].ChildNodes[iii_].InnerText;
                                                        int TempI = Temp.IndexOf("&nbsp;");
                                                        SatTrans[SatTransI] = Temp.Substring(0, TempI);
                                                        SatTransI++;
                                                        SatTrans[SatTransI] = Temp.Substring(TempI + 6);
                                                    }
                                                    else
                                                    {
                                                        SatTrans[SatTransI] = Prepare(NNN.ChildNodes[i_].ChildNodes[ii_].ChildNodes[iii_].InnerText).Replace("tp ", "");
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    while (Trans0 < 3)
                                    {
                                        SatTransI++;
                                        Trans0++;
                                    }
                                }
                            }

                            BeamListAdd(SatTrans[3]);

                            SatTransI++;
                            TDi++;
                            TDi++;

                            // Provider name
                            SatTrans[SatTransI] = Prepare(NN.ChildNodes[RowlistTD[i][TDi]].InnerText);
                            ChannelType = "";
                            string ChannelString = NN.ChildNodes[RowlistTD[i][TDi]].InnerHtml.ToLowerInvariant();
                            int ChannelStringT = ChannelString.IndexOf("<a");
                            if (ChannelStringT >= 0)
                            {
                                ChannelString = ChannelString.Substring(ChannelStringT);
                                int ChannelStringTX = ChannelString.IndexOf(">");
                                if (ChannelStringTX > 0)
                                {
                                    ChannelString = ChannelString.Substring(0, ChannelStringTX + 1);
                                    if (ChannelString.Contains("tvchannels"))
                                    {
                                        ChannelType = "TV";
                                    }
                                    if (ChannelString.Contains("radiochannels"))
                                    {
                                        ChannelType = "R";
                                    }
                                    //SatTrans[SatTransI] = "{" + ChannelString + "}";
                                }
                            }
                            SatTransI++;
                            TDi++;
                            TDi++;

                            // System
                            int TDii = 0;
                            NNN = NN.ChildNodes[RowlistTD[i][TDi]];
                            int SystemColSpan = 1;
                            if (NNN.Attributes.Contains("colspan"))
                            {
                                SystemColSpan = ToInt(NNN.Attributes["colspan"].Value);
                            }
                            for (int i_ = 0; i_ < NNN.ChildNodes.Count; i_++)
                            {
                                if (NNN.ChildNodes[i_].Name == "font")
                                {
                                    for (int ii_ = 0; ii_ < NNN.ChildNodes[i_].ChildNodes.Count; ii_++)
                                    {
                                        if (TDii <= 0)
                                        {
                                            if (NNN.ChildNodes[i_].ChildNodes[ii_].Name == "br")
                                            {
                                                TDii++;
                                            }
                                            else
                                            {
                                                SatTrans[SatTransI] = Prepare(NNN.ChildNodes[i_].ChildNodes[ii_].InnerText);
                                            }
                                        }
                                    }
                                }
                            }
                            SatTransI++;
                            TDi++;



                            // SR-FEC, Modulation
                            PrepareInfo(NN.ChildNodes[RowlistTD[i][TDi]]);
                            if (InfoText.Count > 0)
                            {
                                SatTrans[SatTransI] = InfoText[0];
                            }
                            SatTransI++;
                            if (InfoText.Count > 1)
                            {
                                if (InfoColor[1] != "darkgreen")
                                {
                                    SatTrans[SatTransI] = InfoText[1];
                                }
                            }
                            SatTransI++;
                            TDi++;


                            // ONID-TID
                            PrepareInfo(NN.ChildNodes[RowlistTD[i][TDi]]);
                            if (InfoText.Count > 0)
                            {
                                if (InfoColor[0] == "darkgreen")
                                {
                                    SatTrans[SatTransI] = InfoText[0];
                                }
                            }
                            SatTransI++;
                            TDi++;

                            // Source
                            // Updated
                            while (RowlistTD[i].Count <= TDi)
                            {
                                TDi--;
                            }
                            PrepareInfo(NN.ChildNodes[RowlistTD[i][TDi]]);
                            if (InfoText.Count > 0)
                            {
                                SatTrans[SatTransI] = "";
                                for (int ii = 0; ii < (InfoText.Count - 1); ii++)
                                {
                                    if (ii > 0)
                                    {
                                        SatTrans[SatTransI] += Separator;
                                    }
                                    SatTrans[SatTransI] += InfoText[ii];
                                }
                            }
                            SatTransI++;
                            if (InfoText.Count > 0)
                            {
                                SatTrans[SatTransI] = InfoText[InfoText.Count - 1];
                            }

                            // Row
                            SatTransI++;
                            SatTrans[SatTransI] = RowN.ToString();

                            for (int ii = 0; ii < 13; ii++)
                            {
                                if (ii > 0)
                                {
                                    FS1W.Write("\t");
                                }
                                FS1W.Write(SatTrans[ii]);
                            }

                            FS1W.WriteLine();



                            if ((ChannelType != "") && ChannelFromFirstRow)
                            {
                                PrepareChannel = 2;
                            }
                        }
                        else
                        {
                            if (ChannelFromOtherRows)
                            {
                                PrepareChannel = 1;
                                TransSpan--;
                            }
                        }


                        if (PrepareChannel > 0)
                        {
                            SatChan = new string[20];

                            int TDi = 0;

                            if (PrepareChannel == 2)
                            {
                                TDi = 3;
                            }
                            else
                            {
                                if (FirstItem)
                                {
                                    TDi = 1;
                                    FirstItem = false;
                                }
                            }

                            SatChan[1] = Prepare(NN.ChildNodes[RowlistTD[i][TDi]].InnerText);
                            ChannelType = "UNK";
                            string ChannelString = NN.ChildNodes[RowlistTD[i][TDi]].InnerHtml.ToLowerInvariant();
                            int ChannelStringT = ChannelString.IndexOf("<a");
                            if (ChannelStringT >= 0)
                            {
                                ChannelString = ChannelString.Substring(ChannelStringT);
                                int ChannelStringTX = ChannelString.IndexOf(">");
                                if (ChannelStringTX > 0)
                                {
                                    ChannelString = ChannelString.Substring(0, ChannelStringTX + 1);
                                    if (ChannelString.Contains("tvchannels"))
                                    {
                                        ChannelType = "TV";
                                    }
                                    if (ChannelString.Contains("radiochannels"))
                                    {
                                        ChannelType = "R";
                                    }
                                    //SatTrans[SatTransI] = "{" + ChannelString + "}";
                                }
                            }
                            TDi++;

                            SatChan[2] = "No";
                            if (NN.ChildNodes[RowlistTD[i][TDi]].InnerHtml.Contains("freetv"))
                            {
                                if (ChannelType == "UNK")
                                {
                                    ChannelType = "TV";
                                }
                                SatChan[2] = "Yes";
                            }
                            if (NN.ChildNodes[RowlistTD[i][TDi]].InnerHtml.Contains("freeradio"))
                            {
                                if (ChannelType == "UNK")
                                {
                                    ChannelType = "R";
                                }
                                SatChan[2] = "Yes";
                            }
                            TDi++;


                            // Encryption
                            PrepareInfo(NN.ChildNodes[RowlistTD[i][TDi]]);
                            SatChan[3] = "";
                            for (int ii = (PrepareChannel == 2 ? 1 : 0); ii < InfoText.Count; ii++)
                            {
                                if (ii > (PrepareChannel == 2 ? 1 : 0))
                                {
                                    SatChan[3] += "|";
                                }
                                SatChan[3] += InfoText[ii];
                            }

                            TDi++;

                            // SID, VPID
                            if (PrepareChannel == 2)
                            {
                                PrepareInfo(NN.ChildNodes[RowlistTD[i][TDi]]);
                                int GreenItem = -1;
                                for (int ii = 0; ii < InfoColor.Count; ii++)
                                {
                                    if (InfoColor[ii] == "darkgreen")
                                    {
                                        GreenItem = ii;
                                    }
                                }
                                if (GreenItem < 0)
                                {
                                    if (InfoText.Count == 2)
                                    {
                                        GreenItem = 1;
                                    }
                                }

                                if (GreenItem < 0)
                                {
                                    throw new Exception("XXX");
                                }

                                SatChan[4] = InfoText[GreenItem];
                                SatChan[5] = "";
                                if (true)
                                {
                                    if (InfoText.Count > (GreenItem + 1))
                                    {
                                        SatChan[5] = InfoText[GreenItem + 1];
                                        if (SatChan[5].StartsWith("-"))
                                        {
                                            SatChan[5] = SatChan[5].Substring(1).Trim();
                                        }
                                    }
                                }

                                InfoColor.ToString();
                                InfoText.ToString();
                            }
                            else
                            {
                                SatChan[4] = Prepare(NN.ChildNodes[RowlistTD[i][TDi]].InnerText);
                                int ColSpan = 1;
                                if (NN.ChildNodes[RowlistTD[i][TDi]].Attributes.Contains("colspan"))
                                {
                                    ColSpan = ToInt(NN.ChildNodes[RowlistTD[i][TDi]].Attributes["colspan"].Value);
                                    SatChan[5] = ColSpan.ToString();
                                }
                                if (ColSpan == 1)
                                {
                                    TDi++;
                                    SatChan[5] = Prepare(NN.ChildNodes[RowlistTD[i][TDi]].InnerText);
                                }
                                else
                                {
                                    if (ColSpan == 2)
                                    {
                                        SatChan[5] = "";
                                    }
                                }
                            }
                            TDi++;

                            // APID Lang.
                            PrepareInfo(NN.ChildNodes[RowlistTD[i][TDi]]);
                            if (PrepareChannel == 2)
                            {
                                if (InfoColor[0] == "darkgreen")
                                {
                                    InfoColor.RemoveAt(0);
                                    InfoText.RemoveAt(0);
                                }
                                if (InfoColor[0] == "blue")
                                {
                                    InfoColor.RemoveAt(0);
                                    InfoText.RemoveAt(0);
                                }
                            }
                            SatChan[6] = LangList();

                            // Type
                            if (TypeByPID)
                            {
                                ChannelType = "UNK";
                                if ((SatChan[5] == "") && (SatChan[6] == ""))
                                {
                                    ChannelType = "DATA";
                                }
                                if ((SatChan[5] != "") && (SatChan[6] != ""))
                                {
                                    ChannelType = "TV";
                                }
                                if ((SatChan[5] == "") && (SatChan[6] != ""))
                                {
                                    ChannelType = "R";
                                }
                                if ((SatChan[5] != "") && (SatChan[6] == ""))
                                {
                                    ChannelType = "IMG";
                                }
                            }
                            SatChan[0] = ChannelType;

                            PrepareChannel = 0;

                            // Row
                            SatChan[7] = RowN.ToString();
                        }


                        if (SatChan != null)
                        {
                            FS2W.Write(SatTrans[0]);
                            FS2W.Write("\t");
                            FS2W.Write(SatTrans[1]);
                            FS2W.Write("\t");
                            FS2W.Write(SatTrans[7]);
                            FS2W.Write("\t");
                            FS2W.Write(SatTrans[3]);
                            for (int ii = 0; ii < 8; ii++)
                            {
                                FS2W.Write("\t");
                                FS2W.Write(SatChan[ii]);
                            }
                            FS2W.WriteLine();
                        }

                    }
                }
                //FS1W.WriteLine("TABLE END");
            }
        }
	}
}
