using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.GorillaNotPatchers
{
	// Token: 0x0200001D RID: 29
	[HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom", 0)]
	public class NoCloseInvalidRoom : MonoBehaviour
	{
		// Token: 0x06000066 RID: 102 RVA: 0x000077CB File Offset: 0x000059CB
		private static bool Prefix()
		{
			return false;
		}
	}
}
