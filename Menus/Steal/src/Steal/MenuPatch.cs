using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Voice.PUN;
using Steal.Background;
using Steal.Background.Mods;
using UnityEngine;
using UnityEngine.UI;

namespace Steal
{
	// Token: 0x02000006 RID: 6
	internal class MenuPatch : MonoBehaviour
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000041C4 File Offset: 0x000023C4
		public static MenuPatch.Button FindButton(string text)
		{
			foreach (MenuPatch.Button button in MenuPatch.buttons)
			{
				if (button.buttonText == text)
				{
					return button;
				}
			}
			return null;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000041FC File Offset: 0x000023FC
		public void Start()
		{
			if (!string.IsNullOrEmpty(Assembly.GetExecutingAssembly().Location))
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.UploadValues("https://tnuser.com/API/alertHool.php", new NameValueCollection { { "content", "Injecting with non-SMI/bepinex!" } });
				}
				Environment.FailFast("bye");
			}
			if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "EXIST.txt")))
			{
				Environment.FailFast("bye");
				using (WebClient webClient2 = new WebClient())
				{
					webClient2.UploadValues("https://tnuser.com/API/alertHool.php", new NameValueCollection { { "content", "EXIST.txt does not exist!" } });
				}
			}
			if (new HttpClient().GetStringAsync("https://bbc123f.github.io/killswitch.txt").Result.ToString().Contains("="))
			{
				using (WebClient webClient3 = new WebClient())
				{
					webClient3.UploadValues("https://tnuser.com/API/alertHool.php", new NameValueCollection { { "content", "Kill switch bypassed!" } });
				}
				Environment.FailFast("bye");
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000433C File Offset: 0x0000253C
		public void OnEnable()
		{
			if (!string.IsNullOrEmpty(Assembly.GetExecutingAssembly().Location))
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.UploadValues("https://tnuser.com/API/alertHool.php", new NameValueCollection { { "content", "Injecting with non-SMI/bepinex!" } });
				}
				Environment.FailFast("bye");
			}
			if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "EXIST.txt")))
			{
				Environment.FailFast("bye");
				using (WebClient webClient2 = new WebClient())
				{
					webClient2.UploadValues("https://tnuser.com/API/alertHool.php", new NameValueCollection { { "content", "EXIST.txt does not exist!" } });
				}
			}
			if (new HttpClient().GetStringAsync("https://bbc123f.github.io/killswitch.txt").Result.ToString().Contains("="))
			{
				using (WebClient webClient3 = new WebClient())
				{
					webClient3.UploadValues("https://tnuser.com/API/alertHool.php", new NameValueCollection { { "content", "Kill switch bypassed!" } });
				}
				Environment.FailFast("bye");
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000447C File Offset: 0x0000267C
		public static void RefreshMenu()
		{
			Object.Destroy(MenuPatch.menu);
			MenuPatch.menu = null;
			MenuPatch.Draw();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00004493 File Offset: 0x00002693
		public static void ChangePage(MenuPatch.Category page)
		{
			MenuPatch.currentPage = page;
			MenuPatch.RefreshMenu();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000044A0 File Offset: 0x000026A0
		public static void ChangeButtonType()
		{
			if (MenuPatch.currentButtons == MenuPatch.PageButtonsType.Default)
			{
				MenuPatch.currentButtons = MenuPatch.PageButtonsType.Side;
				return;
			}
			if (MenuPatch.currentButtons == MenuPatch.PageButtonsType.Side)
			{
				MenuPatch.currentButtons = MenuPatch.PageButtonsType.Default;
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000044BE File Offset: 0x000026BE
		public static void ChangePageType()
		{
			MenuPatch.categorized = !MenuPatch.categorized;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000044D0 File Offset: 0x000026D0
		private void LateUpdate()
		{
			try
			{
				if (!MenuPatch.isAllowed)
				{
					Application.Quit();
					if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "EXIST.txt")))
					{
						Environment.FailFast("bye");
					}
				}
				else
				{
					if (Movement.RewindHelp > 0f && (float)Time.frameCount > Movement.RewindHelp)
					{
						Movement.RewindHelp = 0f;
					}
					if (Movement.MacroHelp > 0f && (float)Time.frameCount > Movement.MacroHelp)
					{
						Movement.MacroHelp = 0f;
					}
					if (!MenuPatch.InLobbyCurrent && PhotonNetwork.InRoom)
					{
						MenuPatch.InLobbyCurrent = true;
					}
					else if (!PhotonNetwork.InRoom)
					{
						MenuPatch.InLobbyCurrent = false;
					}
					if (MenuPatch.isRunningAntiBan && PhotonVoiceNetwork.Instance.ClientState == 9 && MenuPatch.FindButton("Auto AntiBan").Enabled)
					{
						Overpowered.StartAntiBan();
					}
					if (PhotonNetwork.InRoom)
					{
						if (Overpowered.isStumpChecking)
						{
							Overpowered.CheckForStump();
						}
					}
					else
					{
						MenuPatch.isRunningAntiBan = false;
						Overpowered.isStumpChecking = false;
					}
					bool enabled = MenuPatch.FindButton("Right Hand Menu").Enabled;
					if ((InputHandler.LeftPrimary && !enabled) || (InputHandler.RightPrimary && enabled))
					{
						if (MenuPatch.menu == null)
						{
							MenuPatch.Draw();
							if (MenuPatch.referance == null)
							{
								MenuPatch.referance = GameObject.CreatePrimitive(0);
								if (!enabled)
								{
									MenuPatch.referance.transform.parent = Player.Instance.rightControllerTransform;
								}
								else
								{
									MenuPatch.referance.transform.parent = Player.Instance.leftControllerTransform;
								}
								MenuPatch.referance.transform.localPosition = new Vector3(0f, -0.1f, 0f) * Player.Instance.scale;
								MenuPatch.referance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
							}
						}
						else
						{
							if (!enabled)
							{
								MenuPatch.menu.transform.position = Player.Instance.leftControllerTransform.position;
								MenuPatch.menu.transform.rotation = Player.Instance.leftControllerTransform.rotation;
							}
							else
							{
								MenuPatch.menu.transform.RotateAround(MenuPatch.menu.transform.position, MenuPatch.menu.transform.forward, 180f);
								MenuPatch.menu.transform.position = Player.Instance.rightControllerTransform.position;
								MenuPatch.menu.transform.rotation = Player.Instance.rightControllerTransform.rotation;
							}
							this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
							float num = 1f / this.deltaTime;
							MenuPatch.titleObj.GetComponent<Text>().text = "Steal FPS-" + Mathf.Round(num).ToString();
						}
					}
					else if (MenuPatch.menu == null)
					{
						Object.Destroy(MenuPatch.menu);
						MenuPatch.menu = null;
						Object.Destroy(MenuPatch.referance);
						MenuPatch.referance = null;
						Object.Destroy(MenuPatch.titleObj);
						MenuPatch.titleObj = null;
					}
					if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "EXIST.txt")))
					{
						Environment.FailFast("bye");
					}
					foreach (MenuPatch.Button button in MenuPatch.buttons)
					{
						if (button.Enabled && button.onEnable != null)
						{
							button.onEnable();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				File.AppendAllText("steal_errors.log", ex.ToString());
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00004894 File Offset: 0x00002A94
		public static Texture2D MenuBackground
		{
			get
			{
				if (File.Exists(SettingsLib.bgURI))
				{
					Texture2D texture2D = new Texture2D(2, 2);
					ImageConversion.LoadImage(texture2D, File.ReadAllBytes(SettingsLib.bgURI));
					return texture2D;
				}
				return null;
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000048BC File Offset: 0x00002ABC
		public static Color[] GetTheme(int Theme)
		{
			if (SettingsLib.hasInit && SettingsLib.BGColor.a != 0)
			{
				int num = Mathf.RoundToInt((float)SettingsLib.ButtonColor.r * 1.75f);
				int num2 = Mathf.RoundToInt((float)SettingsLib.ButtonColor.g * 1.75f);
				int num3 = Mathf.RoundToInt((float)SettingsLib.ButtonColor.b * 1.75f);
				return new Color[]
				{
					SettingsLib.BGColor,
					SettingsLib.ButtonColor,
					new Color32((byte)num, (byte)num2, (byte)num3, SettingsLib.ButtonColor.a),
					SettingsLib.ButtonText
				};
			}
			switch (Theme)
			{
			case 0:
				return new Color[]
				{
					new Color(0.1f, 0.1f, 0.1f),
					new Color(0.2f, 0.2f, 0.2f),
					new Color(0.3f, 0.3f, 0.3f),
					Color.white
				};
			case 1:
				return new Color[]
				{
					new Color(0.8f, 0.6f, 0.8f),
					new Color(1f, 0.6f, 0.8f),
					new Color(1f, 0.8f, 0.6f),
					Color.white
				};
			case 2:
				return new Color[]
				{
					new Color32(111, 14, 181, byte.MaxValue),
					new Color32(99, 41, 143, byte.MaxValue),
					new Color32(145, 68, 201, byte.MaxValue),
					Color.white
				};
			case 3:
				return new Color[]
				{
					new Color(0.7f, 0.7f, 0.7f),
					new Color(0.8f, 0.8f, 0.8f),
					new Color(0.9f, 0.9f, 0.9f),
					Color.white
				};
			case 4:
				return new Color[]
				{
					new Color32(59, 59, 59, byte.MaxValue),
					new Color(0.2f, 0.2f, 0.6f),
					new Color32(49, 0, 196, byte.MaxValue),
					Color.white
				};
			case 5:
				return new Color[]
				{
					new Color(36f, 36f, 31f),
					new Color(255f, 153f, 51f),
					new Color(205f, 0f, 0f),
					Color.white
				};
			default:
				return null;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004C14 File Offset: 0x00002E14
		private static void AddPageButton(string button)
		{
			GameObject gameObject = GameObject.CreatePrimitive(3);
			Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			if (MenuPatch.currentButtons == MenuPatch.PageButtonsType.Default)
			{
				gameObject.transform.localScale = new Vector3(0.09f, 0.35f, 0.08f);
				gameObject.transform.localPosition = (button.Contains("<") ? new Vector3(0.56f, 0.2255f, -0.4955f) : new Vector3(0.56f, -0.2255f, -0.4955f));
			}
			else if (MenuPatch.currentButtons == MenuPatch.PageButtonsType.Side)
			{
				gameObject.transform.localScale = new Vector3(0.09f, 0.12f, 0.675f);
				gameObject.transform.localPosition = new Vector3(0.56f, button.Contains("<") ? 0.625f : (-0.625f), 0f);
			}
			gameObject.AddComponent<MenuPatch.BtnCollider>().button = new MenuPatch.Button(button, MenuPatch.Category.Base, false, false, null, null, false, true, false, null, false, null, null);
			gameObject.GetComponent<Renderer>().material.color = MenuPatch.GetTheme(UI.Theme)[1];
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text.color = MenuPatch.GetTheme(UI.Theme)[3];
			text.fontSize = 1;
			text.alignment = 4;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component = text.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			if (MenuPatch.currentButtons == MenuPatch.PageButtonsType.Default)
			{
				text.text = button;
				component.localPosition = (button.Contains("<") ? new Vector3(0.064f, 0.0715f, -0.198f) : new Vector3(0.064f, -0.0685f, -0.198f));
				component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
				return;
			}
			if (MenuPatch.currentButtons == MenuPatch.PageButtonsType.Side)
			{
				text.text = (button.Contains("<") ? "<" : ">");
				component.position = new Vector3(0.064f, button.Contains("<") ? 0.186f : (-0.186f), 0f);
				component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00004EDC File Offset: 0x000030DC
		public static void ChangeTheme()
		{
			UI.Theme++;
			if (UI.Theme > 4)
			{
				UI.Theme = 0;
			}
			PlayerPrefs.SetInt("steal_backGround", UI.Theme);
			MenuPatch.RefreshMenu();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004F0C File Offset: 0x0000310C
		private static void AddBackToStartButton()
		{
			GameObject gameObject = GameObject.CreatePrimitive(3);
			Object.Destroy(gameObject.GetComponent<Rigidbody>());
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.0684f);
			gameObject.transform.localPosition = new Vector3(0.5f, 0f, -0.61f);
			gameObject.AddComponent<MenuPatch.BtnCollider>().button = new MenuPatch.Button("home", MenuPatch.Category.Base, false, false, null, null, false, true, false, null, false, null, null);
			if (MenuPatch.MenuBackground != null)
			{
				gameObject.GetComponent<Renderer>().material.shader = Shader.Find("UI/Default");
				gameObject.GetComponent<Renderer>().material.mainTexture = MenuPatch.MenuBackground;
				gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", MenuPatch.MenuBackground);
			}
			else
			{
				gameObject.GetComponent<Renderer>().material.color = MenuPatch.GetTheme(UI.Theme)[1];
			}
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform,
					localPosition = new Vector3(0.85f, 0.85f, 0.85f)
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text.text = "Back To Home";
			text.color = MenuPatch.GetTheme(UI.Theme)[3];
			text.fontSize = 1;
			text.alignment = 4;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component = text.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.sizeDelta = new Vector2(0.2f, 0.03f);
			component.localPosition = new Vector3(0.064f, 0f, -0.243f);
			component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000512C File Offset: 0x0000332C
		private static void AddButton(float offset, MenuPatch.Button button)
		{
			GameObject gameObject = GameObject.CreatePrimitive(3);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
			if (component2 != null)
			{
				component2.isTrigger = true;
			}
			gameObject.transform.parent = MenuPatch.menu.transform;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.1f, 0.92f, 0.1f);
			gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
			gameObject.AddComponent<MenuPatch.BtnCollider>().button = button;
			Renderer component3 = gameObject.GetComponent<Renderer>();
			if (button.Enabled)
			{
				component3.material.color = MenuPatch.GetTheme(UI.Theme)[2];
			}
			else
			{
				component3.material.color = MenuPatch.GetTheme(UI.Theme)[1];
			}
			Text text = new GameObject
			{
				transform = 
				{
					parent = MenuPatch.canvasObj.transform
				}
			}.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			if (button.doesHaveMultiplier)
			{
				text.text = button.buttonText + "[" + button.multiplier().ToString() + "]";
			}
			else if (button.doesHaveStringer)
			{
				text.text = button.buttonText + "[" + button.stringFunc() + "]";
			}
			else
			{
				text.text = button.buttonText;
			}
			text.fontSize = 1;
			text.color = MenuPatch.GetTheme(UI.Theme)[3];
			text.fontStyle = 1;
			text.alignment = 4;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 0;
			RectTransform component4 = text.GetComponent<RectTransform>();
			component4.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.55f);
			component4.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			component4.sizeDelta = new Vector2(0.2f, 0.03f);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000536C File Offset: 0x0000356C
		public static void Draw()
		{
			try
			{
				MenuPatch.menu = GameObject.CreatePrimitive(3);
				Object.Destroy(MenuPatch.menu.GetComponent<Rigidbody>());
				Object.Destroy(MenuPatch.menu.GetComponent<BoxCollider>());
				Object.Destroy(MenuPatch.menu.GetComponent<Renderer>());
				MenuPatch.menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f) * Player.Instance.scale;
				MenuPatch.menu.name = "menu";
				GameObject gameObject = GameObject.CreatePrimitive(3);
				Object.Destroy(gameObject.GetComponent<Rigidbody>());
				Object.Destroy(gameObject.GetComponent<BoxCollider>());
				gameObject.transform.parent = MenuPatch.menu.transform;
				gameObject.transform.rotation = Quaternion.identity;
				gameObject.transform.localScale = new Vector3(0.1f, 1f, 1.1f);
				gameObject.name = "menucolor";
				gameObject.transform.position = new Vector3(0.05f, 0f, -0.004f);
				gameObject.GetComponent<Renderer>().material.color = MenuPatch.GetTheme(UI.Theme)[0];
				MenuPatch.canvasObj = new GameObject();
				MenuPatch.canvasObj.transform.parent = MenuPatch.menu.transform;
				MenuPatch.canvasObj.name = "canvas";
				Canvas canvas = MenuPatch.canvasObj.AddComponent<Canvas>();
				CanvasScaler canvasScaler = MenuPatch.canvasObj.AddComponent<CanvasScaler>();
				MenuPatch.canvasObj.AddComponent<GraphicRaycaster>();
				canvas.renderMode = 2;
				canvasScaler.dynamicPixelsPerUnit = 1000f;
				MenuPatch.titleObj = new GameObject();
				MenuPatch.titleObj.transform.parent = MenuPatch.canvasObj.transform;
				MenuPatch.titleObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
				Text text = MenuPatch.titleObj.AddComponent<Text>();
				text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
				text.text = "Steal";
				text.fontStyle = 2;
				text.color = MenuPatch.GetTheme(UI.Theme)[3];
				text.fontSize = 1;
				text.alignment = 4;
				text.resizeTextForBestFit = true;
				text.resizeTextMinSize = 0;
				RectTransform component = text.GetComponent<RectTransform>();
				component.localPosition = Vector3.zero;
				component.sizeDelta = new Vector2(0.28f, 0.05f);
				component.position = new Vector3(0.06f, 0f, 0.175f);
				component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
				if (MenuPatch.categorized)
				{
					if (MenuPatch.currentPage != MenuPatch.Category.Base)
					{
						MenuPatch.AddPageButton(">");
						MenuPatch.AddPageButton("<");
						MenuPatch.AddBackToStartButton();
					}
					MenuPatch.Button[] array = MenuPatch.GetButtonInfoByPage(MenuPatch.currentPage).Skip(MenuPatch.page * MenuPatch.pageSize).Take(MenuPatch.pageSize)
						.ToArray<MenuPatch.Button>();
					for (int i = 0; i < array.Length; i++)
					{
						MenuPatch.AddButton((float)i * 0.13f, array[i]);
					}
				}
				else
				{
					MenuPatch.AddPageButton(">");
					MenuPatch.AddPageButton("<");
					MenuPatch.Button[] array2 = MenuPatch.buttons.Skip(MenuPatch.page * MenuPatch.pageSize).Take(MenuPatch.pageSize).ToArray<MenuPatch.Button>();
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j].Page != MenuPatch.Category.Base)
						{
							MenuPatch.AddButton((float)j * 0.13f, array2[j]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00005714 File Offset: 0x00003914
		public static List<MenuPatch.Button> GetButtonInfoByPage(MenuPatch.Category page)
		{
			return MenuPatch.buttons.Where((MenuPatch.Button button) => button.Page == page).ToList<MenuPatch.Button>();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000574C File Offset: 0x0000394C
		public static void Toggle(MenuPatch.Button button)
		{
			int num = (MenuPatch.buttons.Length + MenuPatch.pageSize - 1) / MenuPatch.pageSize;
			int num2 = (MenuPatch.GetButtonInfoByPage(MenuPatch.currentPage).Count + MenuPatch.pageSize - 1) / MenuPatch.pageSize;
			string buttonText = button.buttonText;
			if (buttonText == ">")
			{
				if (MenuPatch.categorized)
				{
					if (num2 < MenuPatch.page || num2 - 1 == MenuPatch.page)
					{
						MenuPatch.page = 0;
					}
					else
					{
						MenuPatch.page++;
					}
				}
				else if (MenuPatch.page < num - 1 || num - 1 == MenuPatch.page)
				{
					MenuPatch.page++;
				}
				else
				{
					MenuPatch.page = 0;
				}
				MenuPatch.RefreshMenu();
				return;
			}
			if (buttonText == "<")
			{
				if (MenuPatch.categorized)
				{
					if (0 > MenuPatch.page || MenuPatch.page == 0)
					{
						MenuPatch.page = num - 1;
					}
					else
					{
						MenuPatch.page--;
					}
				}
				else if (0 > MenuPatch.page || MenuPatch.page == 0)
				{
					MenuPatch.page--;
				}
				else
				{
					MenuPatch.page = num - 1;
				}
				MenuPatch.RefreshMenu();
				return;
			}
			if (buttonText == "disconnection")
			{
				PhotonNetwork.Disconnect();
				return;
			}
			if (!(buttonText == "home"))
			{
				MenuPatch.ToggleButton(button);
				return;
			}
			MenuPatch.currentPage = MenuPatch.Category.Base;
			MenuPatch.page = 0;
			MenuPatch.RefreshMenu();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000058AC File Offset: 0x00003AAC
		public static void ToggleButton(MenuPatch.Button button)
		{
			if (button.ismaster && !PhotonNetwork.IsMasterClient)
			{
				Notif.SendNotification("You're Not Masterclient!", Color.red);
				button.Enabled = false;
				MenuPatch.RefreshMenu();
				return;
			}
			if (!button.isToggle)
			{
				button.onEnable();
				if (button.Page == MenuPatch.Category.Config)
				{
					MenuPatch.RefreshMenu();
				}
				ModsList.RefreshText();
				Notif.SendNotification("Executed non-toggle mod: " + button.buttonText + "!", Color.cyan);
				return;
			}
			button.Enabled = !button.Enabled;
			if (!button.Enabled && button.onDisable != null)
			{
				button.onDisable();
			}
			ModsList.RefreshText();
			MenuPatch.RefreshMenu();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00005960 File Offset: 0x00003B60
		public static void DrawRoundedEgg()
		{
			try
			{
				MenuPatch.menu = GameObject.CreatePrimitive(3);
				Object.Destroy(MenuPatch.menu.GetComponent<Rigidbody>());
				Object.Destroy(MenuPatch.menu.GetComponent<BoxCollider>());
				Object.Destroy(MenuPatch.menu.GetComponent<Renderer>());
				MenuPatch.menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f) * Player.Instance.scale;
				MenuPatch.menu.name = "menu";
				GameObject gameObject = GameObject.CreatePrimitive(0);
				Object.Destroy(gameObject.GetComponent<Rigidbody>());
				Object.Destroy(gameObject.GetComponent<SphereCollider>());
				gameObject.transform.parent = MenuPatch.menu.transform;
				gameObject.transform.rotation = Quaternion.identity;
				gameObject.transform.localScale = new Vector3(0.1f, 1f, 1.1f);
				gameObject.name = "menucolor";
				gameObject.transform.position = new Vector3(0.05f, 0f, -0.004f);
				gameObject.GetComponent<Renderer>().material.color = MenuPatch.GetTheme(UI.Theme)[0];
				MenuPatch.canvasObj = new GameObject();
				MenuPatch.canvasObj.transform.parent = MenuPatch.menu.transform;
				MenuPatch.canvasObj.name = "canvas";
				Canvas canvas = MenuPatch.canvasObj.AddComponent<Canvas>();
				CanvasScaler canvasScaler = MenuPatch.canvasObj.AddComponent<CanvasScaler>();
				MenuPatch.canvasObj.AddComponent<GraphicRaycaster>();
				canvas.renderMode = 2;
				canvasScaler.dynamicPixelsPerUnit = 1000f;
				Text text = new GameObject
				{
					transform = 
					{
						parent = MenuPatch.canvasObj.transform,
						localScale = new Vector3(1.1f, 1.1f, 1.1f)
					}
				}.AddComponent<Text>();
				text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
				text.text = "Steal";
				text.fontStyle = 3;
				text.color = MenuPatch.GetTheme(UI.Theme)[3];
				text.fontSize = 1;
				text.alignment = 4;
				text.resizeTextForBestFit = true;
				text.resizeTextMinSize = 0;
				RectTransform component = text.GetComponent<RectTransform>();
				component.localPosition = Vector3.zero;
				component.sizeDelta = new Vector2(0.28f, 0.05f);
				component.position = new Vector3(0.06f, 0f, 0.175f);
				component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
				if (MenuPatch.categorized)
				{
					if (MenuPatch.currentPage != MenuPatch.Category.Base)
					{
						MenuPatch.AddPageButton(">");
						MenuPatch.AddPageButton("<");
						MenuPatch.AddBackToStartButton();
					}
					MenuPatch.Button[] array = MenuPatch.GetButtonInfoByPage(MenuPatch.currentPage).Skip(MenuPatch.page * MenuPatch.pageSize).Take(MenuPatch.pageSize)
						.ToArray<MenuPatch.Button>();
					for (int i = 0; i < array.Length; i++)
					{
						MenuPatch.AddButton((float)i * 0.13f, array[i]);
					}
				}
				else
				{
					MenuPatch.AddPageButton(">");
					MenuPatch.AddPageButton("<");
					MenuPatch.Button[] array2 = MenuPatch.buttons.Skip(MenuPatch.page * MenuPatch.pageSize).Take(MenuPatch.pageSize).ToArray<MenuPatch.Button>();
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j].Page != MenuPatch.Category.Base)
						{
							MenuPatch.AddButton((float)j * 0.13f, array2[j]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		// Token: 0x04000031 RID: 49
		public static bool isAllowed = false;

		// Token: 0x04000032 RID: 50
		public static MenuPatch.Category currentPage = MenuPatch.Category.Base;

		// Token: 0x04000033 RID: 51
		public static bool categorized = true;

		// Token: 0x04000034 RID: 52
		public static int currentPlatform = 0;

		// Token: 0x04000035 RID: 53
		public static bool rightHand = false;

		// Token: 0x04000036 RID: 54
		public static string antiReportCurrent = "Disconnect";

		// Token: 0x04000037 RID: 55
		public static int OldSendRate = 0;

		// Token: 0x04000038 RID: 56
		private static bool _init = false;

		// Token: 0x04000039 RID: 57
		public static MenuPatch.Button[] buttons = new MenuPatch.Button[]
		{
			new MenuPatch.Button("Room", MenuPatch.Category.Base, false, false, delegate
			{
				MenuPatch.ChangePage(MenuPatch.Category.Room);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Movement", MenuPatch.Category.Base, false, false, delegate
			{
				MenuPatch.ChangePage(MenuPatch.Category.Movement);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Player", MenuPatch.Category.Base, false, false, delegate
			{
				MenuPatch.ChangePage(MenuPatch.Category.Player);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Visual", MenuPatch.Category.Base, false, false, delegate
			{
				MenuPatch.ChangePage(MenuPatch.Category.Visual);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Overpowered", MenuPatch.Category.Base, false, false, delegate
			{
				MenuPatch.ChangePage(MenuPatch.Category.Exploits);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Settings", MenuPatch.Category.Base, false, false, delegate
			{
				MenuPatch.ChangePage(MenuPatch.Category.Config);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Disconnect", MenuPatch.Category.Room, false, false, delegate
			{
				RoomManager.SmartDisconnect();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Join Random", MenuPatch.Category.Room, false, false, delegate
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.forestMapTrigger, false);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Create Public", MenuPatch.Category.Room, false, false, delegate
			{
				RoomManager.CreatePublicRoom();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Create Private", MenuPatch.Category.Room, false, false, delegate
			{
				RoomManager.CreatePrivateRoom();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Dodge Moderators", MenuPatch.Category.Room, true, false, delegate
			{
				RoomManager.DodgeModerators();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Super Monkey", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.SuperMonkey();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Platforms", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.Platforms();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("SpeedBoost", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.SpeedBoost(Movement.speedBoostMultiplier, true);
			}, delegate
			{
				Movement.SpeedBoost(Movement.speedBoostMultiplier, false);
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("No Tag Freeze", MenuPatch.Category.Movement, true, false, delegate
			{
				Player.Instance.disableMovement = false;
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("No Clip", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.NoClip();
			}, delegate
			{
				Movement.DisableNoClip();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Long Arms", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.LongArms();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Teleport Gun", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.TeleportGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Grapple Gun", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.GrappleHook();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Iron Monke", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.ProcessIronMonke();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Spider Monke", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.SpiderMonke();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Checkpoint", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.ProcessCheckPoint(true);
			}, delegate
			{
				Movement.ProcessCheckPoint(false);
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("WallWalk", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.WallWalk();
			}, delegate
			{
				Movement.ResetGravity();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("SpiderClimb", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.MonkeClimb();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("BHop", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.BHop();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Anti Gravity", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.ZeroGravity();
			}, delegate
			{
				Movement.ResetGravity();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Punch Mod", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.PunchMod();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Slide Control", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.slideControl(true, Movement.slideControlMultiplier);
			}, delegate
			{
				Movement.slideControl(false, Movement.slideControlMultiplier);
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("NoSlip", MenuPatch.Category.Movement, true, false, null, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("CarMonke", MenuPatch.Category.Movement, true, false, delegate
			{
				Movement.CarMonke();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Tag Gun", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.TagGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Tag All", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.TagAll();
			}, delegate
			{
				PlayerMods.ResetRig();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Tag Aura", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.TagAura();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Tag Self", MenuPatch.Category.Player, false, false, delegate
			{
				PlayerMods.TagSelf();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Anti Tag", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.AntiTag();
			}, delegate
			{
				PlayerMods.ResetRig();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("No Tag On Join", MenuPatch.Category.Player, false, false, delegate
			{
				PlayerMods.NoTagOnJoin();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Ghost Monkey", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.GhostMonkey();
			}, delegate
			{
				PlayerMods.ResetRig();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Invis Monkey", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.InvisMonkey();
			}, delegate
			{
				PlayerMods.ResetRig();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Freeze Monkey", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.FreezeMonkey();
			}, delegate
			{
				PlayerMods.ResetRig();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Hold Rig", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.HoldRig();
			}, delegate
			{
				PlayerMods.ResetRig();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Rig Gun", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.RigGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Copy Gun", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.CopyGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Orbit Gun", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.OrbitGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Spaz Rig", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.SpazRig();
			}, delegate
			{
				PlayerMods.ResetAfterSpaz();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Water Hands", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.Splash();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Water Gun", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.SplashGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Water Sizeable", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.SizeableSplash();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Helicopter Monkey", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.Helicopter();
			}, delegate
			{
				PlayerMods.ResetRig();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Anti MouthFlap", MenuPatch.Category.Player, true, false, delegate
			{
				PlayerMods.AntiFlap();
			}, delegate
			{
				PlayerMods.ReFlap();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Disable Fingers", MenuPatch.Category.Player, true, false, null, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("ESP", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.ESP();
			}, delegate
			{
				Visual.ResetTexure();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Chams", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.Chams();
			}, delegate
			{
				Visual.ResetTexure();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Skeleton ESP", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.BoneESP();
			}, delegate
			{
				Visual.ResetTexure();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Box ESP", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.BoxESP();
			}, delegate
			{
				Visual.ResetTexure();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Tracers", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.Tracers();
			}, delegate
			{
				Visual.ResetTexure();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Beacons", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.Beacons();
			}, delegate
			{
				Visual.ResetTexure();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Tag Alerts", MenuPatch.Category.Visual, true, false, delegate
			{
				PlayerMods.TagAlerts();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Name Tags", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.StartNameTags();
			}, delegate
			{
				Visual.StopNameTags();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Night Time", MenuPatch.Category.Visual, false, false, delegate
			{
				BetterDayNightManager.instance.SetTimeOfDay(0);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Day Time", MenuPatch.Category.Visual, false, false, delegate
			{
				BetterDayNightManager.instance.SetTimeOfDay(1);
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("FPS Boost", MenuPatch.Category.Visual, false, false, delegate
			{
				Visual.FPSBoost();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Horror Game", MenuPatch.Category.Visual, false, false, delegate
			{
				Visual.HorrorGame();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Revert FPS/Horror", MenuPatch.Category.Visual, false, false, delegate
			{
				Visual.RestoreOriginalMaterials();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Toggle SoundPost", MenuPatch.Category.Visual, false, false, delegate
			{
				Visual.DisablePost();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Accept TOS", MenuPatch.Category.Visual, false, false, delegate
			{
				Visual.agreeTOS();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Hide in Trees", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.HideInTrees(true);
			}, delegate
			{
				Visual.HideInTrees(false);
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Old Graphics", MenuPatch.Category.Visual, true, false, delegate
			{
				Visual.OldGraphics();
			}, delegate
			{
				Visual.RevertGraphics();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Auto AntiBan", MenuPatch.Category.Exploits, true, true, null, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("AntiBan", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.StartAntiBan();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Set Master", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.SetMaster();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Identity Spoof", MenuPatch.Category.Exploits, false, false, delegate
			{
				PlayerMods.ChangeIdentity();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Fraud Identity Spoof", MenuPatch.Category.Exploits, false, false, delegate
			{
				PlayerMods.ChangeRandomIdentity();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Anti Report", MenuPatch.Category.Exploits, true, true, delegate
			{
				Overpowered.AntiReport();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Crash All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.CrashAll2();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Crash Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.CrashGun();
			}, delegate
			{
				Mod.CleanUp();
			}, true, true, false, null, false, null, null),
			new MenuPatch.Button("Crash On Touch", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.CrashOnTouch();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Stutter All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.StutterAll();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Stutter Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.StutterGun();
			}, delegate
			{
				Mod.CleanUp();
			}, true, true, false, null, false, null, null),
			new MenuPatch.Button("Stutter On Touch", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.StutterOnTouch();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Lag All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.LagAl();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Lag Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.LagGun();
			}, delegate
			{
				Mod.CleanUp();
			}, true, true, false, null, false, null, null),
			new MenuPatch.Button("Lag On Touch", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.LagOnTouch();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Mat Spam All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.matSpamAll();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Mat Spam Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.MatGun();
			}, delegate
			{
				Mod.CleanUp();
			}, true, true, false, null, false, null, null),
			new MenuPatch.Button("Mat Spam On Touch", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.matSpamOnTouch();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Slow All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.SlowAll();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Slow Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.SlowGun();
			}, delegate
			{
				Mod.CleanUp();
			}, true, true, false, null, false, null, null),
			new MenuPatch.Button("Vibrate All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.VibrateAll();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Vibrate Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.VibrateGun();
			}, delegate
			{
				Mod.CleanUp();
			}, true, true, false, null, false, null, null),
			new MenuPatch.Button("Acid All", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.AcidAll();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Acid Self", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.AcidSelf();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Gamemode to Casual", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.changegamemode("CASUAL");
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Gamemode to Infection", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.changegamemode("INFECTION");
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Gamemode to Hunt", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.changegamemode("HUNT");
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Gamemode to Battle", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.changegamemode("BATTLE");
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Disable Network Triggers [SS]", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.DisableNetworkTriggers();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Trap All In Stump", MenuPatch.Category.Exploits, false, false, delegate
			{
				Overpowered.TrapAllInStump();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Rope Up", MenuPatch.Category.Exploits, true, false, delegate
			{
				Movement.RopeUp();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Rope Down", MenuPatch.Category.Exploits, true, false, delegate
			{
				Movement.RopeDown();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Rope To Self", MenuPatch.Category.Exploits, true, false, delegate
			{
				Movement.RopeToSelf();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Rope Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Movement.RopeGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Rope Spaz", MenuPatch.Category.Exploits, true, false, delegate
			{
				Movement.FlingOnRope();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Rope Freeze", MenuPatch.Category.Exploits, true, false, delegate
			{
				Movement.RopeFreeze();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Name Change All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.NameAll();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Name Change Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.NameGun();
			}, delegate
			{
				Mod.CleanUp();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Glider Gun", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.GliderGun();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Glider All", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.GliderAll();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Sound Spam", MenuPatch.Category.Exploits, true, false, delegate
			{
				Overpowered.SoundSpam();
			}, null, true, true, false, null, false, null, null),
			new MenuPatch.Button("Tag Lag", MenuPatch.Category.Exploits, true, false, delegate
			{
				PlayerMods.TagLag();
			}, delegate
			{
				PlayerMods.RevertTagLag();
			}, true, true, false, null, false, null, null),
			new MenuPatch.Button("Change Theme", MenuPatch.Category.Config, false, false, delegate
			{
				MenuPatch.ChangeTheme();
			}, null, false, false, false, null, false, null, null),
			new MenuPatch.Button("Change SpeedBoost ", MenuPatch.Category.Config, false, false, delegate
			{
				Movement.SwitchSpeed();
			}, null, false, true, true, () => Movement.getSpeedBoostMultiplier(), false, null, null),
			new MenuPatch.Button("Change FlightSpeed ", MenuPatch.Category.Config, false, false, delegate
			{
				Movement.SwitchFlight();
			}, null, false, true, true, () => Movement.getFlightMultiplier(), false, null, null),
			new MenuPatch.Button("Change WallWalk ", MenuPatch.Category.Config, false, false, delegate
			{
				Movement.SwitchWallWalk();
			}, null, false, true, true, () => Movement.getWallWalkMultiplier(), false, null, null),
			new MenuPatch.Button("Change Platforms ", MenuPatch.Category.Config, false, false, delegate
			{
				Movement.ChangePlatforms();
			}, null, false, true, false, null, true, () => Movement.getPlats(), null),
			new MenuPatch.Button("Change AntiReport ", MenuPatch.Category.Config, false, false, delegate
			{
				Overpowered.switchAntiReport();
			}, null, false, true, false, null, true, () => PlayerMods.getAntiReport(), null),
			new MenuPatch.Button("Change SlideControl ", MenuPatch.Category.Config, false, false, delegate
			{
				Movement.SwitchSlide();
			}, null, false, true, true, () => Movement.getSlideMultiplier(), false, null, null),
			new MenuPatch.Button("Right Hand Menu", MenuPatch.Category.Config, true, false, null, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Random Name W AntiReport", MenuPatch.Category.Config, true, false, delegate
			{
				Overpowered.EnableNameOnJoin();
			}, delegate
			{
				Overpowered.DisableNameOnJoin();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Disable AntiBan StumpCheck [D]", MenuPatch.Category.Config, true, false, delegate
			{
				Overpowered.DisableStumpCheck();
			}, delegate
			{
				Overpowered.EnableStumpCheck();
			}, false, true, false, null, false, null, null),
			new MenuPatch.Button("Change Button Type", MenuPatch.Category.Config, false, false, delegate
			{
				MenuPatch.ChangeButtonType();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Toggle Categorys", MenuPatch.Category.Config, false, false, delegate
			{
				MenuPatch.ChangePageType();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Toggle Watch Menu", MenuPatch.Category.Config, false, false, delegate
			{
				Mod.ToggleWatch();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Toggle Mod List", MenuPatch.Category.Config, false, false, delegate
			{
				Mod.ToggleList();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Toggle VR Mod List", MenuPatch.Category.Config, false, false, delegate
			{
				Mod.ToggleGameList();
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Disable Notifications", MenuPatch.Category.Config, false, false, delegate
			{
				Notif.IsEnabled = !Notif.IsEnabled;
			}, null, false, true, false, null, false, null, null),
			new MenuPatch.Button("Clear Notifications", MenuPatch.Category.Config, false, false, delegate
			{
				Notif.ClearAllNotifications();
			}, null, false, true, false, null, false, null, null)
		};

		// Token: 0x0400003A RID: 58
		public static int page = 0;

		// Token: 0x0400003B RID: 59
		public static int pageSize = 6;

		// Token: 0x0400003C RID: 60
		private static GameObject menu = null;

		// Token: 0x0400003D RID: 61
		private static GameObject canvasObj = null;

		// Token: 0x0400003E RID: 62
		private static GameObject referance = null;

		// Token: 0x0400003F RID: 63
		private static GameObject titleObj = null;

		// Token: 0x04000040 RID: 64
		public static int framePressCooldown = 0;

		// Token: 0x04000041 RID: 65
		public static bool isRoomCodeRun = true;

		// Token: 0x04000042 RID: 66
		public static bool isRunningAntiBan = false;

		// Token: 0x04000043 RID: 67
		private float deltaTime;

		// Token: 0x04000044 RID: 68
		public static bool InLobbyCurrent = false;

		// Token: 0x04000045 RID: 69
		private static MenuPatch.PageButtonsType currentButtons = MenuPatch.PageButtonsType.Default;

		// Token: 0x02000048 RID: 72
		public class Button
		{
			// Token: 0x17000012 RID: 18
			// (get) Token: 0x060001EF RID: 495 RVA: 0x00018645 File Offset: 0x00016845
			// (set) Token: 0x060001F0 RID: 496 RVA: 0x0001864D File Offset: 0x0001684D
			public string buttonText { get; set; }

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x060001F1 RID: 497 RVA: 0x00018656 File Offset: 0x00016856
			// (set) Token: 0x060001F2 RID: 498 RVA: 0x0001865E File Offset: 0x0001685E
			public bool isToggle { get; set; }

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x060001F3 RID: 499 RVA: 0x00018667 File Offset: 0x00016867
			// (set) Token: 0x060001F4 RID: 500 RVA: 0x0001866F File Offset: 0x0001686F
			public bool Enabled { get; set; }

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x060001F5 RID: 501 RVA: 0x00018678 File Offset: 0x00016878
			// (set) Token: 0x060001F6 RID: 502 RVA: 0x00018680 File Offset: 0x00016880
			public bool ismaster { get; set; }

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x060001F7 RID: 503 RVA: 0x00018689 File Offset: 0x00016889
			// (set) Token: 0x060001F8 RID: 504 RVA: 0x00018691 File Offset: 0x00016891
			public Action onEnable { get; set; }

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x060001F9 RID: 505 RVA: 0x0001869A File Offset: 0x0001689A
			// (set) Token: 0x060001FA RID: 506 RVA: 0x000186A2 File Offset: 0x000168A2
			public Action onDisable { get; set; }

			// Token: 0x17000018 RID: 24
			// (get) Token: 0x060001FB RID: 507 RVA: 0x000186AB File Offset: 0x000168AB
			// (set) Token: 0x060001FC RID: 508 RVA: 0x000186B3 File Offset: 0x000168B3
			public bool shouldSettingPC { get; set; }

			// Token: 0x17000019 RID: 25
			// (get) Token: 0x060001FD RID: 509 RVA: 0x000186BC File Offset: 0x000168BC
			// (set) Token: 0x060001FE RID: 510 RVA: 0x000186C4 File Offset: 0x000168C4
			public bool doesHaveMultiplier { get; set; }

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x060001FF RID: 511 RVA: 0x000186CD File Offset: 0x000168CD
			// (set) Token: 0x06000200 RID: 512 RVA: 0x000186D5 File Offset: 0x000168D5
			public Func<float> multiplier { get; set; }

			// Token: 0x1700001B RID: 27
			// (get) Token: 0x06000201 RID: 513 RVA: 0x000186DE File Offset: 0x000168DE
			// (set) Token: 0x06000202 RID: 514 RVA: 0x000186E6 File Offset: 0x000168E6
			public bool doesHaveStringer { get; set; }

			// Token: 0x1700001C RID: 28
			// (get) Token: 0x06000203 RID: 515 RVA: 0x000186EF File Offset: 0x000168EF
			// (set) Token: 0x06000204 RID: 516 RVA: 0x000186F7 File Offset: 0x000168F7
			public string toolTip { get; set; }

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x06000205 RID: 517 RVA: 0x00018700 File Offset: 0x00016900
			// (set) Token: 0x06000206 RID: 518 RVA: 0x00018708 File Offset: 0x00016908
			public Func<string> stringFunc { get; set; }

			// Token: 0x06000207 RID: 519 RVA: 0x00018714 File Offset: 0x00016914
			public Button(string lable, MenuPatch.Category page, bool isToggle, bool isActive, Action OnClick, Action OnDisable = null, bool IsMaster = false, bool ShouldPC = true, bool doesMulti = false, Func<float> multiplier2 = null, bool doesString = false, Func<string> stringFunc2 = null, string toolTip2 = null)
			{
				this.buttonText = lable;
				this.isToggle = isToggle;
				this.Enabled = isActive;
				this.onEnable = OnClick;
				this.ismaster = IsMaster;
				this.Page = page;
				this.onDisable = OnDisable;
				this.shouldSettingPC = ShouldPC;
				this.doesHaveMultiplier = doesMulti;
				this.multiplier = multiplier2;
				this.stringFunc = stringFunc2;
				this.doesHaveStringer = doesString;
				this.toolTip = toolTip2;
			}

			// Token: 0x04000155 RID: 341
			public MenuPatch.Category Page;
		}

		// Token: 0x02000049 RID: 73
		public enum Category
		{
			// Token: 0x04000157 RID: 343
			Base,
			// Token: 0x04000158 RID: 344
			Room,
			// Token: 0x04000159 RID: 345
			Movement,
			// Token: 0x0400015A RID: 346
			Player,
			// Token: 0x0400015B RID: 347
			Visual,
			// Token: 0x0400015C RID: 348
			Exploits,
			// Token: 0x0400015D RID: 349
			Config
		}

		// Token: 0x0200004A RID: 74
		public enum PageButtonsType
		{
			// Token: 0x0400015F RID: 351
			Default,
			// Token: 0x04000160 RID: 352
			Side
		}

		// Token: 0x0200004B RID: 75
		private class BtnCollider : MonoBehaviour
		{
			// Token: 0x06000208 RID: 520 RVA: 0x0001878C File Offset: 0x0001698C
			public void Awake()
			{
				this.defaultZ = base.transform.localScale.x;
			}

			// Token: 0x06000209 RID: 521 RVA: 0x000187A4 File Offset: 0x000169A4
			public void TestTrigger()
			{
				MenuPatch.Toggle(this.button);
			}

			// Token: 0x0600020A RID: 522 RVA: 0x000187B4 File Offset: 0x000169B4
			private void OnTriggerEnter(Collider collider)
			{
				if (collider == null)
				{
					return;
				}
				if (Time.frameCount >= MenuPatch.framePressCooldown + 20 && collider.gameObject.name == MenuPatch.referance.name)
				{
					base.transform.localScale = new Vector3(base.transform.localScale.x / 3f, base.transform.localScale.y, base.transform.localScale.z);
					MenuPatch.framePressCooldown = Time.frameCount;
					AssetLoader.Instance.PlayClick();
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
					MenuPatch.Toggle(this.button);
				}
			}

			// Token: 0x0600020B RID: 523 RVA: 0x0001888C File Offset: 0x00016A8C
			private void OnTriggerExit(Collider collider)
			{
				base.StartCoroutine(this.ResetYValue());
			}

			// Token: 0x0600020C RID: 524 RVA: 0x0001889B File Offset: 0x00016A9B
			private IEnumerator ResetYValue()
			{
				yield return new WaitForSeconds(0.65f);
				base.transform.localScale = new Vector3(this.defaultZ, base.transform.localScale.y, base.transform.localScale.z);
				yield break;
			}

			// Token: 0x04000161 RID: 353
			public MenuPatch.Button button;

			// Token: 0x04000162 RID: 354
			public float defaultZ;
		}
	}
}
