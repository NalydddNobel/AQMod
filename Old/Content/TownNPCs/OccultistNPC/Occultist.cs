using Aequus.Common.NPCs.Bestiary;
using Aequus.Common.NPCs.Components;
using Aequus.Common.Projectiles;
using Aequus.Content.DataSets;
using Aequus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.Localization;

namespace Aequus.Old.Content.TownNPCs.OccultistNPC;

[AutoloadHead()]
public class Occultist : ModNPC, IModifyShoppingSettings {
    public const byte STATE_Passive = 0;
    public const byte STATE_Sleeping = 1;
    public const byte STATE_SleepFalling = 2;

    public const int CHANCE_SLEEP = (int)Main.dayLength * 5; // 270,000

    public byte state;

    private bool _dayTimeSwap;
    private bool _saidGhostDialogue;

    public override List<string> SetNPCNameList() {
        return new() {
            "Abadeer",
            "Cally",
            "Brimmy",
            "Sinh",
            "Vincera",
            "Spectre",
            "Kurskan",
            "Maykr",
        };
    }

    internal void SetupShopQuotes(Mod shopQuotes) {
        shopQuotes.Call("AddNPC", Mod, Type);
        shopQuotes.Call("SetColor", Type, Color.Lerp(Color.White, Color.DarkRed, 0.5f) * 1.5f);
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[NPC.type] = 25;
        NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
        NPCID.Sets.AttackFrameCount[NPC.type] = 4;
        NPCID.Sets.DangerDetectRange[NPC.type] = 400;
        NPCID.Sets.AttackType[NPC.type] = 2;
        NPCID.Sets.AttackTime[NPC.type] = 10;
        NPCID.Sets.AttackAverageChance[NPC.type] = 10;
        NPCID.Sets.HatOffsetY[NPC.type] = 2;

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() {
            Velocity = 1f,
            Direction = -1,
            Scale = 1f,
        });

        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Lovestruck] = true;

        NPC.Happiness
            .SetBiomeAffection<DesertBiome>(AffectionLevel.Like)
            .SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Hate)
            .SetNPCAffection(NPCID.Clothier, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Like)
            .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Guide, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Hate);

        NPCHappiness.Get(NPCID.TaxCollector).SetNPCAffection(Type, AffectionLevel.Love);
        NPCHappiness.Get(NPCID.ArmsDealer).SetNPCAffection(Type, AffectionLevel.Like);
        NPCHappiness.Get(NPCID.Dryad).SetNPCAffection(Type, AffectionLevel.Dislike);
        NPCHappiness.Get(NPCID.Demolitionist).SetNPCAffection(Type, AffectionLevel.Hate);
        NPCHappiness.Get(NPCID.BestiaryGirl).SetNPCAffection(Type, AffectionLevel.Hate);
    }

    public override void SetDefaults() {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 10;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.lavaImmune = true;
        AnimationType = NPCID.Guide;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.DesertBiome);
    }

    public override void AddShops() {
        NPCShop shop = new(Type);
        shop
            .Register();
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) {
        return World.DownedDemonSiegeT1;
    }

    public override string GetChat() {
        if (state > 0) {
            return this.GetDialogue("Awaken").Value;
        }

        return Main.rand.Next(GetAvailableChat(Main.LocalPlayer).ToArray()).Value;
    }

    public IEnumerable<LocalizedText> GetAvailableChat(Player localPlayer) {
        const int BASIC_LINES_COUNT = 5;
        const int NIGHT_LINES_COUNT = 2;
        const int BLOODMOON_LINES_COUNT = 3;
        const int RARE_LINE_CHANCE = 7;

        bool downedHardmodeBoss = NPC.downedMechBossAny || NPC.downedQueenSlime || NPC.downedPlantBoss || NPC.downedGolemBoss || NPC.downedMoonlord;
        if (Main.hardMode) {
            if (!downedHardmodeBoss) {
                yield return this.GetDialogue("EarlyHardmode");
            }
            if (NPC.downedGolemBoss && !NPC.TowerActiveNebula && !NPC.TowerActiveSolar && !NPC.TowerActiveStardust && !NPC.TowerActiveVortex && NPC.MoonLordCountdown <= 0 && !NPC.AnyNPCs(NPCID.MoonLordCore)) {
                yield return this.GetDialogue("Cultists");
            }
        }
        else {
            if (downedHardmodeBoss) {
                yield return this.GetDialogue("Waffles");
            }
        }
        if (!Main.dayTime) {
            if (Main.bloodMoon) {
                for (int i = 0; i < BLOODMOON_LINES_COUNT; i++) {
                    yield return this.GetDialogue($"BloodMoon.{i}");
                }
            }
            else {
                for (int i = 0; i < NIGHT_LINES_COUNT; i++) {
                    yield return this.GetDialogue($"Night.{i}");
                }
            }
            //if (GlimmerZone.EventActive) {
            // yield return this.GetDialogue("Glimmer");
            //}
        }

        for (int i = 0; i < BASIC_LINES_COUNT; i++) {
            yield return this.GetDialogue(i.ToString());
        }

        if (Main.rand.NextBool(RARE_LINE_CHANCE)) {
            yield return this.GetDialogue("Rare");
        }

        if (Main.IsItAHappyWindyDay) {
            yield return this.GetDialogue("WindyDay");
        }
        if (Main.raining) {
            yield return this.GetDialogue("Rain");
        }
        if (Main.IsItStorming) {
            yield return this.GetDialogue("Thunderstorm");
        }
        if (BirthdayParty.PartyIsUp) {
            yield return this.GetDialogue("Party");
        }
        if (localPlayer.ZoneGraveyard) {
            yield return this.GetDialogue("Graveyard");
        }
        if (NPC.AnyNPCs(NPCID.Princess)) {
            yield return this.GetDialogue("Princess");
        }
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
        }
    }

    public override bool CanGoToStatue(bool toKingStatue) {
        return !toKingStatue;
    }

    private bool WakeFromFalling() {
        var tile = Framing.GetTileSafely(NPC.Bottom.ToTileCoordinates());
        return (NPC.collideY && NPC.velocity.Y == 0f)
            || tile.IsSolid() || tile.SolidTopType()
            || NPC.localAI[0] > 100f;
    }

    public override bool PreAI() {
        if (NPC.shimmering) {
            if (state == STATE_Sleeping) {
                state = STATE_SleepFalling;
            }
        }
        if (state == STATE_SleepFalling) {
            NPC.noGravity = false;
            NPC.velocity.X *= 0.9f;
            NPC.knockBackResist = 0.5f;
            if (WakeFromFalling()) {
                NPC.rotation = 0f;
                if (NPC.localAI[0] == 0f) {
                    SoundEngine.PlaySound(SoundID.Item118 with { Pitch = -0.1f }, NPC.Center);
                }
                NPC.localAI[0]++;
                NPC.ai[0]++;
                if (NPC.ai[0] > 100f) {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    state = STATE_Passive;
                    NPC.netUpdate = true;
                    if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.talkNPC == NPC.whoAmI && !string.IsNullOrEmpty(Main.npcChatText)) {
                        Main.npcChatCornerItem = 0;
                        NPCLoader.GetChat(NPC, ref Main.npcChatText);
                    }
                }
            }
            else {
                NPC.rotation += NPC.direction * NPC.velocity.Y * 0.1f;
                NPC.ai[0] = 0f;
            }
            return false;
        }

        if (state == STATE_Sleeping) {
            if ((!Main.dayTime && Main.rand.NextBool(400)) || NPC.life < NPC.lifeMax) {
                NPC.ai[0] = 0f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.position.Y += 16f;
                state = STATE_Passive;
                NPC.netUpdate = true;
                return false;
            }
            if (NPC.ai[0] > 0f) {
                NPC.ai[0]++;
                if (NPC.ai[0] > 10f) {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.position.Y += 16f;
                    state = STATE_SleepFalling;
                    NPC.netUpdate = true;
                }
                return false;
            }
            NPC.noGravity = true;
            NPC.velocity *= 0.1f;
            NPC.knockBackResist = 0f;
            if (!TileHelper.ScanUp(NPC.Top.ToTileCoordinates(), 2, out var roof)) {
                state = STATE_SleepFalling;
                return false;
            }
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && (Main.player[i].talkNPC == NPC.whoAmI || Main.player[i].Distance(NPC.Center) < 100f && Main.player[i].ghost)) {
                    NPC.ai[0]++;
                    return false;
                }
            }
            return false;
        }

        NPC.noGravity = false;
        if (!_dayTimeSwap) {
            _dayTimeSwap = true;
            Main.dayTime = !Main.dayTime;
        }
        return true;
    }

    private void CheckSleepState() {
        if (Main.dayTime || !Main.rand.NextBool(CHANCE_SLEEP)) {
            return;
        }

        var tileCoordinates = NPC.Top.ToTileCoordinates();
        if (!TileHelper.ScanUp(tileCoordinates, 15, out var roof)
            || Main.tileSolidTop[Main.tile[roof].TileType]
            || PlayerCollision()
            || TileHelper.ScanTilesSquare(roof.X, roof.Y, 8, TileHelper.HasShimmer)) {
            return;
        }

        state = STATE_Sleeping;
        NPC.ai[0] = 0f;
        NPC.ai[1] = 0f;
        NPC.ai[2] = 0f;
        NPC.ai[3] = 0f;
        NPC.localAI[0] = 0f;
        NPC.localAI[1] = 0f;
        NPC.localAI[2] = 0f;
        NPC.localAI[3] = 0f;
        NPC.Top = roof.ToWorldCoordinates();
        NPC.noGravity = true;
        NPC.netUpdate = true;
        NPC.velocity *= 0.1f;

        bool PlayerCollision() {
            Rectangle myHitbox = NPC.Hitbox;

            Rectangle ghostDetectionHitbox = NPC.Hitbox;
            ghostDetectionHitbox.Inflate(32, 32);

            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active) {
                    if (Main.player[i].ghost && Main.player[i].Hitbox.Intersects(ghostDetectionHitbox)) {
                        return true;
                    }
                    else if (Main.player[i].Hitbox.Intersects(myHitbox)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public override void AI() {
        if ((int)NPC.ai[0] == 14) {
            NPC.ai[1] += 0.9f;
            if (Main.GameUpdateCount % 7 == 0) {
                var d = Dust.NewDustDirect(NPC.position + new Vector2(0f, NPC.height - 4), NPC.width, 4, DustID.PurpleCrystalShard, 0f, -4f);
                d.velocity *= 0.5f;
                d.velocity.X *= 0.5f;
                d.noGravity = true;
            }
            return;
        }

        if (NPC.life < NPC.lifeMax) {
            return;
        }

        if (Main.netMode != NetmodeID.MultiplayerClient) {
            CheckSleepState();
        }
        if (Main.netMode != NetmodeID.Server) {
            if (!_saidGhostDialogue && Main.LocalPlayer.Distance(NPC.Center) < 200f && Main.LocalPlayer.ghost) {
                _saidGhostDialogue = true;
                Main.NewText(Language.GetTextValueWith("Mods.Aequus.OccultistEasterEgg", new { Name = NPC.GivenName, PlayerName = Main.LocalPlayer.name }));
            }
        }
    }

    public override void PostAI() {
        if (_dayTimeSwap) {
            Main.dayTime = !Main.dayTime;
            _dayTimeSwap = false;
        }
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        int dustAmount = Math.Clamp(hit.Damage / 3, NPC.life > 0 ? 1 : 40, 40);
        for (int k = 0; k < dustAmount; k++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.Y, newColor: Color.Violet);
        }
        if (NPC.life <= 0) {
            NPC.NewGore(AequusTextures.OccultistGoreWing, NPC.position, NPC.velocity);
            NPC.NewGore(AequusTextures.OccultistGoreWing, NPC.position, NPC.velocity).rotation += MathHelper.Pi;
            NPC.NewGore(AequusTextures.OccultistGoreHead, NPC.position, NPC.velocity);

            if (Main.rand.NextBool(4)) {
                NPC.NewGore(AequusTextures.OccultistGoreBook, NPC.position, Vector2.UnitY * -2f);
            }
        }
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
        damage = 20;
        knockback = 8f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
        cooldown = 60;
        randExtraCooldown = 2;
    }

    public override void TownNPCAttackMagic(ref float auraLightMultiplier) {
        auraLightMultiplier = 0f;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
        projType = ModContent.ProjectileType<OccultistProjSpawner>();
        attackDelay = 12;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
        multiplier = 6f;
        randomOffset = 1.5f;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(state);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        state = reader.ReadByte();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        SpriteEffects spriteEffect = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        if (state == STATE_Sleeping || state == STATE_SleepFalling) {
            NPC.frame.Y = NPC.frame.Height * 1;
            var sleepingTexture = AequusTextures.OccultistSleep;
            var sleepingGlow = AequusTextures.OccultistSleep_Glow;
            if (state == STATE_Sleeping) {
                var sleepingFrame = sleepingTexture.Frame(verticalFrames: 19, frameY: NPC.ai[0] > 0f ? 1 : 0);
                var sleepingOrigin = new Vector2(sleepingTexture.Value.Width / 2f, 0f);
                spriteBatch.Draw(sleepingTexture.Value, NPC.Top + new Vector2(0f, 4f) - screenPos, sleepingFrame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, sleepingOrigin, NPC.scale, spriteEffect, 0f);
                spriteBatch.Draw(sleepingGlow.Value, NPC.Top + new Vector2(0f, 4f) - screenPos, sleepingFrame, Color.White, NPC.rotation, sleepingOrigin, NPC.scale, spriteEffect, 0f);
            }
            else if (state == STATE_SleepFalling) {
                int frameY = 1;
                if (NPC.collideY && NPC.velocity.Y == 0f) {
                    frameY += 1 + (int)(NPC.ai[0] / 6);
                    NPC.rotation = 0f;
                }
                var sleepingFrame = sleepingTexture.Frame(verticalFrames: 19, frameY: frameY);
                var sleepingOrigin = sleepingFrame.Size() / 2f;
                spriteBatch.Draw(sleepingTexture.Value, NPC.Center + new Vector2(NPC.direction * 3f, -5f) - screenPos, sleepingFrame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, sleepingOrigin, NPC.scale, spriteEffect, 0f);
                spriteBatch.Draw(sleepingGlow.Value, NPC.Center + new Vector2(NPC.direction * 3f, -5f) - screenPos, sleepingFrame, Color.White, NPC.rotation, sleepingOrigin, NPC.scale, spriteEffect, 0f);
            }
            return false;
        }
        if (NPC.frame.Y >= NPC.frame.Height * 23) {
            NPC.frameCounter = 0;
            NPC.frame.Y = NPC.frame.Height * 21;
        }

        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 offset = new Vector2(NPC.width / 2f, NPC.height / 2f + NPC.gfxOffY + DrawOffsetY - 4f);
        Rectangle frame = NPC.frame;
        Vector2 origin = frame.Size() / 2f;

        spriteBatch.Draw(texture, NPC.position + offset - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, spriteEffect, 0f);
        spriteBatch.Draw(AequusTextures.Occultist_Glow.Value, NPC.position + offset - screenPos, frame, NPC.GetNPCColorTintedByBuffs(Color.White), NPC.rotation, origin, NPC.scale, spriteEffect, 0f);
        if ((int)NPC.ai[0] == 14) {
            var bloomFrame = AequusTextures.Bloom.Frame(verticalFrames: 2);
            spriteBatch.Draw(AequusTextures.Bloom, NPC.position + offset - screenPos + new Vector2(2f * -NPC.spriteDirection, NPC.height / 2f + 6f).RotatedBy(NPC.rotation),
                bloomFrame, Color.BlueViolet * 0.5f, NPC.rotation, AequusTextures.Bloom.Size() / 2f, NPC.scale * 0.5f, spriteEffect, 0f);
            var auraFrame = TextureAssets.Extra[51].Value.Frame(verticalFrames: 4, frameY: (int)(Main.GlobalTimeWrappedHourly * 9f) % 4);
            spriteBatch.Draw(TextureAssets.Extra[51].Value, NPC.position + offset - screenPos + new Vector2(4f * -NPC.spriteDirection, NPC.height / 2f + 8f).RotatedBy(NPC.rotation),
                auraFrame, Color.BlueViolet * 0.7f, NPC.rotation, new Vector2(auraFrame.Width / 2f, auraFrame.Height), NPC.scale, spriteEffect, 0f);
        }
        return false;
    }

    public void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
        DialogueHack.ReplaceKeys(ref settings.HappinessReport, "[HateBiomeQuote]|",
            $"Mods.Aequus.TownNPCMood.Occultist.HateBiome_{(player.ZoneSnow ? "Snow" : "Evils")}", (s) => new { BiomeName = s[1], });
        DialogueHack.ReplaceKeys(ref settings.HappinessReport, "[LikeNPCQuote]|",
            $"Mods.Aequus.TownNPCMood.Occultist.LikeNPC_{(player.isNearNPC(NPCID.Demolitionist) ? "Demolitionist" : "Clothier")}", (s) => new { NPCName = s[1], });
    }

    public override bool? CanBeHitByProjectile(Projectile projectile) {
        if (ProjectileSets.OccultistIgnore.Contains(projectile.type)) {
            return false;
        }

        // Check if the enemy which shot this projectile can hit the Occultist.
        // Enemies which can't hit the occultist all spawn in the Underworld, or are from the Demon Siege.
        if (projectile.TryGetGlobalProjectile(out ProjectileSource sources) && sources.HasNPCOwner) {
            if (!NPCLoader.CanHitNPC(Main.npc[sources.parentNPCIndex], NPC)) {
                return false;
            }
        }

        return null;
    }

    public override bool CanBeHitByNPC(NPC attacker) {
        return !NPCSets.OccultistIgnore.Contains(attacker.type);
    }
}

//public class OccultistParticle : BaseParticle<OccultistParticle> {
//    public float t;
//    public float opacity;
//    public float scale;

//    protected override void SetDefaults() {
//        SetHorizontalAndVerticallyFramedTexture(AequusTextures.OccultistRune, 3, 14, 0);
//        t = Main.rand.Next(100);
//        opacity = 0f;
//        scale = Scale;
//    }

//    public override void Update(ref ParticleRendererSettings settings) {
//        if (t > 400f) {
//            opacity -= 0.001f + opacity * 0.09f;
//        }
//        else {
//            if (opacity < 1f) {
//                opacity += 0.09f;
//                if (opacity > 1f)
//                    opacity = 1f;
//            }
//            if (Scale < scale) {
//                Scale += 0.015f;
//                if (Scale > scale)
//                    Scale = scale;
//            }
//        }
//        Velocity *= 0.9f;
//        if (Scale <= 0.1f || opacity <= 0f) {
//            ShouldBeRemovedFromRenderer = true;
//            return;
//        }
//        Position += Velocity;
//        Position.Y += Helper.Wave(t * -0.02f, -0.1f, 0.1f);
//        Lighting.AddLight(Position, new Vector3(0.6f, 0.1f, 0.05f));
//        t++;
//    }

//    public override Color GetParticleColor(ref ParticleRendererSettings settings) {
//        return Color.White * opacity;
//    }

//    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
//        spritebatch.Draw(texture, Position - Main.screenPosition,
//            frame.Frame(frameX: 2, frameY: 0), Color.OrangeRed.UseA(0) * opacity * Helper.Wave(t * 0.1f, 0.5f, 1.1f), Rotation, origin, Scale, SpriteEffects.None, 0f);

//        base.Draw(ref settings, spritebatch);
//    }
//}