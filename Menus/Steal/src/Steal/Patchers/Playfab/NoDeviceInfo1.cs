using System;
using HarmonyLib;
using PlayFab;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.Playfab
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(PlayFabClientAPI), "ReportDeviceInfo")]
	internal class NoDeviceInfo1 : MonoBehaviour
	{
		// Token: 0x0600004A RID: 74 RVA: 0x000075FA File Offset: 0x000057FA
		private static bool Prefix()
		{
			ShowConsole.Log("NoDeviceInfo1");
			return false;
		}
	}
}
