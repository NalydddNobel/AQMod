using Aequus.Buffs.Pets;
using Aequus.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class FamiliarPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item2;
            Item.value = Item.buyPrice(gold: 20);
            Item.rare = ItemRarities.RarityPet;
            Item.shoot = ModContent.ProjectileType<FamiliarPet>();
            Item.buffType = ModContent.BuffType<FamiliarBuff>();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(Item.buffType, 3600, true);
        }
    }
}