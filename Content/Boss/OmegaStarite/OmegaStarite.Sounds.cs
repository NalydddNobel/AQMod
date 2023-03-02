using Terraria.Audio;

namespace Aequus.Content.Boss.OmegaStarite
{
    public partial class OmegaStarite
    {
        public static SoundStyle ATK_Charge;
        public static SoundStyle ATK_Dodge;
        public static SoundStyle ATK_Force;
        public static SoundStyle ATK_LaserAmbient;
        public static SoundStyle ATK_LaserCharge;
        public static SoundStyle ATK_LaserEnd;
        public static SoundStyle ATK_LaserTransition;
        public static SoundStyle ATK_OrbitalStars;
        public static SoundStyle ATK_OrbitalTransition;
        public static SoundStyle ATK_StarBarrage;
        public static SoundStyle ATK_Stars;
        public static SoundStyle ATK_Transition;
        public static SoundStyle Dead_0;
        public static SoundStyle Dead_1;
        public static SoundStyle Hit;
        public static SoundStyle Hit_Crit;

        private void LoadSounds()
        {
            ATK_Charge = GetSound("ATK_Charge");
            ATK_Dodge = GetSound("ATK_Dodge");
            ATK_Force = GetSound("ATK_Force");
            ATK_LaserAmbient = GetSound("ATK_LaserAmbient");
            ATK_LaserCharge = GetSound("ATK_LaserCharge");
            ATK_LaserEnd = GetSound("ATK_LaserEnd");
            ATK_LaserTransition = GetSound("ATK_LaserTransition");
            ATK_OrbitalStars = GetSound("ATK_OrbitalStars", 2);
            ATK_OrbitalStars.Volume = 0.05f;
            ATK_OrbitalStars.MaxInstances = 0;
            ATK_OrbitalStars.PitchVariance = 0.4f;
            ATK_OrbitalTransition = GetSound("ATK_OrbitalTransition");
            ATK_StarBarrage = GetSound("ATK_StarBarrage");
            ATK_Stars = GetSound("ATK_Stars", 2);
            ATK_Stars.PitchVariance = 0.1f;
            ATK_Transition = GetSound("ATK_Transition", 3);
            Dead_0 = GetSound("DEAD_0");
            Dead_1 = GetSound("DEAD_1");
            Hit = GetSound("HIT", 3);
            Hit.Volume = 0.6f;
            Hit.Pitch = -0.025f;
            Hit.PitchVariance = 0.05f;
            Hit_Crit = GetSound("HIT_Crit");
        }

        private void UnloadSounds()
        {
            ATK_Charge =
            ATK_Dodge =
            ATK_Force =
            ATK_LaserAmbient =
            ATK_LaserCharge =
            ATK_LaserEnd =
            ATK_LaserTransition =
            ATK_OrbitalStars =
            ATK_OrbitalTransition =
            ATK_StarBarrage =
            ATK_Stars =
            ATK_Transition =
            Dead_0 =
            Dead_1 =
            Hit =
            Hit_Crit =
            default;
        }
    }
}