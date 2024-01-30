using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using System;

namespace Aequus.Content.Equipment.Accessories.WeightedHorseshoe;

public class WeightedHorseshoe : ModItem, IUpdateItemDye {
    public static Single MaxFallSpeedMultiplier = 2f;
    public static Single DamagingFallSpeedThreshold = 11f;
    public static Single EnemyFallDamage = 75f;
    public static Single EnemyFallKnockback = 10f;
    public static Single SlimeMountFallDamageMultiplier = 2f;

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.LuckyHorseshoe;
        ItemID.Sets.ShimmerTransformToItem[ItemID.LuckyHorseshoe] = Type;
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, Boolean hideVisual) {
        if (player.grappling[0] != -1) {
            return;
        }

        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        player.maxFallSpeed *= MaxFallSpeedMultiplier;
        aequusPlayer.accWeightedHorseshoe = Item;

        Int32 gravityDirection = Math.Sign(player.gravDir);
        if (Math.Sign(player.velocity.Y) != gravityDirection) {
            return;
        }

        Single fallSpeed = Math.Abs(player.velocity.Y);
        if (fallSpeed > DamagingFallSpeedThreshold) {
            aequusPlayer.visualAfterImages = true;
        }
    }

    private void UpdateFloorEffects(Player player, AequusPlayer aequusPlayer) {
        Int32 gravDir = Math.Sign(player.gravDir);
        Single playerHeight = gravDir == -1 ? -2f : player.height + 2f;
        var floorCoordinates = new Vector2(player.position.X + player.width / 2f, player.position.Y + playerHeight);
        var floorTileCoordinates = floorCoordinates.ToTileCoordinates();
        if (!TileHelper.ScanDown(floorTileCoordinates, 3, out floorTileCoordinates)) {
            return;
        }

        var floorTile = Framing.GetTileSafely(floorTileCoordinates);
        if (floorTile.HasUnactuatedTile && (Main.tileSolid[floorTile.TileType] || Main.tileSolidTop[floorTile.TileType]) && Helper.IsFalling(aequusPlayer.transitionVelocity, player.gravDir)) {
            Single oldFallSpeed = Math.Abs(aequusPlayer.transitionVelocity.Y);
            if (oldFallSpeed > DamagingFallSpeedThreshold && player.velocity.Y < 1f) {
                Single intensity = Math.Min((oldFallSpeed - DamagingFallSpeedThreshold) / 10f, 1f);
                Int32 particleAmount = (Int32)Math.Max(60f * intensity, 5f);

                for (Int32 i = 0; i < particleAmount; i++) {
                    var d = Main.dust[WorldGen.KillTile_MakeTileDust(floorTileCoordinates.X, floorTileCoordinates.Y, floorTile)];
                    d.position = floorCoordinates;
                    d.position.X += Main.rand.NextFloat(-player.width / 2f, player.width / 2f);
                    d.position.Y += Main.rand.NextFloat(-2f, 2f);
                    d.noGravity = true;
                    d.velocity *= 0.5f;
                    d.velocity.X += Main.rand.NextFloat(-i / 10f, i / 10f);
                    d.velocity /= 0.5f;
                    d.scale *= Main.rand.NextFloat(0.9f, 1.1f + (particleAmount - i) / (Single)particleAmount);
                    if (Math.Sign(d.velocity.Y) == gravDir) {
                        d.velocity.Y *= 0.1f;
                    }
                }
                //Main.NewText($"emit a big gigashit volcano of intensity: {intensity}");
            }
        }
    }

    public void UpdateItemDye(Player player, Boolean isNotInVanitySlot, Boolean isSetToHidden, Item armorItem, Item dyeItem) {
        if (isSetToHidden) {
            return;
        }

        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        aequusPlayer.showHorseshoeAnvilRope = true;
        aequusPlayer.cHorseshoeAnvil = dyeItem.dye;

        UpdateFloorEffects(player, aequusPlayer);
    }
}