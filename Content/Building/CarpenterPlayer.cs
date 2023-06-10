using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Building {
    public class CarpenterPlayer : ModPlayer {
        public bool freeCameraGift;

        public override void Initialize() {
            freeCameraGift = false;
        }

        public override void SaveData(TagCompound tag) {
            tag["freeCameraGift"] = freeCameraGift;
        }

        public override void LoadData(TagCompound tag) {
            freeCameraGift = tag.Get("freeCameraGift", defaultValue: false);
        }
    }
}