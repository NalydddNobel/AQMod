using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using System;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.Critters.SeaFirefly;

[BestiaryBiome<PollutedOceanBiomeSurface>]
[BestiaryBiome<PollutedOceanBiomeUnderground>]
public class SeaFirefly : ModNPC {
    public override void SetDefaults() {
        NPC.lifeMax = 5;
        NPC.width = 8;
        NPC.height = 8;
        NPC.npcSlots = 0.1f;
        NPC.friendly = true;
        NPC.noGravity = true;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void AI() {
        base.AI();
        if (NPC.wet) {
            if (NPC.direction == 0) {
                NPC.TargetClosest(faceTarget: true);
            }

            NPC.noGravity = true;
            NPC.aiStyle = -1;

            TileHelper.ScanUp(NPC.Center.ToTileCoordinates(), 3, out Point above, TileHelper.HasNoLiquid);
            Tile topTile = Framing.GetTileSafely(above);
            if (topTile.LiquidAmount < 255) {
                if (NPC.velocity.Y < 2f) {
                    NPC.velocity.Y += 0.02f;
                }
                if (NPC.velocity.Y < 0f) {
                    NPC.velocity.X *= 0.95f;
                }
            }
            else {
                if (NPC.velocity.Y > -1f) {
                    NPC.velocity.Y -= 0.02f;
                }
            }

            NPC.ai[3] += 0.02f;
            NPC.velocity.Y += Helper.Oscillate(NPC.ai[3], -0.004f, 0.004f);

            if (NPC.collideX) {
                NPC.direction = -NPC.direction;
                NPC.velocity.X = -NPC.oldVelocity.X;
            }
            NPC.velocity.X += NPC.direction * 0.02f;
            if (Math.Abs(NPC.velocity.X) > 2f) {
                NPC.velocity.X *= 0.95f;
            }
            if (Math.Abs(NPC.velocity.Y) > 0.5f) {
                NPC.velocity.X *= 0.9f;
            }

            NPC.CollideWithOthers();
        }
        else {
            NPC.aiStyle = NPCAIStyleID.Piranha;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (!NPC.IsABestiaryIconDummy) {
            Vector2 glowPosition = NPC.Center;
            float wave = Helper.Oscillate(glowPosition.X * 0.003f + Main.GlobalTimeWrappedHourly, 1f);

            Color color = Color.Lerp(new Color(30, 90, 255, 50), new Color(40, 255, 255, 50), wave) * 0.5f;
            if (Main.getGoodWorld) {
                color = ExtendColor.HueSet(Color.Red, wave) with { A = 0 } * 0.5f;
            }

            float scale = Helper.Oscillate(NPC.whoAmI + Main.GlobalTimeWrappedHourly, 0.4f, 0.7f);
            SeaFireflyRenderer.Instance.Enqueue(new SeaFireflyShaderRequest(glowPosition, NPC.scale * scale, color));
        }

        var draw = NPC.GetDrawInfo();
        Texture2D texture = draw.Texture;
        Rectangle frame = texture.Frame(2, 6, 0, (int)(Main.GameUpdateCount / 6 % 6));
        spriteBatch.Draw(texture, draw.Position - screenPos, frame, Color.White, 0f, frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.Bloom, draw.Position - screenPos, null, new Color(5, 5, 40, 0), 0f, AequusTextures.Bloom.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        return false;
    }
}
