using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public class AequusProjectile : GlobalProjectile
    {
        public static int ParentProjectile;
        public static int ParentNPC;

        public int itemUsed = 0;
        public int ammoUsed = 0;
        public int npcOwner = -1;
        public int projectileOwnerIdentity = -1;
        public int projectileOwner = -1;

        public override bool InstancePerEntity => true;

        public bool FromItem => itemUsed > 0;
        public bool FromAmmo => ammoUsed > 0;
        public bool HasProjectileOwner => projectileOwnerIdentity > -1;
        public bool HasNPCOwner => npcOwner > -1;

        public override void Load()
        {
            ParentProjectile = -1;
            ParentNPC = -1;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            itemUsed = -1;
            ammoUsed = -1;
            npcOwner = ParentNPC;
            projectileOwnerIdentity = ParentProjectile;
            if (!projectile.hostile && projectile.owner > -1 && projectile.owner < Main.maxPlayers)
            {
                int projOwner = Main.player[projectile.owner].Aequus().projectileIdentity;
                if (projOwner != -1)
                {
                    projectileOwnerIdentity = projOwner;
                }
            }
            if (source is EntitySource_ItemUse_WithAmmo itemUse_WithAmmo)
            {
                itemUsed = itemUse_WithAmmo.Item.netID;
                ammoUsed = itemUse_WithAmmo.AmmoItemIdUsed;
            }
            else if (source is EntitySource_ItemUse itemUse)
            {
                itemUsed = itemUse.Item.netID;
            }
            else if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC)
                {
                    npcOwner = parent.Entity.whoAmI;
                }
                else if (parent.Entity is Projectile parentProjectile)
                {
                    projectileOwnerIdentity = parentProjectile.identity;
                }
            }
            projectileOwner = projectileOwnerIdentity;
        }

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                if (projectileOwnerIdentity > 0)
                {
                    projectileOwner = AequusHelpers.FindProjectileIdentity(projectile.owner, projectileOwnerIdentity);
                    if (projectileOwner == -1)
                    {
                        projectileOwnerIdentity = -1;
                    }
                }
            }
            return true;
        }

        public override void PostAI(Projectile projectile)
        {
            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (aequus.accGlowCore > 0)
                {
                    AequusPlayer.TeamContext = Main.player[projectile.owner].team;
                    GlowCore.AddLight(projectile, aequus.accGlowCore);
                    AequusPlayer.TeamContext = 0;
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

        public int ProjectileOwner(Projectile projectile)
        {
            return AequusHelpers.FindProjectileIdentity(projectile.owner, projectileOwnerIdentity);
        }
    }
}