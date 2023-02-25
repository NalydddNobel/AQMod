using Aequus.Items.Vanity.Pets.Light;
using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="DragonBall"/>
    /// </summary>
    public class OmegaStariteBuff : BasePetBuff
    {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<OmegaStaritePet>();
    }
}
