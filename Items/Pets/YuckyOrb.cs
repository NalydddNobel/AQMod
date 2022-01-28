using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Pets
{
    public class YuckyOrb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item2;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<Projectiles.Pets.AnglerFish>();
            item.buffType = ModContent.BuffType<Buffs.Pets.AnglerFish>();
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(item.buffType, 3600, true);
        }
    }
}