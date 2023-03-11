using Aequus.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Consumables.Permanent
{
    public class MoneyTrashcan : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.expert = true;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = 9999;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return player.altFunctionUse != 2;
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.Aequus();
            if (player.altFunctionUse == 2)
            {
                aequus.usedPermaTrashMoney = false;
                return true;
            }
            if (!aequus.usedPermaTrashMoney)
            {
                aequus.usedPermaTrashMoney = true;
                return true;
            }

            return false;
        }
    }
}

namespace Aequus
{
    public partial class AequusPlayer
    {
        private static Asset<Texture2D> TrashCanTexture;

        [SaveData("CrabsonTrashcan")]
        public bool usedPermaTrashMoney;
        public float trashMoney;

        public Item lastTrashItem;

        public double GetTrashPrice(int itemType, int itemStack)
        {
            double multiplier = Math.Clamp(trashMoney, 0.0, 1.0) / 5.0;
            long value = ContentSamples.ItemsByType[itemType].value;
            var price = value * itemStack * multiplier;
            return price;
        }
        public double GetTrashPrice(Item item)
        {
            return GetTrashPrice(item.netID, item.stack);
        }

        public void ResetEffects_TrashMoney()
        {
            trashMoney = usedPermaTrashMoney ? 0.25f : 0f;
            if (Player.whoAmI == Main.myPlayer)
            {
                if (usedPermaTrashMoney)
                {
                    TextureAssets.Trash = AequusTextures.MoneyTrashcan_UI;
                }
                else
                {
                    TextureAssets.Trash = TrashCanTexture;
                }
            }
        }

        public void Load_TrashMoney()
        {
            ItemSlot.OnItemTransferred += ItemSlot_OnItemTransferred;
            On.Terraria.UI.ItemSlot.OverrideLeftClick += ItemSlot_OverrideLeftClick;
            TrashCanTexture = TextureAssets.Trash;
        }

        public void Unload_TrashMoney()
        {
            ItemSlot.OnItemTransferred -= ItemSlot_OnItemTransferred;
            if (TrashCanTexture != null)
            {
                TextureAssets.Trash = TrashCanTexture;
            }
            TrashCanTexture = null;
        }

        private static bool ItemSlot_OverrideLeftClick(On.Terraria.UI.ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot)
        {
            var returnValue = orig(inv, context, slot);
            if (returnValue)
            {
                return true;
            }

            if (context == ItemSlot.Context.TrashItem)
            {
                if (inv[slot].IsAir)
                {
                    return false;
                }
                double amount = Main.LocalPlayer.Aequus().GetTrashPrice(inv[slot]);
                //Main.NewText($"Taken {Lang.GetItemName(inv[slot].type)} ({inv[slot].stack}) out of trash.");
                if (!Main.LocalPlayer.BuyItem((int)amount))
                {
                    return true;
                }
            }
            return false;
        }

        private static void ItemSlot_OnItemTransferred(ItemSlot.ItemTransferInfo info)
        {
            if (info.ToContext == ItemSlot.Context.TrashItem)
            {
                double amount = Main.LocalPlayer.Aequus().GetTrashPrice(info.ItemType, info.TransferAmount);
                //Main.NewText($"Sent {Lang.GetItemName(info.ItemType)} ({info.TransferAmount}) to trash.");
                Helper.DropMoney(new EntitySource_Misc("Aequus: Money Trashcan"), Main.LocalPlayer.Hitbox, (long)amount, quiet: false);
            }
        }
    }
}