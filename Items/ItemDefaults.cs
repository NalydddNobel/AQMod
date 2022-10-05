using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public static class ItemDefaults
    {
        public const int RarityCrabCrevice = ItemRarityID.Blue;
        public const int RarityGlimmer = ItemRarityID.Green;
        public const int RarityDemonSiege = ItemRarityID.Orange;
        public const int RarityOmegaStarite = ItemRarityID.LightRed;
        public const int RarityGaleStreams = ItemRarityID.LightRed;
        public const int RarityDustDevil = ItemRarityID.Pink;

        public const int RarityBanner = ItemRarityID.Blue;
        public const int RarityBossMasks = ItemRarityID.Blue;
        public const int RarityDemoniteCrimtane = ItemRarityID.Blue;
        public const int RarityDungeon = ItemRarityID.Green;
        public const int RarityQueenBee = ItemRarityID.Green;
        public const int RarityJungle = ItemRarityID.Orange;
        public const int RarityMolten = ItemRarityID.Orange;
        public const int RarityPet = ItemRarityID.Orange;
        public const int RarityWallofFlesh = ItemRarityID.LightRed;
        public const int RarityPreMechs = ItemRarityID.LightRed;
        public const int RarityCobaltMythrilAdamantite = ItemRarityID.LightRed;
        public const int RarityMechs = ItemRarityID.Pink;
        public const int RarityPlantera = ItemRarityID.Lime;
        public const int RarityHardmodeDungeon = ItemRarityID.Yellow;
        public const int RarityMartians = ItemRarityID.Yellow;
        public const int RarityDukeFishron = ItemRarityID.Yellow;
        public const int RarityLunaticCultist = ItemRarityID.Cyan;
        public const int RarityPillars = ItemRarityID.Red;
        public const int RarityMoonLord = ItemRarityID.Red;

        public static int CrabCreviceValue => Item.sellPrice(silver: 25);
        public static int GlimmerValue => Item.sellPrice(silver: 75);
        public static int DemonSiegeValue => Item.sellPrice(gold: 2, silver: 50);
        public static int MemorialistItemBuyValue => Item.buyPrice(gold: 20);
        public static int OmegaStariteValue => Item.sellPrice(gold: 4, silver: 50);
        public static int GaleStreamsValue => Item.sellPrice(gold: 4);
        public static int DustDevilValue => Item.sellPrice(gold: 5);

        public static int PotionsValue => Item.sellPrice(silver: 2);
        public static int CorruptionValue => Item.sellPrice(silver: 50);
        public static int CrimsonValue => Item.sellPrice(silver: 55);
        public static int BloodMimicItemValue => Item.sellPrice(gold: 1);
        public static int DungeonValue => Item.sellPrice(gold: 1, silver: 75);
        public static int PostMechsEnergyWeaponValue => Item.sellPrice(gold: 6, silver: 50);
        public static int PillarWeaponValue => Item.sellPrice(gold: 10);

        public static void SetGlowMask(this Item item)
        {
            item.glowMask = AequusGlowMasks.GetID(item.type);
        }

        public static void DefaultToCursorDye(this Item item)
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.DrinkLiquid;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.maxStack = 9999;
        }

        public static void DefaultToNecromancy(this Item item, int timeBetweenShots)
        {
            item.useTime = timeBetweenShots;
            item.useAnimation = timeBetweenShots;
            item.useStyle = ItemUseStyleID.Shoot;
            item.DamageType = NecromancyDamageClass.Instance;
            item.noMelee = true;
        }

        public static void DefaultToHoldUpItem(this Item item)
        {
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldUp;
        }

        public static void DefaultToDopeSword<T>(this Item item, int swingTime) where T : ModProjectile
        {
            item.useTime = swingTime;
            item.useAnimation = swingTime;
            item.shoot = ModContent.ProjectileType<T>();
            item.shootSpeed = 1f;
            item.DamageType = DamageClass.Melee;
            item.useStyle = ItemUseStyleID.Shoot;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
        }

        public static void StaticDefaultsToDrink(this ModItem modItem, params Color[] colors)
        {
            ItemID.Sets.IsFood[modItem.Type] = true;
            ItemID.Sets.DrinkParticleColors[modItem.Type] = colors;
            Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
        }

        public static void StaticDefaultsToFood(this ModItem modItem, params Color[] colors)
        {
            ItemID.Sets.IsFood[modItem.Type] = true;
            ItemID.Sets.FoodParticleColors[modItem.Type] = colors;
            Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
        }

        public static Vector2 WorldDrawPos(Item item, Texture2D texture)
        {
            return new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
        }
    }
}