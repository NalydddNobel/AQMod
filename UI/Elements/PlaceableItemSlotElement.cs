using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Aequus.UI.Elements
{
    public class PlaceableItemSlotElement : ItemSlotElement
    {
        public Func<Item, bool> CanPlaceInSlot;

        public PlaceableItemSlotElement(Texture2D back, SpriteFrameData icon = null) : base(back, icon)
        {
        }

        private void UpdateItemSwaps()
        {
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (Main.mouseItem.IsAir)
                {
                    if (HasItem)
                    {
                        Utils.Swap(ref item, ref Main.mouseItem);
                    }
                    return;
                }

                if (CanPlaceInSlot?.Invoke(Main.mouseItem) == false)
                {
                    return;
                }
                Utils.Swap(ref item, ref Main.mouseItem);
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateItemSwaps();
        }
    }
}