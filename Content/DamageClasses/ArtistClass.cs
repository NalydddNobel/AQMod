using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.DamageClasses {
    public class ArtistClass : DamageClass {
        public static ArtistClass Instance { get; private set; }
        public static Color DamageColor = new(150, 255, 225);

        public override void Load() {
            Instance = this;
        }

        public override void Unload() {
            Instance = null;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
            if (damageClass == Summon || damageClass == Magic) {
                return StatInheritanceData.Full;
            }
            return base.GetModifierInheritance(damageClass);
        }
    }
}
