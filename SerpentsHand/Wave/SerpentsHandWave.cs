using PlayerRoles;
using Respawning.Announcements;
using Respawning.Waves;
using Respawning.Waves.Generic;
// wave spawn SerpentsHandWave
namespace SerpentsHand.Wave;
// VT: My code all right reserved
public sealed class SerpentsHandWave : TimeBasedWave, ICustomWave, ILimitedWave, IAnnouncedWave
{
    public const int MaxLeader = 1;
    public const int MaxSpecialist = 3;

    public int InitialRespawnTokens 
    { 
        get => Plugin.Instance.Config.InitialRespawnTokens; 
        set => Plugin.Instance.Config.InitialRespawnTokens = value; 
    }
    public int RespawnTokens { get; set; }
    public override float InitialSpawnInterval => Plugin.Instance.Config.InitialSpawnInterval;
    public override int MaxWaveSize => Plugin.Instance.Config.MaxWaveSize;
    public override Faction TargetFaction => Faction.SCP;
    public WaveAnnouncementBase Announcement { get; } = new SerpentsHandAnnouncement();

    public void GenerateUnit() { }

    public override void PopulateQueue(Queue<RoleTypeId> queueToFill, int playersToSpawn)
    {
        throw new NotImplementedException();
    }

    public void SpawnPlayer(List<ReferenceHub> players)
    {
        int count;
        count = Math.Min(players.Count, MaxLeader);
        for (int i = 0; i < count; i++)
        {
            Plugin.Instance.Config.SerpentsHandLeader.AddRole(Player.Get(players[i]));
        }

        count = Math.Min(players.Count - MaxLeader, MaxSpecialist);
        for (int i = 0; i < count; i++)
        {
            Plugin.Instance.Config.SerpentsHandSpecialist.AddRole(Player.Get(players[MaxLeader + i]));
        }

        const int offset = MaxSpecialist + MaxLeader;
        count = players.Count - offset;
        for (int i = 0; i < count; i++)
        {
            Plugin.Instance.Config.SerpentsHandSolder.AddRole(Player.Get(players[offset + i]));
        }
    }
}