using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Steal.Background.Security.Auth
{
	// Token: 0x0200003B RID: 59
	public class auth : MonoBehaviour
	{
		// Token: 0x060000EC RID: 236 RVA: 0x0000C150 File Offset: 0x0000A350
		public void license2(string username)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "steal", "wid.txt"));
			string text2 = encryption.sha256(encryption.iv_key());
			encryption.encrypt(text, this.enckey, text2);
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("license"));
			nameValueCollection["key"] = encryption.encrypt(username, this.enckey, text2);
			nameValueCollection["hwid"] = text;
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text2;
			string text3 = auth.req(nameValueCollection);
			text3 = encryption.decrypt(text3, this.enckey, text2);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text3);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				this.load_user_data(response_structure.info);
			}
			MenuPatch.isAllowed = true;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000C29D File Offset: 0x0000A49D
		private IEnumerator Error_ApplicationNotSetupCorrectly()
		{
			ShowConsole.LogError("Application is not setup correctly. Please make sure you entered the correct application name, secret, ownerID and version and try again.");
			yield return new WaitForSeconds(3f);
			Environment.FailFast("bye");
			Application.Quit();
			yield break;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000C2A8 File Offset: 0x0000A4A8
		public auth(string name, string ownerid, string secret, string version)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(ownerid) || string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(version))
			{
				base.StartCoroutine(this.Error_ApplicationNotSetupCorrectly());
			}
			this.name = name;
			this.ownerid = ownerid;
			this.secret = secret;
			this.version = version;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000C337 File Offset: 0x0000A537
		private IEnumerator Error_ApplicatonNotFound()
		{
			ShowConsole.LogError("Application was not found. Please check your application information.");
			yield return new WaitForSeconds(3f);
			Environment.FailFast("bye");
			Application.Quit();
			yield break;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000C340 File Offset: 0x0000A540
		public void init()
		{
			this.enckey = encryption.sha256(encryption.iv_key());
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("init"));
			nameValueCollection["ver"] = encryption.encrypt(this.version, this.secret, text);
			nameValueCollection["hash"] = null;
			nameValueCollection["enckey"] = encryption.encrypt(this.enckey, this.secret, text);
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			if (text2 == "KeyAuth_Invalid")
			{
				base.StartCoroutine(this.Error_ApplicatonNotFound());
			}
			text2 = encryption.decrypt(text2, this.secret, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				this.load_app_data(response_structure.appinfo);
				this.sessionid = response_structure.sessionid;
				this.initialized = true;
				return;
			}
			if (response_structure.message == "invalidver")
			{
				this.app_data.downloadLink = response_structure.download;
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000C4AB File Offset: 0x0000A6AB
		private IEnumerator Error_PleaseInitializeFirst()
		{
			ShowConsole.LogError("Please Initialize First. Put KeyAuthApp.Init(); on the start function of your login scene.");
			yield return new WaitForSeconds(3f);
			Environment.FailFast("bye");
			Application.Quit();
			yield break;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000C4B4 File Offset: 0x0000A6B4
		public void login(string username, string pass)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("login"));
			nameValueCollection["username"] = encryption.encrypt(username, this.enckey, text);
			nameValueCollection["pass"] = encryption.encrypt(pass, this.enckey, text);
			nameValueCollection["hwid"] = encryption.encrypt(deviceUniqueIdentifier, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				this.load_user_data(response_structure.info);
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000C5FC File Offset: 0x0000A7FC
		public void check()
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("check"));
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000C6E4 File Offset: 0x0000A8E4
		public void setvar(string var, string data)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("setvar"));
			nameValueCollection["var"] = encryption.encrypt(var, this.enckey, text);
			nameValueCollection["data"] = encryption.encrypt(data, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000C7FC File Offset: 0x0000A9FC
		public string getvar(string var)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("getvar"));
			nameValueCollection["var"] = encryption.encrypt(var, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				return response_structure.response;
			}
			return null;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000C90C File Offset: 0x0000AB0C
		public void ban()
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("ban"));
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000C9F4 File Offset: 0x0000ABF4
		public string var(string varid)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("var"));
			nameValueCollection["varid"] = encryption.encrypt(varid, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				return response_structure.message;
			}
			return null;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000CB08 File Offset: 0x0000AD08
		public List<auth.users> fetchOnline()
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("fetchOnline"));
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				return response_structure.users;
			}
			return null;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000CC00 File Offset: 0x0000AE00
		public List<auth.msg> chatget(string channelname)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("chatget"));
			nameValueCollection["channel"] = encryption.encrypt(channelname, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				return response_structure.messages;
			}
			return null;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000CD10 File Offset: 0x0000AF10
		public bool chatsend(string msg, string channelname)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("chatsend"));
			nameValueCollection["message"] = encryption.encrypt(msg, this.enckey, text);
			nameValueCollection["channel"] = encryption.encrypt(channelname, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			return response_structure.success;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000CE30 File Offset: 0x0000B030
		public bool checkblack()
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("checkblacklist"));
			nameValueCollection["hwid"] = encryption.encrypt(deviceUniqueIdentifier, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			return response_structure.success;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000CF40 File Offset: 0x0000B140
		public string webhook(string webid, string param, string body = "", string conttype = "")
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
				return null;
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("webhook"));
			nameValueCollection["webid"] = encryption.encrypt(webid, this.enckey, text);
			nameValueCollection["params"] = encryption.encrypt(param, this.enckey, text);
			nameValueCollection["body"] = encryption.encrypt(body, this.enckey, text);
			nameValueCollection["conttype"] = encryption.encrypt(conttype, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			string text2 = auth.req(nameValueCollection);
			text2 = encryption.decrypt(text2, this.enckey, text);
			auth.response_structure response_structure = this.response_decoder.string_to_generic<auth.response_structure>(text2);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				return response_structure.response;
			}
			return null;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000D098 File Offset: 0x0000B298
		public void log(string message)
		{
			if (!this.initialized)
			{
				base.StartCoroutine(this.Error_PleaseInitializeFirst());
			}
			string text = encryption.sha256(encryption.iv_key());
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes("log"));
			nameValueCollection["pcuser"] = encryption.encrypt(Environment.UserName, this.enckey, text);
			nameValueCollection["message"] = encryption.encrypt(message, this.enckey, text);
			nameValueCollection["sessionid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.sessionid));
			nameValueCollection["name"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.name));
			nameValueCollection["ownerid"] = encryption.byte_arr_to_str(Encoding.Default.GetBytes(this.ownerid));
			nameValueCollection["init_iv"] = text;
			auth.req(nameValueCollection);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000D190 File Offset: 0x0000B390
		public static string checksum(string filename)
		{
			string text;
			using (MD5 md = MD5.Create())
			{
				using (FileStream fileStream = File.OpenRead(filename))
				{
					text = BitConverter.ToString(md.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
				}
			}
			return text;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000D200 File Offset: 0x0000B400
		private static string req(NameValueCollection post_data)
		{
			string text;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					byte[] array = webClient.UploadValues("https://keyauth.win/api/1.0/", post_data);
					text = Encoding.Default.GetString(array);
				}
			}
			catch (WebException ex)
			{
				if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.TooManyRequests)
				{
					ShowConsole.LogError("You're connecting too fast. Please slow down your requests and try again");
					Application.Quit();
					text = "";
				}
				else
				{
					ShowConsole.LogError("Connection failed. Please try again");
					Application.Quit();
					text = "";
				}
			}
			return text;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000D29C File Offset: 0x0000B49C
		private void load_app_data(auth.app_data_structure data)
		{
			this.app_data.numUsers = data.numUsers;
			this.app_data.numOnlineUsers = data.numOnlineUsers;
			this.app_data.numKeys = data.numKeys;
			this.app_data.version = data.version;
			this.app_data.customerPanelLink = data.customerPanelLink;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000D300 File Offset: 0x0000B500
		private void load_user_data(auth.user_data_structure data)
		{
			this.user_data.username = data.username;
			this.user_data.ip = data.ip;
			this.user_data.hwid = data.hwid;
			this.user_data.createdate = data.createdate;
			this.user_data.lastlogin = data.lastlogin;
			this.user_data.subscriptions = data.subscriptions;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000D373 File Offset: 0x0000B573
		private void load_response_struct(auth.response_structure data)
		{
			this.response.success = data.success;
			this.response.message = data.message;
		}

		// Token: 0x040000B1 RID: 177
		public string name;

		// Token: 0x040000B2 RID: 178
		public string ownerid;

		// Token: 0x040000B3 RID: 179
		public string secret;

		// Token: 0x040000B4 RID: 180
		public string version;

		// Token: 0x040000B5 RID: 181
		private string sessionid;

		// Token: 0x040000B6 RID: 182
		private string enckey;

		// Token: 0x040000B7 RID: 183
		private bool initialized;

		// Token: 0x040000B8 RID: 184
		public auth.app_data_class app_data = new auth.app_data_class();

		// Token: 0x040000B9 RID: 185
		public auth.user_data_class user_data = new auth.user_data_class();

		// Token: 0x040000BA RID: 186
		public auth.response_class response = new auth.response_class();

		// Token: 0x040000BB RID: 187
		private json_wrapper response_decoder = new json_wrapper(new auth.response_structure());

		// Token: 0x02000059 RID: 89
		[DataContract]
		private class response_structure
		{
			// Token: 0x17000025 RID: 37
			// (get) Token: 0x060002DC RID: 732 RVA: 0x000191D6 File Offset: 0x000173D6
			// (set) Token: 0x060002DD RID: 733 RVA: 0x000191DE File Offset: 0x000173DE
			[DataMember]
			public bool success { get; set; }

			// Token: 0x17000026 RID: 38
			// (get) Token: 0x060002DE RID: 734 RVA: 0x000191E7 File Offset: 0x000173E7
			// (set) Token: 0x060002DF RID: 735 RVA: 0x000191EF File Offset: 0x000173EF
			[DataMember]
			public string sessionid { get; set; }

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x060002E0 RID: 736 RVA: 0x000191F8 File Offset: 0x000173F8
			// (set) Token: 0x060002E1 RID: 737 RVA: 0x00019200 File Offset: 0x00017400
			[DataMember]
			public string contents { get; set; }

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x060002E2 RID: 738 RVA: 0x00019209 File Offset: 0x00017409
			// (set) Token: 0x060002E3 RID: 739 RVA: 0x00019211 File Offset: 0x00017411
			[DataMember]
			public string response { get; set; }

			// Token: 0x17000029 RID: 41
			// (get) Token: 0x060002E4 RID: 740 RVA: 0x0001921A File Offset: 0x0001741A
			// (set) Token: 0x060002E5 RID: 741 RVA: 0x00019222 File Offset: 0x00017422
			[DataMember]
			public string message { get; set; }

			// Token: 0x1700002A RID: 42
			// (get) Token: 0x060002E6 RID: 742 RVA: 0x0001922B File Offset: 0x0001742B
			// (set) Token: 0x060002E7 RID: 743 RVA: 0x00019233 File Offset: 0x00017433
			[DataMember]
			public string download { get; set; }

			// Token: 0x1700002B RID: 43
			// (get) Token: 0x060002E8 RID: 744 RVA: 0x0001923C File Offset: 0x0001743C
			// (set) Token: 0x060002E9 RID: 745 RVA: 0x00019244 File Offset: 0x00017444
			[DataMember(IsRequired = false, EmitDefaultValue = false)]
			public auth.user_data_structure info { get; set; }

			// Token: 0x1700002C RID: 44
			// (get) Token: 0x060002EA RID: 746 RVA: 0x0001924D File Offset: 0x0001744D
			// (set) Token: 0x060002EB RID: 747 RVA: 0x00019255 File Offset: 0x00017455
			[DataMember(IsRequired = false, EmitDefaultValue = false)]
			public auth.app_data_structure appinfo { get; set; }

			// Token: 0x1700002D RID: 45
			// (get) Token: 0x060002EC RID: 748 RVA: 0x0001925E File Offset: 0x0001745E
			// (set) Token: 0x060002ED RID: 749 RVA: 0x00019266 File Offset: 0x00017466
			[DataMember]
			public List<auth.msg> messages { get; set; }

			// Token: 0x1700002E RID: 46
			// (get) Token: 0x060002EE RID: 750 RVA: 0x0001926F File Offset: 0x0001746F
			// (set) Token: 0x060002EF RID: 751 RVA: 0x00019277 File Offset: 0x00017477
			[DataMember]
			public List<auth.users> users { get; set; }
		}

		// Token: 0x0200005A RID: 90
		public class msg
		{
			// Token: 0x1700002F RID: 47
			// (get) Token: 0x060002F1 RID: 753 RVA: 0x00019288 File Offset: 0x00017488
			// (set) Token: 0x060002F2 RID: 754 RVA: 0x00019290 File Offset: 0x00017490
			public string message { get; set; }

			// Token: 0x17000030 RID: 48
			// (get) Token: 0x060002F3 RID: 755 RVA: 0x00019299 File Offset: 0x00017499
			// (set) Token: 0x060002F4 RID: 756 RVA: 0x000192A1 File Offset: 0x000174A1
			public string author { get; set; }

			// Token: 0x17000031 RID: 49
			// (get) Token: 0x060002F5 RID: 757 RVA: 0x000192AA File Offset: 0x000174AA
			// (set) Token: 0x060002F6 RID: 758 RVA: 0x000192B2 File Offset: 0x000174B2
			public string timestamp { get; set; }
		}

		// Token: 0x0200005B RID: 91
		public class users
		{
			// Token: 0x17000032 RID: 50
			// (get) Token: 0x060002F8 RID: 760 RVA: 0x000192C3 File Offset: 0x000174C3
			// (set) Token: 0x060002F9 RID: 761 RVA: 0x000192CB File Offset: 0x000174CB
			public string credential { get; set; }
		}

		// Token: 0x0200005C RID: 92
		[DataContract]
		private class user_data_structure
		{
			// Token: 0x17000033 RID: 51
			// (get) Token: 0x060002FB RID: 763 RVA: 0x000192DC File Offset: 0x000174DC
			// (set) Token: 0x060002FC RID: 764 RVA: 0x000192E4 File Offset: 0x000174E4
			[DataMember]
			public string username { get; set; }

			// Token: 0x17000034 RID: 52
			// (get) Token: 0x060002FD RID: 765 RVA: 0x000192ED File Offset: 0x000174ED
			// (set) Token: 0x060002FE RID: 766 RVA: 0x000192F5 File Offset: 0x000174F5
			[DataMember]
			public string ip { get; set; }

			// Token: 0x17000035 RID: 53
			// (get) Token: 0x060002FF RID: 767 RVA: 0x000192FE File Offset: 0x000174FE
			// (set) Token: 0x06000300 RID: 768 RVA: 0x00019306 File Offset: 0x00017506
			[DataMember]
			public string hwid { get; set; }

			// Token: 0x17000036 RID: 54
			// (get) Token: 0x06000301 RID: 769 RVA: 0x0001930F File Offset: 0x0001750F
			// (set) Token: 0x06000302 RID: 770 RVA: 0x00019317 File Offset: 0x00017517
			[DataMember]
			public string createdate { get; set; }

			// Token: 0x17000037 RID: 55
			// (get) Token: 0x06000303 RID: 771 RVA: 0x00019320 File Offset: 0x00017520
			// (set) Token: 0x06000304 RID: 772 RVA: 0x00019328 File Offset: 0x00017528
			[DataMember]
			public string lastlogin { get; set; }

			// Token: 0x17000038 RID: 56
			// (get) Token: 0x06000305 RID: 773 RVA: 0x00019331 File Offset: 0x00017531
			// (set) Token: 0x06000306 RID: 774 RVA: 0x00019339 File Offset: 0x00017539
			[DataMember]
			public List<auth.Data> subscriptions { get; set; }
		}

		// Token: 0x0200005D RID: 93
		[DataContract]
		private class app_data_structure
		{
			// Token: 0x17000039 RID: 57
			// (get) Token: 0x06000308 RID: 776 RVA: 0x0001934A File Offset: 0x0001754A
			// (set) Token: 0x06000309 RID: 777 RVA: 0x00019352 File Offset: 0x00017552
			[DataMember]
			public string numUsers { get; set; }

			// Token: 0x1700003A RID: 58
			// (get) Token: 0x0600030A RID: 778 RVA: 0x0001935B File Offset: 0x0001755B
			// (set) Token: 0x0600030B RID: 779 RVA: 0x00019363 File Offset: 0x00017563
			[DataMember]
			public string numOnlineUsers { get; set; }

			// Token: 0x1700003B RID: 59
			// (get) Token: 0x0600030C RID: 780 RVA: 0x0001936C File Offset: 0x0001756C
			// (set) Token: 0x0600030D RID: 781 RVA: 0x00019374 File Offset: 0x00017574
			[DataMember]
			public string numKeys { get; set; }

			// Token: 0x1700003C RID: 60
			// (get) Token: 0x0600030E RID: 782 RVA: 0x0001937D File Offset: 0x0001757D
			// (set) Token: 0x0600030F RID: 783 RVA: 0x00019385 File Offset: 0x00017585
			[DataMember]
			public string version { get; set; }

			// Token: 0x1700003D RID: 61
			// (get) Token: 0x06000310 RID: 784 RVA: 0x0001938E File Offset: 0x0001758E
			// (set) Token: 0x06000311 RID: 785 RVA: 0x00019396 File Offset: 0x00017596
			[DataMember]
			public string customerPanelLink { get; set; }

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x06000312 RID: 786 RVA: 0x0001939F File Offset: 0x0001759F
			// (set) Token: 0x06000313 RID: 787 RVA: 0x000193A7 File Offset: 0x000175A7
			[DataMember]
			public string downloadLink { get; set; }
		}

		// Token: 0x0200005E RID: 94
		public class app_data_class
		{
			// Token: 0x1700003F RID: 63
			// (get) Token: 0x06000315 RID: 789 RVA: 0x000193B8 File Offset: 0x000175B8
			// (set) Token: 0x06000316 RID: 790 RVA: 0x000193C0 File Offset: 0x000175C0
			public string numUsers { get; set; }

			// Token: 0x17000040 RID: 64
			// (get) Token: 0x06000317 RID: 791 RVA: 0x000193C9 File Offset: 0x000175C9
			// (set) Token: 0x06000318 RID: 792 RVA: 0x000193D1 File Offset: 0x000175D1
			public string numOnlineUsers { get; set; }

			// Token: 0x17000041 RID: 65
			// (get) Token: 0x06000319 RID: 793 RVA: 0x000193DA File Offset: 0x000175DA
			// (set) Token: 0x0600031A RID: 794 RVA: 0x000193E2 File Offset: 0x000175E2
			public string numKeys { get; set; }

			// Token: 0x17000042 RID: 66
			// (get) Token: 0x0600031B RID: 795 RVA: 0x000193EB File Offset: 0x000175EB
			// (set) Token: 0x0600031C RID: 796 RVA: 0x000193F3 File Offset: 0x000175F3
			public string version { get; set; }

			// Token: 0x17000043 RID: 67
			// (get) Token: 0x0600031D RID: 797 RVA: 0x000193FC File Offset: 0x000175FC
			// (set) Token: 0x0600031E RID: 798 RVA: 0x00019404 File Offset: 0x00017604
			public string customerPanelLink { get; set; }

			// Token: 0x17000044 RID: 68
			// (get) Token: 0x0600031F RID: 799 RVA: 0x0001940D File Offset: 0x0001760D
			// (set) Token: 0x06000320 RID: 800 RVA: 0x00019415 File Offset: 0x00017615
			public string downloadLink { get; set; }
		}

		// Token: 0x0200005F RID: 95
		public class user_data_class
		{
			// Token: 0x17000045 RID: 69
			// (get) Token: 0x06000322 RID: 802 RVA: 0x00019426 File Offset: 0x00017626
			// (set) Token: 0x06000323 RID: 803 RVA: 0x0001942E File Offset: 0x0001762E
			public string username { get; set; }

			// Token: 0x17000046 RID: 70
			// (get) Token: 0x06000324 RID: 804 RVA: 0x00019437 File Offset: 0x00017637
			// (set) Token: 0x06000325 RID: 805 RVA: 0x0001943F File Offset: 0x0001763F
			public string ip { get; set; }

			// Token: 0x17000047 RID: 71
			// (get) Token: 0x06000326 RID: 806 RVA: 0x00019448 File Offset: 0x00017648
			// (set) Token: 0x06000327 RID: 807 RVA: 0x00019450 File Offset: 0x00017650
			public string hwid { get; set; }

			// Token: 0x17000048 RID: 72
			// (get) Token: 0x06000328 RID: 808 RVA: 0x00019459 File Offset: 0x00017659
			// (set) Token: 0x06000329 RID: 809 RVA: 0x00019461 File Offset: 0x00017661
			public string createdate { get; set; }

			// Token: 0x17000049 RID: 73
			// (get) Token: 0x0600032A RID: 810 RVA: 0x0001946A File Offset: 0x0001766A
			// (set) Token: 0x0600032B RID: 811 RVA: 0x00019472 File Offset: 0x00017672
			public string lastlogin { get; set; }

			// Token: 0x1700004A RID: 74
			// (get) Token: 0x0600032C RID: 812 RVA: 0x0001947B File Offset: 0x0001767B
			// (set) Token: 0x0600032D RID: 813 RVA: 0x00019483 File Offset: 0x00017683
			public List<auth.Data> subscriptions { get; set; }
		}

		// Token: 0x02000060 RID: 96
		public class Data
		{
			// Token: 0x1700004B RID: 75
			// (get) Token: 0x0600032F RID: 815 RVA: 0x00019494 File Offset: 0x00017694
			// (set) Token: 0x06000330 RID: 816 RVA: 0x0001949C File Offset: 0x0001769C
			public string subscription { get; set; }

			// Token: 0x1700004C RID: 76
			// (get) Token: 0x06000331 RID: 817 RVA: 0x000194A5 File Offset: 0x000176A5
			// (set) Token: 0x06000332 RID: 818 RVA: 0x000194AD File Offset: 0x000176AD
			public string expiry { get; set; }

			// Token: 0x1700004D RID: 77
			// (get) Token: 0x06000333 RID: 819 RVA: 0x000194B6 File Offset: 0x000176B6
			// (set) Token: 0x06000334 RID: 820 RVA: 0x000194BE File Offset: 0x000176BE
			public string timeleft { get; set; }
		}

		// Token: 0x02000061 RID: 97
		public class response_class
		{
			// Token: 0x1700004E RID: 78
			// (get) Token: 0x06000336 RID: 822 RVA: 0x000194CF File Offset: 0x000176CF
			// (set) Token: 0x06000337 RID: 823 RVA: 0x000194D7 File Offset: 0x000176D7
			public bool success { get; set; }

			// Token: 0x1700004F RID: 79
			// (get) Token: 0x06000338 RID: 824 RVA: 0x000194E0 File Offset: 0x000176E0
			// (set) Token: 0x06000339 RID: 825 RVA: 0x000194E8 File Offset: 0x000176E8
			public string message { get; set; }
		}
	}
}
