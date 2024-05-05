using System;
using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace Steal.Patchers
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(Player))]
	[HarmonyPatch("LateUpdate", 0)]
	internal class TeleportationLib : MonoBehaviour
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00007438 File Offset: 0x00005638
		internal static bool Prefix(Player __instance, ref Vector3 ___lastPosition, ref Vector3[] ___velocityHistory, ref Vector3 ___lastHeadPosition, ref Vector3 ___lastLeftHandPosition, ref Vector3 ___lastRightHandPosition, ref Vector3 ___currentVelocity, ref Vector3 ___denormalizedVelocityAverage)
		{
			bool flag;
			try
			{
				if (TeleportationLib.teleporting)
				{
					Vector3 vector = TeleportationLib.destination - __instance.bodyCollider.transform.position + __instance.transform.position;
					try
					{
						__instance.bodyCollider.attachedRigidbody.velocity = Vector3.zero;
						__instance.bodyCollider.attachedRigidbody.isKinematic = true;
						___velocityHistory = new Vector3[__instance.velocityHistorySize];
						___currentVelocity = Vector3.zero;
						___denormalizedVelocityAverage = Vector3.zero;
						___lastRightHandPosition = vector;
						___lastLeftHandPosition = vector;
						___lastHeadPosition = vector;
						__instance.transform.position = vector;
						___lastPosition = vector;
						__instance.bodyCollider.attachedRigidbody.isKinematic = false;
					}
					catch
					{
					}
					TeleportationLib.teleporting = false;
				}
				flag = true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				flag = true;
			}
			return flag;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00007538 File Offset: 0x00005738
		internal static void Teleport(Vector3 TeleportDestination)
		{
			TeleportationLib.teleporting = true;
			TeleportationLib.destination = TeleportDestination;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00007546 File Offset: 0x00005746
		internal static void TeleportOnce(Vector3 TeleportDestination, bool stateDepender)
		{
			if (stateDepender)
			{
				if (!TeleportationLib.teleportOnce)
				{
					TeleportationLib.teleporting = true;
					TeleportationLib.destination = TeleportDestination;
				}
				TeleportationLib.teleportOnce = true;
				return;
			}
			TeleportationLib.teleportOnce = false;
		}

		// Token: 0x04000046 RID: 70
		private static bool teleporting;

		// Token: 0x04000047 RID: 71
		private static Vector3 destination;

		// Token: 0x04000048 RID: 72
		private static bool teleportOnce;
	}
}
