using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Core.ContentGeneration;
using System;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.Critters.SeaFirefly;

[WorkInProgress]
[BestiaryBiome<PollutedOceanBiomeSurface>]
[BestiaryBiome<PollutedOceanBiomeUnderground>]
public class SeaFirefly : UnifiedCritter {
    public override int BestiaryCritterSort => NPCID.Firefly;

    public override void OnSetDefaults() {
        NPC.width = 8;
        NPC.height = 8;
        NPC.npcSlots = 0.1f;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void SetItemDefaults(Item Item) {
        Item.width = 16;
        Item.height = 16;
        Item.value = Item.sellPrice(silver: 4);
        Item.rare = ItemRarityID.Blue;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void AI() {
        if (NPC.wet) {
            if (NPC.ai[2] == 0f) {
                NPC.ai[2] = 1f;
                IEntitySource mySource = NPC.GetSource_FromThis();
                int amount = Main.rand.Next(8);
                for (int i = 0; i < amount; i++) {
                    Point spawnCoordinates = (NPC.Center + Main.rand.NextVector2Unit() * 4f).ToPoint();
                    NPC.NewNPC(mySource, spawnCoordinates.X, spawnCoordinates.Y, Type, NPC.whoAmI, ai2: NPC.ai[2]);
                }
            }
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
            NPC.velocity.Y += 0.3f;
        }

        Lighting.AddLight(NPC.Center, new Vector3(0.1f, 0.2f, 0.3f));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (!NPC.IsABestiaryIconDummy) {
            Vector2 glowPosition = NPC.Center;
            float wave = Helper.Oscillate(NPC.whoAmI + glowPosition.X * 0.01f + Main.GlobalTimeWrappedHourly * 4f, 1f);

            Color color = Color.Lerp(new Color(30, 90, 255, 50), new Color(40, 255, 255, 50), wave) * 0.5f;
            if (Main.getGoodWorld) {
                color = ExtendColor.HueSet(Color.Red, wave) with { A = 0 } * 0.5f;
            }

            float scale = Helper.Oscillate(NPC.whoAmI + Main.GlobalTimeWrappedHourly, 0.4f, 0.8f);
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
