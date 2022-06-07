using Aequus.Common;
using Aequus.Projectiles.Ranged.Birds;
using Aequus.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Slingshot : ModItem
    {
        public static SoundStyle? STTTTTTTTTTTTTTTTRETCHSound { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                STTTTTTTTTTTTTTTTRETCHSound = new SoundStyle("Aequus/Sounds/Items/Slingshot/stretch") { Volume = 0.2f, };
            }
        }

        public override void SetStaticDefaults()
        {
            LootPools.Chests.Add(new LootPools.Chests.FrontChestLoot(Type, 4), ChestTypes.Skyware);
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.width = 32;
            Item.height = 24;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<SlingshotBirdProj>();
            Item.shootSpeed = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = STTTTTTTTTTTTTTTTRETCHSound;
            Item.value = Item.sellPrice(gold: 2);
            Item.knockBack = 1f;
            Item.useAmmo = SlingshotAmmos.BirdAmmo;
            Item.channel = true;
        }
    }

    public class SlingshotAmmos : GlobalItem
    {
        public static int BirdAmmo;

        public override void Load()
        {
            BirdAmmo = ItemID.Bird;
        }

        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.Bird || item.type == ItemID.Cardinal || item.type == ItemID.BlueJay ||
                item.type == ItemID.Duck || item.type == ItemID.MallardDuck || item.type == ItemID.Seagull ||
                item.type == ItemID.Grebe)
            {
                item.ammo = BirdAmmo;
            }
        }
    }
}