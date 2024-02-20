using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.Audio;
using Terraria.UI;

namespace Aequus.Core.UI.Elements;

public class AequusPlaceableItemSlotElement : AequusItemSlotElement {
    public Func<Item, bool> CanPlaceInSlot;
    public Func<Item, bool> TakeOutOfSlot;
    public Action<Item, Item> OnSlotSwap;
    public bool SingleStack;

    public AequusPlaceableItemSlotElement(Texture2D back, int context, Asset<Texture2D> icon = null, Rectangle? frame = null) : base(context, back, icon, frame) {
    }

    private void DoClick(ref Item incomingItem) {
        item ??= new();

        if (incomingItem.IsAir) {
            if (HasItem) {
                if (TakeOutOfSlot?.Invoke(incomingItem) == false) {
                    return;
                }

                OnSlotSwap?.Invoke(item, incomingItem);
                Utils.Swap(ref item, ref incomingItem);
                SoundEngine.PlaySound(SoundID.Grab);
            }
            return;
        }

        if (CanPlaceInSlot?.Invoke(incomingItem) == false) {
            return;
        }

        if (SingleStack) {
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

    public override void LeftClick(UIMouseEvent evt) {
        DoClick(ref Main.mouseItem);
        base.LeftClick(evt);
    }
}