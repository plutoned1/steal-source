using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using BepInEx;
using Cinemachine;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Steal.Background;
using Steal.Background.Mods;
using Steal.Patchers;
using UnityEngine;

namespace Steal
{
	// Token: 0x02000005 RID: 5
	public class UI : MonoBehaviour
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002E14 File Offset: 0x00001014
		public void OnEnable()
		{
			if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "EXIST.txt")))
			{
				Environment.FailFast("bye");
			}
			new HttpClient();
			if (new HttpClient().GetStringAsync("https://bbc123f.github.io/killswitch.txt").Result.ToString().Contains("="))
			{
				Environment.FailFast("bye");
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002E80 File Offset: 0x00001080
		public void Start()
		{
			if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "EXIST.txt")))
			{
				Environment.FailFast("bye");
			}
			new HttpClient();
			if (new HttpClient().GetStringAsync("https://bbc123f.github.io/killswitch.txt").Result.ToString().Contains("="))
			{
				Environment.FailFast("bye");
			}
			UI.UILib.Init();
			try
			{
				if (UI.versionTexture == null)
				{
					UI.versionTexture = AssetLoader.DownloadBackround("https://tnuser.com/API/files/VersionPNG.png");
					UI.patchNotesTexture = AssetLoader.DownloadBackround("https://tnuser.com/API/files/pencil.png");
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			DiscordRPC.Init();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002F38 File Offset: 0x00001138
		public void Update()
		{
			if (UI.freecam)
			{
				Movement.AdvancedWASD(this.speed);
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002F4C File Offset: 0x0000114C
		public static void MakeFPC(bool refrence)
		{
			if (refrence)
			{
				if (GorillaTagger.Instance.thirdPersonCamera && GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.activeSelf)
				{
					GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.SetActive(false);
					return;
				}
			}
			else if (GorillaTagger.Instance.thirdPersonCamera && !GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.activeSelf)
			{
				GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.SetActive(true);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003004 File Offset: 0x00001204
		public static void PauseCam(bool refrence)
		{
			if (refrence)
			{
				if (GorillaTagger.Instance.thirdPersonCamera && GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.activeSelf)
				{
					GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
					return;
				}
			}
			else if (GorillaTagger.Instance.thirdPersonCamera && !GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.activeSelf)
			{
				GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000030FF File Offset: 0x000012FF
		public void OnGUI()
		{
			if (!this.GUIShown)
			{
				return;
			}
			this.window = GUI.Window(1, this.window, new GUI.WindowFunction(this.Window), "");
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003130 File Offset: 0x00001330
		public void Window(int id)
		{
			try
			{
				if (UI.myFont == null)
				{
					UI.myFont = Font.CreateDynamicFontFromOSFont("Gill Sans Nova", 18);
				}
				UI.UILib.SetTextures();
				UI.deltaTime += (Time.deltaTime - UI.deltaTime) * 0.1f;
				GUI.DrawTexture(new Rect(0f, 0f, this.window.width, this.window.height), UI.UILib.windowTexture, 0, false, 0f, GUI.color, Vector4.zero, new Vector4(6f, 6f, 6f, 6f));
				GUI.DrawTexture(new Rect(0f, 0f, 100f, this.window.height), UI.UILib.sidePannelTexture, 0, false, 0f, GUI.color, Vector4.zero, new Vector4(6f, 0f, 0f, 6f));
				GUIStyle guistyle = new GUIStyle(GUI.skin.label);
				guistyle.font = UI.myFont;
				GUI.Label(new Rect(10f, 5f, this.window.width, 30f), " steal.lol", guistyle);
				GUILayout.BeginArea(new Rect(7.5f, 30f, 100f, this.window.height));
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				for (int i = 0; i < this.pages.Length; i++)
				{
					GUILayout.Space(5f);
					string text = this.pages[i];
					if (UI.UILib.RoundedPageButton(text, i, new GUILayoutOption[] { GUILayout.Width(85f) }))
					{
						UI.Page = i;
						Debug.Log("Switched to page: " + text);
					}
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
				if (UI.Page == 0)
				{
					GUI.DrawTexture(new Rect(110f, 70f, 150f, 70f), UI.UILib.sidePannelTexture, 0, false, 0f, GUI.color, Vector4.zero, new Vector4(2.8f, 2.8f, 2.8f, 2.8f));
					GUI.DrawTexture(new Rect(110f, 150f, 380f, 200f), UI.UILib.sidePannelTexture, 0, false, 0f, GUI.color, Vector4.zero, new Vector4(2.8f, 2.8f, 2.8f, 2.8f));
					GUIStyle guistyle2 = new GUIStyle("label");
					guistyle2.font = UI.myFont;
					guistyle2.normal.textColor = Color.white;
					guistyle2.fontSize = 20;
					guistyle2.alignment = 3;
					GUIStyle guistyle3 = new GUIStyle("label");
					guistyle3.font = UI.myFont;
					guistyle3.normal.textColor = Color.gray * 1.2f;
					guistyle3.fontSize = 17;
					guistyle3.alignment = 3;
					GUI.Label(new Rect(115f, 2f, 200f, 45f), "Welcome Back!", guistyle2);
					GUI.Label(new Rect(115f, 25f, 200f, 45f), "Home", guistyle3);
					GUIStyle guistyle4 = new GUIStyle("label");
					guistyle4.font = UI.myFont;
					guistyle4.normal.textColor = Color.white;
					guistyle4.fontSize = 13;
					guistyle4.alignment = 3;
					GUI.Label(new Rect(142f, 68f, 100f, 30f), "Version", guistyle4);
					GUI.Label(new Rect(118f, 72f, 25f, 25f), UI.versionTexture);
					GUIStyle guistyle5 = new GUIStyle("label");
					guistyle5.font = UI.myFont;
					guistyle5.normal.textColor = Color.white;
					guistyle5.fontSize = 18;
					guistyle5.alignment = 3;
					GUI.Label(new Rect(123f, 95f, 100f, 30f), string.Concat(new string[]
					{
						2.ToString(),
						".",
						5.ToString(),
						".",
						1.ToString()
					}), guistyle5);
					GUI.Label(new Rect(142f, 152f, 100f, 30f), "Patch Notes", guistyle4);
					GUI.Label(new Rect(118f, 155f, 25f, 25f), UI.patchNotesTexture);
					GUI.Label(new Rect(125f, 185f, 200f, 150f), string.Concat(new string[]
					{
						"Fixed AntiBan", "\n", "Fixed Crash Stuff", "\n", "Changed Sutter Methods", "\n", "Added Glider Mods", "\n", "Updated UI", "\n",
						"Fixed Menu Being Invisible"
					}));
					this.roomStr = (this.shouldHideRoom ? GUI.PasswordField(new Rect(265f, 70f, 150f, 25f), this.roomStr, '⋆') : GUI.TextField(new Rect(265f, 70f, 150f, 25f), this.roomStr));
					if (UI.UILib.RoundedButton(new Rect(420f, 70f, 75f, 20f), "HIDE", Array.Empty<GUILayoutOption>()))
					{
						this.shouldHideRoom = !this.shouldHideRoom;
					}
					if (UI.UILib.RoundedButton(new Rect(265f, 98f, 150f, 20f), "Join Room", Array.Empty<GUILayoutOption>()))
					{
						this.roomStr = Regex.Replace(this.roomStr.ToUpper(), "[^a-zA-Z0-9]", "");
						PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(this.roomStr);
					}
					if (UI.UILib.RoundedButton(new Rect(265f, 120f, 150f, 20f), "Set Name", Array.Empty<GUILayoutOption>()))
					{
						this.roomStr = Regex.Replace(this.roomStr.ToUpper(), "[^a-zA-Z0-9]", "");
						PhotonNetwork.LocalPlayer.NickName = this.roomStr;
						PlayerPrefs.SetString("playerName", this.roomStr);
						GorillaComputer.instance.offlineVRRigNametagText.text = this.roomStr;
						GorillaTagger.Instance.offlineVRRig.playerName = this.roomStr;
						PlayerPrefs.Save();
					}
				}
				GUILayout.BeginArea(new Rect(115f, 30f, 370f, this.window.height - 50f));
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				switch (UI.Page)
				{
				case 1:
					this.searchString = GUILayout.TextField(this.searchString, Array.Empty<GUILayoutOption>());
					this.scroll[0] = GUILayout.BeginScrollView(this.scroll[0], Array.Empty<GUILayoutOption>());
					foreach (MenuPatch.Button button in MenuPatch.buttons)
					{
						if (button.buttonText.IndexOf(this.searchString, StringComparison.OrdinalIgnoreCase) >= 0 && button.Page != MenuPatch.Category.Config && button.Page != MenuPatch.Category.Base && UI.UILib.RoundedToggleButton(button.buttonText, button, Array.Empty<GUILayoutOption>()))
						{
							MenuPatch.Toggle(button);
						}
					}
					GUILayout.EndScrollView();
					break;
				case 2:
					this.scroll[0] = GUILayout.BeginScrollView(this.scroll[0], Array.Empty<GUILayoutOption>());
					foreach (MenuPatch.Button button2 in MenuPatch.buttons)
					{
						if (button2.Page == MenuPatch.Category.Room && UI.UILib.RoundedToggleButton(button2.buttonText, button2, Array.Empty<GUILayoutOption>()))
						{
							MenuPatch.Toggle(button2);
						}
					}
					GUILayout.Space(10f);
					if (PhotonNetwork.CurrentRoom != null)
					{
						foreach (Player player in PhotonNetwork.PlayerListOthers)
						{
							VRRig vrrig = GorillaGameManager.instance.FindPlayerVRRig(player);
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							if (!vrrig.mainSkin.material.name.Contains("fected"))
							{
								GUILayout.Label(UI.UILib.ApplyColorFilter(vrrig.mainSkin.material.color), new GUILayoutOption[]
								{
									GUILayout.Width(30f),
									GUILayout.Height(30f)
								});
							}
							else
							{
								if (UI.infectedTexture == null)
								{
									UI.infectedTexture = RoomManager.ConvertToTexture2D(vrrig.mainSkin.material.mainTexture);
								}
								GUILayout.Label(UI.infectedTexture, new GUILayoutOption[]
								{
									GUILayout.Width(30f),
									GUILayout.Height(30f)
								});
							}
							UI.UILib.PlayerButton(player.NickName, new GUILayoutOption[]
							{
								GUILayout.Width(120f),
								GUILayout.Height(30f)
							});
							if (UI.UILib.RoundedPlayerButton("Teleport", new GUILayoutOption[]
							{
								GUILayout.Width(90f),
								GUILayout.Height(30f)
							}))
							{
								TeleportationLib.Teleport(vrrig.transform.position);
							}
							if (PhotonNetwork.LocalPlayer.IsMasterClient)
							{
								if (!vrrig.mainSkin.material.name.Contains("fected"))
								{
									if (UI.UILib.RoundedPlayerButton("Tag", new GUILayoutOption[]
									{
										GUILayout.Width(90f),
										GUILayout.Height(30f)
									}))
									{
										PlayerMods.TagPlayer(player);
									}
								}
								else if (UI.UILib.RoundedPlayerButton("Untag", new GUILayoutOption[]
								{
									GUILayout.Width(90f),
									GUILayout.Height(30f)
								}))
								{
									PlayerMods.UnTagPlayer(player);
								}
							}
							GUILayout.EndHorizontal();
							GUILayout.Space(10f);
						}
					}
					else
					{
						GUILayout.Label("Please join a room!", Array.Empty<GUILayoutOption>());
					}
					GUILayout.EndScrollView();
					break;
				case 3:
					this.scroll[0] = GUILayout.BeginScrollView(this.scroll[0], Array.Empty<GUILayoutOption>());
					foreach (MenuPatch.Button button3 in MenuPatch.buttons)
					{
						if (button3.Page == MenuPatch.Category.Movement && UI.UILib.RoundedToggleButton(button3.buttonText, button3, Array.Empty<GUILayoutOption>()))
						{
							MenuPatch.Toggle(button3);
						}
					}
					GUILayout.EndScrollView();
					break;
				case 4:
					this.scroll[0] = GUILayout.BeginScrollView(this.scroll[0], Array.Empty<GUILayoutOption>());
					foreach (MenuPatch.Button button4 in MenuPatch.buttons)
					{
						if (button4.Page == MenuPatch.Category.Player && UI.UILib.RoundedToggleButton(button4.buttonText, button4, Array.Empty<GUILayoutOption>()))
						{
							MenuPatch.Toggle(button4);
						}
					}
					GUILayout.EndScrollView();
					break;
				case 5:
					this.scroll[0] = GUILayout.BeginScrollView(this.scroll[0], Array.Empty<GUILayoutOption>());
					foreach (MenuPatch.Button button5 in MenuPatch.buttons)
					{
						if (button5.Page == MenuPatch.Category.Visual && UI.UILib.RoundedToggleButton(button5.buttonText, button5, Array.Empty<GUILayoutOption>()))
						{
							MenuPatch.Toggle(button5);
						}
					}
					GUILayout.EndScrollView();
					break;
				case 6:
					this.scroll[0] = GUILayout.BeginScrollView(this.scroll[0], Array.Empty<GUILayoutOption>());
					foreach (MenuPatch.Button button6 in MenuPatch.buttons)
					{
						if (button6.Page == MenuPatch.Category.Exploits && UI.UILib.RoundedToggleButton(button6.buttonText, button6, Array.Empty<GUILayoutOption>()))
						{
							MenuPatch.Toggle(button6);
						}
					}
					GUILayout.EndScrollView();
					break;
				case 7:
					if (UI.UILib.RoundedButton("Freecam Mode", UI.freecam, Array.Empty<GUILayoutOption>()))
					{
						Movement.previousMousePosition = UnityInput.Current.mousePosition;
						UI.freecam = !UI.freecam;
					}
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label("Speed: " + this.speed.ToString(), Array.Empty<GUILayoutOption>());
					GUILayout.EndHorizontal();
					this.speed = GUILayout.HorizontalSlider(this.speed, 0.1f, 100f, Array.Empty<GUILayoutOption>());
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					if (UI.UILib.RoundedButton("Reset Speed", Array.Empty<GUILayoutOption>()))
					{
						this.speed = 10f;
					}
					GUILayout.EndHorizontal();
					GUILayout.Label("Camera Settings", Array.Empty<GUILayoutOption>());
					GUILayout.Label(string.Format("Camera FOV: {0}", (int)GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>().fieldOfView), Array.Empty<GUILayoutOption>());
					GUILayout.Label(string.Format("Current FOV: {0}", (int)UI.fov), Array.Empty<GUILayoutOption>());
					UI.fov = GUILayout.HorizontalSlider(UI.fov, 1f, 179f, Array.Empty<GUILayoutOption>());
					if (UI.UILib.RoundedButton("Set FOV", Array.Empty<GUILayoutOption>()))
					{
						GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>().fieldOfView = UI.fov;
						GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = UI.fov;
					}
					if (UI.UILib.RoundedButton("Reset FOV", Array.Empty<GUILayoutOption>()))
					{
						if (GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0) && GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).gameObject.activeSelf && GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).transform.GetChild(0) && GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).transform.GetChild(0).gameObject.activeSelf)
						{
							GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>().fieldOfView = 60f;
							GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60f;
						}
						UI.fov = 60f;
					}
					if (UI.UILib.RoundedButton("First Person Camera", Array.Empty<GUILayoutOption>()))
					{
						this.fpc = !this.fpc;
						UI.MakeFPC(this.fpc);
					}
					if (UI.UILib.RoundedButton("Pause Camera", Array.Empty<GUILayoutOption>()))
					{
						this.campause = !this.campause;
						UI.PauseCam(this.campause);
						ShowConsole.Log("Pause Camera : Is " + this.campause.ToString());
					}
					break;
				case 8:
					this.scroll[0] = GUILayout.BeginScrollView(this.scroll[0], Array.Empty<GUILayoutOption>());
					foreach (MenuPatch.Button button7 in MenuPatch.buttons)
					{
						if (button7.Page == MenuPatch.Category.Config && UI.UILib.RoundedToggleButton(button7.buttonText, button7, Array.Empty<GUILayoutOption>()))
						{
							MenuPatch.Toggle(button7);
						}
					}
					GUILayout.EndScrollView();
					break;
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
				GUI.DragWindow(new Rect(0f, 0f, 100000f, 100000f));
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				throw;
			}
		}

		// Token: 0x0400001E RID: 30
		private bool GUIShown = true;

		// Token: 0x0400001F RID: 31
		private bool shouldHideRoom;

		// Token: 0x04000020 RID: 32
		public Rect window = new Rect(10f, 10f, 500f, 400f);

		// Token: 0x04000021 RID: 33
		public Vector2[] scroll = new Vector2[30];

		// Token: 0x04000022 RID: 34
		public static int Page = 0;

		// Token: 0x04000023 RID: 35
		public static int Theme = 0;

		// Token: 0x04000024 RID: 36
		public static Texture2D infectedTexture = null;

		// Token: 0x04000025 RID: 37
		public static Texture2D versionTexture;

		// Token: 0x04000026 RID: 38
		public static Texture2D patchNotesTexture;

		// Token: 0x04000027 RID: 39
		private string[] pages = new string[] { "Home", "Search", "Room", "Movement", "Player", "Render", "Exploits", "Freecam", "Config" };

		// Token: 0x04000028 RID: 40
		private string roomStr = "text here";

		// Token: 0x04000029 RID: 41
		private string searchString = "Query to search";

		// Token: 0x0400002A RID: 42
		public static float deltaTime;

		// Token: 0x0400002B RID: 43
		public static float fov = 60f;

		// Token: 0x0400002C RID: 44
		public static Font myFont;

		// Token: 0x0400002D RID: 45
		private float speed = 10f;

		// Token: 0x0400002E RID: 46
		private bool campause;

		// Token: 0x0400002F RID: 47
		public static bool freecam;

		// Token: 0x04000030 RID: 48
		private bool fpc;

		// Token: 0x02000047 RID: 71
		private static class UILib
		{
			// Token: 0x060001E0 RID: 480 RVA: 0x000177B8 File Offset: 0x000159B8
			public static void Init()
			{
				if (UI.Theme == 0)
				{
					UI.UILib.pageButtonHoverTexture = UI.UILib.ApplyColorFilter(new Color32(34, 115, 179, byte.MaxValue));
					UI.UILib.pageButtonTexture = UI.UILib.ApplyColorFilter(new Color32(39, 132, 204, byte.MaxValue));
					UI.UILib.buttonTexture = UI.UILib.ApplyColorFilter(new Color32(27, 27, 27, byte.MaxValue));
					UI.UILib.buttonHoverTexture = UI.UILib.ApplyColorFilter(new Color32(35, 35, 35, byte.MaxValue));
					UI.UILib.buttonClickTexture = UI.UILib.ApplyColorFilter(new Color32(44, 44, 44, byte.MaxValue));
					UI.UILib.windowTexture = UI.UILib.ApplyColorFilter(new Color32(17, 17, 17, byte.MaxValue));
					UI.UILib.sidePannelTexture = UI.UILib.ApplyColorFilter(new Color32(37, 37, 37, byte.MaxValue));
					UI.UILib.boxTexture = UI.UILib.ApplyColorFilter(new Color32(0, 0, 0, byte.MaxValue));
					UI.UILib.TextBox = UI.UILib.CreateRoundedTexture2(12, new Color32(35, 35, 35, byte.MaxValue));
				}
			}

			// Token: 0x060001E1 RID: 481 RVA: 0x000178F0 File Offset: 0x00015AF0
			public static void SetTextures()
			{
				GUI.skin.label.richText = true;
				GUI.skin.button.richText = true;
				GUI.skin.window.richText = true;
				GUI.skin.textField.richText = true;
				GUI.skin.box.richText = true;
				GUI.skin.window.border.bottom = 5;
				GUI.skin.window.border.left = 5;
				GUI.skin.window.border.top = 5;
				GUI.skin.window.border.right = 5;
				GUI.skin.window.active.background = null;
				GUI.skin.window.normal.background = null;
				GUI.skin.window.hover.background = null;
				GUI.skin.window.focused.background = null;
				GUI.skin.window.onFocused.background = null;
				GUI.skin.window.onActive.background = null;
				GUI.skin.window.onHover.background = null;
				GUI.skin.window.onNormal.background = null;
				GUI.skin.button.active.background = UI.UILib.buttonClickTexture;
				GUI.skin.button.normal.background = UI.UILib.buttonHoverTexture;
				GUI.skin.button.hover.background = UI.UILib.buttonTexture;
				GUI.skin.button.onActive.background = UI.UILib.buttonClickTexture;
				GUI.skin.button.onHover.background = UI.UILib.buttonHoverTexture;
				GUI.skin.button.onNormal.background = UI.UILib.buttonTexture;
				GUI.skin.box.active.background = UI.UILib.boxTexture;
				GUI.skin.box.normal.background = UI.UILib.boxTexture;
				GUI.skin.box.hover.background = UI.UILib.boxTexture;
				GUI.skin.box.onActive.background = UI.UILib.boxTexture;
				GUI.skin.box.onHover.background = UI.UILib.boxTexture;
				GUI.skin.box.onNormal.background = UI.UILib.boxTexture;
				GUI.skin.textField.active.background = UI.UILib.TextBox;
				GUI.skin.textField.normal.background = UI.UILib.TextBox;
				GUI.skin.textField.hover.background = UI.UILib.TextBox;
				GUI.skin.textField.focused.background = UI.UILib.TextBox;
				GUI.skin.textField.onFocused.background = UI.UILib.TextBox;
				GUI.skin.textField.onActive.background = UI.UILib.TextBox;
				GUI.skin.textField.onHover.background = UI.UILib.TextBox;
				GUI.skin.textField.onNormal.background = UI.UILib.TextBox;
				GUI.skin.horizontalSlider.active.background = UI.UILib.buttonTexture;
				GUI.skin.horizontalSlider.normal.background = UI.UILib.buttonTexture;
				GUI.skin.horizontalSlider.hover.background = UI.UILib.buttonTexture;
				GUI.skin.horizontalSlider.focused.background = UI.UILib.buttonTexture;
				GUI.skin.horizontalSlider.onFocused.background = UI.UILib.buttonTexture;
				GUI.skin.horizontalSlider.onActive.background = UI.UILib.buttonTexture;
				GUI.skin.horizontalSlider.onHover.background = UI.UILib.buttonTexture;
				GUI.skin.horizontalSlider.onNormal.background = UI.UILib.buttonTexture;
				GUI.skin.verticalScrollbar.border = new RectOffset(0, 0, 0, 0);
				GUI.skin.verticalScrollbar.fixedWidth = 0f;
				GUI.skin.verticalScrollbar.fixedHeight = 0f;
				GUI.skin.verticalScrollbarThumb.fixedHeight = 0f;
				GUI.skin.verticalScrollbarThumb.fixedWidth = 5f;
			}

			// Token: 0x060001E2 RID: 482 RVA: 0x00017D70 File Offset: 0x00015F70
			public static Texture2D ApplyColorFilter(Color color)
			{
				Texture2D texture2D = new Texture2D(30, 30);
				Color[] array = new Color[900];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = color;
				}
				texture2D.SetPixels(array);
				texture2D.Apply();
				return texture2D;
			}

			// Token: 0x060001E3 RID: 483 RVA: 0x00017DB8 File Offset: 0x00015FB8
			private static Texture2D CreateRoundedTexture2(int size, Color color)
			{
				Texture2D texture2D = new Texture2D(size, size);
				Color[] array = new Color[size * size];
				for (int i = 0; i < size * size; i++)
				{
					int num = i % size;
					int num2 = i / size;
					if (Mathf.Sqrt((float)((num - size / 2) * (num - size / 2) + (num2 - size / 2) * (num2 - size / 2))) <= (float)(size / 2))
					{
						array[i] = color;
					}
					else
					{
						array[i] = Color.clear;
					}
				}
				texture2D.SetPixels(array);
				texture2D.Apply();
				return texture2D;
			}

			// Token: 0x060001E4 RID: 484 RVA: 0x00017E38 File Offset: 0x00016038
			public static Texture2D CreateRoundedTexture(int size, Color color)
			{
				Texture2D texture2D = new Texture2D(size, size);
				Color[] array = new Color[size * size];
				float num = (float)size / 2f;
				float num2 = num * num;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						float num3 = ((float)i - num) * ((float)i - num) + ((float)j - num) * ((float)j - num);
						if (num3 <= num2)
						{
							float num4 = 1f - num3 / num2;
							array[j * size + i] = new Color(color.r, color.g, color.b, num4);
						}
						else
						{
							array[j * size + i] = Color.clear;
						}
					}
				}
				texture2D.SetPixels(array);
				texture2D.Apply();
				return texture2D;
			}

			// Token: 0x060001E5 RID: 485 RVA: 0x00017EF8 File Offset: 0x000160F8
			public static bool RoundedToggleButton(string content, MenuPatch.Button button, params GUILayoutOption[] options)
			{
				Texture2D texture2D = (button.Enabled ? UI.UILib.buttonClickTexture : UI.UILib.buttonTexture);
				Rect rect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);
				if (rect.Contains(Event.current.mousePosition))
				{
					texture2D = UI.UILib.buttonHoverTexture;
				}
				if (rect.Contains(Event.current.mousePosition) && Event.current.type == null)
				{
					texture2D = UI.UILib.buttonClickTexture;
					return true;
				}
				string text = button.buttonText;
				if (button.Page == MenuPatch.Category.Config)
				{
					if (button.doesHaveMultiplier)
					{
						text = button.buttonText + "[" + button.multiplier().ToString() + "]";
					}
					else if (button.doesHaveStringer)
					{
						text = button.buttonText + "[" + button.stringFunc() + "]";
					}
					else
					{
						text = button.buttonText;
					}
				}
				UI.UILib.DrawTexture(rect, texture2D, 6, default(Vector4));
				UI.UILib.DrawText(new Rect(rect.x, rect.y - 3f, rect.width, 25f), text, 12, Color.white, 0, true, true);
				return false;
			}

			// Token: 0x060001E6 RID: 486 RVA: 0x00018030 File Offset: 0x00016230
			public static bool RoundedPlayerButton(string content, params GUILayoutOption[] options)
			{
				Texture2D texture2D = UI.UILib.buttonTexture;
				Rect rect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);
				if (rect.Contains(Event.current.mousePosition))
				{
					texture2D = UI.UILib.buttonHoverTexture;
				}
				if (rect.Contains(Event.current.mousePosition) && Event.current.type == null)
				{
					texture2D = UI.UILib.buttonClickTexture;
					return true;
				}
				UI.UILib.DrawTexture(rect, texture2D, 6, default(Vector4));
				UI.UILib.DrawText(new Rect(rect.x, rect.y, rect.width, 25f), content, 12, Color.white, 0, true, true);
				return false;
			}

			// Token: 0x060001E7 RID: 487 RVA: 0x000180DC File Offset: 0x000162DC
			public static bool RoundedButton(string content, params GUILayoutOption[] options)
			{
				Texture2D texture2D = UI.UILib.buttonTexture;
				Rect rect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);
				if (rect.Contains(Event.current.mousePosition))
				{
					texture2D = UI.UILib.buttonHoverTexture;
				}
				if (rect.Contains(Event.current.mousePosition) && Event.current.type == null)
				{
					texture2D = UI.UILib.buttonClickTexture;
					return true;
				}
				UI.UILib.DrawTexture(rect, texture2D, 6, default(Vector4));
				UI.UILib.DrawText(new Rect(rect.x, rect.y - 3f, rect.width, 25f), content, 12, Color.white, 0, true, true);
				return false;
			}

			// Token: 0x060001E8 RID: 488 RVA: 0x00018190 File Offset: 0x00016390
			public static bool RoundedButton(string content, bool refrence, params GUILayoutOption[] options)
			{
				Texture2D texture2D = UI.UILib.buttonTexture;
				Rect rect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);
				if (rect.Contains(Event.current.mousePosition))
				{
					texture2D = UI.UILib.buttonHoverTexture;
				}
				if (rect.Contains(Event.current.mousePosition) && Event.current.type == null)
				{
					texture2D = UI.UILib.buttonClickTexture;
					return true;
				}
				if (refrence)
				{
					texture2D = UI.UILib.buttonClickTexture;
				}
				UI.UILib.DrawTexture(rect, texture2D, 6, default(Vector4));
				UI.UILib.DrawText(new Rect(rect.x, rect.y - 3f, rect.width, 25f), content, 12, Color.white, 0, true, true);
				return false;
			}

			// Token: 0x060001E9 RID: 489 RVA: 0x0001824C File Offset: 0x0001644C
			public static bool RoundedButton(Rect rect, string content, params GUILayoutOption[] options)
			{
				Texture2D texture2D = UI.UILib.buttonTexture;
				if (rect.Contains(Event.current.mousePosition))
				{
					texture2D = UI.UILib.buttonHoverTexture;
				}
				if (rect.Contains(Event.current.mousePosition) && Event.current.type == null)
				{
					texture2D = UI.UILib.buttonClickTexture;
					return true;
				}
				UI.UILib.DrawTexture(rect, texture2D, 6, default(Vector4));
				UI.UILib.DrawText(new Rect(rect.x, rect.y - 3f, rect.width, 25f), content, 12, Color.white, 0, true, true);
				return false;
			}

			// Token: 0x060001EA RID: 490 RVA: 0x000182E8 File Offset: 0x000164E8
			public static bool PlayerButton(string content, params GUILayoutOption[] options)
			{
				Texture2D texture2D = UI.UILib.buttonTexture;
				Rect rect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);
				UI.UILib.DrawText(new Rect(rect.x, rect.y - 3f, rect.width, 25f), content, 12, Color.white, 0, true, true);
				return false;
			}

			// Token: 0x060001EB RID: 491 RVA: 0x00018348 File Offset: 0x00016548
			public static void DrawText(Rect rect, string text, int fontSize = 12, Color textColor = default(Color), FontStyle fontStyle = 0, bool centerX = false, bool centerY = true)
			{
				GUIStyle guistyle = new GUIStyle(GUI.skin.label);
				guistyle.fontSize = fontSize;
				guistyle.font = UI.myFont;
				guistyle.normal.textColor = textColor;
				float num = (centerX ? (rect.x + rect.width / 2f - guistyle.CalcSize(new GUIContent(text)).x / 2f) : rect.x);
				float num2 = (centerY ? (rect.y + rect.height / 2f - guistyle.CalcSize(new GUIContent(text)).y / 2f) : rect.y);
				GUI.Label(new Rect(num, num2, rect.width, rect.height), new GUIContent(text), guistyle);
			}

			// Token: 0x060001EC RID: 492 RVA: 0x00018418 File Offset: 0x00016618
			public static bool RoundedPageButton(string content, int i, params GUILayoutOption[] options)
			{
				if (UI.Page == i)
				{
					Texture2D texture2D = UI.UILib.pageButtonTexture;
					UI.UILib.pageButtonRect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);
					if (UI.UILib.pageButtonRect.Contains(Event.current.mousePosition))
					{
						texture2D = UI.UILib.pageButtonHoverTexture;
					}
					if (UI.UILib.pageButtonRect.Contains(Event.current.mousePosition) && Event.current.type == null)
					{
						return true;
					}
					UI.UILib.DrawTexture(UI.UILib.pageButtonRect, texture2D, 8, default(Vector4));
					UI.UILib.DrawText(new Rect(UI.UILib.pageButtonRect.x, UI.UILib.pageButtonRect.y - 3f, UI.UILib.pageButtonRect.width, 25f), content, 12, Color.white, 1, true, true);
					return false;
				}
				else
				{
					Texture2D texture2D2 = UI.UILib.buttonTexture;
					UI.UILib.pageButtonRect = GUILayoutUtility.GetRect(new GUIContent(content), GUI.skin.button, options);
					if (UI.UILib.pageButtonRect.Contains(Event.current.mousePosition))
					{
						texture2D2 = UI.UILib.buttonHoverTexture;
					}
					if (UI.UILib.pageButtonRect.Contains(Event.current.mousePosition) && Event.current.type == null)
					{
						texture2D2 = UI.UILib.buttonClickTexture;
						return true;
					}
					UI.UILib.DrawTexture(UI.UILib.pageButtonRect, texture2D2, 8, default(Vector4));
					UI.UILib.DrawText(new Rect(UI.UILib.pageButtonRect.x, UI.UILib.pageButtonRect.y - 3f, UI.UILib.pageButtonRect.width, 25f), content, 12, Color.white, 1, true, true);
					return false;
				}
			}

			// Token: 0x060001ED RID: 493 RVA: 0x000185A1 File Offset: 0x000167A1
			private static void DrawTexture(Rect rect, Texture2D texture, int borderRadius, Vector4 borderRadius4 = default(Vector4))
			{
				if (borderRadius4 == Vector4.zero)
				{
					borderRadius4..ctor((float)borderRadius, (float)borderRadius, (float)borderRadius, (float)borderRadius);
				}
				GUI.DrawTexture(rect, texture, 0, false, 0f, GUI.color, Vector4.zero, borderRadius4);
			}

			// Token: 0x0400013F RID: 319
			public static Texture2D sidePannelTexture;

			// Token: 0x04000140 RID: 320
			public static Texture2D TextBox = new Texture2D(2, 2);

			// Token: 0x04000141 RID: 321
			public static Texture2D pageButtonTexture = new Texture2D(2, 2);

			// Token: 0x04000142 RID: 322
			public static Texture2D pageButtonHoverTexture = new Texture2D(2, 2);

			// Token: 0x04000143 RID: 323
			public static Texture2D buttonTexture = new Texture2D(2, 2);

			// Token: 0x04000144 RID: 324
			public static Texture2D buttonHoverTexture = new Texture2D(2, 2);

			// Token: 0x04000145 RID: 325
			public static Texture2D buttonClickTexture = new Texture2D(2, 2);

			// Token: 0x04000146 RID: 326
			public static Texture2D windowTexture = new Texture2D(2, 2);

			// Token: 0x04000147 RID: 327
			public static Texture2D boxTexture = new Texture2D(2, 2);

			// Token: 0x04000148 RID: 328
			private static Rect pageButtonRect;
		}
	}
}
