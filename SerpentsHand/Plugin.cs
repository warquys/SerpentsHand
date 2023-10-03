using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using System;
using Server = Exiled.Events.Handlers.Server;

namespace SerpentsHand
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "Serpents Hand";
        public override string Author => "yanox, Michal78900, Marco15453, Vicious Vikki, & Misfiy";
        public override Version RequiredExiledVersion => new Version(8, 2, 1);
        public override Version Version => new Version(8, 2, 4);

        public static Plugin Instance;

        public bool IsSpawnable = false;
        public bool IsForced = false;

        private EventHandlers eventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            Config.SerpentsHand.Register();
            eventHandlers = new EventHandlers(this);

            Server.RoundStarted += eventHandlers.OnRoundStarted;
            Server.RespawningTeam += eventHandlers.OnRespawningTeam;
            Server.EndingRound += eventHandlers.OnEndingRound;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomRole.UnregisterRoles();
            Server.RoundStarted -= eventHandlers.OnRoundStarted;
            Server.RespawningTeam -= eventHandlers.OnRespawningTeam;
            Server.EndingRound -= eventHandlers.OnEndingRound;

            eventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}
