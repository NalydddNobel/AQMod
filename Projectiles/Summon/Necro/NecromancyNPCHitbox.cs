using Aequus.NPCs;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class NecromancyNPCHitbox : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            int npc = (int)Projectile.ai[0];
            if (!Main.npc[npc].active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Main.npc[npc].Center;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var d = target.GetGlobalNPC<DeathEffects>();
            d.zombieSoul = Math.Max(60, d.zombieSoul);
        }
    }
}