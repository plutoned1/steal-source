using System;
using HarmonyLib;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.GorillaNotPatchers
{
	// Token: 0x0200001C RID: 28
	[HarmonyPatch(typeof(GorillaNot), "SendReport", 0)]
	public class NoSendReport : MonoBehaviour
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00007794 File Offset: 0x00005994
		private static bool Prefix(string susReason, string susId, string susNick)
		{
			ShowConsole.Log(string.Concat(new string[] { susNick, " was reported! Reason: ", susReason, " ID: ", susId }));
			return false;
		}
	}
}
