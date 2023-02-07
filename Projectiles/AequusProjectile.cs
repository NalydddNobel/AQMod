using Aequus.Buffs;
using Aequus.Common;
using Aequus.Common.ModPlayers;
using Aequus.Content;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Accessories.Summon;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Weapons.Ranged;
using Aequus.Particles;
using Aequus.Projectiles.GlobalProjs;
using Aequus.Projectiles.Misc.Bobbers;
using Aequus.Projectiles.Misc.Friendly;
using Aequus.Projectiles.Ranged;
using Aequus.Tiles.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Projectiles
{
    public class AequusProjectile : GlobalProjectile, IPostSetupContent
    {
        public static HashSet<int> HeatDamage { get; private set; }
        public static HashSet<int> BlacklistSpecialEffects { get; private set; }

        public static int pWhoAmI;
        public static int pIdentity;
        public static int pNPC;

        public bool heatDamage;
        public ushort frenzyTime;

        public int extraUpdatesTemporary;

        public int transform;
        public int timeAlive;
        public byte enemyRebound;

        /// <summary>
        /// The item source used to spawn this projectile. Defaults to 0 (<see cref="ItemID.None"/>)
        /// </summary>
        public int sourceItemUsed;
        /// <summary>
        /// The ammo source used to spawn this projectile. Defaults to 0 (<see cref="ItemID.None"/>)
        /// </summary>
        public int sourceAmmoUsed;
        /// <summary>
        /// The npc index which spawned this projectile. Be aware the NPC may have died or was swapped with another, so this value is useless for now. Defaults to -1.
        /// </summary>
        public int sourceNPC;
        /// <summary>
        /// The ID of the projectile which spawned this projectile. Defaults to 0 (<see cref="ProjectileID.None"/>).
        /// </summary>
        public int sourceProjType;
        /// <summary>
        /// The identity (<see cref="Projectile.identity"/>) of the projectile which spawned this projectile. Defaults to -1.
        /// </summary>
        public int sourceProjIdentity;
        /// <summary>
        /// An approximated index of the projectile which spawned this projectile. Defaults to -1.
        /// </summary>
        public int sourceProj;

        public override bool InstancePerEntity => true;

        public bool FromItem => sourceItemUsed > 0;
        public bool FromAmmo => sourceAmmoUsed > 0;
        public bool HasProjectileOwner => sourceProjIdentity > -1;
        public bool HasNPCOwner => sourceNPC > -1;
        public bool MissingProjectileOwner => sourceProjIdentity == -1 && sourceProj != -1;

        public AequusProjectile()
        {
            sourceNPC = -1;
            sourceProjIdentity = -1;
            sourceProj = -1;
            sourceAmmoUsed = 0;
            sourceItemUsed = 0;
            sourceProjType = 0;
            transform = 0;
            timeAlive = 0;
        }

        public override void Load()
        {
            HeatDamage = new HashSet<int>();
            BlacklistSpecialEffects = new HashSet<int>()
            {
                ProjectileID.MolotovFire,
                ProjectileID.MolotovFire2,
                ProjectileID.MolotovFire3,
                ProjectileID.VilethornBase,
                ProjectileID.NettleBurstLeft,
                ProjectileID.NettleBurstRight,
                ProjectileID.CrystalVileShardShaft,
                ProjectileID.FallingStar,
                ProjectileID.FallingStarSpawner,
                ProjectileID.StardustDragon2,
                ProjectileID.StardustDragon3,
            };
            pIdentity = -1;
            pWhoAmI = -1;
            pNPC = -1;
            On.Terraria.Projectile.Update += Projectile_Update;
        }

        private static void Projectile_Update(On.Terraria.Projectile.orig_Update orig, Projectile self, int i)
        {
            pIdentity = self.identity;
            pWhoAmI = i;

            AequusHelpers.iterations = 0;
            orig(self, i);

            if (!self.active)
                return;
            self.TryGetGlobalProjectile<AequusProjectile>(out var aequus);
            if (aequus != null)
            {
                if (aequus.extraUpdatesTemporary > 0)
                {
                    float minionSlotsOld = Main.player[self.owner].slotsMinions;
                    float minionSlots = Main.player[self.owner].slotsMinions - self.minionSlots;
                    for (int k = 0; k < aequus.extraUpdatesTemporary; k++)
                    {
                        AequusHelpers.iterations = k + 1;
                        Main.player[self.owner].slotsMinions = minionSlots;
                        orig(self, i);
                    }
                    Main.player[self.owner].slotsMinions = minionSlotsOld;
                }
            }
            pIdentity = self.identity;
            pWhoAmI = -1;
        }

        public void PostSetupContent(Aequus aequus)
        {
            var contentFile = new ContentArrayFile("HeatDamage", ProjectileID.Search);
            contentFile.AddToHashSet("Projectiles", HeatDamage);
        }

        public override void Unload()
        {
            HeatDamage?.Clear();
            HeatDamage = null;
        }

        public override void SetDefaults(Projectile projectile)
        {
            if (HeatDamage.Contains(projectile.type))
            {
                heatDamage = true;
            }
            frenzyTime = 0;
            extraUpdatesTemporary = 0;
            timeAlive = 0;
            enemyRebound = 0;
        }

        public void InheritPreviousSourceData(Projectile projectile, Projectile parent)
        {
            if (projectile.owner == parent.owner && parent.TryGetGlobalProjectile<AequusProjectile>(out var aequus))
            {
                if (projectile.friendly && projectile.timeLeft > 4)
                {
                    if (aequus.sourceItemUsed != 0)
                    {
                        sourceItemUsed = aequus.sourceItemUsed;
                    }
                    if (aequus.sourceAmmoUsed != 0)
                    {
                        sourceAmmoUsed = aequus.sourceAmmoUsed;
                    }
                }
            }
        }
        public void TryInherit(Projectile projectile, IEntitySource source)
        {
            if (!projectile.hostile && projectile.HasOwner())
            {
                int projOwner = Main.player[projectile.owner].Aequus().projectileIdentity;
                if (projOwner != -1)
                {
                    sourceProjIdentity = projOwner;
                }
            }
            if (source is EntitySource_ItemUse_WithAmmo itemUse_WithAmmo)
            {
                if (itemUse_WithAmmo.Item != null)
                {
                    sourceItemUsed = itemUse_WithAmmo.Item.netID;
                    sourceAmmoUsed = itemUse_WithAmmo.AmmoItemIdUsed;
                    if (itemUse_WithAmmo.Item.ModItem is ItemHooks.IOnSpawnProjectile onSpawnHook)
                    {
                        onSpawnHook.OnCreateProjectile(projectile, this, source);
                    }
                }
            }
            else if (source is EntitySource_ItemUse itemUse)
            {
                if (itemUse.Item != null)
                    sourceItemUsed = itemUse.Item.netID;
                if (itemUse.Item.ModItem is ItemHooks.IOnSpawnProjectile onSpawnHook)
                {
                    onSpawnHook.OnCreateProjectile(projectile, this, source);
                }
            }
            else if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC)
                {
                    sourceNPC = parent.Entity.whoAmI;
                }
                else if (parent.Entity is Projectile parentProj)
                {
                    sourceProjIdentity = parentProj.identity;
                    if (parentProj.owner == Main.myPlayer && !parentProj.hostile
                    && parentProj.sentry && Main.player[projectile.owner].active && Main.player[parentProj.owner].Aequus().accSentryInheritence != null)
                    {
                        var aequus = Main.player[projectile.owner].Aequus();
                        var parentSentry = parentProj.GetGlobalProjectile<SentryAccessoriesManager>();
                        pWhoAmI = projectile.whoAmI;
                        pIdentity = projectile.identity;
                        try
                        {
                            foreach (var i in AequusPlayer.GetEquips(Main.player[projectile.owner], armor: false, sentrySlot: true))
                            {
                                if (SentryAccessoriesDatabase.OnShoot.TryGetValue(i.type, out var onShoot))
                                {
                                    onShoot(new SentryAccessoriesDatabase.OnShootInfo() { Source = source, Projectile = projectile, ParentProjectile = parentProj, Player = Main.player[projectile.owner], Accessory = i, });
                                }
                            }
                        }
                        catch
                        {
                        }
                        pIdentity = -1;
                        pWhoAmI = -1;
                    }
                }
            }
            if (sourceProjIdentity != -1)
            {
                sourceProj = AequusHelpers.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
                if (sourceProj == -1)
                {
                    sourceProjIdentity = -1;
                }
                else
                {
                    sourceProjType = Main.projectile[sourceProj].type;
                    InheritPreviousSourceData(projectile, Main.projectile[sourceProj]);
                }
            }
            if (sourceItemUsed != -1 && ItemLoader.GetItem(sourceItemUsed) is ItemHooks.IOnSpawnProjectile onSpawnHook2)
            {
                onSpawnHook2.IndirectInheritence(projectile, this, source);
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            sourceItemUsed = -1;
            sourceAmmoUsed = -1;
            sourceNPC = pNPC;
            sourceProjIdentity = pIdentity;
            sourceProj = -1;

            if (Main.gameMenu)
                return;

            try
            {
                TryInherit(projectile, source);

                if (source is EntitySource_ItemUse itemUse)
                {
                    if (projectile.type == ProjectileID.BoneGloveProj && itemUse.Entity is Player player && player.GetModPlayer<AequusPlayer>().ExpertBoost)
                    {
                        transform = ModContent.ProjectileType<Bonesaw>();
                        projectile.velocity *= 1.25f;
                        projectile.damage = (int)(projectile.damage * 1.5f);
                    }
                }
            }
            catch
            {
            }
            if (sourceNPC == -1 && projectile.friendly && projectile.damage > 0 && !projectile.npcProj && projectile.timeLeft > 60 && projectile.type != ModContent.ProjectileType<HyperCrystalProj>())
            {
                var aequus = Main.player[projectile.owner].Aequus();

                if (aequus.accHyperCrystal != null && aequus.hyperCrystalCooldown == 0 && projectile.Distance(Main.player[projectile.owner].Center) < HyperCrystalProj.MaxDistance)
                {
                    aequus.hyperCrystalCooldown = aequus.hyperCrystalCooldownMax;
                    int oldPIdentity = pIdentity;
                    pIdentity = projectile.identity;
                    Projectile.NewProjectile(Main.player[projectile.owner].GetSource_Accessory(aequus.accHyperCrystal), projectile.Center, projectile.velocity,
                        ModContent.ProjectileType<HyperCrystalProj>(), projectile.damage, projectile.knockBack, projectile.owner);
                    pIdentity = oldPIdentity;
                }
                if (Main.player[projectile.owner].HasBuff<DeathsEmbraceBuff>())
                {
                    float rotation = Main.rand.NextFloat(-0.025f, 0.025f);
                    for (int i = 0; i < 2; i++)
                    {
                        if (Main.rand.NextBool())
                        {
                            rotation += Main.rand.NextFloat(-0.05f, 0.05f);
                        }
                    }
                    if (Main.rand.NextBool(6))
                    {
                        rotation += Main.rand.NextFloat(-0.075f, 0.075f);
                    }
                    if (Main.rand.NextBool(12))
                    {
                        rotation += Main.rand.NextFloat(-0.075f, 0.075f);
                    }
                    projectile.velocity = projectile.velocity.RotatedBy(rotation);
                }
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if ((enemyRebound == 1 || enemyRebound == 2) && Main.rand.NextBool(1 + projectile.extraUpdates))
            {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? DustID.Torch : DustID.IceTorch, Scale: Main.rand.NextFloat(1f, 2.8f));
                d.velocity *= 0.5f;
                d.velocity += -projectile.velocity * 0.33f;
                d.noGravity = true;
            }

            if (transform > 0)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    int p = Projectile.NewProjectile(new EntitySource_Misc($"Aequus: Transform"), projectile.Center, projectile.velocity, transform,
                        projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    Main.projectile[p].miscText = projectile.miscText;
                }

                projectile.Kill();
                transform = 0;
                return false;
            }

            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                if (sourceProjIdentity > 0)
                {
                    sourceProj = AequusHelpers.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
                    if (sourceProj == -1 || Main.projectile[sourceProj].type != sourceProjType)
                    {
                        sourceProjIdentity = -1;
                        if (projectile.ModProjectile is ProjectileHooks.IOnUnmatchingProjectileParents unmatchingMethod)
                        {
                            unmatchingMethod.OnUnmatchingProjectileParents(this, sourceProj);
                        }
                    }
                }
            }

            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                if (Main.myPlayer == projectile.owner && projectile.numUpdates == -1 && projectile.velocity.Length() > 1f)
                {
                    int p = Projectile.NewProjectile(new EntitySource_Parent(projectile), projectile.Center, Vector2.Normalize(projectile.velocity) * 0.01f,
                        ModContent.ProjectileType<RaygunTrailProj>(), 0, 0f, projectile.owner);
                    Main.projectile[p].rotation = projectile.rotation;
                    Main.projectile[p].netUpdate = true;
                    Main.projectile[p].ModProjectile<RaygunTrailProj>().color = Raygun.GetColor(projectile).UseA(0);
                }
                if (projectile.type == ProjectileID.ChlorophyteBullet)
                {
                    projectile.alpha = 255;
                }
            }
            if (Main.netMode != NetmodeID.Server && sourceItemUsed == ModContent.ItemType<Hitscanner>()
                && Main.myPlayer == projectile.owner && projectile.oldVelocity != Vector2.Zero)
            {
                ScreenCulling.SetPadding(109);
                if (ScreenCulling.OnScreenWorld(projectile.position))
                {
                    var diff = projectile.oldVelocity;
                    int amt = (int)Math.Max(Math.Ceiling(diff.Length() / 4), 1);
                    var v = Vector2.Normalize(diff);
                    for (int i = 0; i < amt; i++)
                    {
                        var spawnLoc = projectile.Center + v * i * 4f;
                        if (Main.rand.NextBool(100))
                        {
                            Dust.NewDustPerfect(spawnLoc, DustID.Smoke, v, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                        }
                        ParticleSystem.New<MonoParticle>(ParticleLayer.BehindProjs).Setup(spawnLoc, v, Color.Lerp(Color.White, Color.Yellow, 0.1f).UseA(0) * Main.rand.NextFloat(0.3f, 1f), scale: Main.rand.NextFloat(0.9f, 1.44f), rotation: Main.rand.NextFloat(MathHelper.TwoPi));
                    }
                }
                if (projectile.type == ProjectileID.ChlorophyteBullet)
                {
                    projectile.alpha = 255;
                }
            }
            return true;
        }

        public override void PostAI(Projectile projectile)
        {
            timeAlive++;
            if (AequusHelpers.iterations == 0)
            {
                if (frenzyTime > 0)
                {
                    if (Main.rand.NextBool(2 * (projectile.extraUpdates + 1 + extraUpdatesTemporary)))
                    {
                        var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height,
                            DustID.RedTorch, projectile.velocity.X, projectile.velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                        d.noGravity = true;
                        d.fadeIn = d.scale + 0.2f;
                        d.noLightEmittence = true;
                    }
                    frenzyTime--;
                    if (frenzyTime == 0)
                        extraUpdatesTemporary--;
                }
            }
            if ((projectile.friendly || projectile.bobber) && projectile.owner >= 0 && projectile.owner != 255 && !projectile.npcProj && !GlowCore.ProjectileBlacklist.Contains(projectile.type))
            {
                var glowCore = Main.player[projectile.owner].Aequus();
                if (glowCore.accGlowCore > 0)
                {
                    GlowCore.AddLight(projectile.Center, Main.player[projectile.owner], Main.player[projectile.owner].Aequus());
                }
            }

            if (Main.myPlayer == projectile.owner)
            {
                if (projectile.bobber && Main.GameUpdateCount % 30 == 0 && Main.player[projectile.owner].Aequus().accNeonFish != null)
                {
                    int target = projectile.FindTargetWithLineOfSight(500f);
                    if (target != -1)
                    {
                        Projectile.NewProjectile(projectile.GetSource_Accessory(Main.player[projectile.owner].Aequus().accNeonFish), projectile.Center,
                            Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 25f, ModContent.ProjectileType<NeonFishLaser>(),
                            (int)(Main.player[projectile.owner].HeldItem.fishingPole * (Main.hardMode ? 1f : 1.5f) * Main.player[projectile.owner].Aequus().accNeonFish.Aequus().accStacks), 12f, projectile.owner);
                    }
                }
            }

            if (CanGetSpecialAccEffects(projectile))
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (aequus.accLittleInferno > 0)
                {
                    LittleInferno.InfernoPotionEffect(Main.player[projectile.owner], projectile.Center);
                }
            }

            if (projectile.type == ProjectileID.PortalGunBolt)
            {
                var checkTileCoords = projectile.Center.ToTileCoordinates();
                bool inWorld = WorldGen.InWorld(checkTileCoords.X, checkTileCoords.Y, 10);
                if (inWorld)
                {
                    if (Main.tile[checkTileCoords].HasTile && !Main.tile[checkTileCoords].IsActuated && Main.tile[checkTileCoords].TileType == ModContent.TileType<EmancipationGrillTile>())
                    {
                        projectile.Kill();
                        return;
                    }
                }
            }
        }

        public int ProjectileOwner(Projectile projectile)
        {
            return AequusHelpers.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (sourceItemUsed == ModContent.ItemType<StarPhish>() && (target.wet || target.honeyWet || target.lavaWet || target.HasBuff(BuffID.Wet)))
            {
                damage = (int)(damage * 1.25f);
            }
        }

        public void OnHit(Projectile projectile, Entity ent, int damage, float kb, bool crit)
        {
            var entity = new EntityCommons(ent);
            if (enemyRebound == 1 || enemyRebound == 2)
            {
                entity.AddBuff(BuffID.OnFire3, 600);
                entity.AddBuff(BuffID.Frostburn2, 600);
            }
            if (sourceItemUsed != 0 && projectile.friendly && projectile.HasOwner())
            {
                if (sourceItemUsed == Main.player[projectile.owner].HeldItemFixed().type)
                {
                    Main.player[projectile.owner].Aequus().itemHits++;
                }
            }
            if (projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (Main.player[projectile.owner].Aequus().accFrostburnTurretSquid > 0 && Main.rand.NextBool(Math.Max(3 / Main.player[projectile.owner].Aequus().accFrostburnTurretSquid, 1)))
                {
                    entity.AddBuff(BuffID.Frostburn2, 240 * Main.player[projectile.owner].Aequus().accFrostburnTurretSquid);
                }
            }
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                Raygun.SpawnExplosion(projectile.GetSource_OnHit(ent), projectile);
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(projectile, target, damage, knockback, crit);
            if (!target.SpawnedFromStatue && !target.immortal && target.Aequus().oldLife >= target.lifeMax && projectile.DamageType == DamageClass.Summon && Main.player[projectile.owner].Aequus().accWarHorn > 0)
            {
                int proj = (projectile.minion || projectile.sentry) ? projectile.whoAmI : AequusHelpers.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
                if (proj != -1)
                {
                    var aequus = Main.projectile[proj].Aequus();
                    if (aequus.frenzyTime == 0)
                    {
                        aequus.extraUpdatesTemporary++;
                    }
                    if (aequus.frenzyTime <= 30)
                    {
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            PacketSystem.SyncSound(SoundPacket.WarHorn, projectile.Center);
                        }
                        else
                        {
                            WarHorn.EmitSound(projectile.Center);
                        }
                    }
                    aequus.frenzyTime = (ushort)(240 * Main.player[projectile.owner].Aequus().accWarHorn);
                    for (int i = 0; i < 20; i++)
                    {
                        var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height,
                            DustID.RedTorch, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                        d.noGravity = true;
                        d.velocity *= 5f;
                        d.fadeIn = d.scale + 0.2f;
                        d.noLightEmittence = true;
                    }
                    Main.projectile[proj].netUpdate = true;
                }
            }
        }
        public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            OnHit(projectile, target, damage, 1f, crit);
        }
        public override void OnHitPvp(Projectile projectile, Player target, int damage, bool crit)
        {
            OnHit(projectile, target, damage, 1f, crit);
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                Raygun.SpawnExplosion(projectile.GetSource_Death(), projectile);
            }
            if (enemyRebound == 2)
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.DirectionTo(Main.player[projectile.owner].Center) * -0.1f,
                            ModContent.ProjectileType<StormcloakExplosionProj>(), 120, 0f, projectile.owner, ai0: Main.rand.Next(2));
            }
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (frenzyTime > 0)
            {
                float frenzyOpacity = frenzyTime < 60 ? frenzyTime / 60f : 1f;

                var texture = ModContent.Request<Texture2D>("Aequus/Projectiles/Melee/Swords/Swish2", AssetRequestMode.ImmediateLoad).Value;

                var color = Color.Red;
                var drawCoords = projectile.Center - Main.screenPosition;
                var textureOrigin = texture.Size() / 2f;
                float scale = projectile.scale * 0.5f;
                int swishTimeMax = 20;
                int swishTime = frenzyTime % swishTimeMax;
                float swishOpacity = frenzyOpacity;
                if (swishTime < 8)
                {
                    swishOpacity *= swishTime / 8f;
                }
                else if (swishTime > swishTimeMax - 8)
                {
                    swishOpacity *= 1f - (swishTimeMax - swishTime) / 8f;
                }
                Main.EntitySpriteDraw(Textures.Bloom[0].Value, drawCoords, null, color, 0f, Textures.Bloom[0].Value.Size() / 2f, scale, SpriteEffects.None, 0);
                for (int i = -1; i <= 1; i += 2)
                {
                    Main.EntitySpriteDraw(texture, drawCoords + new Vector2(i * projectile.Frame().Width * (1f - frenzyTime % swishTimeMax / (float)swishTimeMax), 0f), null, color.UseA(128) * swishOpacity, MathHelper.PiOver2 * i, textureOrigin, scale * 0.5f, SpriteEffects.None, 0);
                }
            }
            if (CanGetSpecialAccEffects(projectile))
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (aequus.accLittleInferno > 0)
                {
                    float opacity = Math.Clamp((Main.myPlayer == projectile.owner ? 0.4f : 0.15f) / Main.player[projectile.owner].ownedProjectileCounts[projectile.type], 0.05f, 1f);
                    if (timeAlive < 30)
                    {
                        opacity *= timeAlive / 30f;
                    }
                    LittleInferno.DrawInfernoRings(projectile.Center - Main.screenPosition, opacity);
                }
            }

            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                if (!Raygun.BulletColor.ContainsKey(projectile.type))
                {
                    var clr = Raygun.CheckRayColor(projectile);
                    Raygun.BulletColor[projectile.type] = (p) => p.GetAlpha(clr);
                }
                return projectile.velocity.Length() < 1f;
            }
            return true;
        }

        public bool CanGetSpecialAccEffects(Projectile projectile)
        {
            return projectile.friendly && projectile.damage > 0 && projectile.owner >= 0 && projectile.owner != 255 && !projectile.npcProj && Main.player[projectile.owner].heldProj != projectile.whoAmI && !BlacklistSpecialEffects.Contains(projectile.type);
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(timeAlive);
            binaryWriter.Write(enemyRebound);
            bitWriter.WriteBit(transform > 0);
            if (transform > 0)
            {
                binaryWriter.Write(transform);
            }
            bitWriter.WriteBit(frenzyTime > 0);
            if (frenzyTime > 0)
            {
                binaryWriter.Write(frenzyTime);
            }
            bitWriter.WriteBit(extraUpdatesTemporary > 0);
            if (extraUpdatesTemporary > 0)
            {
                binaryWriter.Write((ushort)extraUpdatesTemporary);
            }
            bitWriter.WriteBit(sourceItemUsed > 0);
            if (sourceItemUsed > 0)
            {
                binaryWriter.Write((ushort)sourceItemUsed);
            }
            bitWriter.WriteBit(sourceAmmoUsed > 0);
            if (sourceAmmoUsed > 0)
            {
                binaryWriter.Write((ushort)sourceAmmoUsed);
            }
            bitWriter.WriteBit(sourceNPC > -1);
            if (sourceNPC > -1)
            {
                binaryWriter.Write((ushort)sourceNPC);
            }
            bitWriter.WriteBit(sourceProjIdentity > -1);
            if (sourceProjIdentity > -1)
            {
                binaryWriter.Write((ushort)sourceProjIdentity);
            }
            bitWriter.WriteBit(sourceProj > -1);
            if (sourceProj > -1)
            {
                binaryWriter.Write((ushort)sourceProj);
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            timeAlive = binaryReader.ReadInt32();
            enemyRebound = binaryReader.ReadByte();
            if (bitReader.ReadBit())
            {
                transform = binaryReader.ReadInt32();
            }
            if (bitReader.ReadBit())
            {
                frenzyTime = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                extraUpdatesTemporary = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                sourceItemUsed = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                sourceAmmoUsed = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                sourceNPC = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                sourceProjIdentity = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                sourceProj = binaryReader.ReadUInt16();
            }
        }

        public static void Scale(Projectile projectile, int amt)
        {
            projectile.position.X -= amt / 2f;
            projectile.position.Y -= amt / 2f;
            projectile.width += amt;
            projectile.height += amt;
            projectile.netUpdate = true;
        }
    }
}