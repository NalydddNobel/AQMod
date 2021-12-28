using AQMod.Common;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities
{
    public class CelesitalEightBall : ModItem, IAutoloadType
    {
        public static string TextKey { get; private set; }
        public static object[] Arguments { get; private set; }

        public static string GetTextValue()
        {
            if (string.IsNullOrEmpty(TextKey))
                return Language.GetTextValue(AQText.Key + "Common.EightballAnswer20");
            if (Arguments != null)
            {
                return Language.GetTextValue(TextKey, Arguments);
            }
            return Language.GetTextValue(TextKey);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Yellow;
            item.useAnimation = 120;
            item.useTime = 120;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool AltFunctionUse(Player player) => true;

        public static void ResetStatics()
        {
            TextKey = null;
            Arguments = null;
        }

        private static void setText(string key)
        {
            TextKey = key;
            Arguments = null;
        }

        private static void setText(string key, object argument)
        {
            TextKey = key;
            Arguments = new object[] { argument };
        }

        private static void setText(string key, params object[] arguments)
        {
            TextKey = key;
            Arguments = arguments;
        }

        private static void spawnText(Player player, string key)
        {
            string text = Language.GetTextValue(key);
            int t = CombatText.NewText(player.getRect(), new Color(Main.mouseColor.R, Main.mouseColor.G, Main.mouseColor.B, 255), 0, true);
            Main.combatText[t].text = text;
            Main.combatText[t].position.X = player.Center.X - (Main.fontCombatText[1].MeasureString(text).X / 2f);
        }

        public override bool UseItem(Player player)
        {
            string key = "";
            if (player.altFunctionUse == 2)
            {
                key = AQText.Key + "Common.EightballAnswer" + Main.rand.Next(20);
                spawnText(player, key);
                if (Main.myPlayer == player.whoAmI)
                    setText(key);
            }
            else
            {
                if (player.frostArmor)
                {
                    key = AQText.Key + "Common.EightballMisc5";
                    spawnText(player, key);
                    if (Main.myPlayer == player.whoAmI)
                        setText(key);
                    return true;
                }
                if (OmegaStariteScenes.OmegaStariteIndexCache != -1 && Main.rand.NextBool())
                {
                    if (TextKey == AQText.Key + "Common.EightballMisc3")
                    {
                        key = AQText.Key + "Common.EightballMisc4";
                        spawnText(player, key);
                        if (Main.myPlayer == player.whoAmI)
                            setText(key);
                    }
                    else
                    {
                        key = AQText.Key + "Common.EightballMisc3";
                        spawnText(player, key);
                        if (Main.myPlayer == player.whoAmI)
                            setText(key);
                    }
                    return true;
                }
                if (GlimmerEvent.IsActive && Main.rand.NextBool())
                {
                    if ((int)(player.Center.X / 16) == GlimmerEvent.tileX)
                    {
                        key = AQText.Key + "Common.EightballMisc2";
                        spawnText(player, key);
                        if (Main.myPlayer == player.whoAmI)
                            setText(key);
                        return true;
                    }
                }
                if (Main.raining && Main.rand.NextBool())
                {
                    key = AQText.Key + "Common.EightballMisc0";
                    spawnText(player, key);
                    if (Main.myPlayer == player.whoAmI)
                        setText(key);
                    return true;
                }
                if (Main.rand.NextBool(3))
                {
                    int[] numbers = new int[]
                    {
                        Main.maxBackgrounds,
                        Main.maxBuffTypes,
                        Main.maxCharSelectHair,
                        Main.maxChests,
                        Main.maxClouds,
                        Main.maxCloudTypes,
                        Main.maxCombatText,
                        Main.maxDust,
                        Main.maxDustToDraw, // lol
                        Main.maxExtras,
                        Main.maxGlowMasks,
                        Main.maxGore,
                        Main.maxGoreTypes,
                        Main.maxHairTotal,
                        Main.maxInventory,
                        Main.maxItems,
                        Main.maxItemSounds,
                        Main.maxItemText,
                        Main.maxItemTypes,
                        Main.maxLiquidTypes,
                        Main.maxMusic,
                        Main.maxNPCHitSounds,
                        Main.maxNPCKilledSounds,
                        Main.maxNPCs,
                        Main.maxNPCTypes,
                        Main.maxNPCUpdates,
                        Main.maxPlayers,
                        Main.maxProjectiles,
                        Main.maxProjectileTypes,
                        Main.maxStars,
                        Main.maxStarTypes,
                        Main.maxTileSets,
                        Main.maxTilesX, // baht - estrelar
                        Main.maxTilesY, // :|
                        Main.maxWallTypes,
                        Main.maxWings,
                        Main.MaxBannerTypes,
                        Main.MaxTimeout,
                    };
                    key = AQText.Key + "Common.EightballMisc3";
                    int obj = numbers[Main.rand.Next(numbers.Length)];
                    string text = Language.GetTextValue(key, obj);
                    spawnText(player, text);
                    if (Main.myPlayer == player.whoAmI)
                        setText(key, obj);
                    return true;
                }
                int tileX = (int)player.Center.X / 16;
                if (tileX % 5 == 0 && Main.rand.NextBool())
                {
                    key = AQText.Key + "Common.EightballMisc6";
                    spawnText(player, key);
                    if (Main.myPlayer == player.whoAmI)
                        setText(key);
                    return true;
                }
                spawnText(player, AQText.Key + "Common.EightballAnswer20");
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "EightBall", GetTextValue()));
        }

        void IAutoloadType.OnLoad()
        {
            ResetStatics();
        }

        void IAutoloadType.Unload()
        {
            ResetStatics();
        }
    }
}