using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Light.AnglerLamp;

public abstract class AnglerLampBase : ModItem, ITransformItem {
    public float animation;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(AnglerLamp.ConsumeRate));

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.useAmmo = AmmoID.Gel;
        Item.holdStyle = ItemHoldStyleID.HoldLamp;
        Item.useStyle = ItemUseStyleID.RaiseLamp;
        Item.useTime = 4;
        Item.useAnimation = 4;
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

    public abstract void Transform(Player player);
}