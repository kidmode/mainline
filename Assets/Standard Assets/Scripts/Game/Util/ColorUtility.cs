using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorUtility : object
{
	public static List<char> HEX_CHARS = new List<char>("0123456789abcdef".ToCharArray());
	public static List<char> HEX_CHARS_UPPER = new List<char>("0123456789ABCDEF".ToCharArray());
	
	public static ArrayList PLAYER_HAIR_COLOR = new ArrayList( new string[]{"0x000000",
												// brown		auburn		black		blond		red
												 "0x3b2000", "0x5d1e12", "0x252525", "0xb18040", "0x0d205a"});
												 
	public static ArrayList USER_TEAM_COLOR_MAIN = new ArrayList( new string[]{"0x000000",
													"0xffffff","0x000000","0x48286c","0xf5af1b","0xf26532",
													"0xd21033","0x008080","0x05854c","0x006bb6"});
//													// green		yellow		red
//													"0x05854c", "0xf5af1b", "0xd21033",
//													// navy blue	white		black
//													"0x01244c", "0xffffff", "0x000000",
//													// purple		orange		dark gray
//													"0x48286c", "0xf26532", "0x1e3344",
//													// pink		light blue	dark red
//													"0xFF1152", "0x0095ca", "0xa12910",
//													// blue			gold		light blue
//													"0x006bb6", "0xffc33c", "0x85a2c6",
//													// hunter green	silver		blue
//													"0x00330a", "0xefefef", "0x29588b"]; //Added extra black at 0 to align colors with IDs.
	
    public static ArrayList USER_TEAM_COLOR_IMAGE =  new ArrayList( new string[]{"Create Team/DeNA_Football_HUD_Customize_Colors_White",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Black",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Purple",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Yellow0",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Orange",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Red",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Teal",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Green",
													"Create Team/DeNA_Football_HUD_Customize_Colors_Blue"});
	public static int HexToInt(char p_hexChar) 
	{
		string l_hex = "" + p_hexChar;
		int l_return = -1;
		
		switch (l_hex)
		{
			case "0": l_return = 0; break;
			case "1": l_return = 1; break;
			case "2": l_return = 2; break;
			case "3": l_return = 3; break;
			case "4": l_return = 4; break;
			case "5": l_return = 5; break;
			case "6": l_return = 6; break;
			case "7": l_return = 7; break;
			case "8": l_return = 8; break;
			case "9": l_return = 9; break;
			case "A": case "a": l_return = 10; break;
			case "B": case "b": l_return = 11; break;
			case "C": case "c": l_return = 12; break;
			case "D": case "d": l_return = 13; break;
			case "E": case "e": l_return = 14; break;
			case "F": case "f": l_return = 15; break;
			default: break;
		}
		
		return l_return;
	}
	 
	//assumes 0x## ## ##
	public static Color HexToRGB(string p_color)
	{
		uint l_colorInt = HexToRGBInt(p_color);
		Color32 l_color = new Color32();

		//When alpha is 0, set 1
		if (l_colorInt <= 0xffffff)
		{
			l_color.a = 255;
		}
		else
		{
			l_color.a = (byte)((l_colorInt >> 24) & 0xFF);				
		}
		
		l_color.r = (byte)((l_colorInt >> 16) & 0xFF);
		l_color.g = (byte)((l_colorInt >> 8) & 0xFF);
		l_color.b = (byte)(((l_colorInt) & 0xFF));		

		return l_color;
	}
	
	//Does the same as above, but fills an already created color object.
	public static Color HexToRGB(string p_color, Color32 p_colorObj)
	{
		uint l_colorInt = HexToRGBInt(p_color);
		Color32 l_color = p_colorObj;

		//When alpha is 0, set 1
		if (l_colorInt <= 0xffffff)
		{
			l_color.a = 255;
		}
		else
		{
			l_color.a = (byte)((l_colorInt >> 24) & 0xFF);				
		}
		
		l_color.r = (byte)((l_colorInt >> 16) & 0xFF);
		l_color.g = (byte)((l_colorInt >> 8) & 0xFF);
		l_color.b = (byte)((l_colorInt) & 0xFF);		

		return l_color;
	}
	
	//Assuming 0x#, conver hex to int a way 1000x faster than system functions
	public static uint HexToRGBInt(string p_color)
	{
		uint v = 0;

		uint zero = '0';
		uint a = 'a';
		uint A = 'A';
		uint nine = '9';
		uint l_length = (uint)p_color.Length;

		for (int i = 2; i < l_length; ++i)
		{
			uint c = p_color[i];
			if (c < zero) return 0; //invalid character
			if (c > nine) //shift alphabetic characters down
			{
				if (c >= a) c -= a - A; //upper-case 'a' or higher
				if (c > nine) c -= A-1-nine; //make consecutive with digits
			}
			c -= zero; //convert char to hex digit value
			v = v << 4; //shift left by a hex digit
			v += c; //add the current digit
		}
		 
		return v;
	}
}
