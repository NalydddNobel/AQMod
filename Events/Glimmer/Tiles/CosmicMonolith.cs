using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Events.Glimmer.Tiles
{
    public class CosmicMonolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BloodMoonMonolith);
            Item.accessory = true;
            Item.vanity = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.createTile = ModContent.TileType<CosmicMonolithTile>();
            Item.placeStyle = 0;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CosmicMonolithScene.Active = 10;
        }

        public override void UpdateVanity(Player player)
        {
            CosmicMonolithScene.Active = 10;
        }
    }

    public class CosmicMonolithTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 18 };
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(10, 139, 166, 255));
            DustType = 1;
            AnimationFrameHeight = 18 * 4 + 2;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0.1f;
            b = 0.15f;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            drawData.finalColor = Color.White;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<CosmicMonolith>());
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameY >= AnimationFrameHeight)
            {
                CosmicMonolithScene.Active = 30;
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame >= 11)
                    frame = 0;
            }
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            HitWire(i, j);
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<CosmicMonolith>();
        }

        public override void HitWire(int i, int j)
        {
            int x = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int y = j - Main.tile[i, j].TileFrameY / 18 % 4;
            for (int l = x; l < x + 2; l++)
            {
                for (int m = y; m < y + 4; m++)
                {
                    if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == Type)
                    {
                        if (Main.tile[l, m].TileFrameY < 18 * 4 + 2)
                        {
                            Main.tile[l, m].TileFrameY += 18 * 4 + 2;
                        }
                        else
                        {
                            Main.tile[l, m].TileFrameY -= 18 * 4 + 2;
                        }
                    }
                }
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(x, y);
                Wiring.SkipWire(x, y + 1);
                Wiring.SkipWire(x, y + 2);
                Wiring.SkipWire(x, y + 3);
                Wiring.SkipWire(x + 1, y);
                Wiring.SkipWire(x + 1, y + 1);
                Wiring.SkipWire(x + 1, y + 2);
                Wiring.SkipWire(x + 1, y + 3);
            }
            NetMessage.SendTileSquare(-1, x, y, 2, 4);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int frameX = Main.tile[i, j].TileFrameX;
            int frameY = Main.tile[i, j].TileFrameY;
            int height = frameY % (18 * 4 + 2) == 54 ? 18 : 16;
            if (frameY >= 18 * 4 + 2)
            {
                frameY += Main.tileFrame[Type] * (18 * 4 + 2);
            }
            var t = Main.instance.TilePaintSystem.TryGetTileAndRequestIfNotReady(Type, 0, Main.tile[i, j].TileColor);
            if (t == null)
                t = TextureAssets.Tile[Type].Value;
            spriteBatch.Draw(t, new Vector2(i * 16f, j * 16f) + AequusHelpers.TileDrawOffset - Main.screenPosition, new Rectangle(frameX, frameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class CosmicMonolithScene : ModSceneEffect
    {
        public const string Key = "Aequus:CosmicMonolith";

        public static int Active { get; set; }

        public override bool IsSceneEffectActive(Player player)
        {
            return Active > 0;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SkyManager.Instance[Key] = new GlimmerSky() { checkDistance = false, };
            }
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                if (!SkyManager.Instance[Key].IsActive())
                {
                    SkyManager.Instance.Activate(Key);
                }
            }
            else
            {
                if (SkyManager.Instance[Key].IsActive())
                {
                    SkyManager.Instance.Deactivate(Key);
                }
            }
        }
    }
}