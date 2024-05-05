using System;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Steal.Patchers.VRRigPatchers;
using UnityEngine;
using UnityEngine.XR;

namespace Steal.Background.Mods
{
	// Token: 0x02000042 RID: 66
	internal class PlayerMods : Mod
	{
		// Token: 0x06000191 RID: 401 RVA: 0x00013F0C File Offset: 0x0001210C
		public static PhotonView GetPhotonViewFromRig(VRRig rig)
		{
			try
			{
				PhotonView value = Traverse.Create(rig).Field("photonView").GetValue<PhotonView>();
				if (value != null)
				{
					return value;
				}
			}
			catch
			{
				throw;
			}
			return null;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00013F54 File Offset: 0x00012154
		public static void saveKeys()
		{
			if (GorillaGameManager.instance != null)
			{
				if (PlayerMods.GetGameMode().Contains("INFECTION"))
				{
					if (PlayerMods.GorillaTagManager == null)
					{
						PlayerMods.GorillaTagManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
						return;
					}
				}
				else if (PlayerMods.GetGameMode().Contains("HUNT"))
				{
					if (PlayerMods.GorillaHuntManager == null)
					{
						PlayerMods.GorillaHuntManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
						return;
					}
				}
				else if (PlayerMods.GetGameMode().Contains("BATTLE") && PlayerMods.GorillaBattleManager == null)
				{
					PlayerMods.GorillaBattleManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
				}
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00014009 File Offset: 0x00012209
		public static void AntiFlap()
		{
			Traverse.Create(GorillaTagger.Instance.offlineVRRig).Field("speakingLoudness").SetValue(0);
			GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = false;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00014045 File Offset: 0x00012245
		public static void ReFlap()
		{
			GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = true;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0001405C File Offset: 0x0001225C
		public static string getAntiReport()
		{
			return MenuPatch.antiReportCurrent;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00014064 File Offset: 0x00012264
		public static string GetGameMode()
		{
			string text = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString();
			if (text.Contains("INFECTION"))
			{
				return "INFECTION";
			}
			if (text.Contains("HUNT"))
			{
				return "HUNT";
			}
			if (text.Contains("BATTLE"))
			{
				return "BATTLE";
			}
			if (text.Contains("CASUAL"))
			{
				return "CASUAL";
			}
			return "ERROR";
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000140DC File Offset: 0x000122DC
		public static Vector3 GetMiddle(Vector3 vector)
		{
			return new Vector3(vector.x / 2f, vector.y / 2f, vector.z / 2f);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00014108 File Offset: 0x00012308
		public static void Splash()
		{
			if (Time.time > PlayerMods.splashtimeout + 0.5f)
			{
				PlayerMods.splashtimeout = Time.time;
				GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", 0, new object[]
				{
					Player.Instance.rightControllerTransform.position,
					Random.rotation,
					400f,
					100f,
					false,
					true
				});
				GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", 0, new object[]
				{
					Player.Instance.leftControllerTransform.position,
					Random.rotation,
					400f,
					100f,
					false,
					true
				});
			}
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0001420C File Offset: 0x0001240C
		public static void SizeableSplash()
		{
			if (InputHandler.RightTrigger && (double)Time.time > (double)PlayerMods.splashtimeout + 0.5)
			{
				GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", 0, new object[]
				{
					PlayerMods.GetMiddle(Player.Instance.rightControllerTransform.position + Player.Instance.leftControllerTransform.position),
					Random.rotation,
					Vector3.Distance(Player.Instance.rightControllerTransform.position, Player.Instance.leftControllerTransform.position),
					Vector3.Distance(Player.Instance.rightControllerTransform.position, Player.Instance.leftControllerTransform.position),
					false,
					true
				});
				PlayerMods.splashtimeout = Time.time;
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00014308 File Offset: 0x00012508
		public static void SplashGun()
		{
			GunLib.GunLibData gunLibData = GunLib.Shoot();
			if (gunLibData != null)
			{
				if (gunLibData.isShooting && gunLibData.isTriggered)
				{
					GorillaTagger instance = GorillaTagger.Instance;
					if (GorillaTagger.Instance.offlineVRRig.enabled)
					{
						Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
					}
					GorillaTagger.Instance.offlineVRRig.enabled = false;
					GorillaTagger.Instance.offlineVRRig.transform.position = gunLibData.hitPosition - new Vector3(0f, 1f, 0f);
					if (Time.time > PlayerMods.splashtimeout)
					{
						GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", 0, new object[]
						{
							gunLibData.hitPosition + new Vector3(0f, 1f, 0f),
							Quaternion.Euler(new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360))),
							4f,
							100f,
							true,
							false
						});
						PlayerMods.splashtimeout = Time.time + 0.1f;
						return;
					}
				}
				else
				{
					PlayerMods.ResetRig();
				}
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0001446C File Offset: 0x0001266C
		public static void ChangeIdentity()
		{
			string text = "";
			for (int i = 0; i < 12; i++)
			{
				text += Random.Range(0, 9).ToString();
			}
			GorillaComputer.instance.offlineVRRigNametagText.text = text;
			GorillaComputer.instance.currentName = text;
			GorillaComputer.instance.savedName = text;
			PhotonNetwork.LocalPlayer.NickName = text;
			byte b = (byte)Random.Range(0, 255);
			byte b2 = (byte)Random.Range(0, 255);
			byte b3 = (byte)Random.Range(0, 255);
			Color color = new Color32(b, b2, b3, byte.MaxValue);
			GorillaTagger.Instance.offlineVRRig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
			if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
			{
				GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", 1, new object[] { color.r, color.g, color.b, false });
				return;
			}
			GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Add(PhotonNetwork.LocalPlayer.UserId);
			PlayerMods.ChangeIdentity();
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000145D0 File Offset: 0x000127D0
		public static void ChangeRandomIdentity()
		{
			int num = Random.Range(0, PhotonNetwork.PlayerListOthers.Length);
			Player player = PhotonNetwork.PlayerListOthers[num];
			string nickName = player.NickName;
			GorillaComputer.instance.offlineVRRigNametagText.text = nickName;
			GorillaComputer.instance.currentName = nickName;
			GorillaComputer.instance.savedName = nickName;
			PhotonNetwork.LocalPlayer.NickName = nickName;
			Color color = GorillaGameManager.instance.FindPlayerVRRig(player).mainSkin.material.color;
			GorillaTagger.Instance.offlineVRRig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
			if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
			{
				GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", 1, new object[] { color.r, color.g, color.b, false });
				return;
			}
			GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Add(PhotonNetwork.LocalPlayer.UserId);
			PlayerMods.ChangeRandomIdentity();
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00014703 File Offset: 0x00012903
		public static void ResetRig()
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
			PlayerMods.ghostToggled = true;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0001471C File Offset: 0x0001291C
		public static void GhostMonkey()
		{
			if (!XRSettings.isDeviceActive)
			{
				if (GorillaTagger.Instance.offlineVRRig.enabled)
				{
					Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
				}
				GorillaTagger.Instance.offlineVRRig.enabled = false;
				return;
			}
			if (InputHandler.RightPrimary)
			{
				if (!PlayerMods.ghostToggled && GorillaTagger.Instance.offlineVRRig.enabled)
				{
					if (GorillaTagger.Instance.offlineVRRig.enabled)
					{
						Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
					}
					GorillaTagger.Instance.offlineVRRig.enabled = false;
					PlayerMods.ghostToggled = true;
					return;
				}
				if (!PlayerMods.ghostToggled && !GorillaTagger.Instance.offlineVRRig.enabled)
				{
					GorillaTagger.Instance.offlineVRRig.enabled = true;
					PlayerMods.ghostToggled = true;
					return;
				}
			}
			else
			{
				PlayerMods.ghostToggled = false;
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000147F0 File Offset: 0x000129F0
		public static void InvisMonkey()
		{
			if (!XRSettings.isDeviceActive)
			{
				if (GorillaTagger.Instance.offlineVRRig.enabled)
				{
					Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
				}
				GorillaTagger.Instance.offlineVRRig.enabled = false;
				GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(999f, 999f, 999f);
				return;
			}
			if (InputHandler.RightPrimary)
			{
				if (!PlayerMods.ghostToggled && GorillaTagger.Instance.offlineVRRig.enabled)
				{
					if (GorillaTagger.Instance.offlineVRRig.enabled)
					{
						Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
					}
					GorillaTagger.Instance.offlineVRRig.enabled = false;
					GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(999f, 999f, 999f);
					PlayerMods.ghostToggled = true;
					return;
				}
				if (!PlayerMods.ghostToggled && !GorillaTagger.Instance.offlineVRRig.enabled)
				{
					GorillaTagger.Instance.offlineVRRig.enabled = true;
					PlayerMods.ghostToggled = true;
					return;
				}
			}
			else
			{
				PlayerMods.ghostToggled = false;
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00014918 File Offset: 0x00012B18
		public static void FreezeMonkey()
		{
			if (InputHandler.RightPrimary)
			{
				if (!PlayerMods.ghostToggled && GorillaTagger.Instance.offlineVRRig.enabled)
				{
					if (GorillaTagger.Instance.offlineVRRig.enabled)
					{
						Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
					}
					GorillaTagger.Instance.offlineVRRig.enabled = false;
					PlayerMods.ghostToggled = true;
				}
				else if (!PlayerMods.ghostToggled && !GorillaTagger.Instance.offlineVRRig.enabled)
				{
					GorillaTagger.Instance.offlineVRRig.enabled = true;
					PlayerMods.ghostToggled = true;
				}
			}
			else
			{
				PlayerMods.ghostToggled = false;
			}
			if (!GorillaTagger.Instance.offlineVRRig.enabled)
			{
				GorillaTagger.Instance.offlineVRRig.transform.position = Player.Instance.bodyCollider.transform.position + new Vector3(0f, 0.2f, 0f);
				GorillaTagger.Instance.offlineVRRig.transform.rotation = Player.Instance.bodyCollider.transform.rotation;
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00014A30 File Offset: 0x00012C30
		public static void CopyGun()
		{
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData.isShooting && gunLibData.isTriggered && gunLibData.isLocked)
			{
				if (GorillaTagger.Instance.offlineVRRig.enabled)
				{
					Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
				}
				GorillaTagger.Instance.offlineVRRig.enabled = false;
				GorillaTagger.Instance.offlineVRRig.transform.position = gunLibData.lockedPlayer.transform.position;
				GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = gunLibData.lockedPlayer.rightHand.rigTarget.transform.position;
				GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = gunLibData.lockedPlayer.leftHand.rigTarget.transform.position;
				GorillaTagger.Instance.offlineVRRig.transform.rotation = gunLibData.lockedPlayer.transform.rotation;
				GorillaTagger.Instance.offlineVRRig.head.rigTarget.rotation = gunLibData.lockedPlayer.head.rigTarget.rotation;
				return;
			}
			PlayerMods.ResetRig();
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00014B84 File Offset: 0x00012D84
		public static void RigGun()
		{
			GunLib.GunLibData gunLibData = GunLib.Shoot();
			if (gunLibData != null)
			{
				if (gunLibData.isShooting && gunLibData.isTriggered)
				{
					if (GorillaTagger.Instance.offlineVRRig.enabled)
					{
						Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
					}
					GorillaTagger.Instance.offlineVRRig.enabled = false;
					GorillaTagger.Instance.offlineVRRig.transform.position = gunLibData.hitPosition + new Vector3(0f, 0.6f, 0f);
					return;
				}
				PlayerMods.ResetRig();
			}
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00014C14 File Offset: 0x00012E14
		public static void HoldRig()
		{
			if (InputHandler.RightGrip)
			{
				if (GorillaTagger.Instance.offlineVRRig.enabled)
				{
					Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
				}
				GorillaTagger.Instance.offlineVRRig.enabled = false;
				GorillaTagger.Instance.offlineVRRig.transform.position = Player.Instance.rightControllerTransform.position;
				return;
			}
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00014C90 File Offset: 0x00012E90
		public static void NoTagOnJoin()
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", false);
			Hashtable hashtable2 = hashtable;
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable2, null, null);
			GorillaTagger.Instance.myVRRig.Owner.SetCustomProperties(hashtable2, null, null);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00014CDC File Offset: 0x00012EDC
		public static void TagAlerts()
		{
			if (Time.time > PlayerMods.resetAlerts)
			{
				PlayerMods.resetAlerts = Time.time + 1f;
				for (int i = 0; i < PlayerMods.hasSentAlert.Length; i++)
				{
					PlayerMods.hasSentAlert[i] = false;
				}
			}
			for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
			{
				VRRig vrrig = GorillaParent.instance.vrrigs[j];
				if (vrrig.isOfflineVRRig || vrrig == null || !vrrig.mainSkin.material.name.Contains("fect"))
				{
					return;
				}
				if (Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position) < 3.5f)
				{
					if (!PlayerMods.hasSentAlert[j])
					{
						PlayerMods.hasSentAlert[j] = true;
						Notif.SendNotification(string.Format("Lava Monkey Detected {0}M Away!", Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position)), Color.red);
					}
				}
				else
				{
					PlayerMods.hasSentAlert[j] = false;
				}
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00014E08 File Offset: 0x00013008
		public static VRRig GetClosestUntagged()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
			{
				float num2 = Vector3.Distance(vrrig2.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
				if (num2 < num && !vrrig2.mainSkin.material.name.Contains("fected") && vrrig2 != GorillaTagger.Instance.offlineVRRig)
				{
					vrrig = vrrig2;
					num = num2;
				}
			}
			return vrrig;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00014EC4 File Offset: 0x000130C4
		public static VRRig GetClosestTagged()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
			{
				float num2 = Vector3.Distance(vrrig2.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
				if (num2 < num && vrrig2.mainSkin.material.name.Contains("fected") && vrrig2 != GorillaTagger.Instance.offlineVRRig)
				{
					vrrig = vrrig2;
					num = num2;
				}
			}
			return vrrig;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00014F80 File Offset: 0x00013180
		public static void AntiTag()
		{
			if (PhotonNetwork.InRoom)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().currentInfected.Remove(PhotonNetwork.LocalPlayer);
				}
				if (Vector3.Distance(PlayerMods.GetClosestTagged().transform.position, Player.Instance.headCollider.transform.position) < 3.5f && !Physics.Linecast(Player.Instance.transform.position, PlayerMods.GetClosestTagged().transform.position, LayerMask.NameToLayer("Gorilla Tag Collider")))
				{
					GorillaTagger.Instance.offlineVRRig.enabled = false;
					GorillaTagger.Instance.offlineVRRig.transform.position = Player.Instance.headCollider.transform.position - new Vector3(0f, 20f, 0f);
					return;
				}
				GorillaTagger.Instance.offlineVRRig.enabled = true;
			}
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00015090 File Offset: 0x00013290
		public static void TagAura()
		{
			foreach (Player player in PhotonNetwork.PlayerListOthers)
			{
				if (player != null)
				{
					PlayerMods.ProcessTagAura(player);
				}
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x000150C0 File Offset: 0x000132C0
		public static void ProcessTagAura(Player pl)
		{
			if (!GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().currentInfected.Contains(pl) && Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, GorillaGameManager.instance.FindPlayerVRRig(pl).transform.position) < GorillaGameManager.instance.tagDistanceThreshold)
			{
				RPCSUB.ReportTag(pl);
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0001512C File Offset: 0x0001332C
		public static void Helicopter()
		{
			try
			{
				VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
				offlineVRRig.enabled = false;
				Vector3 vector;
				vector..ctor(0f, 0.05f, 0f);
				Vector3 vector2;
				vector2..ctor(0f, 10f, 0f);
				offlineVRRig.transform.position += vector;
				Quaternion quaternion = Quaternion.Euler(offlineVRRig.transform.rotation.eulerAngles + vector2);
				offlineVRRig.transform.rotation = quaternion;
				offlineVRRig.head.rigTarget.transform.rotation = quaternion;
				Vector3 vector3 = offlineVRRig.transform.position - offlineVRRig.transform.right;
				Vector3 vector4 = offlineVRRig.transform.position + offlineVRRig.transform.right;
				offlineVRRig.leftHand.rigTarget.transform.position = vector3;
				offlineVRRig.rightHand.rigTarget.transform.position = vector4;
				offlineVRRig.leftHand.rigTarget.transform.rotation = quaternion;
				offlineVRRig.rightHand.rigTarget.transform.rotation = quaternion;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				throw;
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0001528C File Offset: 0x0001348C
		public static void OrbitGun()
		{
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.isTriggered && gunLibData.isLocked && gunLibData.lockedPlayer != null && gunLibData.isShooting)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				VRRig offlineVRRig = instance.offlineVRRig;
				Component myVRRig = instance.myVRRig;
				Transform transform = offlineVRRig.transform;
				Transform transform2 = myVRRig.transform;
				Vector3 position = gunLibData.lockedPlayer.transform.position;
				offlineVRRig.enabled = false;
				Vector3 normalized = (position - transform.position).normalized;
				Vector3 vector = transform.position + normalized * (11f * Time.deltaTime);
				transform.position = vector;
				transform2.position = vector;
				transform.LookAt(position);
				transform2.LookAt(position);
				PlayerMods.UpdateRigTarget(offlineVRRig.head.rigTarget.transform, vector, transform.rotation);
				PlayerMods.UpdateRigTarget(offlineVRRig.leftHand.rigTarget.transform, vector + transform.right * -1f, transform.rotation);
				PlayerMods.UpdateRigTarget(offlineVRRig.rightHand.rigTarget.transform, vector + transform.right * 1f, transform.rotation);
				return;
			}
			PlayerMods.ResetRig();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x000153E8 File Offset: 0x000135E8
		public static void ColorToBoard()
		{
			Color color = new Color32(0, 5, 2, byte.MaxValue);
			GorillaTagger.Instance.offlineVRRig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
			GorillaTagger.Instance.offlineVRRig.enabled = false;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", 0, new object[] { color.r, color.g, color.b });
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.computerScreenRenderer.transform.position;
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00015608 File Offset: 0x00013808
		public static void SpazRig()
		{
			if (!PlayerMods.doonce)
			{
				PlayerMods.offsetl = GorillaTagger.Instance.offlineVRRig.leftHand.trackingPositionOffset;
				PlayerMods.offsetr = GorillaTagger.Instance.offlineVRRig.rightHand.trackingPositionOffset;
				PlayerMods.offsethead = GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset;
				PlayerMods.doonce = true;
			}
			Vector3 vector = PlayerMods.<SpazRig>g__offsetVariance|40_0(0.1f);
			GorillaTagger instance = GorillaTagger.Instance;
			instance.offlineVRRig.leftHand.trackingPositionOffset = PlayerMods.offsetl + vector;
			instance.offlineVRRig.rightHand.trackingPositionOffset = PlayerMods.offsetr + vector;
			instance.offlineVRRig.head.trackingPositionOffset = PlayerMods.offsethead + vector;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x000156D0 File Offset: 0x000138D0
		public static void ResetAfterSpaz()
		{
			GorillaTagger instance = GorillaTagger.Instance;
			instance.offlineVRRig.leftHand.trackingPositionOffset = PlayerMods.offsetl;
			instance.offlineVRRig.rightHand.trackingPositionOffset = PlayerMods.offsetr;
			instance.offlineVRRig.head.trackingPositionOffset = PlayerMods.offsethead;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00015720 File Offset: 0x00013920
		public static void UpdateRigTarget(Transform rigTarget, Vector3 position, Quaternion rotation)
		{
			rigTarget.position = position;
			rigTarget.rotation = rotation;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00015730 File Offset: 0x00013930
		public static void TagGun()
		{
			bool isMasterClient = PhotonNetwork.IsMasterClient;
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.isTriggered && gunLibData.isLocked && gunLibData.lockedPlayer != null && gunLibData.isShooting && PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null)
			{
				PlayerMods.saveKeys();
				if (!isMasterClient)
				{
					if (PlayerMods.GetGameMode().Contains("INFECTION"))
					{
						if (GorillaTagger.Instance.offlineVRRig.enabled)
						{
							Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
						}
						GorillaTagger.Instance.offlineVRRig.enabled = false;
						GorillaTagger.Instance.offlineVRRig.transform.position = gunLibData.lockedPlayer.transform.position;
						PlayerMods.ProcessTagAura(PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner);
					}
					if (PlayerMods.GetGameMode().Contains("HUNT"))
					{
						if (GorillaTagger.Instance.offlineVRRig.enabled)
						{
							Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
						}
						GorillaTagger.Instance.offlineVRRig.enabled = false;
						if (GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget == PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner)
						{
							GorillaTagger.Instance.offlineVRRig.transform.position = GorillaGameManager.instance.FindPlayerVRRig(GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget).transform.position;
							PlayerMods.ProcessTagAura(GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget);
							return;
						}
					}
					if (PlayerMods.GetGameMode().Contains("BATTLE") && PlayerMods.GorillaBattleManager.playerLives[PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner.ActorNumber] > 0)
					{
						GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetComponent<PhotonView>().RPC("ReportSlingshotHit", 2, new object[]
						{
							new Vector3(0f, 0f, 0f),
							PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner,
							RPCSUB.IncrementLocalPlayerProjectileCount()
						});
					}
					GorillaTagger.Instance.offlineVRRig.enabled = true;
				}
				else
				{
					Player owner = PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner;
					if (PlayerMods.GetGameMode().Contains("INFECTION"))
					{
						if (PlayerMods.GorillaTagManager.isCurrentlyTag)
						{
							PlayerMods.GorillaTagManager.ChangeCurrentIt(owner, true);
						}
						else if (!PlayerMods.GorillaTagManager.currentInfected.Contains(owner))
						{
							PlayerMods.GorillaTagManager.currentInfected.Add(owner);
						}
					}
					if (PlayerMods.GetGameMode().Contains("HUNT"))
					{
						if (GorillaTagger.Instance.offlineVRRig.enabled)
						{
							Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
						}
						GorillaTagger.Instance.offlineVRRig.enabled = false;
						if (GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget == PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner)
						{
							GorillaTagger.Instance.offlineVRRig.transform.position = GorillaGameManager.instance.FindPlayerVRRig(GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget).transform.position;
							PlayerMods.ProcessTagAura(GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget);
							return;
						}
					}
					if (PlayerMods.GetGameMode().Contains("BATTLE") && PlayerMods.GorillaBattleManager.playerLives[PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner.ActorNumber] > 0)
					{
						PlayerMods.GorillaBattleManager.playerLives[PlayerMods.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner.ActorNumber] = 0;
					}
				}
				if (!gunLibData.isShooting || !gunLibData.isTriggered)
				{
					PlayerMods.ResetRig();
				}
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00015B3C File Offset: 0x00013D3C
		public static void TagSelf()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().currentInfected.Add(PhotonNetwork.LocalPlayer);
				GorillaGameManager.instance.InfrequentUpdate();
				GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().UpdateState();
				GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().UpdateInfectionState();
				GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().UpdateTagState(true);
				GameMode.ReportHit();
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00015BBA File Offset: 0x00013DBA
		public static void TagLag()
		{
			GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().tagCoolDown = 1E+10f;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00015BD5 File Offset: 0x00013DD5
		public static void RevertTagLag()
		{
			GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().tagCoolDown = 5f;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00015BF0 File Offset: 0x00013DF0
		public static void TagPlayer(Player player)
		{
			GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().currentInfected.Add(player);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00015C0C File Offset: 0x00013E0C
		public static void UnTagPlayer(Player player)
		{
			GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().currentInfected.Remove(player);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00015C2C File Offset: 0x00013E2C
		public static void TagAll()
		{
			foreach (Player player in PhotonNetwork.PlayerListOthers)
			{
				if (!PhotonNetwork.IsMasterClient)
				{
					PlayerMods.GetGameMode().Contains("INFECTION");
					if (PlayerMods.GetGameMode().Contains("HUNT") && GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget != null)
					{
						if (GorillaTagger.Instance.offlineVRRig.enabled)
						{
							Steal.Patchers.VRRigPatchers.OnDisable.Prefix(GorillaTagger.Instance.offlineVRRig);
						}
						GorillaTagger.Instance.offlineVRRig.enabled = false;
						GorillaTagger.Instance.offlineVRRig.transform.position = GorillaGameManager.instance.FindPlayerVRRig(GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget).transform.position;
						PlayerMods.ProcessTagAura(GorillaTagger.Instance.offlineVRRig.huntComputer.GetComponent<GorillaHuntComputer>().myTarget);
						return;
					}
					if (PlayerMods.GetGameMode().Contains("BATTLE"))
					{
						GorillaBattleManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
						if (component.playerLives[player.ActorNumber] > 0)
						{
							PhotonView component2 = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetComponent<PhotonView>();
							if (PhotonNetwork.IsMasterClient)
							{
								component.playerLives[player.ActorNumber] = 0;
								return;
							}
							component2.RPC("ReportSlingshotHit", 2, new object[]
							{
								new Vector3(0f, 0f, 0f),
								player,
								RPCSUB.IncrementLocalPlayerProjectileCount()
							});
						}
					}
				}
				else
				{
					GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().currentInfected.Add(player);
				}
			}
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00015E28 File Offset: 0x00014028
		[CompilerGenerated]
		internal static Vector3 <SpazRig>g__offsetVariance|40_0(float amount)
		{
			return new Vector3(Random.Range(-amount, amount), Random.Range(-amount, amount), Random.Range(-amount, amount));
		}

		// Token: 0x0400012B RID: 299
		public static GorillaBattleManager GorillaBattleManager;

		// Token: 0x0400012C RID: 300
		public static GorillaHuntManager GorillaHuntManager;

		// Token: 0x0400012D RID: 301
		public static GorillaTagManager GorillaTagManager;

		// Token: 0x0400012E RID: 302
		private static bool ghostToggled = false;

		// Token: 0x0400012F RID: 303
		private static bool[] hasSentAlert = new bool[10];

		// Token: 0x04000130 RID: 304
		private static float resetAlerts;

		// Token: 0x04000131 RID: 305
		private static Vector3 offsetl;

		// Token: 0x04000132 RID: 306
		private static Vector3 offsetr;

		// Token: 0x04000133 RID: 307
		private static Vector3 offsethead;

		// Token: 0x04000134 RID: 308
		private static bool doonce = false;

		// Token: 0x04000135 RID: 309
		private static float splashtimeout;
	}
}
