using Aequus.Common.Recipes;
using Aequus.Items.Equipment.PetsUtility.Drone;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.MagicMirrors.PhaseMirror {
    public class PhaseMirror : ModItem, IPhaseMirror {
        public List<(int, int, Dust)> DustEffectCache { get; set; }
        public int UseAnimationMax => 64;

        public override void Load() {
            On_Player.HasUnityPotion += Player_HasUnityPotion;
            On_Player.TakeUnityPotion += Player_TakeUnityPotion;
        }

        private static void Player_TakeUnityPotion(On_Player.orig_TakeUnityPotion orig, Player self) {
            if (self.FindItemInInvOrVoidBag(item => item.ModItem is IPhaseMirror, out bool _) != null)
                return;
            orig(self);
        }

        private static bool Player_HasUnityPotion(On_Player.orig_HasUnityPotion orig, Player self) {
            if (self.FindItemInInvOrVoidBag(item => item.ModItem is IPhaseMirror, out bool _) != null)
                return true;
            return orig(self);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.IceMirror);
            Item.rare = ItemRarityID.Green;
            Item.useTime = UseAnimationMax;
            Item.useAnimation = UseAnimationMax;
            DustEffectCache = new();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame) {
            if (!player.JustDroppedAnItem) {
                IPhaseMirror.UsePhaseMirror(player, Item, this);
            }
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<PersonalDronePack>());
        }

        public void Teleport(Player player, Item item, IPhaseMirror me) {
            player.Spawn(PlayerSpawnContext.RecallFromItem);
        }

        public void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
            dustType = DustID.MagicMirror;
            dustColor = Color.White;
        }
    }
}