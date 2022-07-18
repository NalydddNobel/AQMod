﻿using Aequus.Graphics;
using Aequus.Items.Placeable;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles
{
    public class ForceAntiGravityBlockTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(Color.Orange);
            DustType = DustID.OrangeStainedGlass;
            ItemDrop = ModContent.ItemType<ForceAntiGravityBlock>();
            HitSound = SoundID.Tink;
            MineResist = 1.5f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.33f;
            g = 0.1f;
            b = 0f;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int tileHeight = GetTileHeight(i, j, 13);
            if (tileHeight == 0)
                return true;

            var texture = TextureCache.Bloom[2].Value;
            var drawCoords = new Vector2(i * 16f, j * 16f + 8f) - Main.screenPosition + AequusHelpers.TileDrawOffset;
            var frame = new Rectangle(texture.Width / 2, 0, 1, texture.Height / 2);
            var scale = new Vector2(16f, (tileHeight * 16 + 32) / frame.Height);
            spriteBatch.Draw(texture, drawCoords, frame, new Color(200, 50, 10, 0) * 0.5f, MathHelper.Pi, new Vector2(frame.Width, frame.Height), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawCoords, frame, new Color(255, 100, 20, 0) * 0.33f, MathHelper.Pi, new Vector2(frame.Width, frame.Height), scale, SpriteEffects.None, 0f);

            DrawParticles(i, j, tileHeight, spriteBatch);

            foreach (var p in GetInteractiblePlayers(i, j, tileHeight))
            {
                p.Aequus().antiGravityTile = -20;
            }
            return true;
        }

        public void DrawParticles(int i, int j, int tileHeight, SpriteBatch spriteBatch)
        {
            var rand = AequusEffects.EffectRand;
            int seed = rand.SetRand(i * j + j - i);

            var texture = ModContent.Request<Texture2D>(AequusHelpers.GetPath<MonoDust>());
            var origin = new Vector2(4f, 4f);
            var drawCoords = new Vector2(i * 16f, j * 16f + 8f) - Main.screenPosition + AequusHelpers.TileDrawOffset;
            int dustAmt = (int)(rand.Rand(tileHeight) / 1.5f + 2f);
            for (int k = 0; k < dustAmt; k++)
            {
                float p = rand.Rand(22f) + Main.GlobalTimeWrappedHourly * rand.Rand(2f, 5.2f);
                p %= 22f;
                p /= 22f;
                p = (float)Math.Pow(p, 3f);
                p *= 22f;
                p -= 2f;
                var frame = new Rectangle(0, 10 * (int)rand.Rand(3), 8, 8);
                var dustDrawOffset = new Vector2(AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * rand.Rand(0.45f, 1f), 0f, 16f), tileHeight * 16f - p * 16f);
                float opacity = rand.Rand(0.1f, 1f);
                float scale = rand.Rand(0.25f, 1.2f);
                if (p > 0f && p < tileHeight)
                {
                    if (p < 6f)
                    {
                        float progress = p / 6f;
                        opacity *= progress;
                        scale *= progress;
                    }
                    spriteBatch.Draw(texture.Value, drawCoords + dustDrawOffset, frame, 
                        Color.Orange.UseA(0) * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }

            rand.SetRand(seed);
        }

        public List<Player> GetInteractiblePlayers(int i, int j, int tileHeight)
        {
            var r = new Rectangle(i * 16, j * 16, 16, tileHeight * 16);
            var p = new List<Player>();
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                if (Main.player[k].active && !Main.player[k].dead && !Main.player[k].ghost)
                {
                    if (r.Intersects(Main.player[k].getRect()))
                    {
                        p.Add(Main.player[k]);
                    }
                }
            }
            return p;
        }

        public int GetTileHeight(int i, int j, int maxHeight = 20)
        {
            for (int k = 1; k < maxHeight; k++)
            {
                if (j + k > Main.maxTilesY - 10)
                {
                    return k;
                }
                if (Main.tile[i, j + k].IsSolid())
                {
                    return k;
                }
            }
            return maxHeight;
        }
    }
}