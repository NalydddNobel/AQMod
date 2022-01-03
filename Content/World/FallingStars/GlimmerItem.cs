using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.World.FallingStars
{
    public sealed class GlimmerItem : GlobalItem
    {
        public bool shimmering;

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (shimmering)
            {
                if (Main.dayTime)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Dust.NewDust(item.position, item.width, item.height, 15, item.velocity.X, item.velocity.Y, 150, default(Color), 1.2f);
                    }
                    if (AQGraphics.Rendering.Culling.InScreenWorld(item.getRect()))
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            Gore.NewGore(item.position, new Vector2(item.velocity.X, item.velocity.Y), Main.rand.Next(16, 18));
                        }
                    }
                    item.active = false;
                    item.type = 0;
                    item.stack = 0;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(21, -1, -1, null, item.whoAmI);
                    }
                }
                else
                {
                    var dustColor = default(Color);
                    switch (item.type) 
                    {
                        default:
                            {
                                Lighting.AddLight((int)((item.position.X + (float)item.width) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.8f, 0.7f, 0.1f);
                            }
                            break;

                        case ItemID.ManaCrystal:
                            {
                                Lighting.AddLight((int)((item.position.X + (float)item.width) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.1f, 0.1f, 0.8f);
                                dustColor = Color.Blue;
                            }
                            break;

                        case ItemID.LifeCrystal:
                            {
                                Lighting.AddLight((int)((item.position.X + (float)item.width) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.7f, 0.1f, 0.4f);
                            }
                            break;
                    }
                    if (Main.GameUpdateCount % 12 == 0)
                    {
                        var d = Dust.NewDustPerfect(item.Center + new Vector2(0f, (float)item.height * 0.2f) + Main.rand.NextVector2CircularEdge(item.width, (float)item.height * 0.6f) * (0.3f + Main.rand.NextFloat() * 0.5f), 228, new Vector2(0f, (0f - Main.rand.NextFloat()) * 0.3f - 1.5f), 127, dustColor);
                        d.scale = 0.5f;
                        d.fadeIn = 1.1f;
                        d.noGravity = true;
                        d.noLight = true;
                    }
                }
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            shimmering = false;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (shimmering)
            {
                var auraColor = default(Color);
                var auraColor2 = default(Color);
                switch (item.type)
                {
                    case ItemID.ManaCrystal:
                        {
                            auraColor = new Color(80, 80, 50, 50);
                            auraColor2 = new Color(150, 150, 130, 127);
                        }
                        break;
                    case ItemID.LifeCrystal:
                        {
                            auraColor = new Color(100, 20, 110, 50);
                            auraColor2 = new Color(230, 30, 250, 127);
                        }
                        break;
                }
                AQGraphics.Rendering.DrawFallenStarAura(item, spriteBatch, scale, auraColor, auraColor2);
            }
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
    }
}