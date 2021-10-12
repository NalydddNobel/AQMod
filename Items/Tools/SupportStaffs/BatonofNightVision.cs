using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Projectiles.Support;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.SupportStaffs
{
    public class BatonofNightVision : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlayData(AQUtils.GetPath(this) + "_Glow", new Color(128, 128, 128, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 28;
            item.useTime = 28;
            item.buffType = BuffID.NightOwl;
            item.buffTime = 600;
            item.shoot = ModContent.ProjectileType<NightOwl>();
            item.shootSpeed = 9f;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item9;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var nightOwl = NightOwl.SpawnNightOwl(position, damage, knockBack, player);
            nightOwl.maxPower = 900f;
            nightOwl.baton = true;
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<StaffofNightVision>());
            r.AddIngredient(ItemID.HallowedBar, 4);
            r.AddIngredient(ItemID.SoulofSight);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}