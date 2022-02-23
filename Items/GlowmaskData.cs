using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items
{
    internal struct GlowmaskData : ICloneable
    {
        public interface IWorld
        {
            void Draw(GlowmaskData glowmask, Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI);
        }
        public interface IInventory
        {
            void Draw(GlowmaskData glowmask, Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale);
        }
        public interface IPlayerHeld
        {
            void Draw(GlowmaskData glowmask, Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info);
        }

        public static Dictionary<int, GlowmaskData> ItemToGlowmask;

        private readonly IWorld _world;
        private readonly IInventory _inv;
        private readonly IPlayerHeld _held;
        public readonly Texture2D Tex;

        public GlowmaskData(Texture2D tex, IWorld world = null, IInventory inv = null, IPlayerHeld held = null)
        {
            Tex = tex;
            _world = world;
            _inv = inv;
            _held = held;
        }

        public void DrawWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            _world?.Draw(this, item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }

        public void DrawInv(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            _inv?.Draw(this, item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public void DrawHeld(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            _held?.Draw(this, player, aQPlayer, item, info);
        }

        object ICloneable.Clone()
        {
            return new GlowmaskData();
        }
    }
}