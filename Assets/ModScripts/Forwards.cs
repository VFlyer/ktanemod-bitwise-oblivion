using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MemberForwarding;

namespace BitwiseOblivion
{
    internal static class Forwards
    {
        [MemberForward("TwitchGame", "NotesDictionary", "TwitchPlaysAssembly")]
        [ObjectReference("TwitchGame", "Instance", "TwitchPlaysAssembly")]
        internal static extern Dictionary<int, string> NotesDictionary
        {
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)] get;
        }
        
        [MemberForward("TwitchGame", "Modules", "TwitchPlaysAssembly")]
        [ObjectReference("TwitchGame", "Instance", "TwitchPlaysAssembly")]
        internal static extern IList TwitchModules
        {
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)] get;
        }

        [MemberForward("IRCMessage", "UserNickName", "TwitchPlaysAssembly")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)]
        internal static extern string UserNickName(object __instance);

        [MemberForward("TwitchModule", "Code", "TwitchPlaysAssembly")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)]
        internal static extern string TwitchModuleID(object __instance);

        [MemberForward("TwitchModule", "BombComponent", "TwitchPlaysAssembly")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)]
        internal static extern object TwitchModuleBombComponent(object __instance);
        
        [MemberForward("BombComponent", "GetModuleDisplayName", KModkit.ReflectionHelper.GameAssembly)]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)]
        internal static extern string ModuleDisplayName(object __instance);

        [MemberForward("TwitchModule", "Solver", "TwitchPlaysAssembly")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)]
        internal static extern object TwitchModuleSolver(object __isntance);

        [MemberForward("ComponentSolver", "ModInfo", "TwitchPlaysAssembly")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)]
        internal static extern object TwitchModuleInfo(object __instance);

        [MemberForward("ModuleInformation", "moduleID", "TwitchPlaysAssembly")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.InternalCall)]
        internal static extern string TwitchModuleComponentID(object __instance);
    }
}
