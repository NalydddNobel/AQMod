using AQMod.Common.WorldEvents;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities
{
    public class CelesitalEightBall : ModItem
    {
        public static string Text { get; set; }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 120;
            item.useTime = 120;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool AltFunctionUse(Player player) => true;

        private static void spawnText(Player player, string text)
        {
            int t = CombatText.NewText(player.getRect(), new Color(Main.mouseColor.R, Main.mouseColor.G, Main.mouseColor.B, 255), 0, true);
            Main.combatText[t].text = text;
            Main.combatText[t].position.X = player.Center.X - Main.fontCombatText[1].MeasureString(text).X / 2f;
            Text = text;
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
                return true;
            if (player.altFunctionUse == 2)
            {
                spawnText(player, AQText.EightballAnswer(Main.rand.Next(20)).Value);
            }
            else
            {
                if (player.frostArmor)
                {
                    spawnText(player, AQText.EightballMisc(5).Value);
                    return true;
                }
                if (AQMod.OmegaStariteIndexCache != -1 && Main.rand.NextBool())
                {
                    if (Text == AQText.EightballMisc(3).Value)
                    {
                        spawnText(player, AQText.EightballMisc(4).Value);
                    }
                    else
                    {
                        spawnText(player, AQText.EightballMisc(3).Value);
                    }
                    return true;
                }
                if (GlimmerEvent.IsActive && Main.rand.NextBool())
                {
                    if ((int)(player.Center.X / 16) == GlimmerEvent.X)
                    {
                        spawnText(player, AQText.EightballMisc(2).Value);
                        return true;
                    }
                }
                if (Main.raining && Main.rand.NextBool())
                {
                    spawnText(player, AQText.EightballMisc(0).Value);
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
                    spawnText(player, string.Format(AQText.EightballMisc(3).Value, numbers[Main.rand.Next(numbers.Length)]));
                    return true;
                }
                int tileX = (int)player.Center.X / 16;
                if (tileX % 5 == 0 && Main.rand.NextBool())
                {
                    spawnText(player, AQText.EightballMisc(6).Value);
                    return true;
                }
                spawnText(player, AQText.EightballAnswer(20).Value);
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "EightBall", Text));
        }
    }
}