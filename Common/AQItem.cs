using AQMod.Common.WorldGeneration;
using AQMod.Content.Dusts;
using AQMod.Items.Accessories;
using AQMod.Items.BuffItems;
using AQMod.Items.BuffItems.Foods;
using AQMod.Items.Tools.Rods;
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
        public static int CorruptionWeaponValue => Item.sellPrice(silver: 50);
        public static int CrimsonWeaponValue => Item.sellPrice(silver: 55);
        public static int GlimmerWeaponValue => Item.sellPrice(silver: 80);
        public static int PillarWeaponValue => Item.sellPrice(gold: 10);
        public static int CrabsonWeaponValue => Item.sellPrice(silver: 25);
        public static int DemonSiegeWeaponValue => Item.sellPrice(silver: 80);
        public static int EnergyBuyValue => Item.sellPrice(gold: 2);
        public static int EnergySellValue => Item.sellPrice(silver: 5);
        public static int PotionValue => Item.sellPrice(silver: 2);
        public static int EnergyWeaponValue => Item.sellPrice(gold: 5, silver: 50);
        public static int OmegaStariteWeaponValue => Item.sellPrice(gold: 2);
        public static int AtmosphericCurrentsValue => Item.sellPrice(gold: 4);

        public static Color GetAlphaDemonSiegeWeapon(Color lightColor)
        {
            return new Color(lightColor.R * lightColor.R, lightColor.G * lightColor.G, lightColor.B * lightColor.B, lightColor.A);
        }

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

        public static bool ItemOnGroundAlready(int type)
        {
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i].active && Main.item[i].type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.modItem is ISpecialFood specialFood)
            {
                if (AQPlayer.IsQuickBuffing && AQPlayer.HasFoodBuff(player.whoAmI))
                {
                    return false;
                }
                if (base.CanUseItem(item, player))
                {
                    item.buffType = 0;
                    player.AddBuff(specialFood.ChangeBuff(player), item.buffTime);
                    player.ConsumeItem(item.type);
                    if (item.UseSound != null)
                        Main.PlaySound(item.UseSound, player.Center);
                    if (!AQPlayer.IsQuickBuffing)
                        return true;
                }
                return false;
            }
            return base.CanUseItem(item, player);
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
            }
        }

        public override float UseTimeMultiplier(Item item, Player player)
        {
            if (item.type == ItemID.Starfury && player.GetModPlayer<AQPlayer>().moonShoes)
                return 1.5f;
            return 1f;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (item.type < Main.maxItemTypes)
                return true;
            if (!AQMod.ItemOverlays.GetOverlay(item.type)?.PreDrawWorld(item, lightColor, alphaColor, ref rotation, ref scale, whoAmI) == false)
            {
                return false;
            }
            return true;
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (item.type < Main.maxItemTypes)
                return;
            AQMod.ItemOverlays.GetOverlay(item.type)?.PostDrawWorld(item, lightColor, alphaColor, rotation, scale, whoAmI);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type < Main.maxItemTypes)
                return true;
            if (!AQMod.ItemOverlays.GetOverlay(item.type)?.PreDrawInventory(Main.LocalPlayer, Main.LocalPlayer.GetModPlayer<AQPlayer>(), item, position, frame, drawColor, itemColor, origin, scale) == false)
            {
                return false;
            }
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type < Main.maxItemTypes)
                return;
            AQMod.ItemOverlays.GetOverlay(item.type)?.PostDrawInventory(Main.LocalPlayer, Main.LocalPlayer.GetModPlayer<AQPlayer>(), item, position, frame, drawColor, itemColor, origin, scale);
        }

        internal static Vector2 getItemDrawPos_NoAnimation(Item item)
        {
            return new Vector2(item.position.X - Main.screenPosition.X + Main.itemTexture[item.type].Width / 2 + item.width / 2 - Main.itemTexture[item.type].Width / 2, item.position.Y - Main.screenPosition.Y + Main.itemTexture[item.type].Height / 2 + item.height - Main.itemTexture[item.type].Height + 2f);
        }

        internal static void energy_DoUpdate(Item Item, Color clr, Vector3 lightClr)
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

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            grabRange = (int)(grabRange * player.GetModPlayer<AQPlayer>().grabReachMult);
        }

        public static bool MirrorItemUseCheck(Player player)
        {
            if (player.position.Y - 500f > Main.worldSurface * 16f)
            {
                return false;
            }
            int tileX = (int)(player.position.X + player.width / 2f) / 16;
            int tileY = (int)(player.position.Y + player.height / 2f) / 16;
            if (Main.tile[tileX, tileY] == null)
            {
                Main.tile[tileX, tileY] = new Tile();
            }
            if (AQWorldGen.TileObstructedFromLight(tileX, tileY))
            {
                return false;
            }
            for (int i = 0; i < 15; i++)
            {
                if (tileY - 1 - i <= 0)
                {
                    break;
                }
                if (Main.tile[tileX, tileY - 1 - i] == null)
                {
                    Main.tile[tileX, tileY - 1 - i] = new Tile();
                }
                if (AQWorldGen.TileObstructedFromLight(tileX, tileY - 1 - i))
                {
                    return false;
                }
            }
            return true;
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

        internal static void energy_SetDefaults(Item item, int rarity, int price)
        {
            item.width = 24;
            item.height = 24;
            item.rare = rarity;
            item.value = price;
            item.maxStack = 999;
        }
    }
}