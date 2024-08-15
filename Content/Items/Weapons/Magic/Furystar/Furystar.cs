using Aequus.Common.Items;
using Terraria.DataStructures;

namespace Aequus.Content.Items.Weapons.Magic.Furystar;

public class Furystar : ModItem {
    public static readonly float ExtraStarChance = 0.25f;
    public static readonly int MaxExtraStars = 5;

    public override void SetStaticDefaults() {
        //Element.Air.AddItem(Type);
        Item.staff[Type] = true;
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
        Item.value = ItemDefaults.NPCSkyMerchant;
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

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
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