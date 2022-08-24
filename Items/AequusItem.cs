using Aequus.Common;
using Aequus.Graphics;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools.Info;
using Aequus.Items.Tools.Mining;
using Aequus.Items.Tools.Misc;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusItem : GlobalItem, IAddRecipes
    {
        public static HashSet<int> LegendaryFish { get; private set; }
        public static HashSet<int> SummonStaff { get; private set; }
        public static HashSet<int> CritOnlyModifier { get; private set; }
        public static HashSet<int> BankEquipFuncs { get; private set; }

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public byte shopQuoteType;
        public byte noGravityTime;

        public override void Load()
        {
            LegendaryFish = new HashSet<int>();
            SummonStaff = new HashSet<int>();
            CritOnlyModifier = new HashSet<int>()
            {
                PrefixID.Keen,
                PrefixID.Zealous,
            };
            BankEquipFuncs = new HashSet<int>()
            {
                ItemID.CellPhone,
                ItemID.PDA,
                ItemID.REK,
                ItemID.GoblinTech,
                ItemID.FishFinder,
                ItemID.GPS,
                ItemID.Radar,
                ItemID.LifeformAnalyzer,
                ItemID.TallyCounter,
                ItemID.DPSMeter,
                ItemID.Stopwatch,
                ItemID.MetalDetector,
                ItemID.Sextant,
                ItemID.WeatherRadio,
                ItemID.FishermansGuide,
                ItemID.Compass,
                ItemID.DepthMeter,
                ItemID.PlatinumWatch,
                ItemID.TungstenWatch,
                ItemID.TinWatch,
                ItemID.GoldWatch,
                ItemID.SilverWatch,
                ItemID.CopperWatch,

                ItemID.DiscountCard,
                ItemID.ShadowKey,
            };
            On.Terraria.GameContent.Creative.ItemFilters.Weapon.FitsFilter += Weapon_FitsFilter;
            On.Terraria.GameContent.Creative.ItemFilters.Tools.FitsFilter += Tools_FitsFilter;
            On.Terraria.GameContent.Creative.ItemFilters.MiscAccessories.FitsFilter += MiscAccessories_FitsFilter;
        }

        private bool MiscAccessories_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.MiscAccessories.orig_FitsFilter orig, Terraria.GameContent.Creative.ItemFilters.MiscAccessories self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is RichMansMonocle || entry.ModItem is ForgedCard || entry.ModItem is FaultyCoin;
        }

        private bool Tools_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.Tools.orig_FitsFilter orig, Terraria.GameContent.Creative.ItemFilters.Tools self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is PhysicsGun || entry.ModItem is Bellows || entry.ModItem is GhostlyGrave || entry.ModItem is Pumpinator;
        }

        private bool Weapon_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.Weapon.orig_FitsFilter orig, Terraria.GameContent.Creative.ItemFilters.Weapon self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is BaseSoulCandle;
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = ContentSamples.ItemsByType[i];
                if (IsSummonStaff(item))
                {
                    SummonStaff.Add(i);
                }
            }
        }
        public static bool IsSummonStaff(Item item)
        {
            return item.damage > 0 && item.DamageType == DamageClass.Summon && item.shoot > ProjectileID.None && item.useStyle > ItemUseStyleID.None && (ContentSamples.ProjectilesByType[item.shoot].minionSlots > 0f || ContentSamples.ProjectilesByType[item.shoot].sentry);
        }

        public override void Unload()
        {
            SummonStaff?.Clear();
            SummonStaff = null;
            BankEquipFuncs?.Clear();
            BankEquipFuncs = null;
            CritOnlyModifier?.Clear();
            CritOnlyModifier = null;
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
            else if (item.type == ItemID.DiscountCard)
            {
                item.accessory = false;
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            noGravityTime = 0;
            if (item.type == ItemID.DiscountCard && !player.discount)
            {
                player.ApplyEquipFunctional(item, false);
            }
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

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(noGravityTime);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            noGravityTime = reader.ReadByte();
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<AequusPlayer>().moroSummonerFruit && AequusItem.SummonStaff.Contains(item.type))
            {
                mult = 0f;
            }
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            if (player.GetModPlayer<AequusPlayer>().moroSummonerFruit && AequusItem.SummonStaff.Contains(item.type))
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