/*
 * Created by SharpDevelop.
 * User: XXX
 * Date: 2020-03-23
 * Time: 11:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace SatRTV
{
	/// <summary>
	/// Description of AppCoreSat_4SatBeams.
	/// </summary>
	public class AppCoreSat_4SatBeams : AppCoreSat
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
            FS1W.WriteLine("Freq\tPol\tSR\tFEC\tSat\tBeam\tMod\tFormat\tInfo\tRow");

            FileStream FS2 = new FileStream(ChanFileName(I), FileMode.CreateNew, FileAccess.Write);
            StreamWriter FS2W = new StreamWriter(FS2);
            FS2W.WriteLine("Freq\tPol\tSR\tBeam\tType\tChannel\tT\tEncryption\tFTA\tPackage\tRes\tCompression\tV_PID\tA_PID\tSID\tOwner\tDate\tRow");

            BeamList.Clear();

            HtmlAgilityPack.HtmlDocument HTMLDoc = new HtmlAgilityPack.HtmlDocument();
            FileStream HFS = new FileStream(DataFileName(I), FileMode.Open, FileAccess.Read);
            StreamReader HFSR = new StreamReader(HFS);
            string RAW = HFSR.ReadToEnd();
            HFSR.Close();
            HFS.Close();
            HTMLDoc.LoadHtml(RAW);

            HtmlAgilityPack.HtmlNode ChannelTable = null;
            ChannelGetNode(HTMLDoc.DocumentNode, ref ChannelTable);

            string[] SatTrans = null;
            string[] SatChan = null;

            if (ChannelTable != null)
            {
            	for (int i = 0; i < ChannelTable.ChildNodes.Count; i++)
            	{
            		if (ChannelTable.ChildNodes[i].Name.ToUpperInvariant() == "TR")
            		{
            			List<HtmlAgilityPack.HtmlNode> TD = new List<HtmlAgilityPack.HtmlNode>();
            			for (int ii = 0; ii < ChannelTable.ChildNodes[i].ChildNodes.Count; ii++)
            			{
            				if ((ChannelTable.ChildNodes[i].ChildNodes[ii].Name.ToUpperInvariant() == "TD") || (ChannelTable.ChildNodes[i].ChildNodes[ii].Name.ToUpperInvariant() == "TH"))
            				{
            					TD.Add(ChannelTable.ChildNodes[i].ChildNodes[ii]);
            				}
            			}
            			
            			
            			// Transponder
            			if (ChannelTable.ChildNodes[i].Attributes["class"].Value == "index group_tr")
            			{
            				if (TD[1].Attributes["class"].Value == "group_td nobr")
            				{
            					HtmlAgilityPack.HtmlNode NN = TD[1].LastChild;
            					SatTrans = new string[20];
            					
            					SatTrans[0] = ClearString(NN.ChildNodes[1].InnerText);
            					SatTrans[1] = ClearString(NN.ChildNodes[2].InnerText);
            					SatTrans[2] = ClearString(NN.ChildNodes[3].InnerText);
            					SatTrans[3] = ClearString(NN.ChildNodes[4].InnerText);
            					SatTrans[4] = ClearString(NN.ChildNodes[5].InnerText);
            					SatTrans[5] = ClearString(NN.ChildNodes[6].InnerText);
            					SatTrans[6] = ClearString(NN.ChildNodes[7].InnerText);
            					SatTrans[7] = ClearString(NN.ChildNodes[8].InnerText);
            					SatTrans[8] = ClearString(NN.ChildNodes[9].InnerText);
            					
            					FS1W.Write(SatTrans[0]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[1]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[2]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[3]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[4]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[5]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[6]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[7]);
            					FS1W.Write("\t");
            					FS1W.Write(SatTrans[8]);
            					FS1W.Write("\t");
            					FS1W.Write(i.ToString());
            					FS1W.WriteLine();
            					BeamListAdd(SatTrans[5]);
            				}
            			}

            			// Channel
            			if (ChannelTable.ChildNodes[i].Attributes["class"].Value.StartsWith("index class_tr"))
            			{
            				if (TD.Count == 16)
            				{
            					SatChan = new string[20];
            					
            					SatChan[0] = ClearString(RemoveEscape(TD[2].InnerText));
            					SatChan[1] = ClearString(TD[3].InnerText);
            					
            					// Encryption
            					PrepareInfo2(TD[4]);
            					SatChan[2] = "";
            					for (int ii = 0; ii < InfoText.Count; ii++)
            					{
            						if (SatChan[2] != "")
            						{
            							SatChan[2] += "|";
            						}
            						SatChan[2] += InfoText[ii];
            					}
            					
            					// Package
            					PrepareInfo2(TD[5]);
            					SatChan[3] = "";
            					for (int ii = 0; ii < InfoText.Count; ii++)
            					{
            						if (SatChan[3] != "")
            						{
            							SatChan[3] += "|";
            						}
            						SatChan[3] += InfoText[ii];
            					}

            					SatChan[4] = ClearString(TD[6].InnerText);
            					SatChan[5] = ClearString(TD[7].InnerText);
            					SatChan[6] = ClearString(TD[8].InnerText);
            					
            					PrepareInfo2(TD[9]);
            					for (int ii = 0; ii < InfoText.Count; ii++)
            					{
            						InfoText[ii] = LangRemCodec(InfoText[ii], "AAC");
            						InfoText[ii] = LangRemCodec(InfoText[ii], "AC3");
            						InfoText[ii] = LangRemCodec(InfoText[ii], "DD");
            						InfoText[ii] = LangRemCodec(InfoText[ii], "(E-AC3)");
            					}
            					List<string> InfoTextX = new List<string>();
            					for (int ii = 0; ii < InfoText.Count; ii++)
            					{
            						string[] X = InfoText[ii].Split(' ');
            						for (int iii = 1; iii < X.Length; iii++)
            						{
            							InfoTextX.Add(X[0] + " " + X[iii]);
            						}
            					}
            					InfoText.Clear();
            					for (int ii = 0; ii < InfoTextX.Count; ii++)
            					{
            						InfoText.Add(InfoTextX[ii]);
            					}
            					
            					
            					bool HasV = ((SatChan[6] != "") && (SatChan[6] != "0"));
            					bool HasA = false;
            					
            					
            					LangList();
            					SatChan[7] = "";
            					for (int ii = 0; ii < InfoText.Count; ii++)
            					{
            						if (SatChan[7] != "")
            						{
            							SatChan[7] += "|";
            						}
            						SatChan[7] += InfoText[ii];
            						HasA = true;
            					}

            					SatChan[8] = ClearString(TD[10].InnerText);
            					SatChan[9] = ClearString(TD[11].InnerText);
            					SatChan[10] = ClearString(TD[12].InnerText);
            					
            					FS2W.Write(SatTrans[0]);
            					FS2W.Write("\t");
            					FS2W.Write(SatTrans[1]);
            					FS2W.Write("\t");
            					FS2W.Write(SatTrans[2]);
            					FS2W.Write("\t");
            					FS2W.Write(SatTrans[5]);
            					FS2W.Write("\t");
            					if ((!HasA) && (!HasV))
            					{
            						FS2W.Write("DATA");
            					}
            					if ((HasA) && (!HasV))
            					{
            						FS2W.Write("R");
            					}
            					if ((!HasA) && (HasV))
            					{
            						FS2W.Write("IMG");
            					}
            					if ((HasA) && (HasV))
            					{
            						FS2W.Write("TV");
            					}
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[0]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[1]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[2]);
            					FS2W.Write("\t");
            					
            					bool IsFTA = false;
            					if (SatChan[2].ToUpperInvariant().Contains("|FTA|"))
            					{
            						IsFTA = true;
            					}
            					if (SatChan[2].ToUpperInvariant().StartsWith("FTA|"))
            					{
            						IsFTA = true;
            					}
            					if (SatChan[2].ToUpperInvariant().EndsWith("|FTA"))
            					{
            						IsFTA = true;
            					}
            					if (SatChan[2].ToUpperInvariant().Equals("FTA"))
            					{
            						IsFTA = true;
            					}
            					FS2W.Write(IsFTA ? "Yes" : "No");

            					FS2W.Write("\t");
            					FS2W.Write(SatChan[3]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[4]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[5]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[6]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[7]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[8]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[9]);
            					FS2W.Write("\t");
            					FS2W.Write(SatChan[10]);
            					FS2W.Write("\t");
            					FS2W.Write(i.ToString());
            					FS2W.Write("\t");


            					FS2W.WriteLine();
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
        
        string LangRemCodec(string LangTag, string CodecInfo)
        {
        	if (LangTag.StartsWith(CodecInfo))
        	{
        		LangTag = LangTag.Substring(CodecInfo.Length);
        	}
        	if (LangTag.EndsWith(CodecInfo))
        	{
        		LangTag = LangTag.Substring(0, LangTag.Length - CodecInfo.Length);
        	}
        	LangTag = LangTag.Replace(" " + CodecInfo + " ", " ");
        	return LangTag;
        }
        
        void ChannelGetNode(HtmlAgilityPack.HtmlNode DocNode, ref HtmlAgilityPack.HtmlNode TableNode)
        {
        	if (DocNode.Name.ToUpperInvariant() == "TABLE")
        	{
        		if (DocNode.Id == "channel_grid")
        		{
        			TableNode = DocNode;
        		}
        	}
        	if (DocNode.HasChildNodes)
        	{
        		for (int i = 0; i < DocNode.ChildNodes.Count; i++)
        		{
        			ChannelGetNode(DocNode.ChildNodes[i], ref TableNode);
        		}
        	}
        }
	}
}
