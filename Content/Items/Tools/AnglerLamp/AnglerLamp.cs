using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Core.Autoloading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.AnglerLamp;

[AutoloadGlowMask]
public class AnglerLamp : ModItem {
    public static int PotSightRange { get; set; } = 300;
    public static int ConsumptionRate { get; set; } = 180;
    public static Vector3 LightColor { get; set; } = new Vector3(0.5f, 0.38f, 0.12f);
    public static float LightBrightness { get; set; } = 1.7f;
    public static float LightUseBrightness { get; set; } = 2.5f;
    public static int DebuffType { get; set; } = BuffID.Confused;
    public static int DebuffTime { get; set; } = 180;
    public static int DebuffRange { get; set; } = 240;

    private List<Dust> _dustEffects = new();

    public float animation;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(AnglerLamp.ConsumptionRate), AnglerLamp.PotSightRange / 16);

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 24;
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.useAmmo = AmmoID.Gel;
        Item.holdStyle = ItemHoldStyleID.HoldLamp;
        Item.useStyle = ItemUseStyleID.RaiseLamp;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.UseSound = SoundID.Item20;
        Item.autoReuse = true;
    }

    public override bool CanUseItem(Player player) {
        return !player.wet;
    }

    public override bool? UseItem(Player player) {
        player.ConsumeItem(ItemID.Gel);
        var lampPosition = player.MountedCenter + new Vector2(player.direction * 17f, -2f * player.gravDir);
        _dustEffects.Clear();
        for (int i = 0; i < 10; i++) {
            var d = Dust.NewDustPerfect(lampPosition, DustID.Torch, Scale: 1.5f);
            d.velocity = (i / 10f * MathHelper.TwoPi).ToRotationVector2() * 2f;
            d.noGravity = true;
            d.fadeIn = d.scale + 0.2f;
            d.noLight = true;
            _dustEffects.Add(d);
        }
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].damage > 0 && player.Distance(Main.npc[i].Center) < DebuffRange) {
                Main.npc[i].AddBuff(DebuffType, DebuffTime);
            }
        }
        return true;
    }

    public override void HoldItem(Player player) {
        for (int i = 0; i < _dustEffects.Count; i++) {
            if (!_dustEffects[i].active) {
                _dustEffects.RemoveAt(i);
                i--;
            }
            else {
                _dustEffects[i].position += player.position - player.oldPosition;
            }
        }

        if (player.wet) {
            Item.holdStyle = ItemHoldStyleID.None;
            return;
        }

        player.aggro += 200;
        player.GetModPlayer<AequusPlayer>().potSightRange += PotSightRange;
        Item.holdStyle = ItemHoldStyleID.HoldLamp;

        float brightness = player.ItemTimeIsZero ? LightBrightness : LightUseBrightness;
        Lighting.AddLight(player.Center, LightColor * brightness);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public override void PostUpdate() {
        Lighting.AddLight(Item.Center, LightColor * LightBrightness / 2f);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, MathF.Sin(animation * MathHelper.TwoPi * 2f) * animation * 0.3f, origin, scale, SpriteEffects.None, 0f);
        if (animation > 0f) {
            animation -= 0.033f;
            if (animation < 0f) {
                animation = 0f;
            }
        }
        return false;
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        animation = 0f;
    }
}