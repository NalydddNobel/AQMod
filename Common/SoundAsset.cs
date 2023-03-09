using Terraria.Audio;

namespace Aequus.Common
{
    public record SoundAsset
    {
        private SoundStyle sound;
        public readonly string Path;
        public SoundStyle Sound 
        {
            get
            {
                return sound;
            }
        }

        public float Volume;
        public float Pitch;
        public float PitchVariance;

        public SoundAsset(string path, int amount = 1)
        {
            Path = path;
            Volume = 1f;
            Pitch = 0f;
            PitchVariance = 0f;
            if (amount <= 1)
            {
                sound = new SoundStyle(path);
            }
            else
            {
                sound = new SoundStyle(path, 0, amount);
            }
        }

        internal void Unload()
        {
            sound = default;
        }

        public static implicit operator SoundStyle(SoundAsset value)
        {
            return value.Sound with
            {
                Volume = value.Volume,
                Pitch = value.Pitch,
                PitchVariance = value.PitchVariance,
            };
        }
    }
}