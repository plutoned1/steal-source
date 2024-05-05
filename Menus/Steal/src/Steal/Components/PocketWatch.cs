using System;
using System.Linq;
using GorillaNetworking;
using Steal.Background;
using UnityEngine;
using UnityEngine.UI;

namespace Steal.Components
{
	// Token: 0x0200002B RID: 43
	internal class PocketWatch : MonoBehaviour
	{
		// Token: 0x06000091 RID: 145 RVA: 0x000094DB File Offset: 0x000076DB
		public void OnDisable()
		{
			Object.Destroy(PocketWatch.computer);
			Object.Destroy(PocketWatch.computerText);
			Object.Destroy(this.computerMaterial);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000094FC File Offset: 0x000076FC
		public void FixedUpdate()
		{
			if (GorillaTagger.Instance.offlineVRRig == null)
			{
				return;
			}
			if (PocketWatch.computer == null)
			{
				PocketWatch.computer = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
				PocketWatch.computer.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f);
				GameObject gameObject = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)/HuntWatch_ScreenLocal/Canvas/Anchor");
				PocketWatch.computerText = gameObject.transform.GetChild(0).gameObject;
				this.computerMaterial = gameObject.transform.GetChild(1).gameObject;
				gameObject.transform.GetChild(2).gameObject.SetActive(false);
				gameObject.transform.GetChild(3).gameObject.SetActive(false);
				gameObject.transform.GetChild(4).gameObject.SetActive(false);
				gameObject.transform.GetChild(5).gameObject.SetActive(false);
				gameObject.transform.GetChild(6).gameObject.SetActive(false);
				PocketWatch.computerText.GetComponent<Text>().text = "Start Menu With Right Stick";
				this.computerMaterial.GetComponent<Image>().material = new Material(GorillaComputer.instance.pressedMaterial);
				this.computerMaterial.SetActive(true);
				Renderer component = PocketWatch.computer.GetComponent<Renderer>();
				component.material.shader = Shader.Find("UI/Default");
				component.material.SetColor("_Color", MenuPatch.GetTheme(UI.Theme)[2]);
				return;
			}
			if (PocketWatch.computer != null && PocketWatch.computerText != null && this.computerMaterial != null)
			{
				if (PocketWatch.computer.GetComponent<GorillaHuntComputer>() && (!PocketWatch.computer.activeSelf || PocketWatch.computer.GetComponent<GorillaHuntComputer>().enabled))
				{
					PocketWatch.computer.SetActive(true);
					PocketWatch.computer.GetComponent<GorillaHuntComputer>().enabled = false;
				}
				if (InputHandler.LeftJoystick.x < -0.85f && !InputHandler.LeftStickClick && Time.frameCount >= this.buttonCooldown + 20)
				{
					int num = MenuPatch.buttons.ToList<MenuPatch.Button>().FindIndex((MenuPatch.Button b) => b.buttonText == this.currentButton.buttonText);
					num--;
					this.currentButton = MenuPatch.buttons[num];
					this.buttonCooldown = Time.frameCount;
				}
				if (InputHandler.LeftJoystick.x > 0.85f && !InputHandler.LeftStickClick)
				{
					if (Time.frameCount >= this.buttonCooldown + 20)
					{
						int num2 = MenuPatch.buttons.ToList<MenuPatch.Button>().FindIndex((MenuPatch.Button b) => b.buttonText == this.currentButton.buttonText);
						num2++;
						this.currentButton = MenuPatch.buttons[num2];
						this.buttonCooldown = Time.frameCount;
					}
				}
				else if (Time.frameCount >= this.buttonCooldown + 20 && InputHandler.LeftStickClick)
				{
					MenuPatch.Toggle(this.currentButton);
					this.buttonCooldown = Time.frameCount;
				}
				if (PocketWatch.computerText.GetComponent<Text>() && MenuPatch.buttons[6] != null && this.currentButton != null && PocketWatch.computerText != null)
				{
					if (PocketWatch.computerText.GetComponent<Text>().text == "Start Menu With RightSticks" && InputHandler.LeftStickClick)
					{
						this.currentButton = MenuPatch.buttons[6];
						return;
					}
					if (this.currentButton.Enabled)
					{
						this.computerMaterial.GetComponent<Image>().material.color = Color.green;
						PocketWatch.computerText.GetComponent<Text>().color = Color.white;
					}
					else
					{
						this.computerMaterial.GetComponent<Image>().material.color = Color.red;
						PocketWatch.computerText.GetComponent<Text>().color = Color.white;
					}
					PocketWatch.computerText.GetComponent<Text>().text = this.currentButton.buttonText;
				}
			}
		}

		// Token: 0x04000065 RID: 101
		internal static GameObject computer;

		// Token: 0x04000066 RID: 102
		internal static GameObject computerText;

		// Token: 0x04000067 RID: 103
		private GameObject computerMaterial;

		// Token: 0x04000068 RID: 104
		public static float lastButtonMovement;

		// Token: 0x04000069 RID: 105
		private MenuPatch.Button currentButton = MenuPatch.buttons[6];

		// Token: 0x0400006A RID: 106
		private int buttonCooldown;
	}
}
