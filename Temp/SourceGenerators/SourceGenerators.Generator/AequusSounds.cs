using Aequus.Core.Assets;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;

namespace Aequus {
    /// <summary>(Amt Sounds: 29)</summary>
    [CompilerGenerated]
    public class AequusSounds : ILoadable {                    
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((SoundAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>Full Path: Aequus/Assets/Sounds/Tile/Conductive<para>Num Variants: 4</para></summary>
        public static readonly SoundAsset Conductive = new SoundAsset("Aequus/Assets/Sounds/Tile/Conductive", 4);
        /// <summary>Full Path: Aequus/Assets/Sounds/Tile/Conductive0<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset Conductive0 = new SoundAsset("Aequus/Assets/Sounds/Tile/Conductive0", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Tile/Conductive1<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset Conductive1 = new SoundAsset("Aequus/Assets/Sounds/Tile/Conductive1", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Tile/Conductive2<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset Conductive2 = new SoundAsset("Aequus/Assets/Sounds/Tile/Conductive2", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Tile/Conductive3<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset Conductive3 = new SoundAsset("Aequus/Assets/Sounds/Tile/Conductive3", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/CrossbowReload<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset CrossbowReload = new SoundAsset("Aequus/Assets/Sounds/Items/CrossbowReload", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/CrossbowShoot<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset CrossbowShoot = new SoundAsset("Aequus/Assets/Sounds/Items/CrossbowShoot", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/DaggerHit<para>Num Variants: 3</para></summary>
        public static readonly SoundAsset DaggerHit = new SoundAsset("Aequus/Assets/Sounds/OnHit/DaggerHit", 3);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/DaggerHit0<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset DaggerHit0 = new SoundAsset("Aequus/Assets/Sounds/OnHit/DaggerHit0", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/DaggerHit1<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset DaggerHit1 = new SoundAsset("Aequus/Assets/Sounds/OnHit/DaggerHit1", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/DaggerHit2<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset DaggerHit2 = new SoundAsset("Aequus/Assets/Sounds/OnHit/DaggerHit2", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/HeavySwing<para>Num Variants: 6</para></summary>
        public static readonly SoundAsset HeavySwing = new SoundAsset("Aequus/Assets/Sounds/Items/HeavySwing", 6);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/HeavySwing0<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset HeavySwing0 = new SoundAsset("Aequus/Assets/Sounds/Items/HeavySwing0", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/HeavySwing1<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset HeavySwing1 = new SoundAsset("Aequus/Assets/Sounds/Items/HeavySwing1", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/HeavySwing2<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset HeavySwing2 = new SoundAsset("Aequus/Assets/Sounds/Items/HeavySwing2", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/HeavySwing3<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset HeavySwing3 = new SoundAsset("Aequus/Assets/Sounds/Items/HeavySwing3", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/HeavySwing4<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset HeavySwing4 = new SoundAsset("Aequus/Assets/Sounds/Items/HeavySwing4", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/HeavySwing5<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset HeavySwing5 = new SoundAsset("Aequus/Assets/Sounds/Items/HeavySwing5", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/InflictStunned<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset InflictStunned = new SoundAsset("Aequus/Assets/Sounds/OnHit/InflictStunned", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Tile/OmniGemBreak<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset OmniGemBreak = new SoundAsset("Aequus/Assets/Sounds/Tile/OmniGemBreak", 1);
        /// <summary>Full Path: Aequus/Assets/Music/PollutedOcean<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset PollutedOcean = new SoundAsset("Aequus/Assets/Music/PollutedOcean", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/PowerReady<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset PowerReady = new SoundAsset("Aequus/Assets/Sounds/PowerReady", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/RopeRetract<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset RopeRetract = new SoundAsset("Aequus/Assets/Sounds/Items/RopeRetract", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/SwordHit<para>Num Variants: 4</para></summary>
        public static readonly SoundAsset SwordHit = new SoundAsset("Aequus/Assets/Sounds/OnHit/SwordHit", 4);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/SwordHit0<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset SwordHit0 = new SoundAsset("Aequus/Assets/Sounds/OnHit/SwordHit0", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/SwordHit1<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset SwordHit1 = new SoundAsset("Aequus/Assets/Sounds/OnHit/SwordHit1", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/SwordHit2<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset SwordHit2 = new SoundAsset("Aequus/Assets/Sounds/OnHit/SwordHit2", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/SwordHit3<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset SwordHit3 = new SoundAsset("Aequus/Assets/Sounds/OnHit/SwordHit3", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/UseDagger<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset UseDagger = new SoundAsset("Aequus/Assets/Sounds/Items/UseDagger", 1);
    }
}