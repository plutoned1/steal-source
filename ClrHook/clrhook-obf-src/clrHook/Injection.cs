using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.Injection
{
    public class Injector
    {
        public ModuleDefMD TargetModule { get; }
        public Type RuntimeType { get; }
        public List<IDnlibDef> Members { get; }
        public Injector(ModuleDefMD targetModule, Type type, bool injectType = true)
        {
            TargetModule = targetModule;
            RuntimeType = type;
            Members = new List<IDnlibDef>();
            if (injectType)
                InjectType();
        }
        public Injector(ModuleDefMD targetModule, Type type, TypeDef typedef)
        {
            TargetModule = targetModule;
            RuntimeType = type;
            Members = new List<IDnlibDef>();
            InjectType(typedef);
        }
        public void InjectType(TypeDef def)
        {
            var typeModule = ModuleDefMD.Load(RuntimeType.Module);
            var typeDefs = typeModule.ResolveTypeDef(MDToken.ToRID(RuntimeType.MetadataToken));
            Members.AddRange(InjectHelper.Inject(typeDefs, def, TargetModule).ToList());
        }
        public void InjectType()
        {
            var typeModule = ModuleDefMD.Load(RuntimeType.Module);
            var typeDefs = typeModule.ResolveTypeDef(MDToken.ToRID(RuntimeType.MetadataToken));
            Members.AddRange(InjectHelper.Inject(typeDefs, TargetModule.GlobalType, TargetModule).ToList());
        }
        public IDnlibDef FindMember(string name)
        {
            foreach (var member in Members)
                if (member.Name == name)
                    return member;
            throw new Exception("Can't find the member !");
        }
    }
}
