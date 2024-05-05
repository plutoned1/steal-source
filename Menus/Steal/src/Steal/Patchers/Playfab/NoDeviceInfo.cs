using System;
using HarmonyLib;
using PlayFab.Internal;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.Playfab
{
	// Token: 0x02000010 RID: 16
	[HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
	internal class NoDeviceInfo : MonoBehaviour
	{
		// Token: 0x0600004C RID: 76 RVA: 0x0000760F File Offset: 0x0000580F
		private static bool Prefix()
		{
			ShowConsole.Log("NoDeviceInfo");
			return false;
		}
	}
}
