using Aequus.Core.Collections;
using Aequus.Core.UI;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ObjectData;
using Terraria.UI;

namespace Aequus.Old.Content.Equipment.Accessories.HoloLens;

public class ChestLensInterface : UILayer {

    public static readonly DictionaryRemoveQueue<Point, ChestLensInfo> ChestLens = new();

    public override void OnUnload() {
        ChestLens.Clear();
    }

    public override void OnPreUpdatePlayers() {
        if (!Main.LocalPlayer.GetModPlayer<AequusPlayer>().accHoloLens) {
            this.Deactivate();
        }
    }

    public override bool OnUIUpdate(GameTime gameTime) {
        var mousePoint = Point.Zero;
        if (WorldGen.InWorld(Player.tileTargetX, Player.tileTargetY, 30) && Main.tileContainer[Main.tile[Player.tileTargetX, Player.tileTargetY].TileType]) {
            if (Main.LocalPlayer.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.QuickStackToNearbyChests)) {
                var tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
                mousePoint = new Point(Player.tileTargetX - tile.TileFrameX % 36 / 18, Player.tileTargetY - tile.TileFrameY % 36 / 18);
                if (!ChestLens.ContainsKey(mousePoint)) {
                    ChestLens.Add(mousePoint, new ChestLensInfo(mousePoint));
                }
            }
        }

        if (ChestLens.Count <= 0) {
            return true;
        }

        var removeCache = new List<Point>();
        foreach (var lens in ChestLens.Values) {
            if (!lens.EnsureChestState() || !lens.Update(gameTime, mousePoint)) {
                removeCache.Add(lens.point);
                break;
            }
        }
        foreach (var p in removeCache) {
            ChestLens.Remove(p);
        }

        return true;
    }

    protected override bool DrawSelf() {
        if (ChestLens.Count <= 0) {
            return true;
        }

        foreach (var lens in ChestLens.Values) {
            if (!lens.EnsureChestState()) {
                break;
            }
            lens.Draw(Main.spriteBatch);
        }
        return true;
    }

    public override void OnRemove() {
        ChestLens.Clear();
    }

    public ChestLensInterface() : base("HoloLens", InterfaceLayerNames.EntityHealthBars_16, InterfaceScaleType.Game) { }

    public class ChestLensInfo {
        public Point point;
        public Vector2 drawPoint;
        public float timer;

        public ChestLensInfo(Point point) {
            timer = 0f;
            this.point = point;
            drawPoint = point.ToWorldCoordinates(0f, 0f);
            var tileObjectData = TileObjectData.GetTileData(Main.tile[point]);
            if (tileObjectData != null) {
                drawPoint.X += tileObjectData.Width * 16f / 2f;
            }
        }

        public bool EnsureChestState() {
            return Chest.FindChest(point.X, point.Y) != -1;
        }

        public bool Update(GameTime gameTime, Point mousePoint) {
            int chestID = Chest.FindChest(point.X, point.Y);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                if (chestID == -1) {
                    return false;
                }
                var c = Main.chest[chestID];
                if (!c.IsSynced() || Main.GameUpdateCount % 30 == 0) {
                    Aequus.GetPacket<PacketRequestChestItems>().Send(Main.myPlayer, chestID);
                    return true;
                }
            }
            if (point == mousePoint && (!(Main.LocalPlayer.chestX == point.X && Main.LocalPlayer.chestY == point.Y) || Main.LocalPlayer.chest == -1)) {
                if (timer < 5f) {
                    timer += 1f;
                }
            }
            else {
                timer -= 1f;
            }
            return timer > 0f;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (timer <= 0f) {
                return;
            }

            int chestID = Chest.FindChest(point.X, point.Y);
            if (chestID == -1) {
                return;
            }

            var c = Main.chest[chestID];
            float opacity = (float)Math.Pow(timer / 5f, 2f);
            float inventoryScale = Main.inventoryScale;
            var texture = TextureAssets.InventoryBack13.Value;
            Main.inventoryScale = (0.4f + opacity * 0.2f) * Main.UIScale;
            var separation = new Vector2(texture.Width * Main.inventoryScale + 2f, texture.Height * Main.inventoryScale + 2f);
            int ten = 10;
            var topLeft = drawPoint - new Vector2(separation.X * ten / 2f, separation.Y * (Chest.maxItems / ten));
            for (int i = 0; i < Chest.maxItems; i++) {
                int x = i % ten;
                int y = i / ten;

                var drawCoords = topLeft + new Vector2(separation.X * x, separation.Y * y) - Main.screenPosition;
                var clr = Main.mouseColor.HueAdd(GetHueForItem(c.item[i]));
                Main.spriteBatch.Draw(texture, drawCoords, null, (clr * 0.2f) with { A = 255 } * opacity * 0.66f, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(TextureAssets.InventoryBack16.Value, drawCoords, null, clr * opacity * 0.66f, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
                ItemSlotDrawHelper.DrawSimple(spriteBatch, c.item[i], drawCoords + texture.Size() / 2f * Main.inventoryScale, ItemSlot.Context.ChestItem, color: Color.White * opacity, maxSize: 20);
            }
            Main.inventoryScale = inventoryScale;
        }

        public float GetHueForItem(Item item) {
            if (item.shoot > ProjectileID.None) {
                if (Main.projPet[item.shoot]) {
                    if (item.buffType > 0 && Main.lightPet[item.buffType]) {
                        return 0.3f;
                    }
                    return 0.25f;
                }
            }
            if (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0)
                return 0.9f;
            if (item.pick > 0)
                return 0.8f;
            if (item.hammer > 0)
                return 0.7f;
            if (item.axe > 0)
                return 0.6f;
            if (item.damage > 0) {
                if (item.ammo > 0)
                    return 0.7f;
                if (item.DamageType == null)
                    return 0.1f;
                if (item.DamageType.CountsAsClass(DamageClass.Melee))
                    return 0.2f;
                if (item.DamageType.CountsAsClass(DamageClass.Ranged))
                    return 0.3f;
                if (item.DamageType.CountsAsClass(DamageClass.Magic))
                    return 0.4f;
                if (item.DamageType.CountsAsClass(DamageClass.Summon))
                    return 0.5f;
                if (item.DamageType.CountsAsClass(Aequus.NecromancyClass))
                    return 0.5f;
                return 0.1f;
            }
            if (item.shoot > ProjectileID.None)
                return 0.5f;
            if (item.buffType > 0 && item.buffTime > 0)
                return 0.4f;
            if (item.makeNPC > 0)
                return 0.25f;
            if (item.createTile >= TileID.Dirt || item.tileWand > -1)
                return 0.2f;
            if (item.createWall >= WallID.None)
                return 0.1f;
            if (item.accessory) {
                if (item.defense > 0)
                    return 0.65f;
                return 0.6f;
            }
            if (item.material)
                return 0.3f;
            return 0f;
        }
    }
}