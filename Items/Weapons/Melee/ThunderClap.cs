using AQMod.Assets.LegacyItemOverlays;
using AQMod.Content.Dusts;
using AQMod.Effects.ScreenEffects;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class ThunderClap : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow"), new Color(100, 100, 100, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 32;
            item.width = 30;
            item.height = 30;
            item.useTime = 30;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.rare = AQItem.Rarities.GaleStreamsRare;
            item.value = AQItem.Prices.GaleStreamsValue;
            item.melee = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.ThunderClap>();
            item.knockBack = 32f;
            item.scale = 1.25f;
            item.autoReuse = true;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<RedSpriteDust>());
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 800);
            target.GetGlobalNPC<AQNPC>().ChangeTemperature(target, 60);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int p = Projectile.NewProjectile(new Vector2(Main.MouseWorld.X + Main.rand.NextFloat(-20f, 20f), position.Y - 666f), new Vector2(0f, 0f), type, damage * 2, 0f, player.whoAmI);
            Main.PlaySound(SoundID.Item122, position);
            if (AQMod.TonsofScreenShakes)
            {
                ScreenShakeManager.AddEffect(new BasicScreenShake(16, 10));
            }
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SlapHand);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 10);
            r.AddIngredient(ModContent.ItemType<Materials.Fluorescence>(), 12);
            r.AddIngredient(ItemID.SoulofFlight, 20);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}