using Aequus.Common.ContentTemplates.Generic;
using System;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Aequus.Content.Fishing.CrabPots;

public class ObsidianCrabPot : UnifiedCrabPot, IAddRecipes {
    public ModItem? Item { get; private set; }

    public override void Load() {
        Item = new InstancedTileItem(this, Settings: new() {
            Rare = ItemRarityID.Blue,
            Value = Terraria.Item.sellPrice(silver: 20)
        });

        Mod.AddContent(Item);
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

    void IAddRecipes.AddRecipes() {
        foreach (var otherItem in Mod.GetContent<ModItem>()) {
            if (otherItem.Item.createTile != ModContent.TileType<CrabPot>()) {
                continue;
            }

            Item!.CreateRecipe()
                .AddIngredient(otherItem)
                .AddIngredient(ItemID.Obsidian, 20)
                .AddTile(TileID.Hellforge)
                .Register();
        }
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