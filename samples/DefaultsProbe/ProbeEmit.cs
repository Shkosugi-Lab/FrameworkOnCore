using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;

namespace DefaultsProbe
{
    /// <summary>
    /// n2cms が同梱する Castle DynamicProxy の PermissionUtil と同型(IPermission への拡張メソッド)。
    /// 互換層の権限クラスが IPermission であることの適合検証用。
    /// </summary>
    public static class ProbePermissionUtil
    {
        public static bool IsGranted(this IPermission permission)
        {
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(permission);
            try
            {
                return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Castle DynamicProxy の ModuleScope と同型(AppDomain 経由の動的アセンブリ生成・保存)。
    /// .NET で削除された Reflection.Emit API の書き換えと、書き換え後に実行時に型を作れることの検証用。
    /// </summary>
    public class ProbeTypeFactory
    {
        private readonly bool savePhysicalAssembly;

        public ProbeTypeFactory(bool savePhysicalAssembly)
        {
            this.savePhysicalAssembly = savePhysicalAssembly;
        }

        public bool CanControlPolicy()
        {
            return new SecurityPermission(SecurityPermissionFlag.ControlPolicy).IsGranted();
        }

        public ModuleBuilder CreateModule()
        {
            var assemblyName = new AssemblyName("ProbeDynamic");
            if (savePhysicalAssembly)
            {
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                    assemblyName, AssemblyBuilderAccess.RunAndSave, AppDomain.CurrentDomain.BaseDirectory);
                return assemblyBuilder.DefineDynamicModule("ProbeDynamic", "ProbeDynamic.dll", false);
            }

            var runOnly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return runOnly.DefineDynamicModule("ProbeDynamic", false);
        }

        public string Save(ModuleBuilder module)
        {
            AssemblyBuilder assemblyBuilder = (AssemblyBuilder)module.Assembly;
            assemblyBuilder.Save("ProbeDynamic.dll");
            return "ProbeDynamic.dll";
        }

        /// <summary>実行時に型を 1 つ生成する(Greet() が「こんにちは」を返す)。</summary>
        public object CreateGreeter()
        {
            var type = CreateModule().DefineType("ProbeGreeter", TypeAttributes.Public);
            var method = type.DefineMethod("Greet", MethodAttributes.Public, typeof(string), Type.EmptyTypes);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldstr, "こんにちは");
            il.Emit(OpCodes.Ret);
            return Activator.CreateInstance(type.CreateType());
        }
    }

    /// <summary>
    /// メンバー名が field のクラス(Castle DynamicProxy の FieldReference と同型)。C# 14 ではアクセサー内の
    /// field が自動生成の裏側のフィールドを指すため、変換で @field にしないと常に null を返す。その検証用。
    /// </summary>
    public class ProbeFieldHolder
    {
        private readonly string field;

        public ProbeFieldHolder(string field)
        {
            this.field = field;
        }

        public string Value
        {
            get { return field; }
        }

        public string Upper => field.ToUpperInvariant();
    }
}
