using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace AQMod.Sounds.Ambience
{
    public sealed class StariteSpeech : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 0.02f;
            soundInstance.Pan = pan;
            soundInstance.Pitch *= -2f;
            return soundInstance;
        }
    }
}