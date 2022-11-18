using Aequus.Items.Pets.Light;
using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="PersonalDronePack"/>
    /// </summary>
    public class DroneBuff : BasePetBuff
    {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<DronePet>();
    }
}