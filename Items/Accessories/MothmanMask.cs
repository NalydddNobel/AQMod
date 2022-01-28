using AQMod.Content.DedicatedItemTags;
using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class MothmanMask : ModItem, IDedicatedItem, IUpdateEquipVisuals
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = AQItem.Rarities.DedicatedItem;
            item.value = Item.sellPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.mothmanMask = true;
            if (player.statLife >= player.statLifeMax2)
            {
                player.allDamage += 0.15f;
                player.meleeCrit += 15;
                player.rangedCrit += 15;
                player.magicCrit += 15;
                player.thrownCrit += 15;
                player.minionKB += 1f;
            }
        }

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            drawEffects.mask = PlayerMaskID.CataMask;
            drawEffects.cMask = player.dye[i % AQPlayer.MaxDye].dye;
            if (player.head == ArmorIDs.Head.ShadowHelmet &&
                player.body == ArmorIDs.Body.ShadowScalemail &&
                player.legs == ArmorIDs.Legs.ShadowGreaves)
            {
                drawEffects.MothmanMaskEyeColor = new Color(75, 10, 150, 0);
            }
            else if (player.head == ArmorIDs.Head.AncientShadowHelmet &&
                player.body == ArmorIDs.Body.AncientShadowScalemail &&
                player.legs == ArmorIDs.Legs.AncientShadowGreaves)
            {
                drawEffects.MothmanMaskEyeColor = PlayerDrawEffects.MothmanMaskEyeColorShadowScale;
            }
            else if (player.head == ArmorIDs.Head.MoltenHelmet &&
                player.body == ArmorIDs.Body.MoltenBreastplate &&
                player.legs == ArmorIDs.Legs.MoltenGreaves)
            {
                drawEffects.MothmanMaskEyeColor = new Color(140 - (int)(Math.Sin(Main.GlobalTime * 10f) * 30), 90 + (int)(Math.Cos(Main.GlobalTime * 10f) * 30), 10, 0);
            }
        }

        Color IDedicatedItem.DedicatedItemColor => Dedication.YoutuberColor;
        IDedicationType IDedicatedItem.DedicationType => new Dedication();
    }
}