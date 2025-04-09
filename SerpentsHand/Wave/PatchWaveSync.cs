using NorthwoodLib.Pools;
using PlayerRoles;
using Respawning;
using Respawning.Waves;

namespace SerpentsHand.Wave;

[HarmonyPatch]
public static class PatchWaveSync
{
    // VT: My code all right reserved
    [HarmonyPatch(typeof(WaveUpdateMessage), nameof(WaveUpdateMessage.ServerSendUpdate))]
    [HarmonyPrefix]
    public static bool ServerSendUpdate(SpawnableWaveBase wave) => wave is not ICustomWave vtCustomWave;

    // Do not CALL THE FUKING EXILED EVENT for safty
    [HarmonyPatch(typeof(WaveSpawner), nameof(WaveSpawner.SpawnWave))]
    [HarmonyPriority(Priority.LowerThanNormal)] // ME: CRY OF JOY ABOUT IT, I CAN DO MULITPLE PREFIX, I NOW WHO TO HANDLE PRIORITY
    [HarmonyPrefix]
    public static bool SpawnWave(SpawnableWaveBase wave, ref List<ReferenceHub> __result)
    {
        if (wave is not ICustomWave vtCustomWave)
            return true;

        List<ReferenceHub> list = ListPool<ReferenceHub>.Shared.Rent();
        try
        {
            Team spawnableTeam = wave.TargetFaction.GetSpawnableTeam();
            List<ReferenceHub> availablePlayers = WaveSpawner.GetAvailablePlayers(spawnableTeam);
            int maxWaveSize = wave.MaxWaveSize;
            int num = Mathf.Min(availablePlayers.Count, maxWaveSize);
            if (num <= 0)
            {
                __result = list;
                return false;
            }
            vtCustomWave.GenerateUnit();

            var announcedWave = wave as IAnnouncedWave;
            if (announcedWave != null)
            {
                announcedWave.Announcement.PlayAnnouncement();
            }

            list.AddRange(availablePlayers.GetRange(0, num));
            vtCustomWave.SpawnPlayer(list);
        }
        catch (Exception e)
        {
            Log.Error($"Spawing {vtCustomWave.GetType().FullName}: " + e);
            return false;
        }
        __result = list;
        return false;
    }
}