using System;
using HarmonyLib;
using PlayFab;
using Steal.Background;
using UnityEngine;

namespace Steal.Patchers.Playfab
{
	// Token: 0x02000011 RID: 17
	[HarmonyPatch(typeof(PlayFabClientAPI), "AttributeInstall")]
	internal class NoAttributeInstall : MonoBehaviour
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00007624 File Offset: 0x00005824
		private static bool Prefix()
		{
			ShowConsole.Log("NoAttributeInstall");
			return false;
		}
	}
}
