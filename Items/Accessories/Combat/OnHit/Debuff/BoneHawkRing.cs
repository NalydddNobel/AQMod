using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Net.Sounds;
using Aequus.Common.Recipes;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Combat.OnHit.Debuff;
using Aequus.Items.Tools;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Debuff {
    public interface IBoneHawkRing : IAccessoryData {
        int InflictChance { get; }
        int DebuffDuration { get; }
    }

    [AutoloadEquip(EquipType.HandsOn)]
    [LegacyName("BoneRing")]
    public class BoneHawkRing : ModItem, IBoneHawkRing {
        public int InflictChance => 10;
        public int DebuffDuration => 300;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.ChanceFrac(InflictChance));

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accBoneRing.SetAccessory(Item, this);
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<Bellows>());
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {
        public Accessory<IBoneHawkRing> accBoneRing = new();

        private void ProcBoneHawkRing(NPC target) {
            int stacks = accBoneRing.EquipmentStacks();
            if (!accBoneRing.TryGetItem(out var item, out var boneHawkRing) || Player.RollLuck(Math.Max(boneHawkRing.InflictChance / stacks, 1)) != 0) {
                return;
            }
            AequusBuff.ApplyBuff<BoneRingWeakness>(target, boneHawkRing.DebuffDuration * stacks, out bool canPlaySound);
            if (canPlaySound) {
                ModContent.GetInstance<WeaknessDebuffSound>().Play(target.Center);
            }
            if (canPlaySound || target.HasBuff<BoneRingWeakness>()) {
                for (int i = 0; i < 12; i++) {
                    var v = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(target.Center + v * new Vector2(Main.rand.NextFloat(target.width / 2f + 16f), Main.rand.NextFloat(target.height / 2f + 16f)), DustID.AncientLight, v * 8f);
                    d.noGravity = true;
                    d.noLightEmittence = true;
                }
            }
        }
    }
}