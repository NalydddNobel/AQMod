using Aequus.Items.Pets;
using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="ToySpaceGun"/>
    /// </summary>
    public class SpaceSquidBuff : BasePetBuff
    {
        protected override int PetProj => ModContent.ProjectileType<SpaceSquidPet>();
    }
}
