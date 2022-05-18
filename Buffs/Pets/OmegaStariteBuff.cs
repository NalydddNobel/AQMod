using Aequus.Projectiles.Misc.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="Items.Misc.Pets.DragonBall"/>
    /// </summary>
    public class OmegaStariteBuff : PetBuffBase
    {
        protected override ref bool ActiveFlag(Player player) => ref player.GetModPlayer<AequusPlayer>().omegaStaritePet;
        protected override int PetProj => ModContent.ProjectileType<OmegaStaritePet>();
    }
}
