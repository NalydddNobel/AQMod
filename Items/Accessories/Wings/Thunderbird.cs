using AQMod.Content.Players;
using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class Thunderbird : ModItem, IDedicatedItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                AQMod.ArmorOverlays.AddWingsOverlay<Thunderbird>(new EquipThunderbirdWingsOverlay());
            }
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = AQItem.Rarities.DedicatedItem;
            item.value = Item.sellPrice(gold: 20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return (lightColor * 0.2f).UseA(255);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var clr = Main.LocalPlayer.FX().NalydGradientPersonal.GetColor(Main.GlobalTime) * 0.2f;
            var texture = Main.itemTexture[item.type];
            var wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            var n = new Vector2(wave, 0f).RotatedBy(Main.GlobalTime * 2.9f);
            clr.A = 0;
            for (int i = 1; i <= 4; i++)
            {
                spriteBatch.Draw(texture, position + n * i * scale, frame, clr, 0f, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, position - n * i * scale, frame, clr, 0f, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var clr = Main.LocalPlayer.FX().NalydGradientPersonal.GetColor(Main.GlobalTime) * 0.2f;
            var texture = Main.itemTexture[item.type];
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var position = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            var wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            var n = new Vector2(wave, 0f).RotatedBy(Main.GlobalTime * 2.9f);
            clr.A = 0;
            for (int i = 1; i <= 4; i++)
            {
                spriteBatch.Draw(texture, position + n * i * scale, frame, clr, rotation, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, position - n * i * scale, frame, clr, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].overrideColor = Main.LocalPlayer.FX().ThunderbirdGradient.GetColor(Main.GlobalTime);
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                TooltipText.DrawNarrizuulText(line);
                return false;
            }
            return true;
        }

        private void UpdateLightning(Player player)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var center = player.Center;
            if (aQPlayer.thunderbirdLightningTimer <= 0)
            {
                var validNPCs = new List<int>();
                float healthPercentage = player.statLife / (float)player.statLifeMax2;
                float yOff = player.height * MathHelper.Lerp(-4f, 4f, healthPercentage);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    var n = Main.npc[i];
                    if (n.active && n.position.Y - yOff > player.position.Y && Vector2.Distance(n.Center, player.Center) < 750f && n.CanBeChasedBy())
                        validNPCs.Add(i);
                }
                if (validNPCs.Count > 0)
                {
                    var npc = Main.npc[validNPCs[Main.rand.Next(validNPCs.Count)]];
                    var npcCenter = npc.Center;
                    var spawnPosition = new Vector2(Main.rand.Next((int)npc.position.X - 20, (int)npc.position.X + npc.width + 20), (int)player.position.Y - 600);
                    if (Collision.CanHitLine(npc.position, npc.width, npc.height, spawnPosition, 2, 2))
                    {
                        float length = (npc.position - spawnPosition).Length();
                        int segments = (int)length / 20;
                        Vector2 add = new Vector2(1f, 0f).RotatedBy((npcCenter - spawnPosition).ToRotation()) * (int)length / segments;
                        Vector2 line = spawnPosition;
                        Vector2 lastPos = line;
                        for (int j = 0; j < segments; j++)
                        {
                            Vector2 pos = line + add.RotatedBy(Main.rand.NextBool() ? -MathHelper.PiOver2 : MathHelper.PiOver2) * Main.rand.NextFloat(1f);
                            var toPos = Vector2.Normalize(lastPos - pos);
                            float distance = Vector2.Distance(pos, lastPos);
                            int count = 0;
                            while (distance > 0)
                            {
                                int d = Dust.NewDust(pos + toPos * 8 * count, 2, 2, DustID.Electric);
                                Main.dust[d].velocity *= 0.1f;
                                Main.dust[d].noGravity = true;
                                count++;
                                distance -= 8f;
                            }
                            lastPos = pos;
                            line += add;
                        }
                        {
                            Vector2 pos = npcCenter;
                            var toPos = Vector2.Normalize(lastPos - pos);
                            float distance = Vector2.Distance(pos, lastPos);
                            int count = 0;
                            while (distance > 0)
                            {
                                int d = Dust.NewDust(pos + toPos * 8 * count, 2, 2, DustID.Electric);
                                Main.dust[d].velocity *= 0.1f;
                                Main.dust[d].noGravity = true;
                                count++;
                                distance -= 8f;
                            }
                        }
                        Main.PlaySound(SoundID.Item122, spawnPosition);
                        player.ApplyDamageToNPC(npc, Main.DamageVar(75f), 0f, 0, false);
                        aQPlayer.thunderbirdLightningTimer = (int)MathHelper.Lerp(60, 120, healthPercentage);
                    }
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.wingTimeMax = 200;
            UpdateLightning(player);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 12f;
            acceleration *= 3f;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.AngelWings);
            r.AddIngredient(ModContent.ItemType<ThunderousPlume>());
            r.AddIngredient(ItemID.SoulofSight, 15);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.DemonWings);
            r.AddIngredient(ModContent.ItemType<ThunderousPlume>());
            r.AddIngredient(ItemID.SoulofSight, 15);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }

        Color IDedicatedItem.Color => new Color(200, 125, 255, 255);
    }
}