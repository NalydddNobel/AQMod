using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated
{
    public class Thunderbird : ModItem, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlr = player.GetModPlayer<AQPlayer>();
            var plrCenter = player.Center;
            if (aQPlr.thunderbirdJumpTimer == 1)
            {
                for (int i = 0; i < 15; i++)
                {
                    int d = Dust.NewDust(player.position, player.height, player.height, DustID.Electric, 0f, 0f, 0, default(Color), 0.5f); // using height for both width and height to make it square
                    Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].position - plrCenter) * 8f;
                }
            }
            if (aQPlr.thunderbirdJumpTimer <= 0 && !player.mount.Active && player.wingTime <= 0 && player.velocity.Y != 0f && !player.HasDoubleJumpLeft())
            {
                player.canCarpet = false;
                bool canThunderbirdJump = player.direction == -1 ? player.velocity.X <= -1f : player.velocity.X >= 1f;
                if (canThunderbirdJump)
                {
                    if (Main.rand.NextBool(8))
                        Dust.NewDust(new Vector2(player.position.X, player.position.Y + player.height), player.width, 10, DustID.Electric, 0f, 0f, 0, default(Color), 0.3f);
                    if (player.controlJump && player.releaseJump)
                    {
                        player.velocity.X = player.direction * player.velocity.X.Abs() * 1.85f;
                        player.velocity.Y = -14f;
                        player.fallStart = (int)(player.position.Y / 16f);
                        aQPlr.thunderbirdJumpTimer = 120;
                        aQPlr.thunderbirdLightningTimer = 240;
                        int dustWidth = player.width * 3;
                        int dustHeight = player.height / 2 + 2;
                        var dustPos = new Vector2(player.Center.X - dustWidth / 2, player.position.Y + player.height - 10f);
                        Main.PlaySound(SoundID.DoubleJump, player.position);
                        for (int i = 0; i < 50; i++)
                        {
                            Dust.NewDust(dustPos, dustWidth, dustHeight, DustID.Electric);
                        }
                    }
                }
            }
            if (aQPlr.thunderbirdLightningTimer <= 0)
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
                        aQPlr.thunderbirdLightningTimer = (int)MathHelper.Lerp(60, 120, healthPercentage);
                    }
                }
            }
        }

        Color IDedicatedItem.DedicatedItemColor() => DedicatedColors.Gerd;
    }
}