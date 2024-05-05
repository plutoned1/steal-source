using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ark.Core
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("FixedUpdate", MethodType.Normal)]
    public class Inputs : MonoBehaviour
    {
        public static bool LeftPrimary;
        public static bool RightPrimary;
        public static bool LeftSecondary;
        public static bool RightSecondary;
        public static bool LeftGrip;
        public static bool RightGrip;
        public static bool LeftTrigger;
        public static bool RightTrigger;

        public static void Prefix()
        {
            LeftPrimary = ControllerInputPoller.instance.leftControllerPrimaryButton;
            RightPrimary = ControllerInputPoller.instance.rightControllerPrimaryButton;
            LeftSecondary = ControllerInputPoller.instance.leftControllerSecondaryButton;
            RightSecondary = ControllerInputPoller.instance.rightControllerSecondaryButton;
            LeftGrip = ControllerInputPoller.instance.leftGrab;
            RightGrip = ControllerInputPoller.instance.rightGrab;
            LeftTrigger = (ControllerInputPoller.instance.leftControllerIndexFloat >= 0.5f);
            RightTrigger = (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.5f);
        }
    }
}
