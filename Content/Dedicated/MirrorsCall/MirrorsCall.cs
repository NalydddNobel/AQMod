using Aequus.Common.Items.Dedications;
using Terraria.DataStructures;

namespace Aequus.Content.Dedicated.MirrorsCall;

public class MirrorsCall : ModItem {
    public override void Load() {
        DedicationRegistry.Register(this, new DefaultDedication("Mr. Gerd26", new Color(110, 110, 128)));
    }

    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<MirrorsCallProj>(7);
        Item.SetWeaponValues(200, 6f, 6);
        Item.width = 24;
        Item.height = 24;
        Item.autoReuse = true;
        Item.rare = ItemRarityID.Red;
        Item.value = Item.sellPrice(gold: 20);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool? UseItem(Player player) {
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override bool AltFunctionUse(Player player) {
        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        return true;
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        for (float f = 0f; f < 1f; f += 0.125f) {
            Vector2 spinningPoint = (f * MathHelper.TwoPi).ToRotationVector2();
            Main.spriteBatch.Draw(
                AequusTextures.MirrorsCall_Aura,
                position + spinningPoint * 8f * scale,
                frame,
                ExtendColor.GetLastPrismColor(Main.LocalPlayer, f * 6f) with { A = 0 } * 0.3f,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out _, out Rectangle frame);
        Texture2D texture = AequusTextures.MirrorsCall_Aura.Value;
        Vector2 position = ExtendItem.WorldDrawPos(Item, frame) + new Vector2(0f, -2f);
        Vector2 origin = frame.Size() / 2f;

        for (float f = 0f; f < 1f; f += 0.125f) {
            Vector2 spinningPoint = (f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 5f).ToRotationVector2();
            Main.spriteBatch.Draw(
                AequusTextures.MirrorsCall_Aura,
                position + spinningPoint * 4f * scale,
                frame,
                ExtendColor.GetLastPrismColor(Main.LocalPlayer, f * 6f) with { A = 0 } * 0.15f,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    public override void AddRecipes() {
#if !DEBUG
        CreateRecipe()
            .AddIngredient(ItemID.PiercingStarlight)
            .AddIngredient(ModContent.ItemType<Old.Content.Items.Weapons.Melee.Slice.Slice>())
            .AddIngredient(ItemID.LunarBar, 10)
            //.AddIngredient(ModContent.ItemType<UltimateEnergy>(), 5)
            .AddIngredient(ItemID.WhitePearl)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
#endif
    }
}