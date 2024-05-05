using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System;
using Helpers.Injection;

public static class StrToHexUtils
{
    public static string String2Hex(string str, bool space)
    {
        if (space)
            return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", " ");
        else
            return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", "");
    }
    public static string Hex2String(string mHex, object obj)
    {
        obj = 0;
        mHex = Regex.Replace(mHex, "[^0-9A-Fa-f]", "");
        if (mHex.Length % 2 != Convert.ToInt32(obj))
            mHex = mHex.Remove(mHex.Length - 1, 1);
        if (mHex.Length <= 0) return "";
        byte[] vBytes = new byte[mHex.Length / 2];
        for (int i = 0; i < mHex.Length; i += 2)
            if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                vBytes[i / 2] = 0;
        return Encoding.Default.GetString(vBytes);
    }
}
public class StringToHex
{
    public static void Execute(ModuleDefMD module)
    {
        var injDec = new Injector(module, typeof(StrToHexUtils));
        var decryptCall = injDec.FindMember("Hex2String") as MethodDef;
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
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode == OpCodes.Ldstr)
                        {
                            string oldString = method.Body.Instructions[i].Operand.ToString();
                            method.Body.Instructions[i].Operand = StrToHexUtils.String2Hex(oldString, true); //true = with space // false = without space !
                            method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Ldnull));
                            method.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Call, decryptCall));
                            i += 2;
                        }
                    }
                    instr.OptimizeBranches();
                }
            }
        }
    }
}