using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.Critters.SeaFirefly;

[BestiaryBiome<PollutedOceanBiomeSurface>]
[BestiaryBiome<PollutedOceanBiomeUnderground>]
public class SeaFirefly : ModNPC {
    public override void SetDefaults() {
        NPC.lifeMax = 5;
        NPC.width = 14;
        NPC.height = 14;
        NPC.friendly = true;
        NPC.noGravity = true;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void AI() {
        base.AI();
        if (NPC.wet) {
            NPC.noGravity = true;
            NPC.aiStyle = -1;

            Point above = NPC.Top.ToTileCoordinates();
            Tile topTile = Framing.GetTileSafely(above);
            if (topTile.LiquidAmount < 255) {
                if (NPC.velocity.Y < 2f) {
                    NPC.velocity.Y += 0.02f;
                }
                if (NPC.velocity.Y < 0f) {
                    NPC.velocity *= 0.8f;
                }
            }
            else {
                if (NPC.velocity.Y > -2f) {
                    NPC.velocity.Y -= 0.02f;
                }
            }
            NPC.ai[3] += 0.02f + Main.rand.NextFloat(0.01f);
            NPC.velocity.Y += Helper.Oscillate(NPC.ai[3], -0.01f, 0.02f);
            NPC.velocity.X *= 0.9f;
            NPC.CollideWithOthers();
        }
        else {
            NPC.aiStyle = NPCAIStyleID.Piranha;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (!NPC.IsABestiaryIconDummy) {
            Color color = new Color(20, 160, 255, 50) * 0.2f;
            if (Main.getGoodWorld) {
                color = Main.DiscoColor;
            }
            SeaFireflyRenderer.Instance.Enqueue(new SeaFireflyShaderRequest(NPC.Center, NPC.scale * Helper.Oscillate(NPC.whoAmI + Main.GlobalTimeWrappedHourly, 1f, 1.25f), color));
        }

        var draw = NPC.GetDrawInfo();
        Texture2D texture = draw.Texture;
        Rectangle frame = texture.Frame(2, 6, 0, (int)(Main.GameUpdateCount / 6 % 6));
        spriteBatch.Draw(texture, draw.Position - screenPos, frame, Color.White, 0f, frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.Bloom, draw.Position - screenPos, null, new Color(5, 5, 40, 0), 0f, AequusTextures.Bloom.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        return false;
    }
}
