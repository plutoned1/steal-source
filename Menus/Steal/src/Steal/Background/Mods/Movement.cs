using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Steal.Patchers;
using UnityEngine;
using UnityEngine.XR;

namespace Steal.Background.Mods
{
	// Token: 0x02000040 RID: 64
	internal class Movement : Mod
	{
		// Token: 0x0600011A RID: 282 RVA: 0x0000DA68 File Offset: 0x0000BC68
		public static float WallWalkMultiplierManager(float currentSpeed)
		{
			float num = currentSpeed;
			if (num == 3f)
			{
				num = 4f;
			}
			else if (num == 4f)
			{
				num = 6f;
			}
			else if (num == 6f)
			{
				num = 7f;
			}
			else if (num == 7f)
			{
				num = 8f;
			}
			else if (num == 8f)
			{
				num = 9f;
			}
			else if (num == 9f)
			{
				num = 3f;
			}
			return num;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000DAD8 File Offset: 0x0000BCD8
		public static float multiplierManager(float currentSpeed)
		{
			float num = currentSpeed;
			if (num == 1.15f)
			{
				num = 1.3f;
			}
			else if (num == 1.3f)
			{
				num = 1.5f;
			}
			else if (num == 1.5f)
			{
				num = 1.7f;
			}
			else if (num == 1.7f)
			{
				num = 2f;
			}
			else if (num == 2f)
			{
				num = 3f;
			}
			else if (num == 3f)
			{
				num = 1.15f;
			}
			return num;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000DB48 File Offset: 0x0000BD48
		public static void Reverse()
		{
			if (InputHandler.LeftGrip)
			{
				for (int i = Movement.positions.Count - 1; i >= 0; i--)
				{
					if (Movement.RewindHelp == 0f)
					{
						Player.Instance.transform.position = Movement.positions[i];
						Movement.positions.RemoveAt(i);
						Movement.RewindHelp = Time.deltaTime + 1f;
					}
				}
				return;
			}
			if (InputHandler.RightGrip)
			{
				Movement.positions.Add(Player.Instance.transform.position);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000DBD5 File Offset: 0x0000BDD5
		public static float getSlideMultiplier()
		{
			return Movement.slideControlMultiplier;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000DBDC File Offset: 0x0000BDDC
		public static void slideControl(bool enable, float control)
		{
			if (enable)
			{
				Player.Instance.slideControl = control;
				return;
			}
			Player.Instance.slideControl = 0.00425f;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000DBFC File Offset: 0x0000BDFC
		public static void SwitchSlide()
		{
			Movement.slideControlMultiplier = Movement.multiplierManager(Movement.slideControlMultiplier);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000DC10 File Offset: 0x0000BE10
		public static void AdvancedWASD(float speed)
		{
			GorillaTagger.Instance.rigidbody.useGravity = false;
			GorillaTagger.Instance.rigidbody.velocity = new Vector3(0f, 0f, 0f);
			float num = speed * Time.deltaTime;
			if (UnityInput.Current.GetKey(304) || UnityInput.Current.GetKey(303))
			{
				num *= 10f;
			}
			if (UnityInput.Current.GetKey(276) || UnityInput.Current.GetKey(97))
			{
				Camera.main.transform.position += Camera.main.transform.right * -1f * num;
			}
			if (UnityInput.Current.GetKey(275) || UnityInput.Current.GetKey(100))
			{
				Camera.main.transform.position += Camera.main.transform.right * num;
			}
			if (UnityInput.Current.GetKey(273) || UnityInput.Current.GetKey(119))
			{
				Camera.main.gameObject.transform.position += Camera.main.transform.forward * num;
			}
			if (UnityInput.Current.GetKey(274) || UnityInput.Current.GetKey(115))
			{
				Camera.main.transform.position += Camera.main.transform.forward * -1f * num;
			}
			if (UnityInput.Current.GetKey(32) || UnityInput.Current.GetKey(280))
			{
				Camera.main.transform.position += Camera.main.transform.up * num;
			}
			if (UnityInput.Current.GetKey(306) || UnityInput.Current.GetKey(281))
			{
				Camera.main.transform.position += Camera.main.transform.up * -1f * num;
			}
			if (UnityInput.Current.GetMouseButton(1))
			{
				Vector3 vector = UnityInput.Current.mousePosition - Movement.previousMousePosition;
				float num2 = Camera.main.transform.localEulerAngles.y + vector.x * 0.3f;
				float num3 = Camera.main.transform.localEulerAngles.x - vector.y * 0.3f;
				Camera.main.transform.localEulerAngles = new Vector3(num3, num2, 0f);
			}
			Movement.previousMousePosition = UnityInput.Current.mousePosition;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000DF0C File Offset: 0x0000C10C
		public static void ProcessCheckPoint(bool on)
		{
			if (on)
			{
				if (InputHandler.RightGrip)
				{
					if (Movement.pointer == null)
					{
						Movement.pointer = GameObject.CreatePrimitive(0);
						Object.Destroy(Movement.pointer.GetComponent<Rigidbody>());
						Object.Destroy(Movement.pointer.GetComponent<SphereCollider>());
						Movement.pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
					}
					Movement.pointer.transform.position = Player.Instance.rightControllerTransform.position;
				}
				else if (!InputHandler.RightGrip && !InputHandler.RightTrigger)
				{
					Object.Destroy(Movement.pointer);
					Movement.pointer = null;
				}
				if (!InputHandler.RightGrip && InputHandler.RightTrigger)
				{
					Movement.pointer.GetComponent<Renderer>().material.color = Color.green;
					TeleportationLib.Teleport(Movement.pointer.transform.position);
				}
				if (!InputHandler.RightTrigger)
				{
					Movement.pointer.GetComponent<Renderer>().material.color = Color.red;
					return;
				}
			}
			else
			{
				Object.Destroy(Movement.pointer);
				Movement.pointer = null;
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000E02C File Offset: 0x0000C22C
		public static void TeleportGun()
		{
			GunLib.GunLibData gunLibData = GunLib.Shoot();
			if (gunLibData != null)
			{
				if (gunLibData.isShooting && gunLibData.isTriggered)
				{
					if (Movement.canTP)
					{
						Movement.canTP = false;
						TeleportationLib.Teleport(gunLibData.hitPosition);
						return;
					}
				}
				else
				{
					Movement.canTP = true;
				}
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000E071 File Offset: 0x0000C271
		public static void ChangePlatforms()
		{
			if (Movement.currentPlatform < 2)
			{
				Movement.currentPlatform++;
				return;
			}
			Movement.currentPlatform = 0;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000E090 File Offset: 0x0000C290
		public static void Platforms()
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = 0
			};
			if (InputHandler.RightGrip)
			{
				if (Movement.RightPlat == null)
				{
					Movement.RightPlat = GameObject.CreatePrimitive(3);
					if (Movement.currentPlatform != 2)
					{
						Movement.RightPlat.transform.position = new Vector3(0f, -0.0175f, 0f) + Player.Instance.rightControllerTransform.position;
					}
					else
					{
						Movement.RightPlat.transform.position = new Vector3(0f, 0.025f, 0f) + Player.Instance.rightControllerTransform.position;
					}
					Movement.RightPlat.transform.rotation = Player.Instance.rightControllerTransform.rotation;
					Movement.RightPlat.transform.localScale = Movement.scale;
					if (Movement.currentPlatform == 1)
					{
						if (Movement.RightPlat.GetComponent<MeshRenderer>() != null)
						{
							Object.Destroy(Movement.RightPlat.GetComponent<MeshRenderer>());
						}
					}
					else
					{
						if (Movement.RightPlat.GetComponent<MeshRenderer>() == null)
						{
							Movement.RightPlat.AddComponent<MeshRenderer>();
						}
						Movement.RightPlat.GetComponent<Renderer>().material.color = Color.black;
						PhotonNetwork.RaiseEvent(110, new object[]
						{
							Movement.RightPlat.transform.position,
							Movement.RightPlat.transform.rotation,
							Movement.scale,
							Movement.RightPlat.GetComponent<Renderer>().material.color
						}, raiseEventOptions, SendOptions.SendReliable);
					}
				}
			}
			else if (Movement.RightPlat != null)
			{
				PhotonNetwork.RaiseEvent(111, null, raiseEventOptions, SendOptions.SendReliable);
				Object.Destroy(Movement.RightPlat);
				Movement.RightPlat = null;
			}
			if (InputHandler.LeftGrip)
			{
				if (Movement.LeftPlat == null)
				{
					Movement.LeftPlat = GameObject.CreatePrimitive(3);
					Movement.LeftPlat.transform.localScale = Movement.scale;
					if (Movement.currentPlatform != 2)
					{
						Movement.LeftPlat.transform.position = new Vector3(0f, -0.0175f, 0f) + Player.Instance.leftControllerTransform.position;
					}
					else
					{
						Movement.LeftPlat.transform.position = new Vector3(0f, 0.025f, 0f) + Player.Instance.leftControllerTransform.position;
					}
					Movement.LeftPlat.transform.rotation = Player.Instance.leftControllerTransform.rotation;
					if (Movement.currentPlatform != 1)
					{
						if (Movement.LeftPlat.GetComponent<MeshRenderer>() == null)
						{
							Movement.LeftPlat.AddComponent<MeshRenderer>();
						}
						Movement.LeftPlat.GetComponent<Renderer>().material.color = Color.black;
						PhotonNetwork.RaiseEvent(110, new object[]
						{
							Movement.LeftPlat.transform.position,
							Movement.LeftPlat.transform.rotation,
							Movement.scale,
							Movement.LeftPlat.GetComponent<Renderer>().material.color
						}, raiseEventOptions, SendOptions.SendReliable);
						return;
					}
					if (Movement.LeftPlat.GetComponent<MeshRenderer>() != null)
					{
						Object.Destroy(Movement.LeftPlat.GetComponent<MeshRenderer>());
						return;
					}
				}
			}
			else if (Movement.LeftPlat != null)
			{
				PhotonNetwork.RaiseEvent(121, null, raiseEventOptions, SendOptions.SendReliable);
				Object.Destroy(Movement.LeftPlat);
				Movement.LeftPlat = null;
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000E444 File Offset: 0x0000C644
		public static void PlatformNetwork(EventData data)
		{
			if (data.Code == 110)
			{
				object[] array = (object[])data.CustomData;
				Movement.RightPlat_Networked[data.Sender] = GameObject.CreatePrimitive(3);
				Movement.RightPlat_Networked[data.Sender].transform.position = (Vector3)array[0];
				Movement.RightPlat_Networked[data.Sender].transform.rotation = (Quaternion)array[1];
				Movement.RightPlat_Networked[data.Sender].transform.localScale = (Vector3)array[2];
				Movement.RightPlat_Networked[data.Sender].GetComponent<Renderer>().material.color = (Color)array[3];
				Movement.RightPlat_Networked[data.Sender].GetComponent<BoxCollider>().enabled = false;
			}
			if (data.Code == 120)
			{
				object[] array2 = (object[])data.CustomData;
				Movement.LeftPlat_Networked[data.Sender] = GameObject.CreatePrimitive(3);
				Movement.LeftPlat_Networked[data.Sender].transform.position = (Vector3)array2[0];
				Movement.LeftPlat_Networked[data.Sender].transform.rotation = (Quaternion)array2[1];
				Movement.LeftPlat_Networked[data.Sender].transform.localScale = (Vector3)array2[2];
				Movement.LeftPlat_Networked[data.Sender].GetComponent<Renderer>().material.color = (Color)array2[3];
				Movement.LeftPlat_Networked[data.Sender].GetComponent<BoxCollider>().enabled = false;
			}
			if (data.Code == 110)
			{
				Object.Destroy(Movement.RightPlat_Networked[data.Sender]);
				Movement.RightPlat_Networked[data.Sender] = null;
			}
			if (data.Code == 121)
			{
				Object.Destroy(Movement.LeftPlat_Networked[data.Sender]);
				Movement.LeftPlat_Networked[data.Sender] = null;
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000E620 File Offset: 0x0000C820
		public static void CarMonke()
		{
			if (!Movement.Start)
			{
				Movement.multiplier = 3f;
				Movement.Start = true;
			}
			Movement.left_joystick = InputHandler.LeftJoystick;
			RaycastHit raycastHit;
			Physics.Raycast(Player.Instance.bodyCollider.transform.position, Vector3.down, ref raycastHit, 100f, Movement.layers);
			Movement.head_direction = Player.Instance.headCollider.transform.forward;
			Movement.roll_direction = Vector3.ProjectOnPlane(Movement.head_direction, raycastHit.normal);
			if (Movement.left_joystick.y != 0f)
			{
				if (Movement.left_joystick.y < 0f)
				{
					if (Movement.speed > -Movement.maxs)
					{
						Movement.speed -= Movement.acceleration * Math.Abs(Movement.left_joystick.y) * Time.deltaTime;
					}
				}
				else if (Movement.speed < Movement.maxs)
				{
					Movement.speed += Movement.acceleration * Math.Abs(Movement.left_joystick.y) * Time.deltaTime;
				}
			}
			else if (Movement.speed < 0f)
			{
				Movement.speed += Movement.acceleration * Time.deltaTime * 0.5f;
			}
			else if (Movement.speed > 0f)
			{
				Movement.speed -= Movement.acceleration * Time.deltaTime * 0.5f;
			}
			if (Movement.speed > Movement.maxs)
			{
				Movement.speed = Movement.maxs;
			}
			if (Movement.speed < -Movement.maxs)
			{
				Movement.speed = -Movement.maxs;
			}
			if (Movement.speed != 0f && raycastHit.distance < Movement.distance)
			{
				Player.Instance.bodyCollider.attachedRigidbody.velocity = Movement.roll_direction.normalized * Movement.speed * Movement.multiplier;
			}
			if (Player.Instance.IsHandTouching(true) || Player.Instance.IsHandTouching(false))
			{
				Movement.speed *= 0.75f;
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000E834 File Offset: 0x0000CA34
		public static void Rewind()
		{
			MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
			if (InputHandler.LeftGrip)
			{
				Movement.Macro.Add(Player.Instance.transform.position);
			}
			if (InputHandler.RightGrip)
			{
				Player.Instance.bodyCollider.attachedRigidbody.useGravity = false;
				if (!Movement.clipped)
				{
					foreach (MeshCollider meshCollider in array)
					{
						meshCollider.transform.localScale = meshCollider.transform.localScale / 10000f;
					}
				}
				Movement.clipped = true;
				for (int j = 0; j < Movement.Macro.Count; j++)
				{
					if (Movement.MacroHelp == 0f)
					{
						Player.Instance.transform.position = Movement.Macro[j];
						Movement.Macro.RemoveAt(j);
						Movement.MacroHelp = Time.deltaTime + 1f;
					}
				}
			}
			else
			{
				Player.Instance.bodyCollider.attachedRigidbody.useGravity = true;
			}
			if (Movement.clipped && !InputHandler.RightGrip)
			{
				foreach (MeshCollider meshCollider2 in array)
				{
					meshCollider2.transform.localScale = meshCollider2.transform.localScale * 10000f;
				}
				Movement.clipped = false;
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000E985 File Offset: 0x0000CB85
		public static void DisableReverseTime()
		{
			Movement.positions.Clear();
			Movement.Macro.Clear();
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000E99C File Offset: 0x0000CB9C
		public static void MonkeClimb()
		{
			Player instance = Player.Instance;
			if (!PhotonNetwork.InLobby)
			{
				Movement.RG = ControllerInputPoller.instance.rightControllerGripFloat;
				Movement.LG = ControllerInputPoller.instance.leftControllerGripFloat;
				RaycastHit raycastHit;
				bool flag = Physics.Raycast(instance.leftControllerTransform.position, instance.leftControllerTransform.right, ref raycastHit, 0.2f, Movement.layers);
				if ((Physics.Raycast(instance.rightControllerTransform.position, -instance.rightControllerTransform.right, ref raycastHit, 0.2f, Movement.layers) || Movement.AirMode) && Movement.RG > 0.5f)
				{
					if (!Movement.RGrabbing)
					{
						Movement.CurHandPos = instance.rightControllerTransform.position;
						Movement.RGrabbing = true;
						Movement.LGrabbing = false;
					}
					Movement.ApplyVelocity(instance.rightControllerTransform.position, Movement.CurHandPos, instance);
					return;
				}
				if ((flag || Movement.AirMode) && Movement.LG > 0.5f)
				{
					if (!Movement.LGrabbing)
					{
						Movement.CurHandPos = instance.leftControllerTransform.position;
						Movement.LGrabbing = true;
						Movement.RGrabbing = false;
					}
					Movement.ApplyVelocity(instance.leftControllerTransform.position, Movement.CurHandPos, instance);
					return;
				}
				if (Movement.LGrabbing || Movement.RGrabbing)
				{
					Physics.gravity = new Vector3(0f, -9.81f, 0f);
					Movement.LGrabbing = false;
					Movement.RGrabbing = false;
				}
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000EB01 File Offset: 0x0000CD01
		public static void FasterSwimming()
		{
			if (Player.Instance.InWater)
			{
				Player.Instance.bodyCollider.attachedRigidbody.velocity = Player.Instance.bodyCollider.attachedRigidbody.velocity * 1.013f;
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000EB44 File Offset: 0x0000CD44
		public static void BHop()
		{
			if (InputHandler.RightSecondary)
			{
				if (Movement.canBHop)
				{
					Movement.isBHop = !Movement.isBHop;
					Movement.canBHop = false;
				}
			}
			else
			{
				Movement.canBHop = true;
			}
			if (Movement.isBHop)
			{
				if (Player.Instance.IsHandTouching(false))
				{
					Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
					Player.Instance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
					Player.Instance.GetComponent<Rigidbody>().AddForce(Vector3.up * 270f, 1);
					Player.Instance.GetComponent<Rigidbody>().AddForce(GorillaTagger.Instance.offlineVRRig.rightHandPlayer.transform.right * 220f, 1);
				}
				if (Player.Instance.IsHandTouching(true))
				{
					Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
					Player.Instance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
					Player.Instance.GetComponent<Rigidbody>().AddForce(Vector3.up * 270f, 1);
					Player.Instance.GetComponent<Rigidbody>().AddForce(-GorillaTagger.Instance.offlineVRRig.leftHandPlayer.transform.right * 220f, 1);
				}
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000EC97 File Offset: 0x0000CE97
		public static void ZeroGravity()
		{
			Physics.gravity = new Vector3(0f, 0f, 0f);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000ECB4 File Offset: 0x0000CEB4
		public static void WallWalk()
		{
			Vector3 vector = default(Vector3);
			if ((Player.Instance.wasLeftHandTouching || Player.Instance.wasRightHandTouching) && (InputHandler.LeftGrip || InputHandler.RightGrip))
			{
				vector = ((RaycastHit)typeof(Player).GetField("lastHitInfoHand", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Player.Instance)).normal;
			}
			if (InputHandler.LeftGrip || InputHandler.RightGrip)
			{
				Player.Instance.bodyCollider.attachedRigidbody.AddForce(vector * -5f * Movement.WallWalkMultiplier, 5);
				Physics.gravity = new Vector3(0f, 0f, 0f);
				return;
			}
			Physics.gravity = new Vector3(0f, -9.81f, 0f);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000ED88 File Offset: 0x0000CF88
		public static void ResetGravity()
		{
			Physics.gravity = new Vector3(0f, -9.81f, 0f);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000EDA4 File Offset: 0x0000CFA4
		private static void ApplyVelocity(Vector3 pos, Vector3 target, Player __instance)
		{
			Physics.gravity = new Vector3(0f, 0f, 0f);
			Vector3 vector = target - pos;
			__instance.bodyCollider.attachedRigidbody.velocity = vector * 65f;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000EDED File Offset: 0x0000CFED
		public static void LeftDrawRope(Player __instance)
		{
			if (Movement.leftjoint)
			{
				Movement.leftlr.SetPosition(0, __instance.leftControllerTransform.position);
				Movement.leftlr.SetPosition(1, Movement.leftgrapplePoint);
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000EE24 File Offset: 0x0000D024
		public static void LeftStopGrapple()
		{
			Movement.leftlr.positionCount = 0;
			Object.Destroy(Movement.leftjoint);
			Movement.canleftgrapple = true;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000EE44 File Offset: 0x0000D044
		public static void SpiderMonke()
		{
			ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "spiderpatch.cfg"), true);
			Movement.sp = configFile.Bind<float>("Configuration", "Spring", 10f, "spring");
			Movement.dp = configFile.Bind<float>("Configuration", "Damper", 30f, "damper");
			Movement.ms = configFile.Bind<float>("Configuration", "MassScale", 12f, "massscale");
			Movement.rc = configFile.Bind<Color>("Configuration", "webColor", Color.white, "webcolor hex code");
			if (!Movement.wackstart)
			{
				GameObject gameObject = new GameObject();
				Movement.Spring = Movement.sp.Value;
				Movement.Damper = Movement.dp.Value;
				Movement.MassScale = Movement.ms.Value;
				Movement.grapplecolor = Movement.rc.Value;
				Movement.lr = Player.Instance.gameObject.AddComponent<LineRenderer>();
				Movement.lr.material = new Material(Shader.Find("Sprites/Default"));
				Movement.lr.startColor = Movement.grapplecolor;
				Movement.lr.endColor = Movement.grapplecolor;
				Movement.lr.startWidth = 0.02f;
				Movement.lr.endWidth = 0.02f;
				Movement.leftlr = gameObject.AddComponent<LineRenderer>();
				Movement.leftlr.material = new Material(Shader.Find("Sprites/Default"));
				Movement.leftlr.startColor = Movement.grapplecolor;
				Movement.leftlr.endColor = Movement.grapplecolor;
				Movement.leftlr.startWidth = 0.02f;
				Movement.leftlr.endWidth = 0.02f;
				Movement.wackstart = true;
			}
			Movement.DrawRope(Player.Instance);
			Movement.LeftDrawRope(Player.Instance);
			if (InputHandler.RightTrigger)
			{
				if (Movement.cangrapple)
				{
					Movement.Spring = Movement.sp.Value;
					Movement.StartGrapple(Player.Instance);
					Movement.cangrapple = false;
				}
			}
			else
			{
				Movement.StopGrapple(Player.Instance);
			}
			if (InputHandler.LeftTrigger)
			{
				Movement.Spring /= 2f;
			}
			else
			{
				Movement.Spring = Movement.sp.Value;
			}
			if (InputHandler.LeftTrigger)
			{
				if (Movement.canleftgrapple)
				{
					Movement.Spring = Movement.sp.Value;
					Movement.LeftStartGrapple(Player.Instance);
					Movement.canleftgrapple = false;
					return;
				}
			}
			else
			{
				Movement.LeftStopGrapple();
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000F0A8 File Offset: 0x0000D2A8
		public static void LeftStartGrapple(Player __instance)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(__instance.leftControllerTransform.position, __instance.leftControllerTransform.forward, ref raycastHit, Movement.maxDistance))
			{
				Movement.leftgrapplePoint = raycastHit.point;
				Movement.leftjoint = __instance.gameObject.AddComponent<SpringJoint>();
				Movement.leftjoint.autoConfigureConnectedAnchor = false;
				Movement.leftjoint.connectedAnchor = Movement.leftgrapplePoint;
				float num = Vector3.Distance(__instance.bodyCollider.attachedRigidbody.position, Movement.leftgrapplePoint);
				Movement.leftjoint.maxDistance = num * 0.8f;
				Movement.leftjoint.minDistance = num * 0.25f;
				Movement.leftjoint.spring = Movement.Spring;
				Movement.leftjoint.damper = Movement.Damper;
				Movement.leftjoint.massScale = Movement.MassScale;
				Movement.leftlr.positionCount = 2;
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000F188 File Offset: 0x0000D388
		public static void StartGrapple(Player __instance)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(__instance.rightControllerTransform.position, __instance.rightControllerTransform.forward, ref raycastHit, Movement.maxDistance))
			{
				Movement.grapplePoint = raycastHit.point;
				Movement.joint = __instance.gameObject.AddComponent<SpringJoint>();
				Movement.joint.autoConfigureConnectedAnchor = false;
				Movement.joint.connectedAnchor = Movement.grapplePoint;
				float num = Vector3.Distance(__instance.bodyCollider.attachedRigidbody.position, Movement.grapplePoint);
				Movement.joint.maxDistance = num * 0.8f;
				Movement.joint.minDistance = num * 0.25f;
				Movement.joint.spring = Movement.Spring;
				Movement.joint.damper = Movement.Damper;
				Movement.joint.massScale = Movement.MassScale;
				Movement.lr.positionCount = 2;
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000F267 File Offset: 0x0000D467
		public static void DrawRope(Player __instance)
		{
			if (Movement.joint)
			{
				Movement.lr.SetPosition(0, __instance.rightControllerTransform.position);
				Movement.lr.SetPosition(1, Movement.grapplePoint);
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000F29E File Offset: 0x0000D49E
		public static void StopGrapple(Player __instance)
		{
			Movement.lr.positionCount = 0;
			Object.Destroy(Movement.joint);
			Movement.cangrapple = true;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000F2BC File Offset: 0x0000D4BC
		public static void ProcessIronMonke()
		{
			Rigidbody attachedRigidbody = Player.Instance.bodyCollider.attachedRigidbody;
			if (InputHandler.RightGrip)
			{
				attachedRigidbody.AddForce(8f * Player.Instance.rightControllerTransform.right, 5);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 50f * attachedRigidbody.velocity.magnitude, GorillaTagger.Instance.tapHapticDuration);
			}
			if (InputHandler.LeftGrip)
			{
				attachedRigidbody.AddForce(-8f * Player.Instance.leftControllerTransform.right, 5);
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 50f * attachedRigidbody.velocity.magnitude, GorillaTagger.Instance.tapHapticDuration);
			}
			if (InputHandler.LeftGrip | InputHandler.RightGrip)
			{
				attachedRigidbody.velocity = Vector3.ClampMagnitude(attachedRigidbody.velocity, 50f);
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000F3B4 File Offset: 0x0000D5B4
		public static void GrappleHook()
		{
			RaycastHit raycastHit;
			Physics.Raycast(Player.Instance.rightControllerTransform.position, -Player.Instance.rightControllerTransform.up, ref raycastHit);
			if (InputHandler.RightGrip && !InputHandler.RightTrigger)
			{
				Movement.disablegrapple = false;
				if (Movement.lineRenderer == null)
				{
					Movement.lineRenderer = new GameObject("line").AddComponent<LineRenderer>();
					Movement.lineRenderer.startColor = new Color(0f, 0f, 0f, 1f);
					Movement.lineRenderer.endColor = new Color(0f, 0f, 0f, 1f);
					Movement.lineRenderer.startWidth = 0.01f;
					Movement.lineRenderer.endWidth = 0.01f;
					Movement.lineRenderer.positionCount = 2;
					Movement.lineRenderer.useWorldSpace = true;
					Movement.lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
				}
			}
			if (InputHandler.RightGrip && InputHandler.RightTrigger && !Movement.disablegrapple)
			{
				if (Vector3.Distance(Player.Instance.bodyCollider.transform.position, raycastHit.point) < 4f)
				{
					Movement.disablegrapple = true;
					Object.Destroy(Movement.lineRenderer);
					Movement.lineRenderer = null;
					return;
				}
				Vector3 vector = (raycastHit.point - Player.Instance.bodyCollider.transform.position).normalized * 30f;
				Player.Instance.bodyCollider.attachedRigidbody.AddForce(vector, 5);
			}
			if (!InputHandler.RightGrip && !InputHandler.RightTrigger && Movement.lineRenderer != null)
			{
				Object.Destroy(Movement.lineRenderer);
			}
			if (Movement.lineRenderer != null)
			{
				Movement.lineRenderer.SetPosition(0, raycastHit.point);
				Movement.lineRenderer.SetPosition(1, Player.Instance.rightControllerTransform.position);
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000F5C0 File Offset: 0x0000D7C0
		public static void AirstrikeGun()
		{
			GunLib.GunLibData gunLibData = GunLib.Shoot();
			if (gunLibData != null && gunLibData.isShooting && gunLibData.isTriggered)
			{
				Player.Instance.transform.position = gunLibData.hitPosition;
				Player.Instance.GetComponent<Rigidbody>().velocity = new Vector3(0f, 55f, 0f);
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000F620 File Offset: 0x0000D820
		public static void SuperMonkey()
		{
			if (InputHandler.RightPrimary)
			{
				Player.Instance.transform.position += Player.Instance.rightControllerTransform.forward * Time.deltaTime * (12f * Movement.flightMultiplier);
				Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
				if (!Movement.flying)
				{
					Movement.flying = true;
				}
			}
			else if (Movement.flying)
			{
				Player.Instance.GetComponent<Rigidbody>().velocity = Player.Instance.headCollider.transform.forward * Time.deltaTime * 12f;
				Movement.flying = false;
			}
			if (InputHandler.RightSecondary)
			{
				if (!Movement.gravityToggled && Player.Instance.bodyCollider.attachedRigidbody.useGravity)
				{
					Player.Instance.bodyCollider.attachedRigidbody.useGravity = false;
					Movement.gravityToggled = true;
					return;
				}
				if (!Movement.gravityToggled && !Player.Instance.bodyCollider.attachedRigidbody.useGravity)
				{
					Player.Instance.bodyCollider.attachedRigidbody.useGravity = true;
					Movement.gravityToggled = true;
					return;
				}
			}
			else
			{
				Movement.gravityToggled = false;
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000F760 File Offset: 0x0000D960
		public static void NoClip()
		{
			if (InputHandler.LeftTrigger || !XRSettings.isDeviceActive)
			{
				if (!Movement.isnoclipped)
				{
					MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
					if (array != null)
					{
						foreach (MeshCollider meshCollider in array)
						{
							if (meshCollider.enabled)
							{
								meshCollider.enabled = false;
							}
						}
					}
					Movement.isnoclipped = true;
					return;
				}
			}
			else if (Movement.isnoclipped)
			{
				MeshCollider[] array3 = Resources.FindObjectsOfTypeAll<MeshCollider>();
				if (array3 != null)
				{
					foreach (MeshCollider meshCollider2 in array3)
					{
						if (!meshCollider2.enabled)
						{
							meshCollider2.enabled = true;
						}
					}
				}
				Movement.isnoclipped = false;
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000F7F4 File Offset: 0x0000D9F4
		public static void DisableNoClip()
		{
			MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
			if (array != null)
			{
				foreach (MeshCollider meshCollider in array)
				{
					if (!meshCollider.enabled)
					{
						meshCollider.enabled = true;
					}
				}
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000F830 File Offset: 0x0000DA30
		public static void LongArms()
		{
			if (InputHandler.LeftTrigger)
			{
				Movement.LongArmsOffset += 0.05f;
			}
			if (InputHandler.RightTrigger)
			{
				Movement.LongArmsOffset -= 0.05f;
			}
			if (InputHandler.LeftPrimary)
			{
				Player.Instance.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
				Player.Instance.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
				return;
			}
			Player.Instance.rightHandOffset = new Vector3(0f, Movement.LongArmsOffset, 0f);
			Player.Instance.leftHandOffset = new Vector3(0f, Movement.LongArmsOffset, 0f);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000F8EB File Offset: 0x0000DAEB
		public static void SwitchSpeed()
		{
			Movement.speedBoostMultiplier = Movement.multiplierManager(Movement.speedBoostMultiplier);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000F8FC File Offset: 0x0000DAFC
		public static void SwitchFlight()
		{
			Movement.flightMultiplier = Movement.multiplierManager(Movement.flightMultiplier);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000F90D File Offset: 0x0000DB0D
		public static void SwitchWallWalk()
		{
			Movement.WallWalkMultiplier = Movement.multiplierManager(Movement.WallWalkMultiplier);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000F920 File Offset: 0x0000DB20
		public static string getPlats()
		{
			string text = "";
			if (MenuPatch.currentPlatform == 0)
			{
				text = "Default";
			}
			else if (MenuPatch.currentPlatform == 1)
			{
				text = "Invisible";
			}
			else if (MenuPatch.currentPlatform == 2)
			{
				text = "Sticky";
			}
			return text;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000F964 File Offset: 0x0000DB64
		public static void PunchMod()
		{
			int num = -1;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig != GorillaTagger.Instance.offlineVRRig)
				{
					num++;
					Vector3 position = vrrig.rightHandTransform.position;
					Vector3 position2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
					if ((double)Vector3.Distance(position, position2) < 0.25)
					{
						Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.Normalize(vrrig.rightHandTransform.position - Movement.lastRight[num]) * 10f;
					}
					Movement.lastRight[num] = vrrig.rightHandTransform.position;
					if ((double)Vector3.Distance(vrrig.leftHandTransform.position, position2) < 0.25)
					{
						Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.Normalize(vrrig.rightHandTransform.position - Movement.lastLeft[num]) * 10f;
					}
					Movement.lastLeft[num] = vrrig.leftHandTransform.position;
				}
			}
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000FAEC File Offset: 0x0000DCEC
		public static void SpeedBoost(float speedMult, bool Enable)
		{
			if (!Enable)
			{
				Player.Instance.maxJumpSpeed = 6.5f;
				Player.Instance.jumpMultiplier = 1.1f;
				return;
			}
			Player.Instance.maxJumpSpeed = 6.5f * speedMult;
			Player.Instance.jumpMultiplier = 1.1f * speedMult;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000FB40 File Offset: 0x0000DD40
		public static void SendRopeRPC(Vector3 velocity)
		{
			if (Time.time > Movement.ropetimeout + 0.1f)
			{
				Movement.ropetimeout = Time.time;
				foreach (KeyValuePair<int, GorillaRopeSwing> keyValuePair in Traverse.Create(RopeSwingManager.instance).Field("ropes").GetValue<Dictionary<int, GorillaRopeSwing>>())
				{
					PhotonView photonView = RopeSwingManager.instance.photonView;
					string text = "SetVelocity";
					RpcTarget rpcTarget = 0;
					object[] array = new object[5];
					array[0] = keyValuePair.Key;
					array[1] = 1;
					array[2] = velocity;
					array[3] = true;
					photonView.RPC(text, rpcTarget, array);
				}
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000FC04 File Offset: 0x0000DE04
		public static void RopeFreeze()
		{
			if (Time.time > Movement.ropetimeout + 0.1f)
			{
				Movement.ropetimeout = Time.time;
				foreach (KeyValuePair<int, GorillaRopeSwing> keyValuePair in Traverse.Create(RopeSwingManager.instance).Field("ropes").GetValue<Dictionary<int, GorillaRopeSwing>>())
				{
					PhotonView photonView = RopeSwingManager.instance.photonView;
					string text = "SetVelocity";
					RpcTarget rpcTarget = 0;
					object[] array = new object[5];
					array[0] = keyValuePair.Key;
					array[1] = 1;
					array[2] = Vector3.zero;
					array[3] = true;
					photonView.RPC(text, rpcTarget, array);
				}
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000FCCC File Offset: 0x0000DECC
		public static void RopeUp()
		{
			Movement.SendRopeRPC(new Vector3(0f, 100f, 1f));
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000FCE7 File Offset: 0x0000DEE7
		public static void FlingOnRope()
		{
			Movement.RopeUp();
			Movement.RopeDown();
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000FCF3 File Offset: 0x0000DEF3
		public static void RopeDown()
		{
			Movement.SendRopeRPC(new Vector3(0f, -100f, 1f));
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000FD10 File Offset: 0x0000DF10
		public static void RopeToSelf()
		{
			if (Time.time > Movement.ropetimeout + 0.1f)
			{
				Movement.ropetimeout = Time.time;
				foreach (KeyValuePair<int, GorillaRopeSwing> keyValuePair in Traverse.Create(RopeSwingManager.instance).Field("ropes").GetValue<Dictionary<int, GorillaRopeSwing>>())
				{
					Vector3 vector = Player.Instance.transform.position - keyValuePair.Value.transform.position;
					float magnitude = vector.magnitude;
					float num = 9999f;
					float num2 = Mathf.Clamp01(num / magnitude);
					Vector3 vector2 = (keyValuePair.Value.transform.position + vector.normalized * magnitude * num2 - keyValuePair.Value.transform.position).normalized * num;
					PhotonView photonView = RopeSwingManager.instance.photonView;
					string text = "SetVelocity";
					RpcTarget rpcTarget = 0;
					object[] array = new object[5];
					array[0] = keyValuePair.Key;
					array[1] = 1;
					array[2] = vector2;
					array[3] = true;
					photonView.RPC(text, rpcTarget, array);
				}
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000FE6C File Offset: 0x0000E06C
		public static float getSpeedBoostMultiplier()
		{
			return Movement.speedBoostMultiplier;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000FE73 File Offset: 0x0000E073
		public static float getFlightMultiplier()
		{
			return Movement.flightMultiplier;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000FE7A File Offset: 0x0000E07A
		public static float getWallWalkMultiplier()
		{
			return Movement.WallWalkMultiplier;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000FE84 File Offset: 0x0000E084
		public static void RopeGun()
		{
			GunLib.GunLibData gunLibData = GunLib.Shoot();
			if (gunLibData != null && gunLibData.isShooting && gunLibData.isTriggered && Time.time > Movement.ropetimeout + 0.1f)
			{
				Movement.ropetimeout = Time.time;
				foreach (GorillaRopeSwing gorillaRopeSwing in Object.FindObjectsOfType<GorillaRopeSwing>())
				{
					Vector3 vector = gunLibData.hitPosition - gorillaRopeSwing.transform.position;
					float magnitude = vector.magnitude;
					float num = 9999f;
					float num2 = Mathf.Clamp01(num / magnitude);
					Vector3 vector2 = (gorillaRopeSwing.transform.position + vector.normalized * magnitude * num2 - gorillaRopeSwing.transform.position).normalized * num;
					PhotonView photonView = RopeSwingManager.instance.photonView;
					string text = "SetVelocity";
					RpcTarget rpcTarget = 0;
					object[] array2 = new object[5];
					array2[0] = gorillaRopeSwing.ropeId;
					array2[1] = 4;
					array2[2] = vector2;
					array2[3] = true;
					photonView.RPC(text, rpcTarget, array2);
				}
			}
		}

		// Token: 0x040000C1 RID: 193
		private static GameObject LeftPlat;

		// Token: 0x040000C2 RID: 194
		private static GameObject RightPlat;

		// Token: 0x040000C3 RID: 195
		private static GameObject pointer;

		// Token: 0x040000C4 RID: 196
		private static GameObject[] LeftPlat_Networked = new GameObject[9999];

		// Token: 0x040000C5 RID: 197
		private static GameObject[] RightPlat_Networked = new GameObject[9999];

		// Token: 0x040000C6 RID: 198
		public static Vector3 scale = new Vector3(0.0125f, 0.28f, 0.3825f);

		// Token: 0x040000C7 RID: 199
		private static bool gravityToggled;

		// Token: 0x040000C8 RID: 200
		private static bool flying;

		// Token: 0x040000C9 RID: 201
		private static string oldRoom;

		// Token: 0x040000CA RID: 202
		private static bool canTP = false;

		// Token: 0x040000CB RID: 203
		private static float RG;

		// Token: 0x040000CC RID: 204
		private static float LG;

		// Token: 0x040000CD RID: 205
		private static bool RGrabbing;

		// Token: 0x040000CE RID: 206
		private static Vector3 CurHandPos;

		// Token: 0x040000CF RID: 207
		private static bool LGrabbing;

		// Token: 0x040000D0 RID: 208
		private static bool AirMode;

		// Token: 0x040000D1 RID: 209
		private static int layers = int.MaxValue;

		// Token: 0x040000D2 RID: 210
		public static float Spring;

		// Token: 0x040000D3 RID: 211
		public static float Damper;

		// Token: 0x040000D4 RID: 212
		public static float MassScale;

		// Token: 0x040000D5 RID: 213
		public static ConfigEntry<float> sp;

		// Token: 0x040000D6 RID: 214
		public static ConfigEntry<float> dp;

		// Token: 0x040000D7 RID: 215
		public static Color grapplecolor;

		// Token: 0x040000D8 RID: 216
		public static ConfigEntry<float> ms;

		// Token: 0x040000D9 RID: 217
		public static ConfigEntry<Color> rc;

		// Token: 0x040000DA RID: 218
		public static bool wackstart = false;

		// Token: 0x040000DB RID: 219
		public static bool canleftgrapple = true;

		// Token: 0x040000DC RID: 220
		private static bool canBHop = false;

		// Token: 0x040000DD RID: 221
		private static bool isBHop = false;

		// Token: 0x040000DE RID: 222
		private static bool isnoclipped = false;

		// Token: 0x040000DF RID: 223
		private static float LongArmsOffset = 0f;

		// Token: 0x040000E0 RID: 224
		private static LineRenderer lineRenderer;

		// Token: 0x040000E1 RID: 225
		private static bool disablegrapple = false;

		// Token: 0x040000E2 RID: 226
		private static RaycastHit hit;

		// Token: 0x040000E3 RID: 227
		public static float speedBoostMultiplier = 1.15f;

		// Token: 0x040000E4 RID: 228
		public static float flightMultiplier = 1.15f;

		// Token: 0x040000E5 RID: 229
		public static float WallWalkMultiplier = 1.15f;

		// Token: 0x040000E6 RID: 230
		private static bool clipped = false;

		// Token: 0x040000E7 RID: 231
		public static List<Vector3> Macro = new List<Vector3>();

		// Token: 0x040000E8 RID: 232
		public static float MacroHelp = 0f;

		// Token: 0x040000E9 RID: 233
		private static Vector3 head_direction;

		// Token: 0x040000EA RID: 234
		private static Vector3 roll_direction;

		// Token: 0x040000EB RID: 235
		private static Vector2 left_joystick;

		// Token: 0x040000EC RID: 236
		private static bool Start = false;

		// Token: 0x040000ED RID: 237
		private static float multiplier = 1f;

		// Token: 0x040000EE RID: 238
		private static float speed = 0f;

		// Token: 0x040000EF RID: 239
		private static float maxs = 10f;

		// Token: 0x040000F0 RID: 240
		private static float acceleration = 5f;

		// Token: 0x040000F1 RID: 241
		private static float distance = 0.35f;

		// Token: 0x040000F2 RID: 242
		public static List<Vector3> positions = new List<Vector3>();

		// Token: 0x040000F3 RID: 243
		public static float RewindHelp = 0f;

		// Token: 0x040000F4 RID: 244
		internal static Vector3 previousMousePosition;

		// Token: 0x040000F5 RID: 245
		public static float slideControlMultiplier = 1.15f;

		// Token: 0x040000F6 RID: 246
		public static string[] platformTypes = new string[] { "Normal", "Invisible", "Sticky" };

		// Token: 0x040000F7 RID: 247
		public static bool cangrapple = true;

		// Token: 0x040000F8 RID: 248
		public static float maxDistance = 100f;

		// Token: 0x040000F9 RID: 249
		private static float myVarY1;

		// Token: 0x040000FA RID: 250
		private static LineRenderer lr;

		// Token: 0x040000FB RID: 251
		public static Vector3 grapplePoint;

		// Token: 0x040000FC RID: 252
		public static Vector3 leftgrapplePoint;

		// Token: 0x040000FD RID: 253
		private static float myVarY2;

		// Token: 0x040000FE RID: 254
		private static float gainSpeed;

		// Token: 0x040000FF RID: 255
		public static SpringJoint joint;

		// Token: 0x04000100 RID: 256
		public static Vector3? leftHandOffsetInitial = null;

		// Token: 0x04000101 RID: 257
		public static Vector3? rightHandOffsetInitial = null;

		// Token: 0x04000102 RID: 258
		public static SpringJoint leftjoint;

		// Token: 0x04000103 RID: 259
		public static LineRenderer leftlr;

		// Token: 0x04000104 RID: 260
		public static float? maxArmLengthInitial = null;

		// Token: 0x04000105 RID: 261
		private static int currentPlatform;

		// Token: 0x04000106 RID: 262
		private static float ropetimeout;

		// Token: 0x04000107 RID: 263
		public static Vector3[] lastLeft = new Vector3[]
		{
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero
		};

		// Token: 0x04000108 RID: 264
		public static Vector3[] lastRight = new Vector3[]
		{
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero
		};
	}
}
