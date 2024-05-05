using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.GorillaNotPatchers
{
	// Token: 0x02000020 RID: 32
	[HarmonyPatch(typeof(GorillaNot), "QuitDelay", 5)]
	public class NoQuitDelay : MonoBehaviour
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000077EC File Offset: 0x000059EC
		private static bool Prefix()
		{
			return false;
		}
	}
}
