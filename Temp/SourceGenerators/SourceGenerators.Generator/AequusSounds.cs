using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

namespace Aequus {
    /// <summary>(Amt Sounds: 4)</summary>
    [CompilerGenerated]
    public class AequusSounds : ILoadable {                    
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((SoundAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>Full Path: Aequus/Assets/Sounds/Items/CrossbowReload<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset CrossbowReload = new SoundAsset("Aequus/Assets/Sounds/Items/CrossbowReload", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/CrossbowShoot<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset CrossbowShoot = new SoundAsset("Aequus/Assets/Sounds/Items/CrossbowShoot", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/OnHit/InflictStunned<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset InflictStunned = new SoundAsset("Aequus/Assets/Sounds/OnHit/InflictStunned", 1);
        /// <summary>Full Path: Aequus/Assets/Sounds/Items/RopeRetract<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset RopeRetract = new SoundAsset("Aequus/Assets/Sounds/Items/RopeRetract", 1);
    }
}