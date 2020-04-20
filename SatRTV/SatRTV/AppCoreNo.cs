/*
 * Created by SharpDevelop.
 * User: XXX
 * Date: 2020-04-19
 * Time: 19:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace SatRTV
{
    /// <summary>
    /// Description of AppCoreNo.
    /// </summary>
    public class AppCoreNo
    {
        public string[] TransNoFields;
        public string[] TransNoMode;
        public string[] ChanNoFields;
        public string[] ChanNoMode;
        public string TempDir;
        int SourceCount = 4;

        public AppCoreNo()
        {
        }

        void CreateNoList(string Priority, string FileNamePattern, string[] FieldList, string[] FieldMode, List<bool> SatSelected)
        {
            for (int Sat = 0; Sat < SatSelected.Count; Sat++)
            {
                if (SatSelected[Sat])
                {
                    Console.Write("Satellite " + Sat.ToString() + "...");

                    List<string[]>[] NoListRaw = new List<string[]>[SourceCount + 1];
                    int[] NoFld = new int[SourceCount + 1];
                    int MaxItemNo = 0;

                    for (int i = 1; i <= SourceCount; i++)
                    {
                        if (Priority.Contains(i.ToString()))
                        {
                            NoListRaw[i] = new List<string[]>();

                            FileStream FS = new FileStream(TempDir + "Data" + i.ToString() + Path.DirectorySeparatorChar.ToString() + FileNamePattern.Replace("?", Sat.ToString().PadLeft(3, '0')), FileMode.Open, FileAccess.Read);
                            StreamReader SR = new StreamReader(FS);
                            string Str = SR.ReadLine();
                            bool IsHeader = true;
                            List<string> HeaderFld = new List<string>();
                            int[] HeaderIdx = new int[FieldList.Length];
                            int LineN = 0;
                            while (Str != null)
                            {
                                LineN++;
                                if (IsHeader)
                                {
                                    HeaderFld.AddRange(Str.Split('\t'));
                                    NoFld[i] = HeaderFld.IndexOf("No");
                                    if (NoFld[i] < 0)
                                    {
                                        SR.Close();
                                        FS.Close();
                                        Console.WriteLine("No \"No\" field in source" + i.ToString() + ".");
                                        return;
                                    }
                                    for (int ii = 0; ii < FieldList.Length; ii++)
                                    {
                                        HeaderIdx[ii] = HeaderFld.IndexOf(FieldList[ii]);
                                        if (HeaderIdx[ii] < 0)
                                        {
                                            SR.Close();
                                            FS.Close();
                                            Console.WriteLine("No \"" + FieldList[ii] + "\" field in source" + i.ToString() + ".");
                                            return;
                                        }
                                    }
                                    IsHeader = false;
                                }
                                else
                                {
                                    int ItempNo = 0;
                                    string[] Str_ = Str.Split('\t');
                                    if (int.TryParse(Str_[NoFld[i]], out ItempNo))
                                    {
                                        if (ItempNo > 0)
                                        {
                                            if (NoListRaw[i].Count > (ItempNo + 1))
                                            {
                                                SR.Close();
                                                FS.Close();
                                                Console.WriteLine("Channel " + ItempNo.ToString() + " not in order in source " + i.ToString() + ", line " + LineN.ToString() + ".");
                                                return;
                                            }
                                            while (NoListRaw[i].Count <= ItempNo)
                                            {
                                                NoListRaw[i].Add(null);
                                            }
                                            if (MaxItemNo < ItempNo)
                                            {
                                                MaxItemNo = ItempNo;
                                            }
                                            if (NoListRaw[i][ItempNo] != null)
                                            {
                                                SR.Close();
                                                FS.Close();
                                                Console.WriteLine("Ambiguous channel " + ItempNo.ToString() + " in source " + i.ToString() + ", line " + LineN.ToString() + ".");
                                                return;
                                            }
                                            string[] Str__ = new string[FieldList.Length];
                                            for (int ii = 0; ii < FieldList.Length; ii++)
                                            {
                                                Str__[ii] = Str_[HeaderIdx[ii]];
                                            }
                                            NoListRaw[i][ItempNo] = Str__;
                                        }
                                    }
                                }
                                Str = SR.ReadLine();
                            }
                            SR.Close();
                            FS.Close();
                        }
                    }
                    for (int i = 1; i <= SourceCount; i++)
                    {
                        if (Priority.Contains(i.ToString()))
                        {
                            while (NoListRaw[i].Count <= MaxItemNo)
                            {
                                NoListRaw[i].Add(null);
                            }
                        }
                    }

                    FileStream FS2 = new FileStream(TempDir + FileNamePattern.Replace("?", Sat.ToString().PadLeft(3, '0')), FileMode.Create, FileAccess.Write);
                    StreamWriter SW = new StreamWriter(FS2);
                    for (int i = 0; i < FieldList.Length; i++)
                    {
                        if (i > 0)
                        {
                            SW.Write("\t");
                        }
                        SW.Write(FieldList[i]);
                    }
                    SW.WriteLine();
                    for (int i = 1; i <= MaxItemNo; i++)
                    {
                        for (int ii = 0; ii < FieldList.Length; ii++)
                        {
                            if (ii > 0)
                            {
                                SW.Write("\t");
                            }

                            if (FieldList[ii] == "No")
                            {
                                SW.Write(i.ToString());
                            }
                            else
                            {
                                string StrTemp = "";
                                List<string> StrTemp_ = new List<string>();
                                for (int iii = 0; iii < Priority.Length; iii++)
                                {
                                    int PrN = int.Parse(Priority[iii].ToString());
                                    if (NoListRaw[PrN][i] != null)
                                    {
                                        string NoListRaw_ = NoListRaw[PrN][i][ii];
                                        if ((NoListRaw_ != null) && (NoListRaw_ != ""))
                                        {
                                            switch (FieldMode[ii])
                                            {
                                                default:
                                                    {
                                                        StrTemp = NoListRaw_;
                                                        iii = Priority.Length;
                                                    }
                                                    break;
                                                case "1":
                                                    {
                                                        if (!StrTemp_.Contains(NoListRaw_))
                                                        {
                                                            if (StrTemp_.Count > 0)
                                                            {
                                                                StrTemp = StrTemp + "|";
                                                            }
                                                            StrTemp = StrTemp + NoListRaw_;
                                                            StrTemp_.Add(NoListRaw_);
                                                        }
                                                    }
                                                    break;
                                                case "2":
                                                    {
                                                        if (NoListRaw_.StartsWith("[", StringComparison.InvariantCulture) && NoListRaw_.EndsWith("]", StringComparison.InvariantCulture))
                                                        {
                                                            string LangTempS = NoListRaw_.Substring(1, NoListRaw_.Length - 2);
                                                            if (LangTempS.Replace("][", "").Contains("[") || LangTempS.Replace("][", "").Contains("]"))
                                                            {
                                                                SW.Close();
                                                                FS2.Close();
                                                                Console.WriteLine("Incorrect language data on channel " + i.ToString() + " in source" + PrN.ToString() + ".");
                                                                return;
                                                            }
                                                            string[] LangTemp = LangTempS.Replace("][", "[").Split('[');
                                                            for (int iiii = 0; iiii < LangTemp.Length; iiii++)
                                                            {
                                                                if (!StrTemp_.Contains(LangTemp[iiii]))
                                                                {
                                                                    StrTemp_.Add(LangTemp[iiii]);
                                                                }
                                                            }
                                                            StrTemp_.Sort();
                                                            StrTemp = "";
                                                            for (int iiii = 0; iiii < StrTemp_.Count; iiii++)
                                                            {
                                                                StrTemp = StrTemp + "[" + StrTemp_[iiii] + "]";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            SW.Close();
                                                            FS2.Close();
                                                            Console.WriteLine("Incorrect language data on channel " + i.ToString() + " in source" + PrN.ToString() + ".");
                                                            return;
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                SW.Write(StrTemp);
                            }
                        }
                        SW.WriteLine();
                    }
                    SW.Close();
                    FS2.Close();

                    Console.WriteLine("OK");
                }
            }
        }

        public void TransNo(string Priority, List<bool> SatList)
        {
            CreateNoList(ClearPriority(Priority), "TransList?.txt", TransNoFields, TransNoMode, SatList);
        }

        public void ChanNo(string Priority, List<bool> SatList)
        {
            CreateNoList(ClearPriority(Priority), "ChanList?.txt", ChanNoFields, ChanNoMode, SatList);
        }

        string ClearPriority(string P)
        {
            string P_ = "";
            string Chr = "";
            for (int i = 1; i <= SourceCount; i++)
            {
                Chr = Chr + i.ToString();
            }
            for (int i = 0; i < P.Length; i++)
            {
                string Pi = P[i].ToString();
                if ((Chr.Contains(Pi)) && (!P_.Contains(Pi)))
                {
                    P_ = P_ + Pi;
                }
            }
            return P_;
        }
    }
}
