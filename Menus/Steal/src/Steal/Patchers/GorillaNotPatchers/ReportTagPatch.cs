using System;
using HarmonyLib;
using Photon.Realtime;
using UnityEngine;

namespace Steal.Patchers.GorillaNotPatchers
{
	// Token: 0x0200001B RID: 27
	[HarmonyPatch(typeof(GorillaTagManager), "ReportTag", 0)]
	public class ReportTagPatch : MonoBehaviour
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00007714 File Offset: 0x00005914
		private static bool Prefix(Player taggedPlayer, Player taggingPlayer)
		{
			GorillaTagManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
			if (component.currentInfected.Contains(taggingPlayer) && !component.currentInfected.Contains(taggedPlayer) && (double)Time.time > component.lastTag + (double)component.tagCoolDown)
			{
				VRRig vrrig = component.FindPlayerVRRig(taggingPlayer);
				VRRig vrrig2 = component.FindPlayerVRRig(taggedPlayer);
				if (!vrrig.CheckDistance(vrrig2.transform.position, 5f))
				{
					return false;
				}
			}
			return true;
		}
	}
}
