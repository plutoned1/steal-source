using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.GorillaNotPatchers
{
	// Token: 0x0200001E RID: 30
	[HarmonyPatch(typeof(GorillaNot), "ShouldDisconnectFromRoom")]
	internal class NoShouldDisconnectFromRoom : MonoBehaviour
	{
		// Token: 0x06000068 RID: 104 RVA: 0x000077D6 File Offset: 0x000059D6
		private static bool Prefix()
		{
			return false;
		}
	}
}
