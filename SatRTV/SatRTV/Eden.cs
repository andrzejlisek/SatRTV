/*
 * Created by SharpDevelop.
 * User: XXX
 * Date: 2020-03-23
 * Time: 11:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SatRTV
{
	/// <summary>
	/// Description of Eden.
	/// </summary>
	public class Eden
	{
	    public static int HexToInt(string Hex0)
	    {
	        int L = Hex0.Length;
	        string Hex = "";
	        for (int i = 0; i < L; i++)
	        {
	            Hex = Hex0[i] + Hex;
	        }
	        int D = 0;
	        int N = 0;
	        for (int i = L - 1; i >= 0; i--)
	        {
	            D = 0;
	            if (Hex[i] == '0') { D = 0; }
	            if (Hex[i] == '1') { D = 1; }
	            if (Hex[i] == '2') { D = 2; }
	            if (Hex[i] == '3') { D = 3; }
	            if (Hex[i] == '4') { D = 4; }
	            if (Hex[i] == '5') { D = 5; }
	            if (Hex[i] == '6') { D = 6; }
	            if (Hex[i] == '7') { D = 7; }
	            if (Hex[i] == '8') { D = 8; }
	            if (Hex[i] == '9') { D = 9; }
	            if (Hex[i] == 'A') { D = 10; }
	            if (Hex[i] == 'B') { D = 11; }
	            if (Hex[i] == 'C') { D = 12; }
	            if (Hex[i] == 'D') { D = 13; }
	            if (Hex[i] == 'E') { D = 14; }
	            if (Hex[i] == 'F') { D = 15; }
	            if (Hex[i] == 'a') { D = 10; }
	            if (Hex[i] == 'b') { D = 11; }
	            if (Hex[i] == 'c') { D = 12; }
	            if (Hex[i] == 'd') { D = 13; }
	            if (Hex[i] == 'e') { D = 14; }
	            if (Hex[i] == 'f') { D = 15; }
	            if (i == 0) { N = N + (D); }
	            if (i == 1) { N = N + (D * 16); }
	            if (i == 2) { N = N + (D * 256); }
	            if (i == 3) { N = N + (D * 4096); }
	            if (i == 4) { N = N + (D * 65536); }
	            if (i == 5) { N = N + (D * 1048576); }
	            if (i == 6) { N = N + (D * 16777216); }
	            if (i == 7) { N = N + (D * 268435456); }
	        }
	        return N;
	    }
	}
}
