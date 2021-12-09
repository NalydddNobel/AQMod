using AQMod.Common.DeveloperTools;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Umbrella
{
    public class Umystick : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.damage = 45;
            item.useTime = 24;
            item.useAnimation = 24;
            item.rare = AQItem.Rarities.GaleStreamsRare + 1;
            item.value = AQItem.Prices.GaleStreamsValue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.magic = true;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.mana = 20;
            item.holdStyle = Constants.HoldStyle.Umbrella;
        }

        public override void HoldStyle(Player player)
        {
            bool maxSpeed = false;
            player.fallStart = (int)(player.position.Y / 16f);
            if (!player.controlDown)
            {
                if (player.gravDir == -1f)
                {
                    if (player.velocity.Y < -2f)
                    {
                        maxSpeed = true;
                        player.velocity.Y = -2f;
                    }
                }
                else if (player.velocity.Y > 2f)
                {
                    maxSpeed = true;
                    player.velocity.Y = 2f;
                }
            }
            if (player.gravDir == 1)
            {
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                if (!aQPlayer.mysticUmbrellaDelay)
                {
                    if (Main.myPlayer == player.whoAmI && maxSpeed && !player.mouseInterface)
                    {
                        int oldMana = item.mana;
                        item.mana *= 3;
                        if (Main.mouseRight && Main.mouseRightRelease && player.CheckMana(item, pay: true))
                        {
                            player.AddBuff(ModContent.BuffType<Buffs.Debuffs.Delays.UmystickDelay>(), 180);
                            AQSoundPlayer.PlaySound(SoundType.Item, "Sounds/Item/MysticUmbrella_" + Main.rand.Next(2), 0.6f);
                            player.velocity.Y = -12f;
                        }
                        item.mana = oldMana;
                    }
                }
                else
                {
                    if (player.velocity.Y < 0f)
                    {
                        int buffID = player.FindBuffIndex(ModContent.BuffType<Buffs.Debuffs.Delays.UmystickDelay>());
                        if (buffID != -1 && player.buffTime[buffID] > 120)
                        {
                            if (Main.myPlayer == player.whoAmI && Main.mouseRight)
                            {
                                player.velocity.Y += -0.32f;
                                player.gravity *= 0.5f;
                            }
                            if (Main.rand.NextBool())
                            {
                                float x = player.position.X + Main.rand.NextFloat(-24f, 22f);
                                if (player.direction == 1)
                                {
                                    x += player.width;
                                }
                                int d = Dust.NewDust(new Vector2(x, player.position.Y - 14f), 2, 2, 15);
                                Main.dust[d].velocity.X *= 0.1f;
                                Main.dust[d].velocity.Y = -player.velocity.Y * 0.2f;
                            }
                        }
                    }
                }
            }

            player.itemRotation = 0f;
            player.itemLocation.X += -player.direction * 20f;
        }
    }
}