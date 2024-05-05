using System;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Steal.Background.Mods;
using Steal.Patchers.VRRigPatchers;
using UnityEngine;
using UnityEngine.UI;

namespace Steal.Components
{
	// Token: 0x0200002A RID: 42
	internal class NameTags : MonoBehaviour
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00009224 File Offset: 0x00007424
		public static PhotonView GetPhotonViewFromRig(VRRig rig)
		{
			try
			{
				PhotonView value = Traverse.Create(rig).Field("photonView").GetValue<PhotonView>();
				if (value != null)
				{
					return value;
				}
			}
			catch
			{
				throw;
			}
			return null;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000926C File Offset: 0x0000746C
		public void OnDisable()
		{
			if (this.userName != null)
			{
				Object.Destroy(this.userName.gameObject);
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000928C File Offset: 0x0000748C
		private void LateUpdate()
		{
			if (!OnEnable.nameTags || base.GetComponent<VRRig>() == null || base.GetComponent<VRRig>().isOfflineVRRig || NameTags.GetPhotonViewFromRig(base.GetComponent<VRRig>()) == null)
			{
				Object.Destroy(this);
				return;
			}
			if (this.userName == null)
			{
				this.myRig = base.GetComponent<VRRig>();
				this.myPlayer = NameTags.GetPhotonViewFromRig(this.myRig).Controller;
				this.userName = Object.Instantiate<Text>(this.myRig.playerText, this.myRig.playerText.transform.parent);
			}
			this.userName.transform.localPosition = new Vector3(32.025f, 222f, -16.5f);
			if (this.myPlayer.CustomProperties.ContainsKey("steal"))
			{
				if (AdminControls.adminIDS.Contains(this.myPlayer.UserId))
				{
					this.userName.text = "[OWNER]\n" + this.myPlayer.NickName;
					this.userName.color = Color.red;
				}
				if (this.myPlayer.CustomProperties["steal"].ToString() == PhotonNetwork.CurrentRoom.Name)
				{
					this.userName.text = "[PAID]\n" + this.myPlayer.NickName;
					this.userName.color = Color.magenta;
				}
				else if (this.myPlayer.CustomProperties["steal"].ToString() == PhotonNetwork.CurrentRoom.Name + "[FREE]")
				{
					this.userName.text = "[FREE]\n" + this.myPlayer.NickName;
					this.userName.color = Color.green;
				}
			}
			this.userName.transform.localScale = new Vector3(4f, 4f, 4f);
			this.userName.transform.eulerAngles = new Vector3(0f, GameObject.Find("Main Camera").transform.eulerAngles.y, 0f);
		}

		// Token: 0x04000062 RID: 98
		private VRRig myRig;

		// Token: 0x04000063 RID: 99
		private Player myPlayer;

		// Token: 0x04000064 RID: 100
		private Text userName;
	}
}
