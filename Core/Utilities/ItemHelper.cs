using Aequus.Common.ItemModifiers.Cooldowns;
using Aequus.Common.Items.Components;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace Aequus;

public static class ItemHelper {
    public static int GetCooldownTime(this Item item, bool ignorePrefixes = false) {
        if (item.ModItem is not ICooldownItem cooldownItem) {
            return 0;
        }
        double cooldown = cooldownItem.CooldownTime;
        if (!ignorePrefixes && PrefixLoader.GetPrefix(item.prefix) is CooldownPrefixBase cooldownPrefix) {
            cooldown *= cooldownPrefix.CooldownMultiplier;
        }
        return (int)cooldown;
    }

    public static void Transform(this Item item, int newType) {
        int prefix = item.prefix;
        int stack = item.stack;
        item.SetDefaults(newType);
        item.Prefix(prefix);
        item.stack = stack;
    }

    public static void Transform<T>(this Item item) where T : ModItem {
        Transform(item, ModContent.ItemType<T>());
    }

    #region Static Defaults
    public static void StaticDefaultsToDrink(this ModItem modItem, params Color[] colors) {
        ItemID.Sets.IsFood[modItem.Type] = true;
        ItemID.Sets.DrinkParticleColors[modItem.Type] = colors;
        Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
    }

    public static void StaticDefaultsToFood(this ModItem modItem, params Color[] colors) {
        ItemID.Sets.IsFood[modItem.Type] = true;
        Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
    }
    #endregion

    #region Drawing 
    public static Vector2 WorldDrawPos(Item item, Texture2D texture) {
        return new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
    }

    public static void GetItemDrawData(int item, out Rectangle frame) {
        frame = Main.itemAnimations[item] == null ? TextureAssets.Item[item].Value.Frame() : Main.itemAnimations[item].GetFrame(TextureAssets.Item[item].Value);
    }
    public static void GetItemDrawData(this Item item, out Rectangle frame) {
        GetItemDrawData(item.type, out frame);
    }
    #endregion

    #region Tooltips
    internal static readonly string[] VanillaTooltipNames = new[] {
        "ItemName",
        "Favorite", "FavoriteDesc",
        "Social", "SocialDesc",
        "Damage", "CritChance", "Speed", "Knockback",
        "FishingPower", "NeedsBait", "BaitPower",
        "Equipable",
        "WandConsumes",
        "Quest",
        "Vanity",
        "Defense",
        "PickPower", "AxePower", "HammerPower",
        "TileBoost",
        "HealLife", "HealMana", "UseMana",
        "Placeable",
        "Ammo",
        "Consumable",
        "Material",
        "Tooltip#",
        "EtherianManaWarning",
        "WellFedExpert", "BuffTime",
        "OneDropLogo",
        "PrefixDamage", "PrefixSpeed", "PrefixCritChance", "PrefixUseMana", "PrefixSize", "PrefixShootSpeed", "PrefixKnockback",
        "PrefixAccDefense", "PrefixAccMaxMana", "PrefixAccCritChance", "PrefixAccDamage", "PrefixAccMoveSpeed", "PrefixAccMeleeSpeed",
        "SetBonus",
        "Expert", "Master",
        "JourneyResearch",
        "BestiaryNotes",
        "SpecialPrice", "Price",
    };

    private static int FindLineIndex(string name) {
        if (string.IsNullOrEmpty(name)) {
            return 0;
        }

        if (name.StartsWith("Tooltip")) {
            name = "Tooltip#";
        }
        for (int i = 0; i < VanillaTooltipNames.Length; i++) {
            if (name == VanillaTooltipNames[i]) {
                return i;
            }
        }
        return -1;
    }

    public static void AddTooltip(this List<TooltipLine> tooltips, TooltipLine line) {
        tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#"), tooltips.Count), line);
    }

    public static int GetIndex(this List<TooltipLine> tooltips, string lineName, int resultOffset = 0) {
        int myIndex = FindLineIndex(lineName);
        int i = 0;
        for (; i < tooltips.Count; i++) {
            if (tooltips[i].Mod == "Terraria") {
                int index = FindLineIndex(tooltips[i].Name);
                if (index >= myIndex) {
                    if (index != myIndex) {
                        if (resultOffset > 0) {
                            resultOffset--;
                        }
                    }
                    if (lineName == "Tooltip#") {
                        for (; i < tooltips.Count; i++) {
                            if (!tooltips[i].Name.StartsWith("Tooltip")) {
                                goto ReturnClamp;
                            }
                        }
                    }
                    goto ReturnClamp;
                }
            }
        }
    ReturnClamp:
        return Math.Clamp(i + resultOffset, 0, tooltips.Count);
    }

    public static TooltipLine Find(this List<TooltipLine> tooltips, string name) {
        return tooltips.Find((t) => t != null && t.Mod != null && t.Name != null && t.Mod == "Terraria" && t.Name.Equals(name));
    }

    public static TooltipLine ItemName(this List<TooltipLine> tooltips) {
        return tooltips.Find("ItemName");
    }

    public static void RemoveKnockback(this List<TooltipLine> tooltips) {
        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Knockback");
    }

    public static void RemoveCritChance(this List<TooltipLine> tooltips) {
        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "CritChance");
    }

    public static void RemoveCritChanceModifier(this List<TooltipLine> tooltips) {
        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "PrefixCritChance");
    }
    #endregion

    #region Item Lists
    public static int GetNextIndex(this Item[] itemList) {
        int i = 0;
        for (; i < itemList.Length; i++) {
            if (itemList[i] == null) {
                break;
            }
        }
        return i;
    }
    #endregion

    #region Recipes
    public static Item FindIngredient(this Recipe recipe, int itemID) {
        return recipe.requiredItem.Find((item) => item != null && !item.IsAir && item.type == itemID);
    }
    public static bool TryFindIngredient(this Recipe recipe, int itemID, out Item result) {
        result = recipe.FindIngredient(itemID);
        return result != null;
    }

    public static Recipe ReplaceItem(this Recipe r, int item, int newItem, int newItemStack = -1) {
        for (int i = 0; i < r.requiredItem.Count; i++) {
            if (r.requiredItem[i].type == item) {
                int stack = newItemStack <= 0 ? r.requiredItem[i].stack : newItemStack;
                r.requiredItem[i].SetDefaults(newItem);
                r.requiredItem[i].stack = stack;
                break;
            }
        }
        return r;
    }
    public static void ReplaceItemWith(this Recipe r, int item, Action<Recipe, Item> replacementMethod) {
        var itemList = new List<Item>(r.requiredItem);
        r.requiredItem.Clear();
        for (int i = 0; i < itemList.Count; i++) {
            if (itemList[i].type == item) {
                replacementMethod(r, itemList[i]);
            }
            else {
                r.AddIngredient(itemList[i].type, itemList[i].stack);
            }
        }
    }
    #endregion

    public static void LazyCustomSwordDefaults<T>(this Item item, int swingTime) where T : ModProjectile {
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
}