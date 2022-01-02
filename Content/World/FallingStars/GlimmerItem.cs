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
                Rectangle frame;
                if (Main.itemAnimations[item.type] != null)
                {
                    frame = Main.itemAnimations[item.type].GetFrame(Main.itemTexture[item.type]);
                }
                else
                {
                    frame = Main.itemTexture[item.type].Frame();
                }
                Vector2 vector = frame.Size() / 2f;
                Vector2 vector2 = new Vector2((float)(item.width / 2) - vector.X, item.height - frame.Height);
                Vector2 vector3 = item.position - Main.screenPosition + vector + vector2;
                float num7 = (float)Main.GameUpdateCount / 240f + Main.GlobalTime * 0.04f;
                float globalTimeWrappedHourly2 = Main.GlobalTime;
                globalTimeWrappedHourly2 %= 5f;
                globalTimeWrappedHourly2 /= 2.5f;
                if (globalTimeWrappedHourly2 >= 1f)
                {
                    globalTimeWrappedHourly2 = 2f - globalTimeWrappedHourly2;
                }
                globalTimeWrappedHourly2 = globalTimeWrappedHourly2 * 0.5f + 0.5f;
                var auraColor = new Color(50, 50, 255, 50);
                var auraColor2 = new Color(120, 120, 255, 127);
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
                            auraColor = new Color(50, 100, 60, 50);
                            auraColor2 = new Color(130, 200, 150, 127);
                        }
                        break;
                }
                for (float num8 = 0f; num8 < 1f; num8 += 0.25f)
                {
                    spriteBatch.Draw(Main.itemTexture[item.type], vector3 + new Vector2(0f, 8f).RotatedBy((num8 + num7) * ((float)Math.PI * 2f)) * globalTimeWrappedHourly2, frame, auraColor, item.velocity.X * 0.2f, vector, scale, SpriteEffects.None, 0f);
                }
                for (float num9 = 0f; num9 < 1f; num9 += 0.34f)
                {
                    spriteBatch.Draw(Main.itemTexture[item.type], vector3 + new Vector2(0f, 4f).RotatedBy((num9 + num7) * ((float)Math.PI * 2f)) * globalTimeWrappedHourly2, frame, auraColor2, item.velocity.X * 0.2f, vector, scale, SpriteEffects.None, 0f);
                }
            }
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
    }
}