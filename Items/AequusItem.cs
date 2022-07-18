using Aequus.Common;
using Aequus.Graphics;
using Aequus.Items.Accessories;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Candles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
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
        }
        private bool Weapon_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.Weapon.orig_FitsFilter orig, Terraria.GameContent.Creative.ItemFilters.Weapon self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is SoulCandle;
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

        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if (context == "bossBag")
            {
                switch (arg) 
                {
                    case ItemID.QueenBeeBossBag:
                        player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<OrganicEnergy>(), 3);
                        break;
                }
            }
            else if (context == "lockBox")
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(ItemID.LockBox), AequusWorld.DungeonChestItem(Main.rand.Next(AequusWorld.DungeonChestItemTypesMax)));
            }
            else if (context == "crate")
            {
                if (arg == ItemID.IronCrate)
                {
                    if (Main.rand.NextBool(6))
                    {
                        player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<GlowCore>());
                    }

                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<BoneRing>());
                            break;

                        case 1:
                            player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<BattleAxe>());
                            break;

                        case 2:
                            player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<Bellows>());
                            break;
                    }
                }
                else if (arg == ItemID.FloatingIslandFishingCrate || arg == ItemID.FloatingIslandFishingCrateHard)
                {
                    if (Main.rand.NextBool(3))
                    {
                        player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<Slingshot>());
                    }
                }
                else if (arg == ItemID.FrozenCrate || arg == ItemID.FrozenCrateHard)
                {
                    if (Main.rand.NextBool(3))
                    {
                        player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<Slingshot>());
                    }
                }
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