using Aequus.Common.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CrabPots;

public class ObsidianCrabPot : BaseCrabPot {
    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this, rarity: ItemRarityID.Orange, value: Item.sellPrice(silver: 50)).WithRecipe((m) => {
            foreach (var item in Mod.GetContent<ModItem>()) {
                if (item.Item.createTile != ModContent.TileType<CrabPot>()) {
                    continue;
                }

                m.CreateRecipe()
                    .AddIngredient(item)
                    .AddIngredient(ItemID.Obsidian, 20)
                    .AddTile(TileID.Hellforge)
                    .Register();
            }
        }));
    }

    protected override void SetupCrabPotContent() {
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.OnlyInLiquid;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 24 };
        TileObjectData.newTile.DrawYOffset = -16;
        DustType = DustID.Obsidian;
        AddMapEntry(new(123, 90, 68), CreateMapEntryName());
    }

    protected override void CustomPreDraw(int x, int y, int waterYOffset, SpriteBatch spriteBatch, TECrabPot crabPot) {
        if (!crabPot.item.IsAir && Aequus.GameWorldActive) {
            var d = Dust.NewDustPerfect(new Vector2(x + 0.5f, y - 1.4f).ToWorldCoordinates(), crabPot.caught ? DustID.Frost : DustID.Torch, Scale: 2f);
            d.noGravity = true;
            d.velocity.X *= 0.1f;
            d.velocity.Y = -Math.Abs(d.velocity.Y * d.scale * 0.5f);
        }
    }
}