using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Pets
{
    public class HeartMoth : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<AQPlayer>().heartMoth = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.HeartMoth>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.position.X + player.width / 2f, player.position.Y + player.height / 2f, 0f, 0f, ModContent.ProjectileType<Projectiles.Pets.HeartMoth>(), 0, 0f, player.whoAmI, 0f, 0f);
        }
    }
}
