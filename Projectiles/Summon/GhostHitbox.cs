using Aequus.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class GhostHitbox : ModProjectile
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
            Projectile.DamageType = Aequus.NecromancyDamage;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 70;
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
            target.AddBuff(ModContent.BuffType<SoulStolen>(), 1200);
        }
    }
}