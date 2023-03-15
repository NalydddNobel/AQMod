using Aequus;
using Aequus.Buffs;
using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Items.Misc.Dyes;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static System.Formats.Asn1.AsnWriter;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.playerInventory)
            {
                if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.EquipAccessory)
                {
                    PreDraw_CrownOfBlood(item, spriteBatch, position, frame, scale);
                }
            }
            else
            {
                if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.HotbarItem && HasWeaponCooldown.Contains(item.type))
                {
                    PreDraw_Cooldowns(item, spriteBatch, position, frame, scale);
                }
            }
            return true;
        }

        internal void PreDraw_CrownOfBlood(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, float scale)
        {
            var aequus = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
            if (aequus.accBloodCrownSlot > -1 && AequusUI.CurrentItemSlot.Slot == aequus.accBloodCrownSlot)
            {
                var backFrame = TextureAssets.InventoryBack16.Value.Frame();
                var drawPosition = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
                var color = new Color(150, 60, 60, 255);

                sb.Draw(TextureAssets.InventoryBack16.Value, drawPosition, backFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
            }
        }

        private void PostDraw_PrefixPotions(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.prefix >= PrefixID.Count 
                && PrefixLoader.GetPrefix(item.prefix) is PotionPrefixBase potionPrefix 
                && potionPrefix.HasGlint)
            {
                var texture = TextureAssets.Item[item.type].Value;

                Main.spriteBatch.End();
                spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);

                var drawData = new DrawData(
                    texture,
                    position,
                    frame,
                    (itemColor.A > 0 ? itemColor : Main.inventoryBack) with { A = 160 } * Helper.Wave(Main.GlobalTimeWrappedHourly, 0.66f, 1f), 0f,
                    origin,
                    scale, SpriteEffects.None, 0
                );

                var effect = GameShaders.Misc[potionPrefix.ShaderKey];
                effect.Apply(drawData);

                drawData.Draw(spriteBatch);

                Main.spriteBatch.End();
                spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
            }
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            PostDraw_PrefixPotions(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);   
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (reversedGravity)
            {
                rotation = MathHelper.Pi - rotation;
            }
            return true;
        }
    }
}