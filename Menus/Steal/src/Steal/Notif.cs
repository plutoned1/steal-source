using System;
using System.Linq;
using Steal.Background;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

// Token: 0x02000003 RID: 3
internal class Notif : MonoBehaviour
{
	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600000B RID: 11 RVA: 0x00002310 File Offset: 0x00000510
	public static Notif instance
	{
		get
		{
			return Notif._instance;
		}
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002317 File Offset: 0x00000517
	private void Awake()
	{
		Notif._instance = this;
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002320 File Offset: 0x00000520
	private void Init()
	{
		try
		{
			if (GameObject.Find("Main Camera"))
			{
				this.MainCamera = GameObject.Find("Main Camera");
				this.HUDObj = new GameObject();
				this.HUDObj2 = new GameObject();
				this.HUDObj2.name = "NOTIFICATIONLIB_HUD_OBJ2";
				this.HUDObj.name = "NOTIFICATIONLIB_HUD_OBJ";
				this.HUDObj.AddComponent<Canvas>();
				this.HUDObj.AddComponent<CanvasScaler>();
				this.HUDObj.AddComponent<GraphicRaycaster>();
				this.HUDObj.GetComponent<Canvas>().enabled = true;
				this.HUDObj.GetComponent<Canvas>().renderMode = 2;
				this.HUDObj.GetComponent<Canvas>().worldCamera = this.MainCamera.GetComponent<Camera>();
				this.HUDObj.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 5f);
				this.HUDObj.GetComponent<RectTransform>().position = new Vector3(this.MainCamera.transform.position.x, this.MainCamera.transform.position.y, this.MainCamera.transform.position.z);
				this.HUDObj2.transform.position = new Vector3(this.MainCamera.transform.position.x, this.MainCamera.transform.position.y, this.MainCamera.transform.position.z - 4.6f);
				this.HUDObj.transform.parent = this.HUDObj2.transform;
				this.HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0.55f, 0f, 1.6f);
				Vector3 eulerAngles = this.HUDObj.GetComponent<RectTransform>().rotation.eulerAngles;
				eulerAngles.y = -270f;
				this.HUDObj.transform.localScale = new Vector3(1f, 1f, 1f);
				this.HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(eulerAngles);
				Notif.Testtext = new GameObject
				{
					transform = 
					{
						parent = this.HUDObj.transform
					}
				}.AddComponent<Text>();
				Notif.Testtext.text = "";
				Notif.Testtext.fontSize = 10;
				Notif.Testtext.font = GameObject.Find("COC Text").GetComponent<Text>().font;
				Notif.Testtext.rectTransform.sizeDelta = new Vector2(260f, 70f);
				Notif.Testtext.alignment = 6;
				if (!XRSettings.isDeviceActive)
				{
					Notif.Testtext.rectTransform.localScale = new Vector3(0.02f, 0.02f, 2f);
				}
				else
				{
					Notif.Testtext.rectTransform.localScale = new Vector3(0.01f, 0.01f, 1f);
				}
				Notif.Testtext.rectTransform.localPosition = new Vector3(-1.5f, -0.9f, -0.6f);
				Notif.Testtext.material = this.AlertText;
				Notif.NotifiText = Notif.Testtext;
			}
		}
		catch (Exception ex)
		{
			ShowConsole.LogERR(ex);
			throw;
		}
	}

	// Token: 0x0600000E RID: 14 RVA: 0x0000268C File Offset: 0x0000088C
	public void Update()
	{
		try
		{
			if (!this.HasInit && GameObject.Find("Main Camera") != null)
			{
				this.Init();
				this.HasInit = true;
			}
			if (this.HasInit)
			{
				this.HUDObj2.transform.position = new Vector3(this.MainCamera.transform.position.x, this.MainCamera.transform.position.y, this.MainCamera.transform.position.z);
				this.HUDObj2.transform.rotation = this.MainCamera.transform.rotation;
				if (Notif.Testtext.text != "")
				{
					this.NotificationDecayTimeCounter++;
					if (this.NotificationDecayTimeCounter > this.NotificationDecayTime)
					{
						this.Notifilines = null;
						this.newtext = "";
						this.NotificationDecayTimeCounter = 0;
						this.Notifilines = Notif.Testtext.text.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray<string>();
						foreach (string text in this.Notifilines)
						{
							if (text != "")
							{
								this.newtext = this.newtext + text + "\n";
							}
						}
						Notif.Testtext.text = this.newtext;
					}
				}
				else
				{
					this.NotificationDecayTimeCounter = 0;
				}
			}
		}
		catch (Exception ex)
		{
			ShowConsole.LogERR(ex);
			throw;
		}
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002838 File Offset: 0x00000A38
	public static void SendNotification(string NotificationText, Color color)
	{
		try
		{
			if (Notif.IsEnabled)
			{
				if (!XRSettings.isDeviceActive)
				{
					UIAlerts.SendAlert(NotificationText);
				}
				if (!NotificationText.Contains(Environment.NewLine))
				{
					NotificationText += Environment.NewLine;
				}
				Notif.NotifiText.text = Notif.NotifiText.text + NotificationText;
				Notif.PreviousNotifi = NotificationText;
				Notif.Testtext.color = color;
			}
		}
		catch
		{
			throw;
		}
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000028B4 File Offset: 0x00000AB4
	public static void ClearAllNotifications()
	{
		Notif.NotifiText.text = "";
	}

	// Token: 0x06000011 RID: 17 RVA: 0x000028C8 File Offset: 0x00000AC8
	public static void ClearPastNotifications(int amount)
	{
		string text = "";
		foreach (string text2 in Notif.NotifiText.text.Split(Environment.NewLine.ToCharArray()).Skip(amount).ToArray<string>())
		{
			if (text2 != "")
			{
				text = text + text2 + "\n";
			}
		}
		Notif.NotifiText.text = text;
	}

	// Token: 0x04000007 RID: 7
	private static Notif _instance;

	// Token: 0x04000008 RID: 8
	private GameObject HUDObj;

	// Token: 0x04000009 RID: 9
	private GameObject HUDObj2;

	// Token: 0x0400000A RID: 10
	private GameObject MainCamera;

	// Token: 0x0400000B RID: 11
	private static Text Testtext;

	// Token: 0x0400000C RID: 12
	private Material AlertText = new Material(Shader.Find("GUI/Text Shader"));

	// Token: 0x0400000D RID: 13
	private int NotificationDecayTime = 180;

	// Token: 0x0400000E RID: 14
	private int NotificationDecayTimeCounter;

	// Token: 0x0400000F RID: 15
	private string[] Notifilines;

	// Token: 0x04000010 RID: 16
	private string newtext;

	// Token: 0x04000011 RID: 17
	public static string PreviousNotifi;

	// Token: 0x04000012 RID: 18
	private bool HasInit;

	// Token: 0x04000013 RID: 19
	private static Text NotifiText;

	// Token: 0x04000014 RID: 20
	public static bool IsEnabled = true;
}
