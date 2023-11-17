using Aequus.Common.Graphics.Rendering.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class TrashCompactor : ModTile, ISpecialTileRenderer {
    public const int FrameCount = 14;

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

    private static void UseEffects(int i, int j, int totalAmount, int itemType) {
        var spawnLocation = new Vector2(i + 0.5f, j + 2.5f) * 16f;
        for (int l = 0; l < Math.Min(totalAmount, 4); l++) {
            TrashCompactorSystem.SpewAnimations.Add(new(spawnLocation + Main.rand.NextVector2Square(-4f, 4f), new(i, j), itemType) {
                AnimationTime = Main.rand.Next(-10, 2) * l - 45
            });
        }
    }

    private static void UseExtractinator(int i, int j, Item item, TrashCompactorRecipeResults recipeResults, Player player) {
        if (recipeResults.InvalidStack) {
            return;
        }

        //int quantity = 1;
        int quantity = recipeResults.DecraftQuantity;
        item.stack -= quantity * recipeResults.Recipe.createItem.stack;
        if (item.stack <= 0) {
            item.TurnToAir();
        }

        for (int k = 0; k < recipeResults.Recipe.requiredItem.Count; k++) {
            var ingredient = recipeResults.Recipe.requiredItem[k];
            long totalAmount = ingredient.stack * (long)quantity;
            int dropType = ingredient.type;
            if (ingredient.makeNPC > 0) {
                dropType = ItemID.FleshBlock;
                totalAmount *= 4;
            }

            long amount = totalAmount;
            int maxStack = Math.Max(ingredient.maxStack, 1);
            while (amount > 0) {
                var dropAmount = (int)Math.Min(amount, maxStack);
                if (dropAmount > 0) {
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j, "Aequus: Extractinator"), dropType, dropAmount);
                }
                amount -= dropAmount;
            }

            UseEffects(i, j, (int)totalAmount, dropType);
        }

        var key = new Point(i, j);
        if (!TrashCompactorSystem.TileAnimations.TryGetValue(key, out var tileAnimation)) {
            tileAnimation = TrashCompactorSystem.TileAnimations[key] = new();
        }

        tileAnimation.FrameTime = 0;
        tileAnimation.Frame = 0;
        //tileAnimation.ShakeTime = Math.Max(tileAnimation.ShakeTime, MathHelper.Clamp(resultItems.Count * 2, 4f, 8f));

        SoundEngine.PlaySound(SoundID.Item22, new Vector2(i, j) * 16f);
    }

    public override void MouseOver(int i, int j) {
        var player = Main.LocalPlayer;
        var heldItem = player.HeldItemFixed();
        var recipeResults = TrashCompactorRecipeResults.FromItem(heldItem);

        if (recipeResults.Invalid || recipeResults.InvalidStack) {
            return;
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = recipeResults.Recipe.requiredItem[0].type;

        i -= Main.tile[i, j].TileFrameX / 18;
        j -= Main.tile[i, j].TileFrameY / 18;
        if (Aequus.GameWorldActive && player.itemAnimation == player.itemAnimationMax - 2) {
            UseExtractinator(i, j, heldItem, recipeResults, player);
            Recipe.FindRecipes();
        }
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        if (++frameCounter > 6) {
            frameCounter = 0;
            frame = ++frame % FrameCount;
        }
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawVines);
            if (!TrashCompactorSystem.TileAnimations.ContainsKey(new(i, j))) {
                TrashCompactorSystem.TileAnimations[new(i, j)] = new();
            }
        }
        return false;
    }

    public void Render(int i, int j, byte layer) {
        var texture = TextureAssets.Tile[Type].Value;
        var drawOffset = new Vector2(0f, 2f);
        int frame = Main.tileFrame[Type];
        if (TrashCompactorSystem.TileAnimations.TryGetValue(new(i, j), out var tileAnimation)) {
            drawOffset += tileAnimation.Shake;
            frame = tileAnimation.Frame;
        }

        for (int k = i; k < i + 3; k++) {
            for (int l = j; l < j + 3; l++) {
                Main.spriteBatch.Draw(texture, new Vector2(k * 16f, l * 16f) - Main.screenPosition + drawOffset, new Rectangle(Main.tile[k, l].TileFrameX, Main.tile[k, l].TileFrameY + 54 * frame, 16, 16), Lighting.GetColor(k, l), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}