using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Steal.Background.Security
{
	// Token: 0x02000038 RID: 56
	public class AuthClient : MonoBehaviour
	{
		// Token: 0x060000DF RID: 223 RVA: 0x0000BD66 File Offset: 0x00009F66
		private static void MaybeGetNonce(LoginResult obj)
		{
			AuthClient._playFabId = obj.PlayFabId;
			AuthClient._sessionTicket = obj.SessionTicket;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000BD7E File Offset: 0x00009F7E
		private static void playfabErrorFGASA(PlayFabError error)
		{
			Debug.Log("Error my nega! " + error.ToString());
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000BD95 File Offset: 0x00009F95
		public static void asfasf(string ticket)
		{
			PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
			{
				SteamTicket = ticket,
				TitleId = PlayFabSettings.TitleId
			}, new Action<LoginResult>(AuthClient.MaybeGetNonce), new Action<PlayFabError>(AuthClient.playfabErrorFGASA), null, null);
		}

		// Token: 0x040000AA RID: 170
		private static string _sessionTicket;

		// Token: 0x040000AB RID: 171
		private static string _playFabId;
	}
}
