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

        public static int pWhoAmI;
        public static int pIdentity;
        public static int pNPC;

        public bool heatDamage;

        public int defExtraUpdates;

        public int sourceItemUsed;
        public int sourceAmmoUsed;
        public int sourceNPC;
        public int sourceProjType;
        public int sourceProjIdentity;
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
            pIdentity = -1;
            pWhoAmI = -1;
            pNPC = -1;
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
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            defExtraUpdates = projectile.extraUpdates;
            sourceItemUsed = -1;
            sourceAmmoUsed = -1;
            sourceNPC = pNPC;
            sourceProjIdentity = pIdentity;
            if (!projectile.hostile && projectile.owner > -1 && projectile.owner < Main.maxPlayers)
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
                }
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                if (sourceProjIdentity > 0)
                {
                    sourceProj = AequusHelpers.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
                    if (sourceProj == -1 || Main.projectile[sourceProj].type != sourceProjType)
                    {
                        sourceProjIdentity = -1;
                        if (projectile.ModProjectile is Hooks.IOnUnmatchingProjectileParents unmatchingMethod)
                        {
                            unmatchingMethod.OnUnmatchingProjectileParents(this, sourceProj);
                        }
                    }
                }
            }
            return true;
        }

        public int ProjectileOwner(Projectile projectile)
        {
            return AequusHelpers.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(defExtraUpdates > 0);
            if (defExtraUpdates > 0)
            {
                binaryWriter.Write((ushort)defExtraUpdates);
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