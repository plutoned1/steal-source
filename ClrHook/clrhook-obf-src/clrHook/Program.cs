using clrHook;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using Helpers.Injection;
using MindLated.Protection.Renamer;
using PeNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using static MindLated.Protection.Renamer.RenamerPhase;

public static class strtoimgay
{
    public static string sdfsdfsdfsdfsdf(string input)
    {
        return new string(input.Select(c =>
             c >= 'a' && c <= 'z' ? (char)(((c - 'a' + 13) % 26) + 'a') :
             c >= 'A' && c <= 'Z' ? (char)(((c - 'A' + 13) % 26) + 'A') : c).ToArray());
    }
}
public class StringToROT13
{
    public static string Rot13Encrypt(string input)
    {
        return strtoimgay.sdfsdfsdfsdfsdf(input);
    }

    public static void Execute(ModuleDefMD module)
    {
        var injDec = new Injector(module, typeof(strtoimgay));
        var decryptCall = injDec.FindMember("sdfsdfsdfsdfsdf") as MethodDef;
        foreach (TypeDef type in module.Types)
        {
            if (type.IsGlobalModuleType)
                continue;
            if (type.Namespace == "Costura")
                continue;
            foreach (MethodDef method in type.Methods)
            {
                if (method.HasBody)
                {
                    var instr = method.Body.Instructions;
                    method.Body.SimplifyBranches();
                    instr.OptimizeBranches();
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode == OpCodes.Ldstr)
                        {
                            string oldString = instr[i].Operand.ToString();
                            string encryptedString = Rot13Encrypt(oldString);
                            instr[i].OpCode = OpCodes.Nop;
                            instr[i].Operand = null;
                            instr.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, encryptedString));
                            instr.Insert(i + 2, Instruction.Create(OpCodes.Call, decryptCall));
                            i += 2;
                        }
                    }
                    method.Body.SimplifyBranches();
                    instr.OptimizeBranches();
                }
            }
        }
    }

}

class Program
{
    static string status = "IDLE";

    static void titleThing()
    {
        while (true)
        {
            Thread.Sleep(100);
            Console.Title = "clrHook - " + status.ToUpper();
        }
    }

    static void Main(string[] args)
    {
        new Thread(() => titleThing()).Start();
        status = "Obfuscating";
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("[");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("+");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("]");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" clrHook - Made by Yosef, Sial and Visual. Edited by kman");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\n");
        Console.WriteLine("> Enter the path to the file you wanna modify:");
        var filePath = Console.ReadLine().Replace('"', ' ');

        try
        {
            Console.Clear();
            ModuleContext modCtx = ModuleDefMD.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(filePath, modCtx);
            invalidcode(module);
            CaliTest(module);
            //AnnoyingIntagers(module);z
            StringToHex.Execute(module);
            //AnnoyingIntagers(module);
            ModuleWriterOptions ModOpts = new ModuleWriterOptions(module);
            ModOpts.MetadataOptions.Flags = MetadataFlags.AlwaysCreateStringsHeap;

            for (int i = 0; i < 2; i++)
            {
                encryptString(module);
            }
            StringToROT13.Execute(module);
            for (int i = 0; i < 3; i++)
            {
                encryptString(module);
            }
            ControlFlow.Execute(module);
            ControlFlow.Execute(module);
            Jump(module);
            ProtectionPhase(module);
            ExecuteMethodRenaming(module);
            ProxyString(module);
            fakeobfuscation(module);

            junkattrib(module);
            //Arithmetic.Execute(module);
            Execute2(module);

            junkattrib2(module);

            antiDe4Dot(module);
            //AnnoyingStrings(module);
            RenamerPhase.ExecuteClassRenaming(module);
            RenamerPhase.ExecuteNamespaceRenaming(module);
            var opts = new ModuleWriterOptions(module)
            {
                Logger = DummyLogger.NoThrowInstance
            };
            RandomMethods(module);
            module.Write(filePath.Replace(".dll", "") + "_prot.dll", opts);


            var peFile = new PeFile(filePath.Replace(".dll", "") + "_prot.dll");
            var imageOptionalHeader = peFile.ImageNtHeaders.OptionalHeader;

            imageOptionalHeader.SizeOfHeaders = 0;
            imageOptionalHeader.SizeOfImage = 0;
            SaveModifiedPeFile(peFile, filePath.Replace(".dll", "") + "_prot.dll");

            File.Delete(filePath.Replace(".dll", "") + "_prot.dll");
            status = "Finished!";
            Console.WriteLine("Wrote file down to: " + filePath.Replace(".dll", "") + "_prot.dll");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        Thread.Sleep(5000);
        Environment.Exit(0);
        //Environment.FailFast("Timeout reached!");
    }
    public static void invalidcode(ModuleDef module)
    {
        foreach (TypeDef typeDef in module.GetTypes())
        {
            foreach (MethodDef methodDef in typeDef.Methods)
            {
                if (methodDef.HasBody || methodDef.Body.HasInstructions)
                {
                    methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Box, methodDef.Module.Import(typeof(Math))));
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

                if (method.Name == ".ctor" || method.Name == ".cctor" || type.Namespace.Contains("Loading"))
                {
                    continue;
                }

                if (method.Name != "OnTriggerEnter" && method.Name != "LoadObject" && method.Name != "Load" && method.Name != "TestTrigger" && method.Name != "OnTriggerExit" && method.Name != "Init" && method.Name != "Load" && method.Name != "LateUpdate" && method.Name != "Update" && method.Name != "FixedUpdate" && method.Name != "Start" && method.Name != "FixedUpdate" && method.Name != "Awake" && method.Name != "OnGUI" && method.Name != "Prefix" && method.Name != "OnJoinedRoom" && method.Name != "OnPlayerEnteredRoom" && method.Name != "OnPlayerLeftRoom" && method.Name != "OnConnected" && method.Name != "OnLeftRoom" && method.Name != "Postfix")
                {
                    method.Name = GetRandomName();
                }

            }
        }
    }
    public static void AnnoyingIntagers(ModuleDef module)
    {
        foreach (var t in module.Types)
        {
            if (t.IsGlobalModuleType) { continue; }
            foreach (var method in t.Methods)
            {
                if (!method.HasBody) continue;
                if (method.FullName.Contains("get")) continue;
                if (method.FullName.Contains("set")) continue;
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, new Random().Next(20000000)));
                    i += 1;
                }
                method.Body.SimplifyBranches();
            }
        }
    }
    public static void Jump(ModuleDefMD module)
    {
        foreach (var type in module.Types)
        {
            foreach (var meth in type.Methods.ToArray())
            {
                if (!meth.HasBody || !meth.Body.HasInstructions || meth.Body.HasExceptionHandlers) continue;
                for (var i = 0; i < meth.Body.Instructions.Count - 2; i++)
                {
                    var inst = meth.Body.Instructions[i + 1];
                    meth.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, MindLated.Protection.Renamer.RenamerPhase.GenerateString(MindLated.Protection.Renamer.RenamerPhase.RenameMode.Ascii)));
                    meth.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Br_S, inst));
                    i += 2;
                }
            }
        }
    }
    public static void ProtectionPhase(ModuleDef module)
    {
        foreach (TypeDef type in module.Types)
        {
            if (type.IsGlobalModuleType) { continue; }
            if (type.IsSealed) { continue; }
            foreach (FieldDef field in type.Fields)
                if (CanRename(field))
                    field.Name = RenamerPhase.GetRandomName(); 
        }

    }

    public static bool CanRename(TypeDef type)
    {
        if (type.IsGlobalModuleType)
            return false;
        try
        {
            if (type.Namespace.Contains("My"))
                return false;
        }
        catch { }

        if (type.Interfaces.Count > 0)
            return false;
        if (type.IsSpecialName)
            return false;
        if (type.IsRuntimeSpecialName)
            return false;
        else
            return true;
    }
    public static bool CanRename(MethodDef method)
    {
        if (method.IsConstructor)
            return false;
        /* if (method.GetType().GetInterfaces().Count() > 0)
             return false; */
        if (method.IsFamily)
            return false;
        if (method.IsRuntimeSpecialName)
            return false;
        if (method.DeclaringType.IsForwarder)
            return false;
        else
            return true;
    }
    public static bool CanRename(FieldDef field)
    {

        if (field.IsRuntimeSpecialName)
            return false;
        if (field.IsLiteral && field.DeclaringType.IsEnum)
            return false;

        else
            return true;
    }
    public static bool CanRename(EventDef ev)
    {
        if (ev.IsRuntimeSpecialName)
            return false;
        else
            return true;
    }

    public static string GenerateName()
    {
        System.Random random = new System.Random();
        string ascii = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string unreadable = "✓ ✔ ☑ ♥ ❤ ❥ ❣ ☂ ☔ ☎ ☏ ☒ ☘ ☠ ☹ ☺ ☻ ♬ ♻ ♲ ♿ ⚠ ☃ ʚϊɞ ✖ ✗ ✘ ♒ ♬ ✄ ✂ ✆ ✉ ✦ ✧ ♱ ♰ ♂ ♀ ☿ ❤ ❥ ❦ ❧ ™ ® © ♡ ♦ ♢ ♔ ♕ ♚ ♛ ★ ☆ ✮ ✯ ☄ ☾ ☽ ☼ ☀ ☁ ☂ ☃ ☻ ☺ ☹ ۞ ۩ εїз ☎ ☏ ¢ ☚ ☛ ☜ ☝ ☞ ☟ ✍ ✌ ☢ ☣ ☠ ☮ ☯ ♠ ♤ ♣ ♧ ♥࿂ ე ჳ ᆡ ༄ ♨ ๑ ❀ ✿ ψ ♆ ☪ ☭ ♪ ♩ ♫ ℘ ℑ ℜ ℵ ♏ η α ʊ ϟ ღ ツ 回 ₪ ™ © ® ¿ ¡ № ⇨ ❝ ❞ ℃ƺ ◠ ◡ ╭ ╮ ╯ ╰ ★ ☆ ⊙¤ ㊣★☆♀◆◇◣◢◥▲▼△▽⊿◤ ◥▆ ▇ █ █ ■ ";
        return RandomString(random.Next(0, 20), unreadable);

    }
    private static string RandomString(int length, string chars)
    {
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static void Execute2(ModuleDef md)
    {
        foreach (var type in md.GetTypes())
        {
            if (type.IsGlobalModuleType) continue;
            foreach (var method in type.Methods)
            {
                if (!method.HasBody) continue;
                {
                    for (var i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (!method.Body.Instructions[i].IsLdcI4()) continue;
                        var numorig = new Random(Guid.NewGuid().GetHashCode()).Next();
                        var div = new Random(Guid.NewGuid().GetHashCode()).Next();
                        var num = numorig ^ div;

                        var nop = OpCodes.Nop.ToInstruction();

                        var local = new Local(method.Module.ImportAsTypeSig(typeof(int)));
                        method.Body.Variables.Add(local);

                        method.Body.Instructions.Insert(i + 1, OpCodes.Stloc.ToInstruction(local));
                        method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, method.Body.Instructions[i].GetLdcI4Value() - sizeof(float)));
                        method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, num));
                        method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Ldc_I4, div));
                        method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Xor));
                        method.Body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Ldc_I4, numorig));
                        method.Body.Instructions.Insert(i + 7, Instruction.Create(OpCodes.Bne_Un, nop));
                        method.Body.Instructions.Insert(i + 8, Instruction.Create(OpCodes.Ldc_I4, 2));
                        method.Body.Instructions.Insert(i + 9, OpCodes.Stloc.ToInstruction(local));
                        method.Body.Instructions.Insert(i + 10, Instruction.Create(OpCodes.Sizeof, method.Module.Import(typeof(float))));
                        method.Body.Instructions.Insert(i + 11, Instruction.Create(OpCodes.Add));
                        method.Body.Instructions.Insert(i + 12, nop);
                        i += 12;
                    }
                    method.Body.SimplifyBranches();
                }
            }
        }
    }

    public static void ProxyString(ModuleDef module)
    {
        foreach (var type in module.GetTypes())
        {
            if (type.IsGlobalModuleType) continue;
            foreach (var meth in type.Methods)
            {
                if (!meth.HasBody) continue;
                var instr = meth.Body.Instructions;
                foreach (var t in instr)
                {
                    if (t.OpCode != OpCodes.Ldstr) continue;
                    var methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
                    var methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
                    var meth1 = new MethodDefUser(MindLated.Protection.Renamer.RenamerPhase.GenerateString(MindLated.Protection.Renamer.RenamerPhase.RenameMode.Normal),
                        MethodSig.CreateStatic(module.CorLibTypes.String),
                        methImplFlags, methFlags);
                    module.GlobalType.Methods.Add(meth1);
                    meth1.Body = new CilBody();
                    meth1.Body.Variables.Add(new Local(module.CorLibTypes.String));
                    meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, t.Operand.ToString()));
                    meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

                    t.OpCode = OpCodes.Call;
                    t.Operand = meth1;
                }
            }
        }
    }
    static List<MethodDef> caliMethods = new List<MethodDef>();
    public static void CaliTest(ModuleDefMD module)
    {
        foreach (var type in module.Types.ToArray())
        {
            if (type.Namespace.Contains("Auth")) continue;
            if (type.IsGlobalModuleType) continue;
            foreach (var method in type.Methods.ToArray())
            {
                if (caliMethods.Contains(method)) continue;
                if (!method.HasBody) continue;
                if (method.Name.Contains("Window")) continue;
                if (!method.Body.HasInstructions) continue;
                if (method.FullName.Contains("Prefix")) continue;
                if (method.FullName.Contains("Postfix")) continue;
                if (method.FullName.Contains("OnGUI")) continue;
                if (method.FullName.Contains("Costura")) continue;
                if (method.FullName.Contains("get")) continue;
                if (method.FullName.Contains("set")) continue;
                if (method.FullName.Contains("SynergiExtentions")) continue;
                if (method.IsConstructor) continue;
                if (method.DeclaringType.IsGlobalModuleType) continue;
                try
                {
                    var originalMethod = method;
                    MethodDef copyMethod = new MethodDefUser(RenamerPhase.GetRandomName(),
                                                              originalMethod.MethodSig,
                                                              originalMethod.ImplAttributes,
                                                              originalMethod.Attributes);
                    type.Methods.Add(copyMethod);

                    copyMethod.Body = originalMethod.Body;
                    originalMethod.Body = new CilBody();
                    var inst = Instruction.Create(OpCodes.Call, copyMethod);

                    for (int i = 0; i < originalMethod.Parameters.Count; i++)
                    {
                        originalMethod.Body.Instructions.Add(new Instruction(OpCodes.Ldarg, originalMethod.Parameters));
                    }
                    originalMethod.Body.Instructions.Add(inst);
                    originalMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                    copyMethod.Body.SimplifyBranches();
                    copyMethod.Body.OptimizeBranches();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

    }
    public static void RandomMethods(ModuleDefMD module)
    {
        foreach (var type in module.Types)
        {
            if (type == null || type.IsGlobalModuleType || type.IsEnum) continue;

            for (int i = 0; i < 23; i++)
            {
                type.Methods.Add(new MethodDefUser(RenamerPhase.GetRandomName(), new MethodSig(CallingConvention.StdCall), MethodImplAttributes.InternalCall | MethodImplAttributes.Unmanaged, MethodAttributes.Public | MethodAttributes.Static));
            }
        }
    }

    public static void fakeobfuscation(ModuleDefMD module)
    {
        for (int i = 0; i < 40; i++)
        {
            var fakeattrib = new TypeDefUser(RenamerPhase.GetRandomName(), RenamerPhase.GetRandomName(), module.CorLibTypes.Object.TypeDefOrRef);
            fakeattrib.Attributes = TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.WindowsRuntime;
            module.Types.Add(fakeattrib);
        }
    }

    public static void junkattrib(ModuleDefMD module)
    {
        for (int i = 0; i < 100; i++)
        {
            var junkattribute = new TypeDefUser(RenamerPhase.GetRandomName(), RenamerPhase.GetRandomName(), module.CorLibTypes.Object.TypeDefOrRef);
            module.Types.Add(junkattribute);

        }
    }

    public static void junkattrib2(ModuleDefMD module)
    {
        for (int i = 0; i < 100; i++)
        {
            var junkattribute = new TypeDefUser(RenamerPhase.GetRandomName(), RenamerPhase.GetRandomName(), module.CorLibTypes.Object.TypeDefOrRef);
            module.Types.Add(junkattribute);

        }
    }

    public static void antiDe4Dot(ModuleDefMD module)
    {
        Random rnd = new Random();
        InterfaceImpl Interface = new InterfaceImplUser(module.GlobalType);
        for (int i = 200; i < 300; i++)
        {
            TypeDef typedef = new TypeDefUser("", $"Form{i.ToString()}", module.CorLibTypes.GetTypeRef("System", "Attribute"));
            InterfaceImpl interface1 = new InterfaceImplUser(typedef);
            module.Types.Add(typedef);
            typedef.Interfaces.Add(interface1);
            typedef.Interfaces.Add(Interface);
        }
    }

    public static void encryptString(ModuleDef module)
    {
        foreach (TypeDef type in module.Types)
        {
            foreach (MethodDef method in type.Methods)
            {
                if (method.Body == null) continue;
                method.Body.SimplifyBranches();
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                    {
                        string base64toencrypt = method.Body.Instructions[i].Operand.ToString();
                        string base64EncryptedString = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(base64toencrypt));
                        method.Body.Instructions[i].OpCode = OpCodes.Nop;
                        method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, module.Import(typeof(System.Text.Encoding).GetMethod("get_UTF8", new Type[] { }))));
                        method.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Ldstr, base64EncryptedString));
                        method.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Call, module.Import(typeof(System.Convert).GetMethod("FromBase64String", new Type[] { typeof(string) }))));
                        method.Body.Instructions.Insert(i + 4, new Instruction(OpCodes.Callvirt, module.Import(typeof(System.Text.Encoding).GetMethod("GetString", new Type[] { typeof(byte[]) }))));
                        i += 4;
                    }
                }
            }
        }
    }


    static void SaveModifiedPeFile(PeFile peFile, string originalFilePath)
    {
        var fileBytes = System.IO.File.ReadAllBytes(originalFilePath);
        int sizeOfImageOffset = 0xD0;
        int sizeOfHeadersOffset = 0xD4;

        var sizeOfImageBytes = BitConverter.GetBytes(peFile.ImageNtHeaders.OptionalHeader.SizeOfImage);
        var sizeOfHeadersBytes = BitConverter.GetBytes(peFile.ImageNtHeaders.OptionalHeader.SizeOfHeaders);

        Array.Copy(sizeOfImageBytes, 0, fileBytes, sizeOfImageOffset, sizeOfImageBytes.Length);
        Array.Copy(sizeOfHeadersBytes, 0, fileBytes, sizeOfHeadersOffset, sizeOfHeadersBytes.Length);

        var newFilePath = System.IO.Path.GetDirectoryName(originalFilePath) + "\\" +
                          System.IO.Path.GetFileNameWithoutExtension(originalFilePath) + "-pe.dll";

        System.IO.File.WriteAllBytes(newFilePath, fileBytes);
    }

    static void PrintColoredMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
}