using System;
using HarmonyLib;
using PlayFab.Internal;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.Playfab
{
	// Token: 0x0200000B RID: 11
	[HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
	internal class NoInitializeScreenTimeTracker : MonoBehaviour
	{
		// Token: 0x06000042 RID: 66 RVA: 0x000075A6 File Offset: 0x000057A6
		private static bool Prefix()
		{
			ShowConsole.Log("NoInitializeScreenTimeTracker");
			return false;
		}
	}
}
