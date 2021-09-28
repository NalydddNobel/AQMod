using AQMod.Assets.ItemOverlays;
using AQMod.Items.Accessories;
using AQMod.Items.BuffItems;
using AQMod.Items.BuffItems.Foods;
using AQMod.Items.BuffItems.Staffs;
using AQMod.Items.Fishing.Rods;
using AQMod.Items.Misc.Markers;
using AQMod.Items.Weapons.Magic.Support;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQItem : GlobalItem
    {
        public static int GlimmerWeaponValue => Item.sellPrice(silver: 80);
        public static int PillarWeaponValue => Item.sellPrice(gold: 10);
        public static int CrabsonWeaponValue => Item.sellPrice(silver: 25);
        public static int DemonSiegeWeaponValue => Item.sellPrice(silver: 80);
        public static int EnergyBuyValue => Item.sellPrice(gold: 2);
        public static int EnergySellValue => Item.sellPrice(silver: 5);
        public static int PotionValue => Item.sellPrice(silver: 2);
        public static int EnergyWeaponValue => Item.sellPrice(gold: 4, silver: 50);
        public static int OmegaStariteWeaponValue => Item.sellPrice(gold: 4);

        public static class Sets
        {
            public static bool[] CantBeTurnedIntoMolitePotion { get; private set; }

            internal static void Setup()
            {
                CantBeTurnedIntoMolitePotion = new bool[ItemLoader.ItemCount];
                CantBeTurnedIntoMolitePotion[ModContent.ItemType<SpicyEel>()] = true;
            }

            internal static void Unload()
            {
                CantBeTurnedIntoMolitePotion = null;
            }
        }

        internal const string CommonTag_DedicatedItem = "DedicatedItem";
        internal const string CommonTag_IchorDartShotgun = "IchorDartShotgun";

        public static bool ItemOnGroundAlready(int type)
        {
            for (int i= 0; i < Main.maxItems; i++)
            {
                if (Main.item[i].active && Main.item[i].type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            if (player.GetModPlayer<AQPlayer>().spicyEel)
            {
                speed *= 1.1f;
                acceleration *= 1.1f;
            }
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            if (player.GetModPlayer<AQPlayer>().spicyEel)
            {
                ascentWhenFalling *= 1.1f;
                ascentWhenRising *= 1.1f;
                maxCanAscendMultiplier *= 1.1f;
                maxAscentMultiplier *= 1.1f;
                constantAscend *= 1.1f;
            }
        }

        public override void ExtractinatorUse(int extractType, ref int resultType, ref int resultStack)
        {
            var player = Main.LocalPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.ExtractinatorCount++;
            if (aQPlayer.extractinatorVisible)
                CombatText.NewText(new Rectangle(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16), Color.Gray, aQPlayer.ExtractinatorCount);
            if (aQPlayer.ExtractinatorCount % 500 == 0)
                Item.NewItem(new Rectangle(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16), ModContent.ItemType<Extractor>());
            if (aQPlayer.extractinator)
            {
                if (Main.rand.NextBool())
                    resultStack *= 2;
            }
        }

        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            switch (context)
            {
                case "bossBag":
                {
                    switch (arg)
                    {
                        case ItemID.QueenBeeBossBag:
                        {
                            if (Main.rand.NextBool())
                                player.QuickSpawnItem(ModContent.ItemType<BeeRod>());
                        }
                        break;

                        case ItemID.WallOfFleshBossBag:
                        {
                            if (Main.hardMode)
                                player.QuickSpawnItem(ModContent.ItemType<OpposingPotion>(), Main.rand.Next(4) + 1);
                        }
                        break;
                    }
                }
                break;

                case "crate":
                if (arg == ItemID.WoodenCrate)
                {
                    if (Main.rand.NextBool(5))
                    {
                        int[] choices = new int[]
                        {
                            ModContent.ItemType<StaffofNightVision>(),
                            ModContent.ItemType<StaffofRegeneration>(),
                            ModContent.ItemType<StaffofWaterBreathing>(),
                            ModContent.ItemType<StaffofSwiftness>(),
                            ModContent.ItemType<StaffofIronskin>(),
                        };
                        player.QuickSpawnItem(choices[Main.rand.Next(choices.Length)]);
                    }
                }
                break;
            }
        }

        public override float UseTimeMultiplier(Item item, Player player)
        {
            if (item.type == ItemID.Starfury && player.GetModPlayer<AQPlayer>().moonShoes)
                return 1.5f;
            return 1f;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
        {
            if (item.type == ItemID.Starfury && player.GetModPlayer<AQPlayer>().moonShoes)
                mult += 0.1f;
        }

        internal static int FindVanillaTooltipLineIndex(List<TooltipLine> tooltips, string tooltipName)
        {
            switch (tooltipName)
            {
                case "Damage":
                {
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            return i + 1;
                        }
                    }
                }
                break;

                case "Tooltip#":
                for (int i = tooltips.Count - 1; i >= 0; i--)
                {
                    TooltipLine t = tooltips[i];
                    if (t.mod != "Terraria")
                        continue;
                    if (t.Name.StartsWith("Tooltip"))
                        return i;
                    switch (t.Name)
                    {
                        case "Favorite":
                        case "FavoriteDesc":
                        case "Social":
                        case "SocialDesc":
                        case "Damage":
                        case "CritChance":
                        case "Speed":
                        case "Knockback":
                        case "FishingPower":
                        case "NeedsBait":
                        case "BaitPower":
                        case "Equipable":
                        case "WandConsumes":
                        case "Quest":
                        case "Vanity":
                        case "Defense":
                        case "PickPower":
                        case "AxePower":
                        case "HammerPower":
                        case "TileBoost":
                        case "HealLife":
                        case "HealMana":
                        case "UseMana":
                        case "Placeable":
                        case "Ammo":
                        case "Consumable":
                        case "Material":
                        return i + 1;
                    }
                }
                break;

                case "Equipable":
                for (int i = tooltips.Count - 1; i >= 0; i--)
                {
                    TooltipLine t = tooltips[i];
                    if (t.mod != "Terraria")
                        continue;
                    switch (t.Name)
                    {
                        case "Favorite":
                        case "FavoriteDesc":
                        case "Social":
                        case "SocialDesc":
                        case "Damage":
                        case "CritChance":
                        case "Speed":
                        case "Knockback":
                        case "FishingPower":
                        case "NeedsBait":
                        case "BaitPower":
                        case "Equipable":
                        case "Tooltip0":
                        return i + 1;
                    }
                }
                break;

                case "Expert":
                for (int i = tooltips.Count - 1; i >= 0; i--)
                {
                    TooltipLine t = tooltips[i];
                    if (t.mod != "Terraria")
                        continue;
                    if (t.Name.StartsWith("Tooltip"))
                        return i + 1;
                    switch (t.Name)
                    {
                        case "Favorite":
                        case "FavoriteDesc":
                        case "Social":
                        case "SocialDesc":
                        case "Damage":
                        case "CritChance":
                        case "Speed":
                        case "Knockback":
                        case "FishingPower":
                        case "NeedsBait":
                        case "BaitPower":
                        case "Equipable":
                        case "WandConsumes":
                        case "Quest":
                        case "Vanity":
                        case "Defense":
                        case "PickPower":
                        case "AxePower":
                        case "HammerPower":
                        case "TileBoost":
                        case "HealLife":
                        case "HealMana":
                        case "UseMana":
                        case "Placeable":
                        case "Ammo":
                        case "Consumable":
                        case "Material":
                        case "EtherianManaWarning":
                        case "WellFedExpert":
                        case "BuffTime":
                        case "OneDropLogo":
                        case "PrefixDamage":
                        case "PrefixSpeed":
                        case "PrefixCritChance":
                        case "PrefixUseMana":
                        case "PrefixSize":
                        case "PrefixShootSpeed":
                        case "PrefixKnockback":
                        case "PrefixAccDefense":
                        case "PrefixAccMaxMana":
                        case "PrefixAccCritChance":
                        case "PrefixAccDamage":
                        case "PrefixAccMoveSpeed":
                        case "PrefixAccMeleeSpeed":
                        case "SetBonus":
                        case "Expert":
                        return i + 1;
                    }
                }
                break;
            }
            return 1;
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (item.type < Main.maxItemTypes)
                return;
            ItemOverlayLoader.GetOverlay(item.type)?.DrawWorld(item, lightColor, alphaColor, rotation, scale, whoAmI);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type < Main.maxItemTypes)
                return true;
            if (!ItemOverlayLoader.GetOverlay(item.type)?.PreDrawInventory(Main.LocalPlayer, Main.LocalPlayer.GetModPlayer<AQPlayer>(), item, position, frame, drawColor, itemColor, origin, scale) == false)
            {
                return false;
            }
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type < Main.maxItemTypes)
                return;
            ItemOverlayLoader.GetOverlay(item.type)?.PostDrawInventory(Main.LocalPlayer, Main.LocalPlayer.GetModPlayer<AQPlayer>(), item, position, frame, drawColor, itemColor, origin, scale);
        }

        internal static Vector2 getItemDrawPos_NoAnimation(Item item)
        {
            return new Vector2(item.position.X - Main.screenPosition.X + Main.itemTexture[item.type].Width / 2 + item.width / 2 - Main.itemTexture[item.type].Width / 2, item.position.Y - Main.screenPosition.Y + Main.itemTexture[item.type].Height / 2 + item.height - Main.itemTexture[item.type].Height + 2f);
        }

        internal static void AddCommonTooltipLine(List<TooltipLine> tooltips, string name, Color? color = null)
        {
            const string a = "Common.";
            const string b = AQText.Key + a;
            tooltips.Add(new TooltipLine(ModContent.GetInstance<AQMod>(), name, Language.GetTextValue(b + name + "Tag")) { overrideColor = color });
        }
    }
}