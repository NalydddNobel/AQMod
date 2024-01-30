using Aequus.Content.DataSets;
using Aequus.Core;
using Terraria.GameContent;

namespace Aequus.Common.Tiles;

public class PotsGlobalTile : GlobalTile {
    public override System.Boolean? IsTileDangerous(System.Int32 i, System.Int32 j, System.Int32 type, Player player) {
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

    public override void PostDraw(System.Int32 i, System.Int32 j, System.Int32 type, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX % 36 != 18 || Main.tile[i, j].TileFrameY % 36 != 18 || !TileSets.IsSmashablePot.Contains(type)) {
            return;
        }

        i--;
        j--;
        var point = new Point(i, j);
        if (PotsSystem.LootPreviews.TryGetValue(point, out var preview)) {
            return;
        }

        if (PotsSystem.InPotSightRange(Main.LocalPlayer, point, Main.LocalPlayer.GetModPlayer<AequusPlayer>().potSightRange)) {
            GoreDisabler.Begin();
            NewItemCache.Begin();
            NewProjectileCache.Begin();
            NewNPCCache.Begin();

            System.Int32 x2 = 0;
            System.Int32 y2 = j;
            for (x2 += Main.tile[i, j].TileFrameX / 18; x2 > 1; x2 -= 2) {
            }
            x2 *= -1;
            x2 += i;
            System.Int32 num3 = Main.tile[i, j].TileFrameY / 18;
            System.Int32 style = 0;
            while (num3 > 1) {
                num3 -= 2;
                style++;
            }
            y2 -= num3;

            PotsSystem.KillTile_DropItems.Invoke(null, new System.Object[] { x2, y2, Main.tile[x2, y2], true, true });
            PotsSystem.SpawnThingsFromPot.Invoke(null, new System.Object[] { i, j, x2, y2, style });

            PotsSystem.PotLootPreview newPreview;
            if (NewNPCCache.NPCs.Count > 0) {
                System.Int32 npcType = NewNPCCache.NPCs[0].type;
                Main.instance.LoadNPC(npcType);
                var texture = TextureAssets.Npc[npcType];
                newPreview = new(texture.Value, texture.Frame(verticalFrames: Main.npcFrameCount[npcType], frameY: 0), Stack: NewNPCCache.NPCs.Count, Dangerous: !ContentSamples.NpcsByNetId[npcType].friendly && ContentSamples.NpcsByNetId[npcType].damage > 0);
                if (NewNPCCache.NPCs[0].color != Color.Transparent) {
                    newPreview.NPCColor = NewNPCCache.NPCs[0].color;
                }
            }
            else if (NewProjectileCache.Projectiles.Count > 0) {
                System.Int32 projectileType = NewProjectileCache.Projectiles[0].type;
                Main.instance.LoadProjectile(projectileType);
                var texture = TextureAssets.Projectile[projectileType];
                newPreview = new(texture.Value, texture.Frame(verticalFrames: Main.projFrames[projectileType], frameY: 0), Stack: NewProjectileCache.Projectiles.Count, Dangerous: !ContentSamples.ProjectilesByType[projectileType].friendly || ContentSamples.ProjectilesByType[projectileType].hostile || ContentSamples.ProjectilesByType[projectileType].aiStyle == ProjAIStyleID.Explosive);
            }
            else if (NewItemCache.DroppedItems.Count > 0) {
                System.Int32 itemType = NewItemCache.DroppedItems[0].type;
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