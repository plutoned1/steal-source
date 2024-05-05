using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using Steal.Components;
using Steal.Patchers.VRRigPatchers;
using UnityEngine;

namespace Steal.Background.Mods
{
	// Token: 0x02000044 RID: 68
	internal class Visual : Mod
	{
		// Token: 0x060001C6 RID: 454 RVA: 0x0001663C File Offset: 0x0001483C
		public static void ResetTexure()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>())
				{
					Object.Destroy(vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>());
				}
				for (int i = 0; i < Visual.bones.Count<int>(); i += 2)
				{
					if (vrrig.mainSkin.bones[Visual.bones[i]].gameObject.GetComponent<LineRenderer>())
					{
						Object.Destroy(vrrig.mainSkin.bones[Visual.bones[i]].gameObject.GetComponent<LineRenderer>());
					}
				}
			}
			foreach (VRRig vrrig2 in (VRRig[])Object.FindObjectsOfType(typeof(VRRig)))
			{
				if (!vrrig2.isOfflineVRRig && !vrrig2.isMyPlayer)
				{
					vrrig2.ChangeMaterialLocal(vrrig2.currentMatIndex);
				}
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00016774 File Offset: 0x00014974
		public static void FirstPerson()
		{
			GameObject gameObject = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
			gameObject.active = !gameObject.active;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00016790 File Offset: 0x00014990
		public static void Beacons()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
				{
					GameObject gameObject = GameObject.CreatePrimitive(2);
					Object.Destroy(gameObject.GetComponent<BoxCollider>());
					Object.Destroy(gameObject.GetComponent<Rigidbody>());
					Object.Destroy(gameObject.GetComponent<Collider>());
					gameObject.transform.rotation = Quaternion.identity;
					gameObject.transform.localScale = new Vector3(0.04f, 200f, 0.04f);
					gameObject.transform.position = vrrig.transform.position;
					gameObject.GetComponent<MeshRenderer>().material = vrrig.mainSkin.material;
					Object.Destroy(gameObject, Time.deltaTime);
				}
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0001688C File Offset: 0x00014A8C
		public static void BoneESP()
		{
			Material material = new Material(Shader.Find("GUI/Text Shader"));
			material.color = new Color(1f, 1f, 1f);
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!vrrig.isOfflineVRRig)
				{
					if (vrrig.mainSkin.material.name.Contains("fected"))
					{
						material = new Material(Shader.Find("GUI/Text Shader"));
						material.color = new Color(1f, 0f, 0f);
					}
					else
					{
						material = new Material(Shader.Find("GUI/Text Shader"));
						material.color = Color.green;
					}
					LineRenderer lineRenderer = vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>();
					if (!lineRenderer)
					{
						lineRenderer = vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
					}
					lineRenderer.endWidth = 0.03f;
					lineRenderer.startWidth = 0.03f;
					lineRenderer.material = material;
					lineRenderer.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
					lineRenderer.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));
					for (int i = 0; i < Visual.bones.Count<int>(); i += 2)
					{
						LineRenderer lineRenderer2 = vrrig.mainSkin.bones[Visual.bones[i]].gameObject.GetComponent<LineRenderer>();
						if (!lineRenderer2)
						{
							lineRenderer2 = vrrig.mainSkin.bones[Visual.bones[i]].gameObject.AddComponent<LineRenderer>();
							lineRenderer2.endWidth = 0.03f;
							lineRenderer2.startWidth = 0.03f;
						}
						lineRenderer2.material = material;
						lineRenderer2.SetPosition(0, vrrig.mainSkin.bones[Visual.bones[i]].position);
						lineRenderer2.SetPosition(1, vrrig.mainSkin.bones[Visual.bones[i + 1]].position);
					}
				}
			}
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00016B10 File Offset: 0x00014D10
		public static void Tracers()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig != null && !vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
				{
					LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
					lineRenderer.startColor = Color.green;
					lineRenderer.endColor = Color.green;
					lineRenderer.startWidth = 0.01f;
					lineRenderer.endWidth = 0.01f;
					lineRenderer.positionCount = 2;
					lineRenderer.useWorldSpace = true;
					lineRenderer.SetPosition(0, Player.Instance.rightControllerTransform.position);
					lineRenderer.SetPosition(1, vrrig.transform.position);
					lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
					Object.Destroy(lineRenderer, Time.deltaTime);
				}
			}
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00016C1C File Offset: 0x00014E1C
		public static void BoxESP()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig != null && !vrrig.isOfflineVRRig)
				{
					GameObject gameObject = new GameObject("box");
					gameObject.transform.position = vrrig.transform.position;
					GameObject gameObject2 = GameObject.CreatePrimitive(3);
					GameObject gameObject3 = GameObject.CreatePrimitive(3);
					GameObject gameObject4 = GameObject.CreatePrimitive(3);
					GameObject gameObject5 = GameObject.CreatePrimitive(3);
					Object.Destroy(gameObject2.GetComponent<BoxCollider>());
					Object.Destroy(gameObject5.GetComponent<BoxCollider>());
					Object.Destroy(gameObject4.GetComponent<BoxCollider>());
					Object.Destroy(gameObject3.GetComponent<BoxCollider>());
					gameObject2.transform.SetParent(gameObject.transform);
					gameObject2.transform.localPosition = new Vector3(0f, 0.49f, 0f);
					gameObject2.transform.localScale = new Vector3(1f, 0.02f, 0.02f);
					gameObject5.transform.SetParent(gameObject.transform);
					gameObject5.transform.localPosition = new Vector3(0f, -0.49f, 0f);
					gameObject5.transform.localScale = new Vector3(1f, 0.02f, 0.02f);
					gameObject4.transform.SetParent(gameObject.transform);
					gameObject4.transform.localPosition = new Vector3(-0.49f, 0f, 0f);
					gameObject4.transform.localScale = new Vector3(0.02f, 1f, 0.02f);
					gameObject3.transform.SetParent(gameObject.transform);
					gameObject3.transform.localPosition = new Vector3(0.49f, 0f, 0f);
					gameObject3.transform.localScale = new Vector3(0.02f, 1f, 0.02f);
					gameObject2.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
					gameObject5.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
					gameObject4.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
					gameObject3.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
					Color color;
					if (vrrig.mainSkin.material.name.Contains("fected"))
					{
						color = Color.red;
					}
					else
					{
						color = Color.green;
					}
					gameObject2.GetComponent<Renderer>().material.color = color;
					gameObject5.GetComponent<Renderer>().material.color = color;
					gameObject4.GetComponent<Renderer>().material.color = color;
					gameObject3.GetComponent<Renderer>().material.color = color;
					gameObject.transform.LookAt(gameObject.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
					Object.Destroy(gameObject, Time.deltaTime);
				}
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00016F7C File Offset: 0x0001517C
		public static void Chams()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer && vrrig.mainSkin.material.name.Contains("fected"))
				{
					vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
					vrrig.mainSkin.material.color = new Color32(byte.MaxValue, 0, 0, 90);
				}
				else if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
				{
					vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
					vrrig.mainSkin.material.color = new Color32(0, byte.MaxValue, 0, 90);
				}
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0001708C File Offset: 0x0001528C
		public static void ESP()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig != null && !vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
				{
					GameObject gameObject = GameObject.CreatePrimitive(3);
					Object.Destroy(gameObject.GetComponent<BoxCollider>());
					Object.Destroy(gameObject.GetComponent<Rigidbody>());
					gameObject.transform.rotation = vrrig.transform.rotation;
					gameObject.transform.localScale = new Vector3(0.4f, 0.86f, 0.4f);
					gameObject.transform.position = vrrig.transform.position;
					gameObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("GUI/Text Shader"));
					if (vrrig.mainSkin.material.name.Contains("fected"))
					{
						gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
					}
					else
					{
						gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
					}
					Object.Destroy(gameObject, Time.deltaTime);
				}
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x000171E8 File Offset: 0x000153E8
		public static void StartNameTags()
		{
			if (!Steal.Patchers.VRRigPatchers.OnEnable.nameTags)
			{
				Steal.Patchers.VRRigPatchers.OnEnable.nameTags = true;
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.GetComponent<NameTags>() == null)
					{
						vrrig.gameObject.AddComponent<NameTags>();
					}
				}
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00017264 File Offset: 0x00015464
		public static void DisablePost()
		{
			GameObject gameObject = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Terrain/SoundPostForest");
			gameObject.SetActive(!gameObject.activeSelf);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00017280 File Offset: 0x00015480
		public static void RestoreOriginalMaterials()
		{
			foreach (KeyValuePair<Renderer, Material> keyValuePair in Visual.originalMaterials)
			{
				try
				{
					if (keyValuePair.Key != null)
					{
						keyValuePair.Key.material = keyValuePair.Value;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Restore error " + ex.StackTrace + " - " + ex.Message);
				}
			}
			Visual.originalMaterials.Clear();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00017328 File Offset: 0x00015528
		public static void FPSBoost()
		{
			Shader shader = Shader.Find("GorillaTag/UberShader");
			if (shader == null)
			{
				Debug.LogError("GorillaTag/UberShader not found.");
				return;
			}
			Renderer[] array = Resources.FindObjectsOfTypeAll<Renderer>();
			Material material = new Material(shader);
			foreach (Renderer renderer in array)
			{
				try
				{
					if (renderer.material.shader == shader)
					{
						if (!Visual.originalMaterials.ContainsKey(renderer))
						{
							Visual.originalMaterials[renderer] = renderer.material;
						}
						Material material2 = new Material(material)
						{
							color = Color.grey
						};
						renderer.material = material2;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("mat error " + ex.StackTrace + " - " + ex.Message);
				}
			}
			Object.Destroy(material);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0001740C File Offset: 0x0001560C
		public static void HorrorGame()
		{
			Shader shader = Shader.Find("GorillaTag/UberShader");
			if (shader == null)
			{
				Debug.LogError("GorillaTag/UberShader not found.");
				return;
			}
			Renderer[] array = Resources.FindObjectsOfTypeAll<Renderer>();
			Material material = new Material(shader);
			foreach (Renderer renderer in array)
			{
				try
				{
					if (renderer.material.shader == shader)
					{
						if (!Visual.originalMaterials.ContainsKey(renderer))
						{
							Visual.originalMaterials[renderer] = renderer.material;
						}
						Material material2 = new Material(material)
						{
							color = Color.black
						};
						renderer.material = material2;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("mat error " + ex.StackTrace + " - " + ex.Message);
				}
			}
			Object.Destroy(material);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x000174F0 File Offset: 0x000156F0
		public static void agreeTOS()
		{
			GameObject.Find("Miscellaneous Scripts/LegalAgreementCheck/Legal Agreements").GetComponent<LegalAgreements>().testFaceButtonPress = true;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00017508 File Offset: 0x00015708
		public static void HideInTrees(bool enable)
		{
			foreach (GameObject gameObject in Visual.GetAllGameObjects(GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Terrain/SmallTrees")))
			{
				MeshCollider component = gameObject.GetComponent<MeshCollider>();
				if (component != null)
				{
					if (enable)
					{
						component.enabled = false;
					}
					else
					{
						component.enabled = true;
					}
				}
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00017580 File Offset: 0x00015780
		public static List<GameObject> GetAllGameObjects(GameObject parent)
		{
			List<GameObject> list = new List<GameObject>();
			Visual.CollectGameObjectsRecursive(parent, list);
			return list;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0001759C File Offset: 0x0001579C
		private static void CollectGameObjectsRecursive(GameObject parent, List<GameObject> gameObjects)
		{
			gameObjects.Add(parent);
			for (int i = 0; i < parent.transform.childCount; i++)
			{
				Visual.CollectGameObjectsRecursive(parent.transform.GetChild(i).gameObject, gameObjects);
			}
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x000175E0 File Offset: 0x000157E0
		public static void StopNameTags()
		{
			if (Steal.Patchers.VRRigPatchers.OnEnable.nameTags)
			{
				Steal.Patchers.VRRigPatchers.OnEnable.nameTags = false;
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.GetComponent<NameTags>() != null)
					{
						Object.Destroy(vrrig.gameObject.GetComponent<NameTags>());
					}
				}
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00017660 File Offset: 0x00015860
		public static void OldGraphics()
		{
			if (!Visual.oldGraphiks)
			{
				foreach (Renderer renderer in Resources.FindObjectsOfTypeAll<Renderer>())
				{
					if (renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
					{
						string name = renderer.sharedMaterial.mainTexture.name;
						string name2 = renderer.sharedMaterial.name;
						renderer.sharedMaterial.mainTexture.filterMode = 1;
						renderer.sharedMaterial.mainTexture.name = name;
						renderer.sharedMaterial.name = name2;
					}
				}
				Visual.oldGraphiks = true;
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00017704 File Offset: 0x00015904
		public static void RevertGraphics()
		{
			foreach (Renderer renderer in Resources.FindObjectsOfTypeAll<Renderer>())
			{
				if (renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
				{
					renderer.sharedMaterial.mainTexture.filterMode = 0;
				}
			}
			Visual.oldGraphiks = false;
		}

		// Token: 0x04000138 RID: 312
		public static int[] bones = new int[]
		{
			4, 3, 5, 4, 19, 18, 20, 19, 3, 18,
			21, 20, 22, 21, 25, 21, 29, 21, 31, 29,
			27, 25, 24, 22, 6, 5, 7, 6, 10, 6,
			14, 6, 16, 14, 12, 10, 9, 7
		};

		// Token: 0x04000139 RID: 313
		private static Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();

		// Token: 0x0400013A RID: 314
		public static bool oldGraphiks = false;
	}
}
