using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ark.Core.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("FixedUpdate", MethodType.Normal)]
    public class Main : MonoBehaviour
    {
        public static string[] buttons = new string[] { "black" };

        public static GameObject menu = new GameObject();
        static GameObject canvasObj = new GameObject();
        public static GameObject menuPointer = new GameObject();
        static GameObject pointer = new GameObject();
        public static int framePressCooldown = 0;
        public static int number = 0;
        public static int btncount = 8;
        private static bool isinCat = false;
        private static string cat = "";
        public static Font coolFontThing =
            Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        static void Prefix(GorillaLocomotion.Player __instance)
        {
            try
            {
                var inp = Inputs.LeftSecondary;
                if (inp && menu == null)
                {
                    Draw();
                    if (menuPointer == null)
                    {
                        menuPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        menuPointer.GetComponent<MeshRenderer>().material.color = Color.white;
                        menuPointer.transform.parent = (__instance.rightControllerTransform);
                        menuPointer.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                        menuPointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    }
                }
                else if (!inp && menu != null)
                {
                    float spinStrength = 175f;
                    Vector3 Velocity = GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                    Vector3 randomSpin = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * spinStrength;
                    Rigidbody rigidMenu = menu.AddComponent<Rigidbody>();

                    rigidMenu.velocity = Velocity;
                    Object.Destroy(menu, 3f);
                    Object.Destroy(menuPointer);
                    menuPointer = null;
                    menu = null;
                }

                if (inp && menu != null)
                {
                    menu.transform.position = (__instance.leftControllerTransform.position);
                    menu.transform.rotation = (__instance.leftControllerTransform.rotation);
                }
                foreach (var Mod in ModHandler.AllMods)
                {
                    foreach (var Mods1 in Mod.Value.Mods)
                    {
                        if (Mods1.isEnabled)
                        {
                            if (Mods1.enabledMethod != null) Mods1.enabledMethod();
                        }
                        else
                        {
                            if (Mods1.disabledMethod != null) Mods1.disabledMethod();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                File.WriteAllText("ERROR.log", e.ToString());
            }
        }

        static void addbtn1(float offset, bool[] buttonsActive, string text)
        {
            GameObject newBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(newBtn.GetComponent<Rigidbody>());
            newBtn.GetComponent<BoxCollider>().isTrigger = true;
            newBtn.transform.parent = menu.transform;
            newBtn.transform.rotation = Quaternion.identity;
            newBtn.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
            newBtn.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
            newBtn.AddComponent<BtnCollider>().relatedText = $"{text}.Mod";

            int index = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (text == buttons[i])
                {
                    index = i;
                    break;
                }
            }

            if (!buttonsActive[index])
            {
                newBtn.GetComponent<Renderer>().material.color = ark.Core.Config.OffColor;
            }
            else if (buttonsActive[index])
            {
                newBtn.GetComponent<Renderer>().material.color = ark.Core.Config.OnColor;
            }
            else
            {
                newBtn.GetComponent<Renderer>().material.color = Color.red;
            }

            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            Text title = titleObj.AddComponent<Text>();
            title.font = coolFontThing;
            title.color = ark.Core.Config.TextColor;
            title.text = text;
            title.fontStyle = FontStyle.Italic;
            title.fontSize = 1;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.2f, 0.03f);
            titleTransform.localPosition = new Vector3(0.065f, 0f, 0.111f - (offset / 2.55f));
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        static void addbtn2(float offset, string text, string data)
        {
            GameObject newBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(newBtn.GetComponent<Rigidbody>());
            newBtn.GetComponent<BoxCollider>().isTrigger = true;
            newBtn.transform.parent = menu.transform;
            newBtn.transform.rotation = Quaternion.identity;
            newBtn.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
            newBtn.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
            newBtn.AddComponent<BtnCollider>().relatedText = data;
            newBtn.GetComponent<Renderer>().material.color = ark.Core.Config.OffColor;
            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            Text title = titleObj.AddComponent<Text>();
            title.font = coolFontThing;
            title.text = text;
            title.color = ark.Core.Config.TextColor;
            title.fontStyle = FontStyle.Italic;
            title.fontSize = 1;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.2f, 0.03f);
            titleTransform.localPosition = new Vector3(0.065f, 0f, 0.111f - (offset / 2.55f));
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        static void sidebtn(float offset, string data)
        {
            GameObject newBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(newBtn.GetComponent<Rigidbody>());
            newBtn.GetComponent<BoxCollider>().isTrigger = true;
            newBtn.transform.parent = menu.transform;
            newBtn.transform.rotation = Quaternion.identity;
            newBtn.transform.localScale = new Vector3(0.09f, 0.12f, 0.9f);
            newBtn.transform.localPosition = new Vector3(0.56f, 0f - offset, 0f);
            newBtn.AddComponent<BtnCollider>().relatedText = data;
            newBtn.GetComponent<Renderer>().material.color = ark.Core.Config.OffColor;
        }

        public static void Draw()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(menu.GetComponent<Rigidbody>());
            GameObject.Destroy(menu.GetComponent<BoxCollider>());
            GameObject.Destroy(menu.GetComponent<Collider>());
            GameObject.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);

            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(background.GetComponent<Rigidbody>());
            GameObject.Destroy(background.GetComponent<BoxCollider>());
            background.transform.parent = menu.transform;
            background.transform.rotation = Quaternion.identity;
            background.transform.localScale = new Vector3(0.1f, 1f, 1f);
            background.GetComponent<Renderer>().material.color = ark.Core.Config.MenuColor;
            background.transform.position = new Vector3(0.05f, 0f, 0f);

            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScale = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScale.dynamicPixelsPerUnit = 1000;

            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            Text title = titleObj.AddComponent<Text>();
            title.font = coolFontThing;
            title.text =
                $"xor's sexy template";
            title.fontStyle = FontStyle.Italic;
            title.color = ark.Core.Config.TextColor;
            title.fontSize = 1;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.28f, 0.04f);
            titleTransform.position = new Vector3(0.06f, 0f, 0.169f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            var offset = 0.1f;
            var debounce = 0;

            addbtn2(-0.31f, "Disconnect", ".Disc");

            sidebtn(-0.64f, "Back");
            sidebtn(0.64f, "Next");

            if (!isinCat)
            {
                buttons = ModHandler.AllMods.Keys.ToArray();
                for (
                    int i = 0;
                    i < buttons.Skip(number * btncount).Take(btncount).ToArray().Length;
                    i++
                )
                {
                    addbtn2(
                        i * offset + debounce,
                        buttons.Skip(number * btncount).Take(btncount).ToArray()[i],
                        $"{buttons.Skip(number * btncount).Take(btncount).ToArray()[i]}.Category"
                    );
                }
            }
            else
            {
                addbtn2(0.835f, "Go Back", ".Leave");

                ModHandler.AllMods.TryGetValue(cat, out var mG);
                buttons = ModHandler.GetAllNamesInCategory(cat);
                for (
                    int i = 0;
                    i < buttons.Skip(number * btncount).Take(btncount).ToArray().Length;
                    i++
                )
                {
                    addbtn1(
                        i * offset + debounce,
                        ModHandler.GetAllBoolsInCategory(cat),
                        buttons.Skip(number * btncount).Take(btncount).ToArray()[i]
                    );
                }
            }
        }

        public static void ResetMenu()
        {
            GameObject.Destroy(menu);
            menu = null;
            Draw();
        }

        public static void Toggle(string relatedText)
        {
            int maxpages = (buttons.Length + btncount - 1) / btncount;

            if (relatedText == "Back")
            {
                number = (number > 0 ? number - 1 : maxpages - 1);
            }
            else if (relatedText == "Next")
            {
                number = (number < maxpages - 1 ? number + 1 : 0);
            }
            else if (relatedText.EndsWith(".Category"))
            {
                cat = relatedText.Replace(".Category", "");
                isinCat = true;
            }
            else if (relatedText.EndsWith(".Disc"))
            {
                PhotonNetwork.Disconnect();
            }
            else if (relatedText.EndsWith(".Leave"))
            {
                cat = "";
                isinCat = false;
            }
            else if (relatedText.EndsWith(".Mod"))
            {
                ModHandler.AllMods.TryGetValue(cat, out var mG);
                string[] buttons = ModHandler.GetAllNamesInCategory(cat);

                foreach (var ModGroup in ModHandler.AllMods)
                {
                    foreach (var Mod in ModGroup.Value.Mods)
                    {
                        if (Mod.name == relatedText.Replace(".Mod", ""))
                        {
                            Mod.isEnabled = !Mod.isEnabled;
                        }
                    }
                }
            }

            GameObject.Destroy(menu);
            menu = null;
            Draw();
        }
    }
}
