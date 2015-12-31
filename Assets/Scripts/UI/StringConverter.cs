using UnityEngine;
using System.Collections;
using System.Text;


using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;

[ExecuteInEditMode]
public class StringConverter : MonoBehaviour {


	public string originalString;

	public string outputString;


	public string chineseString;

	public string[] splitString;

	public bool update;


	public byte[] utf8;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(update){



			update = false;


			char[] chars = originalString.ToCharArray();

			Debug.Log("  chars " + chars.Length);
//			

//			Encoding.Unicode.GetChars



			splitString = originalString.Split(';');

			for (int i = 0; i < splitString.Length; i++) {

//				splitString[i] = "\\u" + splitString[i].Substring(3);

				splitString[i] = splitString[i].Replace("&#x", "\\u");

			}


			for (int i = 0; i < splitString.Length; i++) {

				string currentString = splitString[i];

				int p = int.Parse(currentString.Substring(2), System.Globalization.NumberStyles.HexNumber);

				splitString[i] = ((char)p).ToString();
				
			}



			char upArrow = '\u6578';

		

			Debug.Log("upArrow " + upArrow);


			string testString = "\u6578";

			char[] stirngTestChar = testString.ToCharArray();

			Debug.Log("  stirngTestChar size " + stirngTestChar.Length);

			for (int i = 0; i < stirngTestChar.Length; i++) {
				
				Debug.Log("  stirngTestChar -------- " + stirngTestChar[i]);
				
			}



			Debug.Log("  chineseString encoded " + HtmlEntities.Encode(chineseString));



//			string unicodeString = "This string contains the unicode character Pi (\u03a0)";

//			Debug.Log(" unicode string " + ((char) Convert.ToInt32("u6578", 16) ).ToString() );



			string maybeC = "U+6578";
			int tp = int.Parse(maybeC.Substring(2), System.Globalization.NumberStyles.HexNumber);
			Debug.Log("  maybeC " + (char)tp);


//			Debug.Log(" encode " + Encoding.UTF32.GetEncoder().
//			Encoding.Unicode.

//			outputString = HtmlEntities.Decode(originalString);
//			char[] charTest = '&#x6578';
//
//			Debug.Log(charTest);

//			Debug.Log(" utf 8 is " + utf8.ToString() );
//
//			for (int i = 0; i < utf8.Length; i++) {
//
//				Debug.Log("  utf8 " + utf8[i]);
//			}
//				GetBytes(originalString);

//			byte[] utf8 = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, ascii);

//			Encoding.ASCII.GetString

//			Encoding.Convert

//			outputString = originalString.Length.ToString();

		}
	
	}




}




class HtmlEntities
{
	
	
	private static Dictionary<string, int> EntitiesToCodePoints;
	private static Dictionary<int, string> CodePointsToEntities;
	
	private static void Init()
	{
		if (EntitiesToCodePoints == null || CodePointsToEntities == null)
		{
			EntitiesToCodePoints = new Dictionary<string, int>();
			CodePointsToEntities = new Dictionary<int, string>();
			
			EntitiesToCodePoints["nbsp"] = 160; EntitiesToCodePoints["iexcl"] = 161; EntitiesToCodePoints["cent"] = 162; EntitiesToCodePoints["pound"] = 163; EntitiesToCodePoints["curren"] = 164; EntitiesToCodePoints["yen"] = 165; EntitiesToCodePoints["brvbar"] = 166; EntitiesToCodePoints["sect"] = 167; EntitiesToCodePoints["uml"] = 168; EntitiesToCodePoints["copy"] = 169; EntitiesToCodePoints["ordf"] = 170; EntitiesToCodePoints["laquo"] = 171; EntitiesToCodePoints["not"] = 172; EntitiesToCodePoints["shy"] = 173; EntitiesToCodePoints["reg"] = 174; EntitiesToCodePoints["macr"] = 175; EntitiesToCodePoints["deg"] = 176; EntitiesToCodePoints["plusmn"] = 177; EntitiesToCodePoints["sup2"] = 178; EntitiesToCodePoints["sup3"] = 179; EntitiesToCodePoints["acute"] = 180; EntitiesToCodePoints["micro"] = 181; EntitiesToCodePoints["para"] = 182; EntitiesToCodePoints["middot"] = 183; EntitiesToCodePoints["cedil"] = 184; EntitiesToCodePoints["sup1"] = 185; EntitiesToCodePoints["ordm"] = 186; EntitiesToCodePoints["raquo"] = 187; EntitiesToCodePoints["frac14"] = 188; EntitiesToCodePoints["frac12"] = 189; EntitiesToCodePoints["frac34"] = 190; EntitiesToCodePoints["iquest"] = 191; EntitiesToCodePoints["Agrave"] = 192; EntitiesToCodePoints["Aacute"] = 193; EntitiesToCodePoints["Acirc"] = 194; EntitiesToCodePoints["Atilde"] = 195; EntitiesToCodePoints["Auml"] = 196; EntitiesToCodePoints["Aring"] = 197; EntitiesToCodePoints["AElig"] = 198; EntitiesToCodePoints["Ccedil"] = 199; EntitiesToCodePoints["Egrave"] = 200; EntitiesToCodePoints["Eacute"] = 201; EntitiesToCodePoints["Ecirc"] = 202; EntitiesToCodePoints["Euml"] = 203; EntitiesToCodePoints["Igrave"] = 204; EntitiesToCodePoints["Iacute"] = 205; EntitiesToCodePoints["Icirc"] = 206; EntitiesToCodePoints["Iuml"] = 207; EntitiesToCodePoints["ETH"] = 208; EntitiesToCodePoints["Ntilde"] = 209; EntitiesToCodePoints["Ograve"] = 210; EntitiesToCodePoints["Oacute"] = 211; EntitiesToCodePoints["Ocirc"] = 212; EntitiesToCodePoints["Otilde"] = 213; EntitiesToCodePoints["Ouml"] = 214; EntitiesToCodePoints["times"] = 215; EntitiesToCodePoints["Oslash"] = 216; EntitiesToCodePoints["Ugrave"] = 217; EntitiesToCodePoints["Uacute"] = 218; EntitiesToCodePoints["Ucirc"] = 219; EntitiesToCodePoints["Uuml"] = 220; EntitiesToCodePoints["Yacute"] = 221; EntitiesToCodePoints["THORN"] = 222; EntitiesToCodePoints["szlig"] = 223; EntitiesToCodePoints["agrave"] = 224; EntitiesToCodePoints["aacute"] = 225; EntitiesToCodePoints["acirc"] = 226; EntitiesToCodePoints["atilde"] = 227; EntitiesToCodePoints["auml"] = 228; EntitiesToCodePoints["aring"] = 229; EntitiesToCodePoints["aelig"] = 230; EntitiesToCodePoints["ccedil"] = 231; EntitiesToCodePoints["egrave"] = 232; EntitiesToCodePoints["eacute"] = 233; EntitiesToCodePoints["ecirc"] = 234; EntitiesToCodePoints["euml"] = 235; EntitiesToCodePoints["igrave"] = 236; EntitiesToCodePoints["iacute"] = 237; EntitiesToCodePoints["icirc"] = 238; EntitiesToCodePoints["iuml"] = 239; EntitiesToCodePoints["eth"] = 240; EntitiesToCodePoints["ntilde"] = 241; EntitiesToCodePoints["ograve"] = 242; EntitiesToCodePoints["oacute"] = 243; EntitiesToCodePoints["ocirc"] = 244; EntitiesToCodePoints["otilde"] = 245; EntitiesToCodePoints["ouml"] = 246; EntitiesToCodePoints["divide"] = 247; EntitiesToCodePoints["oslash"] = 248; EntitiesToCodePoints["ugrave"] = 249; EntitiesToCodePoints["uacute"] = 250; EntitiesToCodePoints["ucirc"] = 251; EntitiesToCodePoints["uuml"] = 252; EntitiesToCodePoints["yacute"] = 253; EntitiesToCodePoints["thorn"] = 254; EntitiesToCodePoints["yuml"] = 255; EntitiesToCodePoints["OElig"] = 338; EntitiesToCodePoints["oelig"] = 339; EntitiesToCodePoints["Scaron"] = 352; EntitiesToCodePoints["scaron"] = 353; EntitiesToCodePoints["Yuml"] = 376; EntitiesToCodePoints["fnof"] = 402; EntitiesToCodePoints["circ"] = 710; EntitiesToCodePoints["tilde"] = 732; EntitiesToCodePoints["Alpha"] = 913; EntitiesToCodePoints["Beta"] = 914; EntitiesToCodePoints["Gamma"] = 915; EntitiesToCodePoints["Delta"] = 916; EntitiesToCodePoints["Epsilon"] = 917; EntitiesToCodePoints["Zeta"] = 918; EntitiesToCodePoints["Eta"] = 919; EntitiesToCodePoints["Theta"] = 920; EntitiesToCodePoints["Iota"] = 921; EntitiesToCodePoints["Kappa"] = 922; EntitiesToCodePoints["Lambda"] = 923; EntitiesToCodePoints["Mu"] = 924; EntitiesToCodePoints["Nu"] = 925; EntitiesToCodePoints["Xi"] = 926; EntitiesToCodePoints["Omicron"] = 927; EntitiesToCodePoints["Pi"] = 928; EntitiesToCodePoints["Rho"] = 929; EntitiesToCodePoints["Sigma"] = 931; EntitiesToCodePoints["Tau"] = 932; EntitiesToCodePoints["Upsilon"] = 933; EntitiesToCodePoints["Phi"] = 934; EntitiesToCodePoints["Chi"] = 935; EntitiesToCodePoints["Psi"] = 936; EntitiesToCodePoints["Omega"] = 937; EntitiesToCodePoints["alpha"] = 945; EntitiesToCodePoints["beta"] = 946; EntitiesToCodePoints["gamma"] = 947; EntitiesToCodePoints["delta"] = 948; EntitiesToCodePoints["epsilon"] = 949; EntitiesToCodePoints["zeta"] = 950; EntitiesToCodePoints["eta"] = 951; EntitiesToCodePoints["theta"] = 952; EntitiesToCodePoints["iota"] = 953; EntitiesToCodePoints["kappa"] = 954; EntitiesToCodePoints["lambda"] = 955; EntitiesToCodePoints["mu"] = 956; EntitiesToCodePoints["nu"] = 957; EntitiesToCodePoints["xi"] = 958; EntitiesToCodePoints["omicron"] = 959; EntitiesToCodePoints["pi"] = 960; EntitiesToCodePoints["rho"] = 961; EntitiesToCodePoints["sigmaf"] = 962; EntitiesToCodePoints["sigma"] = 963; EntitiesToCodePoints["tau"] = 964; EntitiesToCodePoints["upsilon"] = 965; EntitiesToCodePoints["phi"] = 966; EntitiesToCodePoints["chi"] = 967; EntitiesToCodePoints["psi"] = 968; EntitiesToCodePoints["omega"] = 969; EntitiesToCodePoints["thetasym"] = 977; EntitiesToCodePoints["upsih"] = 978; EntitiesToCodePoints["piv"] = 982; EntitiesToCodePoints["ensp"] = 8194; EntitiesToCodePoints["emsp"] = 8195; EntitiesToCodePoints["thinsp"] = 8201; EntitiesToCodePoints["zwnj"] = 8204; EntitiesToCodePoints["zwj"] = 8205; EntitiesToCodePoints["lrm"] = 8206; EntitiesToCodePoints["rlm"] = 8207; EntitiesToCodePoints["ndash"] = 8211; EntitiesToCodePoints["mdash"] = 8212; EntitiesToCodePoints["lsquo"] = 8216; EntitiesToCodePoints["rsquo"] = 8217; EntitiesToCodePoints["sbquo"] = 8218; EntitiesToCodePoints["ldquo"] = 8220; EntitiesToCodePoints["rdquo"] = 8221; EntitiesToCodePoints["bdquo"] = 8222; EntitiesToCodePoints["dagger"] = 8224; EntitiesToCodePoints["Dagger"] = 8225; EntitiesToCodePoints["bull"] = 8226; EntitiesToCodePoints["hellip"] = 8230; EntitiesToCodePoints["permil"] = 8240; EntitiesToCodePoints["prime"] = 8242; EntitiesToCodePoints["Prime"] = 8243; EntitiesToCodePoints["lsaquo"] = 8249; EntitiesToCodePoints["rsaquo"] = 8250; EntitiesToCodePoints["oline"] = 8254; EntitiesToCodePoints["frasl"] = 8260; EntitiesToCodePoints["euro"] = 8364; EntitiesToCodePoints["weierp"] = 8472; EntitiesToCodePoints["image"] = 8465; EntitiesToCodePoints["real"] = 8476; EntitiesToCodePoints["trade"] = 8482; EntitiesToCodePoints["alefsym"] = 8501; EntitiesToCodePoints["larr"] = 8592; EntitiesToCodePoints["uarr"] = 8593; EntitiesToCodePoints["rarr"] = 8594; EntitiesToCodePoints["darr"] = 8595; EntitiesToCodePoints["harr"] = 8596; EntitiesToCodePoints["crarr"] = 8629; EntitiesToCodePoints["lArr"] = 8656; EntitiesToCodePoints["uArr"] = 8657; EntitiesToCodePoints["rArr"] = 8658; EntitiesToCodePoints["dArr"] = 8659; EntitiesToCodePoints["hArr"] = 8660; EntitiesToCodePoints["forall"] = 8704; EntitiesToCodePoints["part"] = 8706; EntitiesToCodePoints["exist"] = 8707; EntitiesToCodePoints["empty"] = 8709; EntitiesToCodePoints["nabla"] = 8711; EntitiesToCodePoints["isin"] = 8712; EntitiesToCodePoints["notin"] = 8713; EntitiesToCodePoints["ni"] = 8715; EntitiesToCodePoints["prod"] = 8719; EntitiesToCodePoints["sum"] = 8721; EntitiesToCodePoints["minus"] = 8722; EntitiesToCodePoints["lowast"] = 8727; EntitiesToCodePoints["radic"] = 8730; EntitiesToCodePoints["prop"] = 8733; EntitiesToCodePoints["infin"] = 8734; EntitiesToCodePoints["ang"] = 8736; EntitiesToCodePoints["and"] = 8743; EntitiesToCodePoints["or"] = 8744; EntitiesToCodePoints["cap"] = 8745; EntitiesToCodePoints["cup"] = 8746; EntitiesToCodePoints["int"] = 8747; EntitiesToCodePoints["there4"] = 8756; EntitiesToCodePoints["sim"] = 8764; EntitiesToCodePoints["cong"] = 8773; EntitiesToCodePoints["asymp"] = 8776; EntitiesToCodePoints["ne"] = 8800; EntitiesToCodePoints["equiv"] = 8801; EntitiesToCodePoints["le"] = 8804; EntitiesToCodePoints["ge"] = 8805; EntitiesToCodePoints["sub"] = 8834; EntitiesToCodePoints["sup"] = 8835; EntitiesToCodePoints["nsub"] = 8836; EntitiesToCodePoints["sube"] = 8838; EntitiesToCodePoints["supe"] = 8839; EntitiesToCodePoints["oplus"] = 8853; EntitiesToCodePoints["otimes"] = 8855; EntitiesToCodePoints["perp"] = 8869; EntitiesToCodePoints["sdot"] = 8901; EntitiesToCodePoints["lceil"] = 8968; EntitiesToCodePoints["rceil"] = 8969; EntitiesToCodePoints["lfloor"] = 8970; EntitiesToCodePoints["rfloor"] = 8971; EntitiesToCodePoints["lang"] = 9001; EntitiesToCodePoints["rang"] = 9002; EntitiesToCodePoints["loz"] = 9674; EntitiesToCodePoints["spades"] = 9824; EntitiesToCodePoints["clubs"] = 9827; EntitiesToCodePoints["hearts"] = 9829; EntitiesToCodePoints["diams"] = 9830;
			CodePointsToEntities[160] = "nbsp"; CodePointsToEntities[161] = "iexcl"; CodePointsToEntities[162] = "cent"; CodePointsToEntities[163] = "pound"; CodePointsToEntities[164] = "curren"; CodePointsToEntities[165] = "yen"; CodePointsToEntities[166] = "brvbar"; CodePointsToEntities[167] = "sect"; CodePointsToEntities[168] = "uml"; CodePointsToEntities[169] = "copy"; CodePointsToEntities[170] = "ordf"; CodePointsToEntities[171] = "laquo"; CodePointsToEntities[172] = "not"; CodePointsToEntities[173] = "shy"; CodePointsToEntities[174] = "reg"; CodePointsToEntities[175] = "macr"; CodePointsToEntities[176] = "deg"; CodePointsToEntities[177] = "plusmn"; CodePointsToEntities[178] = "sup2"; CodePointsToEntities[179] = "sup3"; CodePointsToEntities[180] = "acute"; CodePointsToEntities[181] = "micro"; CodePointsToEntities[182] = "para"; CodePointsToEntities[183] = "middot"; CodePointsToEntities[184] = "cedil"; CodePointsToEntities[185] = "sup1"; CodePointsToEntities[186] = "ordm"; CodePointsToEntities[187] = "raquo"; CodePointsToEntities[188] = "frac14"; CodePointsToEntities[189] = "frac12"; CodePointsToEntities[190] = "frac34"; CodePointsToEntities[191] = "iquest"; CodePointsToEntities[192] = "Agrave"; CodePointsToEntities[193] = "Aacute"; CodePointsToEntities[194] = "Acirc"; CodePointsToEntities[195] = "Atilde"; CodePointsToEntities[196] = "Auml"; CodePointsToEntities[197] = "Aring"; CodePointsToEntities[198] = "AElig"; CodePointsToEntities[199] = "Ccedil"; CodePointsToEntities[200] = "Egrave"; CodePointsToEntities[201] = "Eacute"; CodePointsToEntities[202] = "Ecirc"; CodePointsToEntities[203] = "Euml"; CodePointsToEntities[204] = "Igrave"; CodePointsToEntities[205] = "Iacute"; CodePointsToEntities[206] = "Icirc"; CodePointsToEntities[207] = "Iuml"; CodePointsToEntities[208] = "ETH"; CodePointsToEntities[209] = "Ntilde"; CodePointsToEntities[210] = "Ograve"; CodePointsToEntities[211] = "Oacute"; CodePointsToEntities[212] = "Ocirc"; CodePointsToEntities[213] = "Otilde"; CodePointsToEntities[214] = "Ouml"; CodePointsToEntities[215] = "times"; CodePointsToEntities[216] = "Oslash"; CodePointsToEntities[217] = "Ugrave"; CodePointsToEntities[218] = "Uacute"; CodePointsToEntities[219] = "Ucirc"; CodePointsToEntities[220] = "Uuml"; CodePointsToEntities[221] = "Yacute"; CodePointsToEntities[222] = "THORN"; CodePointsToEntities[223] = "szlig"; CodePointsToEntities[224] = "agrave"; CodePointsToEntities[225] = "aacute"; CodePointsToEntities[226] = "acirc"; CodePointsToEntities[227] = "atilde"; CodePointsToEntities[228] = "auml"; CodePointsToEntities[229] = "aring"; CodePointsToEntities[230] = "aelig"; CodePointsToEntities[231] = "ccedil"; CodePointsToEntities[232] = "egrave"; CodePointsToEntities[233] = "eacute"; CodePointsToEntities[234] = "ecirc"; CodePointsToEntities[235] = "euml"; CodePointsToEntities[236] = "igrave"; CodePointsToEntities[237] = "iacute"; CodePointsToEntities[238] = "icirc"; CodePointsToEntities[239] = "iuml"; CodePointsToEntities[240] = "eth"; CodePointsToEntities[241] = "ntilde"; CodePointsToEntities[242] = "ograve"; CodePointsToEntities[243] = "oacute"; CodePointsToEntities[244] = "ocirc"; CodePointsToEntities[245] = "otilde"; CodePointsToEntities[246] = "ouml"; CodePointsToEntities[247] = "divide"; CodePointsToEntities[248] = "oslash"; CodePointsToEntities[249] = "ugrave"; CodePointsToEntities[250] = "uacute"; CodePointsToEntities[251] = "ucirc"; CodePointsToEntities[252] = "uuml"; CodePointsToEntities[253] = "yacute"; CodePointsToEntities[254] = "thorn"; CodePointsToEntities[255] = "yuml"; CodePointsToEntities[338] = "OElig"; CodePointsToEntities[339] = "oelig"; CodePointsToEntities[352] = "Scaron"; CodePointsToEntities[353] = "scaron"; CodePointsToEntities[376] = "Yuml"; CodePointsToEntities[402] = "fnof"; CodePointsToEntities[710] = "circ"; CodePointsToEntities[732] = "tilde"; CodePointsToEntities[913] = "Alpha"; CodePointsToEntities[914] = "Beta"; CodePointsToEntities[915] = "Gamma"; CodePointsToEntities[916] = "Delta"; CodePointsToEntities[917] = "Epsilon"; CodePointsToEntities[918] = "Zeta"; CodePointsToEntities[919] = "Eta"; CodePointsToEntities[920] = "Theta"; CodePointsToEntities[921] = "Iota"; CodePointsToEntities[922] = "Kappa"; CodePointsToEntities[923] = "Lambda"; CodePointsToEntities[924] = "Mu"; CodePointsToEntities[925] = "Nu"; CodePointsToEntities[926] = "Xi"; CodePointsToEntities[927] = "Omicron"; CodePointsToEntities[928] = "Pi"; CodePointsToEntities[929] = "Rho"; CodePointsToEntities[931] = "Sigma"; CodePointsToEntities[932] = "Tau"; CodePointsToEntities[933] = "Upsilon"; CodePointsToEntities[934] = "Phi"; CodePointsToEntities[935] = "Chi"; CodePointsToEntities[936] = "Psi"; CodePointsToEntities[937] = "Omega"; CodePointsToEntities[945] = "alpha"; CodePointsToEntities[946] = "beta"; CodePointsToEntities[947] = "gamma"; CodePointsToEntities[948] = "delta"; CodePointsToEntities[949] = "epsilon"; CodePointsToEntities[950] = "zeta"; CodePointsToEntities[951] = "eta"; CodePointsToEntities[952] = "theta"; CodePointsToEntities[953] = "iota"; CodePointsToEntities[954] = "kappa"; CodePointsToEntities[955] = "lambda"; CodePointsToEntities[956] = "mu"; CodePointsToEntities[957] = "nu"; CodePointsToEntities[958] = "xi"; CodePointsToEntities[959] = "omicron"; CodePointsToEntities[960] = "pi"; CodePointsToEntities[961] = "rho"; CodePointsToEntities[962] = "sigmaf"; CodePointsToEntities[963] = "sigma"; CodePointsToEntities[964] = "tau"; CodePointsToEntities[965] = "upsilon"; CodePointsToEntities[966] = "phi"; CodePointsToEntities[967] = "chi"; CodePointsToEntities[968] = "psi"; CodePointsToEntities[969] = "omega"; CodePointsToEntities[977] = "thetasym"; CodePointsToEntities[978] = "upsih"; CodePointsToEntities[982] = "piv"; CodePointsToEntities[8194] = "ensp"; CodePointsToEntities[8195] = "emsp"; CodePointsToEntities[8201] = "thinsp"; CodePointsToEntities[8204] = "zwnj"; CodePointsToEntities[8205] = "zwj"; CodePointsToEntities[8206] = "lrm"; CodePointsToEntities[8207] = "rlm"; CodePointsToEntities[8211] = "ndash"; CodePointsToEntities[8212] = "mdash"; CodePointsToEntities[8216] = "lsquo"; CodePointsToEntities[8217] = "rsquo"; CodePointsToEntities[8218] = "sbquo"; CodePointsToEntities[8220] = "ldquo"; CodePointsToEntities[8221] = "rdquo"; CodePointsToEntities[8222] = "bdquo"; CodePointsToEntities[8224] = "dagger"; CodePointsToEntities[8225] = "Dagger"; CodePointsToEntities[8226] = "bull"; CodePointsToEntities[8230] = "hellip"; CodePointsToEntities[8240] = "permil"; CodePointsToEntities[8242] = "prime"; CodePointsToEntities[8243] = "Prime"; CodePointsToEntities[8249] = "lsaquo"; CodePointsToEntities[8250] = "rsaquo"; CodePointsToEntities[8254] = "oline"; CodePointsToEntities[8260] = "frasl"; CodePointsToEntities[8364] = "euro"; CodePointsToEntities[8472] = "weierp"; CodePointsToEntities[8465] = "image"; CodePointsToEntities[8476] = "real"; CodePointsToEntities[8482] = "trade"; CodePointsToEntities[8501] = "alefsym"; CodePointsToEntities[8592] = "larr"; CodePointsToEntities[8593] = "uarr"; CodePointsToEntities[8594] = "rarr"; CodePointsToEntities[8595] = "darr"; CodePointsToEntities[8596] = "harr"; CodePointsToEntities[8629] = "crarr"; CodePointsToEntities[8656] = "lArr"; CodePointsToEntities[8657] = "uArr"; CodePointsToEntities[8658] = "rArr"; CodePointsToEntities[8659] = "dArr"; CodePointsToEntities[8660] = "hArr"; CodePointsToEntities[8704] = "forall"; CodePointsToEntities[8706] = "part"; CodePointsToEntities[8707] = "exist"; CodePointsToEntities[8709] = "empty"; CodePointsToEntities[8711] = "nabla"; CodePointsToEntities[8712] = "isin"; CodePointsToEntities[8713] = "notin"; CodePointsToEntities[8715] = "ni"; CodePointsToEntities[8719] = "prod"; CodePointsToEntities[8721] = "sum"; CodePointsToEntities[8722] = "minus"; CodePointsToEntities[8727] = "lowast"; CodePointsToEntities[8730] = "radic"; CodePointsToEntities[8733] = "prop"; CodePointsToEntities[8734] = "infin"; CodePointsToEntities[8736] = "ang"; CodePointsToEntities[8743] = "and"; CodePointsToEntities[8744] = "or"; CodePointsToEntities[8745] = "cap"; CodePointsToEntities[8746] = "cup"; CodePointsToEntities[8747] = "int"; CodePointsToEntities[8756] = "there4"; CodePointsToEntities[8764] = "sim"; CodePointsToEntities[8773] = "cong"; CodePointsToEntities[8776] = "asymp"; CodePointsToEntities[8800] = "ne"; CodePointsToEntities[8801] = "equiv"; CodePointsToEntities[8804] = "le"; CodePointsToEntities[8805] = "ge"; CodePointsToEntities[8834] = "sub"; CodePointsToEntities[8835] = "sup"; CodePointsToEntities[8836] = "nsub"; CodePointsToEntities[8838] = "sube"; CodePointsToEntities[8839] = "supe"; CodePointsToEntities[8853] = "oplus"; CodePointsToEntities[8855] = "otimes"; CodePointsToEntities[8869] = "perp"; CodePointsToEntities[8901] = "sdot"; CodePointsToEntities[8968] = "lceil"; CodePointsToEntities[8969] = "rceil"; CodePointsToEntities[8970] = "lfloor"; CodePointsToEntities[8971] = "rfloor"; CodePointsToEntities[9001] = "lang"; CodePointsToEntities[9002] = "rang"; CodePointsToEntities[9674] = "loz"; CodePointsToEntities[9824] = "spades"; CodePointsToEntities[9827] = "clubs"; CodePointsToEntities[9829] = "hearts"; CodePointsToEntities[9830] = "diams";
			
		}
	}
	
	public static string Decode(string str)
	{
		Init();
		return Regex.Replace(str, @"&(?:#(?<num>[0-9]+)|(?<named>[0-9A-Za-z]+));", delegate(Match m)
		                     {
			int charCode;
			string captured;
			if (m.Groups["num"].Captures.Count > 0)
			{
				
				if (!int.TryParse(m.Groups["num"].ToString(), out charCode))
				{//Return unaffected
					
					return m.Groups[0].ToString();
				}
			}
			else
			{
				captured = m.Groups["named"].ToString();
				if( EntitiesToCodePoints.ContainsKey(captured) ) {
					charCode = EntitiesToCodePoints[captured];
				}
				else {
					//Return unaffected
					
					return m.Groups[0].ToString();
				}
			}
			return Convert.ToChar(charCode).ToString();
		});
	}
	
	public static string Encode(string str)
	{
		Init();
		return Regex.Replace(str, @"[\u0080-\uDAFF\uE000-\uFFFF]", delegate(Match m)
		                     {
			int codePoint = (int)m.Value[0];
			if (CodePointsToEntities.ContainsKey(codePoint))
			{
				return "&" + CodePointsToEntities[codePoint] + ";";
			}
			return "&#" + codePoint + ";";
		});
	}
}