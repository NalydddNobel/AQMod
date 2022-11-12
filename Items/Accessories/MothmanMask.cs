using Aequus.Items.GlobalItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class MothmanMask : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            TooltipsGlobalItem.Dedicated.Add(Type, new TooltipsGlobalItem.ItemDedication(new Color(50, 75, 250, 255)));
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = Item.sellPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accMothmanMask = Item;
            if (player.statLife >= player.statLifeMax2)
            {
                player.GetDamage(DamageClass.Generic) += 0.15f;
                player.GetCritChance(DamageClass.Generic) += 15;
                player.GetKnockback(DamageClass.Summon) += 1f;
            }
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedMask = Type;
                player.Aequus().cMask = dyeItem.dye;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"))
                {
                    t.Text = AequusHelpers.FormatWith(t.Text, new
                    {
                        Color = Colors.AlphaDarken(Color.Lerp(Color.Red, Color.White, 0.5f)).Hex3(),
                    });
                }
            }
        }

        //void IUpdateVanity.UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        //{
        //    if (player.head == ArmorIDs.Head.ShadowHelmet &&
        //        player.body == ArmorIDs.Body.ShadowScalemail &&
        //        player.legs == ArmorIDs.Legs.ShadowGreaves)
        //    {
        //        drawEffects.MothmanMaskEyeColor = new Color(75, 10, 150, 0);
        //    }
        //    else if (player.head == ArmorIDs.Head.AncientShadowHelmet &&
        //        player.body == ArmorIDs.Body.AncientShadowScalemail &&
        //        player.legs == ArmorIDs.Legs.AncientShadowGreaves)
        //    {
        //        drawEffects.MothmanMaskEyeColor = PlayerDrawEffects.MothmanMaskEyeColorShadowScale;
        //    }
        //    else if (player.head == ArmorIDs.Head.MoltenHelmet &&
        //        player.body == ArmorIDs.Body.MoltenBreastplate &&
        //        player.legs == ArmorIDs.Legs.MoltenGreaves)
        //    {
        //        drawEffects.MothmanMaskEyeColor = new Color(140 - (int)(Math.Sin(Main.GlobalTime * 10f) * 30), 90 + (int)(Math.Cos(Main.GlobalTime * 10f) * 30), 10, 0);
        //    }
        //}
    }
}