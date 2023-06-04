using Aequus.Buffs;
using Aequus.Common.ModPlayers;
using Aequus.Common.Utilities;
using Aequus.Items.Accessories.Misc.Money;
using Aequus.Projectiles.Misc.Friendly;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items {
    [LegacyName("CooldownsItem", "ItemNameTag", "TooltipsGlobal")]
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes {
        public static int SuctionChestCheck;
        public static int suctionChestCheckAmt;

        public record struct NewItem(IEntitySource source, int X, int Y, int Width, int Height, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup);
        public static bool EnablePreventItemDrops = false;
        public static bool EnableCacheItemDrops = false;
        public static List<NewItem> CachedItemDrops = new();

        public int defenseChange = 0;
        public bool naturallyDropped = false;
        public bool prefixPotionsBounded = false;

        public EquipEmpowerment equipEmpowerment = null;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public AequusItem() {
            NameTag = null;
            RenameCount = 0;
        }

        public override void Load() {
            Load_Prefixes();
            Load_DataSets();
            Load_Cooldown();
            Load_Renaming();
            Load_Shimmer();
            EnablePreventItemDrops = false;
            Hook_Item_NewItem = Aequus.Detour(
                typeof(Item).GetMethod("NewItem_Inner", BindingFlags.NonPublic | BindingFlags.Static),
                typeof(AequusItem).GetMethod(nameof(Item_NewItem), BindingFlags.NonPublic | BindingFlags.Static)
            );
            On_NPC.NPCLoot_DropHeals += NPCLoot_DropHeals;
        }

        #region Hooks
        private static object Hook_Item_NewItem;
        private static int Item_NewItem(Func<IEntitySource, int, int, int, int, Item, int, int, bool, int, bool, bool, int> orig, IEntitySource source, int X, int Y, int Width, int Height, Item itemToClone, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup) {
            if (EnableCacheItemDrops) {
                CachedItemDrops.Add(new(source, X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup));
            }

            if (EnablePreventItemDrops) {
                Main.item[Main.maxItems] = new(Type, Stack, pfix);
                return Main.maxItems;
            }

            if (!EnableCacheItemDrops) {
                CachedItemDrops.Clear();
            }

            return orig(source, X, Y, Width, Height, itemToClone, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
        }

        private static void NPCLoot_DropHeals(On_NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer) {
            if (closestPlayer.HasBuff<ManathirstBuff>()) {
                Item.NewItem(self.GetSource_Loot(), self.getRect(), ItemID.Star);
                return;
            }
            if (closestPlayer.HasBuff<BloodthirstBuff>()) {
                Item.NewItem(self.GetSource_Loot(), self.getRect(), ItemID.Heart);
                return;
            }
            orig(self, closestPlayer);
        }
        #endregion

        public void PostSetupContent(Aequus aequus) {
            PostSetupContent_DataSets();
        }

        public void AddRecipes(Aequus aequus) {
            AddRecipes_DataSets();
        }

        public override void Unload() {
            Hook_Item_NewItem = null;
            CachedItemDrops.Clear();
            EnablePreventItemDrops = false;
            Unload_Renaming();
            Unload_Cooldown();
            Unload_DataSets();
        }

        public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded) {

            var slotItem = player.GetAccessory(slot, modded);
            return slotItem.ModItem is not FaultyCoin;
        }

        public override void SetDefaults(Item item) {
            SetDefaults_VanillaChanges(item);
            prefixPotionsBounded = false;
            reversedGravity = false;
            equipEmpowerment = null;
        }

        public override void OnSpawn(Item item, IEntitySource source) {
            itemGravityMultiplier = 1f;
            reversedGravity = false;
            if (source is EntitySource_Loot) {
                naturallyDropped = true;
            }

            if (item.IsACoin || item.IsHeartPickup() || item.IsManaPickup()) {
                return;
            }

            if (Helper.HereditarySource(source, out var entity)) {
                OnSpawn_CheckGravity(entity);
            }
            OnSpawn_CheckLuckyDrop(item);
        }

        public override void UpdateInventory(Item item, Player player) {
            if (itemGravityCheck != 255)
                itemGravityCheck = 0;
            itemGravityMultiplier = 1f;
            reversedGravity = false;
            luckyDrop = 0;
            if (!AequusPlayer.EquipmentModifierUpdate) {
                equipEmpowerment = null;
            }
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
            equipEmpowerment = null;
            CheckNameTag(item);
            Update_LuckyDrop(item);
            Update_CheckGravity(item, ref gravity, ref maxFallSpeed);
            Update_ReversedGravity(item, ref gravity, maxFallSpeed);
        }

        public override void UpdateVanity(Item item, Player player) {
            CheckNameTag(item);
        }

        public override bool? UseItem(Item item, Player player) {
            var aequus = player.Aequus();
            if (item.damage > 0 && !item.noUseGraphic && !item.noMelee && !item.IsATool()
                && aequus.accHyperCrystal != null && aequus.hyperCrystalCooldown <= aequus.hyperCrystalCooldownMax && aequus.hyperCrystalCooldownMelee <= 0) {
                if (aequus.hyperCrystalCooldown <= 0) {
                    aequus.hyperCrystalCooldown = aequus.hyperCrystalCooldownMax;
                    aequus.hyperCrystalCooldownMelee = aequus.hyperCrystalCooldown + item.useTime;
                    if (Main.myPlayer == player.whoAmI) {
                        if (item.useStyle == ItemUseStyleID.Swing || item.channel) {
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center + new Vector2(0f, -80f - player.height), new Vector2(3f * player.direction, 2f),
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 4f);
                        }
                        else {
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center, Vector2.Normalize(Main.MouseWorld - player.Center) * 4f,
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 3f);
                        }
                    }
                }
            }
            if (item.buffType > 0 && item.buffTime > 0) {
                if (prefixPotionsBounded && !Main.persistentBuff[item.buffType]) {
                    player.Aequus().BoundedPotionIDs.Add(item.buffType);
                }
                else {
                    player.Aequus().BoundedPotionIDs.Remove(item.buffType);
                }
            }
            return null;
        }

        public override bool ConsumeItem(Item item, Player player) {
            if (item.damage > 0 && item.CountsAsClass(DamageClass.Throwing) && player.Aequus().ammoAndThrowingCost33 && Main.rand.NextBool(3)) {
                return false;
            }
            return true;
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration) {
            var wingStats = player.Aequus().wingStats;
            speed = wingStats.horizontalSpeed.ApplyTo(speed);
            acceleration = wingStats.horizontalAcceleration.ApplyTo(acceleration);
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
            var wingStats = player.Aequus().wingStats;
            ascentWhenFalling = wingStats.verticalAscentWhenFalling.ApplyTo(ascentWhenFalling);
            ascentWhenRising = wingStats.verticalAscentWhenRising.ApplyTo(ascentWhenRising);
            maxCanAscendMultiplier = wingStats.verticalMaxCanAscendMultiplier.ApplyTo(maxCanAscendMultiplier);
            maxAscentMultiplier = wingStats.verticalMaxAscentMultiplier.ApplyTo(maxAscentMultiplier);
            constantAscend = wingStats.verticalMaxAscentMultiplier.ApplyTo(constantAscend);
        }

        public override void SaveData(Item item, TagCompound tag) {
            SaveDataAttribute.SaveData(tag, this);
        }

        public override void LoadData(Item item, TagCompound tag) {
            SaveDataAttribute.LoadData(tag, this);
        }
    }
}