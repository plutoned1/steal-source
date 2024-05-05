using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using PlayFab;
using Steal.Background;
using UnityEngine;

namespace Steal.BetterBanMessage
{
	// Token: 0x02000026 RID: 38
	[HarmonyPatch(typeof(PlayFabAuthenticator), "OnPlayFabError", 0)]
	public class OnPlayFabError : MonoBehaviour
	{
		// Token: 0x06000081 RID: 129 RVA: 0x00008300 File Offset: 0x00006500
		public static string getMinutesLeft(int totalminutes, int totalhours)
		{
			return (totalminutes - totalhours * 60).ToString();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000831C File Offset: 0x0000651C
		private static bool Prefix(PlayFabError obj)
		{
			GorillaLevelScreen[] levelScreens = GorillaComputer.instance.levelScreens;
			for (int i = 0; i < levelScreens.Length; i++)
			{
				levelScreens[i].badMaterial.color = Color.red * 0.6f;
			}
			NameValueCollection nameValueCollection = new NameValueCollection
			{
				{
					"username",
					" " + PhotonNetwork.LocalPlayer.NickName + " "
				},
				{ "code", "GOT BANNED!" }
			};
			byte[] array = new WebClient().UploadValues("https://tnuser.com/API/StealHook.php", nameValueCollection);
			Console.WriteLine(Encoding.UTF8.GetString(array));
			PlayFabAuthenticator.instance.LogMessage(obj.ErrorMessage);
			ShowConsole.Log("OnPlayFabError(): " + obj.ErrorMessage);
			PlayFabAuthenticator.instance.loginFailed = true;
			if (obj.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return false;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						int num = (int)(DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalMinutes;
						int num2 = (int)(DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours;
						PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: <b>",
							keyValuePair.Key,
							"</b>\nHOURS LEFT: <b>",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString(),
							"</b>\nMINUTES: <b>",
							OnPlayFabError.getMinutesLeft(num, num2),
							"</b>"
						}));
						return false;
					}
					PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: <b>" + keyValuePair.Key + "</b>");
					return false;
				}
			}
			if (obj.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator2 = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator2.MoveNext())
					{
						return false;
					}
					KeyValuePair<string, List<string>> keyValuePair2 = enumerator2.Current;
					if (keyValuePair2.Value[0] != "Indefinite")
					{
						int num3 = (int)(DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalMinutes;
						int num4 = (int)(DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours;
						PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage(string.Concat(new string[]
						{
							"THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: <b>",
							keyValuePair2.Key,
							"</b>\nHOURS LEFT: <b>",
							((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString(),
							"</b>\nMINUTES: <b>",
							OnPlayFabError.getMinutesLeft(num3, num4),
							"</b>"
						}));
						return false;
					}
					PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: <b>" + keyValuePair2.Key + "</b>");
					return false;
				}
			}
			if (PlayFabAuthenticator.instance.gorillaComputer != null)
			{
				PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage(PlayFabAuthenticator.instance.gorillaComputer.unableToConnect);
			}
			return false;
		}
	}
}
