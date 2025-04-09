using System.Reflection;
using Exiled.Events.Commands.Hub;
using Footprinting;
using PlayerRoles;
using PlayerStatsSystem;
using Respawning;
using Respawning.Objectives;

namespace SerpentsHand.Objective;
// VT: My code all right reserved
public sealed class ScpKillObjective : FactionObjectiveBase, ICustomObjective
{
    // DO NOT CALL ServerSendUpdate, it use the index of FactionObjectiveBase inside
    // FactionInfluenceManager.Objectives, if you put an objectif before vanilla one
    // BOOM, if you send a index not register to the client BOOM 
    // That also mean EXILED as method for wave that can kill all of your connections if call 😂 

    public const float KillTimer = -10;
    public const float KillInfluence = 7f;

    readonly int usurpationIndex;

    public ScpKillObjective()
    {
        usurpationIndex = FactionInfluenceManager.Objectives.FindIndex(p => p is HumanKillObjective);
    }

    public override void OnInstanceDestroyed()
    {
        base.OnInstanceDestroyed();
        PlayerStats.OnAnyPlayerDied -= OnKill;
    }

    public override void OnInstanceCreated()
    {
        base.OnInstanceCreated();
        PlayerStats.OnAnyPlayerDied += OnKill;
    }

    private void OnKill(ReferenceHub victimHub, DamageHandlerBase dhb)
    {
        if (dhb is not AttackerDamageHandler attackerDamageHandler)
            return;

        Footprint attacker = attackerDamageHandler.Attacker;
        if (attacker.Hub == null) return;

        var killer = Player.Get(attacker.Hub);
        var victim = Player.Get(victimHub);
        if (killer == null ||  victim == null) return;

        Faction faction = Plugin.Instance.IsSerpentsHand(killer) 
                        ? Faction.SCP
                        : killer.Role.Team.GetFaction();

        if (IsValidFaction(faction) && IsValidEnemy(victim))
        {
            GrantInfluence(faction, KillInfluence);
            ReduceTimer(faction, KillTimer);
            
            var usurpation = new KillObjectiveFootprint
            {
                InfluenceReward = KillInfluence,
                TimeReward = KillTimer,
                AchievingPlayer = new ObjectiveHubFootprint(attacker),
                VictimFootprint = new ObjectiveHubFootprint(victimHub)
            };

            try
            {
                var killObjectif = (HumanKillObjective)FactionInfluenceManager.Objectives[usurpationIndex];
                killObjectif.ObjectiveFootprint = usurpation;
                killObjectif.ServerSendUpdate();
            }
            catch (Exception e)
            {
                Log.Error("Fail to send objectif completion by usurpation: " + e);
            }
        }
    }

    public bool IsValidEnemy(Player victim)
    {
        Faction faction = Plugin.Instance.IsSerpentsHand(victim)
                        ? Faction.SCP
                        : victim.Role.Team.GetFaction();
        return faction != Faction.SCP;
    }

    public override bool IsValidFaction(Faction faction)
    {
        // PATCH the GetFaction to match if the player is a SH role by returning Faction.SCP
        return faction == Faction.SCP;
    }
}
