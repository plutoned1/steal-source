using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ark.Core.Menu
{
    [BepInPlugin("xorsmenu", "xor's menu", "1.0")]
    public class Start : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("xorsmenu");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
