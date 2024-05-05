using System;
using GorillaNetworking;
using HarmonyLib;

namespace Steal.GorillaOS.Patchers
{
	// Token: 0x02000024 RID: 36
	[HarmonyPatch(typeof(GorillaComputer), "ProcessSupportState", 0)]
	public class SupportPatch
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00007FB8 File Offset: 0x000061B8
		private static bool Prefix(GorillaKeyboardButton buttonPressed)
		{
			SupportPatch.focusedModual = 1;
			int num;
			if (int.TryParse(buttonPressed.characterString, out num) && num != 0)
			{
				SupportPatch.focusedModual = num;
			}
			if (buttonPressed.characterString == "enter")
			{
				GorillaOS.Moduals.ToArray()[SupportPatch.focusedModual - 1].enabled = !GorillaOS.Moduals.ToArray()[SupportPatch.focusedModual - 1].enabled;
			}
			GorillaOS.instance.Refresh();
			return true;
		}

		// Token: 0x04000050 RID: 80
		public static int focusedModual = 1;
	}
}
