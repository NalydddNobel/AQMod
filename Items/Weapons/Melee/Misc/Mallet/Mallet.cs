using Aequus.Common.Items;
using Aequus.Common.Items.Variants;
using Aequus.Content.CursorDyes;
using Aequus.Projectiles.Melee;
using ReLogic.Content;
using Terraria.GameContent.Items;

namespace Aequus.Items.Weapons.Melee.Misc.Mallet;
public class Mallet : ModItem {
    public class MalletCursor : ICursorDye {
        protected string Texture;
        protected int malletProj;
        public int Type { get; set; }

        public MalletCursor(string texture, int malletProjID) {
            Texture = texture;
            malletProj = malletProjID;
        }

        bool ICursorDye.DrawThickCursor(ref Vector2 bonus, ref bool smart) {
            return false;
        }

        bool ICursorDye.PreDrawCursor(ref Vector2 bonus, ref bool smart) {
            string texture = Texture;
            if (Main.cursorOverride > 0) {
                return true;
            }

            if (Main.LocalPlayer.ownedProjectileCounts[malletProj] > 0) {
                texture += "_1";
            }
            else {
                texture += "_0";
            }

            float scale = Main.cursorScale * 0.8f;

            var textureAsset = ModContent.Request<Texture2D>(texture, AssetRequestMode.ImmediateLoad);

            var drawCoords = new Vector2(Main.mouseX - 8f, Main.mouseY - 12f);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(texture + "_Outline", AssetRequestMode.ImmediateLoad).Value, drawCoords, null, Main.MouseBorderColor.UseA(255), 0f, default(Vector2), scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(textureAsset.Value, drawCoords, null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public static int CursorDyeID { get; private set; }
    public float MaximumDistance => Item.Variant == ItemVariants.StrongerVariant ? 400f : 200f;

    public override void SetStaticDefaults() {
        AequusItemVariants.AddVariant(Type, ItemVariants.StrongerVariant, Condition.RemixWorld);
        if (Main.netMode != NetmodeID.Server) {
            CursorDyeID = CursorDyeSystem.Register(new MalletCursor($"{Texture}Cursor", ModContent.ProjectileType<MalletProj>()));
        }
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.damage = 16;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = true;
        Item.value = Item.buyPrice(gold: 2, silver: 50);
        Item.rare = ItemRarityID.Blue;
        Item.DamageType = DamageClass.Melee;
        Item.knockBack = 1.25f;
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<MalletProj>();

        if (Item.Variant == ItemVariants.StrongerVariant) {
            Item.SetWeaponValues(105, 3f, 10);
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.rare = ItemDefaults.RarityHardmodeDungeon;
            Item.value = Item.sellPrice(gold: 11);
        }
    }

    public override void HoldItem(Player player) {
        float maximumDistance = MaximumDistance;
        if (Main.myPlayer == player.whoAmI && Main.cursorOverride <= 0 && !player.mouseInterface && Vector2.Distance(Main.MouseWorld, player.Center) < maximumDistance) {
            player.Aequus().cursorDyeOverride = CursorDyeID;
        }
    }

    public override bool? UseItem(Player player) {
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        position = Main.MouseWorld;
        var normal = Vector2.Normalize(position - player.Center);
        velocity = normal * 0.1f;

        float maximumDistance = MaximumDistance;
        if (Vector2.Distance(position, player.Center) > maximumDistance) {
            position = player.Center + normal * maximumDistance;
        }
    }
}