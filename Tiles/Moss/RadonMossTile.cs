using Aequus.Graphics;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Moss
{
    public class RadonMossTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMoss[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(8, 8, 12));

            DustType = 116;
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
                    var loc = new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f - Main.rand.NextFloat(-8f, 8f));
                    if (Main.tile[i, j].IsHalfBlock || Main.tile[i, j].Slope > 0)
                    {
                        loc.Y += 8f;
                    }
                    float max = Main.rand.NextBool(7) ? 3.5f : 1.5f;
                    EffectsSystem.ParticlesAboveDust.Add(BloomParticle.New(loc, new Vector2(0f, Main.rand.NextFloat(-max, 0f)), Color.Black, Color.Black * 0.33f, Main.rand.NextFloat(0.5f, 1.6f), 0.2f, 0f));
                }
                if (Main.rand.NextBool(40) && (!solidTop || !Main.tile[i, j + 1].IsFullySolid() || !Main.tile[i + 1, j].IsFullySolid() || !Main.tile[i - 1, j].IsFullySolid()))
                {
                    EffectsSystem.ParticlesAboveLiquids.Add(FogParticle.New(new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f + Main.rand.NextFloat(16f)),
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.25f), Color.Black, 0.005f, Main.rand.Next(4) * MathHelper.PiOver2));
                }
            }
            return true;
        }

        public override void RandomUpdate(int i, int j)
        {
            GrowMoss(i, j);
        }
        
        public static void GrowMoss(int i, int j)
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
    }
}