using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Pets.Drone {
    /// <summary>
    /// Applied by <see cref="PersonalDronePack"/>
    /// </summary>
    public class DroneBuff : BasePetBuff {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<DronePet>();
    }
}