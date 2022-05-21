using Aequus.Items.Accessories;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public class AequusProjectile : GlobalProjectile
    {
        public int itemUsed;
        public int ammoUsed;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemUse_WithAmmo)
            {
                itemUsed = itemUse_WithAmmo.Item.netID;
                ammoUsed = itemUse_WithAmmo.AmmoItemIdUsed;
            }
            else if (source is EntitySource_ItemUse itemUse)
            {
                itemUsed = itemUse.Item.netID;
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (aequus.accGlowCore > 0)
                {
                    AequusPlayer.teamContext = Main.player[projectile.owner].team;
                    GlowCore.AddLight(projectile, aequus.accGlowCore);
                    AequusPlayer.teamContext = 0;
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (Main.player[projectile.owner].Aequus().accFrostburnSentry && Main.rand.NextBool(6))
                {
                    target.AddBuff(BuffID.Frostburn2, 240);
                }
            }
        }

        public static void DefaultToExplosion(Projectile projectile, int size, DamageClass damageClass, int timeLeft = 2)
        {
            projectile.width = size;
            projectile.height = size;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.DamageType = damageClass;
            projectile.aiStyle = -1;
            projectile.timeLeft = timeLeft;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = projectile.timeLeft + 1;
            projectile.penetrate = -1;
        }
    }
}