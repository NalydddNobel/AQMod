using Aequus.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items {
    public static class ItemDefaults {
        public const int RarityCrabCrevice = ItemRarityID.Blue;
        public const int RarityGlimmer = ItemRarityID.Green;
        public const int RarityDemonSiege = ItemRarityID.Orange;
        public const int RarityUltraStarite = ItemRarityID.Orange;
        public const int RarityOmegaStarite = ItemRarityID.LightRed;
        public const int RarityGaleStreams = ItemRarityID.LightRed;
        public const int RarityDustDevil = ItemRarityID.Pink;

        public const int RarityBanner = ItemRarityID.Blue;
        public const int RarityBossMasks = ItemRarityID.Blue;
        public const int RarityDemoniteCrimtane = ItemRarityID.Blue;
        public const int RarityDungeon = ItemRarityID.Green;
        public const int RarityQueenBee = ItemRarityID.Orange;
        public const int RarityJungle = ItemRarityID.Orange;
        public const int RarityMolten = ItemRarityID.Orange;
        public const int RarityPet = ItemRarityID.Orange;
        public const int RarityWallofFlesh = ItemRarityID.LightRed;
        public const int RarityEarlyHardmode = ItemRarityID.LightRed;
        public const int RarityPreMechs = ItemRarityID.LightRed;
        public const int RarityCobaltMythrilAdamantite = ItemRarityID.LightRed;
        public const int RarityMechs = ItemRarityID.Pink;
        public const int RarityPlantera = ItemRarityID.Lime;
        public const int RarityHardmodeDungeon = ItemRarityID.Yellow;
        public const int RarityTemple = ItemRarityID.Yellow;
        public const int RarityMartians = ItemRarityID.Yellow;
        public const int RarityDukeFishron = ItemRarityID.Yellow;
        public const int RarityLunaticCultist = ItemRarityID.Cyan;
        public const int RarityPillars = ItemRarityID.Red;
        public const int RarityMoonLord = ItemRarityID.Red;

        /// <summary>
        /// 50 silver
        /// </summary>
        public static int ValueCrabCrevice => Item.sellPrice(silver: 50);
        /// <summary>
        /// 1 gold
        /// </summary>
        public static int ValueGlimmer => Item.sellPrice(gold: 1);
        /// <summary>
        /// 2 gold
        /// </summary>
        public static int ValueDemonSiege => Item.sellPrice(gold: 2);
        /// <summary>
        /// 3 gold (<see cref="ValueEarlyHardmode"/>)
        /// </summary>
        public static int ValueOmegaStarite => ValueEarlyHardmode;
        /// <summary>
        /// 2 gold 50 silver
        /// </summary>
        public static int ValueGaleStreams => Item.sellPrice(gold: 2, silver: 50);
        /// <summary>
        /// 3 gold (<see cref="ValueEarlyHardmode"/>)
        /// </summary>
        public static int ValueDustDevil => ValueEarlyHardmode;

        /// <summary>
        /// 2 silver
        /// </summary>
        public static int ValueBuffPotion => Item.sellPrice(silver: 2);
        /// <summary>
        /// 50 silver
        /// </summary>
        public static int ValueEyeOfCthulhu => Item.sellPrice(silver: 50);
        /// <summary>
        /// 50 silver
        /// </summary>
        public static int ValueBloodMoon => Item.sellPrice(silver: 50);
        /// <summary>
        /// 1 gold 75 silver
        /// </summary>
        public static int ValueDungeon => Item.sellPrice(gold: 1, silver: 75);
        /// <summary>
        /// 3 gold
        /// </summary>
        public static int ValueEarlyHardmode => Item.sellPrice(gold: 3);
        /// <summary>
        /// 6 gold
        /// </summary>
        public static int ValueHardmodeDungeon => Item.sellPrice(gold: 6);
        /// <summary>
        /// 10 gold
        /// </summary>
        public static int ValueLunarPillars => Item.sellPrice(gold: 10);

        public static void SetGlowMask(this Item item) {
            item.glowMask = GlowMasksHandler.GetID(item.type);
        }

        public static void DefaultToCursorDye(this Item item) {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.DrinkLiquid;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.maxStack = Item.CommonMaxStack;
        }

        public static void DefaultToNecromancy(this Item item, int timeBetweenShots) {
            item.useTime = timeBetweenShots;
            item.useAnimation = timeBetweenShots;
            item.useStyle = ItemUseStyleID.Shoot;
            item.DamageType = Aequus.NecromancyClass;
            item.noMelee = true;
        }

        public static void DefaultToHoldUpItem(this Item item) {
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldUp;
        }

        public static void FixSwing(this Item item, Player player) {
            //Main.NewText(player.itemTime);
            //Main.NewText(player.toolTime, Color.Orange);
            //Main.NewText(player.itemAnimation + "|" + player.itemAnimationMax, Color.Beige);
            if (item.pick > 0 || item.axe > 0 || item.hammer > 0) {
                if ((player.toolTime > 0 && player.itemTime == 0) || !player.controlUseItem)
                    return;
                player.itemAnimation = Math.Min(player.itemAnimation, player.toolTime);
            }
            player.itemAnimation = player.itemAnimationMax;
        }

        public static void DefaultToAequusSword<T>(this Item item, int swingTime) where T : ModProjectile {
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

        public static void StaticDefaultsToDrink(this ModItem modItem, params Color[] colors) {
            ItemID.Sets.IsFood[modItem.Type] = true;
            ItemID.Sets.DrinkParticleColors[modItem.Type] = colors;
            Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
        }

        public static void StaticDefaultsToFood(this ModItem modItem, params Color[] colors) {
            ItemID.Sets.IsFood[modItem.Type] = true;
            Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
        }

        public static Vector2 WorldDrawPos(Item item, Texture2D texture) {
            return new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
        }
    }
}