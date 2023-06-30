using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Pets.OmegaStarite {
    /// <summary>
    /// Applied by <see cref="DragonBall"/>
    /// </summary>
    public class OmegaStariteBuff : BasePetBuff {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<OmegaStaritePet>();
    }
}
