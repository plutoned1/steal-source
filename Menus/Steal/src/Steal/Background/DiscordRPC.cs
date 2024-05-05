using System;
using System.IO;
using System.Reflection;

namespace Steal.Background
{
	// Token: 0x0200002F RID: 47
	internal class DiscordRPC
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x00009C18 File Offset: 0x00007E18
		public static void Init()
		{
			DiscordRPC.rpcAssembly = Assembly.Load(DiscordRPC.LoadEmbeddedResource("Steal.Resources.DiscordRPC.dll"));
			Type type = DiscordRPC.rpcAssembly.GetType("DiscordRPC.DiscordRpcClient");
			object obj = Activator.CreateInstance(type, new object[] { "1226755656346767380" });
			type.GetMethod("Initialize").Invoke(obj, null);
			Type type2 = DiscordRPC.rpcAssembly.GetType("DiscordRPC.RichPresence");
			object obj2 = Activator.CreateInstance(type2);
			type2.GetProperty("Details").SetValue(obj2, "Using steal.lol Cheat in Gorilla Tag!");
			type2.GetProperty("State").SetValue(obj2, "discord.gg/paste");
			Type type3 = DiscordRPC.rpcAssembly.GetType("DiscordRPC.Assets");
			object obj3 = Activator.CreateInstance(type3);
			type3.GetProperty("LargeImageKey").SetValue(obj3, "dimondresized");
			type3.GetProperty("LargeImageText").SetValue(obj3, "steal.lol");
			type2.GetProperty("Assets").SetValue(obj2, obj3);
			type.GetMethod("SetPresence").Invoke(obj, new object[] { obj2 });
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00009D24 File Offset: 0x00007F24
		private static byte[] LoadEmbeddedResource(string resourceName)
		{
			byte[] array;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			{
				if (manifestResourceStream == null)
				{
					throw new ArgumentException("Resource '" + resourceName + "' not found.");
				}
				using (MemoryStream memoryStream = new MemoryStream())
				{
					manifestResourceStream.CopyTo(memoryStream);
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x04000074 RID: 116
		private static Assembly rpcAssembly;
	}
}
