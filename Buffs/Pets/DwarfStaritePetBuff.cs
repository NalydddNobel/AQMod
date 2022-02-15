using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Pets
{
    public class DwarfStaritePetBuff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<AQPlayer>().dwarfStarite = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.DwarfStaritePet>()] <= 0)
                Projectile.NewProjectile(player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, ModContent.ProjectileType<Projectiles.Pets.DwarfStaritePet>(), 0, 0f, player.whoAmI, 0f, 0f);
        }
    }
}
