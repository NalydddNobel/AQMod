using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="Items.Misc.Pets.ToySpaceGun"/>
    /// </summary>
    public class SpaceSquidBuff : PetBuffBase
    {
        protected override int PetProj => ModContent.ProjectileType<SpaceSquidPet>();
    }
}
