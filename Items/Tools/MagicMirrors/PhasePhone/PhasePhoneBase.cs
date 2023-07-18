using Aequus.Common.Items;
using Aequus.Items.Tools.MagicMirrors.PhaseMirror;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Tools.MagicMirrors.PhasePhone {
    public abstract class PhasePhoneBase : ModItem, IPhaseMirror, ItemHooks.IHoverSlot {
        public List<(int, int, Dust)> DustEffectCache { get; set; }
        public int UseAnimationMax => 64;
        public abstract int ShellphoneClone { get; }
        public abstract int ShellphoneConvert { get; }

        public override void SetStaticDefaults() {
            ContentSamples.CreativeResearchItemPersistentIdOverride.Add(Type, ModContent.ItemType<PhasePhone>());
            ItemID.Sets.ShimmerCountsAsItem[Type] = ModContent.ItemType<PhasePhone>();
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ShellphoneClone);
            Item.rare++;
            Item.useTime = UseAnimationMax;
            Item.useAnimation = UseAnimationMax;
            DustEffectCache = new();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame) {
            if (player.altFunctionUse != 2 && !player.JustDroppedAnItem) {
                IPhaseMirror.UsePhaseMirror(player, Item, this);
            }
        }

        public override void HoldItem(Player player) {
            if (Main.mouseRight && Main.mouseRightRelease && !Main.playerInventory && Main.myPlayer == player.whoAmI) {
                Item.Transform(ShellphoneConvert);
                SoundEngine.PlaySound(SoundID.Unlock);
            }
        }

        public abstract void Teleport(Player player, Item item, IPhaseMirror me);

        public abstract void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor);

        public virtual bool HoverSlot(Item[] inventory, int context, int slot) {
            if (context == ItemSlot.Context.InventoryItem && Main.mouseRight && Main.mouseRightRelease) {
                Item.Transform(ShellphoneConvert);
                SoundEngine.PlaySound(SoundID.Unlock);
            }
            return true;
        }
    }
}
