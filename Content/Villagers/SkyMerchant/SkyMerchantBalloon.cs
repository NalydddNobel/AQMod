
namespace Aequus.Content.Villagers.SkyMerchant;

public class SkyMerchantBalloon : ModNPC {
    public override string Texture => AequusTextures.Balloon.FullPath;

    public int Parent { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }

    public SkyMerchant.TextureDrawSet drawSet;

    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = true,
        };
    }

    public override void SetDefaults() {
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = -1;
        NPC.lifeMax = 250;
        NPC.defense = 10;
        NPC.friendly = true;
        //NPC.hide = true;
        NPC.behindTiles = true;
        NPC.dontTakeDamage = true;
    }

    public override void AI() {
        NPC parent = Main.npc[Parent];
        if (!parent.active || parent.ModNPC is not SkyMerchant skyMerchant) {
            NPC.velocity.Y -= 0.1f;
            NPC.noGravity = true;
            NPC.alpha += 10;

            if (NPC.alpha >= 255) {
                NPC.KillEffects();
            }
            return;
        }

        drawSet = skyMerchant.drawSet;
    }

    public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) {
        boundingBox.Width -= 30;
        boundingBox.X += 15;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        NPC parent = Main.npc[Parent];
        if (!parent.active || parent.ModNPC is not SkyMerchant skyMerchant || !drawSet.Valid) {
            return false;
        }

        var drawCoordinates = NPC.Center - screenPos + new Vector2(0f, DrawOffsetY + NPC.gfxOffY + 8f);
        float opacity = NPC.Opacity;
        spriteBatch.Draw(drawSet.Balloon.Value, drawCoordinates + SkyMerchant.BalloonOffset * NPC.scale, null, drawColor * opacity, NPC.rotation, drawSet.Balloon.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(drawSet.Basket.Value, drawCoordinates, null, drawColor * opacity, NPC.rotation, drawSet.Basket.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        return false;
    }
}
