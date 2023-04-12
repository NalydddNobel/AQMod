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
        public Func<Item, bool> TakeOutOfSlot;
        public Action<Item, Item> OnSlotSwap;
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
                    if (TakeOutOfSlot?.Invoke(incomingItem) == false) {
                        return;
                    }

                    OnSlotSwap?.Invoke(item, incomingItem);
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

                            if (TakeOutOfSlot?.Invoke(incomingItem) == false) {
                                return;
                            }

                            OnSlotSwap?.Invoke(item, incomingItem);
                            incomingItem.stack += item.stack;
                            item.TurnToAir();
                            SoundEngine.PlaySound(SoundID.Grab);
                        }
                        return;
                    }
                    else {

                        OnSlotSwap?.Invoke(item, incomingItem);
                        incomingItem.stack--;
                        item = incomingItem.Clone();
                        item.stack = 1;
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                    return;
                }
            }

            OnSlotSwap?.Invoke(item, incomingItem);
            Utils.Swap(ref item, ref incomingItem);
            SoundEngine.PlaySound(SoundID.Grab);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            DoClick(ref Main.mouseItem);
            base.LeftClick(evt);
        }
    }
}