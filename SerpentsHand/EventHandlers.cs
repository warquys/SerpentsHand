using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Loader;
using MEC;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerpentsHand
{
     internal sealed class EventHandlers
     {
          private Plugin plugin;
          public EventHandlers(Plugin plugin) => this.plugin = plugin;

          private int Respawns = 0;
          private int SHRespawns = 0;
          private CoroutineHandle calcuationCoroutine;

          public void OnRoundStarted()
          {
               plugin.IsSpawnable = false;
               Respawns = 0;
               SHRespawns = 0;

               if (calcuationCoroutine.IsRunning)
                    Timing.KillCoroutines(calcuationCoroutine);

               calcuationCoroutine = Timing.RunCoroutine(spawnCalculation());
          }

          private IEnumerator<float> spawnCalculation()
          {
               while (true)
               {
                    yield return Timing.WaitForSeconds(1f);

                    if (Round.IsEnded)
                         break;

                    if (Math.Round(Respawn.TimeUntilSpawnWave.TotalSeconds, 0) != plugin.Config.SpawnWaveCalculation)
                         continue;

                    if (Respawn.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
                         plugin.IsSpawnable = Loader.Random.Next(100) <= plugin.Config.SerpentsHand.SpawnChance &&
                             Respawns >= plugin.Config.SerpentsHand.RespawnDelay &&
                             SHRespawns < plugin.Config.SerpentsHand.MaxSpawns;
               }
          }

          public void OnRespawningTeam(RespawningTeamEventArgs ev)
          {
               Timing.CallDelayed(0.25f, delegate
               {
                    UpdateChaosCounter();
               });
               if (plugin.IsSpawnable)
               {
                    bool scpAlive = Player.List.Count(x => x.Role.Team == Team.SCPs) > 0;
                    if (!scpAlive && !plugin.Config.SerpentsHand.CanSpawnWithoutScps)
                         return;

                    List<Player> players = new List<Player>();
                    if (ev.Players.Count > plugin.Config.SerpentsHand.MaxSquad)
                         players = ev.Players.GetRange(0, plugin.Config.SerpentsHand.MaxSquad);
                    else
                         players = ev.Players.GetRange(0, ev.Players.Count);

                    foreach (Player player in players)
                    {
                         if (player is null)
                              continue;
                         plugin.Config.SerpentsHand.AddRole(player);
                    }
                    SHRespawns++;
                    if (!string.IsNullOrEmpty(plugin.Config.SerpentsHand.EntryAnnoucement))
                         Cassie.Message(plugin.Config.SerpentsHand.EntryAnnoucement, isSubtitles: plugin.Config.SerpentsHand.Subtitles);

                    if (plugin.Config.SerpentsHand.EntryBroadcast.Duration > 0 || !string.IsNullOrEmpty(plugin.Config.SerpentsHand.EntryBroadcast.Content))
                         foreach (Player player in Player.List.Where(x => x.Role.Team == Team.SCPs))
                              player.Broadcast(plugin.Config.SerpentsHand.EntryBroadcast);

                    plugin.IsSpawnable = false;
                    ev.IsAllowed = false;
                    ev.NextKnownTeam = SpawnableTeamType.None;
               }

               Respawns++;
          }

          public void OnEndingRound(EndingRoundEventArgs ev)
          {
               bool mtfAlive = Player.List.Any(p => p.IsNTF);
               bool ciAlive = Player.List.Any(p => p.IsCHI);
               bool scpAlive = Player.List.Any(p => p.IsScp);
               bool dclassAlive = Player.List.Any(p => p.Role.Type == RoleTypeId.ClassD);
               bool scientistsAlive = Player.List.Any(p => p.Role.Type == RoleTypeId.Scientist);
               bool shAlive = plugin.Config.SerpentsHand.TrackedPlayers.Count > 0;

               if (shAlive)
               {
                    if (mtfAlive) ev.IsRoundEnded = false;
                    if (dclassAlive) ev.IsRoundEnded = false;
                    if (scientistsAlive) ev.IsRoundEnded = false;
                    if (!plugin.Config.ScpsWinWithChaos && ciAlive) ev.IsRoundEnded = false;
               }
          }

          public void OnSpawned(SpawnedEventArgs ev)
          {
               UpdateChaosCounter();
          }

          private void UpdateChaosCounter()
          {
               RoundSummary.singleton.ChaosTargetCount = plugin.Config.ScpsWinWithChaos ? 0 : Player.List.Count(p => p.IsCHI);
          }
     }
}
