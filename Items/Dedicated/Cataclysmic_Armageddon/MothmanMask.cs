using AQMod.Assets.Enumerators;
using AQMod.Common;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated.Cataclysmic_Armageddon
{
    public class MothmanMask : ModItem, IUpdateEquipVisuals, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife >= player.statLifeMax2)
            {
                player.allDamageMult += 0.1f;
                player.meleeCrit += 10;
                player.magicCrit += 10;
                player.rangedCrit += 10;
                player.thrownCrit += 10;
                player.allDamage += 0.2f;
            }
            else
            {
                player.allDamageMult -= 0.1f;
                player.meleeCrit -= 10;
                player.magicCrit -= 10;
                player.rangedCrit -= 10;
                player.thrownCrit -= 10;
                player.allDamage -= 0.2f;
            }
        }

        Color IDedicatedItem.DedicatedItemColor() => DedicatedColors.Cataclysmic_Armageddon;

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQPlayer drawingPlayer, int i)
        {
            drawingPlayer.mask = (int)PlayerMaskID.CataMask;
            drawingPlayer.cMask = player.dye[i % AQPlayer.DYE_WRAP].dye;
            if (player.head == ArmorIDs.Head.ShadowHelmet &&
                player.body == ArmorIDs.Body.ShadowScalemail &&
                player.legs == ArmorIDs.Legs.ShadowGreaves)
            {
                drawingPlayer.cataEyeColor = new Color(75, 10, 150, 0);
            }
            else if (player.head == ArmorIDs.Head.AncientShadowHelmet &&
                player.body == ArmorIDs.Body.AncientShadowScalemail &&
                player.legs == ArmorIDs.Legs.AncientShadowGreaves)
            {
                drawingPlayer.cataEyeColor = new Color(90 + (int)(Math.Cos(Main.GlobalTime * 10f) * 30), 25, 140 - (int)(Math.Sin(Main.GlobalTime * 10f) * 30), 0);
            }
            else if (player.head == ArmorIDs.Head.MoltenHelmet &&
                player.body == ArmorIDs.Body.MoltenBreastplate &&
                player.legs == ArmorIDs.Legs.MoltenGreaves)
            {
                drawingPlayer.cataEyeColor = new Color(140 - (int)(Math.Sin(Main.GlobalTime * 10f) * 30), 90 + (int)(Math.Cos(Main.GlobalTime * 10f) * 30), 10, 0);
            }
        }
    }
}