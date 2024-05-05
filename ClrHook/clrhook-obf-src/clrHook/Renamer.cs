using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clrHook;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace MindLated.Protection.Renamer;

internal class RenamerPhase
{
    public enum RenameMode
    {
        Ascii,
        Key,
        Normal
    }

    private const string Ascii = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static readonly Random Random = new();

    public static readonly string[] NormalNameStrings =
    {
        "TryInvoke", "get_Aliases", "get_AllowedCaller", "get_Help", "get_Name", "get_Permissions", "get_Syntax",
        "Execute", "get_Aliases", "get_AllowedCaller", "get_Help", "get_Name", "get_Permissions", "get_Syntax",
        "Execute", "get_Aliases", "get_AllowedCaller", "get_Help", "get_Name", "get_Permissions", "get_Syntax",
        "Execute", "get_Name", "set_Name", "get_Name", "set_Name", "get_Name", "get_Help", "get_Syntax",
        "get_AllowedCaller", "get_Commands", "set_Commands", "add_OnExecuteCommand", "remove_OnExecuteCommand",
        "Reload", "checkCommandMappings", "checkDuplicateCommandMappings", "Plugins_OnPluginsLoaded",
        "GetCommand", "GetCommand", "getCommandIdentity", "getCommandType", "Register", "Register", "Register",
        "DeregisterFromAssembly", "GetCooldown", "SetCooldown", "Execute", "RegisterFromAssembly",
            "StartGame",
    "UpdatePlayerPosition",
    "FixedUpdatePhysics",
    "LateUpdateCamera",
    "OnCollisionEnterEnemy",
    "OnCollisionExitPlayer",
    "OnTriggerStayObstacle",
    "OnTriggerEnterPowerup",
    "OnTriggerExitGoal",
    "AwakeCharacter",
    "OnEnableControls",
    "OnDisableInputs",
    "OnDestroyObject",
    "InstantiateProjectile",
    "DestroyGameObject",
    "SetActiveUI",
    "GetComponentByTag",
    "SendMessageToAll",
    "BroadcastMessageToAll",
    "FindObjectOfType",
    "FindObjectsByTag",
    "RaycastFromPlayer",
    "RaycastAllInScene",
    "CrossFadeAnimation",
    "LoadSceneLevel",
    "UnloadSceneLevel",
    "PlaySoundEffect",
    "StopMusic",
    "PauseGame",
    "ResumeGame",
    "RotateObject",
    "TranslateObject",
    "ScaleObject",
    "LookAtTarget",
    "InvokeAction",
    "InvokeRepeatingAction",
    "CancelInvokeAction",
    "AddForceToRigidbody",
    "AddTorqueToRigidbody",
    "MoveTowardsPosition",
    "LerpPosition",
    "SlerpRotation",
    "ClampValue",
    "GenerateRandomNumber",
    "LerpFloat",
    "ClampFloat",
    "LerpVector3",
    "LerpQuaternion",
    "GetDeltaTime",
    "GetTimeScale",
    "GetFixedDeltaTime",
    "SetTimeScale",
    "CalculateDistance",
    "CalculateCrossProduct",
    "CalculateDotProduct",
    "CreateRotationFromEulerAngles",
    "CreateRotationToLookAt",
    "SlerpQuaternion",
    "AbsoluteValue",
    "RoundValue",
    "FloorValue",
    "CeilValue",
    "GetMinimumValue",
    "GetMaximumValue",
    "PingPongValue",
    "RepeatValue",
    "GetSign",
    "SmoothStepValue",
    "LerpAngle",
        "CalculateTotalCost",
    "ValidateUserInput",
    "GenerateRandomString",
    "ConvertToUpperCase",
    "SortArrayDescending",
    "ConnectToDatabase",
    "SendEmailNotification",
    "RenderGraph",
    "FindLargestNumber",
    "EncryptData",
    "DeserializeJson",
    "UpdateUserSettings",
    "DrawRectangle",
    "SearchForItem",
    "ExecuteQuery",
    "ParseInputData",
    "GenerateReport",
    "StartTimer",
    "FetchUserData",
    "CreateNewInstance",
    "DeleteFile",
    "AddToShoppingCart",
    "DisplayErrorMessage",
    "InsertDatabaseRecord",
    "CalculateAverage",
    "ValidateEmailAddress",
    "AuthenticateUser",
    "GenerateUniqueID",
    "CalculateDistance",
    "DisplayMenuOptions",
    "ReadFileContents",
    "DisplayLoadingScreen",
    "ConvertToBinary",
    "MergeArrays",
    "CheckPalindrome",
    "CaptureScreenshot",
    "CalculateTax",
    "UpdateInventory",
    "DisplayHelpMenu",
    "ShuffleArray",
    "DeleteDatabaseRecord",
    "ResizeImage",
    "CheckPrimeNumber",
    "FetchWeatherData",
    "ConvertToPDF",
    "DisplayConfirmationDialog",
    "CreateBackup",
    "ConvertCurrency",
    "SendHttpRequest",
    "GenerateHash",
    "ExtractTextFromImage",
    "CreateDirectory",
    "CheckInternetConnection",
    "SaveDataToFile",
    "DrawCircle",
    "GenerateQRCode",
    "TakeScreenshot",
    "CheckFileExists",
    "ParseXML",
    "DisplaySuccessMessage",
    "CheckPasswordStrength",
    "ConnectToAPI",
    "ExportData",
    "ValidateCreditCardNumber",
    "SortLinkedList",
    "FetchNewsFeed",
    "CreateTemporaryFile",
    "CheckFilePermissions",
    "ConvertToRomanNumerals",
    "FetchStockData",
    "SendSMSNotification",
    "DisplayWelcomeMessage",
    "ParseCSV",
    "GenerateUUID",
    "SendFax",
    "DetectLanguage",
    "CheckDiskSpace",
    "DisplayErrorLog",
    "EncryptFile",
    "DecryptData",
    "SendPushNotification",
    "ReadExcelFile",
    "DisplayUserDetails",
    "ExtractZipFile",
    "LoadSettings",
    "CreateThumbnail",
    "ValidateIPv4Address",
    "PlaySound",
    "SendChatMessage",
    "RotateImage",
    "GenerateAuthToken",
    "DeleteDirectory",
    "ExecuteShellCommand",
    "FetchCalendarEvents",
    "CreateDatabaseBackup",
    "AnalyzeData",
    "GenerateBarCode",
    "ConvertToHexadecimal",
    "DisplayImage",
    "LogException",
        "InitializeLevel",
    "MovePlayer",
    "RotateCamera",
    "PlayAnimation",
    "SpawnEnemy",
    "DetectCollision",
    "ApplyDamage",
    "ActivatePowerup",
    "DeactivateObstacle",
    "ResetPlayerPosition",
    "LoadNextLevel",
    "UnloadPreviousLevel",
    "ToggleSound",
    "TogglePause",
    "ResetGame",
    "GenerateProjectile",
    "DestroyObstacle",
    "ToggleUIElement",
    "FindObjectOfTypeWithTag",
    "SendGlobalMessage",
    "InterpolatePosition",
    "InterpolateRotation",
    "LimitValue",
    "GenerateRandomFloat",
    "InterpolateVector3",
    "InterpolateQuaternion",
    "GetFrameDeltaTime",
    "GetGameTimeScale",
    "GetFixedTimeStep",
    "SetFixedTimeScale",
    "ComputeDistance",
    "ComputeCrossProduct",
    "ComputeDotProduct",
    "RotateObjectToEulerAngles",
    "RotateObjectToTarget",
    "InterpolateQuaternionSlerp",
    "CalculateAbsolute",
    "CalculateRoundedValue",
    "CalculateFloorValue",
    "CalculateCeilValue",
    "FindMinimumValue",
    "FindMaximumValue",
    "PingPongMovement",
    "RepeatAnimation",
    "GetSignOfValue",
    "SmoothStepInterpolation",
    "InterpolateAngle"
    };

    private static readonly Dictionary<string, string> Names = new();

    private static string RandomString(int length, string chars)
    {
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static string GetRandomName()
    {
        return NormalNameStrings[Random.Next(NormalNameStrings.Length)];
    }

    public static string GenerateString(RenameMode mode)
    {
        return mode switch
        {
            RenameMode.Ascii => RandomString(Random.Next(3, 12), Ascii),
            RenameMode.Key => RandomString(16, Ascii),
            RenameMode.Normal => GetRandomName(),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }


    public static void ExecuteClassRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            if (type.Name == "GeneratedInternalTypeHelper" || type.Namespace.Contains("Loading") || type.Namespace.Contains("LoadObject") || type.Name == "Resources" || type.Name == "Settings")
            {
                continue;
            }

            if (type.Name.Contains("Resources"))
            {
                return;
            }

            if (type.Name.Contains("Discord"))
            {
                return;
            }

            if (type.Name.Contains("DiscordRPC"))
            {
                return;
            }

            if (Names.TryGetValue(type.Name, out string? nameValue))
            {
                type.Name = nameValue;
            }
            else
            {
                string newName = Convert.ToBase64String(Encoding.UTF8.GetBytes(type.Name));

                Names.Add(type.Name, newName);
                if (type.Name != "Base" && type.Name != "GameObjectPatch")
                    type.Name = newName;
            }
        }

        ApplyChangesToResourcesClasses(module);
    }


    private static void ApplyChangesToResourcesClasses(ModuleDefMD module)
    {
        ModuleDefMD moduleToRename = module;
        foreach (Resource? resource in moduleToRename.Resources)
        {
            foreach (KeyValuePair<string, string> item in Names)
            {
                if (resource.Name.Contains(item.Key))
                {
                    resource.Name = resource.Name.Replace(item.Key, item.Value);
                }
            }
        }

        foreach (TypeDef? type in moduleToRename.GetTypes())
        {
            foreach (PropertyDef? property in type.Properties)
            {
                if (property.Name != "ResourceManager")
                {
                    continue;
                }

                IList<Instruction>? instr = property.GetMethod.Body.Instructions;

                for (int i = 0; i < instr.Count - 3; i++)
                {
                    if (instr[i].OpCode == OpCodes.Ldstr)
                    {
                        foreach (KeyValuePair<string, string> item in Names.Where(item =>
                                     item.Key == instr[i].Operand.ToString()))
                        {
                            instr[i].Operand = item.Value;
                        }
                    }
                }
            }
        }
    }


    public static void ExecuteFieldRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            foreach (FieldDef? field in type.Fields)
            {
                if (field.IsInitOnly)
                {
                    continue;
                }

                if (field.HasCustomAttributes) continue;
                if (Names.TryGetValue(field.Name, out string? nameValue))
                {
                    field.Name = nameValue;
                }
                else
                {
                    string newName = GenerateString(RenameMode.Ascii);

                    Names.Add(field.Name, newName);
                    field.Name = newName;
                }
            }
        }

        ApplyChangesToResourcesField(module);
    }

    private static void ApplyChangesToResourcesField(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (!type.IsGlobalModuleType)
            {

                foreach (MethodDef? methodDef in type.Methods)
                {
                    if (methodDef.Name != "InitializeComponent")
                    {
                        if (!methodDef.HasBody)
                        {
                            continue;
                        }

                        IList<Instruction> instructions = methodDef.Body.Instructions;
                        for (int i = 0; i < instructions.Count - 3; i++)
                        {
                            if (instructions[i].OpCode == OpCodes.Ldstr)
                            {
                                foreach (KeyValuePair<string, string> keyValuePair in Names)
                                {
                                    if (keyValuePair.Key == instructions[i].Operand.ToString())
                                    {
                                        instructions[i].Operand = keyValuePair.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void ExecuteMethodRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }


            if (type.Name == "GeneratedInternalTypeHelper")
            {
                continue;
            }

            foreach (MethodDef? method in type.Methods)
            {
                if (!method.HasBody)
                {
                    continue;
                }

                if (method.IsVirtual || method.IsSpecialName)
                {
                    continue;
                }

                if (method.Name == ".ctor" || method.Name == ".cctor")
                {
                    continue;
                }

                method.Name = GenerateString(RenameMode.Ascii);
            }
        }
    }

    public static void ExecuteModuleRenaming(ModuleDefMD mod)
    {
        foreach (ModuleDef? module in mod.Assembly.Modules)
        {
            bool isWpf = false;
            foreach (AssemblyRef? asmRef in module.GetAssemblyRefs())
            {
                if (asmRef.Name == "WindowsBase" || asmRef.Name == "PresentationCore" ||
                    asmRef.Name == "PresentationFramework" || asmRef.Name == "System.Xaml")
                {
                    isWpf = true;
                }
            }

            if (!isWpf)
            {
                module.Name = GenerateString(RenameMode.Ascii);

                module.Assembly.CustomAttributes.Clear();
                module.Mvid = Guid.NewGuid();
                module.Assembly.Name = GenerateString(RenameMode.Ascii);
                module.Assembly.Version = new Version(Random.Next(1, 9), Random.Next(1, 9), Random.Next(1, 9),
                    Random.Next(1, 9));
            }
        }
    }

    public static void ExecuteNamespaceRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            if (type.Namespace == "" || type.Namespace == "Resources" || type.Namespace.Contains("Loading") || (type.Namespace == "Steal.Background.Security.Auth" && type.Name == "Base"))
            {
                continue;
            }

            if (type.Namespace.Contains("Resources"))
            {
                return;
            }
            string newName = Convert.ToBase64String(Encoding.UTF8.GetBytes(type.Name));

            type.Namespace = newName;
        }

        ApplyChangesToResourcesNamespace(module);
    }

    private static void ApplyChangesToResourcesNamespace(ModuleDefMD module)
    {
        foreach (Resource? resource in module.Resources)
        {
            foreach (KeyValuePair<string, string> item in Names.Where(item => resource.Name.Contains(item.Key)))
            {
                resource.Name = resource.Name.Replace(item.Key, item.Value);
            }
        }

        foreach (TypeDef? type in module.GetTypes())
        {
            foreach (PropertyDef? property in type.Properties)
            {
                if (property.Name != "ResourceManager")
                {
                    continue;
                }


                IList<Instruction>? instr = property.GetMethod.Body.Instructions;

                for (int i = 0; i < instr.Count - 3; i++)
                {
                    if (instr[i].OpCode == OpCodes.Ldstr)
                    {
                        foreach (KeyValuePair<string, string> item in Names.Where(item =>
                                     item.Key == instr[i].Operand.ToString()))
                        {
                            instr[i].Operand = item.Value;
                        }
                    }
                }
            }
        }
    }

    public static void ExecutePropertiesRenaming(ModuleDefMD module)
    {
        foreach (TypeDef? type in module.GetTypes())
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            foreach (PropertyDef? property in type.Properties)
            {
                property.Name = GenerateString(RenameMode.Ascii);
            }
        }
    }
}