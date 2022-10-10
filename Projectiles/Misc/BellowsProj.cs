using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Projectiles.Misc
{
    public class BellowsProj : PumpinatorProj
    {
        public bool _didDust;

        public override bool PushPlayers => false;

        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 4;
            Projectile.hide = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void DoDust()
        {
            if (!_didDust)
            {
                _didDust = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    var v = Vector2.Normalize(Projectile.velocity);
                    var spawnPos = Main.player[Projectile.owner].Center + v * (Main.player[Projectile.owner].width + 10);
                    if (Main.rand.NextBool(4))
                    {
                        var g = Gore.NewGoreDirect(Main.player[Projectile.owner].GetSource_ItemUse(Main.player[Projectile.owner].HeldItem), spawnPos,
                            v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f), GoreID.Smoke1 + Main.rand.Next(3));
                        Main.instance.LoadGore(g.type);
                        g.position -= TextureAssets.Gore[g.type].Size() / 2f;
                        g.scale = Main.rand.NextFloat(0.5f, 1.1f);
                        g.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                    var d = Dust.NewDustDirect(spawnPos, 10, 10, DustID.Smoke);
                    d.velocity *= 0.1f;
                    d.velocity += v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f);
                    d.scale = Main.rand.NextFloat(0.8f, 1.5f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
            }
        }
    }
}