using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using HarmonyLib;
using MemberForwarding;
using UnityEngine;


namespace BitwiseOblivion
{
    internal static class Patcher
    {
        private static Harmony PatchHandler;
        private static Type GameCommandsType;
        internal static List<qkBitwiseOblivion> Modules = new List<qkBitwiseOblivion>();

        internal static void RecordStage(string str1, string str2, List<qkBitwiseOblivion> modules = null)
        {
            modules = modules ?? Modules;
            foreach (var module in modules)
            {
                module.StringIndicator.gameObject.SetActive(false);
                module.RecordStage(str1, str2);
            }
        }
        
        internal static MethodBase GetTwitchMethod(string MethodName)
        {
            return GameCommandsType.GetMethod(MethodName, AccessTools.all);
        }
        
        internal static void Patch()
        {
            if (PatchHandler == null)
            {
                GameCommandsType = ReflectionHelper.FindType("GameCommands", "TwitchPlaysAssembly");
                if (GameCommandsType != null)
                {
                    PatchHandler = new Harmony("qkrisi.bitwiseoblivion.patches");
                    PatchHandler.PatchAll();
                    MemberForwardControls.ForwardTypes("qkrisi.bitwiseoblivion.forwards", typeof(Forwards));
                }
            }
        }
    }

    [HarmonyPatch]
    internal static class NotesPatch
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return Patcher.GetTwitchMethod("SetNotes");
            yield return Patcher.GetTwitchMethod("SetNotesAppend");
        }
        
        private static char cipher(char ch, int key) {  
            if (!Letters.Contains(char.ToUpper(ch))) {
                return ch;  
            }  
  
            char d = char.IsUpper(ch) ? 'A' : 'a';  
            return (char)((((ch + key) - d) % 26) + d);
        }  
        
        private static string Encipher(string input, int key) {  
            string output = string.Empty;  
  
            foreach(char ch in input)  
                output += cipher(ch, key);  
  
            return output;  
        }

        [HarmonyPostfix]
        private static void RecordNoteChange(int index, string user)
        {
            Patcher.RecordStage(Forwards.NotesDictionary[index], Encipher(user, index+1));
        }
    }

    [HarmonyPatch]
    internal static class NamedEnqueuePatch
    {
        private static MethodBase TargetMethod()
        {
            return Patcher.GetTwitchMethod("EnqueueNamedCommand");
        }

        [HarmonyPostfix]
        internal static void RecordQueueEntry(string name, string command)
        {
            var TargetModules = Patcher.Modules.Where(module => module.TwitchTriggers).ToList();
            if (TargetModules.Count == 0)
                return;
            var m = Regex.Match(command, @"^\s*!\s*(\w+)\s+(.+)$");
            string prefix = m.Success
                ? m.Groups[1].Value.Trim()
                : Regex.Match(command, @"^\s*!\s*(.+)$").Groups[1].Value.Trim();
            int _;
            if (int.TryParse(prefix, out _))
            {
                var module = Forwards.TwitchModules.Cast<object>()
                    .FirstOrDefault(tm => Forwards.TwitchModuleID(tm) == prefix);
                if(module != null)
                    prefix = Forwards.ModuleDisplayName(Forwards.TwitchModuleBombComponent(module));
            }
            Patcher.RecordStage(prefix, name, TargetModules);
        }
    }

    [HarmonyPatch]
    internal static class UnnamedEnqueuePatch
    {
        private static MethodBase TargetMethod()
        {
            return Patcher.GetTwitchMethod("EnqueueUnnamedCommand");
        }

        [HarmonyPostfix]
        private static void RecordUnnamedQueueEntry(object msg, string command)
        {
            NamedEnqueuePatch.RecordQueueEntry(Forwards.UserNickName(msg), command);
        }
    }
}
