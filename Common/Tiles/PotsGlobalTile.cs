using Aequus.Content.DataSets;
using Aequus.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Common.Tiles;

public class PotsGlobalTile : GlobalTile {
    public override bool? IsTileDangerous(int i, int j, int type, Player player) {
        if (!TileSets.IsSmashablePot.Contains(type)) {
            return null;
        }

        i -= Main.tile[i, j].TileFrameX % 36 / 18;
        j -= Main.tile[i, j].TileFrameY % 36 / 18;

        if (!PotsSystem.LootPreviews.TryGetValue(new(i, j), out var value)) {
            return null;
        }

        return (!value.Dangerous) ? null : true;
    }

    public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch) {
        if (!TileSets.IsSmashablePot.Contains(type)) {
            return;
        }

        int frameWidth = 36;
        int frameHeight = 36;
        var tileObjectData = TileObjectData.GetTileData(Main.tile[i, j]);
        if (tileObjectData != null) {
            frameWidth = tileObjectData.CoordinateFullWidth;
            frameHeight = tileObjectData.CoordinateFullHeight;
        }
        if (Main.tile[i, j].TileFrameX % frameWidth != 0 || Main.tile[i, j].TileFrameY % frameHeight != 0) {
            return;
        }

        var point = new Point(i, j);
        if (PotsSystem.LootPreviews.TryGetValue(point, out var preview)) {
            return;
        }

        if (PotsSystem.InPotSightRange(Main.LocalPlayer, point, Main.LocalPlayer.GetModPlayer<AequusPlayer>().potSightRange)) {
            GoreDisabler.Begin();
            NewItemCache.Begin();
            NewProjectileCache.Begin();
            NewNPCCache.Begin();

            int x2 = 0;
            int y2 = j;
            for (x2 += Main.tile[i, j].TileFrameX / 18; x2 > 1; x2 -= 2) {
            }
            x2 *= -1;
            x2 += i;
            int num3 = Main.tile[i, j].TileFrameY / 18;
            int style = 0;
            while (num3 > 1) {
                num3 -= 2;
                style++;
            }
            y2 -= num3;

            int oldNetmode = Main.netMode;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                Main.netMode = NetmodeID.Server;
            }
            try {
                PotsSystem.KillTile_DropItems.Invoke(null, new object[] { i, j, Main.tile[i, j], true, true });
                if (Main.tile[i, j].TileType < TileID.Count) {
                    PotsSystem.SpawnThingsFromPot.Invoke(null, new object[] { i, j, x2, y2, style });
                }
                else if (tileObjectData != null && tileObjectData.Width == 1 && tileObjectData.Height == 1) {
                    TileLoader.GetItemDrops(i, j, Main.tile[i, j]);
                }
            }
            catch {
            }
            finally {
                Main.netMode = oldNetmode;
            }

            NewNPCCache.End();
            NewProjectileCache.End();
            NewItemCache.End();
            GoreDisabler.End();

            PotsSystem.PotLootPreview newPreview;
            if (NewNPCCache.NPCs.Count > 0) {
                int npcType = NewNPCCache.NPCs[0].type;
                Main.instance.LoadNPC(npcType);
                var texture = TextureAssets.Npc[npcType];
                newPreview = new(texture.Value, texture.Frame(verticalFrames: Main.npcFrameCount[npcType], frameY: 0), Stack: NewNPCCache.NPCs.Count, Dangerous: !ContentSamples.NpcsByNetId[npcType].friendly && ContentSamples.NpcsByNetId[npcType].damage > 0);
                if (NewNPCCache.NPCs[0].color != Color.Transparent) {
                    newPreview.NPCColor = NewNPCCache.NPCs[0].color;
                }
            }
            else if (NewProjectileCache.Projectiles.Count > 0) {
                int projectileType = NewProjectileCache.Projectiles[0].type;
                Main.instance.LoadProjectile(projectileType);
                var texture = TextureAssets.Projectile[projectileType];
                newPreview = new(texture.Value, texture.Frame(verticalFrames: Main.projFrames[projectileType], frameY: 0), Stack: NewProjectileCache.Projectiles.Count, Dangerous: !ContentSamples.ProjectilesByType[projectileType].friendly || ContentSamples.ProjectilesByType[projectileType].hostile || ContentSamples.ProjectilesByType[projectileType].aiStyle == ProjAIStyleID.Explosive);
            }
            else if (NewItemCache.DroppedItems.Count > 0) {
                int itemType = NewItemCache.DroppedItems[0].type;
                Main.GetItemDrawFrame(itemType, out var itemTexture, out var itemFrame);
                newPreview = new(itemTexture, itemFrame, Stack: NewItemCache.DroppedItems[0].stack, Dangerous: false);
            }
            else {
                newPreview = new(TextureAssets.Cd.Value, null, Stack: 1, Dangerous: true);
            }
            PotsSystem.LootPreviews.Add(point, newPreview);
        }
    }
}