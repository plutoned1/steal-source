using System;
using GorillaNetworking;
using HarmonyLib;

namespace Steal.BetterBanMessage
{
	// Token: 0x02000027 RID: 39
	[HarmonyPatch(typeof(PlayFabAuthenticator), "ShowBanMessage", 0)]
	public class ShowBanMessage
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00008758 File Offset: 0x00006958
		private static bool Prefix(PlayFabAuthenticator.BanInfo banInfo)
		{
			try
			{
				if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
				{
					if (banInfo.BanExpirationTime != "Indefinite")
					{
						int num = (int)(DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalMinutes;
						int num2 = (int)(DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours;
						PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: <b>",
							banInfo.BanMessage,
							"</b>\nHOURS LEFT: <b>",
							((int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString(),
							"</b>\nMINUTES: <b>",
							OnPlayFabError.getMinutesLeft(num, num2),
							"</b>"
						}));
					}
					else
					{
						PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
					}
				}
				return false;
			}
			catch (Exception)
			{
			}
			return false;
		}
	}
}
