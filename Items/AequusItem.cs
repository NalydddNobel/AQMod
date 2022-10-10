using Aequus;
using Aequus.Buffs;
using Aequus.Common;
using Aequus.Common.ItemDrops;
using Aequus.Graphics;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools;
using Aequus.Items.Tools.Misc;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Projectiles.Misc;
using Aequus.Tiles;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items
{
    public class AequusItem : GlobalItem, IAddRecipes
    {
        public delegate bool CustomCoatingFunction(int x, int y, Player player);

        public static HashSet<int> SummonStaff { get; private set; }
        public static HashSet<int> CritOnlyModifier { get; private set; }

        public static List<int> IsLegendaryFish { get; private set; }
        public static Dictionary<int, CustomCoatingFunction> ApplyCustomCoating { get; private set; }
        public static List<CustomCoatingFunction> RemoveCustomCoating { get; private set; }
        public static Dictionary<int, string> RarityNames { get; private set; }

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public byte shopQuoteType;
        public byte noGravityTime;
        public bool accBoost;
        public bool naturallyDropped;
        public bool unOpenedChestItem;

        public override void Load()
        {
            RemoveCustomCoating = new List<CustomCoatingFunction>();
            ApplyCustomCoating = new Dictionary<int, CustomCoatingFunction>();
            IsLegendaryFish = new List<int>();
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
            On.Terraria.GameContent.Creative.ItemFilters.Weapon.FitsFilter += Weapon_FitsFilter;
            On.Terraria.GameContent.Creative.ItemFilters.Tools.FitsFilter += Tools_FitsFilter;
            On.Terraria.GameContent.Creative.ItemFilters.MiscAccessories.FitsFilter += MiscAccessories_FitsFilter;
        }

        private static bool Weapon_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.Weapon.orig_FitsFilter orig, ItemFilters.Weapon self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is BaseSoulCandle;
        }
        private static bool MiscAccessories_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.MiscAccessories.orig_FitsFilter orig, ItemFilters.MiscAccessories self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is RichMansMonocle || entry.ModItem is ForgedCard || entry.ModItem is FaultyCoin;
        }
        private static bool Tools_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.Tools.orig_FitsFilter orig, ItemFilters.Tools self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is PhysicsGun || entry.ModItem is Bellows || entry.ModItem is GhostlyGrave || entry.ModItem is Pumpinator;
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
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
            RemoveCustomCoating?.Clear();
            RemoveCustomCoating = null;
            ApplyCustomCoating?.Clear();
            ApplyCustomCoating = null;
            RarityNames?.Clear();
            RarityNames = null;
            IsLegendaryFish?.Clear();
            IsLegendaryFish = null;
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
            if (naturallyDropped && item.IsACoin && player.Aequus().accFoolsGoldRing)
            {
                int multiplier = 1;
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

            accBoost = false;
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

        public override bool? UseItem(Item item, Player player)
        {
            var aequus = player.Aequus();
            if (item.damage > 0 && !item.noUseGraphic && !item.noMelee
                && aequus.accHyperCrystal != null && aequus.hyperCrystalCooldownMelee == 0)
            {
                aequus.hyperCrystalCooldownMelee = aequus.hyperCrystalCooldownMax / 2;
                if (Main.myPlayer == player.whoAmI)
                {
                    switch (item.useStyle)
                    {
                        case ItemUseStyleID.Swing:
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center + new Vector2(0f, -80f - player.height), new Vector2(3f * player.direction, 3f),
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 2f);
                            break;

                        default:
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center, Vector2.Normalize(Main.MouseWorld - player.Center) * 4f,
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 2f);
                            break;
                    }
                }
            }
            return null;
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
                accBoost = false;
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
            writer.Write(naturallyDropped);
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
                case ItemID.QueenBeeBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    }
                    break;

                case ItemID.TwinsBossBag:
                    {
                        itemLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "AllMechs", "Mods.Aequus.DropCondition.AllMechs"), ModContent.ItemType<TheReconstruction>()));
                    }
                    break;

                case ItemID.IronCrate:
                case ItemID.IronCrateHard:
                    {
                        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GlowCore>(), 6));
                        itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(2, ModContent.ItemType<BoneRing>(), ModContent.ItemType<BattleAxe>(), ModContent.ItemType<Bellows>()));
                    }
                    break;

                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    {
                        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Slingshot>(), 3));
                    }
                    break;

                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    {
                        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrystalDagger>(), 3));
                    }
                    break;

                case ItemID.LockBox:
                    {
                        itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Valari>(), ModContent.ItemType<Revenant>(), ModContent.ItemType<DungeonCandle>(), ModContent.ItemType<PandorasBox>()));
                    }
                    break;
            }
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