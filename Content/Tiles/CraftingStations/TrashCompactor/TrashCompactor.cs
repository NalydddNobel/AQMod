using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Content.Graphics.GameOverlays;
using Aequus.Core.Graphics.Animations;
using Aequus.Core.Graphics.Tiles;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class TrashCompactor : ModTile, ISpecialTileRenderer, INetTileInteraction {
    public const Int32 FrameCount = 14;

    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this, rarity: ItemRarityID.Blue, value: Item.sellPrice(gold: 1)));
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[this.Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorInvalidTiles = new[] { (Int32)TileID.MagicalIceBlock, };
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.CoordinateHeights = new Int32[] { 16, 16, 16 };
        TileObjectData.addTile(this.Type);
        this.DustType = DustID.Stone;
        this.AdjTiles = new Int32[] { TileID.Extractinator };
        this.AddMapEntry(new Color(65, 115, 75), this.CreateMapEntryName());
    }

    public override void NumDust(Int32 i, Int32 j, Boolean fail, ref Int32 num) {
        num = 0;
    }

    public static void UseItemAnimation(Int32 i, Int32 j, Int32 totalAmount, Int32 itemType) {
        var spawnLocation = new Vector2(i + 0.5f, j + 2.5f) * 16f;
        for (Int32 l = 0; l < Math.Min(totalAmount, 4); l++) {
            ModContent.GetInstance<DrawsOverTilesNPCs>().Add(new AnimationItemSpew(spawnLocation + Main.rand.NextVector2Square(-4f, 4f), new(i, j), itemType) {
                AnimationTime = Main.rand.Next(-10, 2) * l - 45
            });
        }
    }

    private static void TransformItemResults(Item item, ref Int32 dropItem, ref Int64 stack) {
        if (item.makeNPC > 0) {
            dropItem = ItemID.FleshBlock;
            stack *= 4;
        }
    }

    private static void ResetTileAnimation(Int32 i, Int32 j) {
        AnimationTrashCompactor tileAnimation = AnimationSystem.GetValueOrAddDefault<AnimationTrashCompactor>(i, j);
        tileAnimation.FrameTime = 1;
        tileAnimation.Frame = 0;
        //tileAnimation.ShakeTime = Math.Max(tileAnimation.ShakeTime, MathHelper.Clamp(resultItems.Count * 2, 4f, 8f));
    }

    private static void UseExtractinator(Int32 i, Int32 j, Item item, TrashCompactorRecipe recipeResults, Player player) {
        if (recipeResults.Invalid) {
            return;
        }

        Int32 quantity = Main.keyState.IsKeyUp(Keys.LeftShift) ? 1 : recipeResults.GetIngredientQuantity(item);
        item.stack -= quantity * recipeResults.Ingredient.stack;
        if (item.stack <= 0) {
            item.TurnToAir();
        }

        if (Main.netMode != NetmodeID.SinglePlayer) {
            ModContent.GetInstance<TileInteractionPacket>().Send(i, j);
        }

        for (Int32 k = 0; k < recipeResults.Results.Count; k++) {
            var result = recipeResults.Results[k];
            Int64 totalAmount = result.stack * (Int64)quantity;
            Int32 dropType = result.type;

            TransformItemResults(result, ref dropType, ref totalAmount);

            Int64 amount = totalAmount;
            Int32 maxStack = Math.Max(result.maxStack, 1);
            while (amount > 0) {
                var dropAmount = (Int32)Math.Min(amount, maxStack);
                if (dropAmount > 0) {
                    player.GiveItem(
                        source: new EntitySource_TileInteraction(player, i, j, "Aequus: Extractinator"),
                        type: dropType,
                        stack: dropAmount,
                        getItemSettings: GetItemSettings.ItemCreatedFromItemUsage);
                }
                amount -= dropAmount;
            }

            UseItemAnimation(i, j, (Int32)totalAmount, dropType);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<PacketTrashCompactorItemAnimation>().Send(i, j, (Int32)totalAmount, dropType);
            }
        }

        ResetTileAnimation(i, j);
        SoundEngine.PlaySound(SoundID.Item22, new Vector2(i, j) * 16f);
    }

    public override void MouseOver(Int32 i, Int32 j) {
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

    public override void AnimateTile(ref Int32 frame, ref Int32 frameCounter) {
        if (++frameCounter > 6) {
            frameCounter = 0;
            frame = ++frame % FrameCount;
        }
    }

    public override void AnimateIndividualTile(Int32 type, Int32 i, Int32 j, ref Int32 frameXOffset, ref Int32 frameYOffset) {
    }

    public override Boolean PreDraw(Int32 i, Int32 j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawVines);
        }
        return false;
    }

    public void Render(Int32 i, Int32 j, Byte layer) {
        var texture = TextureAssets.Tile[Type].Value;
        var drawOffset = new Vector2(0f, 2f);
        Int32 frame = 0;
        if (AnimationSystem.TryGet<AnimationTrashCompactor>(i, j, out var tileAnimation)) {
            drawOffset += tileAnimation.Shake;
            frame = tileAnimation.Frame;
        }

        for (Int32 k = i; k < i + 3; k++) {
            for (Int32 l = j; l < j + 3; l++) {
                if (Main.tile[k, l].IsTileInvisible) {
                    continue;
                }
                var tileColor = Main.tile[k, l].IsTileFullbright ? Color.White : Lighting.GetColor(k, l);
                Main.spriteBatch.Draw(texture, new Vector2(k * 16f, l * 16f) - Main.screenPosition + drawOffset, new Rectangle(Main.tile[k, l].TileFrameX, Main.tile[k, l].TileFrameY + 54 * frame, 16, 16), tileColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }

    public void Receive(Int32 i, Int32 j, BinaryReader binaryReader, Int32 sender) {
        if (Main.netMode == NetmodeID.Server) {
            ModContent.GetInstance<TileInteractionPacket>().Send(i, j, ignoreClient: sender);
        }
        else {
            ResetTileAnimation(i, j);
            SoundEngine.PlaySound(SoundID.Item22, new Vector2(i, j) * 16f);
        }
    }
}