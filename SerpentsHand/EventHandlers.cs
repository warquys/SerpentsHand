using System;
using Exiled.Loader;
using MEC;
using PlayerRoles;

namespace SerpentsHand
{
	internal sealed class EventHandlers
	{
		private Plugin _plugin;
        public EventHandlers(Plugin plugin) => _plugin = plugin;

		public void Enable()
		{
            ServerEvents.EndingRound += OnEndingRound;
        }

		public void Disable()
		{
            ServerEvents.EndingRound -= OnEndingRound;
        }

		public void OnEndingRound(EndingRoundEventArgs ev)
		{
			int serpentsHandCount = _plugin.Config.SerpentsHandSolder.TrackedPlayers.Count +
			                        _plugin.Config.SerpentsHandSpecialist.TrackedPlayers.Count +
			                        _plugin.Config.SerpentsHandLeader.TrackedPlayers.Count;

			if (serpentsHandCount <= 0)
				return;

			bool mtfOrScientistsAlive = ev.ClassList.mtf_and_guards > 0 || ev.ClassList.scientists > 0;
			bool ciOrClassDAlive = ev.ClassList.class_ds > 0;
			bool serpentsHandAlive = serpentsHandCount > 0;

			if (mtfOrScientistsAlive && serpentsHandAlive)
			{
				ev.IsAllowed = false;
			}
			else if (ciOrClassDAlive && serpentsHandAlive)
			{
				ev.IsAllowed = false;
			}
			else
			{
				ev.IsAllowed = true;
			}
		}

		/*public void OnSpawned(SpawnedEventArgs ev)
		{
			if(ev.Player.IsCHI && ev.Reason != SpawnReason.Respawn)
				UpdateCounter();
		}

		private void UpdateCounter() =>
			RoundSummary.singleton.ExtraTargets = Plugin.Instance.Config.SpawnManager.ScpsWinWithChaos ? 0 : Player.List.Count(p => p.IsCHI);*/
	}
}