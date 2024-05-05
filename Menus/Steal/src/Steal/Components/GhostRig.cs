using System;
using UnityEngine;

namespace Steal.Components
{
	// Token: 0x02000028 RID: 40
	internal class GhostRig : MonoBehaviour
	{
		// Token: 0x06000086 RID: 134 RVA: 0x000088A4 File Offset: 0x00006AA4
		public void Awake()
		{
			if (GhostRig.instance == null)
			{
				GhostRig.instance = this;
				GhostRig.hasinstance = true;
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000088C0 File Offset: 0x00006AC0
		public void LateUpdate()
		{
			if (this.ghostRig == null && GorillaTagger.Instance.offlineVRRig != null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(GorillaTagger.Instance.offlineVRRig.gameObject);
				this.ghostRig = gameObject.GetComponent<VRRig>();
				Object.Destroy(gameObject.GetComponent<Rigidbody>());
				this.ghostRig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
				this.ghostRig.mainSkin.material.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 40);
				this.ghostRig.transform.position = Vector3.zero;
				this.ghostRig.enabled = false;
				GameObject.Find("Player Objects/GorillaParent/Local Gorilla Player(Clone)/VR Constraints/LeftArm/Left Arm IK/SlideAudio").SetActive(false);
				GameObject.Find("Player Objects/GorillaParent/Local Gorilla Player(Clone)/VR Constraints/RightArm/Right Arm IK/SlideAudio").SetActive(false);
			}
			if (this.ghostRig == null)
			{
				return;
			}
			if (this.ghostRig.enabled && GorillaTagger.Instance.offlineVRRig.enabled)
			{
				this.ghostRig.transform.position = Vector3.zero;
				this.ghostRig.enabled = false;
			}
			if (!GorillaTagger.Instance.offlineVRRig.enabled)
			{
				this.ghostRig.enabled = true;
				this.ghostRig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
				this.ghostRig.mainSkin.material.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 40);
			}
		}

		// Token: 0x04000052 RID: 82
		private VRRig ghostRig;

		// Token: 0x04000053 RID: 83
		public static GhostRig instance;

		// Token: 0x04000054 RID: 84
		public static bool hasinstance;
	}
}
