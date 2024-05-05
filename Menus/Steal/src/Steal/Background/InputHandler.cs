using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

namespace Steal.Background
{
	// Token: 0x02000031 RID: 49
	public class InputHandler : MonoBehaviour
	{
		// Token: 0x060000AD RID: 173 RVA: 0x0000AD5C File Offset: 0x00008F5C
		public void Awake()
		{
			this.type = InputHandler.HeadsetType();
			this.leftController = InputDevices.GetDeviceAtXRNode(4);
			this.rightController = InputDevices.GetDeviceAtXRNode(5);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000AD84 File Offset: 0x00008F84
		public static InputHandler.VrType HeadsetType()
		{
			if (!XRSettings.isDeviceActive)
			{
				ShowConsole.Log("No VR device detected.");
				return InputHandler.VrType.none;
			}
			if (XRSettings.loadedDeviceName.Contains("Oculus"))
			{
				return InputHandler.VrType.Oculus;
			}
			if (XRSettings.loadedDeviceName.Contains("Windows"))
			{
				return InputHandler.VrType.WindowsMR;
			}
			if (XRSettings.loadedDeviceName.Contains("Open"))
			{
				return InputHandler.VrType.OpenVR;
			}
			return InputHandler.VrType.MockHMD;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000ADDE File Offset: 0x00008FDE
		private static bool CalculateGripState(float grabValue, float grabThreshold)
		{
			return grabValue >= grabThreshold;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000ADE8 File Offset: 0x00008FE8
		public void LateUpdate()
		{
			if (ControllerInputPoller.instance != null)
			{
				ControllerInputPoller instance = ControllerInputPoller.instance;
				if (this.type == InputHandler.VrType.OpenVR)
				{
					InputHandler.RightSecondary = instance.rightControllerPrimaryButton;
					InputHandler.RightPrimary = instance.rightControllerSecondaryButton;
					InputHandler.RightTrigger = InputHandler.CalculateGripState(instance.rightControllerIndexFloat, InputHandler.sensitivity);
					InputHandler.RightGrip = InputHandler.CalculateGripState(instance.rightControllerGripFloat, InputHandler.sensitivity);
					InputHandler.RightJoystick = instance.rightControllerPrimary2DAxis;
					InputHandler.RightStickClick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(2);
					InputHandler.LeftSecondary = instance.leftControllerPrimaryButton;
					InputHandler.LeftPrimary = instance.leftControllerSecondaryButton;
					InputHandler.LeftTrigger = InputHandler.CalculateGripState(instance.leftControllerIndexFloat, InputHandler.sensitivity);
					InputHandler.LeftGrip = InputHandler.CalculateGripState(instance.leftControllerGripFloat, InputHandler.sensitivity);
					InputHandler.LeftJoystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(1);
					InputHandler.LeftStickClick = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(1);
					return;
				}
				this.rightController.TryGetFeatureValue(CommonUsages.primaryButton, ref InputHandler.RightPrimary);
				this.rightController.TryGetFeatureValue(CommonUsages.secondaryButton, ref InputHandler.RightSecondary);
				this.rightController.TryGetFeatureValue(CommonUsages.triggerButton, ref InputHandler.RightTrigger);
				this.rightController.TryGetFeatureValue(CommonUsages.gripButton, ref InputHandler.RightGrip);
				this.rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, ref InputHandler.RightStickClick);
				this.rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, ref InputHandler.RightJoystick);
				this.leftController.TryGetFeatureValue(CommonUsages.primaryButton, ref InputHandler.LeftPrimary);
				this.leftController.TryGetFeatureValue(CommonUsages.secondaryButton, ref InputHandler.LeftSecondary);
				this.leftController.TryGetFeatureValue(CommonUsages.triggerButton, ref InputHandler.LeftTrigger);
				this.leftController.TryGetFeatureValue(CommonUsages.gripButton, ref InputHandler.LeftGrip);
				this.leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, ref InputHandler.LeftStickClick);
				this.leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, ref InputHandler.LeftJoystick);
			}
		}

		// Token: 0x04000078 RID: 120
		public static float sensitivity = 0.5f;

		// Token: 0x04000079 RID: 121
		public static bool RightSecondary;

		// Token: 0x0400007A RID: 122
		public static bool RightPrimary;

		// Token: 0x0400007B RID: 123
		public static bool RightTrigger;

		// Token: 0x0400007C RID: 124
		public static bool RightGrip;

		// Token: 0x0400007D RID: 125
		public static Vector2 RightJoystick;

		// Token: 0x0400007E RID: 126
		public static bool RightStickClick;

		// Token: 0x0400007F RID: 127
		public static bool LeftSecondary;

		// Token: 0x04000080 RID: 128
		public static bool LeftPrimary;

		// Token: 0x04000081 RID: 129
		public static bool LeftGrip;

		// Token: 0x04000082 RID: 130
		public static bool LeftTrigger;

		// Token: 0x04000083 RID: 131
		public static Vector2 LeftJoystick;

		// Token: 0x04000084 RID: 132
		public static bool LeftStickClick;

		// Token: 0x04000085 RID: 133
		private InputHandler.VrType type;

		// Token: 0x04000086 RID: 134
		public InputDevice leftController;

		// Token: 0x04000087 RID: 135
		public InputDevice rightController;

		// Token: 0x02000057 RID: 87
		public enum VrType
		{
			// Token: 0x0400016E RID: 366
			OpenVR,
			// Token: 0x0400016F RID: 367
			Oculus,
			// Token: 0x04000170 RID: 368
			WindowsMR,
			// Token: 0x04000171 RID: 369
			MockHMD,
			// Token: 0x04000172 RID: 370
			none
		}
	}
}
