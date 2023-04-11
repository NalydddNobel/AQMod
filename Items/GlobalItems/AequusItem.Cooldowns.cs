using Aequus;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem
    {
        public const int CooldownBackFramesX = 26;
        public static Asset<Texture2D> CooldownBack { get; private set; }

        /// <summary>
        /// Whether or not this weapon has a cooldown effect. Currently only allows the cooldown background to be drawn behind this item when the player has a cooldown.
        /// </summary>
        public static HashSet<int> HasWeaponCooldown { get; private set; }

        internal void Load_Cooldown()
        {
            HasWeaponCooldown = new HashSet<int>();
            if (!Main.dedServ)
            {
                CooldownBack = ModContent.Request<Texture2D>("Aequus/Assets/UI/CooldownBack");
            }
        }

        internal void Unload_Cooldown()
        {
            HasWeaponCooldown?.Clear();
            HasWeaponCooldown = null;
            CooldownBack = null;
        }

        internal void PreDraw_Cooldowns(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, float scale)
        {
            var aequus = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
            if (aequus.itemCooldown > 0 && aequus.itemCooldownMax > 0)
            {
                float progress = aequus.itemCooldown / (float)aequus.itemCooldownMax;
                DrawCooldownBack(sb, position, frame, scale, Main.inventoryBack * (0.75f + progress * 0.25f), progress);
            }
        }

        public static void DrawCooldownBack(SpriteBatch spriteBatch, Vector2 position, Rectangle itemFrame, float itemScale, Color color, float progress = 1f)
        {
            var backFrame = CooldownBack.Value.Frame(horizontalFrames: CooldownBackFramesX, frameX: (int)(CooldownBackFramesX - CooldownBackFramesX * progress));
            var drawPosition = ItemSlotRenderer.InventoryItemGetCorner(position, itemFrame, itemScale);
            spriteBatch.Draw(CooldownBack.Value, drawPosition, backFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }
    }
}