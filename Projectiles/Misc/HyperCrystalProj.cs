using Aequus.Items.Accessories.Summon.Sentry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class HyperCrystalProj : PlayerAttachedProjBase
    {
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.timeLeft = 20;
        }

        public override void AI()
        {
            float scale = Projectile.scale;

            if (Main.player[Projectile.owner].ownedProjectileCounts[Type] > 10)
            {
                for (int i = Projectile.whoAmI; i >= 0; i--)
                {
                    if (Projectile.CheckHeredity(Main.projectile[i]))
                    {
                        Projectile.timeLeft = Math.Min(Projectile.timeLeft, 2);
                        return;
                    }
                }
            }

            GetOwnerValues(out var player, out var aequus, out int projOwner, out var santank);
            int alpha = aequus.InDanger ? 0 : 128;
            if (projOwner != -1)
                alpha = 200;
            Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, alpha, 0.1f);
            if (aequus?.accHyperCrystal != null && aequus?.hyperCrystalHidden == false)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, aequus.hyperCrystalDiameter, 0.2f);
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0f, 0.2f);
            }

            if (Projectile.scale > 0.1f)
            {
                Projectile.timeLeft = 2;
            }
            if (scale != Projectile.scale)
            {
                Projectile.netUpdate = true;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].Aequus().cMendshroom);
            MendshroomProj.DrawAura(Projectile.Center - Main.screenPosition, Projectile.scale, Projectile.Opacity * 0.2f, ModContent.Request<Texture2D>(Texture + "Aura").Value, TextureAssets.Projectile[Type].Value);
            return false;
        }
    }
}