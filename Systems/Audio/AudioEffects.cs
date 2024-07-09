using Terraria.Audio;

namespace AequusRemake.Systems.Audio;

public class AudioEffects {
    /// <summary>Plays a sound which does not pan, but will get quieter with distance.</summary>
    public static void PlayUnpanningAttenuationSound(SoundStyle sound, Vector2 worldPosition, float distanceThreshold = 2000f) {
        if (Main.dedServ) {
            return;
        }

        SoundEngine.PlaySound(sound, null, (s) => {
            float distance = Vector2.Distance(worldPosition, new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f));
            if (distance > distanceThreshold) {
                return false;
            }

            s.Volume = sound.Volume * (1f - distance / distanceThreshold);
            return true;
        });
    }
}
