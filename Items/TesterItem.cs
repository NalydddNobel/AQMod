using Aequus.Items.Weapons.Magic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    internal class TesterItem : ModItem
    {
        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.width = 20;
            Item.height = 20;
        }

        public override bool? UseItem(Player player)
        {
            int x = AequusHelpers.tileX;
            int y = AequusHelpers.tileY;

            //Main.NewText(Biomes.PeacefulGlimmerBiome.TileLocationX);
            //Main.NewText(Biomes.PeacefulGlimmerBiome.CalcTiles(player));
            Projectile.NewProjectile(null, Main.MouseWorld, Microsoft.Xna.Framework.Vector2.Zero, ModContent.ProjectileType<Projectiles.Monster.ArcubusProjs.ArcubusDamagingFloor>(), 20, 1f, player.whoAmI);
            //Content.Necromancy.NecromancyDatabase.LoadEntriesFile();
            return true;
        }
    }
}