using System;
using HarmonyLib;

namespace Steal.Patchers.Misc
{
	// Token: 0x02000015 RID: 21
	[HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin))]
	[HarmonyPatch("GracePeriod", 5)]
	internal class NoGracePeriod
	{
		// Token: 0x06000056 RID: 86 RVA: 0x000076CF File Offset: 0x000058CF
		public static bool Prefix()
		{
			return false;
		}
	}
}
