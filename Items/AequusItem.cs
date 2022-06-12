using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusItem : GlobalItem
    {
        public static HashSet<int> CritOnlyModifier { get; private set; }
        public static HashSet<int> BankEquipFuncs { get; private set; }

        public override void Load()
        {
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
        }

        public override void Unload()
        {
            BankEquipFuncs?.Clear();
            BankEquipFuncs = null;
            CritOnlyModifier?.Clear();
            CritOnlyModifier = null;
        }

        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.ShadowKey)
            {
                item.rare = ItemRarityID.Blue;
                item.value = Item.buyPrice(gold: 80);
            }
            else if (item.type == ItemID.DiscountCard)
            {
                item.accessory = false;
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.type == ItemID.DiscountCard && !player.discount)
            {
                player.ApplyEquipFunctional(item, false);
            }
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<AequusPlayer>().moroUsed && ItemsCatalogue.SummonStaff.Contains(item.type))
            {
                mult = 0f;
            }
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            if (player.GetModPlayer<AequusPlayer>().moroUsed && ItemsCatalogue.SummonStaff.Contains(item.type))
            {
                return 2f;
            }
            return 1f;
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
    }
}