using System;
using System.IO;
using BepInEx.Logging;
using Photon.Pun;
using Steal.Background.Security.Auth;
using Steal.Components;
using UnityEngine;

namespace Steal.Background.Mods
{
	// Token: 0x0200003F RID: 63
	public class Mod : MonoBehaviourPunCallbacks
	{
		// Token: 0x06000113 RID: 275 RVA: 0x0000D884 File Offset: 0x0000BA84
		public override void OnEnable()
		{
			base.OnEnable();
			if (string.IsNullOrEmpty(Base.key) || Base.ms == null)
			{
				Environment.FailFast("failFast");
				return;
			}
			if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "stealkey.txt")))
			{
				string text = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "stealkey.txt"));
				Base.GetAuth.license2(text);
			}
			Mod.logSource = new ManualLogSource(base.GetType().Name);
			Mod.logSource.LogDebug("Reauthenticated!");
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000D924 File Offset: 0x0000BB24
		public override void OnDisable()
		{
			base.OnDisable();
			if (string.IsNullOrEmpty(Base.key) || Base.ms == null)
			{
				Environment.FailFast("failFast");
				return;
			}
			if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "stealkey.txt")))
			{
				string text = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "stealkey.txt"));
				Base.GetAuth.license2(text);
			}
			Mod.logSource.LogDebug("Reauthenticated!");
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000D9B0 File Offset: 0x0000BBB0
		public static void ToggleWatch()
		{
			GameObject gameObject = GameObject.Find("Steal");
			gameObject.GetComponent<PocketWatch>().enabled = !gameObject.GetComponent<PocketWatch>().enabled;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000D9E4 File Offset: 0x0000BBE4
		public static void ToggleList()
		{
			GameObject gameObject = GameObject.Find("Steal");
			gameObject.GetComponent<ModsList>().enabled = !gameObject.GetComponent<ModsList>().enabled;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000DA18 File Offset: 0x0000BC18
		public static void ToggleGameList()
		{
			GameObject gameObject = GameObject.Find("Steal");
			gameObject.GetComponent<ModsListInterface>().enabled = !gameObject.GetComponent<ModsListInterface>().enabled;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000DA49 File Offset: 0x0000BC49
		public static void CleanUp()
		{
			PlayerMods.ResetRig();
			GunLib.GunCleanUp();
			Movement.ResetGravity();
			Visual.ResetTexure();
		}

		// Token: 0x040000C0 RID: 192
		public static ManualLogSource logSource;
	}
}
