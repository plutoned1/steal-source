using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Config = ark.Core.Config;

namespace ark.Core.UI
{
    [BepInPlugin("xorsui", "xor's ui", "1.0.0")]
    public class UIProcess : BaseUnityPlugin
    {
        public static UITexture oTexture = new UITexture() { DefaultColor = Color.white };

        public static List<string> buttonTextList = new List<string>();

        private void OnGUI()
        {
            foreach (var Mod in ModHandler.AllMods)
            {
                foreach (var Mods1 in Mod.Value.Mods)
                {
                    if (Mods1.isEnabled)
                    {
                        if (!buttonTextList.Contains(Mods1.name))
                            buttonTextList.Add(Mods1.name);
                    }
                    else
                    {
                        if (buttonTextList.Contains(Mods1.name))
                            buttonTextList.Remove(Mods1.name);
                    }
                }
            }

            foreach (string itemlol in buttonTextList)
            {
                GUILayout.Label(itemlol);
            }

            GUI.backgroundColor = new Color32(0, 0, 0, 0);

            windowRect = GUI.Window(31415, windowRect, new GUI.WindowFunction(this.Orbit), $"");
        }

        public static Rect windowRect = new Rect(20, 10, 500, 500);

        public void Space()
        {
            GUILayout.Space(2);
        }

        public void GUIButton(string name, Color color, Action action)
        {
            GUI.backgroundColor = new Color32(0, 0, 0, 0);
            GUI.contentColor = ark.Core.Config.TextColor;
            if (GUILayout.Button(""))
                action();
            GUI.backgroundColor = Color.white;
            Rect rect = GUILayoutUtility.GetLastRect();
            oTexture.Draw(rect, color, 5f);
            GUI.backgroundColor = new Color32(0, 0, 0, 0);
            GUI.contentColor = ark.Core.Config.TextColor;
            if (GUI.Button(rect, name))
                action();
        }


        public static Vector2[] ScrollVectors = new Vector2[999];
        public static Vector2[] ScrollVectors2 = new Vector2[999];
        private int tabNum;

        public void Orbit(int windowID)
        {
            oTexture.Draw(
                new Rect(0, 10, windowRect.width, windowRect.height - 10),
                ark.Core.Config.MenuColor,
                12f
            );

            GUIButton(
                $"{ark.Core.Config.Name}",
                ark.Core.Config.OffColor,
                () => { }
            );

            GUILayout.BeginHorizontal();

            foreach (var Mod in ModHandler.AllMods.Keys)
            {
                GUIButton(
                    Mod,
                    tabNum == Array.IndexOf(ModHandler.AllMods.Keys.ToArray(), Mod) ? ark.Core.Config.OnColor : ark.Core.Config.OffColor,
                    () =>
                    {
                        tabNum = Array.IndexOf(ModHandler.AllMods.Keys.ToArray(), Mod);
                    }
                );
            }

            GUILayout.EndHorizontal();

            ScrollVectors[tabNum] = GUILayout.BeginScrollView(ScrollVectors[tabNum]);

            foreach (var Mod in ModHandler.AllMods)
            {
                if (tabNum == Array.IndexOf(ModHandler.AllMods.Keys.ToArray(), Mod.Key))
                {
                    foreach (var Mods1 in Mod.Value.Mods)
                    {
                        GUIButton(
                            Mods1.name,
                            Mods1.isEnabled
                                ? ark.Core.Config.OnColor 
                                : ark.Core.Config.OffColor,
                            () =>
                            {
                                Mods1.isEnabled = !Mods1.isEnabled;
                            }
                        );
                    }
                }
            }

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}
