using Aequus.Common.Items.Components;
using Aequus.Content.Items.Tools.MagicMirrors.PhaseMirror;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.Items.Tools.MagicMirrors.PhasePhone {
    public abstract class PhasePhoneBase : ModItem, IPhaseMirror, IHoverSlot {
        public List<(int, int, Dust)> DustEffectCache { get; set; }
        public int UseAnimationMax => 64;
        public abstract int ShellphoneClone { get; }
        public abstract int ShellphoneConvert { get; }

        public override void SetStaticDefaults() {
            ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = ModContent.ItemType<PhasePhone>();
            ItemID.Sets.ShimmerCountsAsItem[Type] = ModContent.ItemType<PhasePhone>();
            ItemID.Sets.WorksInVoidBag[Type] = true;
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
            if (Main.myPlayer == player.whoAmI && Main.mouseRight && Main.mouseRightRelease && !Main.LocalPlayer.lastMouseInterface && Main.LocalPlayer.ItemTimeIsZero) {
                Item.Transform(ShellphoneConvert);
                SoundEngine.PlaySound(SoundID.Unlock);
            }
        }

        // TODO: Make this support other modded info accessories which add their ingredients to the PDA.
        public override void UpdateInfoAccessory(Player player) {
            player.GetModPlayer<AequusPlayer>().infiniteWormhole = true;
            player.accWatch = 3;
            player.accCompass = 1;
            player.accDepthMeter = 1;
            player.accCalendar = true;
            player.accDreamCatcher = true;
            player.accFishFinder = true;
            player.accJarOfSouls = true;
            player.accOreFinder = true;
            player.accStopwatch = true;
            player.accThirdEye = true;
            player.accWeatherRadio = true;
            player.accCritterGuide = true;
        }

        public abstract void Teleport(Player player, Item item, IPhaseMirror me);

        public abstract void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor);

        public virtual bool HoverSlot(Item[] inventory, int context, int slot) {
            if (context == ItemSlot.Context.InventoryItem && Main.mouseRight && Main.mouseRightRelease && Main.LocalPlayer.ItemTimeIsZero) {
                Item.Transform(ShellphoneConvert);
                SoundEngine.PlaySound(SoundID.Unlock);
                Main.mouseRightRelease = false;
            }
            return false;
        }
    }
}
