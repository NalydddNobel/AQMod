using AequusRemake.Core.ContentGeneration;
using System;
using Terraria.Enums;
using Terraria.ObjectData;

namespace AequusRemake.Systems.Fishing.CrabPots;

public class ObsidianCrabPot : UnifiedCrabPot {
    public override void Load() {
        ModItem item = new InstancedTileItem(this, rarity: ItemRarityID.Orange, value: Item.sellPrice(silver: 50));

        Mod.AddContent(item);
        AequusRemake.OnAddRecipes += AddRecipes;

        void AddRecipes() {
            foreach (var otherItem in Mod.GetContent<ModItem>()) {
                if (otherItem.Item.createTile != ModContent.TileType<CrabPot>()) {
                    continue;
                }

                item.CreateRecipe()
                    .AddIngredient(otherItem)
                    .AddIngredient(ItemID.Obsidian, 20)
                    .AddTile(TileID.Hellforge)
                    .Register();
            }
        }
    }

    protected override void SetupCrabPotContent() {
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.OnlyInLiquid;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 24 };
        TileObjectData.newTile.DrawYOffset = -16;
        DustType = DustID.Obsidian;
        AddMapEntry(new Color(123, 90, 68), CreateMapEntryName());
    }

    protected override void CustomPreDraw(int x, int y, int waterYOffset, SpriteBatch spriteBatch, TECrabPot crabPot) {
        if (!crabPot.item.IsAir && GameWorldActive) {
            var d = Dust.NewDustPerfect(new Vector2(x + 0.5f, y - 1.4f).ToWorldCoordinates(), crabPot.caught ? DustID.Frost : DustID.Torch, Scale: 2f);
            d.noGravity = true;
            d.velocity.X *= 0.1f;
            d.velocity.Y = -Math.Abs(d.velocity.Y * d.scale * 0.5f);
        }
    }
}