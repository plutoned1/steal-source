using System;
using System.IO;
using System.Net;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace Steal.Background
{
	// Token: 0x02000033 RID: 51
	internal class SettingsLib : MonoBehaviour
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x0000B880 File Offset: 0x00009A80
		public static void Load()
		{
			SettingsLib.hasInit = true;
			string value = SettingsLib.cfgFile.Bind<string>("Customization", "Custom_Background", "NONE", "You can make this either a URL or a file path!").Value;
			SettingsLib.bgURI = value;
			if (value != null && value != "NONE")
			{
				if (value.Contains("https"))
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(value);
					httpWebRequest.Method = "HEAD";
					bool flag;
					try
					{
						httpWebRequest.GetResponse();
						flag = true;
					}
					catch
					{
						flag = false;
					}
					if (flag)
					{
						SettingsLib.bgURI = value;
					}
					else
					{
						SettingsLib.bgURI = "NONE";
						Debug.Log("URL DOES NOT EXIST!");
					}
				}
				else if (!File.Exists(value))
				{
					SettingsLib.bgURI = "NONE";
					Debug.Log("PATH DOES NOT EXIST!");
				}
				else
				{
					SettingsLib.bgURI = value;
				}
			}
			SettingsLib.BGColor = SettingsLib.cfgFile.Bind<Color>("Customization", "Custom_Background_Color", new Color(0f, 0f, 0f, 0f), "Set to 00000000 if you want it to the default theme!").Value;
			SettingsLib.ButtonColor = SettingsLib.cfgFile.Bind<Color>("Customization", "Custom_Button_Color", new Color(0f, 0f, 0f, 0f), "Set to 00000000 if you want it to the default theme!").Value;
			SettingsLib.ButtonText = SettingsLib.cfgFile.Bind<Color>("Customization", "Custom_Text_Color", new Color(0f, 0f, 0f, 0f), "Set to 00000000 if you want it to the default text theme!").Value;
			SettingsLib.LegacyGhost = SettingsLib.cfgFile.Bind<bool>("Customization", "Legacy_Ghost", true, "When turning on things like Invis Monkey or Ghost Monkey there will be another rig to take the place of the old one!").Value;
			SettingsLib.ShowConsole = SettingsLib.cfgFile.Bind<bool>("Customization", "Show_Update_Console", true, "When you start your game steal will open a console make this false to disable it!").Value;
			SettingsLib.Catagories = SettingsLib.cfgFile.Bind<bool>("Customization", "Catagories", true, "Enable or disable catagories on the menu! (does not effect GUI)").Value;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000BA88 File Offset: 0x00009C88
		public void OnEnable()
		{
			SettingsLib.Load();
			MenuPatch.categorized = SettingsLib.Catagories;
		}

		// Token: 0x04000097 RID: 151
		public static bool hasInit = false;

		// Token: 0x04000098 RID: 152
		public static ConfigFile cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "stealconfig.cfg"), true);

		// Token: 0x04000099 RID: 153
		public static string bgURI;

		// Token: 0x0400009A RID: 154
		public static Color32 BGColor;

		// Token: 0x0400009B RID: 155
		public static Color32 ButtonColor;

		// Token: 0x0400009C RID: 156
		public static Color32 ButtonText;

		// Token: 0x0400009D RID: 157
		public static bool LegacyGhost;

		// Token: 0x0400009E RID: 158
		public static bool ShowConsole;

		// Token: 0x0400009F RID: 159
		public static bool Catagories;
	}
}
