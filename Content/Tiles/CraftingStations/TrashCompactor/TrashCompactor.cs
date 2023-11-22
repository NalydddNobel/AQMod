using Aequus.Common.Graphics.Rendering.Tiles;
using Aequus.Common.Networking;
using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Content.Graphics.GameOverlays;
using Aequus.Core.Graphics.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class TrashCompactor : ModTile, ISpecialTileRenderer, INetTileInteraction {
    public const int FrameCount = 14;

    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this, rarity: ItemRarityID.Blue, value: Item.sellPrice(gold: 1)));
    }

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
        AddMapEntry(new(65, 115, 75), CreateMapEntryName());
    }

    public static void UseItemAnimation(int i, int j, int totalAmount, int itemType) {
        var spawnLocation = new Vector2(i + 0.5f, j + 2.5f) * 16f;
        for (int l = 0; l < Math.Min(totalAmount, 4); l++) {
            ModContent.GetInstance<DrawsOverTilesNPCs>().Add(new AnimationItemSpew(spawnLocation + Main.rand.NextVector2Square(-4f, 4f), new(i, j), itemType) {
                AnimationTime = Main.rand.Next(-10, 2) * l - 45
            });
        }
    }

    private static void TransformItemResults(Item item, ref int dropItem, ref long stack) {
        if (item.makeNPC > 0) {
            dropItem = ItemID.FleshBlock;
            stack *= 4;
        }
    }

    private static void ResetTileAnimation(int i, int j) {
        var tileAnimation = AnimationSystem.GetValueOrAddDefault<AnimationTrashCompactor>(i, j);
        tileAnimation.FrameTime = 1;
        tileAnimation.Frame = 0;
        //tileAnimation.ShakeTime = Math.Max(tileAnimation.ShakeTime, MathHelper.Clamp(resultItems.Count * 2, 4f, 8f));
    }

    private static void UseExtractinator(int i, int j, Item item, TrashCompactorRecipe recipeResults, Player player) {
        if (recipeResults.Invalid) {
            return;
        }

        int quantity = Main.keyState.IsKeyUp(Keys.LeftShift) ? 1 : recipeResults.GetIngredientQuantity(item);
        item.stack -= quantity * recipeResults.Ingredient.stack;
        if (item.stack <= 0) {
            item.TurnToAir();
        }

        if (Main.netMode != NetmodeID.SinglePlayer) {
            ModContent.GetInstance<TileInteractionPacket>().Send(i, j);
        }

        for (int k = 0; k < recipeResults.Results.Count; k++) {
            var result = recipeResults.Results[k];
            long totalAmount = result.stack * (long)quantity;
            int dropType = result.type;

            TransformItemResults(result, ref dropType, ref totalAmount);

            long amount = totalAmount;
            int maxStack = Math.Max(result.maxStack, 1);
            while (amount > 0) {
                var dropAmount = (int)Math.Min(amount, maxStack);
                if (dropAmount > 0) {
                    player.GiveItem(
                        source: new EntitySource_TileInteraction(player, i, j, "Aequus: Extractinator"),
                        type: dropType,
                        stack: dropAmount,
                        getItemSettings: GetItemSettings.ItemCreatedFromItemUsage);
                }
                amount -= dropAmount;
            }

            UseItemAnimation(i, j, (int)totalAmount, dropType);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<PacketTrashCompactorItemAnimation>().Send(i, j, (int)totalAmount, dropType);
            }
        }

        ResetTileAnimation(i, j);
        SoundEngine.PlaySound(SoundID.Item22, new Vector2(i, j) * 16f);
    }

    public override void MouseOver(int i, int j) {
        var player = Main.LocalPlayer;
        var heldItem = player.HeldItemFixed();
        var recipeResults = TrashCompactorRecipe.FromItem(heldItem);

        if (recipeResults.Invalid) {
            return;
        }

        player.noThrow = 2;
        player.GetModPlayer<AequusPlayer>().disableItem = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = recipeResults.Results[0].type;

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
        }
        return false;
    }

    public void Render(int i, int j, byte layer) {
        var texture = TextureAssets.Tile[Type].Value;
        var drawOffset = new Vector2(0f, 2f);
        int frame = 0;
        if (AnimationSystem.TryGet<AnimationTrashCompactor>(i, j, out var tileAnimation)) {
            drawOffset += tileAnimation.Shake;
            frame = tileAnimation.Frame;
        }

        for (int k = i; k < i + 3; k++) {
            for (int l = j; l < j + 3; l++) {
                if (Main.tile[k, l].IsTileInvisible) {
                    continue;
                }
                var tileColor = Main.tile[k, l].IsTileFullbright ? Color.White : Lighting.GetColor(k, l);
                Main.spriteBatch.Draw(texture, new Vector2(k * 16f, l * 16f) - Main.screenPosition + drawOffset, new Rectangle(Main.tile[k, l].TileFrameX, Main.tile[k, l].TileFrameY + 54 * frame, 16, 16), tileColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }

    public void Receive(int i, int j, BinaryReader binaryReader, int sender) {
        if (Main.netMode == NetmodeID.Server) {
            ModContent.GetInstance<TileInteractionPacket>().Send(i, j, ignoreClient: sender);
        }
        else {
            ResetTileAnimation(i, j);
            SoundEngine.PlaySound(SoundID.Item22, new Vector2(i, j) * 16f);
        }
    }
}