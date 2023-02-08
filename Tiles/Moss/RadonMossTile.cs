using Aequus.Graphics.RenderTargets;
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
            if ((!Main.tile[i, j - 1].IsFullySolid() || !Main.tile[i, j + 1].IsFullySolid() || !Main.tile[i + 1, j].IsFullySolid() || !Main.tile[i - 1, j].IsFullySolid()) && new FastRandom(i * i + j * j * i).Next(2) == 0)
            {
                var lighting = Lighting.GetColor(i, j);
                if (lighting.R != 0 || lighting.G != 0 || lighting.B != 0)
                {
                    RadonMossFogRenderer.Tiles.Add(new Point(i, j));
                    //SpecialTileRenderer.AddSolid(i, j, TileRenderLayer.PostDrawLiquids);
                }
            }
            return true;
        }

        public void Render(int i, int j, TileRenderLayer layer)
        {
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
                    var tile = Main.tile[i + k, j];
                    tile.ClearTile();
                    tile.HasTile = true;
                    tile.TileType = (ushort)radonMossGrass;
                    tile.TileFrameY = 144; // Framing fix, so that the TileFrame method randomizes grass better
                    WorldGen.TileFrame(i + k, j, resetFrame: true);
                    return;
                }
            }
            for (int l = -1; l <= 1; l += 2)
            {
                if (WorldGen.genRand.NextBool(4) && !Main.tile[i, j + l].HasTile && TileLoader.CanPlace(i, j + l, radonMossGrass))
                {
                    var tile = Main.tile[i, j + l];
                    tile.ClearTile();
                    tile.HasTile = true;
                    tile.TileType = (ushort)radonMossGrass;
                    WorldGen.TileFrame(i, j + l, resetFrame: true);
                    return;
                }
            }
        }

        public virtual bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style)
        {
            if (Main.tile[i, j].TileType == TileID.GrayBrick)
            {
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<RadonMossBrickTile>();
                WorldGen.SquareTileFrame(i, j, resetFrame: true);
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