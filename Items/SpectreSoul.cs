using AQMod.Assets.Textures;
using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class SpectreSoul : ModItem
    {
        public override string Texture => AQMod.ModName + "/" + AQTextureAssets.None;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 6;
            item.height = 6;
            item.rare = ItemRarityID.Blue;
            item.buffType = ModContent.BuffType<Buffs.SpectreHealing>();
            item.buffTime = 150;
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange *= 3;
        }

        public override void PostUpdate()
        {
            int d = Dust.NewDust(item.position, item.width, item.height, 180);
            Main.dust[d].scale = 1.2f;
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity.X *= 0.035f;
            Main.dust[d].velocity.Y = -Main.dust[d].velocity.Y.Abs() * 1.25f;
        }

        public override bool OnPickup(Player player)
        {
            player.AddBuff(item.buffType, item.buffTime);
            return false;
        }
    }
}