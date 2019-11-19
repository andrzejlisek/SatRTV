using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatRTV
{
    class AppCoreSat_3FlySat : AppCoreSat
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
            FS1W.WriteLine("Txp No\tSource\tDate\tFreq\tPol\tMode\tSR-FEC\tProvider Name\tEncryption\tONID-TID\tFootPrints\tEIRP(dBW)\tComments\tRow");

            FileStream FS2 = new FileStream(ChanFileName(I), FileMode.CreateNew, FileAccess.Write);
            StreamWriter FS2W = new StreamWriter(FS2);
            FS2W.WriteLine("Freq\tPol\tSR\tBeam\tType\tChannel Name\tFTA\tV.PID\tA.PID\tSID\tRow");

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
                                if (HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes[iiii].Name == "table")
                                {
                                    int RowN = 0;
                                    for (int iiiii = 0; iiiii < HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes[iiii].ChildNodes.Count; iiiii++)
                                    {
                                        if (HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes[iiii].ChildNodes[iiiii].Name == "tr")
                                        {
                                            RowN++;
                                        }
                                    }
                                    if (RowN >= 3)
                                    {
                                        ChannelTable(I, HTMLDoc.DocumentNode.ChildNodes[ii].ChildNodes[iii].ChildNodes[iiii], ref FS1W, ref FS2W, TypeByPID);
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

        private void ChannelTable(int SatN, HtmlAgilityPack.HtmlNode N, ref StreamWriter FS1W, ref StreamWriter FS2W, bool TypeByPID)
        {
            PrepareInfoFlySat = true;

            List<List<HtmlAgilityPack.HtmlNode>> TableObj = new List<List<HtmlAgilityPack.HtmlNode>>();

            for (int i = 0; i < N.ChildNodes.Count; i++)
            {
                if (N.ChildNodes[i].Name == "tr")
                {
                    List<HtmlAgilityPack.HtmlNode> TableObjRow = new List<HtmlAgilityPack.HtmlNode>();
                    for (int ii = 0; ii < N.ChildNodes[i].ChildNodes.Count; ii++)
                    {
                        if (N.ChildNodes[i].ChildNodes[ii].Name == "td")
                        {
                            TableObjRow.Add(N.ChildNodes[i].ChildNodes[ii]);
                        }
                    }

                    if ((TableObjRow.Count > 3) || ((TableObjRow.Count == 1) && TableObjRow[0].InnerText.Contains("Stream")))
                    {
                        if ((!TableObjRow[0].InnerText.Contains("Txp No")) && (!TableObjRow[0].InnerText.Contains("Source")) && (!TableObjRow[0].InnerText.Contains("Date")))
                        {
                            TableObj.Add(TableObjRow);
                        }
                    }
                }
            }

            string[] SatTrans = null;
            string[] SatChan = null;

            int RowN0 = 0;
            int RowN = 0;

            int RowSpanCounter = 0;
            for (int i = 0; i < TableObj.Count; i++)
            {
                if (RowSpanCounter == 0)
                {
                    RowN0 = RowN;
                    RowN = i;

                    try
                    {

                        SatTrans = new string[20];

                        int TdI = 0;
                        PrepareInfo(TableObj[i][TdI]);

                        // Txp No
                        SatTrans[0] = Prepare(InfoText[0]);

                        // Source
                        int TempI = 0;
                        SatTrans[1] = "";
                        for (int ii = 1; ii < (InfoText.Count - 1); ii++)
                        {
                            if (Prepare(InfoText[ii]) != "")
                            {
                                if (TempI > 0)
                                {
                                    SatTrans[1] += Separator;
                                }
                                SatTrans[1] += Prepare(InfoText[ii]);
                                TempI++;
                            }
                        }

                        // Date
                        SatTrans[2] += Prepare(InfoText[InfoText.Count - 1]);
                        TdI++;

                        // Freq, Pol
                        PrepareInfoFlySat = false;
                        PrepareInfo(TableObj[i][TdI]);
                        PrepareInfoFlySat = true;
                        TableObj[i][TdI].InnerText.ToString();

                        string TempS = InfoText[0].Trim();
                        TempI = TempS.IndexOf(" ");
                        if (TempI < 0)
                        {
                            throw new Exception("XXX");
                        }
                        SatTrans[3] += TempS.Substring(0, TempI);
                        SatTrans[4] += TempS.Substring(TempI + 1);

                        // Mode
                        SatTrans[5] += "";
                        for (int ii = 1; ii < InfoText.Count; ii++)
                        {
                            if (ii > 1)
                            {
                                SatTrans[5] += Separator;
                            }
                            SatTrans[5] += Prepare(InfoText[ii]);
                        }

                        // SR-FEC
                        TdI++;
                        PrepareInfo(TableObj[i][TdI]);
                        SatTrans[6] = "";
                        for (int ii = 0; ii < InfoText.Count; ii++)
                        {
                            if (ii > 0)
                            {
                                SatTrans[6] += Separator;
                            }
                            SatTrans[6] += Prepare(InfoText[ii]);
                        }

                        // Provider Name
                        TdI++;
                        PrepareInfo(TableObj[i][TdI]);
                        if (InfoText.Count > 0)
                        {
                            SatTrans[7] = Prepare(InfoText[0]);
                        }
                        else
                        {
                            SatTrans[7] = "";
                        }

                        // Encryption
                        TdI++;
                        TdI++;
                        TdI++;
                        TdI++;
                        PrepareInfo(TableObj[i][TdI]);
                        TempI = 0;
                        SatTrans[8] = "";
                        for (int ii = 0; ii < InfoText.Count; ii++)
                        {
                            if (InfoColor[ii] == "")
                            {
                                if (TempI > 0)
                                {
                                    SatTrans[8] += Separator;
                                }
                                SatTrans[8] += Prepare(InfoText[ii]);
                                TempI++;
                            }
                        }

                        // ONID-TID
                        TempI = 0;
                        SatTrans[9] += "";
                        for (int ii = 0; ii < InfoText.Count; ii++)
                        {
                            if (InfoColor[ii] != "")
                            {
                                if (TempI > 0)
                                {
                                    SatTrans[9] += Separator;
                                }
                                SatTrans[9] += Prepare(InfoText[ii]);
                                TempI++;
                            }
                        }

                        // FootPrints
                        TdI++;
                        PrepareInfo(TableObj[i][TdI]);
                        if (InfoText[0] == "")
                        {
                            InfoText.RemoveAt(0);
                            InfoColor.RemoveAt(0);
                        }
                        if (InfoText.Count < 1)
                        {
                            throw new Exception("XXX");
                        }

                        SatTrans[10] = "";
                        for (int ii = 0; ii < (InfoText.Count - 1); ii++)
                        {
                            if (ii > 0)
                            {
                                SatTrans[10] += Separator;
                            }
                            SatTrans[10] += InfoText[ii];
                        }
                        if (!InfoText[InfoText.Count - 1].Contains("dBW"))
                        {
                            if (InfoText.Count > 1)
                            {
                                SatTrans[10] += Separator;
                            }
                            SatTrans[10] += InfoText[InfoText.Count - 1];
                        }
                        SatTrans[10] = SatTrans[10].Replace(Separator, " ");
                        BeamListAdd(SatTrans[10]);

                        // EIRP(dBW)
                        if (InfoText[InfoText.Count - 1].Contains("dBW"))
                        {
                            SatTrans[11] = InfoText[InfoText.Count - 1];
                        }
                        else
                        {
                            SatTrans[11] = "";
                        }

                        // Comments
                        TdI++;
                        PrepareInfo(TableObj[i][TdI]);
                        TempI = 0;
                        SatTrans[12] = "";
                        for (int ii = 0; ii < InfoText.Count; ii++)
                        {
                            if (InfoText[ii] != "")
                            {
                                if (TempI > 0)
                                {
                                    SatTrans[12] += Separator;
                                }
                                SatTrans[12] += Prepare(InfoText[ii]);
                                TempI++;
                            }
                        }

                        // Row
                        SatTrans[13] = RowN.ToString();


                        for (int ii = 0; ii < 14; ii++)
                        {
                            if (ii > 0)
                            {
                                FS1W.Write("\t");
                            }
                            FS1W.Write(SatTrans[ii]);
                        }
                        FS1W.WriteLine();

                        if (TableObj[i][0].Attributes.Contains("rowspan"))
                        {
                            RowSpanCounter = ToInt(TableObj[i][0].Attributes["rowspan"].Value);
                        }
                        else
                        {
                            RowSpanCounter = 1;
                        }
                        if (SpanChange[SatN].ContainsKey(RowN))
                        {
                            RowSpanCounter += SpanChange[SatN][RowN];
                        }
                    }
                    catch (Exception e)
                    {
                        string ErrMsg = "";
                        ErrMsg = ErrMsg + "Previous row: " + RowN0.ToString() + "\r\n";
                        ErrMsg = ErrMsg + "Current row: " + RowN.ToString() + "\r\n";
                        if (SatTrans != null)
                        {
                            ErrMsg = ErrMsg + "Transponder:";
                            for (int ii = 0; ii < 12; ii++)
                            {
                                ErrMsg = ErrMsg + "[" + SatTrans[ii] + "]";
                            }
                            ErrMsg = ErrMsg + "\r\n";
                        }
                        if (SatChan != null)
                        {
                            ErrMsg = ErrMsg + "Channel:";
                            for (int ii = 0; ii < 8; ii++)
                            {
                                ErrMsg = ErrMsg + "[" + SatChan[ii] + "]";
                            }
                            ErrMsg = ErrMsg + "\r\n";
                        }
                        ErrMsg = ErrMsg + "Parse error: " + e.Message + "\r\n" + e.StackTrace;
                        FS1W.Close();
                        FS2W.Close();
                        throw new Exception(ErrMsg);
                    }
                }
                else
                {
                    if (TableObj[i].Count > 3)
                    {
                        SatChan = new string[20];

                        // Channel Name
                        int TdI = 0;
                        SatChan[4] = Prepare(TableObj[i][TdI].InnerText);

                        // FTA
                        TdI++;
                        SatChan[5] = (TableObj[i][TdI].InnerText.Contains("F")) ? "Yes" : "No";

                        // V.PID
                        TdI++;
                        SatChan[6] = Prepare(TableObj[i][TdI].InnerText);

                        // A.PID
                        TdI++;
                        PrepareInfo(TableObj[i][TdI]);
                        for (int ii = InfoText.Count - 1; ii >= 0; ii--)
                        {
                            if (InfoText[ii].Contains("AC3"))
                            {
                                InfoText.RemoveAt(ii);
                                InfoColor.RemoveAt(ii);
                            }
                        }
                        SatChan[7] = LangList();

                        // SID
                        TdI++;
                        PrepareInfo(TableObj[i][TdI]);
                        SatChan[8] = "";
                        if (InfoText.Count > 0)
                        {
                            if (!InfoText[0].Contains("MPEG"))
                            {
                                SatChan[8] = Prepare(InfoText[0]);
                            }
                        }

                        // Type
                        SatChan[3] = "UNK";
                        if (TypeByPID)
                        {
                            SatChan[3] = "UNK";
                            if ((SatChan[6] == "") && (SatChan[7] == ""))
                            {
                                SatChan[3] = "DATA";
                            }
                            if ((SatChan[6] != "") && (SatChan[7] != ""))
                            {
                                SatChan[3] = "TV";
                            }
                            if ((SatChan[6] == "") && (SatChan[7] != ""))
                            {
                                SatChan[3] = "R";
                            }
                            if ((SatChan[6] != "") && (SatChan[7] == ""))
                            {
                                SatChan[3] = "IMG";
                            }
                        }

                        // Row
                        SatChan[9] = RowN.ToString();

                        // Freq
                        FS2W.Write(SatTrans[3]);

                        // Pol
                        FS2W.Write("\t");
                        FS2W.Write(SatTrans[4]);

                        // SR
                        FS2W.Write("\t");
                        FS2W.Write(SatTrans[6]);

                        // Beam
                        FS2W.Write("\t");
                        FS2W.Write(SatTrans[10]);

                        for (int ii = 3; ii < 10; ii++)
                        {
                            if (ii > 0)
                            {
                                FS2W.Write("\t");
                            }
                            FS2W.Write(SatChan[ii]);
                        }

                        FS2W.WriteLine();
                    }
                }
                RowSpanCounter--;
            }
            PrepareInfoFlySat = false;
        }
    }
}
