using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="Items.Misc.Pets.DragonBall"/>
    /// </summary>
    public class OmegaStariteBuff : PetBuffBase
    {
        protected override int PetProj => ModContent.ProjectileType<OmegaStaritePet>();
    }
}
