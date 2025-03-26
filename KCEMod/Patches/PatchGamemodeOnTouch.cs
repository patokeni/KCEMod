using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2Cpp;

namespace KCEMod.Patches
{
    [HarmonyPatch]
    public static class PatchGamemodeOnTouch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gamemode1PlayerHandler), "Update", new Type[] { })]
        private static void Patch_Update(Gamemode1PlayerHandler __instance)
        {
            // __instance.OnTouch();
        }
    }
}
