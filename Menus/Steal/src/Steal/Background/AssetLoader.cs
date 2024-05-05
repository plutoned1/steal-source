using System;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;

namespace Steal.Background
{
	// Token: 0x0200002E RID: 46
	internal class AssetLoader : MonoBehaviour
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00009B11 File Offset: 0x00007D11
		public static AssetLoader Instance
		{
			get
			{
				return AssetLoader.assetLoader;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00009B18 File Offset: 0x00007D18
		private void Awake()
		{
			AssetLoader.assetLoader = this;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00009B20 File Offset: 0x00007D20
		public void PlayClick()
		{
			if (this.source != null)
			{
				Object.Destroy(this.source);
			}
			this.source = base.gameObject.AddComponent<AudioSource>();
			this.source.clip = AssetLoader.click;
			this.source.loop = false;
			this.source.Play();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00009B80 File Offset: 0x00007D80
		public static Texture2D DownloadBackround(string imagestring)
		{
			byte[] array;
			using (WebClient webClient = new WebClient())
			{
				array = webClient.DownloadData(imagestring);
			}
			Texture2D texture2D = new Texture2D(200, 200);
			ImageConversion.LoadImage(texture2D, array);
			return texture2D;
		}

		// Token: 0x0400006F RID: 111
		private static Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Steal.Resources.click");

		// Token: 0x04000070 RID: 112
		private static AssetBundle bundle = AssetBundle.LoadFromStream(AssetLoader.str);

		// Token: 0x04000071 RID: 113
		public static AudioClip click = AssetLoader.bundle.LoadAsset("click") as AudioClip;

		// Token: 0x04000072 RID: 114
		public AudioSource source;

		// Token: 0x04000073 RID: 115
		private static AssetLoader assetLoader;
	}
}
