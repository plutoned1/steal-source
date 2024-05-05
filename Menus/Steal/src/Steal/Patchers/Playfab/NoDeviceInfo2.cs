using System;
using HarmonyLib;
using PlayFab;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.Playfab
{
	// Token: 0x0200000E RID: 14
	[HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
	internal class NoDeviceInfo2 : MonoBehaviour
	{
		// Token: 0x06000048 RID: 72 RVA: 0x000075E5 File Offset: 0x000057E5
		private static bool Prefix()
		{
			ShowConsole.Log("NoDeviceInfo2");
			return false;
		}
	}
}
