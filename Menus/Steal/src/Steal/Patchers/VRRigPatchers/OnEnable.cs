using System;
using HarmonyLib;
using Steal.Components;
using UnityEngine;

namespace Steal.Patchers.VRRigPatchers
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(VRRig), "OnEnable", 0)]
	public class OnEnable : MonoBehaviour
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00007573 File Offset: 0x00005773
		private static void Postfix(VRRig __instance)
		{
			__instance.gameObject.AddComponent<NameTags>();
		}

		// Token: 0x04000049 RID: 73
		public static bool nameTags;
	}
}
