using System;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers.GorillaNotPatchers
{
	// Token: 0x0200001A RID: 26
	[HarmonyPatch(typeof(GorillaNot), "LogErrorCount", 0)]
	public class NoLogErrorCount : MonoBehaviour
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00007706 File Offset: 0x00005906
		private static bool Prefix(string logString, string stackTrace, LogType type)
		{
			return false;
		}
	}
}
