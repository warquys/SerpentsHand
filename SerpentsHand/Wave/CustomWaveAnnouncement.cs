using System.Text;
using Exiled.API.Extensions;
using NorthwoodLib.Pools;
using PlayerRoles;
using Respawning.Announcements;

namespace SerpentsHand.Wave;
// VT: My code all right reserved
public abstract class CustomWaveAnnouncement : WaveAnnouncementBase, ICustomWaveAnnoncement
{
    public override void PlayAnnouncement()
    {
        // taken from NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(tts, glitchChance, jamChance);
        float num = AlphaWarheadController.Detonated ? 2.5f : 1f;
        float glitchChance = UnityEngine.Random.Range(MinGlitch, MaxGlitch) * num;
        float jamChance = UnityEngine.Random.Range(MinJam, MaxJam) * num;
        StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
        CreateAnnouncementString(stringBuilder);
        string tts = AddGlitchyPhrase(stringBuilder.ToString(), glitchChance, jamChance);
        stringBuilder.Clear();
        CreateTranslation(stringBuilder);
        string text = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
        if (text.Length > 0)
            Cassie.MessageTranslated(tts, text);
        else
            Cassie.Message(tts);
    }

    // No method for this in Exiled why not ...
    // Why do the ServerOnlyAddGlitchyPhrase play it and not return a string ?
    private string AddGlitchyPhrase(string tts, float glitchChance, float jamChance)
    {
        StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
        string[] array = tts.Split(' ');
        int pessimiste = array.Length + 2;
        int esperance = (int)(pessimiste * glitchChance * 3 /*glitch word length*/ 
                        + pessimiste * jamChance * 9 /*jam word length*/);
        stringBuilder.EnsureCapacity(esperance + tts.Length);
        for (int i = 0; i < array.Length; i++)
        {
            stringBuilder.Append(array[i]);
            if (i < array.Length - 1)
            {
                if (UnityEngine.Random.value < glitchChance)
                {
                    stringBuilder.Append(".G" + UnityEngine.Random.Range(1, 7));
                }

                if (UnityEngine.Random.value < jamChance)
                {
                    stringBuilder.Append("JAM_" + UnityEngine.Random.Range(0, 70).ToString("000") + "_" + UnityEngine.Random.Range(2, 6));
                }
            }
        }
        return stringBuilder.ToString();
    }

    public abstract void CreateTranslation(StringBuilder builder);
}

public interface ICustomWaveAnnoncement
{
    void CreateTranslation(StringBuilder builder);
}
