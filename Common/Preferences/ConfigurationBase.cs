using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Preferences {
    [BackgroundColor(10, 10, 40, 220)]
    public abstract class ConfigurationBase : ModConfig, IPostSetupContent {
        protected class DefaultBackgroundColorAttribute : BackgroundColorAttribute {
            public DefaultBackgroundColorAttribute() : base(47, 29, 140, 180) {
            }
        }
        protected class DefaultSecondaryBackgroundColorAttributeAttribute : BackgroundColorAttribute {
            public DefaultSecondaryBackgroundColorAttributeAttribute() : base(80, 80, 130, 180) {
            }
        }
        protected class DefaultSliderColor : SliderColorAttribute {
            public DefaultSliderColor() : base(30, 50, 120, 255) {
            }
        }

        public virtual void Load(Mod mod) {
        }

        public virtual void PostSetupContent(Aequus aequus) {
        }

        public virtual void Unload() {
        }
    }
}