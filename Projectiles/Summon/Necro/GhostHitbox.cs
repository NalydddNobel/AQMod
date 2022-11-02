using Aequus.Buffs.Debuffs;
using Microsoft.Xna.Framework;
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
            Projectile.DamageType = NecromancyDamageClass.Instance;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 120;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            //AequusHelpers.dustDebug(Projectile.getRect());
            int npc = (int)Projectile.ai[0];
            if (Main.npc[npc].active)
            {
                Projectile.timeLeft = 2;
            }
            int size = (int)Projectile.ai[1];
            Projectile.position = Main.npc[npc].position - new Vector2(size / 2f);
            Projectile.width = Main.npc[npc].width + size;
            Projectile.height = Main.npc[npc].height + size;
            Projectile.wet = Main.npc[npc].wet;
            Projectile.lavaWet = Main.npc[npc].lavaWet;
            Projectile.honeyWet = Main.npc[npc].honeyWet;
            Projectile.velocity = Vector2.Normalize(Main.npc[npc].velocity) * 0.1f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SoulStolen>(), 1200);
        }
    }
}