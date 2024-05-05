using System;
using UnityEngine;

namespace Steal.Background
{
	// Token: 0x02000036 RID: 54
	internal class UIAlerts : MonoBehaviour
	{
		// Token: 0x060000DA RID: 218 RVA: 0x0000BC7E File Offset: 0x00009E7E
		public static void SendAlert(string alert)
		{
			UIAlerts.Alert = alert;
			UIAlerts.ShowAlert = true;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000BC8C File Offset: 0x00009E8C
		public void OnGUI()
		{
			if (UIAlerts.ShowAlert)
			{
				GUILayout.Window(100, new Rect((float)(Screen.width / 2), (float)(Screen.height / 2), 230f, 100f), new GUI.WindowFunction(this.Window), "ALERT", Array.Empty<GUILayoutOption>());
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000BCE0 File Offset: 0x00009EE0
		public void Window(int id)
		{
			if (UIAlerts.ShowAlert)
			{
				GUILayout.Space(10f);
				GUILayout.Label(UIAlerts.Alert, Array.Empty<GUILayoutOption>());
				GUILayout.Space(15f);
				if (GUILayout.Button("Close", Array.Empty<GUILayoutOption>()))
				{
					UIAlerts.ShowAlert = false;
				}
				GUI.DragWindow(new Rect(0f, 0f, 100000f, 10000f));
			}
		}

		// Token: 0x040000A5 RID: 165
		public static bool ShowAlert = false;

		// Token: 0x040000A6 RID: 166
		public static string Alert = "";
	}
}
