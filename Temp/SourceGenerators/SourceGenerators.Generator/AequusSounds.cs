using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

namespace Aequus {
    /// <summary>(Amt Sounds: 1)</summary>
    [CompilerGenerated]
    public class AequusSounds : ILoadable {                    
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((SoundAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>Full Path: Aequus/Assets/Sounds/Items/CrossbowShoot<para>Num Variants: 1</para></summary>
        public static readonly SoundAsset CrossbowShoot = new SoundAsset("Aequus/Assets/Sounds/Items/CrossbowShoot", 1);
    }
}