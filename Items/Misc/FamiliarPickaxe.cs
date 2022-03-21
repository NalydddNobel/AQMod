using AQMod.Buffs.Pets;
using AQMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public class FamiliarPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item2;
            item.value = Item.buyPrice(gold: 20);
            item.rare = AQItem.RarityPet;
            item.shoot = ModContent.ProjectileType<MiniPlayerPet>();
            item.buffType = ModContent.BuffType<FamiliarBuff>();
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(item.buffType, 3600, true);
        }
    }
}