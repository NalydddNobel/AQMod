using Aequus.Content.Items.Equipment.Accessories.Light.AnglerLamp;
using Aequus.Core.Generator;
using Terraria;
using Terraria.ID;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public AnglerLamp accAnglerLamp;

    public int anglerLampTime;

    public void UpdateAnglerLamp() {
        if (accAnglerLamp == null) {
            if (anglerLampTime > 0) {
                AnglerLamp.ConsumeGel(Player);
                anglerLampTime = 0;
            }
            return;
        }

        if (Player.wet) {
            if (Player.HeldItem.type == accAnglerLamp.Item.type) {
                Player.HeldItem.holdStyle = ItemHoldStyleID.None;
            }
            return;
        }

        float brightness = accAnglerLamp.LightBrightness;
        if (Player.HeldItem.type == accAnglerLamp.Item.type) {
            Player.HeldItem.holdStyle = ItemHoldStyleID.HoldLamp;
            if (!Player.ItemTimeIsZero || Player.controlUseItem) {
                brightness = accAnglerLamp.LightUseBrightness;
            }
        }
        Lighting.AddLight(Player.Top, accAnglerLamp.LightColor * brightness);
        Lighting.AddLight(Player.Center, accAnglerLamp.LightColor * brightness);
        Lighting.AddLight(Player.Bottom, accAnglerLamp.LightColor * brightness);

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