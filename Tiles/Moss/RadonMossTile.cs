using Aequus.Graphics.Tiles;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Tiles.Moss
{
    public class RadonMossTile : ModTile, ISpecialTileRenderer, TileHooks.IDontRunVanillaRandomUpdate, TileHooks.IOnPlaceTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMoss[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(80, 90, 90));

            DustType = DustID.Ambient_DarkBrown;
            ItemDrop = ItemID.StoneBlock;
            HitSound = SoundID.Dig;

            MineResist = 3f;
            MinPick = 100;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //if (!effectOnly)
            //    Main.tile[i, j].TileType = TileID.Stone;
            //fail = true;
            //effectOnly = true;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (WorldGen.InWorld(i, j, 30) && Aequus.GameWorldActive && Main.rand.NextBool(AequusHelpers.QualityFromFPS(3, 20)))
            {
                bool solidTop = Main.tile[i, j - 1].IsFullySolid();
                if (!solidTop)
                {
                    var loc = new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f + Main.rand.NextFloat(0f, 16f));
                    if (Main.tile[i, j].IsHalfBlock || Main.tile[i, j].Slope > 0)
                    {
                        loc.Y += 8f;
                    }
                    float max = 1f;
                    ParticleSystem.New<RadonAmbientParticle>(ParticleLayer.AboveDust).Setup(loc, new Vector2(0f, Main.rand.NextFloat(-max, 0f)), Color.Black, Color.Black * 0.33f, Main.rand.NextFloat(0.5f, 1.1f), 0.2f, 0f);
                }
                //if (Main.rand.NextBool(40) && (!solidTop || !Main.tile[i, j + 1].IsFullySolid() || !Main.tile[i + 1, j].IsFullySolid() || !Main.tile[i - 1, j].IsFullySolid()))
                //{
                //    EffectsSystem.ParticlesAboveLiquids.Add(FogParticle.New(new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f + Main.rand.NextFloat(16f)),
                //        Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.25f), Color.Black, 0.005f, Main.rand.Next(4) * MathHelper.PiOver2));
                //}
            }
            if (new FastRandom(i * i + j * j * i).Next(2) == 0)
            {
                var lighting = Lighting.GetColor(i, j);
                if (lighting.R != 0 || lighting.G != 0 || lighting.B != 0)
                {
                    SpecialTileRenderer.AddSolid(i, j, TileRenderLayer.PostDrawLiquids);
                }
            }
            return true;
        }

        public void Render(int i, int j, TileRenderLayer layer)
        {
            if (!Main.tile[i, j - 1].IsFullySolid() && !Main.tile[i, j + 1].IsFullySolid() && !Main.tile[i + 1, j].IsFullySolid() && !Main.tile[i - 1, j].IsFullySolid())
            {
                return;
            }
            var rand = new FastRandom(i * i + j * j * i);
            var lighting = AequusHelpers.GetBrightestLight(new Point(i, j), 6);
            float intensity = 1f - (lighting.R + lighting.G + lighting.B) / 765f;
            intensity = MathHelper.Lerp(intensity, 1f, (float)MathHelper.Clamp(Vector2.Distance(new Vector2(i * 16f + 8f, j * 16f + 8f), Main.LocalPlayer.Center) / 600f - MathF.Sin(Main.GlobalTimeWrappedHourly * rand.NextFloat(0.1f, 0.6f)).Abs(), 0f, 1f));
            if (intensity <= 0f)
                return;
            var frame = ParticleTextures.fogParticle.Frame.Frame(0, frameY: rand.Next(ParticleTextures.fogParticle.FramesY));
            Main.spriteBatch.Draw(ParticleTextures.fogParticle.Texture.Value, new Vector2(i * 16f, j * 16f) + new Vector2(8f).RotatedBy(rand.NextFloat(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.NextFloat(0.1f, 0.2f)) - Main.screenPosition, frame, Color.Black * intensity, rand.Next(4) * MathHelper.PiOver2, ParticleTextures.fogParticle.Origin, 4f, SpriteEffects.None, 0f);
        }

        public override void RandomUpdate(int i, int j)
        {
            GrowLongMoss(i, j);
            GrowEvilPlant(i, j);
            AequusTile.SpreadCustomGrass(i, j, TileID.Stone, ModContent.TileType<RadonMossTile>(), 1, color: Main.tile[i, j].TileColor);
            AequusTile.SpreadCustomGrass(i, j, TileID.GrayBrick, ModContent.TileType<RadonMossBrickTile>(), 1, color: Main.tile[i, j].TileColor);
        }

        public static void GrowEvilPlant(int i, int j)
        {
            int checkSize = 50;
            int plant = ModContent.TileType<RadonPlantTile>();
            if (!AequusTile.CheckForType(new Rectangle(i - checkSize, j - checkSize, checkSize * 2, checkSize * 2).Fluffize(20), plant))
            {
                if (Main.tile[i, j - 1].TileType == ModContent.TileType<RadonMossGrass>())
                {
                    var tile = Main.tile[i, j - 1];
                    tile.HasTile = false;
                }
                WorldGen.PlaceTile(i, j - 1, plant, mute: true);
                if (Main.tile[i, j].TileType == plant)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendTileSquare(-1, i - 1, j - 1, 3, 3);
                }
            }
        }
        public static void GrowLongMoss(int i, int j)
        {
            int radonMossGrass = ModContent.TileType<RadonMossGrass>();
            for (int k = -1; k <= 1; k += 2)
            {
                if (WorldGen.genRand.NextBool(4) && !Main.tile[i + k, j].HasTile && TileLoader.CanPlace(i + k, j, radonMossGrass))
                {
                    WorldGen.PlaceTile(i + k, j, radonMossGrass, mute: true);
                    return;
                }
            }
            for (int l = -1; l <= 1; l += 2)
            {
                if (WorldGen.genRand.NextBool(4) && !Main.tile[i, j + l].HasTile && TileLoader.CanPlace(i, j + l, radonMossGrass))
                {
                    WorldGen.PlaceTile(i, j + l, ModContent.TileType<RadonMossGrass>(), mute: true);
                    return;
                }
            }
        }

        public virtual bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style)
        {
            if (Main.tile[i, j].TileType == TileID.GrayBrick)
            {
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<RadonMossBrickTile>();
                if (!mute)
                {
                    SoundEngine.PlaySound(SoundID.Dig, new Vector2(i * 16f + 8f, j * 16f + 8f));
                }
                return true;
            }
            return null;
        }
    }
}