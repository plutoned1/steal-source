using System;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Steal.Background
{
	// Token: 0x02000032 RID: 50
	internal class RPCSUB : MonoBehaviour
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x0000AFF2 File Offset: 0x000091F2
		public string getButtonType(int type)
		{
			if (type == 0)
			{
				return "Hate Speach";
			}
			if (type == 1)
			{
				return "Cheating";
			}
			if (type == 2)
			{
				return "Toxicity";
			}
			return "Other";
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000B018 File Offset: 0x00009218
		public void OnEvent(EventData ev)
		{
			if (ev.Code == 8)
			{
				object[] array = (object[])ev.CustomData;
				ShowConsole.Log(array[4].ToString() + " was reported by the anticheat for " + array[5].ToString());
				Notif.SendNotification(array[4].ToString() + " was reported by the anticheat for " + array[5].ToString(), Color.red);
			}
			if (ev.Code == 50)
			{
				object[] array2 = (object[])ev.CustomData;
				ShowConsole.Log(string.Concat(new string[]
				{
					array2[2].ToString(),
					" was reported by ",
					array2[3].ToString(),
					" for ",
					this.getButtonType(int.Parse(array2[3].ToString()))
				}));
				Notif.SendNotification(string.Concat(new string[]
				{
					array2[2].ToString(),
					" was reported by ",
					array2[3].ToString(),
					" for ",
					this.getButtonType(int.Parse(array2[3].ToString()))
				}), Color.red);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000B133 File Offset: 0x00009333
		public static Texture2D ConvertToTexture2D(Texture texture)
		{
			Texture2D texture2D = new Texture2D(texture.width, texture.height);
			texture2D.SetPixels((texture as Texture2D).GetPixels());
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000B160 File Offset: 0x00009360
		internal static void SendEvent(in byte code, in object evData, in RaiseEventOptions r)
		{
			object[] array = new object[]
			{
				PhotonNetwork.ServerTimestamp,
				code,
				evData
			};
			bool flag = PhotonNetwork.NetworkingClient.OpRaiseEvent(3, array, r, SendOptions.SendReliable);
			ShowConsole.Log("EVENT SENT: " + code.ToString() + " STATUS: " + flag.ToString());
			float num = 0.3f;
			switch (code)
			{
			case 0:
				num = 0.1f;
				break;
			case 1:
				num = 0.1f;
				break;
			case 2:
				num = 1.9f;
				break;
			case 3:
				num = 0.4f;
				break;
			case 4:
				num = -1f;
				break;
			case 5:
				num = -1f;
				break;
			}
			RPCSUB.t = Time.time + num;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000B228 File Offset: 0x00009428
		public static void SetSlowedTime(ReceiverGroup group)
		{
			if (PhotonNetwork.IsMasterClient && Time.time > RPCSUB.t)
			{
				object[] array = new object[] { 2 };
				object obj = array;
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					CachingOption = 0,
					Receivers = 0
				};
				RPCSUB.SendEvent(RPCSUB.EFFECT_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000B27C File Offset: 0x0000947C
		public static void SetTaggedTime(ReceiverGroup group)
		{
			if (PhotonNetwork.IsMasterClient && Time.time > RPCSUB.t)
			{
				object[] array = new object[] { 0 };
				object obj = array;
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					CachingOption = 0,
					Receivers = 0
				};
				RPCSUB.SendEvent(RPCSUB.EFFECT_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000B2D0 File Offset: 0x000094D0
		public static void JoinedTaggedTime(ReceiverGroup group)
		{
			if (PhotonNetwork.IsMasterClient && Time.time > RPCSUB.t)
			{
				object[] array = new object[] { 1 };
				object obj = array;
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					CachingOption = 0,
					Receivers = 0
				};
				RPCSUB.SendEvent(RPCSUB.EFFECT_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000B324 File Offset: 0x00009524
		public static void SetSlowedTime(Player group)
		{
			if (PhotonNetwork.IsMasterClient && Time.time > RPCSUB.t)
			{
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { group.ActorNumber }
				};
				object[] array = new object[] { 2 };
				object obj = array;
				RPCSUB.SendEvent(RPCSUB.EFFECT_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000B384 File Offset: 0x00009584
		public static void SetTaggedTime(Player group)
		{
			if (PhotonNetwork.IsMasterClient && Time.time > RPCSUB.t)
			{
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { group.ActorNumber }
				};
				object[] array = new object[] { 0 };
				object obj = array;
				RPCSUB.SendEvent(RPCSUB.EFFECT_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000B3E4 File Offset: 0x000095E4
		public static void JoinedTaggedTime(Player group)
		{
			if (PhotonNetwork.IsMasterClient && Time.time > RPCSUB.t)
			{
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { group.ActorNumber }
				};
				object[] array = new object[] { 1 };
				object obj = array;
				RPCSUB.SendEvent(RPCSUB.EFFECT_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000B441 File Offset: 0x00009641
		public static int IncrementLocalPlayerProjectileCount()
		{
			return RPCSUB.ProjectileCount++;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000B450 File Offset: 0x00009650
		public static void SendImpactEffect(Vector3 position, float r, float g, float b, float a)
		{
			if (Time.time > RPCSUB.t)
			{
				RPCSUB.impactdata[0] = position;
				RPCSUB.impactdata[1] = r;
				RPCSUB.impactdata[2] = g;
				RPCSUB.impactdata[3] = b;
				RPCSUB.impactdata[4] = a;
				RPCSUB.impactdata[5] = RPCSUB.ProjectileCount;
				object obj = RPCSUB.impactdata;
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					CachingOption = 0,
					Receivers = 0
				};
				RPCSUB.SendEvent(RPCSUB.IMPACT_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000B4E4 File Offset: 0x000096E4
		public static void SendLaunchProjectile(Vector3 position, Vector3 velocity, int projectileHash, int trailHash, bool leftHanded, bool randomColour, float r, float g, float b, float a)
		{
			if (Time.time > RPCSUB.t && Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, position) < 4f)
			{
				GorillaTagger.Instance.offlineVRRig.slingshot.currentState = 128;
				GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab = ObjectPools.instance.GetPoolByHash(projectileHash).objectToPool;
				GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail = ObjectPools.instance.GetPoolByHash(1432124712).objectToPool;
				ObjectPools.instance.Instantiate(projectileHash).GetComponent<SlingshotProjectile>().Launch(position, velocity, PhotonNetwork.LocalPlayer, false, false, RPCSUB.IncrementLocalPlayerProjectileCount(), Mathf.Abs(GorillaTagger.Instance.offlineVRRig.slingshot.projectilePrefab.transform.lossyScale.x), true, new Color(r, g, b, a));
				RPCSUB.projdata[0] = position;
				RPCSUB.projdata[1] = velocity;
				RPCSUB.projdata[2] = 2;
				RPCSUB.projdata[3] = RPCSUB.IncrementLocalPlayerProjectileCount();
				RPCSUB.projdata[4] = randomColour;
				RPCSUB.projdata[5] = r;
				RPCSUB.projdata[6] = g;
				RPCSUB.projdata[7] = b;
				RPCSUB.projdata[8] = a;
				object obj = RPCSUB.projdata;
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					CachingOption = 0,
					Receivers = 0
				};
				RPCSUB.SendEvent(RPCSUB.PROJECTILE_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000B688 File Offset: 0x00009888
		public static void SendSound(int id, float volume)
		{
			if (PhotonNetwork.IsMasterClient && Time.time > RPCSUB.t)
			{
				GorillaTagger.Instance.offlineVRRig.PlayTagSoundLocal(id, volume);
				object[] array = new object[] { id, volume };
				object obj = array;
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					Receivers = 0,
					CachingOption = 0
				};
				RPCSUB.SendEvent(RPCSUB.SOUND, obj, raiseEventOptions);
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000B6F7 File Offset: 0x000098F7
		public static void ReportTag(Player player)
		{
			GameMode.ReportTag(player);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000B700 File Offset: 0x00009900
		public static void JoinPubWithFriends(Player player)
		{
			if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && player != PhotonNetwork.LocalPlayer)
			{
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { player.ActorNumber }
				};
				PhotonNetworkController.Instance.shuffler = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
				PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
				object[] array = new object[]
				{
					PhotonNetworkController.Instance.shuffler,
					PhotonNetworkController.Instance.keyStr
				};
				object obj = array;
				RPCSUB.SendEvent(RPCSUB.JOINWITHFRIEND_ID, obj, raiseEventOptions);
			}
		}

		// Token: 0x04000088 RID: 136
		private static byte PROJECTILE_ID = 0;

		// Token: 0x04000089 RID: 137
		private static byte IMPACT_ID = 1;

		// Token: 0x0400008A RID: 138
		private static byte EFFECT_ID = 2;

		// Token: 0x0400008B RID: 139
		private static byte SOUND = 3;

		// Token: 0x0400008C RID: 140
		private static byte JOINWITHFRIEND_ID = 4;

		// Token: 0x0400008D RID: 141
		private static byte TAG_ID = 5;

		// Token: 0x0400008E RID: 142
		public static Type RoomSystemInstance;

		// Token: 0x0400008F RID: 143
		private static int ProjectileCount = 0;

		// Token: 0x04000090 RID: 144
		private static RaiseEventOptions Others = new RaiseEventOptions
		{
			Receivers = 0,
			CachingOption = 0
		};

		// Token: 0x04000091 RID: 145
		private static RaiseEventOptions All = new RaiseEventOptions
		{
			Receivers = 1,
			CachingOption = 0
		};

		// Token: 0x04000092 RID: 146
		private static RaiseEventOptions Master = new RaiseEventOptions
		{
			Receivers = 2,
			CachingOption = 0
		};

		// Token: 0x04000093 RID: 147
		private static float t;

		// Token: 0x04000094 RID: 148
		private static int eventsraised = 0;

		// Token: 0x04000095 RID: 149
		public static object[] impactdata = new object[6];

		// Token: 0x04000096 RID: 150
		public static object[] projdata = new object[11];
	}
}
