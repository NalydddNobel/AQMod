﻿using Aequus.Common.Items.Components;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Projectiles;

public class ProjectileSource : GlobalProjectile {
    public short parentNPCIndex;
    public int parentItemType;
    public int parentAmmoType;
    /// <summary>
    /// Whether this projectile was spawned by another projectile. Use this to prevent effects occuring multiple times (Like ammo retrival)
    /// </summary>
    public bool isProjectileChild;

    public bool HasNPCOwner => parentNPCIndex != -1;

    [CloneByReference]
    public IManageProjectile ProjectileManager { get; private set; }

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override void SetDefaults(Projectile entity) {
        parentItemType = 0;
        parentNPCIndex = -1;
        ProjectileManager = default;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        if (source is EntitySource_Parent parentSource) {
            if (parentSource.Entity is Projectile parentProjectile && parentProjectile.TryGetGlobalProjectile(out ProjectileSource parentSources)) {
                parentItemType = parentSources.parentItemType;
                parentNPCIndex = parentSources.parentNPCIndex;
                parentAmmoType = parentSources.parentAmmoType;
                isProjectileChild = true;
            }
            else if (parentSource.Entity is Item parentItem) {
                parentItemType = parentItem.type;
            }
            else if (parentSource.Entity is NPC parentNPC) {
                parentNPCIndex = (short)parentNPC.whoAmI;
            }
        }
        if (source is EntitySource_ItemUse_WithAmmo withAmmo) {
            parentAmmoType = withAmmo.AmmoItemIdUsed;
        }
        if (source is IEntitySource_WithStatsFromItem withItem) {
            if (withItem.Item != null) {
                parentItemType = withItem.Item.type;
            }
        }
    }

    public override bool PreAI(Projectile projectile) {
        if (parentNPCIndex > -1 && !Main.npc[parentNPCIndex].active) {
            parentNPCIndex = -1;
        }
        if (parentItemType > ItemID.Count && ItemLoader.GetItem(parentItemType) is IManageProjectile manageProjectile) {
            ProjectileManager = manageProjectile;
        }
        return ProjectileManager?.PreAIProjectile(projectile) ?? true;
    }

    public override void AI(Projectile projectile) {
        ProjectileManager?.AIProjectile(projectile);
    }

    public override void PostAI(Projectile projectile) {
        ProjectileManager?.PostAIProjectile(projectile);
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        return ProjectileManager?.PreDrawProjectile(projectile, ref lightColor) ?? true;
    }

    public override void PostDraw(Projectile projectile, Color lightColor) {
        ProjectileManager?.PostDrawProjectile(projectile, lightColor);
    }

    public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity) {
        return ProjectileManager?.OnTileCollideProjectile(projectile, oldVelocity) ?? true;
    }

    public override bool PreKill(Projectile projectile, int timeLeft) {
        return ProjectileManager?.PreKillProjectile(projectile, timeLeft) ?? true;
    }

    public override void OnKill(Projectile projectile, int timeLeft) {
        ProjectileManager?.OnKillProjectile(projectile, timeLeft);
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
        ProjectileManager?.OnHitNPCProjectile(projectile, target, hit, damageDone);
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
        bitWriter.WriteBit(isProjectileChild);

        bitWriter.WriteBit(parentNPCIndex > -1);
        if (parentNPCIndex > -1) {
            binaryWriter.Write(parentNPCIndex);
        }

        bitWriter.WriteBit(parentItemType > 0);
        if (parentItemType > 0) {
            binaryWriter.Write(parentItemType);
        }
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
        isProjectileChild = bitReader.ReadBit();

        if (bitReader.ReadBit()) {
            parentNPCIndex = binaryReader.ReadInt16();
        }

        if (bitReader.ReadBit()) {
            parentItemType = binaryReader.ReadInt32();
        }
    }
}