using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.Misc
{
	// Token: 0x02000014 RID: 20
	[HarmonyPatch(typeof(GorillaQuitBox), "OnBoxTriggered", 0)]
	internal class GorillaQuitBoxPatcher
	{
		// Token: 0x06000054 RID: 84 RVA: 0x000076AB File Offset: 0x000058AB
		private static bool Prefix()
		{
			TeleportationLib.Teleport(new Vector3(-64f, 12.534f, -83.014f));
			return false;
		}
	}
}
