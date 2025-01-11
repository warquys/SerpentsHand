using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;
using MEC;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace SerpentsHand.Roles
{
    [CustomRole(RoleTypeId.Tutorial)]
    public class SerpentsHand : CustomRole
    {
        public override uint Id { get; set; } = 3;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Serpents Hand Agent";
        public override string Description { get; set; } = "Help the SCPs by killing all other classes";
        public override string CustomInfo { get; set; } = "Serpents Hand Agent";
        public override bool IgnoreSpawnSystem { get; set; } = true;

        public override List<string> Inventory { get; set; } = new()
       {
          $"{ItemType.GunCrossvec}",
          $"{ItemType.KeycardChaosInsurgency}",
          $"{ItemType.GrenadeFlash}",
          $"{ItemType.Radio}",
          $"{ItemType.Medkit}",
          $"{ItemType.ArmorCombat}"
       };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            StaticSpawnPoints = new List<StaticSpawnPoint>
          {
             new()
             {
                Name = "Spawn Point",
                Position = new UnityEngine.Vector3(63f, 992f, -50f),
                Chance = 100
             }
          }
        };
        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
       {
          { AmmoType.Nato9, 120 }
       };

        protected override void SubscribeEvents()
        {
            Timing.CallDelayed(1f, () =>
            {
                PlayerEvent.EnteringPocketDimension += OnEnteringPocketDimension;
                PlayerEvent.Hurting += OnHurting;
                PlayerEvent.Shot += OnShot;
                PlayerEvent.ActivatingGenerator += OnActivatingGenerator;
                PlayerEvent.ChangingRole += OnChangingRole;

                base.SubscribeEvents();
            });
        }

        protected override void UnsubscribeEvents()
        {
            PlayerEvent.EnteringPocketDimension -= OnEnteringPocketDimension;
            PlayerEvent.Hurting -= OnHurting;
            PlayerEvent.Shot -= OnShot;
            PlayerEvent.ActivatingGenerator -= OnActivatingGenerator;
            PlayerEvent.ChangingRole -= OnChangingRole;

            base.UnsubscribeEvents();
        }

        private void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (Check(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker is null) return;
            if ((Check(ev.Player) || Check(ev.Attacker)) && (ev.Player.IsScp || ev.Attacker.IsScp))
                ev.IsAllowed = false;
        }

        private void OnShot(ShotEventArgs ev)
        {
            if (ev.Target != null && ev.Target.IsScp && Check(ev.Player))
                ev.CanHurt = false;
        }

        private void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
        {
            if (Check(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (Plugin.Instance.Config.SpawnManager.AutoConvertTutorial && ev.NewRole == Role && !ev.Player.IsOverwatchEnabled && !Check(ev.Player))
                AddRole(ev.Player);
        }
    }
}
