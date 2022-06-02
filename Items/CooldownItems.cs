using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items
{
    public sealed class CooldownItems : GlobalItem
    {
        public const int CooldownBackFramesX = 26;
        public static Asset<Texture2D> CooldownBack { get; private set; }

        /// <summary>
        /// Whether or not this weapon has a cooldown effect. Currently only allows the cooldown background to be drawn behind this item when the player has a cooldown
        /// </summary>
        public static HashSet<int> HasWeaponCooldown { get; private set; }

        public override void Load()
        {
            HasWeaponCooldown = new HashSet<int>();
            if (!Main.dedServ)
            {
                CooldownBack = ModContent.Request<Texture2D>("Aequus/Assets/UI/CooldownBack");
            }
        }

        public override void Unload()
        {
            HasWeaponCooldown?.Clear();
            HasWeaponCooldown = null;
            CooldownBack = null;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.playerInventory/* && AequusUI.ItemSlotContext == ItemSlot.Context.InventoryItem*/ && HasWeaponCooldown.Contains(item.type))
            {
                var aequus = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
                if (aequus.itemCooldown > 0 && aequus.itemCooldownMax > 0)
                {
                    float progress = aequus.itemCooldown / (float)aequus.itemCooldownMax;
                    DrawCooldownBack(spriteBatch, position, frame, scale, new Color(255, 255, 255, 120) * (0.75f + progress * 0.25f), progress);
                }
            }
            return true;
        }

        public static void DrawCooldownBack(SpriteBatch spriteBatch, Vector2 position, Rectangle itemFrame, float itemScale, Color color, float progress = 1f)
        {
            var backFrame = CooldownBack.Value.Frame(horizontalFrames: CooldownBackFramesX, frameX: (int)(CooldownBackFramesX - CooldownBackFramesX * progress));
            var drawPosition = ItemSlotRenderer.InventoryItemGetCorner(position, itemFrame, itemScale);
            spriteBatch.Draw(CooldownBack.Value, drawPosition, backFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }
    }
}