using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Elites.Items
{
    [LegacyName("ArgonMushroom")]
    public class ElitePlantArgon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlants>(), EliteBuffPlants.Argon);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
    public class ElitePlantArgonHostile : ModItem
    {
        public override string Texture => Helper.GetPath<ElitePlantArgon>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlantsHostile>(), EliteBuffPlants.Argon);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }

    [LegacyName("KryptonMushroom")]
    public class ElitePlantKrypton : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlants>(), EliteBuffPlants.Krypton);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
    public class ElitePlantKryptonHostile : ModItem
    {
        public override string Texture => Helper.GetPath<ElitePlantKrypton>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlantsHostile>(), EliteBuffPlants.Krypton);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }

    public class ElitePlantNeon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlants>(), EliteBuffPlants.Neon);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
    public class ElitePlantNeonHostile : ModItem
    {
        public override string Texture => Helper.GetPath<ElitePlantNeon>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlantsHostile>(), EliteBuffPlants.Neon);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }

    [LegacyName("XenonMushroom")]
    public class ElitePlantXenon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlants>(), EliteBuffPlants.Xenon);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
    public class ElitePlantXenonHostile : ModItem
    {
        public override string Texture => Helper.GetPath<ElitePlantXenon>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EliteBuffPlantsHostile>(), EliteBuffPlants.Xenon);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
}