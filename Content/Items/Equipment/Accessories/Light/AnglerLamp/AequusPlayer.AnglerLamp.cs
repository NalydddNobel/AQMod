using Aequus.Content.Items.Equipment.Accessories.Light.AnglerLamp;
using Aequus.Core.Generator;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public AnglerLamp accAnglerLamp;

    public int anglerLampTime;

    public void UpdateAnglerLamp() {
        if (accAnglerLamp == null) {
            anglerLampTime = 0;
            return;
        }

        Lighting.AddLight(Player.Center, AnglerLamp.LightColor * AnglerLamp.LightBrightness);

        anglerLampTime--;
        if (anglerLampTime <= 0) {
            anglerLampTime = AnglerLamp.ConsumeRate;
            if (!AnglerLamp.ConsumeGel(Player)) {
                accAnglerLamp.Transform(Player);
                accAnglerLamp = null;
            }
        }
    }
}