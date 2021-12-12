using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace AQMod.Sounds.Item
{
    public class ThisIsNotASecretSound_S3K : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 0.1f;
            soundInstance.Pan = pan;
            soundInstance.Pitch = -0.5f;
            return soundInstance;
        }
    }
}