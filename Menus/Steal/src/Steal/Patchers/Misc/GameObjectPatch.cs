using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.Misc
{
	// Token: 0x02000013 RID: 19
	[HarmonyPatch(typeof(GameObject), "CreatePrimitive", 0)]
	internal class GameObjectPatch
	{
		// Token: 0x06000052 RID: 82 RVA: 0x00007650 File Offset: 0x00005850
		private static void Postfix(GameObject __result)
		{
			if (GameObjectPatch.uberShader == null)
			{
				GameObjectPatch.uberShader = Shader.Find("GorillaTag/UberShader");
			}
			__result.GetComponent<Renderer>().material.shader = GameObjectPatch.uberShader;
			__result.GetComponent<Renderer>().material.color = Color.black;
		}

		// Token: 0x0400004A RID: 74
		private static Shader uberShader;
	}
}
