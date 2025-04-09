using Exiled.API.Extensions;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using JetBrains.Annotations;
using PlayerRoles;
using Respawning;
using Respawning.Objectives;
using Respawning.Waves;
using SerpentsHand.Configs;
using SerpentsHand.Objective;
using SerpentsHand.Wave;

namespace SerpentsHand
{
    public sealed class Plugin : Plugin<Config>
	{
        public Harmony Harmony { get; private set; }

        #region Plugin Info
        public override string Name => PluginInfo.PLUGIN_NAME;
		public override string Author => PluginInfo.PLUGIN_AUTHORS;
		public override Version RequiredExiledVersion => new(9, 5, 1);
		public override Version Version => new (PluginInfo.PLUGIN_VERSION);
        public override string Prefix { get; } = PluginInfo.PLUGIN_GUID.ToSnakeCase();
        #endregion

        public static Plugin Instance;
		private EventHandlers eventHandlers;

        public Plugin()
        {
			Instance = this;
            Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        }

        public override void OnEnabled()
		{
            Harmony.PatchAll();
            Config.SerpentsHandSolder.Register();
			Config.SerpentsHandSpecialist.Register();
			Config.SerpentsHandLeader.Register();
			eventHandlers = new EventHandlers(this);
			eventHandlers.Enable();
            FactionInfluenceManager.Objectives.Add(new ScpKillObjective());
            WaveManager.Waves.Add(new SerpentsHandWave());
            // Set the Milestones is really importent else the client get a global error
            // Why idk, verry funny no server error, just the client get a global error.
            // No client trace, just global error 😂😂
            RespawnTokensManager.Milestones[Faction.SCP] = RespawnTokensManager.DefaultMilestone;
            // DO NOT ASK ME WHY
            // This property get set only once and is decremented the rest of the time
            // Is it a bug ? ¯\_(ツ)_/¯
            RespawnTokensManager.AvailableRespawnsLeft += 1; 
            base.OnEnabled();
		}

		public override void OnDisabled()
		{
            Harmony.UnpatchAll(Harmony.Id);
            CustomRole.UnregisterRoles(
                [// BRUH the old version was just removing every custom roles of all plugins
                    Config.SerpentsHandSolder, 
                    Config.SerpentsHandSpecialist, 
                    Config.SerpentsHandLeader
                ]);
			eventHandlers.Disable();
            WaveManager.Waves.RemoveAll(p => p is SerpentsHandWave);
            FactionInfluenceManager.Objectives.RemoveAll(p => p is ScpKillObjective);
            base.OnDisabled();
		}

        public bool IsSerpentsHand(Player player)
        {
            // YOUPI the player have no idea if it as a custom role
            // WHY NOT JUST SAY THAT PLAYER CONTAIN A SPECIFIC FIELD FOR ROLE ID
            return Config.SerpentsHandSolder.Check(player)
                || Config.SerpentsHandSpecialist.Check(player)
                || Config.SerpentsHandLeader.Check(player);
        }
	}
}
