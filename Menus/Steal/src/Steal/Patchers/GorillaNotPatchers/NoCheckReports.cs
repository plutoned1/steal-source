using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.GorillaNotPatchers
{
	// Token: 0x0200001F RID: 31
	[HarmonyPatch(typeof(GorillaNot), "CheckReports", 5)]
	public class NoCheckReports : MonoBehaviour
	{
		// Token: 0x0600006A RID: 106 RVA: 0x000077E1 File Offset: 0x000059E1
		private static bool Prefix()
		{
			return false;
		}
	}
}
