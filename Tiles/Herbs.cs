using AQMod.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public sealed class Herbs : ModTile
    {
		private const int FrameWidth = 26;
		private const int FrameHeight = 30;

        public const int Moonflower = 0;
        public const int Crepe = 1;

		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);

			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newTile.CoordinateWidth = FrameWidth;
			TileObjectData.newTile.CoordinateHeights = new int[] { FrameHeight };

			TileObjectData.newTile.AnchorValidTiles = new int[]
			{
				TileID.Grass,
				TileID.HallowedGrass,
			};

			TileObjectData.newTile.AnchorAlternateTiles = new int[]
			{
				TileID.ClayPot,
				TileID.PlanterBox
			};

			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				TileID.Grass,
				TileID.HallowedGrass,
				TileID.Meteorite,
			};
			TileObjectData.addSubTile(Moonflower);

			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				TileID.Grass,
				TileID.HallowedGrass,
				TileID.Cloud,
				TileID.RainCloud,
				TileID.SnowCloud,
			};
			TileObjectData.addSubTile(Crepe);

			TileObjectData.addTile(Type);

			dustType = 3;
			soundType = SoundID.Grass;

			AddMapEntry(new Color(186, 122, 255, 255), CreateMapEntryName("Moonflower"));
			AddMapEntry(new Color(132, 177, 177, 255), CreateMapEntryName("Crepe"));
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
			float multiplier = Math.Max(Main.tile[i, j].frameX / 56, 0.1f);
			if (Main.tile[i, j].frameX == 56)
			{
				switch (Main.tile[i, j].frameY)
				{
					case 0:
						{
							r = 0.45f * multiplier;
							g = 0.05f * multiplier;
							b = 1f * multiplier;
						}
						break;

					case 32:
						{
							r = 0.1f * multiplier;
							g = 0.325f * multiplier;
							b = 0.1f * multiplier;
						}
						break;
				}
			}
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;
		}

        public override bool Drop(int i, int j)
        {
			if (Main.tile[i, j].frameX == 56)
            {
                switch (Main.tile[i, j].frameY) 
				{
					case 0:
                        {
							Item.NewItem(new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<MoonflowerPollen>());
                        }
						break;
				}	
            }
            return false;
        }

        public override void RandomUpdate(int i, int j)
		{
			if (Main.tile[i, j].frameX < 28)
			{
				Main.tile[i, j].frameX += FrameWidth + 2;
				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(-1, i, j, 1);
				return;
			}
			bool blooming = false;
			switch (Main.tile[i, j].frameY)
            {
				case 0:
                    {
						blooming = !Main.dayTime && Main.time > (Main.nightLength / 2 - 3600) && Main.time < (Main.nightLength / 2 + 3600);
					}
					break;

				case 32:
                    {
						blooming = Main.windSpeed.Abs() > 0.3f;
					}
					break;
            }
			if (blooming)
			{
				if (Main.tile[i, j].frameX < 56)
				{
					Main.tile[i, j].frameX += FrameWidth + 2;
					if (Main.netMode != NetmodeID.SinglePlayer)
						NetMessage.SendTileSquare(-1, i, j, 1);
				}
			}
			else if (Main.tile[i, j].frameX > 28)
			{
				Main.tile[i, j].frameX -= FrameWidth + 2;
				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(-1, i, j, 1);
			}
		}

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			var texture = Main.tileTexture[Type];
			var effects = SpriteEffects.None;
			SetSpriteEffects(i, j, ref effects);
			var frame = new Rectangle(Main.tile[i, j].frameX, Main.tile[i, j].frameY, FrameWidth, FrameHeight);
			var offset = AQMod.Zero - Main.screenPosition;
			var groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f);
			spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, FrameHeight - 2f), 1f, effects, 0f);
			if (Main.tile[i, j].frameX == 56)
            {
				switch (Main.tile[i, j].frameY)
				{
					case 0:
						{
							float wave = AQUtils.Wave(Main.GlobalTime * 0.4f, 0.9f, 1.25f);
							var bloom = AQMod.Texture("Assets/EffectTextures/Bloom");
							var ray = ModContent.GetTexture(this.GetPath("_MoonflowerBloom"));
							var rayPosition = groundPosition + offset + new Vector2(0f, -22f);
							var rayColor = new Color(120, 100, 25, 5);
							var rayScale = new Vector2(0.85f, 0.65f);
							spriteBatch.Draw(bloom, rayPosition, null, rayColor * wave * 0.6f, 0f, bloom.Size() / 2f, rayScale * wave * 0.2f, SpriteEffects.None, 0f);
							spriteBatch.Draw(bloom, rayPosition, null, rayColor * wave * 0.3f, 0f, bloom.Size() / 2f, rayScale * wave * 0.6f, SpriteEffects.None, 0f);
							spriteBatch.Draw(ray, rayPosition, null, rayColor * wave, 0f, ray.Size() / 2f, rayScale * wave, SpriteEffects.None, 0f);
						}
						break;

					case 32:
						{
						}
						break;
				}
			}
			return false;
        }
    }
}