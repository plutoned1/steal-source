using System;
using BepInEx;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

namespace Steal.Background
{
	// Token: 0x02000030 RID: 48
	internal class GunLib
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x00009DA8 File Offset: 0x00007FA8
		public static void GunCleanUp()
		{
			if (GunLib.pointer == null || GunLib.lr == null)
			{
				return;
			}
			Object.Destroy(GunLib.pointer);
			GunLib.pointer = null;
			Object.Destroy(GunLib.lr.gameObject);
			GunLib.lr = null;
			GunLib.data = new GunLib.GunLibData(false, false, false, null, default(Vector3));
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00009E0C File Offset: 0x0000800C
		public static GunLib.GunLibData ShootLock()
		{
			GunLib.GunLibData gunLibData;
			try
			{
				if (XRSettings.isDeviceActive)
				{
					Transform transform;
					if (!MenuPatch.FindButton("Right Hand Menu").Enabled)
					{
						transform = Player.Instance.rightControllerTransform;
						GunLib.data.isShooting = InputHandler.RightGrip;
						GunLib.data.isTriggered = InputHandler.RightTrigger;
					}
					else
					{
						transform = Player.Instance.leftControllerTransform;
						GunLib.data.isShooting = InputHandler.LeftGrip;
						GunLib.data.isTriggered = InputHandler.LeftTrigger;
					}
					if (GunLib.data.isShooting)
					{
						Renderer renderer = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
						if (GunLib.data.lockedPlayer == null && !GunLib.data.isLocked)
						{
							RaycastHit raycastHit;
							if (Physics.Raycast(transform.position - transform.up, -transform.up, ref raycastHit) && GunLib.pointer == null)
							{
								GunLib.pointer = GameObject.CreatePrimitive(0);
								Object.Destroy(GunLib.pointer.GetComponent<Rigidbody>());
								Object.Destroy(GunLib.pointer.GetComponent<SphereCollider>());
								GunLib.pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
								renderer = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
								renderer.material.color = Color.red;
								renderer.material.shader = Shader.Find("GUI/Text Shader");
							}
							if (GunLib.lr == null)
							{
								GunLib.lr = new GameObject("line").AddComponent<LineRenderer>();
								GunLib.lr.endWidth = 0.01f;
								GunLib.lr.startWidth = 0.01f;
								GunLib.lr.material.shader = Shader.Find("GUI/Text Shader");
							}
							GunLib.lr.SetPosition(0, transform.position);
							GunLib.lr.SetPosition(1, raycastHit.point);
							GunLib.data.hitPosition = raycastHit.point;
							GunLib.pointer.transform.position = raycastHit.point;
							VRRig componentInParent = raycastHit.collider.GetComponentInParent<VRRig>();
							if (componentInParent != null)
							{
								if (GunLib.data.isTriggered)
								{
									GunLib.data.lockedPlayer = componentInParent;
									GunLib.data.isLocked = true;
									GunLib.lr.startColor = Color.blue;
									GunLib.lr.endColor = Color.blue;
									renderer.material.color = Color.blue;
								}
								else
								{
									GunLib.data.isLocked = false;
									GunLib.lr.startColor = Color.green;
									GunLib.lr.endColor = Color.green;
									renderer.material.color = Color.green;
									GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
								}
							}
							else
							{
								GunLib.data.isLocked = false;
								GunLib.lr.startColor = Color.red;
								GunLib.lr.endColor = Color.red;
								renderer.material.color = Color.red;
							}
						}
						if (GunLib.data.isTriggered && GunLib.data.lockedPlayer != null)
						{
							GunLib.data.isLocked = true;
							GunLib.lr.SetPosition(0, transform.position);
							GunLib.lr.SetPosition(1, GunLib.data.lockedPlayer.transform.position);
							GunLib.data.hitPosition = GunLib.data.lockedPlayer.transform.position;
							GunLib.pointer.transform.position = GunLib.data.lockedPlayer.transform.position;
							GunLib.lr.startColor = Color.blue;
							GunLib.lr.endColor = Color.blue;
							renderer.material.color = Color.blue;
						}
						else if (GunLib.data.lockedPlayer != null)
						{
							GunLib.data.isLocked = false;
							GunLib.data.lockedPlayer = null;
							GunLib.lr.startColor = Color.red;
							GunLib.lr.endColor = Color.red;
							renderer.material.color = Color.red;
						}
					}
					else
					{
						GunLib.GunCleanUp();
					}
					gunLibData = GunLib.data;
				}
				else
				{
					GunLib.data.isShooting = UnityInput.Current.GetMouseButton(1);
					GunLib.data.isTriggered = UnityInput.Current.GetMouseButton(0);
					if (GunLib.data.isShooting)
					{
						Renderer renderer2 = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
						if (GunLib.data.lockedPlayer == null && !GunLib.data.isLocked)
						{
							Ray ray = ((GameObject.Find("Shoulder Camera").GetComponent<Camera>() != null) ? GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition) : GorillaTagger.Instance.mainCamera.GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition));
							RaycastHit raycastHit;
							if (Physics.Raycast(ray.origin, ray.direction, ref raycastHit) && GunLib.pointer == null)
							{
								GunLib.pointer = GameObject.CreatePrimitive(0);
								Object.Destroy(GunLib.pointer.GetComponent<Rigidbody>());
								Object.Destroy(GunLib.pointer.GetComponent<SphereCollider>());
								GunLib.pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
								renderer2 = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
								renderer2.material.color = Color.red;
								renderer2.material.shader = Shader.Find("GUI/Text Shader");
							}
							if (GunLib.lr == null)
							{
								GunLib.lr = new GameObject("line").AddComponent<LineRenderer>();
								GunLib.lr.endWidth = 0.01f;
								GunLib.lr.startWidth = 0.01f;
								GunLib.lr.material.shader = Shader.Find("GUI/Text Shader");
							}
							GunLib.lr.SetPosition(0, Player.Instance.headCollider.transform.position);
							GunLib.lr.SetPosition(1, raycastHit.point);
							GunLib.data.hitPosition = raycastHit.point;
							GunLib.pointer.transform.position = raycastHit.point;
							VRRig componentInParent2 = raycastHit.collider.GetComponentInParent<VRRig>();
							if (componentInParent2 != null && GunLib.data.lockedPlayer == null)
							{
								if (GunLib.data.isTriggered)
								{
									GunLib.data.lockedPlayer = componentInParent2;
									GunLib.data.isLocked = true;
								}
								else
								{
									GunLib.data.isLocked = false;
									GunLib.lr.startColor = Color.green;
									GunLib.lr.endColor = Color.green;
									renderer2.material.color = Color.green;
								}
							}
							else
							{
								GunLib.data.isLocked = false;
								GunLib.lr.startColor = Color.red;
								GunLib.lr.endColor = Color.red;
								renderer2.material.color = Color.red;
							}
						}
						if (renderer2 != null)
						{
							if (GunLib.data.isTriggered && GunLib.data.lockedPlayer != null)
							{
								GunLib.lr.SetPosition(0, Player.Instance.rightControllerTransform.position);
								GunLib.lr.SetPosition(1, GunLib.data.lockedPlayer.transform.position);
								GunLib.data.hitPosition = GunLib.data.lockedPlayer.transform.position;
								GunLib.pointer.transform.position = GunLib.data.lockedPlayer.transform.position;
								GunLib.data.isLocked = true;
								GunLib.lr.startColor = Color.blue;
								GunLib.lr.endColor = Color.blue;
								renderer2.material.color = Color.blue;
							}
							else if (GunLib.data.lockedPlayer != null)
							{
								GunLib.data.isLocked = false;
								GunLib.data.lockedPlayer = null;
								GunLib.lr.startColor = Color.red;
								GunLib.lr.endColor = Color.red;
								renderer2.material.color = Color.red;
							}
						}
					}
					else
					{
						GunLib.GunCleanUp();
					}
					gunLibData = GunLib.data;
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
				gunLibData = null;
			}
			return gunLibData;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000A6FC File Offset: 0x000088FC
		public static GunLib.GunLibData Shoot()
		{
			GunLib.GunLibData gunLibData;
			try
			{
				if (XRSettings.isDeviceActive)
				{
					Transform transform;
					if (!MenuPatch.FindButton("Right Hand Menu").Enabled)
					{
						transform = Player.Instance.rightControllerTransform;
						GunLib.data.isShooting = InputHandler.RightGrip;
						GunLib.data.isTriggered = InputHandler.RightTrigger;
					}
					else
					{
						transform = Player.Instance.leftControllerTransform;
						GunLib.data.isShooting = InputHandler.LeftGrip;
						GunLib.data.isTriggered = InputHandler.LeftGrip;
					}
					if (GunLib.data.isShooting)
					{
						Renderer renderer = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
						RaycastHit raycastHit;
						if (Physics.Raycast(transform.position - transform.up, -transform.up, ref raycastHit) && GunLib.pointer == null)
						{
							GunLib.pointer = GameObject.CreatePrimitive(0);
							Object.Destroy(GunLib.pointer.GetComponent<Rigidbody>());
							Object.Destroy(GunLib.pointer.GetComponent<SphereCollider>());
							GunLib.pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
							renderer = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
							renderer.material.color = Color.red;
							renderer.material.shader = Shader.Find("GUI/Text Shader");
						}
						if (GunLib.lr == null)
						{
							GunLib.lr = new GameObject("line").AddComponent<LineRenderer>();
							GunLib.lr.endWidth = 0.01f;
							GunLib.lr.startWidth = 0.01f;
							GunLib.lr.material.shader = Shader.Find("GUI/Text Shader");
						}
						GunLib.lr.SetPosition(0, transform.position);
						GunLib.lr.SetPosition(1, raycastHit.point);
						GunLib.data.hitPosition = raycastHit.point;
						GunLib.pointer.transform.position = raycastHit.point;
						VRRig componentInParent = raycastHit.collider.GetComponentInParent<VRRig>();
						if (componentInParent != null)
						{
							if (GunLib.data.isTriggered)
							{
								GunLib.data.lockedPlayer = componentInParent;
								GunLib.data.isLocked = true;
								renderer.material.color = Color.blue;
								GunLib.lr.startColor = Color.blue;
								GunLib.lr.endColor = Color.blue;
							}
							else
							{
								GunLib.lr.startColor = Color.green;
								GunLib.lr.endColor = Color.green;
								renderer.material.color = Color.green;
								GunLib.data.isLocked = false;
								GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 3f, GorillaTagger.Instance.tagHapticDuration / 2f);
							}
						}
						else
						{
							GunLib.lr.startColor = Color.red;
							GunLib.lr.endColor = Color.red;
							renderer.material.color = Color.red;
							GunLib.data.isLocked = false;
						}
					}
					else
					{
						GunLib.GunCleanUp();
					}
					gunLibData = GunLib.data;
				}
				else
				{
					GunLib.data.isShooting = true;
					GunLib.data.isTriggered = UnityInput.Current.GetMouseButton(0);
					Renderer renderer2 = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
					Ray ray = ((GameObject.Find("Shoulder Camera").GetComponent<Camera>() != null) ? GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition) : GorillaTagger.Instance.mainCamera.GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition));
					RaycastHit raycastHit2;
					if (Physics.Raycast(ray.origin, ray.direction, ref raycastHit2) && GunLib.pointer == null)
					{
						GunLib.pointer = GameObject.CreatePrimitive(0);
						Object.Destroy(GunLib.pointer.GetComponent<Rigidbody>());
						Object.Destroy(GunLib.pointer.GetComponent<SphereCollider>());
						GunLib.pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
						renderer2 = ((GunLib.pointer != null) ? GunLib.pointer.GetComponent<Renderer>() : null);
						renderer2.material.color = Color.red;
						renderer2.material.shader = Shader.Find("GUI/Text Shader");
					}
					if (GunLib.lr == null)
					{
						GunLib.lr = new GameObject("line").AddComponent<LineRenderer>();
						GunLib.lr.endWidth = 0.01f;
						GunLib.lr.startWidth = 0.01f;
						GunLib.lr.material.shader = Shader.Find("GUI/Text Shader");
					}
					GunLib.lr.SetPosition(0, Player.Instance.headCollider.transform.position);
					GunLib.lr.SetPosition(1, raycastHit2.point);
					GunLib.data.hitPosition = raycastHit2.point;
					GunLib.pointer.transform.position = raycastHit2.point;
					if (raycastHit2.collider.GetComponentInParent<VRRig>() != null)
					{
						if (GunLib.data.isTriggered)
						{
							GunLib.data.isLocked = true;
							GunLib.lr.startColor = Color.blue;
							GunLib.lr.endColor = Color.blue;
							renderer2.material.color = Color.blue;
						}
						else
						{
							GunLib.data.isLocked = false;
							GunLib.lr.startColor = Color.green;
							GunLib.lr.endColor = Color.green;
							renderer2.material.color = Color.green;
						}
					}
					else
					{
						GunLib.data.isLocked = false;
						GunLib.lr.startColor = Color.red;
						GunLib.lr.endColor = Color.red;
						renderer2.material.color = Color.red;
					}
					gunLibData = GunLib.data;
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
				gunLibData = null;
			}
			return gunLibData;
		}

		// Token: 0x04000075 RID: 117
		private static GameObject pointer;

		// Token: 0x04000076 RID: 118
		private static LineRenderer lr;

		// Token: 0x04000077 RID: 119
		private static GunLib.GunLibData data = new GunLib.GunLibData(false, false, false, null, default(Vector3));

		// Token: 0x02000056 RID: 86
		public class GunLibData
		{
			// Token: 0x17000020 RID: 32
			// (get) Token: 0x060002CF RID: 719 RVA: 0x0001909C File Offset: 0x0001729C
			// (set) Token: 0x060002D0 RID: 720 RVA: 0x000190A4 File Offset: 0x000172A4
			public VRRig lockedPlayer { get; set; }

			// Token: 0x17000021 RID: 33
			// (get) Token: 0x060002D1 RID: 721 RVA: 0x000190AD File Offset: 0x000172AD
			// (set) Token: 0x060002D2 RID: 722 RVA: 0x000190B5 File Offset: 0x000172B5
			public bool isShooting { get; set; }

			// Token: 0x17000022 RID: 34
			// (get) Token: 0x060002D3 RID: 723 RVA: 0x000190BE File Offset: 0x000172BE
			// (set) Token: 0x060002D4 RID: 724 RVA: 0x000190C6 File Offset: 0x000172C6
			public bool isLocked { get; set; }

			// Token: 0x17000023 RID: 35
			// (get) Token: 0x060002D5 RID: 725 RVA: 0x000190CF File Offset: 0x000172CF
			// (set) Token: 0x060002D6 RID: 726 RVA: 0x000190D7 File Offset: 0x000172D7
			public Vector3 hitPosition { get; set; }

			// Token: 0x17000024 RID: 36
			// (get) Token: 0x060002D7 RID: 727 RVA: 0x000190E0 File Offset: 0x000172E0
			// (set) Token: 0x060002D8 RID: 728 RVA: 0x000190E8 File Offset: 0x000172E8
			public bool isTriggered { get; set; }

			// Token: 0x060002D9 RID: 729 RVA: 0x000190F1 File Offset: 0x000172F1
			public GunLibData(bool stateTriggered, bool triggy, bool foundPlayer, VRRig player = null, Vector3 hitpos = default(Vector3))
			{
				this.lockedPlayer = player;
				this.isShooting = stateTriggered;
				this.isLocked = foundPlayer;
				this.hitPosition = hitpos;
				this.isTriggered = triggy;
			}
		}
	}
}
