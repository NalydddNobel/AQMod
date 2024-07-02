using Aequus.Core;
using Aequus.Old.Content.Items.Materials;
using Terraria.DataStructures;
using tModLoaderExtended.GlowMasks;

namespace Aequus.Old.Content.Items.Weapons.Summon.Minions.StariteMinion;

[AutoloadGlowMask]
public class StariteStaff : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.GamepadWholeScreenUseRange[Item.type] = true;
    }

    public override void SetDefaults() {
        Item.SetWeaponValues(20, 5f);
        Item.mana = 10;
        Item.DamageType = DamageClass.Summon;
        Item.rare = Commons.Rare.EventGlimmer;
        Item.value = Commons.Cost.EventGlimmer;
        Item.useTime = 24;
        Item.useAnimation = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.UseSound = SoundID.Item44;
        Item.shootSpeed = 8f;
        ModContent.GetInstance<StariteMinion>().SetItemStats(Item);
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 255);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        player.AddBuff(Item.buffType, 2);
        player.SpawnMinionOnCursor(source, player.whoAmI, type, damage, knockback);
        return false;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<StariteMaterial>(14)
            .AddIngredient(ItemID.FallenStar, 3)
            .AddTile(TileID.Anvils)
            .Register();
    }
}