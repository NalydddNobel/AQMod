using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public sealed class AQItem : GlobalItem
    {
        public static class Prices
        {
            public static int PotionValue => Item.sellPrice(silver: 2);
            public static int EnergySellValue => Item.sellPrice(silver: 10);
            public static int EnergyBuyValue => Item.buyPrice(gold: 3);
            public static int CrabsonWeaponValue => Item.sellPrice(silver: 25);
            public static int CorruptionWeaponValue => Item.sellPrice(silver: 50);
            public static int CrimsonWeaponValue => Item.sellPrice(silver: 55);
            public static int GlimmerWeaponValue => Item.sellPrice(silver: 75);
            public static int DemonSiegeWeaponValue => Item.sellPrice(silver: 80);
            public static int OmegaStariteWeaponValue => Item.sellPrice(gold: 4, silver: 50);
            public static int GaleStreamsValue => Item.sellPrice(gold: 4);
            public static int PostMechsEnergyWeaponValue => Item.sellPrice(gold: 6, silver: 50);
            public static int PillarWeaponValue => Item.sellPrice(gold: 10);
        }

        public static class Rarities
        {
            public const int CrabsonWeaponRare = ItemRarityID.Blue;
            public const int StariteWeaponRare = ItemRarityID.Green;
            public const int PetRare = ItemRarityID.Orange;
            public const int GoreNestRare = ItemRarityID.Orange;
            public const int OmegaStariteRare = ItemRarityID.LightRed;
            public const int GaleStreamsRare = ItemRarityID.LightRed;
            public const int PillarWeaponRare = ItemRarityID.Red;
        }

        public static void Energy_SetDefaults(Item item, int rarity, int price)
        {
            item.width = 24;
            item.height = 24;
            item.rare = rarity;
            item.value = price;
            item.maxStack = 999;
        }

        public static void Energy_DoUpdate(Item Item, Color clr, Vector3 lightClr)
        {
            int chance = 15;
            if (Item.velocity.Length() > 1f)
                chance = 5;
            if (Main.rand.NextBool(chance))
            {
                clr.A = 0;
                int d = Dust.NewDust(Item.position, Item.width, Item.height - 4, ModContent.DustType<EnergyPulse>(), 0f, 0f, 0, clr);
                Main.dust[d].alpha = Main.rand.Next(0, 35);
                Main.dust[d].scale = Main.rand.NextFloat(0.95f, 1.15f);
                if (Main.dust[d].scale > 1f)
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-3.5f, -1.75f));
            }
            Lighting.AddLight(Item.position, lightClr);
        }
    }
}