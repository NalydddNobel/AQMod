using Aequus.Common.Catalogues;
using Aequus.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class ZombieScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;

            NecromancyTypes.StaffTiers.Add(Type, 1f);

            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(30);
            Item.shoot = ModContent.ProjectileType<NecromancerBolt>();
            Item.shootSpeed = 6f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
        }
    }
}