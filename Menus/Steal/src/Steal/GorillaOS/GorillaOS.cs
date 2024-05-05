using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using GorillaNetworking;
using Steal.GorillaOS.Patchers;
using UnityEngine;

namespace Steal.GorillaOS
{
	// Token: 0x02000021 RID: 33
	public class GorillaOS : MonoBehaviour
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000077F7 File Offset: 0x000059F7
		public static GorillaOS instance
		{
			get
			{
				return GorillaOS._instance;
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000077FE File Offset: 0x000059FE
		private void Awake()
		{
			GorillaOS._instance = this;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00007808 File Offset: 0x00005A08
		public void UpadateTheme(int theme)
		{
			if (GorillaOS.boards == null)
			{
				GorillaOS.boards = Object.FindObjectsOfType<GorillaScoreBoard>();
			}
			GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/StaticUnlit/motdscreen").GetComponent<MeshRenderer>().enabled = true;
			GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/StaticUnlit/screen").GetComponent<MeshRenderer>().enabled = true;
			GorillaComputer.instance.computerScreenRenderer.enabled = true;
			GorillaComputer.instance.wallScreenRenderer.enabled = true;
			GorillaScoreBoard[] array = GorillaOS.boards;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			for (int j = 0; j < GorillaComputer.instance.levelScreens.Length; j++)
			{
				GorillaComputer.instance.levelScreens[j].GetComponent<MeshRenderer>().enabled = true;
			}
			Material material = new Material(Shader.Find("GorillaTag/UberShader"));
			switch (theme)
			{
			case 1:
				material.color = Color.gray * 0.4f;
				break;
			case 2:
				material.color = Color.magenta * 0.2f;
				break;
			case 3:
			{
				GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/StaticUnlit/motdscreen").GetComponent<MeshRenderer>().enabled = false;
				GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/StaticUnlit/screen").GetComponent<MeshRenderer>().enabled = false;
				GorillaComputer.instance.computerScreenRenderer.enabled = false;
				GorillaComputer.instance.wallScreenRenderer.enabled = false;
				array = GorillaOS.boards;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				for (int k = 0; k < GorillaComputer.instance.levelScreens.Length; k++)
				{
					GorillaComputer.instance.levelScreens[k].GetComponent<MeshRenderer>().enabled = false;
				}
				break;
			}
			}
			GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/StaticUnlit/motdscreen").GetComponent<MeshRenderer>().material = material;
			GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/StaticUnlit/screen").GetComponent<MeshRenderer>().material = material;
			GorillaComputer.instance.computerScreenRenderer.material = material;
			GorillaComputer.instance.wallScreenRenderer.material = material;
			array = GorillaOS.boards;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Renderer>().material = material;
			}
			for (int l = 0; l < GorillaComputer.instance.levelScreens.Length; l++)
			{
				GorillaComputer.instance.levelScreens[l].badMaterial = material;
				GorillaComputer.instance.levelScreens[l].goodMaterial = material;
				GorillaComputer.instance.levelScreens[l].UpdateText(GorillaComputer.instance.levelScreens[l].myText.text, true);
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00007AAB File Offset: 0x00005CAB
		public void Reload()
		{
			base.StartCoroutine(this.ReloadPage());
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00007ABA File Offset: 0x00005CBA
		public IEnumerator ReloadPage()
		{
			yield return new WaitForSeconds(1f);
			GorillaComputer.instance.UpdateScreen();
			yield break;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00007AC4 File Offset: 0x00005CC4
		public void Refresh()
		{
			GorillaOS.list = "";
			if (GorillaOS.Moduals == null)
			{
				GorillaOS.Moduals = Object.FindObjectsOfType<BaseUnityPlugin>().ToList<BaseUnityPlugin>();
			}
			foreach (BaseUnityPlugin baseUnityPlugin in GorillaOS.Moduals)
			{
				if (baseUnityPlugin == GorillaOS.Moduals.ToArray()[SupportPatch.focusedModual - 1])
				{
					GorillaOS.list += (baseUnityPlugin.enabled ? ("> <color=green>[+]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n") : ("> <color=red>[-]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n"));
				}
				else
				{
					GorillaOS.list += (baseUnityPlugin.enabled ? ("<color=green>[+]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n") : ("<color=red>[-]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n"));
				}
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00007C00 File Offset: 0x00005E00
		private void Update()
		{
			if (Time.time > this.refresh + 5f && GameObject.Find("BepInEx_Manager"))
			{
				GorillaOS.list = "";
				GorillaOS.Moduals = GameObject.Find("BepInEx_Manager").GetComponents<BaseUnityPlugin>().ToList<BaseUnityPlugin>();
				foreach (BaseUnityPlugin baseUnityPlugin in GorillaOS.Moduals)
				{
					if (baseUnityPlugin == GorillaOS.Moduals.ToArray()[SupportPatch.focusedModual - 1])
					{
						GorillaOS.list += (baseUnityPlugin.enabled ? ("> <color=green>[+]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n") : ("> <color=red>[-]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n"));
					}
					else
					{
						GorillaOS.list += (baseUnityPlugin.enabled ? ("<color=green>[+]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n") : ("<color=red>[-]</color> : " + baseUnityPlugin.GetType().Name.ToUpper() + "\n"));
					}
				}
			}
		}

		// Token: 0x0400004B RID: 75
		private static GorillaOS _instance;

		// Token: 0x0400004C RID: 76
		private float refresh;

		// Token: 0x0400004D RID: 77
		public static List<BaseUnityPlugin> Moduals = new List<BaseUnityPlugin>();

		// Token: 0x0400004E RID: 78
		public static string list;

		// Token: 0x0400004F RID: 79
		public static GorillaScoreBoard[] boards;
	}
}
