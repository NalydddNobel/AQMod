using Aequus.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace Aequus.UI.Elements
{
    public class PlaceableItemSlotElement : ItemSlotElement
    {
        public Func<Item, bool> CanPlaceInSlot;
        public bool StackMustBe1;

        public PlaceableItemSlotElement(Texture2D back, SpriteFrameData icon = null) : base(back, icon)
        {
        }

        private void DoClick(ref Item incomingItem)
        {
            item ??= new();

            if (incomingItem.IsAir)
            {
                if (HasItem)
                {
                    Utils.Swap(ref item, ref incomingItem);
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                return;
            }

            if (CanPlaceInSlot?.Invoke(incomingItem) == false)
            {
                return;
            }

            if (StackMustBe1) {
                if (incomingItem.stack > 1) {
                    if (!item.IsAir) {
                        if (item.type == incomingItem.type) {
                            incomingItem.stack += item.stack;
                            item.TurnToAir();
                            SoundEngine.PlaySound(SoundID.Grab);
                        }
                        return;
                    }
                    else {
                        Main.mouseItem.stack--;
                        item = Main.mouseItem.Clone();
                        item.stack = 1;
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                    return;
                }
            }

            Utils.Swap(ref item, ref incomingItem);
            SoundEngine.PlaySound(SoundID.Grab);
        }

        public override void Click(UIMouseEvent evt)
        {
            DoClick(ref Main.mouseItem);
            base.Click(evt);
        }
    }
}