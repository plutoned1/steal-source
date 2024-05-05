using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BepInEx;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using HarmonyLib;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using Valve.VR;
using static NetworkSystem;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;
using Player = Photon.Realtime.Player;
using ark.Mods;
using Room = ark.Mods.Room;
using ark.Core.Menu;
using ark.Core;
using ark.Core.UI;


namespace ark
{
    [BepInPlugin("xorsnotis", "xor's notis", "1.0")]
    public class notif : BaseUnityPlugin
    {
        static GameObject MainCamera;
        static GameObject HUDObj;
        static GameObject HUDObj2;
        static Text Testtext;
        static Material AlertText = new Material(Shader.Find("GUI/Text Shader"));
        static int NotificationDecayTime = 150;
        static int NotificationDecayTimeCounter = 0;
        public static int NoticationThreshold = 30; //Amount of notifications before they stop queuing up
        static string[] Notifilines;
        static string newtext;
        public static string PreviousNotifi;
        static bool HasInit = false;
        static Text NotifiText;
        public static bool IsEnabled = true;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin NotificationLib is loaded!");
            Harmony.CreateAndPatchAll(typeof(notif));
        }

        private static void Init()
        {
            //this is mostly copy pasted from LHAX, which was also made by me.
            //LHAX got leaked the day before this. so i might as well make this public cus people asked me to.
            MainCamera = GameObject.Find("Main Camera");
            HUDObj = new GameObject(); //GameObject.CreatePrimitive(PrimitiveType.Cube);
            HUDObj2 = new GameObject();
            HUDObj2.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.AddComponent<Canvas>();
            HUDObj.AddComponent<CanvasScaler>();
            HUDObj.AddComponent<GraphicRaycaster>();
            HUDObj.GetComponent<Canvas>().enabled = true;
            HUDObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            HUDObj.GetComponent<Canvas>().worldCamera = MainCamera.GetComponent<Camera>();
            HUDObj.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
            //HUDObj.CreatePrimitive()
            HUDObj.GetComponent<RectTransform>().position = new Vector3(
                MainCamera.transform.position.x,
                MainCamera.transform.position.y,
                MainCamera.transform.position.z
            ); //new Vector3(-67.151f, 11.914f, -82.749f);
            HUDObj2.transform.position = new Vector3(
                MainCamera.transform.position.x,
                MainCamera.transform.position.y,
                MainCamera.transform.position.z
            );
            HUDObj.transform.parent = HUDObj2.transform;
            HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
            var Temp = HUDObj.GetComponent<RectTransform>().rotation.eulerAngles;
            Temp.y = -270f;
            HUDObj.transform.localScale = new Vector3(1f, 1f, 1f);
            HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(Temp);
            GameObject TestText = new GameObject();
            TestText.transform.parent = HUDObj.transform;
            Testtext = TestText.AddComponent<Text>();
            Testtext.text = "";
            Testtext.fontSize = 10;
            Testtext.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            Testtext.rectTransform.sizeDelta = new Vector2(260, 140);
            Testtext.alignment = TextAnchor.LowerLeft;
            Testtext.rectTransform.localScale = new Vector3(0.01f, 0.01f, 1.4f);
            Testtext.rectTransform.localPosition = new Vector3(-1.5f, -.9f, -.6f);
            Testtext.material = AlertText;
            NotifiText = Testtext;
        }

        private static void FixedUpdate()
        {
            //This is a bad way to do this, but i do not want to rely on utila.
            if (HasInit == false)
            {
                if (GameObject.Find("Main Camera") != null)
                {
                    Init();
                    HasInit = true;
                }
            }
            HUDObj2.transform.position = new Vector3(
                MainCamera.transform.position.x,
                MainCamera.transform.position.y,
                MainCamera.transform.position.z
            );
            HUDObj2.transform.rotation = MainCamera.transform.rotation;
            //HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
            if (Testtext.text != "") //THIS CAUSES A MEMORY LEAK!!!!! -no longer causes a memory leak
            {
                NotificationDecayTimeCounter++;
                if (NotificationDecayTimeCounter > NotificationDecayTime)
                {
                    Notifilines = null;
                    newtext = "";
                    NotificationDecayTimeCounter = 0;
                    Notifilines = Testtext
                        .text.Split(Environment.NewLine.ToCharArray())
                        .Skip(1)
                        .ToArray();
                    foreach (string Line in Notifilines)
                    {
                        if (Line != "")
                        {
                            newtext = newtext + Line + "\n";
                        }
                    }

                    Testtext.text = newtext;
                }
            }
            else
            {
                NotificationDecayTimeCounter = 0;
            }
        }

        public static void send(string NotificationText)
        {
            if (IsEnabled)
            {
                if (!NotificationText.Contains(Environment.NewLine))
                {
                    NotificationText = NotificationText + Environment.NewLine;
                }
                NotifiText.text += NotificationText;
                PreviousNotifi = NotificationText;
            }
        }

        public static void clearAll()
        {
            NotifiText.text = "";
        }

        public static void clearPast(int amount)
        {
            string[] Notifilines = null;
            string newtext = "";
            Notifilines = NotifiText
                .text.Split(Environment.NewLine.ToCharArray())
                .Skip(amount)
                .ToArray();
            foreach (string Line in Notifilines)
            {
                if (Line != "")
                {
                    newtext = newtext + Line + "\n";
                }
            }

            NotifiText.text = newtext;
        }

        //This is probably not the way i should have fixed it, but it "works" (I CANNOT STRESS THIS ENOUGH, DONT DO IT LIKE THIS!)
        #region HackyFixedUpdateFix
        [HarmonyPatch(typeof(GorillaLocomotion.Player), "FixedUpdate")]
        [HarmonyPrefix]
        static bool FixedUpdateHook()
        {
            FixedUpdate();
            return true;
        }
        #endregion
    }

    public class ModGroup
    {
        public List<Mod> Mods;
    }
    public class Mod
    {
        public string name;
        public bool isEnabled;
        public Action enabledMethod;
        public Action disabledMethod;
    }

    public class ModHandler
    {
        public static int[] ints = new int[999];
        public static float[] floats = new float[999];
        public static bool[] bools = new bool[999];

        private static Dictionary<GameObject, Rigidbody> activeBreadcrumbs =
            new Dictionary<GameObject, Rigidbody>();
        private static int spreadAngle = 25;

        private static void CreateAndTrackBreadcrumbs(Vector3 position, Vector3 velocity)
        {
            GameObject breadcrumb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            breadcrumb.transform.position = position;
            breadcrumb.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            Rigidbody breadcrumbRigidbody = breadcrumb.AddComponent<Rigidbody>();
            breadcrumbRigidbody.velocity = velocity;
            breadcrumbRigidbody.useGravity = false;
            breadcrumb.GetComponent<Collider>().enabled = false;

            breadcrumb.GetComponent<Renderer>().material.color = Color.grey;

            activeBreadcrumbs.Add(breadcrumb, breadcrumbRigidbody);

            UnityEngine.Object.Destroy(breadcrumb, .3f);
        }

        private static bool hasRemovedThisFrame = false;

        public static void BeePlayer(VRRig player)
        {
            foreach (
                BeeSwarmManager swarm in UnityEngine.Object.FindObjectsOfType<BeeSwarmManager>()
            )
            {
                foreach (
                    AngryBeeSwarm swarm2 in UnityEngine.Object.FindObjectsOfType<AngryBeeSwarm>()
                )
                {
                    swarm.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    swarm.photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    swarm.photonView.OwnershipTransfer = OwnershipOption.Takeover;
                    swarm.photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                    swarm2.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    swarm2.photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    swarm2.photonView.OwnershipTransfer = OwnershipOption.Takeover;
                    swarm2.photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                    swarm2.Emerge(player.transform.position, new Vector3(-999f, -999f, -999f));
                    swarm2.currentState = AngryBeeSwarm.ChaseState.Chasing;
                }
            }
        }

        public static void BetaSetColor(Photon.Realtime.Player p, Color c)
        {
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable[byte.MaxValue] = c;
            Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
            dictionary.Add(251, hashtable);
            dictionary.Add(254, p.ActorNumber);
            dictionary.Add(250, true);
            PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOperation(
                252,
                dictionary,
                SendOptions.SendUnreliable
            );
            Flush();
        }

        public static void SetName(Photon.Realtime.Player p, string n)
        {
            Hashtable hashtable = new Hashtable();
            hashtable[byte.MaxValue] = n;
            Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
            dictionary.Add(251, hashtable);
            dictionary.Add(254, p.ActorNumber);
            dictionary.Add(250, true);
            PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOperation(
                252,
                dictionary,
                SendOptions.SendUnreliable
            );
            Flush();
        }

        public static void BreakRig(PhotonView v)
        {
            int num = v.ViewID;
            Hashtable ServerCleanDestroyEvent = new Hashtable();
            RaiseEventOptions ServerCleanOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.RemoveFromRoomCache
            };
            ServerCleanDestroyEvent[0] = num;
            ServerCleanOptions.CachingOption = EventCaching.AddToRoomCache;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(
                204,
                ServerCleanDestroyEvent,
                ServerCleanOptions,
                SendOptions.SendUnreliable
            );
            Flush();
        }

        public static void Crash(PhotonView v)
        {
            Hashtable hashtable = new Hashtable();
            hashtable[(byte)0] = v.Owner.ActorNumber;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(
                207,
                hashtable,
                null,
                SendOptions.SendReliable
            );
            Flush();
        }

        public static VRRig PlayerToRig(Photon.Realtime.Player p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }

        public static void Flush()
        {
            if (!hasRemovedThisFrame)
            {
                hasRemovedThisFrame = true;
                RaiseEventOptions options = new RaiseEventOptions();
                options.CachingOption = EventCaching.RemoveFromRoomCache;
                options.TargetActors = new int[1] { PhotonNetwork.LocalPlayer.ActorNumber };
                RaiseEventOptions optionsdos = options;
                PhotonNetwork.NetworkingClient.OpRaiseEvent(
                    200,
                    null,
                    optionsdos,
                    SendOptions.SendReliable
                );
                GorillaNot.instance.rpcErrorMax = int.MaxValue;
                GorillaNot.instance.rpcCallLimit = int.MaxValue;
                GorillaNot.instance.logErrorMax = int.MaxValue;
                PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
                PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig);
                PhotonNetwork.RemoveBufferedRPCs(GorillaTagger.Instance.myVRRig.ViewID, null, null);
                PhotonNetwork.RemoveRPCsInGroup(int.MaxValue);
                PhotonNetwork.SendAllOutgoingCommands();
                GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            }
        }

        public static void ReqAntiBan(Action thing)
        {
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED"))
                notif.send("Please enable anti-ban!");
            else
                thing();
        }

        public static void ReqMaster(Action thing)
        {
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED"))
                notif.send("Please enable anti-ban!");
            else if (!PhotonNetwork.IsMasterClient)
                notif.send("Please enable set master!");
            else
                thing();
        }

        public static float lastTime = -1f;
        public static bool antibanworked = false;

        public static float AntiBanDelay = 5f;
        public static bool AntiBanExecuted = false;

        public static PhotonView GetView(VRRig rig)
        {
            PhotonView value = Traverse.Create(rig).Field("photonView").GetValue<PhotonView>();
            bool flag = value != null;
            PhotonView result;
            if (flag)
            {
                result = value;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public static GameObject pointer = null;

        public static void Gun(RaycastHit hit, System.Action action)
        {
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetMouseButton(1))
            {
                if (pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    pointer.GetComponent<Renderer>().material.shader = Shader.Find(
                        "GUI/Text Shader"
                    );
                }
                pointer.transform.position = hit.point;
                if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.5f || UnityInput.Current.GetMouseButton(0))
                {
                    pointer.GetComponent<Renderer>().material.color = Color.green;
                    action();
                }
                else
                {
                    pointer.GetComponent<Renderer>().material.color = Color.white;
                }
            }
            else
            {
                UnityEngine.GameObject.Destroy(pointer);
            }
        }

        public static RaycastHit GunCast()
        {
            RaycastHit hit = new RaycastHit();
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(
                    GorillaLocomotion.Player.Instance.rightControllerTransform.position
                        - GorillaLocomotion.Player.Instance.rightControllerTransform.up,
                    -GorillaLocomotion.Player.Instance.rightControllerTransform.up,
                    out hit
                );
            }
            if (UnityInput.Current.GetMouseButton(1))
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(UnityInput.Current.mousePosition), out hit);
            }
            return hit;
        }

        public static void Disable(string ModName)
        {
            foreach (var Mod in AllMods)
            {
                foreach (var Mods1 in Mod.Value.Mods)
                {
                    if (Mods1.name == ModName)
                    {
                        Mods1.isEnabled = false;
                        UIProcess.buttonTextList.Remove(ModName);
                        GameObject.Destroy(Main.menu);
                        Main.menu = null;
                        Main.Draw();
                    }
                }
            }
        }

        public static void Rename(string ModName, string NewName)
        {
            foreach (var Mod in AllMods)
            {
                foreach (var Mods1 in Mod.Value.Mods)
                {
                    if (Mods1.name == ModName)
                    {
                        Mods1.name = NewName;
                        GameObject.Destroy(Main.menu);
                        Main.menu = null;
                        Main.Draw();
                    }
                }
            }
        }

        public static bool[] GetAllBoolsInCategory(string CategoryName)
        {
            List<bool> allBools = new List<bool>();
            foreach (var Mod in AllMods)
            {
                if (Mod.Key == CategoryName)
                {
                    foreach (var Mod2 in Mod.Value.Mods)
                    {
                        allBools.Add(Mod2.isEnabled);
                    }
                }
            }

            return allBools.ToArray();
        }

        public static string[] GetAllNamesInCategory(string CategoryName)
        {
            List<string> allNames = new List<string>();
            foreach (var Mod in AllMods)
            {
                if (Mod.Key == CategoryName)
                {
                    foreach (var Mod2 in Mod.Value.Mods)
                    {
                        allNames.Add(Mod2.name);
                    }
                }
            }

            return allNames.ToArray();
        }

        public static Dictionary<string, ModGroup> AllMods = new Dictionary<string, ModGroup>()
        {
            {
                "Settings",
                new ModGroup()
                {
                    Mods = new List<Mod>()
                    {
                        new Mod() { name="Clear Notifications", enabledMethod=Settings.ClearNotifications },
                    }
                }
            },
            {
                "Room",
                new ModGroup()
                {
                    Mods = new List<Mod>()
                    {
                        new Mod() { name="Join Random", enabledMethod=Room.JoinRandom },
                    }
                }
            },
        };
    }
}
