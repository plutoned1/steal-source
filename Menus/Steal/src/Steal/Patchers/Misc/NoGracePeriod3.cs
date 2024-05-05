using System;
using HarmonyLib;

namespace Steal.Patchers.Misc
{
	// Token: 0x02000017 RID: 23
	[HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2))]
	[HarmonyPatch("GracePeriod", 5)]
	internal class NoGracePeriod3
	{
		// Token: 0x0600005A RID: 90 RVA: 0x000076E5 File Offset: 0x000058E5
		public static bool Prefix()
		{
			return false;
		}
	}
}
