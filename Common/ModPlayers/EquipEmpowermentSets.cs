using Aequus.Items.Accessories.CrownOfBlood;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.ModPlayers {
    public class EquipEmpowermentSets : ILoadable {
        public static readonly Dictionary<int, Action<Item, Player, bool>> SpecialUpdate = new();
        public static readonly Dictionary<int, Action<IEntitySource, Item, Projectile>> OnSpawnProjectile = new();

        public void Load(Mod mod) {
            SpecialUpdate[ItemID.EoCShield] = CrownOfBloodItem.SpecialUpdate_ShieldOfCthulhu;
            SpecialUpdate[ItemID.HiveBackpack] = CrownOfBloodItem.SpecialUpdate_HivePack;
            SpecialUpdate[ItemID.BrainOfConfusion] = CrownOfBloodItem.SpecialUpdate_BrainOfConfusion;
            SpecialUpdate[ItemID.BoneHelm] = CrownOfBloodItem.SpecialUpdate_BoneHelm;
            OnSpawnProjectile[ItemID.BoneGlove] = CrownOfBloodItem.OnSpawn_BoneGlove;
            OnSpawnProjectile[ItemID.VolatileGelatin] = CrownOfBloodItem.OnSpawn_VolatileGelatin;
            OnSpawnProjectile[ItemID.BoneHelm] = CrownOfBloodItem.OnSpawn_VolatileGelatin;
        }

        public void Unload() {
            SpecialUpdate.Clear();
            OnSpawnProjectile.Clear();
        }
    }
}