using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Steal.Components
{
	// Token: 0x02000029 RID: 41
	public class ModsListInterface : MonoBehaviour
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00008A74 File Offset: 0x00006C74
		public void OnDisable()
		{
			ModsListInterface.Testtext.text = "";
			ModsListInterface.Testtext.gameObject.SetActive(false);
			ModsListInterface.HUDObj2ROOM = null;
			Object.Destroy(ModsListInterface.HUDObj2ROOM);
			ModsListInterface.HUDObjROOM = null;
			Object.Destroy(ModsListInterface.HUDObjROOM);
			ModsListInterface.HUDObj = null;
			Object.Destroy(ModsListInterface.HUDObj);
			ModsListInterface.HUDObj2 = null;
			Object.Destroy(ModsListInterface.HUDObj2);
			this.loaded1 = false;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00008AE8 File Offset: 0x00006CE8
		public void LateUpdate()
		{
			if (!XRSettings.isDeviceActive)
			{
				base.enabled = false;
				return;
			}
			ModsListInterface.modsEnabled = ModsList.modsEnabled;
			if (!this.loaded1)
			{
				ModsListInterface.MainCamera = GameObject.Find("Main Camera");
				ModsListInterface.HUDObjROOM = new GameObject();
				ModsListInterface.HUDObj2ROOM = new GameObject();
				ModsListInterface.HUDObj2ROOM.name = "CLIENT_HUB_OVERLAYROOM";
				ModsListInterface.HUDObjROOM.name = "CLIENT_HUB_OVERLAYROOM";
				ModsListInterface.HUDObjROOM.AddComponent<Canvas>();
				ModsListInterface.HUDObjROOM.AddComponent<CanvasScaler>();
				ModsListInterface.HUDObjROOM.AddComponent<GraphicRaycaster>();
				ModsListInterface.HUDObjROOM.GetComponent<Canvas>().enabled = true;
				ModsListInterface.HUDObjROOM.GetComponent<Canvas>().renderMode = 2;
				ModsListInterface.HUDObjROOM.GetComponent<Canvas>().worldCamera = ModsListInterface.MainCamera.GetComponent<Camera>();
				ModsListInterface.HUDObjROOM.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 5f);
				ModsListInterface.HUDObjROOM.GetComponent<RectTransform>().position = new Vector3(ModsListInterface.MainCamera.transform.position.x, ModsListInterface.MainCamera.transform.position.y, ModsListInterface.MainCamera.transform.position.z);
				ModsListInterface.HUDObj2ROOM.transform.position = new Vector3(ModsListInterface.MainCamera.transform.position.x, ModsListInterface.MainCamera.transform.position.y, ModsListInterface.MainCamera.transform.position.z - 4.6f);
				ModsListInterface.HUDObjROOM.transform.parent = ModsListInterface.HUDObj2ROOM.transform;
				ModsListInterface.HUDObjROOM.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
				Vector3 eulerAngles = ModsListInterface.HUDObjROOM.GetComponent<RectTransform>().rotation.eulerAngles;
				eulerAngles.y = -270f;
				ModsListInterface.HUDObjROOM.transform.localScale = new Vector3(1f, 1f, 1f);
				ModsListInterface.HUDObjROOM.GetComponent<RectTransform>().rotation = Quaternion.Euler(eulerAngles);
				ModsListInterface.TesttextROOM = new GameObject
				{
					transform = 
					{
						parent = ModsListInterface.HUDObjROOM.transform
					}
				}.AddComponent<Text>();
				ModsListInterface.TesttextROOM.text = "";
				ModsListInterface.TesttextROOM.fontSize = 10;
				ModsListInterface.TesttextROOM.font = GameObject.Find("COC Text").GetComponent<Text>().font;
				ModsListInterface.TesttextROOM.rectTransform.sizeDelta = new Vector2(260f, 300f);
				ModsListInterface.TesttextROOM.rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.7f);
				ModsListInterface.TesttextROOM.rectTransform.localPosition = new Vector3(-2.4f, -2f, 2f);
				ModsListInterface.TesttextROOM.material = ModsListInterface.AlertText;
				ModsListInterface.NotifiTextROOM = ModsListInterface.TesttextROOM;
				ModsListInterface.TesttextROOM.alignment = 0;
				ModsListInterface.HUDObj = new GameObject();
				ModsListInterface.HUDObj2 = new GameObject();
				ModsListInterface.HUDObj2.name = "CLIENT_HUB_OVERLAY";
				ModsListInterface.HUDObj.name = "CLIENT_HUB_OVERLAY";
				ModsListInterface.HUDObj.AddComponent<Canvas>();
				ModsListInterface.HUDObj.AddComponent<CanvasScaler>();
				ModsListInterface.HUDObj.AddComponent<GraphicRaycaster>();
				ModsListInterface.HUDObj.GetComponent<Canvas>().enabled = true;
				ModsListInterface.HUDObj.GetComponent<Canvas>().renderMode = 2;
				ModsListInterface.HUDObj.GetComponent<Canvas>().worldCamera = ModsListInterface.MainCamera.GetComponent<Camera>();
				ModsListInterface.HUDObj.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 5f);
				ModsListInterface.HUDObj.GetComponent<RectTransform>().position = new Vector3(ModsListInterface.MainCamera.transform.position.x, ModsListInterface.MainCamera.transform.position.y, ModsListInterface.MainCamera.transform.position.z);
				ModsListInterface.HUDObj2.transform.position = new Vector3(ModsListInterface.MainCamera.transform.position.x, ModsListInterface.MainCamera.transform.position.y, ModsListInterface.MainCamera.transform.position.z - 4.6f);
				ModsListInterface.HUDObj.transform.parent = ModsListInterface.HUDObj2.transform;
				ModsListInterface.HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
				Vector3 eulerAngles2 = ModsListInterface.HUDObj.GetComponent<RectTransform>().rotation.eulerAngles;
				eulerAngles.y = -270f;
				ModsListInterface.HUDObj.transform.localScale = new Vector3(1f, 1f, 1f);
				ModsListInterface.HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(eulerAngles);
				ModsListInterface.Testtext = new GameObject
				{
					transform = 
					{
						parent = ModsListInterface.HUDObj.transform
					}
				}.AddComponent<Text>();
				ModsListInterface.Testtext.text = "";
				ModsListInterface.Testtext.fontSize = 10;
				ModsListInterface.Testtext.font = GameObject.Find("COC Text").GetComponent<Text>().font;
				ModsListInterface.Testtext.rectTransform.sizeDelta = new Vector2(260f, 300f);
				ModsListInterface.Testtext.rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.7f);
				ModsListInterface.Testtext.rectTransform.localPosition = new Vector3(-2.4f, -2f, 0f);
				ModsListInterface.Testtext.material = ModsListInterface.AlertText;
				ModsListInterface.NotifiText = ModsListInterface.Testtext;
				ModsListInterface.Testtext.alignment = 2;
				this.loaded1 = true;
			}
			ModsListInterface.HUDObj2.transform.transform.position = new Vector3(ModsListInterface.MainCamera.transform.position.x, ModsListInterface.MainCamera.transform.position.y, ModsListInterface.MainCamera.transform.position.z);
			ModsListInterface.HUDObj2.transform.rotation = ModsListInterface.MainCamera.transform.rotation;
			ModsListInterface.HUDObj2ROOM.transform.transform.position = new Vector3(ModsListInterface.MainCamera.transform.position.x, ModsListInterface.MainCamera.transform.position.y, ModsListInterface.MainCamera.transform.position.z);
			ModsListInterface.HUDObj2ROOM.transform.rotation = ModsListInterface.MainCamera.transform.rotation;
			string text = string.Join("\n", ModsListInterface.modsEnabled);
			ModsListInterface.Testtext.color = MenuPatch.GetTheme(UI.Theme)[2];
			ModsListInterface.Testtext.text = "Mods Enabled:\n<color=white>" + text + "</color>";
		}

		// Token: 0x04000055 RID: 85
		private bool loaded1;

		// Token: 0x04000056 RID: 86
		private static GameObject HUDObjROOM;

		// Token: 0x04000057 RID: 87
		private static GameObject HUDObj2ROOM;

		// Token: 0x04000058 RID: 88
		private static GameObject MainCamera;

		// Token: 0x04000059 RID: 89
		private static Text TesttextROOM;

		// Token: 0x0400005A RID: 90
		private static TextAnchor textAnchor = 2;

		// Token: 0x0400005B RID: 91
		private static Material AlertText = new Material(Shader.Find("GUI/Text Shader"));

		// Token: 0x0400005C RID: 92
		private static Text NotifiTextROOM;

		// Token: 0x0400005D RID: 93
		private static GameObject HUDObj;

		// Token: 0x0400005E RID: 94
		private static GameObject HUDObj2;

		// Token: 0x0400005F RID: 95
		private static Text Testtext;

		// Token: 0x04000060 RID: 96
		private static Text NotifiText;

		// Token: 0x04000061 RID: 97
		public static List<string> modsEnabled = ModsList.modsEnabled;
	}
}
