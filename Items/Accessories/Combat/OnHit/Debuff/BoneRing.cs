using Aequus.Buffs.Debuffs;
using Aequus.Common.Buffs;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.ModPlayers;
using Aequus.Common.Net.Sounds;
using Aequus.Items.Accessories.Combat.OnHit.Debuff;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Debuff {
    public interface IBoneRing : IAccessoryData {
        int DebuffDuration { get; }
    }

    [AutoloadEquip(EquipType.HandsOn)]
    [LegacyName("BoneHawkRing")]
    public class BoneRing : ModItem, IBoneRing {
        public int DebuffDuration => 30;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.MultiplierPercentDifference(BoneRingWeakness.MovementSpeedMultiplier), DebuffDuration / 60f);

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(
                TextHelper.Create.MultiplierPercentDifference(BoneRingWeakness.MovementSpeedMultiplier), 
                DebuffDuration * 2 / 60f)
            ));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accBoneRing.SetAccessory(Item, this);
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public Accessory<IBoneRing> accBoneRing = new();

        private void ProcBoneRing(NPC target) {
            int stacks = accBoneRing.EquipmentStacks();
            if (!accBoneRing.TryGetItem(out var item, out var BoneRing)) {
                return;
            }
            AequusBuff.ApplyBuff<BoneRingWeakness>(target, BoneRing.DebuffDuration * stacks, out bool canPlaySound);

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