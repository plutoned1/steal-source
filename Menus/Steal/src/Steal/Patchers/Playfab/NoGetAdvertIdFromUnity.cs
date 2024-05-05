using System;
using HarmonyLib;
using PlayFab.Internal;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.Playfab
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(PlayFabDeviceUtil), "GetAdvertIdFromUnity")]
	internal class NoGetAdvertIdFromUnity : MonoBehaviour
	{
		// Token: 0x06000044 RID: 68 RVA: 0x000075BB File Offset: 0x000057BB
		private static bool Prefix()
		{
			ShowConsole.Log("NoGetAdvertIdFromUnity");
			return false;
		}
	}
}
