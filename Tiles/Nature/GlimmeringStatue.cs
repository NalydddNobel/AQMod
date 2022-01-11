using AQMod.Common.WorldGeneration;
using AQMod.Content.World;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Nature
{
    public class GlimmeringStatue : ModTile
    {
        internal static bool TryGenGlimmeringStatue(int x, int y)
        {
            if (!AQWorldGen.ActiveAndSolid(x, y) && !AQWorldGen.ActiveAndSolid(x - 1, y) && AQWorldGen.ActiveAndSolid(x, y + 1) && AQWorldGen.ActiveAndSolid(x - 1, y + 1) && Main.tile[x, y].wall == WallID.None)
            {
                PlaceUndisoveredGlimmeringStatue(x, y);
                return true;
            }
            return false;
        }

        public static bool PlaceUndisoveredGlimmeringStatue(int x, int y)
        {
            return PlaceUndiscoveredGlimmeringStatue(new Point(x, y));
        }

        public static bool PlaceUndiscoveredGlimmeringStatue(Point p)
        {
            WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<GlimmeringStatue>());
            Tile tile = Main.tile[p.X, p.Y];
            if (tile.type == ModContent.TileType<GlimmeringStatue>())
            {
                var objectData = TileObjectData.GetTileData(tile);
                int x = p.X - tile.frameX % 36 / 18;
                int y = p.Y - tile.frameY / 18;
                x += 1;
                y += 2;
                objectData.HookPostPlaceMyPlayer.hook(x, y, tile.type, objectData.Style, 0);
                int index = ModContent.GetInstance<TEGlimmeringStatue>().Find(x - 1, y - 2);
                if (index == -1)
                {
                    return false;
                }
                TEGlimmeringStatue glimmeringStatue = (TEGlimmeringStatue)TileEntity.ByID[index];
                glimmeringStatue.discovered = false;
                return true;
            }
            return false;
        }

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEGlimmeringStatue>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(75, 139, 166), Lang.GetItemName(ModContent.ItemType<Items.Placeable.Nature.GlimmeringStatue>()));
            dustType = 15;
            disableSmartCursor = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.Nature.GlimmeringStatue>());
            ModContent.GetInstance<TEGlimmeringStatue>().Kill(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<Items.Placeable.Nature.GlimmeringStatue>();
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int x = i - tile.frameX % 36 / 18;
            int y = j - tile.frameY / 18;
            int index = ModContent.GetInstance<TEGlimmeringStatue>().Find(x, y);
            if (index != -1 && !GlimmerEvent.IsGlimmerEventCurrentlyActive() && (PassingDays.daysPassedSinceLastGlimmerEvent <= 1 || Main.rand.NextBool(PassingDays.daysPassedSinceLastGlimmerEvent)))
            {
                int d = Dust.NewDust(new Vector2(x * 16f, y * 16f), 32, 16, dustType);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.2f);
                Main.dust[d].velocity.X *= 0.3f;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 2f);
            }
        }

        public override bool NewRightClick(int i, int j)
        {
            Main.PlaySound(SoundID.Mech, i * 16, j * 16, 0);
            HitWire(i, j);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int x = i - tile.frameX / 18 % 2;
            int y = j - tile.frameY / 18 % 3;
            int index = ModContent.GetInstance<TEGlimmeringStatue>().Find(x, y);
            if (index != -1 && !GlimmerEvent.IsGlimmerEventCurrentlyActive() && PassingDays.daysPassedSinceLastGlimmerEvent > 0)
            {
                int d = Dust.NewDust(new Vector2(x * 16f, y * 16f), 32, 16, dustType);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.2f);
                Main.dust[d].velocity.X *= 0.3f;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 2f);
                //CombatText.NewText(new Rectangle(x * 16, y * 16, 32, 16), GlimmerEvent.stariteProjectileColor, GlimmerEvent.spawnChance, true);
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(x, y);
                Wiring.SkipWire(x + 1, y);
                Wiring.SkipWire(x, y + 1);
                Wiring.SkipWire(x + 1, y + 1);
                Wiring.SkipWire(x, y + 2);
                Wiring.SkipWire(x + 1, y + 2);
            }
        }
    }
}