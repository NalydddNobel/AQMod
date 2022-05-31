using Aequus.Common;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class MendshroomAuraProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.timeLeft = 20;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            int projIdentity = (int)Projectile.ai[0] - 1;
            if (projIdentity > -1)
            {
                projIdentity = AequusHelpers.FindProjectileIdentity(Projectile.owner, projIdentity);
                if (projIdentity == -1 || !Main.projectile[projIdentity].active || !Main.projectile[projIdentity].TryGetGlobalProjectile<SantankSentryProjectile>(out var value))
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.Center = Main.projectile[projIdentity].Center;
                Projectile.scale = (value.dummyPlayer?.GetModPlayer<MendshroomPlayer>()?._accMendshroomDiameter).GetValueOrDefault(0f);
            }
            else
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.scale = Main.player[Projectile.owner].GetModPlayer<MendshroomPlayer>()._accMendshroomDiameter;
            }

            if (Projectile.scale > 0.1f)
            {
                Projectile.timeLeft = 2;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            AequusPlayer.DrawLegacyAura(Projectile.Center, Projectile.scale, Projectile.Opacity, new Color(10, 128, 10, 0));
            return false;
        }
    }
}