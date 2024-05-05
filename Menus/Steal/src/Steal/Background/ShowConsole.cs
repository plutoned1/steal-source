using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

namespace Steal.Background
{
	// Token: 0x02000035 RID: 53
	internal class ShowConsole : MonoBehaviour
	{
		// Token: 0x060000D0 RID: 208
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		// Token: 0x060000D1 RID: 209
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		// Token: 0x060000D2 RID: 210
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AllocConsole();

		// Token: 0x060000D3 RID: 211
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeConsole();

		// Token: 0x060000D4 RID: 212 RVA: 0x0000BB00 File Offset: 0x00009D00
		private async void Awake()
		{
			ShowConsole.AllocConsole();
			this.console = ShowConsole.GetConsoleWindow();
			ShowConsole.ShowWindow(this.console, 5);
			this.writer = new StreamWriter(Console.OpenStandardOutput());
			this.writer.AutoFlush = true;
			Console.Title = "STEAL.LOL ON TOP";
			Console.SetOut(this.writer);
			this.Logo();
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000BB38 File Offset: 0x00009D38
		public void Logo()
		{
			ShowConsole.Log("  .     '     ,\r\n    _________\r\n _ /_|_____|_\\ _\r\n   '. \\   / .'\r\n     '.\\ /.'\r\n       '.'");
			try
			{
				VersionJSON versionJSON = JsonConvert.DeserializeObject<VersionJSON[]>(new WebClient().DownloadString("https://tnuser.com/API/files/StealVersion.json"))[0];
				if (int.Parse(versionJSON.Revisions) > 1 || int.Parse(versionJSON.Major) > 2 || int.Parse(versionJSON.Minor) > 5)
				{
					ShowConsole.Log(string.Concat(new string[]
					{
						"Steal.lol Version: ",
						2.ToString(),
						".",
						5.ToString(),
						".",
						1.ToString(),
						" Update Available: TRUE"
					}));
				}
				else
				{
					ShowConsole.Log(string.Concat(new string[]
					{
						"Steal.lol Version: ",
						2.ToString(),
						".",
						5.ToString(),
						".",
						1.ToString(),
						" Update Available: FALSE"
					}));
				}
			}
			catch (Exception ex)
			{
				ShowConsole.Log(ex.ToString());
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000BC58 File Offset: 0x00009E58
		public static void Log(object message)
		{
			Console.WriteLine(message);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000BC60 File Offset: 0x00009E60
		public static void LogERR(object message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000BC74 File Offset: 0x00009E74
		public static void LogError(object message)
		{
		}

		// Token: 0x040000A3 RID: 163
		private IntPtr console;

		// Token: 0x040000A4 RID: 164
		private StreamWriter writer;
	}
}
