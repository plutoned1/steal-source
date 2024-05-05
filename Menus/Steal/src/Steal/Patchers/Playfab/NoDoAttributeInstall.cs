using System;
using HarmonyLib;
using PlayFab.Internal;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.Playfab
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(PlayFabDeviceUtil), "DoAttributeInstall")]
	internal class NoDoAttributeInstall : MonoBehaviour
	{
		// Token: 0x06000046 RID: 70 RVA: 0x000075D0 File Offset: 0x000057D0
		private static bool Prefix()
		{
			ShowConsole.Log("NoDoAttributeInstall");
			return false;
		}
	}
}
