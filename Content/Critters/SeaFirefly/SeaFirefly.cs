using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Graphics;
using Aequus.Core.Particles;
using System;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Critters.SeaFirefly;

[WorkInProgress]
[BestiaryBiome<PollutedOceanBiomeSurface>]
[BestiaryBiome<PollutedOceanBiomeUnderground>]
public class SeaFirefly : UnifiedCritter, DrawLayers.IDrawLayer {
    public override int BestiaryCritterSort => NPCID.Firefly;

    #region States
    public const int Initialization = 0;
    public const int Normal = 1;
    #endregion

    #region Properties
    public const int HorizontalFrames = 8;
    public static readonly int LightTime = 1800;
    public static readonly int DarkTime = 240;
    public static readonly int CycleTime = LightTime + DarkTime;

    public bool IsLit => WetTimer % CycleTime > DarkTime;

    public int State { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
    public float WetTimer { get => NPC.ai[1]; set => NPC.ai[1] = value; }
    public float LightOpacity { get => NPC.localAI[0]; set => NPC.localAI[0] = value; }
    #endregion

    #region Initialization
    public override void OnLoad() {
        SeaFireflyRegistry.LoadAll(this);
    }

    public override void OnSetStaticDefaults() {
        Main.npcFrameCount[Type] = 2;
    }

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

    public override void OnSpawn(IEntitySource source) {
        if (Main.remixWorld) {
            color = SeaFireflyRegistry.RainbowIndex;
        }
        if (FromCritterItem(source, out Item item) && item.ModItem is SeaFireflyItem fireflyItem) {
            color = fireflyItem.Color;
        }
    }
    #endregion

    #region AI
    public override void AI() {
        switch (State) {
            case Initialization:
                AI_Initalize();
                goto default;

            default:
                if (NPC.wet) {
                    if (IsLit) {
                        Lighting.AddLight(NPC.Center, Current.GetLightColor(NPC.Center));
                    }

                    if (NPC.direction == 0) {
                        NPC.TargetClosest(faceTarget: true);
                    }

                    NPC.noGravity = true;
                    NPC.aiStyle = -1;
                    WetTimer++;

                    AI_WetMovement();
                    NPC.CollideWithOthers();

                    if (Main.netMode != NetmodeID.Server) {
                        AI_SpawnFireflies();

                        if (IsLit) {
                            LightOpacity += 0.1f;
                            if (LightOpacity > 1f) {
                                LightOpacity = 1f;
                            }
                        }
                        else if (LightOpacity > 0f) {
                            LightOpacity -= 0.1f;
                            if (LightOpacity < 0f) {
                                LightOpacity = 0f;
                            }
                        }
                    }
                }
                else {
                    AI_DryMovement();
                }
                break;
        }

        NPC.rotation = NPC.velocity.X * 0.2f;
        NPC.spriteDirection = NPC.direction;
    }

    void AI_Initalize() {
        if (NPC.ai[2] == 0f) {
            NPC.ai[2] = 1f;
            //IEntitySource mySource = NPC.GetSource_FromThis();
            //int amount = Main.rand.Next(8);
            //for (int i = 0; i < amount; i++) {
            //    Point spawnCoordinates = (NPC.Center + Main.rand.NextVector2Unit() * 4f).ToPoint();
            //    NPC.NewNPC(mySource, spawnCoordinates.X, spawnCoordinates.Y, Type, NPC.whoAmI, ai2: NPC.ai[2]);
            //}
            foreach (SeaFireflyItem modItem in ModContent.GetContent<SeaFireflyItem>()) {
                if (modItem.Color == color) {
                    NPC.catchItem = modItem.Type;
                }
            }
        }
    }

    void AI_SpawnFireflies() {
        if (Cull2D.Rectangle(NPC.Hitbox, 128, 128) && Main.rand.NextBool(15)) {
            int frame = Main.rand.NextBool(12) ? 1 : Main.rand.Next(2, 4);
            Particle<SeaFireflyClusters.Particle>.New().Setup(NPC.Center, (byte)frame, (int)WetTimer, color);
        }
    }

    void AI_WetMovement() {
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
            if (NPC.velocity.Y > 0f) {
                NPC.velocity.Y *= 0.9f;
            }
        }

        NPC.velocity.Y += Helper.Oscillate(WetTimer * 0.02f, -0.004f, 0.004f);

        if (NPC.collideX) {
            NPC.direction = -NPC.direction;
            NPC.velocity.X = -NPC.oldVelocity.X;
        }
        NPC.velocity.X += NPC.direction * 0.02f;
        if (Math.Abs(NPC.velocity.X) > 0.75f) {
            NPC.velocity.X *= 0.95f;
        }
        if (Math.Abs(NPC.velocity.Y) > 0.5f) {
            NPC.velocity.X *= 0.9f;
        }
    }

    void AI_DryMovement() {
        WetTimer = 0f;
        if (NPC.velocity.Y == 0f) {
            NPC.velocity.Y = -3f;

            NPC.velocity.X += Main.rand.NextFloat(-3f, 3f);
            NPC.direction = Math.Sign(NPC.velocity.X);
        }

        NPC.velocity.Y += 0.3f;
        NPC.velocity.X *= 0.94f;
    }
    #endregion

    #region Drawing
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.IsABestiaryIconDummy) {
            DrawSeaFirefly(spriteBatch, screenPos);
        }
        else {
            DrawLayers.Instance.PostDrawLiquids += NPC;
        }
        return false;
    }

    void DrawLayers.IDrawLayer.DrawOntoLayer(SpriteBatch spriteBatch, DrawLayers.DrawLayer layer) {
        var draw = NPC.GetDrawInfo();
        Color drawColor = NPC.GetNPCColorTintedByBuffs(ExtendLight.Get(NPC.Center));

        DrawSeaFirefly(spriteBatch, Main.screenPosition);
    }

    private void DrawSeaFirefly(SpriteBatch spriteBatch, Vector2 screenPos) {
        var draw = NPC.GetDrawInfo();
        Color drawColor = NPC.IsABestiaryIconDummy ? Color.White : NPC.GetNPCColorTintedByBuffs(ExtendLight.Get(NPC.Center));
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        DrawSeaFirefly(Current, spriteBatch, draw.Position, screenPos, drawColor, NPC.Opacity, NPC.IsABestiaryIconDummy ? 0 : LightOpacity, NPC.rotation, effects, NPC.scale, 0, NPC.whoAmI, IsLit && !NPC.IsABestiaryIconDummy);
    }

    public static void DrawSeaFirefly(ISeaFireflyInstanceData colorVariant, SpriteBatch spriteBatch, Vector2 worldPosition, Vector2 screenPosition, Color drawColor, float globalOpacity, float lightOpacity, float rotation, SpriteEffects effects, float scale = 1f, int frame = 0, int randomSeed = 0, bool isLit = false) {
        Texture2D texture = AequusTextures.SeaFirefly;
        Rectangle frameRect = texture.Frame(HorizontalFrames, 2, frame, isLit ? 1 : 0);
        Vector2 origin = frameRect.Size() / 2f;
        Vector2 drawCoordinates = worldPosition - screenPosition;

        lightOpacity *= globalOpacity;

        if (lightOpacity > 0f && frame < 5) {
            float wave = Helper.Oscillate(randomSeed + worldPosition.X * 0.01f + Main.GlobalTimeWrappedHourly * 4f, 1f);

            Color color = colorVariant.GetGlowColor(new GlowColorContext(worldPosition, randomSeed));
            float waveScale = Helper.Oscillate(randomSeed + Main.GlobalTimeWrappedHourly, 0.4f, 0.8f);
            SeaFireflyRenderer.Instance.Enqueue(new SeaFireflyShaderRequest(worldPosition, scale * waveScale * lightOpacity, color * 0.5f));
        }

        if (isLit && frame < 7) {
            spriteBatch.Draw(AequusTextures.Bloom, drawCoordinates, null, colorVariant.GetBugColor() * 0.01f * globalOpacity, 0f, AequusTextures.Bloom.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        spriteBatch.Draw(texture, drawCoordinates, frameRect, drawColor * globalOpacity * 0.75f, rotation, origin, scale, effects, 0f);
        spriteBatch.Draw(texture, drawCoordinates, frameRect.Frame(1, 0), Color.White * globalOpacity, rotation, origin, scale, effects, 0f);
    }
    #endregion

    #region IO
    public const string Tag_Color = "color";

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(color);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        color = reader.ReadByte();
    }

    public override void SaveData(TagCompound tag) {
        if (color > 0) {
            tag[Tag_Color] = color;
        }
    }

    public override void LoadData(TagCompound tag) {
        tag.TryGet(Tag_Color, out color);
    }
    #endregion

    #region Colors
    public byte color;

    public ISeaFireflyInstanceData Current => SeaFireflyRegistry.GetPalette(color);
    #endregion
}
