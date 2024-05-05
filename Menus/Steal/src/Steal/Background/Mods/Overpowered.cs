using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.ClientModels;
using Steal.Components;
using UnityEngine;
using UnityEngine.XR;

namespace Steal.Background.Mods
{
	// Token: 0x02000041 RID: 65
	internal class Overpowered : Mod
	{
		// Token: 0x06000150 RID: 336 RVA: 0x00010226 File Offset: 0x0000E426
		public static void DisableNameOnJoin()
		{
			Overpowered.changeNameOnJoin = false;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0001022E File Offset: 0x0000E42E
		public static void EnableNameOnJoin()
		{
			Overpowered.changeNameOnJoin = true;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00010236 File Offset: 0x0000E436
		public static void DisableStumpCheck()
		{
			Overpowered.StumpCheck = false;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0001023E File Offset: 0x0000E43E
		public static void EnableStumpCheck()
		{
			Overpowered.StumpCheck = true;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00010248 File Offset: 0x0000E448
		public static void saveKeys()
		{
			if (GorillaGameManager.instance != null)
			{
				if (Overpowered.GetGameMode().Contains("INFECTION"))
				{
					if (Overpowered.GorillaTagManager == null)
					{
						Overpowered.GorillaTagManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
						return;
					}
				}
				else if (Overpowered.GetGameMode().Contains("HUNT"))
				{
					if (Overpowered.GorillaHuntManager == null)
					{
						Overpowered.GorillaHuntManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
						return;
					}
				}
				else if (Overpowered.GetGameMode().Contains("BATTLE") && Overpowered.GorillaBattleManager == null)
				{
					Overpowered.GorillaBattleManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
				}
			}
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000102FD File Offset: 0x0000E4FD
		public static bool IsModded()
		{
			return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("MODDED");
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0001032B File Offset: 0x0000E52B
		public static bool IsMaster()
		{
			return PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsMasterClient;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00010340 File Offset: 0x0000E540
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

		// Token: 0x06000158 RID: 344 RVA: 0x00010388 File Offset: 0x0000E588
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

		// Token: 0x06000159 RID: 345 RVA: 0x00010400 File Offset: 0x0000E600
		public static void switchAntiReport()
		{
			if (MenuPatch.antiReportCurrent == "Disconnect")
			{
				MenuPatch.antiReportCurrent = "Rejoin";
				return;
			}
			if (MenuPatch.antiReportCurrent == "Rejoin")
			{
				MenuPatch.antiReportCurrent = "JoinRandom";
				return;
			}
			if (MenuPatch.antiReportCurrent == "JoinRandom")
			{
				MenuPatch.antiReportCurrent = "Crash";
				return;
			}
			if (MenuPatch.antiReportCurrent == "Crash")
			{
				MenuPatch.antiReportCurrent = "Float";
				return;
			}
			if (MenuPatch.antiReportCurrent == "Float")
			{
				MenuPatch.antiReportCurrent = "Disconnect";
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00010498 File Offset: 0x0000E698
		public static void AntiReport()
		{
			try
			{
				Overpowered.MoveCrashHandler();
				foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
				{
					if (gorillaPlayerScoreboardLine.linePlayer.IsLocal)
					{
						Transform transform = gorillaPlayerScoreboardLine.reportButton.gameObject.transform;
						foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
						{
							if (vrrig != GorillaTagger.Instance.offlineVRRig && Overpowered.GetPhotonViewFromRig(vrrig) != null)
							{
								Player owner = Overpowered.GetPhotonViewFromRig(vrrig).Owner;
								float num = Vector3.Distance(vrrig.rightHandTransform.position, transform.position);
								float num2 = Vector3.Distance(vrrig.leftHandTransform.position, transform.position);
								float num3;
								if (MenuPatch.antiReportCurrent == "Float")
								{
									num3 = 6f;
								}
								else if (MenuPatch.antiReportCurrent == "Crash")
								{
									num3 = 6f;
								}
								else
								{
									num3 = 0.35f;
								}
								if (num < num3 || num2 < num3 || Overpowered.IsVectorNear(vrrig.rightHandTransform.position, vrrig.leftHandTransform.position, 0.01f))
								{
									if (MenuPatch.antiReportCurrent == "Disconnect")
									{
										PhotonNetwork.Disconnect();
									}
									else if (MenuPatch.antiReportCurrent == "Rejoin")
									{
										string name = PhotonNetwork.CurrentRoom.Name;
										PhotonNetwork.Disconnect();
										PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(name);
									}
									else if (MenuPatch.antiReportCurrent == "JoinRandom")
									{
										PhotonNetwork.Disconnect();
										PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.forestMapTrigger, false);
									}
									else if (MenuPatch.antiReportCurrent == "Crash")
									{
										if (!Overpowered.IsModded())
										{
											return;
										}
										Overpowered.crashedPlayer = Overpowered.GetPhotonViewFromRig(vrrig).Owner;
										Overpowered.crashPlayerPosition = vrrig.transform.position;
										Overpowered.NameAll();
									}
									else if (MenuPatch.antiReportCurrent == "Float")
									{
										if (!Overpowered.IsMaster())
										{
											return;
										}
										AngryBeeSwarm.instance.targetPlayer = owner;
										AngryBeeSwarm.instance.grabbedPlayer = owner;
										AngryBeeSwarm.instance.currentState = 8;
									}
									Notif.SendNotification(string.Concat(new string[]
									{
										owner.NickName,
										" tried to report you, ",
										MenuPatch.antiReportCurrent,
										" ",
										PhotonNetwork.CurrentRoom.Name
									}), Color.red);
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000107B4 File Offset: 0x0000E9B4
		private static bool IsVectorNear(Vector3 vectorA, Vector3 vectorB, float threshold)
		{
			return Vector3.Distance(vectorA, vectorB) <= threshold;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000107C3 File Offset: 0x0000E9C3
		public static void SoundSpam()
		{
			RPCSUB.SendSound(Random.Range(0, 4), 100f);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000107D8 File Offset: 0x0000E9D8
		public static void changegamemode(string gamemode)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			if (Overpowered.GetGameMode() == "HUNT")
			{
				hashtable.Add("gameMode", PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace("HUNT", gamemode));
			}
			if (Overpowered.GetGameMode() == "BATTLE")
			{
				hashtable.Add("gameMode", PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace("BATTLE", gamemode));
			}
			if (Overpowered.GetGameMode() == "INFECTION")
			{
				hashtable.Add("gameMode", PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace("INFECTION", gamemode));
			}
			if (Overpowered.GetGameMode() == "CASUAL")
			{
				hashtable.Add("gameMode", PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace("CASUAL", gamemode));
			}
			object value = Traverse.Create(typeof(GameMode)).Field("instance").GetValue<GameMode>();
			PhotonNetwork.Destroy(GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)"));
			Traverse.Create(value).Field("activeNetworkHandler").SetValue(null);
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			if (PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("CASUAL"))
			{
				PhotonNetwork.InstantiateRoomObject("GameMode", Vector3.zero, Quaternion.identity, 0, new object[] { 0 });
				return;
			}
			if (PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("INFECTION"))
			{
				PhotonNetwork.InstantiateRoomObject("GameMode", Vector3.zero, Quaternion.identity, 0, new object[] { 1 });
				return;
			}
			if (PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("HUNT"))
			{
				PhotonNetwork.InstantiateRoomObject("GameMode", Vector3.zero, Quaternion.identity, 0, new object[] { 2 });
				return;
			}
			if (PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("BATTLE"))
			{
				PhotonNetwork.InstantiateRoomObject("GameMode", Vector3.zero, Quaternion.identity, 0, new object[] { 3 }).GetComponent<GorillaBattleManager>().RandomizeTeams();
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00010A78 File Offset: 0x0000EC78
		public static void CheckForStump()
		{
			if (Time.time > Overpowered.CheckedFor + 1.5f)
			{
				Overpowered.StartAntiBan();
				Overpowered.CheckedFor = Time.time;
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00010A9C File Offset: 0x0000EC9C
		public static void AcidMat(Player player)
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj.ToString().Contains("MODDED"))
			{
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(PhotonNetwork.CurrentRoom.PlayerCount);
				ScienceExperimentManager.PlayerGameState[] array = new ScienceExperimentManager.PlayerGameState[10];
				for (int i = 0; i < (int)PhotonNetwork.CurrentRoom.PlayerCount; i++)
				{
					array[i].touchedLiquid = true;
					array[i].playerId = player.ActorNumber;
				}
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(array);
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00010B5C File Offset: 0x0000ED5C
		public static void AcidMatAll(Player player)
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj.ToString().Contains("MODDED"))
			{
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(PhotonNetwork.CurrentRoom.PlayerCount);
				ScienceExperimentManager.PlayerGameState[] array = new ScienceExperimentManager.PlayerGameState[10];
				for (int i = 0; i < (int)PhotonNetwork.CurrentRoom.PlayerCount; i++)
				{
					array[i].touchedLiquid = true;
					array[i].playerId = ((PhotonNetwork.PlayerList[i] == null) ? 0 : PhotonNetwork.PlayerList[i].ActorNumber);
				}
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(array);
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00010C2C File Offset: 0x0000EE2C
		public static void AcidMatoff(Player player = null)
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj.ToString().Contains("MODDED"))
			{
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(PhotonNetwork.CurrentRoom.PlayerCount);
				ScienceExperimentManager.PlayerGameState[] array = new ScienceExperimentManager.PlayerGameState[10];
				for (int i = 0; i < (int)PhotonNetwork.CurrentRoom.PlayerCount; i++)
				{
					array[i].touchedLiquid = true;
					array[i].playerId = 900000000;
				}
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(array);
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00010CE8 File Offset: 0x0000EEE8
		public static void CrashGun()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			Overpowered.CrashHandler();
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && gunLibData.isLocked && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null)
			{
				Overpowered.crashedPlayer = Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner;
				Overpowered.crashPlayerPosition = gunLibData.lockedPlayer.transform.position;
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00010D60 File Offset: 0x0000EF60
		public static void FloatGun()
		{
			if (!Overpowered.IsMaster())
			{
				return;
			}
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && gunLibData.isLocked && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null)
			{
				Player owner = Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner;
				if (Time.time > Overpowered.floatmoveT)
				{
					Overpowered.floatmove = !Overpowered.floatmove;
					Overpowered.floatmoveT = (Overpowered.floatmove ? (Time.time + 4f) : (Time.time + 0.2f));
				}
				if (Overpowered.floatmove)
				{
					AngryBeeSwarm.instance.targetPlayer = owner;
					AngryBeeSwarm.instance.grabbedPlayer = owner;
					AngryBeeSwarm.instance.currentState = 8;
					return;
				}
				AngryBeeSwarm.instance.targetPlayer = owner;
				AngryBeeSwarm.instance.grabbedPlayer = owner;
				AngryBeeSwarm.instance.currentState = 1;
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00010E4C File Offset: 0x0000F04C
		public static void GliderGun()
		{
			GunLib.GunLibData gunLibData = GunLib.Shoot();
			if (gunLibData != null && gunLibData.isShooting && gunLibData.isTriggered)
			{
				foreach (object obj in GameObject.Find("Environment Objects/PersistentObjects_Prefab/Gliders_Placement_Prefab/Root").transform)
				{
					foreach (object obj2 in ((Transform)obj).transform)
					{
						GliderHoldable component = ((Transform)obj2).gameObject.GetComponent<GliderHoldable>();
						component.OnGrab(null, null);
						component.photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
						component.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
						GameObject.Find("Environment Objects/PersistentObjects_Prefab/Gliders_Placement_Prefab").transform.position = gunLibData.hitPosition;
						GameObject.Find("Environment Objects/PersistentObjects_Prefab/Gliders_Placement_Prefab").transform.rotation = Quaternion.EulerAngles(Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.3f), Random.Range(0.5f, 1.3f));
						component.gameObject.transform.position = gunLibData.hitPosition + new Vector3(Random.Range(-3f, 3f), 2f, Random.Range(-3f, 3f));
					}
				}
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0001100C File Offset: 0x0000F20C
		public static void GliderAll()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				foreach (object obj in GameObject.Find("Environment Objects/PersistentObjects_Prefab/Gliders_Placement_Prefab/Root").transform)
				{
					foreach (object obj2 in ((Transform)obj).transform)
					{
						Transform transform = (Transform)obj2;
						GliderHoldable component = transform.gameObject.GetComponent<GliderHoldable>();
						component.OnGrab(null, null);
						component.photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
						component.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
						GameObject.Find("Environment Objects/PersistentObjects_Prefab/Gliders_Placement_Prefab").transform.position = transform.position;
						GameObject.Find("Environment Objects/PersistentObjects_Prefab/Gliders_Placement_Prefab").transform.rotation = Quaternion.EulerAngles(Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.3f), Random.Range(0.5f, 1.3f));
						component.gameObject.transform.position = vrrig.transform.position + new Vector3(Random.Range(-3f, 3f), 2f, Random.Range(-3f, 3f));
					}
				}
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0001120C File Offset: 0x0000F40C
		public static void StopMovement()
		{
			if (!Overpowered.IsMaster())
			{
				return;
			}
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && gunLibData.isLocked && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null)
			{
				Player owner = Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner;
				if (Time.time > Overpowered.stopmoveT)
				{
					Overpowered.stopmoveT = Time.time + 0.4f;
					Overpowered.stopmove = !Overpowered.stopmove;
				}
				if (Overpowered.stopmove)
				{
					AngryBeeSwarm.instance.targetPlayer = owner;
					AngryBeeSwarm.instance.grabbedPlayer = owner;
					AngryBeeSwarm.instance.currentState = 8;
					return;
				}
				AngryBeeSwarm.instance.targetPlayer = owner;
				AngryBeeSwarm.instance.grabbedPlayer = owner;
				AngryBeeSwarm.instance.currentState = 1;
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x000112E4 File Offset: 0x0000F4E4
		public static void VibrateGun()
		{
			if (!Overpowered.IsMaster())
			{
				return;
			}
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && (double)Time.time > (double)Overpowered.Vibrateallcooldown + 0.5 && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient)
				{
					RPCSUB.JoinedTaggedTime(Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner);
				}
				Overpowered.Vibrateallcooldown = Time.time;
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00011368 File Offset: 0x0000F568
		public static void SlowGun()
		{
			if (!Overpowered.IsMaster())
			{
				return;
			}
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && (double)Time.time > (double)Overpowered.slowallcooldown + 0.5 && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient)
				{
					RPCSUB.SetTaggedTime(Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner);
				}
				Overpowered.slowallcooldown = Time.time;
			}
		}

		// Token: 0x06000169 RID: 361 RVA: 0x000113EA File Offset: 0x0000F5EA
		public static void VibrateAll()
		{
			if ((double)Time.time > (double)Overpowered.Vibrateallcooldown + 0.5)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient)
				{
					RPCSUB.JoinedTaggedTime(0);
				}
				Overpowered.Vibrateallcooldown = Time.time;
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00011420 File Offset: 0x0000F620
		public static void SlowAll()
		{
			if ((double)Time.time > (double)Overpowered.slowallcooldown + 0.5)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient)
				{
					RPCSUB.SetTaggedTime(0);
				}
				Overpowered.slowallcooldown = Time.time;
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00011458 File Offset: 0x0000F658
		public static void TrapAllInStump()
		{
			if (Overpowered.IsModded() && Overpowered.IsMaster())
			{
				string text = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString();
				string text2;
				if (text.TryReplace("forest", "", out text2))
				{
					text = text2;
				}
				string text3;
				if (text.TryReplace("canyon", "", out text3))
				{
					text = text3;
				}
				string text4;
				if (text.TryReplace("city", "", out text4))
				{
					text = text4;
				}
				string text5;
				if (text.TryReplace("basement", "", out text5))
				{
					text = text5;
				}
				string text6;
				if (text.TryReplace("clouds", "", out text6))
				{
					text = text6;
				}
				string text7;
				if (text.TryReplace("mountain", "", out text7))
				{
					text = text7;
				}
				string text8;
				if (text.TryReplace("beach", "", out text8))
				{
					text = text8;
				}
				string text9;
				if (text.TryReplace("cave", "", out text9))
				{
					text = text9;
				}
				Hashtable hashtable = new Hashtable();
				hashtable.Add("gameMode", text);
				Hashtable hashtable2 = hashtable;
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable2, null, null);
				PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable2, null, null);
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00011580 File Offset: 0x0000F780
		public static void DisableNetworkTriggers()
		{
			if (Overpowered.IsMaster() && Overpowered.IsModded())
			{
				string[] array = new string[] { "beach", "city", "basement", "clouds", "forest", "mountains", "canyon", "cave" };
				string text = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString();
				for (int i = 0; i < array.Length; i++)
				{
					string text2;
					text.AddIfNot(array[i], out text2);
					text = text2;
				}
				Hashtable hashtable = new Hashtable();
				hashtable.Add("gameMode", text);
				Hashtable hashtable2 = hashtable;
				Overpowered.oldTriggers = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString();
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable2, null, null);
				PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable2, null, null);
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00011670 File Offset: 0x0000F870
		public static void EnableNetworkTriggers()
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("gameMode", Overpowered.oldTriggers);
			Hashtable hashtable2 = hashtable;
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable2, null, null);
			PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable2, null, null);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x000116B4 File Offset: 0x0000F8B4
		public static void sscosmetic()
		{
			foreach (CosmeticsController.CosmeticItem cosmeticItem in CosmeticsController.instance.allCosmetics)
			{
				if (cosmeticItem.itemName == "LBAFV.")
				{
					CosmeticsController.instance.itemToBuy = cosmeticItem;
				}
				CosmeticsController.instance.PurchaseItem();
				if (cosmeticItem.itemName == "LBAAK.")
				{
					CosmeticsController.instance.itemToBuy = cosmeticItem;
				}
				CosmeticsController.instance.PurchaseItem();
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0001175C File Offset: 0x0000F95C
		public static void NameAll()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			if (Time.time > Overpowered.boardTimer)
			{
				Overpowered.boardTimer = Time.time + 0.045f;
				foreach (Player player in PhotonNetwork.PlayerList)
				{
					Hashtable hashtable = new Hashtable();
					hashtable[byte.MaxValue] = PhotonNetwork.LocalPlayer.NickName;
					Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
					dictionary.Add(251, hashtable);
					dictionary.Add(254, player.ActorNumber);
					dictionary.Add(250, true);
					PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOperation(252, dictionary, SendOptions.SendUnreliable);
				}
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00011828 File Offset: 0x0000FA28
		public static void NameGun()
		{
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null && Time.time > Overpowered.boardTimer)
			{
				Overpowered.boardTimer = Time.time + 0.06f;
				Hashtable hashtable = new Hashtable();
				hashtable[byte.MaxValue] = PhotonNetwork.LocalPlayer.NickName;
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(251, hashtable);
				dictionary.Add(254, Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).OwnerActorNr);
				dictionary.Add(250, true);
				PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOperation(252, dictionary, SendOptions.SendUnreliable);
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00011908 File Offset: 0x0000FB08
		public static bool InStumpCheck()
		{
			Overpowered.isStumpChecking = true;
			Transform transform = GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").transform;
			foreach (Player player in PhotonNetwork.PlayerListOthers)
			{
				if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(player.UserId))
				{
					return false;
				}
				foreach (object obj in transform)
				{
					Transform transform2 = (Transform)obj;
					if (Overpowered.IsVectorNear(GorillaGameManager.instance.FindPlayerVRRig(player).transform.position, transform2.position, 6f))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000119E4 File Offset: 0x0000FBE4
		public static void StartAntiBan()
		{
			try
			{
				MenuPatch.isRunningAntiBan = false;
				if (Overpowered.IsModded())
				{
					PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
					Notif.SendNotification("AntiBan Already Enabled Or Your Not In A Lobby!", Color.white);
				}
				else if (Overpowered.StumpCheck && !Overpowered.InStumpCheck())
				{
					Notif.SendNotification("<color=red>A Player is about to leave/In stump!..</color> <color=green>Retrying..</color>", Color.white);
				}
				else if (Time.time <= Overpowered.antibancooldown)
				{
					if (PhotonVoiceNetwork.Instance.Client.LoadBalancingPeer.PeerState != 3)
					{
						Notif.SendNotification("Please wait until the lobby has fully loaded!", Color.white);
					}
					else
					{
						Overpowered.antibancooldown = Time.time + 10f;
						Overpowered.AntiBan();
					}
				}
			}
			catch
			{
				Debug.LogError("Unknown Error!");
				throw;
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00011AA4 File Offset: 0x0000FCA4
		public static void AntiBan()
		{
			Debug.Log("Running...");
			ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
			executeCloudScriptRequest.FunctionName = "RoomClosed";
			executeCloudScriptRequest.FunctionParameter = new
			{
				GameId = PhotonNetwork.CurrentRoom.Name,
				Region = Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper(),
				UserId = PhotonNetwork.LocalPlayer.UserId,
				ActorNr = PhotonNetwork.LocalPlayer,
				ActorCount = PhotonNetwork.ViewCount,
				AppVersion = PhotonNetwork.AppVersion
			};
			PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate(ExecuteCloudScriptResult result)
			{
				Debug.Log(result.FunctionName + " Has Been Executed!");
			}, null, null, null);
			string text = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace(GorillaComputer.instance.currentQueue, GorillaComputer.instance.currentQueue + "MODDED_MODDED_").Replace(Overpowered.GetGameMode(), Overpowered.GetGameMode() + Overpowered.GetGameMode());
			Hashtable hashtable = new Hashtable();
			hashtable.Add("gameMode", text);
			Hashtable hashtable2 = hashtable;
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable2, null, null);
			Notif.ClearAllNotifications();
			Notif.SendNotification("Antiban and Set Master Enabled!", Color.white);
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
			Overpowered.isStumpChecking = false;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00011BD8 File Offset: 0x0000FDD8
		public static void matSpamAll()
		{
			if (!Overpowered.IsMaster())
			{
				return;
			}
			Overpowered.saveKeys();
			if (Overpowered.GetGameMode().Contains("INFECTION"))
			{
				Overpowered.MatAllInf();
				return;
			}
			if (Time.time > Overpowered.mattimer)
			{
				if (Overpowered.ltagged)
				{
					Overpowered.ltagged = false;
					if (Overpowered.GetGameMode().Contains("INFECTION"))
					{
						Overpowered.GorillaTagManager.currentInfected.Clear();
						Overpowered.CopyInfectedListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("HUNT"))
					{
						Overpowered.GorillaHuntManager.currentHunted.Clear();
						Overpowered.CopyHuntDataListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("BATTLE"))
					{
						foreach (Player player in PhotonNetwork.PlayerList)
						{
							Overpowered.GorillaBattleManager.playerLives[player.ActorNumber] = 0;
						}
					}
				}
				else
				{
					Overpowered.ltagged = true;
					if (Overpowered.GetGameMode().Contains("INFECTION"))
					{
						Overpowered.GorillaTagManager.currentInfected = PhotonNetwork.PlayerList.ToList<Player>();
						Overpowered.CopyInfectedListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("HUNT"))
					{
						Overpowered.GorillaHuntManager.currentHunted = PhotonNetwork.PlayerList.ToList<Player>();
						Overpowered.CopyHuntDataListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("BATTLE"))
					{
						foreach (Player player2 in PhotonNetwork.PlayerList)
						{
							Overpowered.GorillaBattleManager.playerLives[player2.ActorNumber] = 3;
							Overpowered.CopyHuntDataListToArray();
						}
					}
				}
				Overpowered.mattimer = Time.time + 0.08f;
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00011D78 File Offset: 0x0000FF78
		public static void matSpamselggg()
		{
			if (!Overpowered.IsMaster())
			{
				return;
			}
			Overpowered.saveKeys();
			if (Time.time > Overpowered.mattimer)
			{
				if (Overpowered.ltagged)
				{
					Overpowered.ltagged = false;
					if (Overpowered.GetGameMode().Contains("INFECTION"))
					{
						Overpowered.GorillaTagManager.currentInfected.Remove(PhotonNetwork.LocalPlayer);
						Overpowered.CopyInfectedListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("HUNT"))
					{
						Overpowered.GorillaHuntManager.currentHunted.Remove(PhotonNetwork.LocalPlayer);
						Overpowered.CopyHuntDataListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("BATTLE"))
					{
						Overpowered.GorillaBattleManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 0;
					}
				}
				else
				{
					Overpowered.ltagged = true;
					if (Overpowered.GetGameMode().Contains("INFECTION"))
					{
						Overpowered.GorillaTagManager.currentInfected.Add(PhotonNetwork.LocalPlayer);
						Overpowered.CopyInfectedListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("HUNT"))
					{
						Overpowered.GorillaHuntManager.currentHunted.Add(PhotonNetwork.LocalPlayer);
						Overpowered.CopyHuntDataListToArray();
					}
					else if (Overpowered.GetGameMode().Contains("BATTLE"))
					{
						Overpowered.GorillaBattleManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 3;
						Overpowered.CopyHuntDataListToArray();
					}
				}
				Overpowered.mattimer = Time.time + 0.08f;
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00011EE4 File Offset: 0x000100E4
		public static void matSpamOnTouch()
		{
			if (!Overpowered.IsModded() && !Overpowered.IsMaster())
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				double num = (double)Vector3.Distance(GorillaTagger.Instance.rightHandTransform.transform.position, vrrig.transform.position);
				float num2 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.transform.position, vrrig.transform.position);
				float num3 = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, vrrig.transform.position);
				if ((num <= 0.3 || (double)num2 <= 0.3 || (double)num3 <= 0.5) && !vrrig.isMyPlayer && !vrrig.isOfflineVRRig)
				{
					Overpowered.saveKeys();
					Player owner = Overpowered.GetPhotonViewFromRig(vrrig).Owner;
					if (owner != null && Time.time > Overpowered.matguntimer)
					{
						Overpowered.matguntimer = Time.time + 0.1f;
						if (Overpowered.ltagged)
						{
							Overpowered.ltagged = false;
							if (Overpowered.GetGameMode().Contains("INFECTION"))
							{
								Overpowered.GorillaTagManager.currentInfected.Remove(owner);
								Overpowered.CopyInfectedListToArray();
							}
							else if (Overpowered.GetGameMode().Contains("HUNT"))
							{
								Overpowered.GorillaHuntManager.currentHunted.Remove(owner);
								Overpowered.GorillaHuntManager.currentTarget.Remove(owner);
								Overpowered.CopyHuntDataListToArray();
							}
							else if (Overpowered.GetGameMode().Contains("BATTLE"))
							{
								Overpowered.GorillaBattleManager.playerLives[owner.ActorNumber] = 0;
							}
						}
						else
						{
							Overpowered.ltagged = true;
							if (Overpowered.GetGameMode().Contains("INFECTION"))
							{
								Overpowered.GorillaTagManager.currentInfected.Add(owner);
								Overpowered.CopyInfectedListToArray();
							}
							else if (Overpowered.GetGameMode().Contains("HUNT"))
							{
								Overpowered.GorillaHuntManager.currentHunted.Add(owner);
								Overpowered.GorillaHuntManager.currentTarget.Add(owner);
								Overpowered.CopyHuntDataListToArray();
							}
							else if (Overpowered.GetGameMode().Contains("BATTLE"))
							{
								Overpowered.GorillaBattleManager.playerLives[owner.ActorNumber] = 3;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0001217C File Offset: 0x0001037C
		private static void CopyHuntDataListToArray()
		{
			Overpowered.copyListToArrayIndex = 0;
			while (Overpowered.copyListToArrayIndex < 10)
			{
				Overpowered.GorillaHuntManager.currentHuntedArray[Overpowered.copyListToArrayIndex] = 0;
				Overpowered.GorillaHuntManager.currentTargetArray[Overpowered.copyListToArrayIndex] = 0;
				Overpowered.copyListToArrayIndex++;
			}
			Overpowered.copyListToArrayIndex = Overpowered.GorillaHuntManager.currentHunted.Count - 1;
			while (Overpowered.copyListToArrayIndex >= 0)
			{
				if (Overpowered.GorillaHuntManager.currentHunted[Overpowered.copyListToArrayIndex] == null)
				{
					Overpowered.GorillaHuntManager.currentHunted.RemoveAt(Overpowered.copyListToArrayIndex);
				}
				Overpowered.copyListToArrayIndex--;
			}
			Overpowered.copyListToArrayIndex = Overpowered.GorillaHuntManager.currentTarget.Count - 1;
			while (Overpowered.copyListToArrayIndex >= 0)
			{
				if (Overpowered.GorillaHuntManager.currentTarget[Overpowered.copyListToArrayIndex] == null)
				{
					Overpowered.GorillaHuntManager.currentTarget.RemoveAt(Overpowered.copyListToArrayIndex);
				}
				Overpowered.copyListToArrayIndex--;
			}
			Overpowered.copyListToArrayIndex = 0;
			while (Overpowered.copyListToArrayIndex < Overpowered.GorillaHuntManager.currentHunted.Count)
			{
				Overpowered.GorillaHuntManager.currentHuntedArray[Overpowered.copyListToArrayIndex] = Overpowered.GorillaHuntManager.currentHunted[Overpowered.copyListToArrayIndex].ActorNumber;
				Overpowered.copyListToArrayIndex++;
			}
			Overpowered.copyListToArrayIndex = 0;
			while (Overpowered.copyListToArrayIndex < Overpowered.GorillaHuntManager.currentTarget.Count)
			{
				Overpowered.GorillaHuntManager.currentTargetArray[Overpowered.copyListToArrayIndex] = Overpowered.GorillaHuntManager.currentTarget[Overpowered.copyListToArrayIndex].ActorNumber;
				Overpowered.copyListToArrayIndex++;
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0001231C File Offset: 0x0001051C
		private static void CopyInfectedListToArray()
		{
			Overpowered.iterator1 = 0;
			while (Overpowered.iterator1 < Overpowered.GorillaTagManager.currentInfectedArray.Length)
			{
				Overpowered.GorillaTagManager.currentInfectedArray[Overpowered.iterator1] = 0;
				Overpowered.iterator1++;
			}
			Overpowered.iterator1 = Overpowered.GorillaTagManager.currentInfected.Count - 1;
			while (Overpowered.iterator1 >= 0)
			{
				if (Overpowered.GorillaTagManager.currentInfected[Overpowered.iterator1] == null)
				{
					Overpowered.GorillaTagManager.currentInfected.RemoveAt(Overpowered.iterator1);
				}
				Overpowered.iterator1--;
			}
			Overpowered.iterator1 = 0;
			while (Overpowered.iterator1 < Overpowered.GorillaTagManager.currentInfected.Count)
			{
				Overpowered.GorillaTagManager.currentInfectedArray[Overpowered.iterator1] = Overpowered.GorillaTagManager.currentInfected[Overpowered.iterator1].ActorNumber;
				Overpowered.iterator1++;
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0001240A File Offset: 0x0001060A
		public static void SetMaster()
		{
			if (!Overpowered.IsModded())
			{
				Notif.SendNotification("Enable AntiBan!!", Color.red);
				return;
			}
			if (!Overpowered.IsMaster())
			{
				PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
				return;
			}
			Notif.SendNotification("You are already masterclient!", Color.red);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00012448 File Offset: 0x00010648
		public static void AcidSpam()
		{
			if (Time.time > Overpowered.canacidchange)
			{
				Overpowered.canacidchange = Time.time + 0.8f;
				if (Overpowered.shouldacidchange)
				{
					Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(10);
					ScienceExperimentManager.PlayerGameState[] array = new ScienceExperimentManager.PlayerGameState[10];
					for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
					{
						array[i].touchedLiquid = true;
						array[i].playerId = PhotonNetwork.PlayerList[i].ActorNumber;
					}
					Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(array);
					Overpowered.shouldacidchange = false;
					return;
				}
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(10);
				ScienceExperimentManager.PlayerGameState[] array2 = new ScienceExperimentManager.PlayerGameState[10];
				for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
				{
					array2[j].touchedLiquid = false;
					array2[j].playerId = PhotonNetwork.PlayerList[j].ActorNumber;
				}
				Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(array2);
				Overpowered.shouldacidchange = true;
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0001258C File Offset: 0x0001078C
		public static void AcidSelf()
		{
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(10);
			ScienceExperimentManager.PlayerGameState[] value = Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").GetValue<ScienceExperimentManager.PlayerGameState[]>();
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
				{
					value[i].playerId = PhotonNetwork.LocalPlayer.ActorNumber;
					value[i].touchedLiquid = true;
				}
			}
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(value);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00012638 File Offset: 0x00010838
		public static void UnAcidSelf()
		{
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(0);
			ScienceExperimentManager.PlayerGameState[] value = Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").GetValue<ScienceExperimentManager.PlayerGameState[]>();
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
				{
					value[i].playerId = PhotonNetwork.LocalPlayer.ActorNumber;
					value[i].touchedLiquid = false;
				}
			}
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(value);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x000126E4 File Offset: 0x000108E4
		public static void UnAcidAll()
		{
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(0);
			ScienceExperimentManager.PlayerGameState[] value = Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").GetValue<ScienceExperimentManager.PlayerGameState[]>();
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				value[i].touchedLiquid = false;
				value[i].playerId = ((PhotonNetwork.PlayerList[i] == null) ? 0 : PhotonNetwork.PlayerList[i].ActorNumber);
			}
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(value);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00012790 File Offset: 0x00010990
		public static void AcidAll()
		{
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(10);
			ScienceExperimentManager.PlayerGameState[] value = Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").GetValue<ScienceExperimentManager.PlayerGameState[]>();
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				value[i].touchedLiquid = true;
				value[i].playerId = ((PhotonNetwork.PlayerList[i] == null) ? 0 : PhotonNetwork.PlayerList[i].ActorNumber);
			}
			Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(value);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0001283C File Offset: 0x00010A3C
		public static void MatAllInf()
		{
			if (Overpowered.manager == null)
			{
				Overpowered.manager = GameObject.Find("Gorilla Tag Manager").GetComponent<GorillaTagManager>();
			}
			if (Time.time > Overpowered.a + 0.084f)
			{
				foreach (Player player in PhotonNetwork.PlayerListOthers)
				{
					if (Overpowered.ewenum == 0)
					{
						Overpowered.UnAcidAll();
						Overpowered.manager.currentInfected.Add(player);
					}
					else if (Overpowered.ewenum == 1)
					{
						Overpowered.AcidAll();
					}
					else if (Overpowered.ewenum == 2)
					{
						Overpowered.manager.currentInfected.Remove(player);
					}
				}
				if (Overpowered.ewenum != 2)
				{
					Overpowered.ewenum++;
				}
				else
				{
					Overpowered.ewenum = 0;
				}
				Overpowered.a = Time.time;
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00012904 File Offset: 0x00010B04
		public static void CrashAll2()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			float num = Mathf.Cos(Overpowered.colorFloat * 3.1415927f * 2f) * 0.5f + 0.5f;
			float num2 = Mathf.Sin(Overpowered.colorFloat * 3.1415927f * 2f) * 0.5f + 0.5f;
			float num3 = Mathf.Cos(Overpowered.colorFloat * 3.1415927f * 2f + 1.5707964f) * 0.5f + 0.5f;
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", 1, true, new object[] { num, num2, num3 });
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", 1, true, new object[] { num, num2, num3 });
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", 1, true, new object[] { num, num2, num3 });
			if (Mathf.RoundToInt(1f / UI.deltaTime) < 100)
			{
				GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", 1, true, new object[] { num, num2, num3 });
				GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", 1, true, new object[] { num, num2, num3 });
				GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", 1, true, new object[] { num, num2, num3 });
				GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", 1, true, new object[] { num, num2, num3 });
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00012B20 File Offset: 0x00010D20
		public static void MoveCrashHandler()
		{
			if (Overpowered.crashedPlayer != null)
			{
				if (Utils.InRoom(Overpowered.crashedPlayer))
				{
					Overpowered.colorFloat = Mathf.Repeat(Overpowered.colorFloat + Time.deltaTime * float.PositiveInfinity, 1f);
					float num = Mathf.Cos(Overpowered.colorFloat * 3.1415927f * 2f) * 0.5f + 0.5f;
					float num2 = Mathf.Sin(Overpowered.colorFloat * 3.1415927f * 2f) * 0.5f + 0.5f;
					float num3 = Mathf.Cos(Overpowered.colorFloat * 3.1415927f * 2f + 1.5707964f) * 0.5f + 0.5f;
					PhotonNetwork.SendRate = 1;
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					return;
				}
				Overpowered.crashedPlayer = null;
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00012D0C File Offset: 0x00010F0C
		public static void CrashHandlerMulti()
		{
			List<Player> list = new List<Player>();
			Dictionary<Player, Vector3> dictionary = new Dictionary<Player, Vector3>();
			foreach (KeyValuePair<Player, Vector3> keyValuePair in Overpowered.crashedPlayers)
			{
				Player key = keyValuePair.Key;
				Vector3 value = keyValuePair.Value;
				if (key == null || !Utils.InRoom(key))
				{
					list.Add(key);
				}
				else
				{
					Vector3 position = GorillaGameManager.instance.FindPlayerVRRig(key).transform.position;
					if (value != position)
					{
						dictionary[key] = position;
						Overpowered.UpdatePlayerColor(key);
						Overpowered.UpdatePlayerColor(key);
					}
				}
			}
			foreach (Player player in list)
			{
				Overpowered.crashedPlayers.Remove(player);
			}
			foreach (KeyValuePair<Player, Vector3> keyValuePair2 in dictionary)
			{
				Overpowered.crashedPlayers[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00012E5C File Offset: 0x0001105C
		private static void UpdatePlayerColor(Player player)
		{
			float num = Mathf.Repeat(Time.time * float.PositiveInfinity, 1f);
			float num2 = Mathf.Cos(num * 3.1415927f * 2f) * 0.5f + 0.5f;
			float num3 = Mathf.Sin(num * 3.1415927f * 2f) * 0.5f + 0.5f;
			float num4 = Mathf.Cos(num * 3.1415927f * 2f + 1.5707964f) * 0.5f + 0.5f;
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", player, true, new object[] { num2, num3, num4 });
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00012F18 File Offset: 0x00011118
		public static void CrashHandler()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			if (Overpowered.crashedPlayer != null)
			{
				if (Utils.InRoom(Overpowered.crashedPlayer))
				{
					Overpowered.colorFloat = Mathf.Repeat(Overpowered.colorFloat + Time.deltaTime * float.PositiveInfinity, 1f);
					float num = Mathf.Cos(Overpowered.colorFloat * 3.1415927f * 2f) * 0.5f + 0.5f;
					float num2 = Mathf.Sin(Overpowered.colorFloat * 3.1415927f * 2f) * 0.5f + 0.5f;
					float num3 = Mathf.Cos(Overpowered.colorFloat * 3.1415927f * 2f + 1.5707964f) * 0.5f + 0.5f;
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
					if (XRSettings.isDeviceActive && Mathf.RoundToInt(1f / UI.deltaTime) < 100)
					{
						if (Overpowered.crashPlayerPosition != GorillaGameManager.instance.FindPlayerVRRig(Overpowered.crashedPlayer).transform.position)
						{
							GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
							GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
							Overpowered.crashPlayerPosition = GorillaGameManager.instance.FindPlayerVRRig(Overpowered.crashedPlayer).transform.position;
							return;
						}
					}
					else if (Mathf.RoundToInt(1f / UI.deltaTime) < 100)
					{
						GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
						GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", Overpowered.crashedPlayer, true, new object[] { num, num2, num3 });
						return;
					}
				}
				else
				{
					Overpowered.crashedPlayer = null;
				}
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000131F5 File Offset: 0x000113F5
		public static void LagAl()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			Overpowered.Lag(1);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00013208 File Offset: 0x00011408
		public static void Lag(Player target)
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			PhotonNetwork.SendRate = 1;
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", target, true, new object[] { 1f, 1f, 1f });
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", target, true, new object[] { 1f, 1f, 1f });
		}

		// Token: 0x06000187 RID: 391 RVA: 0x000132AC File Offset: 0x000114AC
		public static void Lag(RpcTarget target)
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			PhotonNetwork.SendRate = 1;
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", target, true, new object[] { 1f, 1f, 1f });
			GorillaTagger.Instance.myVRRig.RpcSecure("InitializeNoobMaterial", target, true, new object[] { 1f, 1f, 1f });
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00013350 File Offset: 0x00011550
		public static void LagGun()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && gunLibData.isLocked && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null)
			{
				Overpowered.Lag(Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner);
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x000133AC File Offset: 0x000115AC
		public static void CrashOnTouch()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			Overpowered.CrashHandler();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!vrrig.isMyPlayer && !vrrig.isOfflineVRRig)
				{
					float num = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.transform.position, vrrig.transform.position);
					float num2 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.transform.position, vrrig.transform.position);
					float num3 = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, vrrig.transform.position);
					float num4 = Vector3.Distance(vrrig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					float num5 = Vector3.Distance(vrrig.leftHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					float num6 = Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					if ((double)Time.time > (double)Overpowered.lagtimeout + 0.002)
					{
						Overpowered.lagtimeout = Time.time;
						if ((double)num <= 0.3 || (double)num2 <= 0.3 || (double)num3 <= 0.5 || (double)num4 <= 0.3 || (double)num5 <= 0.3 || (double)num6 <= 0.5)
						{
							if (num <= 0.3f)
							{
								GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
							}
							if (num2 <= 0.3f)
							{
								GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
							}
							Overpowered.crashedPlayer = Overpowered.GetPhotonViewFromRig(vrrig).Owner;
							Overpowered.crashPlayerPosition = vrrig.transform.position;
						}
					}
				}
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00013620 File Offset: 0x00011820
		public static void LagOnTouch()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!vrrig.isMyPlayer && !vrrig.isOfflineVRRig && Overpowered.GetPhotonViewFromRig(vrrig) != null)
				{
					float num = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.transform.position, vrrig.transform.position);
					float num2 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.transform.position, vrrig.transform.position);
					float num3 = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, vrrig.transform.position);
					float num4 = Vector3.Distance(vrrig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					float num5 = Vector3.Distance(vrrig.leftHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					float num6 = Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					if ((double)Time.time > (double)Overpowered.lagtimeout + 0.002)
					{
						Overpowered.lagtimeout = Time.time;
						if ((double)num <= 0.3 || (double)num2 <= 0.3 || (double)num3 <= 0.5 || (double)num4 <= 0.3 || (double)num5 <= 0.3 || (double)num6 <= 0.5)
						{
							if (num <= 0.3f)
							{
								GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
							}
							if (num2 <= 0.3f)
							{
								GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
							}
							Overpowered.Lag(Overpowered.GetPhotonViewFromRig(vrrig).Owner);
						}
					}
				}
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0001388C File Offset: 0x00011A8C
		public static void StutterGun()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null && gunLibData.isLocked && Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer) != null && gunLibData.lockedPlayer != null)
			{
				PhotonView photonViewFromRig = Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer);
				if (photonViewFromRig != null)
				{
					LoadBalancingClient networkingClient = PhotonNetwork.NetworkingClient;
					byte b = 204;
					Hashtable hashtable = new Hashtable();
					hashtable.Add(0, photonViewFromRig.ViewID);
					networkingClient.OpRaiseEvent(b, hashtable, new RaiseEventOptions
					{
						TargetActors = new int[] { photonViewFromRig.Owner.ActorNumber }
					}, SendOptions.SendUnreliable);
				}
			}
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0001394C File Offset: 0x00011B4C
		public static void StutterOnTouch()
		{
			if (!Overpowered.IsModded())
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!vrrig.isMyPlayer && !vrrig.isOfflineVRRig && Overpowered.GetPhotonViewFromRig(vrrig) != null)
				{
					float num = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.transform.position, vrrig.transform.position);
					float num2 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.transform.position, vrrig.transform.position);
					float num3 = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, vrrig.transform.position);
					float num4 = Vector3.Distance(vrrig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					float num5 = Vector3.Distance(vrrig.leftHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					float num6 = Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
					if ((double)num <= 0.3 || (double)num2 <= 0.3 || (double)num3 <= 0.5 || (double)num4 <= 0.3 || (double)num5 <= 0.3 || (double)num6 <= 0.5)
					{
						if (num <= 0.3f)
						{
							GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
						}
						if (num2 <= 0.3f)
						{
							GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
						}
						if (vrrig != null)
						{
							PhotonView photonViewFromRig = Overpowered.GetPhotonViewFromRig(vrrig);
							if (photonViewFromRig != null)
							{
								LoadBalancingClient networkingClient = PhotonNetwork.NetworkingClient;
								byte b = 204;
								Hashtable hashtable = new Hashtable();
								hashtable.Add(0, photonViewFromRig.ViewID);
								networkingClient.OpRaiseEvent(b, hashtable, new RaiseEventOptions
								{
									TargetActors = new int[] { photonViewFromRig.Owner.ActorNumber }
								}, SendOptions.SendUnreliable);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00013BF4 File Offset: 0x00011DF4
		public static void StutterAll()
		{
			if (Overpowered.IsModded())
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig != null)
					{
						PhotonView photonViewFromRig = Overpowered.GetPhotonViewFromRig(vrrig);
						if (photonViewFromRig != null && Time.time > Overpowered.stutterTimeout)
						{
							Overpowered.stutterTimeout = Time.time + 0.002f;
							PhotonNetwork.SendRate = 1;
							LoadBalancingClient networkingClient = PhotonNetwork.NetworkingClient;
							byte b = 204;
							Hashtable hashtable = new Hashtable();
							hashtable.Add(0, photonViewFromRig.ViewID);
							networkingClient.OpRaiseEvent(b, hashtable, new RaiseEventOptions
							{
								Receivers = 0
							}, SendOptions.SendUnreliable);
						}
					}
				}
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00013CC8 File Offset: 0x00011EC8
		public static void MatGun()
		{
			if (!Overpowered.IsMaster())
			{
				return;
			}
			GunLib.GunLibData gunLibData = GunLib.ShootLock();
			if (gunLibData != null && gunLibData.lockedPlayer != null)
			{
				Overpowered.saveKeys();
				Player owner = Overpowered.GetPhotonViewFromRig(gunLibData.lockedPlayer).Owner;
				if (owner != null && Time.time > Overpowered.matguntimer)
				{
					Overpowered.matguntimer = Time.time + 0.1f;
					if (Overpowered.ltagged)
					{
						Overpowered.ltagged = false;
						if (Overpowered.GetGameMode().Contains("INFECTION"))
						{
							Overpowered.GorillaTagManager.currentInfected.Remove(owner);
							Overpowered.CopyInfectedListToArray();
							return;
						}
						if (Overpowered.GetGameMode().Contains("HUNT"))
						{
							Overpowered.GorillaHuntManager.currentHunted.Remove(owner);
							Overpowered.GorillaHuntManager.currentTarget.Remove(owner);
							Overpowered.CopyHuntDataListToArray();
							return;
						}
						if (Overpowered.GetGameMode().Contains("BATTLE"))
						{
							Overpowered.GorillaBattleManager.playerLives[owner.ActorNumber] = 0;
							return;
						}
					}
					else
					{
						Overpowered.ltagged = true;
						if (Overpowered.GetGameMode().Contains("INFECTION"))
						{
							Overpowered.GorillaTagManager.currentInfected.Add(owner);
							Overpowered.CopyInfectedListToArray();
							return;
						}
						if (Overpowered.GetGameMode().Contains("HUNT"))
						{
							Overpowered.GorillaHuntManager.currentHunted.Add(owner);
							Overpowered.GorillaHuntManager.currentTarget.Add(owner);
							Overpowered.CopyHuntDataListToArray();
							return;
						}
						if (Overpowered.GetGameMode().Contains("BATTLE"))
						{
							Overpowered.GorillaBattleManager.playerLives[owner.ActorNumber] = 3;
						}
					}
				}
			}
		}

		// Token: 0x04000109 RID: 265
		public static GorillaBattleManager GorillaBattleManager;

		// Token: 0x0400010A RID: 266
		public static GorillaHuntManager GorillaHuntManager;

		// Token: 0x0400010B RID: 267
		public static GorillaTagManager GorillaTagManager;

		// Token: 0x0400010C RID: 268
		public static GorillaScoreBoard[] boards = null;

		// Token: 0x0400010D RID: 269
		public static float antibancooldown = -3E+12f;

		// Token: 0x0400010E RID: 270
		public static bool antiban = true;

		// Token: 0x0400010F RID: 271
		private static float boardTimer;

		// Token: 0x04000110 RID: 272
		private static float slowallcooldown = -1f;

		// Token: 0x04000111 RID: 273
		private static float Vibrateallcooldown = -1f;

		// Token: 0x04000112 RID: 274
		private static bool floatmove;

		// Token: 0x04000113 RID: 275
		private static float floatmoveT;

		// Token: 0x04000114 RID: 276
		private static bool stopmove;

		// Token: 0x04000115 RID: 277
		private static float stopmoveT;

		// Token: 0x04000116 RID: 278
		private static float mattimer = 0f;

		// Token: 0x04000117 RID: 279
		private static int iterator1;

		// Token: 0x04000118 RID: 280
		private static int copyListToArrayIndex;

		// Token: 0x04000119 RID: 281
		private static bool ltagged = false;

		// Token: 0x0400011A RID: 282
		private static float matguntimer = -222f;

		// Token: 0x0400011B RID: 283
		private static float lagtimeout;

		// Token: 0x0400011C RID: 284
		public static float colorFloat = 0f;

		// Token: 0x0400011D RID: 285
		private static Player crashedPlayer;

		// Token: 0x0400011E RID: 286
		public static bool isStumpChecking = false;

		// Token: 0x0400011F RID: 287
		public static float CheckedFor = 0f;

		// Token: 0x04000120 RID: 288
		private static string oldTriggers;

		// Token: 0x04000121 RID: 289
		private static bool shouldacidchange = false;

		// Token: 0x04000122 RID: 290
		private static float canacidchange = -100f;

		// Token: 0x04000123 RID: 291
		private static GorillaTagManager manager = null;

		// Token: 0x04000124 RID: 292
		private static float a = 0f;

		// Token: 0x04000125 RID: 293
		private static Vector3 crashPlayerPosition = Vector3.zero;

		// Token: 0x04000126 RID: 294
		private static int ewenum;

		// Token: 0x04000127 RID: 295
		private static Dictionary<Player, Vector3> crashedPlayers = new Dictionary<Player, Vector3>();

		// Token: 0x04000128 RID: 296
		private static bool changeNameOnJoin;

		// Token: 0x04000129 RID: 297
		public static bool StumpCheck = true;

		// Token: 0x0400012A RID: 298
		private static float stutterTimeout;
	}
}
