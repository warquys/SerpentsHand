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
               Plugin.Instance.IsSpawnable = false;
               Respawns = 0;
               SHRespawns = 0;
               Plugin.Instance.IsForced = false;

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

                    if (Math.Round(Respawn.TimeUntilSpawnWave.TotalSeconds, 0) != Plugin.Instance.Config.SpawnWaveCalculation)
                         continue;

                    if (Respawn.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
                        Plugin.Instance.IsSpawnable = Loader.Random.Next(100) <= Plugin.Instance.Config.SerpentsHand.SpawnChance &&
                        Respawns >= Plugin.Instance.Config.SerpentsHand.RespawnDelay &&
                        SHRespawns < Plugin.Instance.Config.SerpentsHand.MaxSpawns || Plugin.Instance.IsForced;
            }
          }

          public void OnRespawningTeam(RespawningTeamEventArgs ev)
          {
               Timing.CallDelayed(0.25f, delegate
               {
                    UpdateChaosCounter();
               });
               if (Plugin.Instance.IsSpawnable || Plugin.Instance.IsForced)
               {
                    bool scpAlive = Player.List.Count(x => x.Role.Team == Team.SCPs) > 0;
                    if (!scpAlive && !Plugin.Instance.Config.SerpentsHand.CanSpawnWithoutScps)
                         return;

                    List<Player> players = new List<Player>();
                    if (ev.Players.Count > Plugin.Instance.Config.SerpentsHand.MaxSquad)
                         players = ev.Players.GetRange(0, Plugin.Instance.Config.SerpentsHand.MaxSquad);
                    else
                         players = ev.Players.GetRange(0, ev.Players.Count);

                    foreach (Player player in players)
                    {
                         if (player is null)
                              continue;
                         Plugin.Instance.Config.SerpentsHand.AddRole(player);
                    }
                    SHRespawns++;
                    if (!string.IsNullOrEmpty(Plugin.Instance.Config.SerpentsHand.EntryAnnoucement))
                         Cassie.Message(Plugin.Instance.Config.SerpentsHand.EntryAnnoucement, isSubtitles: Plugin.Instance.Config.SerpentsHand.Subtitles);

                    if (Plugin.Instance.Config.SerpentsHand.EntryBroadcast.Duration > 0 || !string.IsNullOrEmpty(Plugin.Instance.Config.SerpentsHand.EntryBroadcast.Content))
                         foreach (Player player in Player.List.Where(x => x.Role.Team == Team.SCPs))
                              player.Broadcast(Plugin.Instance.Config.SerpentsHand.EntryBroadcast);

                    Plugin.Instance.IsSpawnable = false;
                    Plugin.Instance.IsForced = false;
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
               bool shAlive = Plugin.Instance.Config.SerpentsHand.TrackedPlayers.Count > 0;

               if (shAlive)
               {
                    if (mtfAlive) ev.IsRoundEnded = false;
                    if (dclassAlive) ev.IsRoundEnded = false;
                    if (scientistsAlive) ev.IsRoundEnded = false;
                    if (!Plugin.Instance.Config.ScpsWinWithChaos && ciAlive) ev.IsRoundEnded = false;
               }
          }

          public void OnSpawned(SpawnedEventArgs ev)
          {
               UpdateChaosCounter();
          }

          private void UpdateChaosCounter()
          {
               RoundSummary.singleton.ChaosTargetCount = Plugin.Instance.Config.ScpsWinWithChaos ? 0 : Player.List.Count(p => p.IsCHI);
          }
     }
}
