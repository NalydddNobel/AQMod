using Aequus;
using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Movement.WeightedHorseshoe;

public class WeightedHorseshoe : ModItem, IUpdateItemDye {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        player.maxFallSpeed *= 2f;
        aequusPlayer.accWeightedHorseshoe = Item;

        int gravityDirection = Math.Sign(player.gravDir);
        if (Math.Sign(player.velocity.Y) != gravityDirection) {
            return;
        }

        float fallSpeed = Math.Abs(player.velocity.Y);
        if (fallSpeed > 11f) {
            aequusPlayer.visualAfterImages = true;
        }
    }

    private void UpdateFloorEffects(Player player, AequusPlayer aequusPlayer) {
        const float FallThreshold = 11f;
        int gravDir = Math.Sign(player.gravDir);
        float playerHeight = gravDir == -1 ? -2f : player.height + 2f;
        var floorCoordinates = new Vector2(player.position.X + player.width / 2f, player.position.Y + playerHeight);
        var floorTileCoordinates = floorCoordinates.ToTileCoordinates();
        if (!TileHelper.ScanDown(floorTileCoordinates, 3, out floorTileCoordinates)) {
            return;
        }

        var floorTile = Framing.GetTileSafely(floorTileCoordinates);
        if (floorTile.HasUnactuatedTile && (Main.tileSolid[floorTile.TileType] || Main.tileSolidTop[floorTile.TileType]) && Helper.IsFalling(aequusPlayer.transitionVelocity, player.gravDir)) {
            float oldFallSpeed = Math.Abs(aequusPlayer.transitionVelocity.Y);
            if (oldFallSpeed > FallThreshold && player.velocity.Y < 1f) {
                float intensity = Math.Min((oldFallSpeed - FallThreshold) / 10f, 1f);
                int particleAmount = (int)Math.Max(60f * intensity, 5f);

                for (int i = 0; i < particleAmount; i++) {
                    var d = Main.dust[WorldGen.KillTile_MakeTileDust(floorTileCoordinates.X, floorTileCoordinates.Y, floorTile)];
                    d.position = floorCoordinates;
                    d.position.X += Main.rand.NextFloat(-player.width / 2f, player.width / 2f);
                    d.position.Y += Main.rand.NextFloat(-2f, 2f);
                    d.noGravity = true;
                    d.velocity *= 0.5f;
                    d.velocity.X += Main.rand.NextFloat(-i / 10f, i / 10f);
                    d.velocity /= 0.5f;
                    d.scale *= Main.rand.NextFloat(0.9f, 1.1f + (particleAmount - i) / (float)particleAmount);
                    if (Math.Sign(d.velocity.Y) == gravDir) {
                        d.velocity.Y *= 0.1f;
                    }
                }
                //Main.NewText($"emit a big gigashit volcano of intensity: {intensity}");
            }
        }
    }

    public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        if (isSetToHidden) {
            return;
        }

        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        aequusPlayer.showHorseshoeAnvilRope = true;
        aequusPlayer.cHorseshoeAnvil = dyeItem.dye;

        UpdateFloorEffects(player, aequusPlayer);
    }
}