using Aequus;
using Aequus.Common.Net;
using Aequus.Common.Rendering.Tiles;
using Aequus.Common.Tiles;
using Aequus.Particles.Dusts;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Tiles.MossCaves.ArgonJumpshroom;
public class JumpMushroom : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<JumpMushroomTile>());
    }
}

public class JumpMushroomTile : ModTile, ISpecialTileRenderer, TileHooks.IUniqueTileInteractions {
    public class JumpMushroomDrawData {
        public float time;
        public Vector2 PlayerVelocity;
        public readonly int X;
        public readonly int Y;

        public JumpMushroomDrawData(int x, int y, Vector2 playerVelocity) {
            X = x;
            Y = y;
            PlayerVelocity = playerVelocity;
            time = 0f;
        }
    }

    public static List<JumpMushroomDrawData> drawData;

    public override void Load() {
        drawData = new List<JumpMushroomDrawData>();
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileNoFail[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 18, };
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
        TileObjectData.addTile(Type);
        AddMapEntry(new Color(148, 0, 132), CreateMapEntryName());
        HitSound = SoundID.Dig;
        DustType = DustID.ArgonMoss;
        if (!Main.dedServ) {
            SpecialTileRenderer.UpdateTileEffects += () => UpdateDrawData();
            SpecialTileRenderer.ClearTileEffects += () => drawData.Clear();
        }
    }

    public static void UpdateDrawData() {
        for (int i = 0; i < drawData.Count; i++) {
            drawData[i].time++;
            if (drawData[i].time > 160) {
                drawData.RemoveAt(i);
                i--;
                continue;
            }
        }
    }

    public override void Unload() {
        drawData?.Clear();
        drawData = null;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 1f;
        g = 0.2f;
        b = 0.66f;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayer.PreDrawVines);
        }
        return false;
    }

    public void CheckInteractions(int i, int j) {
        // Update velocity effects on the client, and trust vanilla to sync the player
        // Checked only when the tile is nearby for efficiency reasons
        if (Main.tile[i, j].TileFrameX != 0 || Main.tile[i, j].TileFrameY != 0)
            return;

        var player = Main.LocalPlayer;
        if (player.velocity.Y < 4f)
            return;
        if ((int)(player.position.Y + player.height) / 16 != j)
            return;
        int k = (int)(player.position.X + player.width / 2) / 16;
        for (int x = k - 2; x <= k + 2; x++) {
            if (x == i) {
                Interact(player, i, j);
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
                    PacketSystem.Get<PacketUniqueTileInteraction>().Send(player, i, j);
                }
            }
        }
    }

    void ISpecialTileRenderer.Render(int i, int j, byte layer) {
        if (Aequus.GameWorldActive) {
            CheckInteractions(i, j);
        }
        var floor = new Vector2(i * 16f + 8f, j * 16f + 32f);
        var texture = TextureAssets.Tile[Type].Value;
        var frame = texture.Frame(verticalFrames: 2);
        Main.spriteBatch.Draw(texture, floor - Main.screenPosition, frame.Frame(frameX: 0, frameY: 1), Helper.GetColor(floor),
            0f, new Vector2(frame.Width / 2f, frame.Height - 4f), 1f, SpriteEffects.None, 0f);
        var capPosition = floor + new Vector2(0f, -24f);
        float rotation = 0f;
        var playerVelocity = Vector2.Zero;
        float intensity = 0f;
        float maxIntensity = 4f;
        foreach (var d in drawData) {
            if (d.X == i && d.Y == j) {
                playerVelocity = d.PlayerVelocity;
                intensity = maxIntensity - d.time / 40f;
                float movementIntensity = intensity * Math.Clamp(d.PlayerVelocity.Y / -6f, 1f, 2f);
                float wave = MathF.Sin(d.time / 6f);
                if (wave > 0f) {
                    capPosition.Y += wave * movementIntensity * 1.5f;
                }
                else {
                    capPosition.Y += wave * movementIntensity * 0.65f;
                }
                rotation = movementIntensity * d.PlayerVelocity.X * 0.005f * Math.Max((-wave + 1f) / 2f, 0f);
                break;
            }
        }
        Main.spriteBatch.Draw(texture, capPosition - Main.screenPosition, frame, Helper.GetColor(capPosition),
            rotation, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        float jumpAnimation = intensity - (maxIntensity - 1f);
        if (jumpAnimation > 0f) {
            for (int k = 0; k < 3 && jumpAnimation <= 1f; k++) {
                Main.spriteBatch.Draw(TextureAssets.Extra[174].Value, floor + new Vector2(0f, -24f) - Main.screenPosition + playerVelocity * MathF.Pow(1f - jumpAnimation, 1.33f) * 26f, null, new Color(255, 100, 200, 0) * 0.6f * jumpAnimation,
                    playerVelocity.ToRotation() + MathHelper.PiOver2, TextureAssets.Extra[174].Value.Size() / 2f, new Vector2(1.22f, 0.35f) * jumpAnimation, SpriteEffects.None, 0f);
                jumpAnimation += 0.06f;
            }
        }
    }

    public void SetInteractAnimation(Player player, int i, int j) {
        for (int k = 0; k < drawData.Count; k++) {
            if (drawData[k].X == i && drawData[k].Y == j) {
                drawData[k].time = 0;
                drawData[k].PlayerVelocity = player.velocity;
                return;
            }
        }
        drawData.Add(new JumpMushroomDrawData(i, j, player.velocity));
    }

    public void Interact(Player player, int i, int j) {
        player.velocity.X += Math.Sign(player.velocity.X) * 2.5f;
        player.velocity.Y = Math.Min(-player.velocity.Y + Math.Max((player.fallStart2 - (int)player.position.Y / 16) / 5f, -10f), -5f);
        SoundEngine.PlaySound(AequusSounds.jumpshroom with { Volume = 0.5f, }, player.Center);
        var color = new Color(255, 140, 190, 160);
        for (int k = 0; k < 14; k++) {
            var d = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<MonoSparkleDust>(), newColor: color);
            d.velocity += player.velocity * Main.rand.NextFloat(0.1f, 1f);
            d.fadeIn = d.scale + 1f;
        }
        for (int k = 0; k < 7; k++) {
            var d = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<MonoSparkleDust>(), newColor: color);
            d.velocity.Y += 5f;
            d.velocity *= 0.33f;
            d.velocity.Y *= k / 7f;
        }
        if (Main.netMode != NetmodeID.Server) {
            SetInteractAnimation(player, i, j);
        }
        else {
            PacketSystem.Get<PacketUniqueTileInteraction>().Send(player, i, j);
        }
    }
}