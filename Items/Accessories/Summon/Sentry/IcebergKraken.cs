using Aequus.Items.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Sentry
{
    public class IcebergKraken : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.GaleStreamsValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SentrySquidPlayer>().autoSentry = true;
            player.GetModPlayer<IcebergKrakenPlayer>().frostburnSentry = true;
            player.maxTurrets++;
        }

        public override void AddRecipes()
        {
            AequusRecipes.SpaceSquidRecipe(this, ModContent.ItemType<SentrySquid>());
        }
    }

    public class IcebergKrakenPlayer : ModPlayer
    {
        public bool frostburnSentry;

        public override void ResetEffects()
        {
            frostburnSentry = false;
        }
    }

    public class IcebergKrakenProjectile : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (Main.player[projectile.owner].GetModPlayer<IcebergKrakenPlayer>().frostburnSentry && Main.rand.NextBool(6))
                {
                    target.AddBuff(BuffID.Frostburn2, 240);
                }
            }
        }
    }
}