﻿using Aequus.Common;
using Aequus.Common.DataSets;
using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.Items.SentryChip;
using Aequus.Common.Projectiles;
using Aequus.Content.ItemPrefixes;
using Aequus.Items.Equipment.Accessories.Misc;
using Aequus.Items.Potions.DeathsEmbrace;
using Aequus.Items.Weapons.Ranged.Misc.StarPhish;
using Aequus.NPCs;
using Aequus.Projectiles.Misc.CrownOfBlood;
using Aequus.Projectiles.Misc.Friendly;
using Aequus.Tiles.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Projectiles;
public partial class AequusProjectile : GlobalProjectile {
    public static int pWhoAmI;
    public static int pIdentity;
    public static int pNPC;

    public bool heatDamage;
    public ushort warHornFrenzy;

    public int extraUpdatesTemporary;

    public int transform;
    public int timeAlive;
    public byte specialState;

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
    public bool aiInit;

    public override bool InstancePerEntity => true;

    public bool FromItem => sourceItemUsed > 0;
    public bool FromAmmo => sourceAmmoUsed > 0;
    public bool HasProjectileOwner => sourceProjIdentity > -1;
    public bool HasNPCOwner => sourceNPC > -1;
    public bool MissingProjectileOwner => sourceProjIdentity == -1 && sourceProj != -1;

    public AequusProjectile() {
        sourceNPC = -1;
        sourceProjIdentity = -1;
        sourceProj = -1;
        sourceAmmoUsed = 0;
        sourceItemUsed = 0;
        sourceProjType = 0;
        transform = 0;
        timeAlive = 0;
        aiInit = false;
        _checkTombstone = false;
    }

    public override void Load() {
        Load_Tombstones();
        pIdentity = -1;
        pWhoAmI = -1;
        pNPC = -1;
        On_Projectile.Update += Projectile_Update;
    }

    private static void Projectile_Update(On_Projectile.orig_Update orig, Projectile self, int i) {
        pIdentity = self.identity;
        pWhoAmI = i;

        Helper.iterations = 0;
        orig(self, i);

        if (!self.active)
            return;
        self.TryGetGlobalProjectile<AequusProjectile>(out var aequus);
        if (aequus != null) {
            if (aequus.extraUpdatesTemporary > 0) {
                float minionSlotsOld = Main.player[self.owner].slotsMinions;
                float minionSlots = Main.player[self.owner].slotsMinions - self.minionSlots;
                for (int k = 0; k < aequus.extraUpdatesTemporary; k++) {
                    Helper.iterations = k + 1;
                    Main.player[self.owner].slotsMinions = minionSlots;
                    orig(self, i);
                }
                Main.player[self.owner].slotsMinions = minionSlotsOld;
            }
        }
        pIdentity = self.identity;
        pWhoAmI = -1;
    }

    public override void Unload() {
        Unload_DataSets();
        Unload_Tombstones();
        pIdentity = -1;
        pWhoAmI = -1;
        pNPC = -1;
    }

    public override void SetDefaults(Projectile projectile) {
        //SetDefaults_Zombie();
        if (ProjectileSets.DealsHeatDamage.Contains(projectile.type)) {
            heatDamage = true;
        }
        warHornFrenzy = 0;
        extraUpdatesTemporary = 0;
        timeAlive = 0;
        specialState = 0;
        aiInit = false;
        _checkTombstone = false;
    }

    public void InheritPreviousSourceData(Projectile projectile, Projectile parent) {
        if (projectile.owner == parent.owner && parent.TryGetGlobalProjectile<AequusProjectile>(out var parentAequusProjectile)) {
            if (projectile.friendly && projectile.timeLeft > 4) {
                if (parentAequusProjectile.sourceItemUsed != 0) {
                    sourceItemUsed = parentAequusProjectile.sourceItemUsed;
                }
                if (parentAequusProjectile.sourceAmmoUsed != 0) {
                    sourceAmmoUsed = parentAequusProjectile.sourceAmmoUsed;
                }
            }
            //zombieInfo.Inherit(parentAequusProjectile.zombieInfo);
        }
    }

    private void ApplyInheritEffects(Projectile projectile, IEntitySource source) {
        if (sourceItemUsed != -1 && ItemLoader.GetItem(sourceItemUsed) is ItemHooks.IOnSpawnProjectile onSpawnHook2) {
            onSpawnHook2.OnSpawnProjectile(projectile, this, source);
        }
        if (HasNPCOwner) {
            var parentNPC = Main.npc[sourceNPC];
            if (parentNPC.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
                friendship = aequusNPC.friendship;
                //zombieInfo.Inherit(aequusNPC.zombieInfo);
            }
        }
        else if (projectile.friendly && projectile.owner == Main.myPlayer) {
            var player = Main.player[projectile.owner];
            var aequusPlayer = player.Aequus();
            OnSpawn_CheckSpread(player, aequusPlayer, projectile);
        }
    }

    public void TryInherit(Projectile projectile, IEntitySource source) {
        if (!projectile.hostile && projectile.HasOwner()) {
            int projOwner = Main.player[projectile.owner].Aequus().projectileIdentity;
            if (projOwner != -1) {
                sourceProjIdentity = projOwner;
            }
        }

        if (source is EntitySource_Parent sourceParent) {
            if (sourceParent.Entity is NPC) {
                sourceNPC = sourceParent.Entity.whoAmI;
            }
            else if (sourceParent.Entity is Projectile parentProjectile) {
                sourceProjIdentity = parentProjectile.identity;
                SentryAccessoriesDatabase.ApplyOnShootInteractions(sourceParent, projectile, this, parentProjectile);
            }
        }

        if (source is IEntitySource_WithStatsFromItem withStatsFromItem) {
            if (withStatsFromItem.Item != null) {
                sourceItemUsed = withStatsFromItem.Item.netID;
                if (withStatsFromItem.Item.ModItem is ItemHooks.IOnSpawnProjectile onSpawnHook) {
                    onSpawnHook.OnShootProjectile(projectile, this, source);
                }
                if (withStatsFromItem.Item.TryGetGlobalItem<EquipBoostGlobalItem>(out var equipBoostGlobalItem) && equipBoostGlobalItem.equipEmpowerment?.HasAbilityBoost == true && EquipBoostDatabase.Instance.OnSpawnProjectile.TryGetValue(withStatsFromItem.Item.type, out var value)) {
                    value(source, withStatsFromItem.Item, projectile);
                }
            }
        }

        if (source is EntitySource_ItemUse itemUse && itemUse.Item != null) {
            if (PrefixLoader.GetPrefix(itemUse.Item.prefix) is AequusPrefix aequusPrefix) {
                aequusPrefix.OnShoot(itemUse.Player, itemUse.Item, projectile);
            }
        }
        if (source is EntitySource_ItemUse_WithAmmo itemUse_WithAmmo) {
            sourceAmmoUsed = itemUse_WithAmmo.AmmoItemIdUsed;
        }

        if (sourceProjIdentity != -1) {
            sourceProj = Helper.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
            if (sourceProj == -1) {
                sourceProjIdentity = -1;
            }
            else {
                sourceProjType = Main.projectile[sourceProj].type;
                InheritPreviousSourceData(projectile, Main.projectile[sourceProj]);
            }
        }
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        sourceItemUsed = -1;
        sourceAmmoUsed = -1;
        sourceNPC = pNPC;
        sourceProjIdentity = pIdentity;
        sourceProj = -1;

        if (Main.gameMenu) {
            return;
        }

        try {
            TryInherit(projectile, source);
            ApplyInheritEffects(projectile, source);
        }
        catch {
        }
        OnSpawn_Tombstones(projectile, source);
        if (sourceNPC == -1 && projectile.friendly && projectile.damage > 0 && !projectile.npcProj && projectile.timeLeft > 60 && projectile.type != ModContent.ProjectileType<HyperCrystalProj>()) {
            var aequus = Main.player[projectile.owner].Aequus();

            if (aequus.accHyperCrystal != null && aequus.hyperCrystalCooldown == 0 && projectile.Distance(Main.player[projectile.owner].Center) < HyperCrystalProj.MaxDistance) {
                aequus.hyperCrystalCooldown = aequus.hyperCrystalCooldownMax;
                int oldPIdentity = pIdentity;
                pIdentity = projectile.identity;
                Projectile.NewProjectile(Main.player[projectile.owner].GetSource_Accessory(aequus.accHyperCrystal), projectile.Center, projectile.velocity,
                    ModContent.ProjectileType<HyperCrystalProj>(), projectile.damage, projectile.knockBack, projectile.owner);
                pIdentity = oldPIdentity;
            }
            if (Main.player[projectile.owner].HasBuff<DeathsEmbraceBuff>()) {
                float rotation = Main.rand.NextFloat(-0.025f, 0.025f);
                for (int i = 0; i < 2; i++) {
                    if (Main.rand.NextBool()) {
                        rotation += Main.rand.NextFloat(-0.05f, 0.05f);
                    }
                }
                if (Main.rand.NextBool(6)) {
                    rotation += Main.rand.NextFloat(-0.075f, 0.075f);
                }
                if (Main.rand.NextBool(12)) {
                    rotation += Main.rand.NextFloat(-0.075f, 0.075f);
                }
                projectile.velocity = projectile.velocity.RotatedBy(rotation);
            }
        }
    }

    public override bool PreAI(Projectile projectile) {
        //PreAI_CheckZombie(projectile);
        if (!aiInit) {
            if (sourceItemUsed != -1 && ItemLoader.GetItem(sourceItemUsed) is ItemHooks.IOnSpawnProjectile onSpawnHook2) {
                onSpawnHook2.InitalizeProjectile(projectile, this);
            }
            InitAI_Tombstones(projectile);
            aiInit = true;
        }

        //if ((specialState == 1 || specialState == 2) && Main.rand.NextBool(1 + projectile.extraUpdates))
        //{
        //    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? DustID.Torch : DustID.IceTorch, Scale: Main.rand.NextFloat(1f, 2.8f));
        //    d.velocity *= 0.5f;
        //    d.velocity += -projectile.velocity * 0.33f;
        //    d.noGravity = true;
        //}

        if (transform > 0) {
            if (Main.myPlayer == projectile.owner) {
                IEntitySource source;
                if (sourceProjIdentity > -1 && Helper.TryFindProjectileIdentity(projectile.owner, sourceProjIdentity, out int sourceProj)) {
                    source = Main.projectile[sourceProj].GetSource_FromThis();
                }
                else {
                    source = projectile.GetSource_FromThis();
                }

                var transformProjectile = Projectile.NewProjectileDirect(source, projectile.Center, projectile.velocity, transform,
                    projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                if (transformProjectile == null || !transformProjectile.active) {
                    return false;
                }

                TryInherit(transformProjectile, projectile.GetSource_FromThis());
                transformProjectile.timeLeft = projectile.timeLeft;
                transformProjectile.miscText = projectile.miscText;
            }

            projectile.active = false;
            transform = 0;
            return false;
        }

        if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255) {
            if (sourceProjIdentity > 0) {
                sourceProj = Helper.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
                if (sourceProj == -1 || Main.projectile[sourceProj].type != sourceProjType) {
                    sourceProjIdentity = -1;
                    if (projectile.ModProjectile is ProjectileHooks.IOnUnmatchingProjectileParents unmatchingMethod) {
                        unmatchingMethod.OnUnmatchingProjectileParents(this, sourceProj);
                    }
                }
            }
        }

        AI_Raygun(projectile);
        if (Main.netMode != NetmodeID.Server) {
            ScreenCulling.Prepare(109);
            bool onScreen = ScreenCulling.OnScreenWorld(projectile.position);
            Effects_Hitscanner(projectile, onScreen);
        }
        return true;
    }

    public override void PostAI(Projectile projectile) {
        PostAI_Stormcloak(projectile);
        timeAlive++;
        if (Helper.iterations == 0) {
            if (warHornFrenzy > 0) {
                if (Main.rand.NextBool(2 * (projectile.extraUpdates + 1 + extraUpdatesTemporary))) {
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height,
                        DustID.RedTorch, projectile.velocity.X, projectile.velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                    d.noLightEmittence = true;
                }
                warHornFrenzy--;
                if (warHornFrenzy == 0)
                    extraUpdatesTemporary--;
            }
        }
        if ((projectile.friendly || projectile.bobber) && projectile.owner >= 0 && projectile.owner != 255 && !projectile.npcProj && !GlowCore.ProjectileBlacklist.Contains(projectile.type)) {
            var glowCore = Main.player[projectile.owner].Aequus();
            if (glowCore.accGlowCore > 0) {
                GlowCore.AddLight(projectile.Center, Main.player[projectile.owner], Main.player[projectile.owner].Aequus());
            }
        }

        if (Main.myPlayer == projectile.owner) {
            var player = Main.player[projectile.owner];
            var aequus = player.Aequus();
            if (projectile.bobber) {
                aequus.UseNeonGenesis(projectile);
            }
        }

        if (CanGetSpecialAccEffects(projectile)) {
            var aequus = Main.player[projectile.owner].Aequus();
        }

        PostAI_UpdateFriendship(projectile);
        //PostAI_CheckZombie(projectile);

        if (projectile.type == ProjectileID.PortalGunBolt) {
            var checkTileCoords = projectile.Center.ToTileCoordinates();
            bool inWorld = WorldGen.InWorld(checkTileCoords.X, checkTileCoords.Y, 10);
            if (inWorld) {
                if (Main.tile[checkTileCoords].HasTile && !Main.tile[checkTileCoords].IsActuated && Main.tile[checkTileCoords].TileType == ModContent.TileType<EmancipationGrillTile>()) {
                    projectile.Kill();
                    return;
                }
            }
        }
    }

    public int ProjectileOwner(Projectile projectile) {
        return Helper.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
        if (sourceItemUsed == ModContent.ItemType<StarPhish>() && (target.wet || target.honeyWet || target.lavaWet || target.HasBuff(BuffID.Wet))) {
            modifiers.FinalDamage *= 1.25f;
        }
    }

    public void OnHit(Projectile projectile, Entity ent, int damage, float kb, bool crit) {
        var entity = new EntityCommons(ent);
        if (specialState == 1 || specialState == 2) {
            entity.AddBuff(BuffID.OnFire3, 600);
            entity.AddBuff(BuffID.Frostburn2, 600);
        }
        if (sourceItemUsed != 0 && projectile.friendly && projectile.HasOwner()) {
            if (sourceItemUsed == Main.player[projectile.owner].HeldItemFixed().type) {
                Main.player[projectile.owner].Aequus().itemHits++;
            }
        }
        if (projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]) {
            if (Main.player[projectile.owner].Aequus().accFrostburnTurretSquid > 0 && Main.rand.NextBool(Math.Max(3 / Main.player[projectile.owner].Aequus().accFrostburnTurretSquid, 1))) {
                entity.AddBuff(BuffID.Frostburn2, 240 * Main.player[projectile.owner].Aequus().accFrostburnTurretSquid);
            }
        }
        OnHit_Raygun(projectile, ent);
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
        OnHit(projectile, target, hit.Damage, hit.Knockback, hit.Crit);
        if (!target.SpawnedFromStatue && !target.immortal && projectile.DamageType == DamageClass.Summon) {
            OnHit_Warhorn(projectile);
        }
    }

    public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info) {
        OnHit(projectile, target, info.Damage, info.Knockback, false);
    }

    public override void OnKill(Projectile projectile, int timeLeft) {
        Kill_Raygun(projectile);
        if (specialState == 2) {
            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.DirectionTo(Main.player[projectile.owner].Center) * -0.1f,
                        ModContent.ProjectileType<StormcloakExplosionProj>(), 120, 0f, projectile.owner, ai0: Main.rand.Next(2));
        }
    }

    public override void DrawBehind(Projectile projectile, int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        if (DrawBehind_CheckStormcloak(projectile, index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI)) {
            return;
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        if (warHornFrenzy > 0) {
            float frenzyOpacity = warHornFrenzy < 60 ? warHornFrenzy / 60f : 1f;

            var texture = AequusTextures.Swish2.Value;

            var color = Color.Red;
            var drawCoords = projectile.Center - Main.screenPosition;
            var textureOrigin = texture.Size() / 2f;
            float scale = projectile.scale * 0.5f;
            int swishTimeMax = 20;
            int swishTime = warHornFrenzy % swishTimeMax;
            float swishOpacity = frenzyOpacity;
            if (swishTime < 8) {
                swishOpacity *= swishTime / 8f;
            }
            else if (swishTime > swishTimeMax - 8) {
                swishOpacity *= 1f - (swishTimeMax - swishTime) / 8f;
            }
            Main.EntitySpriteDraw(AequusTextures.Bloom0, drawCoords, null, color, 0f, AequusTextures.Bloom0.Size() / 2f, scale, SpriteEffects.None, 0);
            for (int i = -1; i <= 1; i += 2) {
                Main.EntitySpriteDraw(texture, drawCoords + new Vector2(i * projectile.Frame().Width * (1f - warHornFrenzy % swishTimeMax / (float)swishTimeMax), 0f), null, color.UseA(128) * swishOpacity, MathHelper.PiOver2 * i, textureOrigin, scale * 0.5f, SpriteEffects.None, 0);
            }
        }
        if (CanGetSpecialAccEffects(projectile)) {
            var aequus = Main.player[projectile.owner].Aequus();
            if (aequus.accLittleInferno > 0) {
                float opacity = Math.Clamp((Main.myPlayer == projectile.owner ? 0.4f : 0.15f) / Main.player[projectile.owner].ownedProjectileCounts[projectile.type], 0.05f, 1f);
                if (timeAlive < 30) {
                    opacity *= timeAlive / 30f;
                }
            }
        }

        return PreDraw_Raygun(projectile);
    }

    public bool CanGetSpecialAccEffects(Projectile projectile) {
        return projectile.friendly && projectile.damage > 0 && projectile.owner >= 0 && projectile.owner != 255 && !projectile.npcProj && Main.player[projectile.owner].heldProj != projectile.whoAmI && !BlacklistSpecialEffects.Contains(projectile.type);
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
        binaryWriter.Write(timeAlive);
        binaryWriter.Write(specialState);
        bitWriter.WriteBit(friendship);

        bitWriter.WriteBit(transform > 0);
        if (transform > 0) {
            binaryWriter.Write(transform);
        }
        bitWriter.WriteBit(warHornFrenzy > 0);
        if (warHornFrenzy > 0) {
            binaryWriter.Write(warHornFrenzy);
        }
        bitWriter.WriteBit(extraUpdatesTemporary > 0);
        if (extraUpdatesTemporary > 0) {
            binaryWriter.Write((ushort)extraUpdatesTemporary);
        }
        bitWriter.WriteBit(sourceItemUsed > 0);
        if (sourceItemUsed > 0) {
            binaryWriter.Write((ushort)sourceItemUsed);
        }
        bitWriter.WriteBit(sourceAmmoUsed > 0);
        if (sourceAmmoUsed > 0) {
            binaryWriter.Write((ushort)sourceAmmoUsed);
        }
        bitWriter.WriteBit(sourceNPC > -1);
        if (sourceNPC > -1) {
            binaryWriter.Write((ushort)sourceNPC);
        }
        bitWriter.WriteBit(sourceProjIdentity > -1);
        if (sourceProjIdentity > -1) {
            binaryWriter.Write((ushort)sourceProjIdentity);
        }
        bitWriter.WriteBit(sourceProj > -1);
        if (sourceProj > -1) {
            binaryWriter.Write((ushort)sourceProj);
        }
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
        timeAlive = binaryReader.ReadInt32();
        specialState = binaryReader.ReadByte();
        friendship = bitReader.ReadBit();
        if (bitReader.ReadBit()) {
            transform = binaryReader.ReadInt32();
        }
        if (bitReader.ReadBit()) {
            warHornFrenzy = binaryReader.ReadUInt16();
        }
        if (bitReader.ReadBit()) {
            extraUpdatesTemporary = binaryReader.ReadUInt16();
        }
        if (bitReader.ReadBit()) {
            sourceItemUsed = binaryReader.ReadUInt16();
        }
        if (bitReader.ReadBit()) {
            sourceAmmoUsed = binaryReader.ReadUInt16();
        }
        if (bitReader.ReadBit()) {
            sourceNPC = binaryReader.ReadUInt16();
        }
        if (bitReader.ReadBit()) {
            sourceProjIdentity = binaryReader.ReadUInt16();
        }
        if (bitReader.ReadBit()) {
            sourceProj = binaryReader.ReadUInt16();
        }
    }

    public static void Scale(Projectile projectile, int amt) {
        projectile.position.X -= amt / 2f;
        projectile.position.Y -= amt / 2f;
        projectile.width += amt;
        projectile.height += amt;
        projectile.netUpdate = true;
    }
}