using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Respawning;

namespace SerpentsHand;

[HarmonyPatch]
public static class TokenMaxRespawnPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(RespawnTokensManager), nameof(RespawnTokensManager.Init))]
    public static void Init()
    {
        // if init is call multiple (probably not)
        RespawnTokensManager.AvailableRespawnsLeft += 1;
    }
}
