using System;
using GorillaLocomotion;
using HarmonyLib;

namespace Steal.Patchers.Misc
{
	// Token: 0x02000019 RID: 25
	[HarmonyPatch(typeof(Player))]
	[HarmonyPatch("AntiTeleportTechnology", 0)]
	internal class AntiTeleport
	{
		// Token: 0x0600005E RID: 94 RVA: 0x000076FB File Offset: 0x000058FB
		public static bool Prefix()
		{
			return false;
		}
	}
}
