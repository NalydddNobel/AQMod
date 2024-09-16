using Aequus.Common.Items;
using Aequus.Common.Items.Dedications;
using Aequus.Systems.Shimmer;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Renderers;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Dedicated;

public class DedicatedFaeling : ModNPC {
    public const int FRAMES_VERTICAL = 5;
    public const int FRAMES_HORIZONTAL = 4;

    public int _catchItemId;

    public override string Texture => AequusTextures.NPC(NPCID.Shimmerfly);

    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = true,
        };
        NPCID.Sets.CountsAsCritter[Type] = true;
        NPCID.Sets.TrailingMode[Type] = NPCID.Sets.TrailingMode[NPCID.Shimmerfly];
        NPCID.Sets.TrailCacheLength[Type] = NPCID.Sets.TrailCacheLength[NPCID.Shimmerfly];
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Shimmerfly);
        NPC.dontTakeDamage = true;
        AIType = NPCID.Shimmerfly;
        AnimationType = NPCID.Shimmerfly;
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_Parent parentSource) {
            Item item = null;
            if (parentSource.Entity is Player player) {
                item = player.HeldItemFixed();
            }
            else if (parentSource.Entity is Item sourceItem) {
                item = sourceItem;
            }

            if (item?.ModItem is FaelingItem faeling) {
                _catchItemId = item.type;
            }
            else if (DedicationRegistry.TryGet(item.type, out var info)) {
                _catchItemId = info.Faeling.Type;
            }
        }
    }

    public override void AI() {
        NPC.catchItem = _catchItemId;
        if (ItemLoader.GetItem(_catchItemId) is FaelingItem faeling) {
            SpawnSparkles(NPC.Center, faeling._dedication);
        }
    }

    public override void OnKill() {
    }

    // Too lazy to transcribe every local variable
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (ItemLoader.GetItem(_catchItemId) is not FaelingItem faeling) {
            return false;
        }

        Texture2D npcTexture = TextureAssets.Npc[Type].Value;
        Vector2 drawCoordinates = NPC.Center - screenPos;
        float hue = Main.rgbToHsl(faeling._dedication.FaelingColor).X;
        Color glowColor = Main.hslToRgb(hue, 1f, 0.65f);
        glowColor.A /= 2;
        var spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Color white = Color.White;
        white.A /= 2;

        float rotation = NPC.rotation;
        Rectangle rectangle = npcTexture.Frame(FRAMES_HORIZONTAL, FRAMES_VERTICAL, frameY: NPC.frame.Y);
        Vector2 origin = rectangle.Size() / 2f;
        float scale = NPC.scale;
        Rectangle bloomFrame = npcTexture.Frame(FRAMES_HORIZONTAL, FRAMES_VERTICAL, frameX: 2);
        Color color2 = new Color(255, 255, 255, 0) * 1f;

        DrawFaelingTrail(spriteBatch, npcTexture, drawCoordinates, hue, origin, scale, spriteEffects);

        spriteBatch.Draw(npcTexture, drawCoordinates, bloomFrame, color2, rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(FRAMES_HORIZONTAL, FRAMES_VERTICAL, frameX: 1, frameY: NPC.frame.Y), white, rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, rectangle, glowColor, rotation, origin, scale, spriteEffects, 0f);
        float colorMultiplier = MathHelper.Clamp((float)Math.Sin(Main.timeForVisualEffects / 60.0) * 0.3f + 0.3f, 0f, 1f);
        float flashScale = 0.8f + (float)Math.Sin(Main.timeForVisualEffects / 15.0 * MathHelper.TwoPi) * 0.3f;
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(FRAMES_HORIZONTAL, FRAMES_VERTICAL, 3, NPC.whoAmI % FRAMES_VERTICAL), Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.5f) * colorMultiplier, rotation, origin, scale * flashScale, 0, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(FRAMES_HORIZONTAL, FRAMES_VERTICAL, 3, 1), Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.5f) * colorMultiplier, rotation, origin, scale * flashScale, 0, 0f);
        return false;
    }

    private void DrawFaelingTrail(SpriteBatch sb, Texture2D texture, Vector2 drawCoordinates, float hue, Vector2 origin, float scale, SpriteEffects spriteEffects) {
        int trailLength = NPC.oldPos.Length;
        int trailStart = trailLength - 6;
        int trailEnd = 5;
        int trailSubtract = 3;

        float rotation = NPC.rotation;

        Rectangle bloomFrame = texture.Frame(FRAMES_HORIZONTAL, FRAMES_VERTICAL, frameX: 2);

        for (int i = trailStart; i >= trailEnd; i -= trailSubtract) {
            Vector2 trailOffset = NPC.oldPos[i] - NPC.position;
            float num6 = Utils.Remap(i, 0f, trailLength, 1f, 0f);
            float num7 = 1f - num6;
            Vector2 spinningpoint = new Vector2((float)Math.Sin((double)(NPC.whoAmI / 17f) + Main.timeForVisualEffects / 9f + (double)(num6 * 2f * ((float)Math.PI * 2f))) * 16f, -32f) * num7;
            Vector2 val = trailOffset;
            trailOffset = val + spinningpoint.RotatedBy(MathHelper.Pi);
            Color color3 = Main.hslToRgb((hue + num7 * 0.5f) % 1f, 1f, 0.5f) with { A = 0 };
            sb.Draw(texture, drawCoordinates + trailOffset, bloomFrame, color3 * num6 * 0.16f, rotation, origin, scale * Utils.Remap(num6 * num6, 0f, 1f, 0f, 2.5f), spriteEffects, 0f);
        }
    }

    public override void SaveData(TagCompound tag) {
        if (_catchItemId >= ItemID.Count) {
            tag["DedicatedName"] = ItemLoader.GetItem(_catchItemId).FullName;
        }
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet("DedicatedName", out string name) && Helper.GetContentFromName(name, out ModItem faeling)) {
            _catchItemId = faeling.Type;
        }
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(_catchItemId);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        _catchItemId = reader.ReadInt32();
    }

    public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) {
        boundingBox.Inflate(-4, -4);
    }

    internal static void SpawnFaelingsFromShimmer(Item item) {
        if (!DedicationRegistry.TryGet(item.type, out _) || ItemID.Sets.ShimmerTransformToItem[item.type] != -1) {
            return;
        }

        int maximumSpawnable = 50;
        int highestNPCSlotIndexWeWillPick = 200;
        int slotsAvailable = NPC.GetAvailableAmountOfNPCsToSpawnUpToSlot(item.stack, highestNPCSlotIndexWeWillPick);

        while (maximumSpawnable > 0 && slotsAvailable > 0 && item.stack > 0) {
            maximumSpawnable--;
            slotsAvailable--;
            item.stack--;
            int npc = NPC.NewNPC(item.GetSource_FromThis(), (int)item.Bottom.X, (int)item.Bottom.Y, ModContent.NPCType<DedicatedFaeling>());
            if (npc >= 0) {
                Main.npc[npc].shimmerTransparency = 1f;
                NetMessage.SendData(MessageID.ShimmerActions, -1, -1, null, 2, npc);
            }
        }

        item.shimmered = true;
        if (item.stack <= 0) {
            item.TurnToAir();
        }

        Shimmer.GetShimmered(item);
    }

    private static void SpawnSparkles(Vector2 spawnCoordinates, IDedicationInfo dedication) {
        if (Main.netMode == NetmodeID.Server || Main.GameUpdateCount % 6 != 0) {
            return;
        }

        spawnCoordinates += Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f, 14f);
        Vector2 velocity = Main.rand.NextVector2Unit() * 0.05f;
        float lifeTime = 45f;

        FadingParticle fadingParticle = new FadingParticle();
        fadingParticle.SetBasicInfo(TextureAssets.Star[Main.rand.Next(4)], null, velocity, spawnCoordinates);
        fadingParticle.SetTypeInfo(lifeTime);
        fadingParticle.AccelerationPerFrame = velocity / lifeTime;
        fadingParticle.ColorTint = Main.hslToRgb(Main.rgbToHsl(dedication.FaelingColor).X, Main.rand.NextFloat(0.5f, 1f), 0.8f);
        fadingParticle.ColorTint.A = 30;
        fadingParticle.FadeInNormalizedTime = 0.5f;
        fadingParticle.FadeOutNormalizedTime = 0.5f;
        fadingParticle.Rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
        fadingParticle.RotationVelocity += 0.05f + Main.rand.NextFloat() * 0.05f;
        fadingParticle.RotationAcceleration -= Main.rand.NextFloat() * 0.0025f;
        fadingParticle.Scale = Vector2.One * (0.4f + 0.6f * Main.rand.NextFloat());
        Main.ParticleSystem_World_OverPlayers.Add(fadingParticle);
    }

    [Autoload(value: false)]
    internal class FaelingItem(ModItem dedicatedItem, IDedicationInfo info) : ModItem, IPostSetupContent {
        private string _name = "Faeling" + dedicatedItem.Name;

        public readonly ModItem _parentItem = dedicatedItem;
        public readonly IDedicationInfo _dedication = info;

        public override string Name => _name;
        public override string Texture => AequusTextures.DedicatedFaelingItem.FullPath;

        public override LocalizedText DisplayName => Language.GetText("ItemName.Shimmerfly");
        public override LocalizedText Tooltip => LocalizedText.Empty;

        protected override bool CloneNewInstances => true;

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Shimmerfly);
            Item.makeNPC = (short)ModContent.NPCType<DedicatedFaeling>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            tooltips.Insert(tooltips.GetIndex("OneDropLogo"), new TooltipLine(Mod, "DedicatedItem", _dedication.GetDedicatedLine().Value) { OverrideColor = _dedication.TextColor });
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
            if (line.Name != "DedicatedItem") {
                return true;
            }

            DedicatedGlobalItem.DrawDedicatedTooltip(line);
            return false;
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                DedicatedGlobalItem.DrawDedicatedItemName(line);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

            var glowColor = GetGlowColor(_dedication);
            spriteBatch.Draw(AequusTextures.DedicatedFaelingItem_Mask, position, frame, glowColor, 0f, origin, scale, SpriteEffects.None, 0f);

            ulong seed = (ulong)(Math.Abs(Main.LocalPlayer.name.GetHashCode()));
            var sparkleTexture = AequusTextures.Sparkles;
            var sparkleOrigin = new Vector2(6f, 5f);
            int size = (int)(TextureAssets.InventoryBack.Value.Width * 0.8f) / 4;
            for (int i = 0; i < 20; i++) {
                float uniqueTimer = (i * 0.1f + Main.GlobalTimeWrappedHourly * 0.3f) % 2f;
                var spriteEffects = Utils.RandomInt(ref seed, 2) == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                var sparkleFrame = sparkleTexture.Frame(verticalFrames: 5, frameY: Utils.RandomInt(ref seed, 5));
                var sparklePosition = position + new Vector2(Utils.RandomInt(ref seed, size), Utils.RandomInt(ref seed, size)).RotatedBy(Main.GlobalTimeWrappedHourly * 0.5f + Utils.RandomInt(ref seed, 100) / 100f * MathHelper.TwoPi);
                float sparkleScaleMultiplier = Utils.RandomInt(ref seed, 100) / 200f + 0.8f;
                if (uniqueTimer > 1f) {
                    continue;
                }

                sparkleFrame.Height -= 2;
                float sparkleScale = MathF.Sin(uniqueTimer * MathHelper.Pi);
                Main.spriteBatch.Draw(sparkleTexture, sparklePosition, sparkleFrame, Color.Lerp(Color.White, glowColor, 0.75f) with { A = 0 } * sparkleScale, 0f, sparkleOrigin, MathF.Pow(sparkleScale, 10f) * sparkleScaleMultiplier, spriteEffects, 0f);
            }

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            var drawCoordinates = Item.Center - Main.screenPosition;
            Main.GetItemDrawFrame(Type, out var texture, out var frame);
            var origin = frame.Size() / 2f;
            spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            Color glowColor = GetGlowColor(_dedication);
            spriteBatch.Draw(AequusTextures.DedicatedFaelingItem_Mask, drawCoordinates, frame, glowColor, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        private static Color GetGlowColor(IDedicationInfo dedicationInfo) {
            return ((dedicationInfo?.FaelingColor ?? Color.White) * 2f) with { A = 220 };
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            SpawnSparkles(Item.Center, _dedication);
        }

        public override void SetStaticDefaults() {
            // Dedicated faelings originally counted as regular faelings for research.
            //ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = ItemID.Shimmerfly;
            ItemID.Sets.ShimmerTransformToItem[Type] = _parentItem.Type;
        }

        void IPostSetupContent.PostSetupContent() {
            if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.TryGetValue(_parentItem.Type, out int parentResearchCount)) {
                // Inherit the parent item's research count, capped at 5.
                Item.ResearchUnlockCount = Math.Min(parentResearchCount, 5);
            }
            else {
                // If the parent item cannot be researched, set it to 0 to make the faeling un-researchable aswell.
                Item.ResearchUnlockCount = 0;
            }
        }
    }
}
