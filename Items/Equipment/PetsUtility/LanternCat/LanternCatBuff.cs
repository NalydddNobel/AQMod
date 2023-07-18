using Aequus.Common.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsUtility.LanternCat {
    /// <summary>
    /// Applied by <see cref="LanternCatSpawner"/>
    /// </summary>
    public class LanternCatBuff : BasePetBuff {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<LanternCatPet>();

        public override void Update(Player player, ref int buffIndex) {
            base.Update(player, ref buffIndex);
            player.Aequus().addLuck += 0.05f;
        }
    }
}