using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items {
    public abstract class OnlineLinkedItem : ModItem {
        public abstract string Link { get; }
        public abstract Asset<Texture2D> ButtonTexture { get; }

        private string _linkCache;
        private Asset<Texture2D> _buttonTextureCache;
        private uint _pressedTime;

        public override void SetDefaults() {
            _linkCache = Link;
            if (!Main.dedServ) {
                _buttonTextureCache = ButtonTexture;
            }
        }

        public virtual bool CanShowButton(int context) {
            return AequusUI.ValidOnlineLinkedSlotContext.Contains(context);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (string.IsNullOrEmpty(_linkCache) || !CanShowButton(AequusUI.CurrentItemSlot.Context)) {
                return;
            }

            var buttonTexture = _buttonTextureCache.Value;
            float buttonScale = Main.inventoryScale * 0.9f;
            Vector2 buttonPosition;
            int context = Math.Abs(AequusUI.CurrentItemSlot.Context);
            if (context == ItemSlot.Context.EquipAccessory) {
                buttonPosition = new(position.X - 26f * Main.inventoryScale, position.Y - 24f * Main.inventoryScale);
            }
            else {
                buttonPosition = new(position.X + 6f * Main.inventoryScale, position.Y - 24f * Main.inventoryScale);
            }
            var destination = new Rectangle((int)buttonPosition.X, (int)buttonPosition.Y, (int)(buttonTexture.Width * buttonScale), (int)(buttonTexture.Height * buttonScale));

            spriteBatch.Draw(buttonTexture, destination, null, Main.inventoryBack);

            if (!destination.Contains(Main.mouseX, Main.mouseY) || !AequusUI.CanDoLeftClickItemActions || AequusUI.linkClickDelay != 0) {
                return;
            }

            AequusUI.DisableItemLeftClick = 2;
            Main.HoverItem = new();
            Main.hoverItemName = "";
            Main.instance.MouseText("Double click to open:\n" + _linkCache, 4);

            if (Main.mouseLeft && Main.mouseLeftRelease) {
                if (_pressedTime < Main.GameUpdateCount) {
                    _pressedTime = Main.GameUpdateCount + 30;
                    return;
                }
                Main.mouseLeftRelease = false;
                Main.NewText(Language.GetTextValue("Mods.Aequus.OpenedLink") + " " + Link, Colors.RarityGreen);
                AequusUI.linkClickDelay = 60;
                Utils.OpenToURL(Link);
            }
        }
    }
}