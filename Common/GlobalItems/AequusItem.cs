using Aequus.Buffs;
using Aequus.Common.Utilities;
using Aequus.Items.Misc;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.Projectiles.Misc.Friendly;
using Aequus.Tiles.CraftingStation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items
{
    [LegacyName("CooldownsItem", "ItemNameTag", "TooltipsGlobal")]
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public static int SuctionChestCheck;
        public static int suctionChestCheckAmt;

        public record struct NewItem(IEntitySource source, int X, int Y, int Width, int Height, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup);
        public static bool EnablePreventItemDrops;
        public static bool EnableCacheItemDrops;
        public static List<NewItem> CachedItemDrops = new();

        public int accStacks;
        public int defenseChange;
        public bool naturallyDropped;
        public bool prefixPotionsBounded;

        internal bool crownOfBloodUsed;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public AequusItem()
        {
            NameTag = null;
            RenameCount = 0;
        }

        public override void Load()
        {
            Load_DataSets();
            Load_Paint();
            Load_Cooldown();
            Load_Tooltips();
            Load_Renaming();
            Load_Shimmer();
            EnablePreventItemDrops = false;
            Hook_Item_NewItem = Aequus.Detour(
                typeof(Item).GetMethod("NewItem_Inner", BindingFlags.NonPublic | BindingFlags.Static),
                typeof(AequusItem).GetMethod(nameof(Item_NewItem), BindingFlags.NonPublic | BindingFlags.Static)
            );
            On.Terraria.NPC.NPCLoot_DropHeals += NPCLoot_DropHeals;
        }

        #region Hooks
        private static object Hook_Item_NewItem;
        private static int Item_NewItem(Func<IEntitySource, int, int, int, int, Item, int, int, bool, int, bool, bool, int> orig, IEntitySource source, int X, int Y, int Width, int Height, Item itemToClone, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup)
        {
            if (EnableCacheItemDrops) {
                CachedItemDrops.Add(new(source, X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup));
            }

            if (EnablePreventItemDrops)
            {
                Main.item[Main.maxItems] = new(Type, Stack, pfix);
                return Main.maxItems;
            }

            if (!EnableCacheItemDrops) {
                CachedItemDrops.Clear();
            }

            return orig(source, X, Y, Width, Height, itemToClone, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
        }

        private static void NPCLoot_DropHeals(On.Terraria.NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer)
        {
            if (closestPlayer.HasBuff<ManathirstBuff>())
            {
                Item.NewItem(self.GetSource_Loot(), self.getRect(), ItemID.Star);
                return;
            }
            if (closestPlayer.HasBuff<BloodthirstBuff>())
            {
                Item.NewItem(self.GetSource_Loot(), self.getRect(), ItemID.Heart);
                return;
            }
            orig(self, closestPlayer);
        }
        #endregion

        public void PostSetupContent(Aequus aequus)
        {
            PostSetupContent_DataSets();
        }

        public void AddRecipes(Aequus aequus)
        {
            AddRecipes_DataSets();
        }

        public override void Unload()
        {
            Hook_Item_NewItem = null;
            CachedItemDrops.Clear();
            EnablePreventItemDrops = false;
            Unload_Renaming();
            Unload_Tooltips();
            Unload_Cooldown();
            Unload_Paint();
            Unload_DataSets();
        }

        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (context is RecipeCreationContext recipeContext && Main.LocalPlayer.adjTile[ModContent.TileType<RecyclingMachineTile>()] && ItemScrap.ScrappableRarities.Contains(item.rare) && Main.LocalPlayer.RollLuck(4) == 0)
            {
                if (recipeContext.recipe.requiredItem.Count == 1 && ItemHelper.CanBeCraftedInto(item.type, recipeContext.recipe.requiredItem[0].type))
                {
                    return;
                }
                var scrap = SetDefaults<ItemScrap>();
                scrap.ModItem<ItemScrap>().Rarity = item.OriginalRarity;
                scrap.ModItem<ItemScrap>().UpdateRarity();
                Main.LocalPlayer.QuickSpawnClonedItemDirect(item.GetSource_FromThis("Aequus: Recipe Scrap"), scrap, 1);
            }
        }

        public override void SetDefaults(Item item)
        {
            SetDefaults_VanillaChanges(item);
            prefixPotionsBounded = false;
            accStacks = 1;
            noGravityTime = 0;
            reversedGravity = false;
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            noGravityTime = 0;
            reversedGravity = false;
            if (source is EntitySource_Loot)
            {
                naturallyDropped = true;
            }
            OnSpawn_CheckLuckyDrop(item, source);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            crownOfBloodUsed = false;
            noGravityTime = 0;
            reversedGravity = false;
            luckyDrop = false;
            if (player.Aequus().accBloodCrownSlot != -2)
            {
                accStacks = 1;
            }
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            crownOfBloodUsed = false;
            accStacks = 1;
            CheckNameTag(item);
            Update_LuckyDrop(item);
            Update_NoGravity(item, ref gravity);
            Update_ReversedGravity(item, ref gravity, maxFallSpeed);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (player.Aequus().accBloodCrownSlot != -2)
            {
                accStacks = 1;
            }
            if (accStacks <= 1)
            {
                crownOfBloodUsed = false;
            }
            CheckNameTag(item);
            UpdateEquip_Prefixes(item, player);
            if (defenseChange < 0)
            {
                player.Aequus().negativeDefense -= defenseChange;
            }
            else
            {
                player.statDefense += defenseChange;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            CheckNameTag(item);
            UpdateAccessory_Prefixes(item, player, hideVisual);
            if (defenseChange < 0)
            {
                player.Aequus().negativeDefense -= defenseChange;
            }
            if (player.Aequus().accBloodCrownSlot != -2)
            {
                accStacks = 1;
            }
            if (accStacks <= 1)
            {
                crownOfBloodUsed = false;
            }
            if (item.type == ItemID.RoyalGel || player.npcTypeNoAggro[NPCID.BlueSlime])
            {
                player.npcTypeNoAggro[ModContent.NPCType<WhiteSlime>()] = true;
            }
        }

        public override void UpdateVanity(Item item, Player player)
        {
            CheckNameTag(item);
        }

        public override bool? UseItem(Item item, Player player)
        {
            var aequus = player.Aequus();
            if (item.damage > 0 && !item.noUseGraphic && !item.noMelee && !item.IsATool()
                && aequus.accHyperCrystal != null && aequus.hyperCrystalCooldown <= aequus.hyperCrystalCooldownMax && aequus.hyperCrystalCooldownMelee <= 0)
            {
                if (aequus.hyperCrystalCooldown <= 0)
                {
                    aequus.hyperCrystalCooldown = aequus.hyperCrystalCooldownMax;
                    aequus.hyperCrystalCooldownMelee = aequus.hyperCrystalCooldown + item.useTime;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        if (item.useStyle == ItemUseStyleID.Swing || item.channel)
                        {
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center + new Vector2(0f, -80f - player.height), new Vector2(3f * player.direction, 2f),
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 4f);
                        }
                        else
                        {
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center, Vector2.Normalize(Main.MouseWorld - player.Center) * 4f,
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 3f);
                        }
                    }
                }
            }
            if (item.buffType > 0 && item.buffTime > 0)
            {
                if (prefixPotionsBounded && !Main.persistentBuff[item.buffType])
                {
                    player.Aequus().BoundedPotionIDs.Add(item.buffType);
                }
                else
                {
                    player.Aequus().BoundedPotionIDs.Remove(item.buffType);
                }
            }
            return null;
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (item.damage > 0 && item.CountsAsClass(DamageClass.Throwing) && player.Aequus().ammoAndThrowingCost33 && Main.rand.NextBool(3))
            {
                return false;
            }
            return true;
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            var wingStats = player.Aequus().wingStats;
            speed = wingStats.horizontalSpeed.ApplyTo(speed);
            acceleration = wingStats.horizontalAcceleration.ApplyTo(acceleration);
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            var wingStats = player.Aequus().wingStats;
            ascentWhenFalling = wingStats.verticalAscentWhenFalling.ApplyTo(ascentWhenFalling);
            ascentWhenRising = wingStats.verticalAscentWhenRising.ApplyTo(ascentWhenRising);
            maxCanAscendMultiplier = wingStats.verticalMaxCanAscendMultiplier.ApplyTo(maxCanAscendMultiplier);
            maxAscentMultiplier = wingStats.verticalMaxAscentMultiplier.ApplyTo(maxAscentMultiplier);
            constantAscend = wingStats.verticalMaxAscentMultiplier.ApplyTo(constantAscend);
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<AequusPlayer>().moroSummonerFruit && SummonStaff.Contains(item.type))
            {
                mult = 0f;
            }
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
        }
    }
}