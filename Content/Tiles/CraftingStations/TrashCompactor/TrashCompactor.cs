using Aequus.Common.Graphics.Rendering.Tiles;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class TrashCompactor : ModTile, ISpecialTileRenderer {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
        TileObjectData.addTile(Type);
        DustType = DustID.Stone;
        AdjTiles = new int[] { TileID.Extractinator };
        AddMapEntry(new(65, 115, 75), TextHelper.GetDisplayName<TrashCompactorItem>());
    }

    private static bool IsRecipeValid(Recipe recipe) {
        if (recipe.Disabled) {
            return false;
        }

        foreach (var condition in recipe.DecraftConditions) {
            if (!condition.IsMet()) {
                return false;
            }
        }

        return true;
    }

    private static List<Item> GetResultItems(Item item) {
        if (item.IsAir || !item.CanShimmer()) {
            return null;
        }
        if (item.createTile > -1 && Main.tileFrameImportant[item.createTile] && !TileID.Sets.Torch[item.createTile]) {
            var tileObjectData = TileObjectData.GetTileData(item.createTile, item.placeStyle);
            if (tileObjectData.Width == 1 && tileObjectData.Height == 1 && !TileID.Sets.RoomNeeds.CountsAsTorch.ContainsAny(item.createTile)) {
                return null;
            }
            List<Item> resultList = null;
            for (int i = 0; i < Recipe.numRecipes; i++) {
                if (Main.recipe[i] == null || Main.recipe[i].createItem.type != item.type || !IsRecipeValid(Main.recipe[i])) {
                    continue;
                }

                // Item has multiple recipes, cannot decraft!
                if (resultList != null) {
                    return null;
                }

                resultList = Main.recipe[i].requiredItem;
            }
            return resultList;
        }
        return null;
    }

    private void UseEffects(int i, int j, int totalAmount, int itemType, int tileType, int tileStyle) {
        var spawnLocation = new Vector2(i + 0.5f, j + 2.5f) * 16f;
        for (int l = 0; l < Math.Min(totalAmount, 4); l++) {
            TrashCompactorSystem.SpewAnimations.Add(new(spawnLocation + Main.rand.NextVector2Square(-4f, 4f), itemType) {
                Time = Main.rand.Next(-10, 2) * l
            });
        }

        if (tileType > -1) {
            for (int l = 0; l < Math.Min(totalAmount, 2) * 5; l++) {
                int d = TileHelper.GetTileDust(i, j, tileType, tileStyle);
                if (d == -1 || d == Main.maxDust) {
                    continue;
                }

                Main.dust[d].position = spawnLocation + Main.rand.NextVector2Square(-4f, 4f);
                Main.dust[d].fadeIn = Main.dust[d].scale;
                Main.dust[d].scale *= 0.4f;
                Main.dust[d].velocity.X = Main.rand.NextFloat(-2f, -1f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-3f, -1f);
            }
        }
    }

    private void UseExtractinator(int i, int j, Item item, List<Item> resultItems, Player player) {
        int quantity = item.stack;
        item.stack -= quantity;
        if (item.stack <= 0) {
            item.TurnToAir();
        }
        for (int k = 0; k < resultItems.Count; k++) {
            long totalAmount = resultItems[k].stack * (long)quantity;
            long amount = totalAmount;
            int maxStack = Math.Max(resultItems[k].maxStack, 1);
            while (amount > 0) {
                var dropAmount = (int)Math.Min(amount, maxStack);
                if (dropAmount > 0) {
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j, "Aequus: Extractinator"), resultItems[k].type, dropAmount);
                }
                amount -= dropAmount;
            }

            UseEffects(i, j, (int)totalAmount, resultItems[k].type, resultItems[k].createTile, resultItems[k].placeStyle);
        }

        var key = new Point(i, j);
        if (!TrashCompactorSystem.ShakeAnimations.TryGetValue(key, out var value)) {
            value = TrashCompactorSystem.ShakeAnimations[key] = new();
        }

        value.Time = Math.Max(value.Time, MathHelper.Clamp(resultItems.Count * 2, 4f, 8f));

        SoundEngine.PlaySound(SoundID.Item22, new Vector2(i, j) * 16f);
    }

    public override void MouseOver(int i, int j) {
        var player = Main.LocalPlayer;
        var heldItem = player.HeldItemFixed();
        var resultItem = GetResultItems(heldItem);

        if (resultItem == null || resultItem.Count <= 0) {
            return;
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = resultItem[0].type;

        i -= Main.tile[i, j].TileFrameX / 18;
        j -= Main.tile[i, j].TileFrameY / 18;
        if (Aequus.GameWorldActive && player.itemAnimation == player.itemAnimationMax - 2) {
            UseExtractinator(i, j, heldItem, resultItem, player);
            Recipe.FindRecipes();
        }
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        if (++frameCounter > 6) {
            frameCounter = 0;
            frame = ++frame % 14;
        }
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawVines);
        }
        return false;
    }

    public void Render(int i, int j, byte layer) {
        var texture = TextureAssets.Tile[Type].Value;
        var drawOffset = new Vector2(0f, 2f);
        if (TrashCompactorSystem.ShakeAnimations.TryGetValue(new(i, j), out var shakeAnim)) {
            drawOffset += shakeAnim.Shake;
        }
        for (int k = i; k < i + 3; k++) {
            for (int l = j; l < j + 3; l++) {
                Main.spriteBatch.Draw(texture, new Vector2(k * 16f, l * 16f) - Main.screenPosition + drawOffset, new Rectangle(Main.tile[k, l].TileFrameX, Main.tile[k, l].TileFrameY + 54 * Main.tileFrame[Type], 16, 16), Lighting.GetColor(k, l), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}