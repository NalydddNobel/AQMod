using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Core;
using Aequus.Core.Graphics.Animations;
using Aequus.Core.Graphics.Tiles;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ObjectData;
using Terraria.Utilities;

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

    public override void NumDust(int i, int j, bool fail, ref int num) => num = 0;

    public static void UseItemAnimation(int i, int j, int totalAmount, int itemType) {
        var spawnLocation = new Vector2(i + 0.5f, j + 2.5f) * 16f;
        for (int l = 0; l < Math.Min(totalAmount, 4); l++) {
            ItemSpewingSystem.Animations.Add(new(spawnLocation + Main.rand.NextVector2Square(-4f, 4f), new(i, j), itemType) {
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
        var tileAnimation = TileAnimationSystem.GetValueOrAddDefault<AnimationTrashCompactor>(i, j);
        tileAnimation.FrameTime = 1;
        tileAnimation.Frame = 0;
        //tileAnimation.ShakeTime = Math.Max(tileAnimation.ShakeTime, MathHelper.Clamp(resultItems.Count * 2, 4f, 8f));
    }

    private static void UseExtractinator(int i, int j, Item item, TrashCompactorRecipe recipeResults, Player player) {
        if (recipeResults.Invalid) {
            return;
        }

        int quantity = !Main.mouseRight ? 1 : recipeResults.GetIngredientQuantity(item);
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
            LegacySpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawVines);
        }
        return false;
    }

    public void Render(int i, int j, byte layer) {
        var texture = TextureAssets.Tile[Type].Value;
        var drawOffset = new Vector2(0f, 2f);
        int frame = 0;
        if (TileAnimationSystem.TryGet<AnimationTrashCompactor>(i, j, out var tileAnimation)) {
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

    #region Item Spewing Animation
    public class ItemSpewingSystem : ModSystem {
        public record class Animation(Vector2 Location, Point16 TileOrigin, int ItemId) : IPoolable {
            public float AnimationTime;
            public bool SpawnedParticles;

            public bool Resting { get; set; }

            private void SpawnItemEffectParticles(Item itemInstance) {
                if (itemInstance.createTile < TileID.Dirt) {
                    return;
                }
                for (int l = 0; l < 3; l++) {
                    int d = TileHelper.GetTileDust(TileOrigin.X, TileOrigin.Y, itemInstance.createTile, itemInstance.placeStyle);
                    if (d == -1 || d == Main.maxDust) {
                        continue;
                    }

                    Main.dust[d].position = Location + Main.rand.NextVector2Square(-4f, 4f);
                    Main.dust[d].fadeIn = Main.dust[d].scale;
                    Main.dust[d].scale *= 0.4f;
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-2f, -1f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-3f, -1f);
                }
            }

            public bool Update() {
                AnimationTime += 1f;

                var itemInstance = ContentSamples.ItemsByType[ItemId];
                if (AnimationTime > 0f && !SpawnedParticles) {
                    lock (TileAnimationSystem.FromTile) {
                        var tileAnimation = TileAnimationSystem.GetValueOrAddDefault<AnimationTrashCompactor>(TileOrigin);
                        tileAnimation.ShakeTime = Math.Min(tileAnimation.ShakeTime + 2f, 4f);
                    }
                    SpawnItemEffectParticles(itemInstance);
                    SoundEngine.PlaySound(SoundID.Item102, Location);
                    SpawnedParticles = true;
                }

                return AnimationTime <= 60;
            }

            private void GetItemDrawValues(float progress, FastRandom rng, ref Vector2 drawLocation, out Color lightColor, out float opacity, out float rotation, out float scale) {
                bool noGravity = ItemID.Sets.ItemNoGravity[ItemId];
                opacity = 1f;
                rotation = (progress * 6f + rng.NextFloat(10f)) * (noGravity ? 0.03f : 1f);
                drawLocation += new Vector2(MathF.Sin(progress * rng.NextFloat(2f, 5f))).RotatedBy(progress * rng.NextFloat(0.05f, 0.1f)) * 10f;
                drawLocation.X += (1f - MathF.Pow(1f - progress, 1.5f)) * rng.NextFloat(-120f, -30f);

                float peakHeight = rng.NextFloat(20f, 60f);
                if (progress < 0.25f) {
                    if (progress < 0.1f) {
                        opacity *= progress / 0.1f;
                    }
                    drawLocation.Y -= MathF.Sin(progress * MathHelper.TwoPi) * peakHeight;
                }
                else {
                    float fallProgress = (progress - 0.25f) / 0.75f;
                    if (noGravity) {
                        drawLocation.Y += MathF.Pow(fallProgress, 2f) * -40f - peakHeight;
                    }
                    else {
                        drawLocation.Y += MathF.Pow(fallProgress, 2f) * 300f - peakHeight;
                    }
                    opacity *= 1f - MathF.Pow(fallProgress, 5f);
                }

                lightColor = LightHelper.GetLightColor(drawLocation);
                scale = rng.NextFloat(0.6f, 0.8f);
            }

            public void Draw(SpriteBatch spriteBatch) {
                if (AnimationTime < 0) {
                    return;
                }

                float progress = AnimationTime / 60f;
                var itemInstance = ContentSamples.ItemsByType[ItemId];
                var drawLocation = Location;
                float sudoRNG = (drawLocation.X * 10f + drawLocation.Y) % 100f;
                var rng = new FastRandom((int)sudoRNG);
                rng.NextSeed();

                GetItemDrawValues(progress, rng, ref drawLocation, out var lightColor, out var opacity, out var rotation, out var scale);
                Main.GetItemDrawFrame(ItemId, out var texture, out var frame);

                Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(Color.White, lightColor)) * opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                if (itemInstance.color != Color.Transparent) {
                    Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(itemInstance.color, lightColor)) * opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                }
            }
        }

        public static readonly List<Animation> Animations = new();

        public override void PreUpdateEntities() {
            lock (Animations) {
                for (int i = 0; i < Animations.Count; i++) {
                    if (!Animations[i].Update()) {
                        Animations.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public static void DrawAll() {
            lock (Animations) {
                foreach (var anim in Animations) {
                    anim.Draw(Main.spriteBatch);
                }
            }
        }

        public override void ClearWorld() {
            lock (Animations) {
                Animations.Clear();
            }
        }

        public override void Unload() {
            lock (Animations) {
                Animations.Clear();
            }
        }
    }
    #endregion
}