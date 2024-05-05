using System;
using HarmonyLib;

namespace Steal.Patchers.Misc
{
	// Token: 0x02000016 RID: 22
	[HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin))]
	[HarmonyPatch("LateUpdate", 0)]
	internal class NoGracePeriod4
	{
		// Token: 0x06000058 RID: 88 RVA: 0x000076DA File Offset: 0x000058DA
		public static bool Prefix()
		{
			return false;
		}
	}
}
