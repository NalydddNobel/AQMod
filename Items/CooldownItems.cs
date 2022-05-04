using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed class CooldownItems : GlobalItem
    {
        /// <summary>
        /// Whether or not this weapon has a cooldown effect. Currently only allows the cooldown background to be drawn behind this item when the player has a cooldown
        /// </summary>
        public static HashSet<int> HasWeaponCooldown { get; private set; }

        public override void Load()
        {
            HasWeaponCooldown = new HashSet<int>();
        }

        public override void Unload()
        {
            HasWeaponCooldown?.Clear();
            HasWeaponCooldown = null;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return !HasWeaponCooldown.Contains(item.type) || player.GetModPlayer<AequusPlayer>().itemCooldown <= 0;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.playerInventory && HasWeaponCooldown.Contains(item.type))
            {
                var aequus = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
                if (aequus.itemCooldown > 0 && aequus.itemCooldownMax > 0)
                {
                    float progress = aequus.itemCooldown / (float)aequus.itemCooldownMax;
                    AequusHelpers.DrawUIBack(spriteBatch, Images.InventoryBack.Value, position, frame, scale, new Color(125, 125, 125, 250) * (0.75f + progress * 0.25f), progress);
                }
            }
            return true;
        }
    }
}