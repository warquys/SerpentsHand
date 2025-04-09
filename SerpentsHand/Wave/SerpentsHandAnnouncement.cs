using System.Text;
using PlayerRoles;
using Respawning.Waves.Generic;

namespace SerpentsHand.Wave;
// VT: My code all right reserved
public sealed class SerpentsHandAnnouncement : CustomWaveAnnouncement
{
    public override void PlayAnnouncement()
    {
        base.PlayAnnouncement();
        // NW i really like your code but, why not pass the player in a Spawn method than a PopulateQueue
        // Why send to all player the translation inside PlayAnnouncement ? because it really do an "Effet de bord"
        foreach (Player player in Player.List.Where(x => x.Role.Team == Team.SCPs))
            player.Broadcast(Plugin.Instance.Config.EntryBroadcast);
    }

    public override void CreateAnnouncementString(StringBuilder builder)
    {
        builder.AppendLine(Plugin.Instance.Config.EntryAnnouncement);
    }

    public override void CreateTranslation(StringBuilder builder)
    {
        builder.AppendLine(Plugin.Instance.Config.EntryAnnouncementTranslation);
    }
}
