using Aequus.Items;
using Aequus.Items.Accessories.Summon.Sentry;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public class SentryAccessoriesProj : GlobalProjectile
    {
        public Player dummyPlayer;
        public bool appliedItemStatChanges;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
        {
            return projectile.sentry;
        }

        public override GlobalProjectile Clone(Projectile projectile, Projectile projectileClone)
        {
            var clone = (SentryAccessoriesProj)base.Clone(projectile, projectileClone);
            if (dummyPlayer != null)
                clone.dummyPlayer = AequusPlayer.ProjectileClone(dummyPlayer);
            return clone;
        }

        public override void SetDefaults(Projectile projectile)
        {
            dummyPlayer = null;
            appliedItemStatChanges = false;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (source is EntitySource_Parent parent)
                {
                    if (parent.Entity is Projectile parentProj)
                    {
                        if (parentProj.owner == Main.myPlayer && !parentProj.hostile
                        && parentProj.sentry && Main.player[projectile.owner].active && Main.player[parentProj.owner].Aequus().sentryInheritItem != null)
                        {
                            var aequus = Main.player[projectile.owner].Aequus();
                            var parentSentry = parentProj.GetGlobalProjectile<SentryAccessoriesProj>();
                            AequusProjectile.pWhoAmI = projectile.whoAmI;
                            AequusProjectile.pIdentity = projectile.identity;
                            try
                            {
                                foreach (var i in AequusPlayer.GetEquips(Main.player[projectile.owner]))
                                {
                                    if (SentryAccessoriesDatabase.OnShoot.TryGetValue(i.type, out var onShoot))
                                    {
                                        onShoot(source, projectile, this, parentProj, parentSentry, i, Main.player[projectile.owner], aequus);
                                    }
                                }
                            }
                            catch
                            {
                            }
                            AequusProjectile.pIdentity = -1;
                            AequusProjectile.pWhoAmI = -1;
                        }
                    }
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (projectile.hostile || projectile.owner < 0 || projectile.owner >= Main.maxPlayers || Main.player[projectile.owner].Aequus().sentryInheritItem == null)
            {
                dummyPlayer = null;
            }
        }

        public void UpdateInheritance(Projectile projectile)
        {
            if (projectile.hostile || !projectile.sentry || projectile.TurretShouldPersist() || projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                appliedItemStatChanges = false;
                return;
            }

            if (dummyPlayer == null)
            {
                dummyPlayer = AequusPlayer.ProjectileClone(Main.player[projectile.owner]);
            }
            dummyPlayer.active = true;
            dummyPlayer.dead = false;
            dummyPlayer.Center = projectile.Center;
            dummyPlayer.velocity = projectile.velocity;
            PlayerLoader.PreUpdate(dummyPlayer);
            dummyPlayer.ResetEffects();
            dummyPlayer.UpdateDyes();
            dummyPlayer.whoAmI = projectile.owner;
            dummyPlayer.Aequus().projectileIdentity = projectile.identity;
            dummyPlayer.wet = projectile.wet;
            dummyPlayer.lavaWet = projectile.lavaWet;
            dummyPlayer.honeyWet = projectile.honeyWet;
            AequusProjectile.pWhoAmI = projectile.whoAmI;
            AequusProjectile.pIdentity = projectile.identity;

            try
            {
                var aequus = Main.player[projectile.owner].Aequus();
                dummyPlayer.Aequus().accExpertBoost = aequus.accExpertBoost;
                foreach (var i in AequusPlayer.GetEquips(Main.player[projectile.owner], armor: false))
                {
                    if (SentryAccessoriesDatabase.OnAI.TryGetValue(i.type, out var ai))
                    {
                        ai(projectile, this, i.Clone(), Main.player[projectile.owner], aequus);
                    }
                    else if (aequus.accExpertBoost)
                    {
                        MechsSentry.ExpertEffect_UpdateAccessory(i, dummyPlayer);
                    }
                }
                appliedItemStatChanges = true;
            }
            catch
            {

            }

            PlayerLoader.PostUpdate(dummyPlayer);
            dummyPlayer.numMinions = 0;
            dummyPlayer.slotsMinions = 0f;
            AequusProjectile.pIdentity = -1;
            AequusProjectile.pWhoAmI = -1;
        }
    }
}