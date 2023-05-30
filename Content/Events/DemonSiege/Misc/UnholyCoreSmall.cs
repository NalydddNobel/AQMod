using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.DemonSiege.Misc {
    public class UnholyCoreSmall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(Type, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override void AddRecipes() {
            Recipe.Create(ItemID.Vilethorn)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.CrimsonRod)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.BallOHurt)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.TheRottedFork)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.BandofStarpower)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.PanicNecklace)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.ShadowOrb)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.CrimsonHeart)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Musket)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.TheUndertaker)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}