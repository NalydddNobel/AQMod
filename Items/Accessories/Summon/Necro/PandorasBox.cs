using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public sealed class PandorasBox : ModItem
    {
        public static List<int> ProjectileTypesShot { get; private set; }

        public override void Load()
        {
            ProjectileTypesShot = new List<int>()
            {
                ProjectileID.WoodenArrowFriendly,
                ProjectileID.FireArrow,
                ProjectileID.ImpFireball,
                ProjectileID.Shuriken,
                ProjectileID.BallofFire,
                ProjectileID.GreenLaser,
                ProjectileID.WaterBolt,
                ProjectileID.Bone,
                ProjectileID.SpikyBall,
                ProjectileID.WaterStream,
                ProjectileID.HarpyFeather,
                ProjectileID.DemonScythe,
                ProjectileID.ThrowingKnife,
                ProjectileID.PoisonedKnife,
                ProjectileID.PoisonDart,
                ProjectileID.Stinger,
                ProjectileID.PoisonDartBlowgun,
                ProjectileID.AmethystBolt,
                ProjectileID.TopazBolt,
                ProjectileID.SapphireBolt,
                ProjectileID.EmeraldBolt,
                ProjectileID.RubyBolt,
                ProjectileID.DiamondBolt,
                ProjectileID.AmberBolt,
                ProjectileID.SnowBallFriendly,
                ProjectileID.IceSpike,
                ProjectileID.Bee,
                ProjectileID.Bullet,
                ProjectileID.StarAnise,
                ProjectileID.Hellwing,
            };
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void Unload()
        {
            ProjectileTypesShot?.Clear();
            ProjectileTypesShot = null;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDungeon;
            Item.value = ItemDefaults.DungeonValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.pandorasBoxItem = Item;
            if (aequus.pandorasBoxSpawnChance == 0 || aequus.pandorasBoxSpawnChance > 140)
            {
                aequus.pandorasBoxSpawnChance = 140;
            }
            aequus.ghostProjExtraUpdates += 1;
        }
    }
}