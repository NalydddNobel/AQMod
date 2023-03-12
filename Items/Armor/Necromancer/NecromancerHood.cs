using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Armor.Necromancer;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Necromancer
{
    [AutoloadEquip(EquipType.Head)]
    public class NecromancerHood : ModItem
    {
        public int[] EnemySpawn;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.defense = 4;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            Item.damage = 60;
            Item.DamageType = NecromancyDamageClass.Instance;
            Item.shoot = ModContent.ProjectileType<NecromancerHoodSpawnerProj>();
            EnemySpawn = new int[]
            {
                NPCID.Skeleton,
                NPCID.ArmoredSkeleton,
                NPCID.SkeletonArcher,
            };
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<NecromancerRobe>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = TextHelper.GetTextValue("ArmorSetBonus.Necromancer");
            player.Aequus().armorNecromancerBattle = this;
            player.Aequus().empoweredLegs = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.Aequus().ghostSlotsMax++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GravetenderHood>()
                .AddIngredient<DemonicEnergy>(1)
                .AddTile(TileID.Loom)
                .TryRegisterBefore(ItemID.GravediggerShovel);
        }
    }

    public class NecromancerHoodSpawnerProj : ModProjectile
    {
        public override string Texture => AequusTextures.None.Path;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.CadetBlue;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}

namespace Aequus
{
    partial class AequusPlayer 
    {
        public NecromancerHood armorNecromancerBattle;

        public void UseNecromancerArmor()
        {
            if (ghostSlots > 0 || armorNecromancerBattle == null)
            {
                return;
            }


        }
    }
}