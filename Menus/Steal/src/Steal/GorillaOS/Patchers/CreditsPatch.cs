using System;
using GorillaNetworking;
using HarmonyLib;

namespace Steal.GorillaOS.Patchers
{
	// Token: 0x02000022 RID: 34
	[HarmonyPatch(typeof(GorillaComputer), "ProcessCreditsState", 0)]
	internal class CreditsPatch
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00007D7C File Offset: 0x00005F7C
		private static bool Prefix(GorillaKeyboardButton buttonPressed)
		{
			if (buttonPressed.characterString == "enter")
			{
				return false;
			}
			int num;
			if (int.TryParse(buttonPressed.characterString, out num) && num != 0)
			{
				GorillaOS.instance.UpadateTheme(num);
			}
			return true;
		}
	}
}
