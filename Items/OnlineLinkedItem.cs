using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items
{
    /// <summary>
    /// Currently only used by the Aequus Music mod's music boxes.
    /// </summary>
    public abstract class OnlineLinkedItem : ModItem
    {
        public abstract string Link { get; }

        public virtual bool CanShowButton(int context)
        {
            return AequusUI.ValidOnlineLinkedSlotContext.Contains(context);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (CanShowButton(AequusUI.itemSlotContext))
            {
                var buttonTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay", AssetRequestMode.ImmediateLoad);
                float buttonScale = Main.inventoryScale * 0.8f;
                var buttonPosition = position + frame.Size() / 2f * scale + TextureAssets.InventoryBack.Value.Size() / 2f * Main.inventoryScale - buttonTexture.Size() * buttonScale + new Vector2(-4f, -4f);
                var destination = new Rectangle((int)buttonPosition.X, (int)buttonPosition.Y, (int)(buttonTexture.Value.Width * buttonScale), (int)(buttonTexture.Value.Height * buttonScale));
                spriteBatch.Draw(buttonTexture.Value, destination, null, Color.White);
                if (destination.Contains(Main.mouseX, Main.mouseY) && AequusUI.CanDoLeftClickItemActions && AequusUI.linkClickDelay == 0)
                {
                    AequusUI.DisableItemLeftClick = 2;
                    Main.HoverItem = new Item();
                    Main.hoverItemName = "";
                    Main.instance.MouseText(Link, 1);
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.mouseLeftRelease = false;
                        Main.NewText(Language.GetTextValue("Mods.Aequus.OpenedLink") + " " + Link, Colors.RarityBlue);
                        AequusUI.linkClickDelay = 60;
                        Utils.OpenToURL(Link);
                    }
                }
            }
        }
    }
}