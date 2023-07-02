using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.SoulGem; 

public class SoulGemFilled : ModItem {
    public override string Texture => Helper.GetPath<SoulGem>();

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<SoulGemFilledTile>());
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.buyPrice(gold: 1, silver: 50);
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, Color.White.UseA(0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 0.33f),
            0f, origin, scale, SpriteEffects.None, 0f);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        Item.GetItemDrawData(out var frame);
        spriteBatch.Draw(TextureAssets.Item[Type].Value, ItemDefaults.WorldDrawPos(Item, TextureAssets.Item[Type].Value) + new Vector2(0f, -2f), frame, Color.White.UseA(0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.33f, 0.66f),
            rotation, TextureAssets.Item[Type].Value.Size() / 2f, scale, SpriteEffects.None, 0f);
    }

    public override void AddRecipes() {
        AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<SoulGem>());
        Recipe.Create(ItemID.LifeCrystal)
            .AddIngredient(Type)
            .AddIngredient<BloodyTearstone>()
            .AddTile(TileID.DemonAltar)
            .Register()
            .DisableDecraft();
    }
}