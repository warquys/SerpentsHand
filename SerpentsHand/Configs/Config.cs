using System.ComponentModel;
using Respawning;

namespace SerpentsHand.Configs
{
    public sealed class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug messages will be shown")]
        public bool Debug { get; set; } = false;

        [Description("The message annouced by CASSIE when Serpents hand spawn. (Empty = Disabled)")]
        public string EntryAnnouncement { get; set; } = "SERPENTS HAND HASENTERED";

        [Description("The CASSIE text that is shown to the player, Subtitles must be set to true")]
        public string EntryAnnouncementTranslation { get; set; } = "Serpents Hand has entered the facility";

        [Description("The broadcast shown to SCPs when the Serpents Hand respawns.")]
        public Exiled.API.Features.Broadcast EntryBroadcast { get; set; } = new Exiled.API.Features.Broadcast("<color=orange>Serpents Hand has entered the facility!</color>");

        public Roles.SerpentsHandLeader SerpentsHandLeader { get; set;} = new();
        public Roles.SerpentsHandSpecialist SerpentsHandSpecialist { get; set; } = new();
        public Roles.SerpentsHandSolder SerpentsHandSolder { get; set; } = new();

        public float InitialSpawnInterval { get; set; } = 250;
        public int MaxWaveSize { get; set; } = 10;
        public int InitialRespawnTokens { get; set; } = RespawnTokensManager.DefaultTokensCount;

    }
}
