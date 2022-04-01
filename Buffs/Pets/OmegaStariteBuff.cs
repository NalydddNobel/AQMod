using Aequus.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    public class OmegaStariteBuff : PetBuffBase
    {
        protected override ref bool ActiveFlag(Player player) => ref player.GetModPlayer<AequusPlayer>().omegaStaritePet;
        protected override int PetProj => ModContent.ProjectileType<OmegaStaritePet>();
    }
}
