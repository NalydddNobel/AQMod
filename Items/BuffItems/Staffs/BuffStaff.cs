using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems.Staffs
{
    public abstract class BuffStaff : ModItem
    {
        protected abstract int BuffType { get; }

        protected virtual int BuffTime => 3600;

        protected abstract int DustType { get; }

        protected virtual int DustType2 => DustType;

        protected virtual int ManaCost => 20;
        protected virtual int Rarity => 1;

        public sealed override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useStyle = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.buffType = BuffType;
            item.buffTime = BuffTime;
            item.rare = Rarity;
            item.mana = ManaCost;
            item.UseSound = SoundID.Item4;
        }

        public sealed override bool UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                var localPlayer = Main.LocalPlayer;
                Vector2 center = localPlayer.Center;
                Vector2 difference = center - player.Center;
                float length = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                if (length < 800f)
                {
                    localPlayer.AddBuff(item.buffType, item.buffTime);
                    var normal = Vector2.Normalize(difference);
                    for (int i = 0; i < 80; i++)
                    {
                        float mult = i * 10f;
                        Vector2 position = center - normal * mult;
                        int d = Dust.NewDust(position, 20, 20, DustType);
                        Main.dust[d].velocity *= 0.05f;
                        if (mult > length)
                            break;
                    }
                }
            }
            Vector2 pos = new Vector2(800f, 0);
            const int circleDustAmount = 80;
            const float rotation = MathHelper.TwoPi / circleDustAmount;
            Vector2 center1 = player.Center;
            for (int i = 0; i < circleDustAmount; i++)
            {
                int d = Dust.NewDust(center1 + pos.RotatedBy(rotation * i), 10, 10, DustType2);
                Main.dust[d].velocity *= 0.01f;
                Main.dust[d].scale *= 1.25f;
                Main.dust[d].noLight = true;
            }
            return true;
        }
    }
}
