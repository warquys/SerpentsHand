using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SerpentsHand.Configs
{
    public sealed class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug messages will be shown")]
        public bool Debug { get; set; } = false;

        [Description("How many seconds before a spawnwave occurs should it calculate the spawn chance")]
        public int SpawnWaveCalculation { get; set; } = 10;

        public Roles.SerpentsHand SerpentsHand { get; set; } = new();
        public Roles.SerpentsHandLeader SerpentsHandLeader { get; set;} = new();
        public Roles.SerpentsHandSpecialist SerpentsHandSpecialist { get; set; } = new();
        public Configs.SpawnManager SpawnManager { get; set; } = new();
    }
}
