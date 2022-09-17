using Aequus.Content;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Ranged;
using Aequus.Projectiles.Misc;
using Aequus.Projectiles.Ranged;
using Aequus.Tiles;
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
    public class AequusProjectile : GlobalProjectile
    {
        public static HashSet<int> HeatDamage { get; private set; }
        public static HashSet<int> BlacklistSpecialEffects { get; private set; }

        public static int pWhoAmI;
        public static int pIdentity;
        public static int pNPC;

        public bool fishDamage;
        public bool heatDamage;
        public ushort frenzyTime;

        public int extraUpdatesTemporary;

        public int transform;
        public int timeAlive;

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
            HeatDamage = new HashSet<int>()
            {
                ProjectileID.CultistBossFireBall,
                ProjectileID.CultistBossFireBallClone,
                ProjectileID.EyeFire,
                ProjectileID.GreekFire1,
                ProjectileID.GreekFire2,
                ProjectileID.GreekFire3,
            };
            BlacklistSpecialEffects = new HashSet<int>()
            {
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
        public void OnSpawn_Inherit(Projectile projectile, IEntitySource source)
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
                sourceItemUsed = itemUse_WithAmmo.Item.netID;
                sourceAmmoUsed = itemUse_WithAmmo.AmmoItemIdUsed;
            }
            else if (source is EntitySource_ItemUse itemUse)
            {
                sourceItemUsed = itemUse.Item.netID;
            }
            else if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC)
                {
                    sourceNPC = parent.Entity.whoAmI;
                }
                else if (parent.Entity is Projectile parentProjectile)
                {
                    sourceProjIdentity = parentProjectile.identity;
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
        }

        public void OnSpawn_CheckItem(Projectile projectile, IEntitySource source)
        {
            if (sourceItemUsed > Main.maxItemTypes)
            {
                var item = ContentSamples.ItemsByType[sourceItemUsed];
                if (item.ModItem is ItemHooks.IOnSpawnProjectile onSpawnHook)
                {
                    onSpawnHook.OnSpawnProjectile(projectile, this, source);
                }
            }
        }

        public void OnSpawn_CheckBonesaw(Projectile projectile, EntitySource_ItemUse itemUse)
        {
            if (projectile.type == ProjectileID.BoneGloveProj && itemUse.Entity is Player player && player.GetModPlayer<AequusPlayer>().ExpertBoost)
            {
                transform = ModContent.ProjectileType<Bonesaw>();
                projectile.velocity *= 1.25f;
                projectile.damage = (int)(projectile.damage * 1.5f);
            }
        }

        public void OnSpawn_CheckTombstones(Projectile projectile, EntitySource_Misc misc)
        {
            if (misc.Context == "PlayerDeath_TombStone")
            {
                var player = Main.player[projectile.owner];
                if (player.position.Y > Main.UnderworldLayer * 16f)
                {
                    transform = Main.rand.Next(CustomTombstones.HellTombstones);
                }
                player.respawnTimer = Math.Min(35, player.respawnTimer);
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            sourceItemUsed = -1;
            sourceAmmoUsed = -1;
            sourceNPC = pNPC;
            sourceProjIdentity = pIdentity;

            if (Main.gameMenu)
                return;

            try
            {
                OnSpawn_Inherit(projectile, source);

                if (source is EntitySource_ItemUse itemUse)
                {
                    OnSpawn_CheckBonesaw(projectile, itemUse);
                }
                else if (source is EntitySource_Misc misc)
                {
                    OnSpawn_CheckTombstones(projectile, misc);
                }
                OnSpawn_CheckItem(projectile, source);
            }
            catch
            {
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (transform > 0)
            {
                int p = Projectile.NewProjectile(new EntitySource_Misc($"Aequus: Transform"), projectile.Center, projectile.velocity, transform,
                    projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                Main.projectile[p].miscText = projectile.miscText;
                projectile.Kill();
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
                    int p = Projectile.NewProjectile(new EntitySource_Parent(projectile), projectile.Center, Vector2.Normalize(projectile.velocity) * 0.01f, ModContent.ProjectileType<RaygunTrailProj>(), 0, 0f, projectile.owner);
                    Main.projectile[p].rotation = projectile.velocity.ToRotation();
                    Main.projectile[p].netUpdate = true;
                    Main.projectile[p].ModProjectile<RaygunTrailProj>().color = Raygun.GetColor(projectile).UseA(0);
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
                    frenzyTime--;
                    if (frenzyTime == 0)
                        extraUpdatesTemporary--;
                }
            }
            if ((projectile.friendly || projectile.bobber) && projectile.owner >= 0 && projectile.owner != 255 && !projectile.npcProj && !GlowCore.ProjectileBlacklist.Contains(projectile.type))
            {
                var glowCore = Main.player[projectile.owner].Aequus();
                if (glowCore.glowCore != -1)
                {
                    GlowCore.AddLight(projectile.Center, Main.player[projectile.owner], Main.player[projectile.owner].Aequus());
                }
            }

            if (CanGetSpecialAccEffects(projectile))
            {
                var aequus = Main.player[projectile.owner].Aequus();
                int chance = 1;
                if (projectile.minion)
                {
                    chance += Main.player[projectile.owner].ownedProjectileCounts[projectile.type] / 2;
                }
                if (aequus.accDustDevilFire && Main.rand.NextBool(chance))
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
            if (fishDamage && (target.wet || target.HasBuff(BuffID.Wet)))
            {
                damage = (int)(damage * 1.25f);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (!target.SpawnedFromStatue && !target.immortal && target.Aequus().oldLife >= target.lifeMax && projectile.DamageType == DamageClass.Summon && Main.player[projectile.owner].Aequus().accWarHorn)
            {
                int proj = (projectile.minion || projectile.sentry) ? projectile.whoAmI : AequusHelpers.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
                if (proj != -1)
                {
                    var aequus = Main.projectile[proj].Aequus();
                    if (aequus.frenzyTime == 0)
                    {
                        aequus.extraUpdatesTemporary++;
                    }
                    aequus.frenzyTime = 240;
                    Main.projectile[proj].netUpdate = true;
                }
            }
            if (sourceItemUsed != 0 && projectile.friendly && projectile.HasOwner())
            {
                var i = Main.player[projectile.owner].HeldItem;
                if (sourceItemUsed == i.type)
                {
                    Main.player[projectile.owner].Aequus().itemHits++;
                }
            }
            if (projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (Main.player[projectile.owner].Aequus().accFrostburnTurretSquid && Main.rand.NextBool(6))
                {
                    target.AddBuff(BuffID.Frostburn2, 240);
                }
            }
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                Raygun.SpawnExplosion(projectile.GetSource_OnHit(target), projectile);
            }
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                Raygun.SpawnExplosion(projectile.GetSource_Death(), projectile);
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
                Main.EntitySpriteDraw(TextureCache.Bloom[0].Value, drawCoords, null, color, 0f, TextureCache.Bloom[0].Value.Size() / 2f, scale, SpriteEffects.None, 0);
                for (int i = -1; i <= 1; i += 2)
                {
                    Main.EntitySpriteDraw(texture, drawCoords + new Vector2(i * projectile.Frame().Width * (1f - frenzyTime % swishTimeMax / (float)swishTimeMax), 0f), null, color.UseA(128) * swishOpacity, MathHelper.PiOver2 * i, textureOrigin, scale * 0.5f, SpriteEffects.None, 0);
                }
            }
            if (CanGetSpecialAccEffects(projectile))
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (aequus.accDustDevilFire)
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
            bitWriter.WriteBit(fishDamage);
            binaryWriter.Write(timeAlive);
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
            fishDamage = bitReader.ReadBit();
            timeAlive = binaryReader.ReadInt32();
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