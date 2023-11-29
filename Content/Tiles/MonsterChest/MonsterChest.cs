using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.MonsterChest;

public class MonsterChest : BaseChest, INetTileInteraction {
    public override int ChestItem => ModContent.ItemType<MonsterChestItem>();

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        AddMapEntry(ColorHelper.ColorFurniture, TextHelper.GetDisplayName<MonsterChestItem>());
        TileID.Sets.HasOutlines[Type] = false;
    }

    public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual) {
        return true;
    }

    public override bool CheckLocked(int i, int j, int left, int top, Player player) {
        return false;
    }

    public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual) {
        dustType = DustID.Copper;
        return true;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var lightColor = Lighting.GetColor(i, j);
        if ((Main.tile[i, j].TileFrameX % 72) != 36 || Main.tile[i, j].TileFrameY != 0) {
            spriteBatch.Draw(TextureAssets.Tile[Type].Value, new Vector2(i, j).ToWorldCoordinates(0f, 0f) - Main.screenPosition + TileHelper.DrawOffset, new(Main.tile[i, j].TileFrameX / 18 * 20, Main.tile[i, j].TileFrameY / 18 * 20, 18, 18), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            return false;
        }

        if (Aequus.GameWorldActive && Lighting.GetColor(i, j).ToVector3().Length() > 0.1f) {
            if (Main.rand.NextBool(2)) {
                var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 32, 32, DustID.Torch, Scale: Main.rand.NextFloat(0.7f, 1f));
                d.velocity *= Main.rand.NextFloat(0.1f);
                d.rotation = 0f;
                d.noGravity = true;
                d.noLight = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.3f, 0.8f);
            }

            var worldLocation = new Vector2(i * 16f + 16f, j * 16f + 16f);
            if (!Main.LocalPlayer.invis && Main.LocalPlayer.Distance(worldLocation) < 200f - Main.LocalPlayer.aggro) {
                TrySummonEnemies(i, j);
            }
        }

        spriteBatch.Draw(TextureAssets.Tile[Type].Value, new Vector2(i, j).ToWorldCoordinates(0f, 0f) - Main.screenPosition + TileHelper.DrawOffset, new(Main.tile[i, j].TileFrameX / 18 * 20, Main.tile[i, j].TileFrameY / 18 * 20, 18, 18), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        return false;
    }

    public static bool TrySummonEnemies(int i, int j) {
        if (NPC.AnyNPCs(ModContent.NPCType<MonsterChestSummon>())) {
            return false;
        }

        int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        int top = j - Main.tile[i, j].TileFrameY % 36 / 18;

        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ModContent.GetInstance<TileInteractionPacket>().Send(i, j);
            return true;
        }

        for (int k = 0; k < 3; k++) {
            NPC.NewNPC(new EntitySource_TileUpdate(left, top), left * 16 + 16, top * 16 + 32, ModContent.NPCType<MonsterChestSummon>(), ai2: -16 * k);
        }
        return true;
    }

    public override void HitWire(int i, int j) {
        TrySummonEnemies(i, j);
    }

    public void Receive(int i, int j, BinaryReader binaryReader, int sender) {
        TrySummonEnemies(i, j);
    }
}