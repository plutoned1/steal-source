using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Steal.Patches
{
	// Token: 0x02000007 RID: 7
	public static class NoLag
	{
		// Token: 0x0200004E RID: 78
		[HarmonyPatch(typeof(GorillaTagManager), "ReportTag", 0)]
		public class ReportTagPatch : MonoBehaviour
		{
			// Token: 0x060002BB RID: 699 RVA: 0x00018DE8 File Offset: 0x00016FE8
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

		// Token: 0x0200004F RID: 79
		[HarmonyPatch(typeof(Gorillanalytics), "Start", 5)]
		public class GorillanalyticsStart : MonoBehaviour
		{
			// Token: 0x060002BD RID: 701 RVA: 0x00018E68 File Offset: 0x00017068
			private static bool Prefix()
			{
				return false;
			}
		}

		// Token: 0x02000050 RID: 80
		[HarmonyPatch(typeof(Gorillanalytics), "UploadGorillanalytics", 0)]
		public class AnalyticsPatch : MonoBehaviour
		{
			// Token: 0x060002BF RID: 703 RVA: 0x00018E73 File Offset: 0x00017073
			private static bool Prefix()
			{
				return false;
			}
		}

		// Token: 0x02000051 RID: 81
		[HarmonyPatch(typeof(VRRig), "RequestMaterialColor", 0)]
		public class NoKickRMC : MonoBehaviour
		{
			// Token: 0x060002C1 RID: 705 RVA: 0x00018E7E File Offset: 0x0001707E
			private static bool Prefix()
			{
				return false;
			}
		}

		// Token: 0x02000052 RID: 82
		[HarmonyPatch(typeof(VRRig), "RequestCosmetics", 0)]
		public class NoKickRC : MonoBehaviour
		{
			// Token: 0x060002C3 RID: 707 RVA: 0x00018E89 File Offset: 0x00017089
			private static bool Prefix()
			{
				return false;
			}
		}

		// Token: 0x02000053 RID: 83
		[HarmonyPatch(typeof(VRRig), "InitializeNoobMaterial", 0)]
		public class InitNoob
		{
			// Token: 0x060002C5 RID: 709 RVA: 0x00018E94 File Offset: 0x00017094
			public static bool Prefix(float red, float green, float blue, PhotonMessageInfoWrapped info, VRRig __instance)
			{
				NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
				if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(__instance.gameObject) && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.GetUserID(info.senderID)))
				{
					GorillaNot.IncrementRPCCall(info, "InitializeNoobMaterial");
					red = Mathf.Clamp(red, 0f, 1f);
					green = Mathf.Clamp(green, 0f, 1f);
					blue = Mathf.Clamp(blue, 0f, 1f);
					__instance.InitializeNoobMaterialLocal(red, green, blue);
				}
				else
				{
					GorillaNot.instance.SendReport("inappropriate tag data being sent init noob", player.UserId, player.NickName);
				}
				return false;
			}
		}

		// Token: 0x02000054 RID: 84
		[HarmonyPatch(typeof(GorillaParent), "LateUpdate", 0)]
		public class GorillaParentPatch
		{
			// Token: 0x060002C7 RID: 711 RVA: 0x00018F68 File Offset: 0x00017168
			public static bool Prefix(GorillaParent __instance)
			{
				if (Time.time > NoLag.GorillaParentPatch.cooldown)
				{
					NoLag.GorillaParentPatch.cooldown = Time.time + 15f;
					if (PhotonNetwork.CurrentRoom != null && GTExt.IsNull(GorillaTagger.Instance.myVRRig))
					{
						GameObject gameObject;
						Traverse.Create(typeof(PhotonPrefabPool)).Field("networkPrefabs").GetValue<Dictionary<string, GameObject>>()
							.TryGetValue("Player Network Controller", out gameObject);
						if (gameObject == null)
						{
							return false;
						}
						NetworkSystem.Instance.NetInstantiate(gameObject, GorillaTagger.Instance.offlineVRRig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.rotation, false);
					}
				}
				return false;
			}

			// Token: 0x04000165 RID: 357
			private static float cooldown;
		}
	}
}
