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
using Exiled.API.Enums;

namespace SerpentsHand
{
	internal sealed class EventHandlers
	{
		private Plugin _plugin;
		public EventHandlers(Plugin plugin) => _plugin = plugin;

		private int Respawns;
		private int SHRespawns;
		private CoroutineHandle calcuationCoroutine;

		public void OnRoundStarted()
		{
			Plugin.Instance.IsSpawnable = false;
			Plugin.Instance.IsForced = false;
			Respawns = 0;
			SHRespawns = 0;

			Timing.KillCoroutines(calcuationCoroutine);

			calcuationCoroutine = Timing.RunCoroutine(SpawnCalculation());
		}

		private IEnumerator<float> SpawnCalculation()
		{
			while (true)
			{
				yield return Timing.WaitForSeconds(1f);

				if (Round.IsEnded)
					break;

				if (Math.Round(Respawn.TimeUntilSpawnWave.TotalSeconds, 0) != Plugin.Instance.Config.SpawnWaveCalculation)
					continue;

				if (Respawn.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
					Plugin.Instance.IsSpawnable = Loader.Random.Next(100) <= Plugin.Instance.Config.SpawnManager.SpawnChance &&
					Respawns >= Plugin.Instance.Config.SpawnManager.RespawnDelay &&
					SHRespawns < Plugin.Instance.Config.SpawnManager.MaxSpawns || Plugin.Instance.IsForced;
			}
		}

		public void OnRespawningTeam(RespawningTeamEventArgs ev)
		{
			if (Plugin.Instance.IsSpawnable || Plugin.Instance.IsForced)
			{
				bool scpAlive = Player.List.Count(x => x.Role.Team == Team.SCPs) != 0;
				if (!scpAlive && !Plugin.Instance.Config.SpawnManager.CanSpawnWithoutScps)
					return;
				
				List<Player> players = ev.Players.GetRange(0, ev.Players.Count > Plugin.Instance.Config.SpawnManager.MaxSquad 
					? Plugin.Instance.Config.SpawnManager.MaxSquad 
					: ev.Players.Count);

				foreach (Player player in players)
				{
                    int SerpentsCount = 0;

                    if (player is null)
						continue;

                    if (SerpentsCount == 0)
                    {
                        Plugin.Instance.Config.SerpentsHandLeader.AddRole(player);
                        SerpentsCount++;
                    }
                    else if (SerpentsCount >= 1 && SerpentsCount <= 3)
                    {
                        Plugin.Instance.Config.SerpentsHandSpecialist.AddRole(player);
                        SerpentsCount++;
                    }
                    else
                    {
                        Plugin.Instance.Config.SerpentsHand.AddRole(player);
                    }
                }
				SHRespawns++;
				if (!string.IsNullOrEmpty(Plugin.Instance.Config.SpawnManager.EntryAnnoucement))
					Cassie.Message(Plugin.Instance.Config.SpawnManager.EntryAnnoucement, isSubtitles: Plugin.Instance.Config.SpawnManager.Subtitles);

				if (Plugin.Instance.Config.SpawnManager.EntryBroadcast.Duration > 0 || !string.IsNullOrEmpty(Plugin.Instance.Config.SpawnManager.EntryBroadcast.Content))
					foreach (Player player in Player.List.Where(x => x.Role.Team == Team.SCPs))
						player.Broadcast(Plugin.Instance.Config.SpawnManager.EntryBroadcast);

				Plugin.Instance.IsSpawnable = false;
				Plugin.Instance.IsForced = false;
				ev.IsAllowed = false;
				ev.NextKnownTeam = SpawnableTeamType.None;
			}

			Timing.CallDelayed(1, UpdateCounter);

			Respawns++;
		}

		public void OnEndingRound(EndingRoundEventArgs ev)
		{
			if (_plugin.Config.SerpentsHand.TrackedPlayers.Count <= 0) return;
			
			if (ev.ClassList.mtf_and_guards != 0 || ev.ClassList.scientists != 0) ev.IsRoundEnded = false;
			else if (ev.ClassList.class_ds != 0) ev.IsRoundEnded = false;
			else if (!_plugin.Config.SpawnManager.ScpsWinWithChaos && ev.ClassList.chaos_insurgents != 0) ev.IsRoundEnded = false;
		}

		public void OnSpawned(SpawnedEventArgs ev)
		{
			if(ev.Player.IsCHI && ev.Reason != SpawnReason.Respawn)
				UpdateCounter();
		}
		
		private void UpdateCounter() =>
			RoundSummary.singleton.ChaosTargetCount = Plugin.Instance.Config.SpawnManager.ScpsWinWithChaos ? 0 : Player.List.Count(p => p.IsCHI);
	}
}