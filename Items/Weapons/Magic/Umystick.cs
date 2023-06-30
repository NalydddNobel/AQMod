using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic {
    public class Umystick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            AequusItem.HasCooldown.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.damage = 36;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.ValueGaleStreams;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 4f;
            Item.mana = 20;
            Item.holdStyle = ItemHoldStyleID.HoldHeavy;
            Item.channel = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<UmystickProj>();
            Item.shootSpeed = 1f;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.HeldItemFixed().ModItem is Umystick umystick)
            {
                umystick.HandleHoldStyleFixed(player);
            }
        }
        private void HandleHoldStyleFixed(Player player)
        {
            if (player.itemAnimation > 0)
                return;
            if (Item.useStyle == ItemUseStyleID.Shoot)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                return;
            }
            Item.noUseGraphic = false;
            Item.holdStyle = 2;
            bool maxSpeed = false;
            player.fallStart = (int)(player.position.Y / 16f);
            var aequus = player.GetModPlayer<AequusPlayer>();
            if (!player.controlDown)
            {
                float cap = aequus.itemCooldown > 0 ? 3f : 2f;
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
                if (aequus.itemCooldown == 0)
                {
                    if (Main.myPlayer == player.whoAmI && maxSpeed && !player.mouseInterface)
                    {
                        int oldMana = Item.mana;
                        Item.mana *= 3;
                        if (Main.mouseRight && Main.mouseRightRelease && player.CheckMana(Item, pay: true) && !aequus.HasCooldown)
                        {
                            aequus.SetCooldown(180, ignoreStats: true, itemReference: Item);
                            if (aequus.itemSwitch > 0)
                            {
                                aequus.itemCooldown *= 3;
                                aequus.itemCooldownMax *= 3;
                            }
                            else
                            {
                                aequus.itemSwitch += (ushort)(aequus.itemCooldown * 2);
                            }
                            aequus.itemCombo = 60;
                            player.velocity.Y = -12f;
                            Projectile.NewProjectile(player.GetSource_ItemUse(Item, "Umystick Jump"), player.position, Vector2.Zero, ModContent.ProjectileType<UmystickDoubleJumpProj>(), 0, 0f, player.whoAmI);
                        }
                        Item.mana = oldMana;
                    }
                }
                else
                {
                    if (player.velocity.Y < 0f)
                    {
                        if (aequus.itemCombo > 0)
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
                                int d = Dust.NewDust(new Vector2(x, player.position.Y - 14f), 2, 2, DustID.MagicMirror);
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
            if (base.CanUseItem(player) && player.CheckMana(Item, pay: false))
            {
                Item.noUseGraphic = true;
                Item.useStyle = ItemUseStyleID.Shoot;
                return true;
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Name.StartsWith("Tooltip"))
                {
                    t.Text = Helper.FormatWith(t.Text, new { PlayerName = Main.LocalPlayer.name, });
                }
            }
        }

        public override void AddRecipes()
        {
            AequusRecipes.AddShimmerCraft(ItemID.Umbrella, Type, Condition.Hardmode);
        }
    }
}