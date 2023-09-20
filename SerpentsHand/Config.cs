using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SerpentsHand
{
     public sealed class Config : IConfig
     {
          [Description("Whether or not the plugin is enabled")]
          public bool IsEnabled { get; set; } = true;

          [Description("Whether or not debug messages will be shown")]
          public bool Debug { get; set; } = false;

          [Description("How many seconds before a spawnwave occurs should it calculate the spawn chance")]
          public int SpawnWaveCalculation { get; set; } = 10;

          [Description("If SCPs and Chaos win together")]
          public bool ScpAndChaosTogether { get; set; } = false;

          public SerpentsHand SerpentsHand { get; set; } = new();
     }
}
