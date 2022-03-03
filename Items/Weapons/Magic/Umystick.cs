using AQMod.Common.ID;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class Umystick : ModItem, ICooldown, ICombo
    {
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.damage = 47;
            item.useTime = 24;
            item.useAnimation = 24;
            item.rare = AQItem.Rarities.GaleStreamsRare + 1;
            item.value = AQItem.Prices.GaleStreamsWeaponValue;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.magic = true;
            item.knockBack = 4f;
            item.mana = 12;
            item.holdStyle = HoldStyleID.Umbrella;
            item.channel = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.Umystick>();
            item.shootSpeed = 1f;
        }

        public override void HoldStyle(Player player)
        {
            if (player.itemAnimation > 0)
                return;
            if (item.useStyle == ItemUseStyleID.HoldingOut)
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                return;
            }
            item.noUseGraphic = false;
            item.holdStyle = 2;
            bool maxSpeed = false;
            player.fallStart = (int)(player.position.Y / 16f);
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (!player.controlDown)
            {
                float cap = aQPlayer.itemCooldown > 0 ? 3f : 2f;
                if (player.gravDir == -1f)
                {
                    if (player.velocity.Y < -cap)
                    {
                        maxSpeed = true;
                        player.velocity.Y = -cap;
                    }
                }
                else if (player.velocity.Y > cap)
                {
                    maxSpeed = true;
                    player.velocity.Y = cap;
                }
            }
            if (player.gravDir == 1)
            {
                if (aQPlayer.itemCooldown == 0)
                {
                    if (Main.myPlayer == player.whoAmI && maxSpeed && !player.mouseInterface)
                    {
                        int oldMana = item.mana;
                        item.mana *= 5;
                        if (Main.mouseRight && Main.mouseRightRelease && player.CheckMana(item, pay: true) && aQPlayer.ItemCooldownCheck(180, effectedByCooldownStats: true, item: item))
                        {
                            if (aQPlayer.itemSwitch > 0)
                            {
                                aQPlayer.itemCooldown *= 3;
                                aQPlayer.itemCooldownMax *= 3;
                            }
                            else
                            {
                                aQPlayer.itemSwitch += (ushort)(aQPlayer.itemCooldown * 2);
                            }
                            aQPlayer.ItemCombo(60, effectedByComboStats: true, doVisuals: false, item: item);
                            AQSound.LegacyPlay(SoundType.Item, AQSound.Paths.MysticUmbrellaJump, 0.6f);
                            player.velocity.Y = -12f;
                        }
                        item.mana = oldMana;
                    }
                }
                else
                {
                    if (player.velocity.Y < 0f)
                    {
                        if (aQPlayer.itemCombo > 0)
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
                                    x += player.width;
                                int d = Dust.NewDust(new Vector2(x, player.position.Y - 14f), 2, 2, 15);
                                Main.dust[d].velocity.X *= 0.1f;
                                Main.dust[d].velocity.Y = -player.velocity.Y * 0.2f;
                            }
                        }
                    }
                }
            }

            if (!player.pulley)
            {
                player.itemRotation = 0f;
                player.itemLocation.X += -player.direction * 20f;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (base.CanUseItem(player) && player.CheckMana(item, pay: false))
            {
                item.noUseGraphic = true;
                item.useStyle = ItemUseStyleID.HoldingOut;
                return true;
            }
            return false;
        }

        ushort ICooldown.Cooldown(Player player, AQPlayer aQPlayer)
        {
            return 0;
        }
    }
}