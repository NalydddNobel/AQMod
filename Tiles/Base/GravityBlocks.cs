using Aequus.Common;
using Aequus.Common.Rendering;
using Aequus.Common.Rendering.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Tiles.Base {
    internal interface IGravityBlock {
        public sbyte GravityType { get; set; }
        public sbyte GetReach(int i, int j) {
            return GetReachDefault(i, j, GravityBlocks.MaximumReach, GravityType);
        }

        protected static sbyte GetReachDefault(int i, int j, sbyte maxReach, sbyte gravityType) {
            var gravity = (sbyte)Math.Sign(-gravityType);
            for (sbyte l = 1; l < maxReach; l++) {
                if (Main.tile[i, j + l * gravity].IsSolid()) {
                    return l;
                }
            }
            return maxReach;
        }
    }

    public class GravityBlocks : ILoadable {
        public const sbyte MaximumReachDefault = 24;
        public static sbyte MaximumReach = MaximumReachDefault;

        public static sbyte CheckGravityBlocks(Vector2 position, int width, int height) {
            int x = (int)((position.X + width / 2f) / 16f);
            int y = (int)((position.Y + height / 2f) / 16f);
            if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x, y - MaximumReach) || !WorldGen.InWorld(x, y + MaximumReach)
                || !TileHelper.IsSectionLoaded(x, y) || !TileHelper.IsSectionLoaded(x, y - MaximumReach) || !TileHelper.IsSectionLoaded(x, y + MaximumReach)) {
                return 0;
            }

            for (int j = MaximumReach; j > -MaximumReach; j--) {

                var tile = Framing.GetTileSafely(x, y + j);
                if (tile.HasTile && TileLoader.GetTile(tile.TileType) is IGravityBlock gravityBlock) {

                    var gravity = gravityBlock.GravityType;
                    int reach = gravityBlock.GetReach(x, y + j);
                    if (Math.Sign(j) != Math.Sign(gravity) || reach < Math.Abs(j)) {
                        continue;
                    }

                    return gravity;
                }
            }
            return 0;
        }

        public void Load(Mod mod) {
            MaximumReach = MaximumReachDefault;
        }

        public void Unload() {
        }
    }

    public abstract class GravityBlockBase : ModTile, ISpecialTileRenderer, IGravityBlock {
        public sbyte GravityType { get; set; }
        public TextureAsset[] Auras { get; set; }
        public TextureAsset DustTexture { get; set; }

        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            HitSound = SoundID.Tink;
        }

        public override bool Slope(int i, int j) {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = fail ? 1 : 3;
        }

        public virtual sbyte GetReach(int i, int j) {
            return IGravityBlock.GetReachDefault(i, j, GravityBlocks.MaximumReach, GravityType);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            if (Main.tile[i, j].IsActuated || Main.tile[i, j].BlockColorAndCoating().Invisible)
                return true;

            var drawCoords = new Vector2(i * 16f + 8f, j * 16f + 8f) - Main.screenPosition + Helper.TileDrawOffset;
            int tileHeight = GetReach(i, j) - 1;
            if (tileHeight == 0)
                return true;

            SpecialTileRenderer.AddSolid(i, j, TileRenderLayer.PostDrawWalls);
            int gravity = Math.Sign(GravityType);
            float rotation = gravity == 1 ? 0f : MathHelper.Pi;
            for (int k = 0; k < Auras.Length; k++) {
                var texture = PaintsRenderer.TryGetPaintedTexture(i, j, Auras[k].Path);
                Vector2 scale = new(1f, (tileHeight * 16f + 32f) / texture.Height);
                spriteBatch.Draw(
                    texture, 
                    drawCoords, 
                    null, 
                    Color.White.UseA(0) * 0.5f,
                    rotation, 
                    new(texture.Width / 2f, texture.Height), 
                    scale,
                    SpriteEffects.None, 0f
                );
            }
            return true;
        }

        protected void DrawParticles(int i, int j, int tileHeight, SpriteBatch spriteBatch) {
            ulong seed = Helper.TileSeed(i, j);
            FastRandom rand = new(Helper.TileSeed(i, j));
            var texture = PaintsRenderer.TryGetPaintedTexture(i, j, DustTexture.Path);
            var origin = new Vector2(4f, 4f);
            var drawCoords = new Vector2(i * 16f, j * 16f + 8f) - Main.screenPosition;
            int dustAmt = (int)(rand.Next(tileHeight) / 1.5f + 2f);
            for (int k = 0; k < dustAmt * 3; k++) {
                float p = rand.Next(50) + Main.GlobalTimeWrappedHourly * rand.NextFloat(2f, 5.2f);
                p %= 50f;
                p /= 50f;
                p = (float)Math.Pow(p, 3f);
                p *= 50f;
                p -= 2f;
                var frame = new Rectangle(0, 10 * rand.Next(3), 8, 8);
                var dustDrawOffset = new Vector2(Helper.Wave(Main.GlobalTimeWrappedHourly * rand.NextFloat(0.45f, 1f), 0f, 16f), tileHeight * 16f - p * 16f);
                float opacity = rand.NextFloat(0.1f, 1f);
                float scale = rand.NextFloat(0.25f, 0.7f);
                if (p > 0f && p < tileHeight) {
                    if (p < 6f) {
                        float progress = p / 6f;
                        opacity *= progress;
                        scale *= progress;
                    }
                    spriteBatch.Draw(texture, drawCoords + dustDrawOffset, frame,
                        Color.White.UseA(0) * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }
        }
        protected virtual void OnSpecialRender(int i, int j, byte layer) {
            DrawParticles(i, j, GetReach(i, j), Main.spriteBatch);
        }

        void ISpecialTileRenderer.Render(int i, int j, byte layer) {
            OnSpecialRender(i, j, layer);
        }
    }
}