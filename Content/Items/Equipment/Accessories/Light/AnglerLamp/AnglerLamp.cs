using Aequus.Core.Autoloading;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Aequus.Content.Items.Equipment.Accessories.Light.AnglerLamp;

[AutoloadGlowMask]
public class AnglerLamp : AnglerLampBase {
    public static int ConsumeRate = 180;

    public virtual int ConsumptionRate => ConsumeRate;
    public virtual Vector3 LightColor => new Vector3(0.5f, 0.38f, 0.12f);
    public virtual float LightBrightness => 1.7f;
    public virtual float LightUseBrightness => 2.5f;

    public override bool CanUseItem(Player player) {
        return !player.wet;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public override void PostUpdate() {
        if (Item.wet) {
            Item.Transform<AnglerLampOff>();
        }
        else {
            Lighting.AddLight(Item.Center, LightColor * LightBrightness / 2f);
        }
    }

    public override void UpdateInventory(Player player) {
        player.GetModPlayer<AequusPlayer>().accAnglerLamp = this;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accAnglerLamp = this;
    }

    public static bool ConsumeGel(Player player) {
        return player.ConsumeItem(AmmoID.Gel);
    }

    public override void Transform(Player player) {
        Item.Transform<AnglerLampOff>();
        (Item.ModItem as AnglerLampBase).animation = 1f;
        SoundEngine.PlaySound(SoundID.Item20);
    }
}