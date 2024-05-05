using System;
using HarmonyLib;
using PlayFab.Internal;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.ModPatches
{
	// Token: 0x02000012 RID: 18
	[HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
	internal class NoInitializeScreenTimeTracker : MonoBehaviour
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00007639 File Offset: 0x00005839
		private static bool Prefix()
		{
			ShowConsole.Log("NoInitializeScreenTimeTracker");
			return false;
		}
	}
}
