using System;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;

namespace Steal.GorillaOS.Patchers
{
	// Token: 0x02000025 RID: 37
	[HarmonyPatch(typeof(GorillaComputer), "UpdateScreen", 0)]
	internal class UpdateScreenPatch
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00008044 File Offset: 0x00006244
		private static void Postfix()
		{
			UpdateScreenPatch.CurrentStatus = "IDLE";
			GorillaComputer.ComputerState currentState = GorillaComputer.instance.currentState;
			if (currentState <= 5)
			{
				if (currentState == null)
				{
					GorillaComputer.instance.screenText.Text = string.Concat(new string[]
					{
						"Welcome ",
						PhotonNetwork.NickName,
						"!\n\n",
						PhotonNetwork.CountOfPlayers.ToString(),
						" PLAYERS ONLINE\n\n",
						PhotonNetwork.CountOfPlayersInRooms.ToString(),
						" PLAYERS IN ROOMS\n\nPRESS ANY KEY TO CLOSE"
					});
					return;
				}
				if (currentState != 5)
				{
					return;
				}
				GorillaComputer.instance.screenText.Text = "ROOM   ::   " + UpdateScreenPatch.CurrentStatus + "\n\nPRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE. PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.\n\nCURRENT ROOM: ";
				if (PhotonNetwork.InRoom)
				{
					GorillaText screenText = GorillaComputer.instance.screenText;
					screenText.Text += PhotonNetwork.CurrentRoom.Name;
					GorillaText screenText2 = GorillaComputer.instance.screenText;
					screenText2.Text = screenText2.Text + "\n\nPLAYERS IN ROOM: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
				}
				else
				{
					GorillaText screenText3 = GorillaComputer.instance.screenText;
					screenText3.Text += "-NOT IN ROOM-";
					GorillaText screenText4 = GorillaComputer.instance.screenText;
					screenText4.Text = screenText4.Text + "\n\nPLAYERS IN ROOMS: " + PhotonNetwork.CountOfPlayersInRooms.ToString();
				}
				GorillaText screenText5 = GorillaComputer.instance.screenText;
				screenText5.Text = screenText5.Text + "\n\nROOM TO JOIN: " + GorillaComputer.instance.roomToJoin;
				if (GorillaComputer.instance.roomFull)
				{
					UpdateScreenPatch.CurrentStatus = "ROOM " + GorillaComputer.instance.roomToJoin + " IS FULL. JOIN ROOM FAILED.";
					GorillaOS.instance.Reload();
					return;
				}
				if (GorillaComputer.instance.roomNotAllowed)
				{
					UpdateScreenPatch.CurrentStatus = "CANNOT JOIN ROOM TYPE FROM HERE.";
					return;
				}
			}
			else if (currentState != 10)
			{
				if (currentState == 14)
				{
					GorillaComputer.instance.screenText.Text = "LOADING <color=red>GORILLA OS</color>...";
					return;
				}
				if (currentState != 15)
				{
					return;
				}
				Traverse.Create(GorillaComputer.instance).Field("displaySupport").SetValue(true);
				GorillaComputer.instance.screenText.Text = string.Concat(new string[]
				{
					"MODS   ::   " + UpdateScreenPatch.CurrentStatus + "\n",
					"MODUALS:\n",
					GorillaOS.list
				});
				return;
			}
			else
			{
				GorillaComputer.instance.screenText.Text = "THEMES   ::   " + UpdateScreenPatch.CurrentStatus + "\n\n1. GRAY\n2. DARK PURPLE\n3. CLEAR\n4. DEFAULT";
			}
		}

		// Token: 0x04000051 RID: 81
		public static string CurrentStatus = "IDLE";
	}
}
