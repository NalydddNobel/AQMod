using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Town.CarpenterNPC {
    public class CarpenterProj : ModProjectile {
        public static List<int> SelectableTiles { get; private set; }

        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.DirtBall;

        public override void Load() {
            SelectableTiles = new List<int>()
            {
                TileID.Dirt,
                TileID.Stone,
                TileID.ArgonMoss,
                TileID.ArgonMossBrick,
                TileID.BlueMoss,
                TileID.BlueMossBrick,
                TileID.BrownMoss,
                TileID.BrownMossBrick,
                TileID.GreenMoss,
                TileID.GreenMossBrick,
                TileID.KryptonMoss,
                TileID.KryptonMossBrick,
                TileID.LavaMoss,
                TileID.LavaMossBrick,
                TileID.PurpleMoss,
                TileID.PurpleMossBrick,
                TileID.RedMoss,
                TileID.RedMossBrick,
                TileID.XenonMoss,
                TileID.XenonMossBrick,
                TileID.Mud,
                TileID.WoodBlock,
                TileID.Ebonwood,
                TileID.Shadewood,
                TileID.DynastyWood,
                TileID.BlueDynastyShingles,
                TileID.RedDynastyShingles,
                TileID.RichMahogany,
                TileID.BorealWood,
                TileID.LivingWood,
                TileID.PalmWood,
                TileID.Sand,
                TileID.Sandstone,
                TileID.HardenedSand,
                TileID.CopperBrick,
                TileID.TinBrick,
                TileID.IronBrick,
                TileID.LeadBrick,
                TileID.SilverBrick,
                TileID.TungstenBrick,
                TileID.GoldBrick,
                TileID.PlatinumBrick,
                TileID.Glass,
                TileID.Granite,
                TileID.GraniteBlock,
                TileID.Marble,
                TileID.MarbleBlock,
                TileID.ClayBlock,
                TileID.RedBrick,
                TileID.Grass,
                TileID.JungleGrass,
                TileID.MushroomGrass,
                TileID.Ash,
                TileID.Silt,
                TileID.IceBlock,
                TileID.IceBrick,
                TileID.Slush,
                TileID.MushroomBlock,
                TileID.HayBlock,
                TileID.CactusBlock,
                TileID.PumpkinBlock,
                TileID.BambooBlock,
                TileID.LargeBambooBlock,
                TileID.LivingMahoganyLeaves,
                TileID.LeafBlock,
                TileID.LivingMahogany,
                TileID.BreakableIce,
                TileID.Cloud,
                TileID.RainCloud,
                TileID.Sunplate,
                TileID.Hive,
                TileID.HoneyBlock,
                TileID.CrispyHoneyBlock,
                TileID.Copper,
                TileID.Tin,
                TileID.Iron,
                TileID.Lead,
                TileID.Silver,
                TileID.Tungsten,
                TileID.Gold,
                TileID.Platinum,
                TileID.Demonite,
                TileID.Crimtane,
                TileID.Obsidian,
                TileID.Amethyst,
                TileID.Topaz,
                TileID.Sapphire,
                TileID.Emerald,
                TileID.Ruby,
                TileID.Diamond,
                TileID.AmberStoneBlock,
                TileID.SandstoneBrick,
                TileID.SnowBrick,
                TileID.Mudstone,
                TileID.IridescentBrick,
                TileID.EbonstoneBrick,
                TileID.CrimstoneBrick,
                TileID.DemoniteBrick,
                TileID.CrimstoneBrick,
                TileID.ObsidianBrick,
                TileID.HellstoneBrick,
                TileID.Coralstone,
                TileID.GrayBrick,
                TileID.AmethystGemspark,
                TileID.AmethystGemsparkOff,
                TileID.TopazGemspark,
                TileID.TopazGemsparkOff,
                TileID.SapphireGemspark,
                TileID.SapphireGemsparkOff,
                TileID.EmeraldGemspark,
                TileID.EmeraldGemsparkOff,
                TileID.RubyGemspark,
                TileID.RubyGemsparkOff,
                TileID.DiamondGemspark,
                TileID.DiamondGemsparkOff,
                TileID.AmberGemspark,
                TileID.AmberGemsparkOff,
            };
        }

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.npcProj = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 10;
            height = 10;
            return true;
        }

        public override void AI() {
            if ((int)Projectile.ai[1] == 0) {
                Projectile.velocity.Y -= 2f;
                Projectile.ai[0] = Main.rand.Next(SelectableTiles) + 1f;
                Projectile.ai[1]++;
                Projectile.netUpdate = true;
            }
            Projectile.velocity.X *= 0.99f;
            Projectile.velocity.Y += 0.15f;
            Projectile.rotation += Projectile.direction * 0.225f;
            Projectile.spriteDirection = Projectile.direction;
        }

        public override bool PreDraw(ref Color lightColor) {
            if (Projectile.ai[0] > 0f) {
                int tileID = (int)Projectile.ai[0] - 1;

                Main.instance.LoadTiles(tileID);
                var t = TextureAssets.Tile[tileID].Value;
                var frame = new Rectangle(162, 54, 16, 16);
                var origin = frame.Size() / 2f;

                var drawCoords = Projectile.Center - Main.screenPosition;

                Main.EntitySpriteDraw(t, drawCoords, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection.ToSpriteEffect(), 0);
                return false;
            }
            return true;
        }
    }
}