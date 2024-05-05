using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using GorillaNetworking;
using HarmonyLib;
using Steal.Background.Mods;
using Steal.Components;
using UnityEngine;
using UnityEngine.XR;

namespace Steal.Background.Security.Auth
{
	// Token: 0x02000039 RID: 57
	internal class Base
	{
		// Token: 0x060000E3 RID: 227
		[DllImport("kernel32.dll")]
		private static extern void ExitProcess(int exitCode);

		// Token: 0x060000E4 RID: 228 RVA: 0x0000BDD8 File Offset: 0x00009FD8
		public static void Init()
		{
			try
			{
				if (Harmony.HasAnyPatches("com.steal.lol"))
				{
					File.WriteAllText("error.txt", "PRE HARMONY PATCHED");
					Base.ExitProcess(0);
					Environment.FailFast("bye");
				}
				else if (Base.GetAuth.response.success)
				{
					File.WriteAllText("error.txt", "FORCED KEY AUTH SUCCESS");
					Application.Quit();
					Environment.FailFast("bye");
				}
				else
				{
					Base.GetAuth.init();
					if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "stealkey.txt")))
					{
						string text = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "stealkey.txt"));
						Base.key = text;
						Base.GetAuth.license2(text);
						if (Base.GetAuth.response.success)
						{
							if (!GameObject.Find("Steal"))
							{
								if (!new WebClient().DownloadString("https://bbc123f.github.io/killswitch.txt").Contains("="))
								{
									Base.ms = new GameObject("Steal");
									Base.ms.AddComponent<ShowConsole>();
									Base.ms.AddComponent<InputHandler>();
									Base.ms.AddComponent<Notif>();
									Base.ms.AddComponent<RPCSUB>();
									Base.ms.AddComponent<AssetLoader>();
									Base.ms.AddComponent<MenuPatch>();
									Base.ms.AddComponent<UI>();
									Base.ms.AddComponent<GhostRig>();
									Base.ms.AddComponent<Movement>();
									Base.ms.AddComponent<Visual>();
									Base.ms.AddComponent<PlayerMods>();
									Base.ms.AddComponent<RoomManager>();
									Base.ms.AddComponent<Overpowered>();
									Base.ms.AddComponent<AdminControls>();
									Base.ms.AddComponent<ModsList>();
									Base.ms.AddComponent<PocketWatch>();
									Base.ms.AddComponent<ModsListInterface>();
									if (!XRSettings.isDeviceActive)
									{
										Base.ms.GetComponent<PocketWatch>().enabled = false;
										Base.ms.GetComponent<ModsListInterface>().enabled = false;
									}
									AuthClient.asfasf(PlayFabAuthenticator.instance.GetSteamAuthTicket());
									new Harmony("com.steal.lol").PatchAll();
									if (File.Exists("steal_error.log"))
									{
										File.Delete("steal_error.log");
									}
									ShowConsole.Log("Auth Success!");
								}
								else
								{
									File.WriteAllText("error.txt", "KILL SWITCHED!");
									ShowConsole.Log("KILL SWITCHED!");
									Environment.FailFast("bye");
								}
							}
							else
							{
								File.WriteAllText("error.txt", "ALREADY INJECTED");
								ShowConsole.Log("ALREADY INJECTED");
								Environment.FailFast("bye");
							}
						}
						else
						{
							File.WriteAllText("error.txt", Base.GetAuth.response.message);
							ShowConsole.Log(Base.GetAuth.response.message);
							Base.ExitProcess(0);
							Environment.FailFast("bye");
						}
					}
					else
					{
						File.WriteAllText("error.txt", "YOUR KEY FILE DOES NOT EXIST");
						Base.ExitProcess(0);
						Environment.FailFast("bye");
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText("error.txt", ex.ToString());
			}
		}

		// Token: 0x040000AC RID: 172
		public static string key;

		// Token: 0x040000AD RID: 173
		public static auth GetAuth = new auth("Steal", "RovpqveRf3", "28dd3f3d424e86309e9d467c19b5936e61cc0abbd55e3360a04334e6044b9144", "1.0");

		// Token: 0x040000AE RID: 174
		public static GameObject ms = null;
	}
}
