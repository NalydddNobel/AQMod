﻿using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class FriendshipMagick : BaseScepter
    {
        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(60);
            Item.shoot = ModContent.ProjectileType<FriendshipMagickProj>();
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);
            Item.mana = 50;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }
    }
}