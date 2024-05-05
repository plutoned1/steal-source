using System;
using HarmonyLib;

namespace Steal.Patchers.Misc
{
	// Token: 0x02000018 RID: 24
	[HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2))]
	[HarmonyPatch("LateUpdate", 0)]
	internal class NoGracePeriod2
	{
		// Token: 0x0600005C RID: 92 RVA: 0x000076F0 File Offset: 0x000058F0
		public static bool Prefix()
		{
			return false;
		}
	}
}
