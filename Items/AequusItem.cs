using Aequus;
using Aequus.Buffs;
using Aequus.Common;
using Aequus.Common.ItemDrops;
using Aequus.Graphics;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Consumables.Bait;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Items.Weapons.Summon.Necro.Scepters;
using Aequus.Projectiles.Misc.Friendly;
using Aequus.Tiles.Misc;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items
{
    public class AequusItem : GlobalItem, IAddRecipes
    {
        public delegate bool CustomCoatingFunction(int x, int y, Player player);

        public static HashSet<int> PrioritizeVoidBagPickup { get; private set; }
        public static HashSet<int> SummonStaff { get; private set; }
        public static HashSet<int> CritOnlyModifier { get; private set; }

        public static List<int> FruitIDs { get; private set; }
        public static List<int> LegendaryFishIDs { get; private set; }

        public static Dictionary<int, CustomCoatingFunction> ApplyCustomCoating { get; private set; }
        public static List<CustomCoatingFunction> RemoveCustomCoating { get; private set; }

        public static Dictionary<int, string> RarityNames { get; private set; }

        private static Dictionary<int, int> ItemToBannerCache;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public byte noGravityTime;
        public int accStacks;
        public bool naturallyDropped;
        public bool unOpenedChestItem;
        public bool prefixPotionsBounded;

        public override void Load()
        {
            PrioritizeVoidBagPickup = new HashSet<int>();
            FruitIDs = new List<int>()
            {
                ItemID.Apple,
                ItemID.Apricot,
                ItemID.Banana,
                ItemID.BlackCurrant,
                ItemID.BloodOrange,
                ItemID.Cherry,
                ItemID.Coconut,
                ItemID.Dragonfruit,
                ItemID.Elderberry,
                ItemID.Grapefruit,
                ItemID.Lemon,
                ItemID.Mango,
                ItemID.Peach,
                ItemID.Pineapple,
                //ItemID.Pomegranate,
                ItemID.Plum,
                ItemID.Rambutan,
                //ItemID.SpicyPepper,
                ItemID.Starfruit,
            };
            ItemToBannerCache = new Dictionary<int, int>();
            RemoveCustomCoating = new List<CustomCoatingFunction>();
            ApplyCustomCoating = new Dictionary<int, CustomCoatingFunction>();
            LegendaryFishIDs = new List<int>();
            SummonStaff = new HashSet<int>();
            CritOnlyModifier = new HashSet<int>()
            {
                PrefixID.Keen,
                PrefixID.Zealous,
            };
            RarityNames = new Dictionary<int, string>()
            {
                [ItemRarityID.Master] = "Mods.Aequus.ItemRarity.-13",
                [ItemRarityID.Expert] = "Mods.Aequus.ItemRarity.-12",
                [ItemRarityID.Quest] = "Mods.Aequus.ItemRarity.-11",
                [ItemRarityID.Gray] = "Mods.Aequus.ItemRarity.-1",
                [ItemRarityID.White] = "Mods.Aequus.ItemRarity.0",
                [ItemRarityID.Blue] = "Mods.Aequus.ItemRarity.1",
                [ItemRarityID.Green] = "Mods.Aequus.ItemRarity.2",
                [ItemRarityID.Orange] = "Mods.Aequus.ItemRarity.3",
                [ItemRarityID.LightRed] = "Mods.Aequus.ItemRarity.4",
                [ItemRarityID.Pink] = "Mods.Aequus.ItemRarity.5",
                [ItemRarityID.LightPurple] = "Mods.Aequus.ItemRarity.6",
                [ItemRarityID.Lime] = "Mods.Aequus.ItemRarity.7",
                [ItemRarityID.Yellow] = "Mods.Aequus.ItemRarity.8",
                [ItemRarityID.Cyan] = "Mods.Aequus.ItemRarity.9",
                [ItemRarityID.Red] = "Mods.Aequus.ItemRarity.10",
                [ItemRarityID.Purple] = "Mods.Aequus.ItemRarity.11",
            };

        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (Aequus.LogMore)
                Aequus.Instance.Logger.Info("Loading rarity name translations...");
            for (int i = ItemRarityID.Purple + 1; i < RarityLoader.RarityCount; i++)
            {
                try
                {
                    var rare = RarityLoader.GetRarity(i);
                    string key = $"Mods.Aequus.ItemRarity.{rare.Mod.Name}.{rare.Name}";
                    if (AequusText.ContainsKey(key))
                    {
                        RarityNames.Add(rare.Type, key);
                        if (Aequus.LogMore)
                            Aequus.Instance.Logger.Info($"Autoloaded rarity key: {key}");
                    }
                    //else if (Aequus.LogMore)
                    //{
                    //    Aequus.Instance.Logger.Info($"Key not found: {key}");
                    //}
                }
                catch
                {
                }
            }

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = ContentSamples.ItemsByType[i];
                if (item.damage > 0 && item.DamageType == DamageClass.Summon && item.shoot > ProjectileID.None && item.useStyle > ItemUseStyleID.None && (ContentSamples.ProjectilesByType[item.shoot].minionSlots > 0f || ContentSamples.ProjectilesByType[item.shoot].sentry))
                {
                    SummonStaff.Add(i);
                }
            }
        }

        public override void Unload()
        {
            PrioritizeVoidBagPickup?.Clear();
            PrioritizeVoidBagPickup = null;
            ItemToBannerCache?.Clear();
            ItemToBannerCache = null;
            RemoveCustomCoating?.Clear();
            RemoveCustomCoating = null;
            ApplyCustomCoating?.Clear();
            ApplyCustomCoating = null;
            RarityNames?.Clear();
            RarityNames = null;
            LegendaryFishIDs?.Clear();
            LegendaryFishIDs = null;
            SummonStaff?.Clear();
            SummonStaff = null;
            CritOnlyModifier?.Clear();
            CritOnlyModifier = null;
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
                Main.LocalPlayer.QuickSpawnClonedItemDirect(item.GetSource_FromThis("Recipe Scrap"), scrap, 1);
            }
        }

        public override bool CanStack(Item item1, Item item2)
        {
            return item1.prefix == item2.prefix;
        }

        public override bool CanStackInWorld(Item item1, Item item2)
        {
            return item1.prefix == item2.prefix;
        }

        public override bool OnPickup(Item item, Player player)
        {
            if (naturallyDropped && item.IsACoin && player.Aequus().accFoolsGoldRing > 0)
            {
                int multiplier = player.Aequus().accFoolsGoldRing;
                if (item.value > Item.silver)
                {
                    multiplier++;
                }
                if (item.value > Item.gold)
                {
                    multiplier++;
                }
                if (item.value > Item.platinum)
                {
                    multiplier++;
                }
                player.AddBuff(ModContent.BuffType<FoolsGoldRingBuff>(), 120 * multiplier);
            }
            naturallyDropped = false;
            if (PrioritizeVoidBagPickup.Contains(item.type))
            {
                if (AequusHelpers.TryStackingInto(player.bank4.item, Chest.maxItems, item))
                {
                    PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, item.stack);
                    item.TurnToAir();
                    SoundEngine.PlaySound(SoundID.Grab.WithVolume(0.3f).WithPitchOffset(0.6f));
                    SoundEngine.PlaySound(SoundID.Item130.WithVolume(0.4f).WithPitchOffset(0.5f));
                    return false;
                }
            }
            return true;
        }

        public override void SetDefaults(Item item)
        {
            if (item.type >= Main.maxItemTypes)
            {
                short id = AequusGlowMasks.GetID(item.type);
                if (id > 0)
                {
                    item.glowMask = id;
                }
            }

            if (item.type == ItemID.ShadowKey)
            {
                item.rare = ItemRarityID.Blue;
                item.value = Item.buyPrice(gold: 15);
            }
            else if (item.type == ItemID.GravityGlobe)
            {
                if (GameplayConfig.Instance.EarlyGravityGlobe)
                {
                    item.expert = false;
                }
            }

            prefixPotionsBounded = false;
            accStacks = 1;
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is EntitySource_Loot)
            {
                naturallyDropped = true;
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            noGravityTime = 0;
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (noGravityTime > 0)
            {
                item.velocity.Y *= 0.95f;
                gravity = 0f;
                noGravityTime--;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.Aequus().accBloodCrownSlot != -2)
                accStacks = 1;
        }

        public override bool? UseItem(Item item, Player player)
        {
            var aequus = player.Aequus();
            if (item.damage > 0 && !item.noUseGraphic && !item.noMelee
                && aequus.accHyperCrystal != null && aequus.hyperCrystalCooldownMelee == 0)
            {
                aequus.hyperCrystalCooldownMelee = aequus.hyperCrystalCooldownMax;
                if (Main.myPlayer == player.whoAmI)
                {
                    if (item.useStyle == ItemUseStyleID.Swing && item.shoot == ProjectileID.None)
                    {
                        Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center + new Vector2(0f, -80f - player.height), new Vector2(3f * player.direction, 3f),
                            ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 4f);
                    }
                    else
                    {
                        Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center, Vector2.Normalize(Main.MouseWorld - player.Center) * 4f,
                            ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 3f);
                    }
                }
            }
            if (item.buffType > 0 && item.buffTime > 0)
            {
                if (prefixPotionsBounded && !Main.persistentBuff[item.buffType])
                {
                    player.Aequus().boundedPotionIDs.Add(item.buffType);
                }
                else
                {
                    player.Aequus().boundedPotionIDs.Remove(item.buffType);
                }
            }
            return null;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.EquipAccessory)
            {
                var aequus = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
                if (aequus.accBloodCrownSlot > -1 && AequusUI.CurrentItemSlot.Slot == aequus.accBloodCrownSlot)
                {
                    var backFrame = TextureAssets.InventoryBack16.Value.Frame();
                    var drawPosition = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
                    var color = new Color(150, 60, 60, 255);

                    spriteBatch.Draw(TextureAssets.InventoryBack16.Value, drawPosition, backFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            var bb = new BitsByte(naturallyDropped);
            writer.Write(bb);
            writer.Write(noGravityTime);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            var bb = (BitsByte)reader.ReadByte();
            naturallyDropped = bb[0];
            noGravityTime = reader.ReadByte();
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<AequusPlayer>().moroSummonerFruit && SummonStaff.Contains(item.type))
            {
                mult = 0f;
            }
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            if (player.GetModPlayer<AequusPlayer>().moroSummonerFruit && SummonStaff.Contains(item.type))
            {
                return 2f;
            }
            return 1f;
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {

                case ItemID.MoonLordBossBag:
                    if (GameplayConfig.Instance.EarlyGravityGlobe)
                        itemLoot.RemoveWhere((itemDrop) => itemDrop is CommonDrop commonDrop && commonDrop.itemId == ItemID.GravityGlobe);
                    if (GameplayConfig.Instance.EarlyPortalGun)
                        itemLoot.RemoveWhere((itemDrop) => itemDrop is CommonDrop commonDrop && commonDrop.itemId == ItemID.PortalGun);
                    break;

                case ItemID.PlanteraBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    break;

                case ItemID.TwinsBossBag:
                case ItemID.DestroyerBossBag:
                case ItemID.SkeletronPrimeBossBag:
                    itemLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "AllMechs", "Mods.Aequus.DropCondition.AllMechs"), ModContent.ItemType<TheReconstruction>()));
                    break;

                case ItemID.IronCrate:
                case ItemID.IronCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GlowCore>(), 6));
                    itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(2, ModContent.ItemType<BoneRing>(), ModContent.ItemType<BattleAxe>(), ModContent.ItemType<Bellows>()));
                    break;

                case ItemID.CorruptFishingCrate:
                case ItemID.CorruptFishingCrateHard:
                case ItemID.CrimsonFishingCrate:
                case ItemID.CrimsonFishingCrateHard:
                case ItemID.DungeonFishingCrate:
                case ItemID.DungeonFishingCrateHard:
                case ItemID.HallowedFishingCrate:
                case ItemID.HallowedFishingCrateHard:
                case ItemID.JungleFishingCrate:
                case ItemID.JungleFishingCrateHard:
                case ItemID.LavaCrate:
                case ItemID.LavaCrateHard:
                case ItemID.OasisCrate:
                case ItemID.OasisCrateHard:
                case ItemID.OceanCrate:
                case ItemID.OceanCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Slingshot>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrystalDagger>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.LockBox:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Valari>(), ModContent.ItemType<Revenant>(), ModContent.ItemType<DungeonCandle>(), ModContent.ItemType<PandorasBox>()));
                    break;
            }
        }

        public static int ItemToBanner(int itemID)
        {
            if (ItemToBannerCache.TryGetValue(itemID, out int banner))
            {
                return banner;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                int bannerID = Item.NPCtoBanner(i);
                if (Item.BannerToItem(bannerID) == itemID)
                {
                    ItemToBannerCache.Add(itemID, bannerID);
                    return bannerID;
                }
            }
            ItemToBannerCache.Add(itemID, 0);
            return 0;
        }

        public static Item SetDefaults(int type, bool checkMaterial = true)
        {
            var i = new Item();
            i.SetDefaults(type, noMatCheck: !checkMaterial);
            return i;
        }
        public static Item SetDefaults<T>(bool checkMaterial = true) where T : ModItem
        {
            return SetDefaults(ModContent.ItemType<T>(), checkMaterial);
        }

        public static int NewItemCloned(IEntitySource source, Vector2 pos, Item item)
        {
            int i = Item.NewItem(source, pos, item.type, item.stack);
            Main.item[i] = item.Clone();
            Main.item[i].active = true;
            Main.item[i].whoAmI = i;
            Main.item[i].Center = pos;
            Main.item[i].stack = item.stack;
            return i;
        }

        public static void AntiGravityNearbyItems(Vector2 position, float distance)
        {
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i].active && !ItemID.Sets.ItemNoGravity[Main.item[i].type]
                    && Vector2.Distance(Main.item[i].Center, position) < distance)
                {
                    Main.item[i].Aequus().noGravityTime = 30;
                }
            }
        }
    }
}