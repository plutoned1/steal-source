using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ark.Core
{
    public class Patchers
    {
        [HarmonyPatch(typeof(VRRig), "OnDisable", MethodType.Normal)]
        public class OnDisable : MonoBehaviour
        {
            public static bool Prefix(VRRig __instance)
            {
                return (__instance != GorillaTagger.Instance.offlineVRRig);
            }
        }

        [HarmonyPatch(typeof(GameObject))]
        [HarmonyPatch("CreatePrimitive", MethodType.Normal)]
        internal class CreatePrimitive
        {
            public static void Postfix(GameObject __result)
            {
                __result.GetComponent<Renderer>().material.shader = Shader.Find(
                    "GorillaTag/UberShader"
                );
            }
        }
    }
}
