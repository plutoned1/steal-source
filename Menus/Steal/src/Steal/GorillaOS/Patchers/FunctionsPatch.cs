using System;
using GorillaNetworking;
using HarmonyLib;

namespace Steal.GorillaOS.Patchers
{
	// Token: 0x02000023 RID: 35
	[HarmonyPatch(typeof(GorillaComputer), "UpdateFunctionScreen", 0)]
	public class FunctionsPatch
	{
		// Token: 0x06000079 RID: 121 RVA: 0x00007DC4 File Offset: 0x00005FC4
		private static bool Prefix()
		{
			string text = string.Concat(new string[]
			{
				((GorillaComputer.instance.currentState == 5) ? ">" : "") + " ROOM\n",
				((GorillaComputer.instance.currentState == 2) ? ">" : "") + " NAME\n",
				((GorillaComputer.instance.currentState == 1) ? ">" : "") + " COLOR\n",
				((GorillaComputer.instance.currentState == 3) ? ">" : "") + " TURN\n",
				((GorillaComputer.instance.currentState == 4) ? ">" : "") + " MIC\n",
				((GorillaComputer.instance.currentState == 6) ? ">" : "") + " QUEUE\n",
				((GorillaComputer.instance.currentState == 7) ? ">" : "") + " GROUP\n",
				((GorillaComputer.instance.currentState == 8) ? ">" : "") + " AUDIO\n",
				((GorillaComputer.instance.currentState == 11) ? ">" : "") + " ITEMS\n",
				((GorillaComputer.instance.currentState == 10) ? ">" : "") + " THEME\n",
				((GorillaComputer.instance.currentState == 15) ? ">" : "") + " MODS\n"
			});
			GorillaComputer.instance.functionSelectText.Text = text;
			return false;
		}
	}
}
