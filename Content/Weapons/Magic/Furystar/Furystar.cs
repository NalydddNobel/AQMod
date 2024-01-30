using Aequus.Common.Items;
using Terraria.DataStructures;

namespace Aequus.Content.Weapons.Magic.Furystar;

public class Furystar : ModItem {
    public static System.Single ExtraStarChance = 0.25f;
    public static System.Int32 MaxExtraStars = 5;

    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Starfury;
        ItemID.Sets.ShimmerTransformToItem[ItemID.Starfury] = Type;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.SetWeaponValues(14, 0.5f);
        Item.DamageType = DamageClass.Magic;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.mana = 18;
        Item.rare = ItemRarityID.Green;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
        Item.shoot = ModContent.ProjectileType<FurystarProj>();
        Item.UseSound = SoundID.Item8;
        Item.shootSpeed = 24f;
        Item.noMelee = true;
        Item.shootsEveryUse = true;
        Item.channel = true;
        Item.noUseGraphic = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White with { A = 200 };
    }

    public override Vector2? HoldoutOrigin() {
        return new Vector2(0, 2) + new Vector2(1f).RotatedBy(Main.GlobalTimeWrappedHourly * 7.5f);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame) {
    }

    public override System.Boolean Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, System.Int32 type, System.Int32 damage, System.Single knockback) {
        //var mouseWorld = Main.MouseWorld;
        //position = mouseWorld + new Vector2(Main.rand.NextFloat(-360f, 360f), -1000f);
        //velocity = Vector2.Normalize(mouseWorld - position) * velocity.Length();
        //var starFuryVector = mouseWorld;
        //var toMouse = (position - mouseWorld).SafeNormalize(new Vector2(0f, -1f));
        //while (starFuryVector.Y > position.Y && WorldGen.SolidTile(starFuryVector.ToTileCoordinates())) {
        //    starFuryVector += toMouse * 16f;
        //}
        //Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, ai1: starFuryVector.Y);
        return true;
    }
}