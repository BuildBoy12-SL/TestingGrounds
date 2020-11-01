namespace TestingGrounds.Patches
{
    using HarmonyLib;
    using Mirror;
    using UnityEngine;
    
    [HarmonyPatch(typeof(RandomSeedSync), nameof(RandomSeedSync.Start))]
    internal static class SeedPatch
    {
        private static bool Prefix(RandomSeedSync __instance)
        {
            if (State.SaveState == null)
                return true;

            if (!__instance.isLocalPlayer || !NetworkServer.active)
                return false;
            
            foreach (var workstation in Object.FindObjectsOfType<WorkStation>())
            {
                var transform = workstation.transform;
                workstation.Networkposition = new Offset
                {
                    position = transform.localPosition,
                    rotation = transform.localRotation.eulerAngles,
                    scale = Vector3.one
                };
            }

            __instance.Networkseed = State.SaveState.Seed;
            return false;
        }
    }
}