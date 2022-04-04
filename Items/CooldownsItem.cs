using Aequus.Common.Players;
using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed class CooldownsItem : GlobalItem
    {
        /// <summary>
        /// Whether or not this weapon has a cooldown effect. Currently only allows the cooldown background to be drawn behind this item when the player has a cooldown
        /// </summary>
        public static HashSet<int> HasWeaponCooldown { get; private set; }

        internal static void OnModLoad()
        {
            HasWeaponCooldown = new HashSet<int>()
            {
                ModContent.ItemType<StudiesOfTheInkblot>(),
            };
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return !HasWeaponCooldown.Contains(item.type) || player.GetModPlayer<ItemVarsPlayer>().itemCooldown <= 0;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.playerInventory && HasWeaponCooldown.Contains(item.type))
            {
                var cooldowns = Main.LocalPlayer.GetModPlayer<ItemVarsPlayer>();
                if (cooldowns.itemCooldown > 0 && cooldowns.itemCooldownMax > 0)
                {
                    float progress = cooldowns.itemCooldown / (float)cooldowns.itemCooldownMax;
                    AequusHelpers.DrawUIBack(spriteBatch, Aequus.Tex("UI/InventoryBack"), position, frame, scale, new Color(155, 155, 105, 250) * (0.75f + progress * 0.25f), progress);
                }
            }
            return true;
        }
    }
}