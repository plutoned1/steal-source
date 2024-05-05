using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.VRRigPatchers
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(VRRig), "OnDisable", 0)]
	public class OnDisable : MonoBehaviour
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00007589 File Offset: 0x00005789
		public static bool Prefix(VRRig __instance)
		{
			return !(__instance == GorillaTagger.Instance.offlineVRRig);
		}
	}
}
