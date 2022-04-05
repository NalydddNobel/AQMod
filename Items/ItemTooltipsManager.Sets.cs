using Aequus.Common.ID;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Potions;
using Aequus.Items.Misc;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed partial class ItemTooltipsHelper : GlobalItem
    {
        public static Color MysteriousGuideTooltip => new Color(225, 100, 255, 255);
        public static Color DemonSiegeTooltip => new Color(255, 170, 150, 255);

        public static Dictionary<int, ItemDedication> Dedicated;

        private static readonly string[] OutdatedTooltipNames = new string[]
        {
            "ItemName",
            "Favorite",
            "FavoriteDesc",
            "Social",
            "SocialDesc",
            "Damage",
            "CritChance",
            "Speed",
            "Knockback",
            "FishingPower",
            "NeedsBait",
            "BaitPower",
            "Equipable",
            "WandConsumes",
            "Quest",
            "Vanity",
            "Defense",
            "PickPower",
            "AxePower",
            "HammerPower",
            "TileBoost",
            "HealLife",
            "HealMana",
            "UseMana",
            "Placeable",
            "Ammo",
            "Consumable",
            "Material",
            "Tooltip#",
            "EtherianManaWarning",
            "WellFedExpert",
            "BuffTime",
            "OneDropLogo",
            "PrefixDamage",
            "PrefixSpeed",
            "PrefixCritChance",
            "PrefixUseMana",
            "PrefixSize",
            "PrefixShootSpeed",
            "PrefixKnockback",
            "PrefixAccDefense",
            "PrefixAccMaxMana",
            "PrefixAccCritChance",
            "PrefixAccDamage",
            "PrefixAccMoveSpeed",
            "PrefixAccMeleeSpeed",
            "SetBonus",
            "Expert",
            "SpecialPrice",
            "Price",
        };

        public override void SetStaticDefaults()
        {
            Dedicated = new Dictionary<int, ItemDedication>()
            {
                [ModContent.ItemType<MirrorsCall>()] = new ItemDedication(new Color(110, 110, 128, 255)),
                [ModContent.ItemType<NoonPotion>()] = new ItemDedication(new Color(200, 80, 50, 255)),
                [ModContent.ItemType<FamiliarPickaxe>()] = new ItemDedication(new Color(200, 65, 70, 255)),
                //[ModContent.ItemType<MothmanMask>()] = new ItemDedication(new Color(50, 75, 250, 255)),
                //[ModContent.ItemType<RustyKnife>()] = new ItemDedication(new Color(30, 255, 60, 255)),
                //[ModContent.ItemType<Thunderbird>()] = new ItemDedication(new Color(200, 125, 255, 255)),
                [ModContent.ItemType<Baguette>()] = new ItemDedication(new Color(187, 142, 42, 255)),
                [ModContent.ItemType<StudiesOfTheInkblot>()] = new ItemDedication(new Color(110, 110, 128, 255)),
            };
        }

        public override void Unload()
        {
            Dedicated?.Clear();
            Dedicated = null;
        }
    }
}