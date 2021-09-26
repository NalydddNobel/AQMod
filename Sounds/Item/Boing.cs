using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Sounds.Item
{
    public class Boing : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 0.25f;
            soundInstance.Pan = pan;
            soundInstance.Pitch = Main.rand.NextFloat(-0.2f, 0.2f);
            return soundInstance;
        }
    }
}