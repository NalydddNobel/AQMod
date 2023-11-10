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

        float brightness = accAnglerLamp.lightBrightness;
        if (Player.HeldItem.type == accAnglerLamp.Item.type) {
            Player.HeldItem.holdStyle = ItemHoldStyleID.HoldLamp;
            if (!Player.ItemTimeIsZero || Player.controlUseItem) {
                brightness = accAnglerLamp.lightUseBrightness;
            }
        }

        //Lighting.AddLight(Player.Top, accAnglerLamp.LightColor * brightness);
        Lighting.AddLight(Player.Center, accAnglerLamp.lightColor * brightness);
        //Lighting.AddLight(Player.Bottom, accAnglerLamp.LightColor * brightness);

        anglerLampTime--;
        if (anglerLampTime <= 0) {
            anglerLampTime = accAnglerLamp.consumptionRate;
            if (!AnglerLamp.ConsumeGel(Player)) {
                accAnglerLamp.Transform(Player);
                accAnglerLamp = null;
            }
        }
    }
}