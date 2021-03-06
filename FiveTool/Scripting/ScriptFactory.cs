﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module;
using FiveTool.Scripting.BuiltIns;
using FiveTool.Scripting.Platform;
using FiveTool.Scripting.Proxies.Ausar.Module;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting
{
    internal static class ScriptFactory
    {
        private const string BuiltInsNamespace = "BuiltIns";

        private static readonly string[] BuiltInScripts =
        {
            "Help.lua", // Must be run first
            "Console.lua",
            "DataTypes.lua",
            "Dialogs.lua",
            "Memory.lua",
            "Modules.lua",
        };

        public static void Initialize()
        {
            UserData.RegisterAssembly();
            UserData.RegisterProxyType<ModuleEntryProxy, ModuleEntry>(r => new ModuleEntryProxy(r));
            UserData.RegisterProxyType<ModuleDataBlockProxy, ModuleDataBlock>(r => new ModuleDataBlockProxy(r));
            Script.GlobalOptions.Platform = new FiveToolPlatformAccessor();
            FiveToolPlatformAccessor.ClearTempFiles();
        }

        public static Script CreateScript()
        {
            var script = new Script();

            script.Globals["AusarModule"] = UserData.CreateStatic(typeof(AusarModuleProxy));

            RegisterBuiltIns(script);
            return script;
        }

        private static void RegisterBuiltIns(Script script)
        {
            DataTypeBuiltIns.Register(script);
            DialogBuiltIns.Register(script);
            ConsoleBuiltIns.Register(script);
            DumpBuiltIns.Register(script);
            MemoryBuiltIns.Register(script);

            foreach (var name in BuiltInScripts)
                RunBuiltInScript(script, name);
        }

        private static void RunBuiltInScript(Script script, string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var ns = typeof(ScriptFactory).Namespace + "." + BuiltInsNamespace;
            using (var stream = assembly.GetManifestResourceStream(ns + "." + name))
            {
                var loaded = script.LoadStream(stream, null, name);
                script.Call(loaded);
            }
        }
    }
}
