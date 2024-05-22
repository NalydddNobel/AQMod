using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.Critters.SeaFirefly;

[BestiaryBiome<PollutedOceanBiomeSurface>]
[BestiaryBiome<PollutedOceanBiomeUnderground>]
public class SeaFirefly : ModNPC {
    public override void SetDefaults() {
        NPC.lifeMax = 5;
        NPC.friendly = true;
        NPC.noGravity = true;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void AI() {
        base.AI();
        if (NPC.wet) {
            NPC.aiStyle = NPCAIStyleID.Jellyfish;
        }
        else {
            NPC.aiStyle = NPCAIStyleID.Piranha;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (!NPC.IsABestiaryIconDummy) {
            SeaFireflyRenderer.Instance.Enqueue(new SeaFireflyShaderRequest(NPC.Center, NPC.scale * 2f, new Color(20, 50, 255, 50)));
        }

        var draw = NPC.GetDrawInfo();
        Texture2D texture = draw.Texture;
        Rectangle frame = texture.Frame(2, 6, 0, (int)(Main.GameUpdateCount / 6 % 6));
        spriteBatch.Draw(texture, draw.Position - screenPos, frame, Color.White, 0f, frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.Bloom, draw.Position - screenPos, null, new Color(5, 5, 40, 0), 0f, AequusTextures.Bloom.Size() / 2f, NPC.scale * 2f, SpriteEffects.None, 0f);
        return false;
    }
}
