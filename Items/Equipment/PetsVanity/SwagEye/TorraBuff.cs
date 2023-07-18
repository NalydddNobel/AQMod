using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.SwagEye {
    /// <summary>
    /// Applied by <see cref="SwagLookingEye"/>
    /// </summary>
    public class TorraBuff : BasePetBuff {
        protected override int PetProj => ModContent.ProjectileType<TorraPet>();
    }
}