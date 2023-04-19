using Aequus;
using Aequus.Common;
using Aequus.Common.Personalities;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes.GoreNest.Tiles;
using Aequus.Content.CrossMod;
using Aequus.Content.Events.DemonSiege.Misc;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Town.ExporterNPC.Quest;
using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Accessories.Misc;
using Aequus.Items.Accessories.Offense;
using Aequus.Items.Accessories.Offense.Crit;
using Aequus.Items.Accessories.Offense.Necro;
using Aequus.Items.Accessories.Offense.Sentry;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Materials.Gems;
using Aequus.Items.Tools;
using Aequus.Items.Tools.GrapplingHooks;
using Aequus.Items.Weapons.Magic.Misc;
using Aequus.Items.Weapons.Melee.Thrown;
using Aequus.Items.Weapons.Necromancy.Candles;
using Aequus.Items.Weapons.Necromancy.Scepters;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Minion;
using Aequus.NPCs;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Misc;
using Aequus.Tiles.Furniture.Paintings.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Town.OccultistNPC {
    [AutoloadHead()]
    public class Occultist : ModNPC, IModifyShoppingSettings {
        public const byte STATE_Passive = 0;
        public const byte STATE_Sleeping = 1;
        public const byte STATE_SleepFalling = 2;

        public byte state;
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

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
                Velocity = 1f,
                Direction = -1,
            });

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData() {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Lovestruck,
                }
            });

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

            ExporterQuestSystem.NPCTypesNoSpawns.Add(Type);
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
            shop.Add<GhostlyGrave>()
                .Add<OccultistCandle>()
                .Add<CrownOfBlood>()
                .Add<CrownOfDarkness>()
                .Add<CrownOfTheGrounded>()
                .Add<Meathook>()
                .Add<UnholyCore>()

                .Add<SpiritBottle>(Condition.TimeNight)
                .Add<SoulGem>(Condition.TimeNight)
                .Add<Wabbajack>(Condition.BloodMoon)

                .Add<Malediction>(Condition.DownedSkeletron)

                .Add(ItemID.ShadowKey, Condition.DownedSkeletron, Condition.MoonPhaseFull)
                .Add<PandorasBox>(Condition.DownedSkeletron, Condition.MoonPhaseFull)

                .Add(ItemID.Handgun, Condition.DownedSkeletron, Condition.MoonPhaseWaningGibbous)
                .AddCrossMod<ThoriumMod>("StreamSting", Condition.DownedSkeletron, Condition.MoonPhaseWaningGibbous)

                .Add(ItemID.MagicMissile, Condition.DownedSkeletron, Condition.MoonPhaseThirdQuarter)
                .Add<Revenant>(Condition.DownedSkeletron, Condition.MoonPhaseThirdQuarter)

                .Add(ItemID.CobaltShield, Condition.DownedSkeletron, Condition.MoonPhaseWaningCrescent)
                .AddCrossMod<ThoriumMod>("StrongestLink", Condition.DownedSkeletron, Condition.MoonPhaseWaningCrescent)

                .Add(ItemID.BlueMoon, Condition.DownedSkeletron, Condition.MoonPhaseNew)
                .AddCrossMod<ThoriumMod>("HighTide", Condition.DownedSkeletron, Condition.MoonPhaseNew)

                .Add(ItemID.Muramasa, Condition.DownedSkeletron, Condition.MoonPhaseWaxingCrescent)
                .Add<DungeonCandle>(Condition.DownedSkeletron, Condition.MoonPhaseWaxingCrescent)

                .Add(ItemID.Valor, Condition.DownedSkeletron, Condition.MoonPhaseFirstQuarter)
                .AddCrossMod<ThoriumMod>("BoneReaper", Condition.DownedSkeletron, Condition.MoonPhaseFirstQuarter)

                .Add<Valari>(Condition.DownedSkeletron, Condition.MoonPhaseWaxingGibbous)
                .AddCrossMod<ThoriumMod>("NaiadShiv", Condition.DownedSkeletron, Condition.MoonPhaseWaxingGibbous)

                .Add<GoreNest>(Condition.Hardmode)
                .Add<PotionCanteen>(Condition.Hardmode)

                .Add<GoreNestPainting>(Condition.NpcIsPresent(NPCID.Painter))
                .Add<InsurgentPainting>(Condition.NpcIsPresent(NPCID.Painter))
                .Register();
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) {
            return AequusWorld.downedEventDemon;
        }

        public override ITownNPCProfile TownNPCProfile() {
            return base.TownNPCProfile();
        }

        public override string GetChat() {
            if (state > 0) {
                return Language.GetTextValue("Mods.Aequus.Chat.Occultist.Awaken");
            }
            var player = Main.LocalPlayer;
            var chat = new SelectableChatHelper("Mods.Aequus.Chat.Occultist.");

            if (Main.hardMode) {
                if (!NPC.downedMechBossAny && !NPC.downedQueenSlime && !AequusWorld.downedDustDevil) {
                    chat.Add("EarlyHardmode");
                }
                if (NPC.downedGolemBoss && !NPC.TowerActiveNebula && !NPC.TowerActiveSolar && !NPC.TowerActiveStardust && !NPC.TowerActiveVortex && NPC.MoonLordCountdown <= 0 && !NPC.AnyNPCs(NPCID.MoonLordCore)) {
                    chat.Add("Cultists");
                }
            }
            if (!Main.dayTime) {
                if (Main.bloodMoon) {
                    chat.Add("BloodMoon.0");
                    chat.Add("BloodMoon.1");
                    chat.Add("BloodMoon.2");
                }
                else {
                    chat.Add("Night.0");
                    chat.Add("Night.1");
                }
                if (GlimmerBiomeManager.EventActive) {
                    chat.Add("Glimmer");
                }
            }
            else {
                chat.Add("Basic.0");
                chat.Add("Basic.1");
                chat.Add("Basic.2");
                chat.Add("Basic.3");
                if (Main.rand.NextBool(7))
                    chat.Add("Basic.Rare");
            }

            if (Main.IsItAHappyWindyDay)
                chat.Add("WindyDay");
            if (Main.raining)
                chat.Add("Rain");
            if (Main.IsItStorming)
                chat.Add("Thunderstorm");
            if (BirthdayParty.PartyIsUp)
                chat.Add("Party");
            if (player.ZoneGraveyard)
                chat.Add("Graveyard");
            if (NPC.AnyNPCs(NPCID.Princess))
                chat.Add("Princess");

            return chat.Get();
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
                if (NPC.collideY && NPC.velocity.Y == 0f) {
                    NPC.rotation = 0f;
                    if (NPC.localAI[0] == 0f) {
                        SoundEngine.PlaySound(SoundID.Item118.WithPitch(-0.1f), NPC.Center);
                    }
                    NPC.localAI[0]++;
                    NPC.ai[0]++;
                    if (NPC.ai[0] > 100f) {
                        NPC.ClearAI(localAI: false);
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
                    NPC.ClearAI(localAI: false);
                    NPC.position.Y += 16f;
                    state = STATE_Passive;
                    NPC.netUpdate = true;
                    return false;
                }
                if (NPC.ai[0] > 0f) {
                    NPC.ai[0]++;
                    if (NPC.ai[0] > 10f) {
                        NPC.ClearAI(localAI: false);
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
                    if (Main.player[i].active && (Main.player[i].talkNPC == NPC.whoAmI || (Main.player[i].Distance(NPC.Center) < 100f && Main.player[i].ghost))) {
                        NPC.ai[0]++;
                        return false;
                    }
                }
                return false;
            }

            NPC.noGravity = false;
            return true;
        }

        private void CheckSleepState() {
            if (!Main.dayTime || !Main.rand.NextBool(500)) {
                return;
            }

            var tileCoordinates = NPC.Top.ToTileCoordinates();
            if (!TileHelper.ScanUp(tileCoordinates, 15, out var roof)
                || Main.tileSolidTop[Main.tile[roof].TileType]
                || Helper.FindFirstPlayerWithin(NPC) != -1
                || TileHelper.ScanTilesSquare(roof.X, roof.Y, 8, TileHelper.HasShimmer)) {
                return;
            }

            state = STATE_Sleeping;
            NPC.ClearAI(localAI: true);
            NPC.Top = roof.ToWorldCoordinates();
            NPC.noGravity = true;
            NPC.netUpdate = true;
            NPC.velocity *= 0.1f;
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

            if (NPC.life < NPC.lifeMax)
                return;

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

        public override void HitEffect(NPC.HitInfo hit) {
            if (Main.netMode == NetmodeID.Server)
                return;
            int dustAmount = (int)Math.Clamp(hit.Damage / 3, NPC.life > 0 ? 1 : 40, 40);
            for (int k = 0; k < dustAmount; k++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SpaceSquidBlood>(), NPC.velocity.X, NPC.velocity.Y);
            }
            if (NPC.life <= 0) {
                var g = NPC.DeathGore("Occultist_1");
                g.rotation += MathHelper.Pi;
                NPC.DeathGore("Occultist_1");
                NPC.DeathGore("Occultist_0", new Vector2(0f, -NPC.height / 2f + 8f));

                if (Main.rand.NextBool(4)) {
                    NPC.DeathGore("Occultist_2", default, new Vector2(0f, -2f));
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
            if (state == STATE_Sleeping || state == STATE_SleepFalling) {
                NPC.frame.Y = NPC.frame.Height * 1;
                var sleepingTexture = ModContent.Request<Texture2D>($"{Texture}Sleep");
                var sleepingGlow = ModContent.Request<Texture2D>($"{Texture}Sleep_Glow");
                if (state == STATE_Sleeping) {
                    var sleepingFrame = sleepingTexture.Frame(verticalFrames: 19, frameY: NPC.ai[0] > 0f ? 1 : 0);
                    var sleepingOrigin = new Vector2(sleepingTexture.Value.Width / 2f, 0f);
                    spriteBatch.Draw(sleepingTexture.Value, NPC.Top + new Vector2(0f, 4f) - screenPos, sleepingFrame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, sleepingOrigin, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
                    spriteBatch.Draw(sleepingGlow.Value, NPC.Top + new Vector2(0f, 4f) - screenPos, sleepingFrame, Color.White, NPC.rotation, sleepingOrigin, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
                }
                else if (state == STATE_SleepFalling) {
                    int frameY = 1;
                    if (NPC.collideY && NPC.velocity.Y == 0f) {
                        frameY += 1 + (int)(NPC.ai[0] / 6);
                        NPC.rotation = 0f;
                    }
                    var sleepingFrame = sleepingTexture.Frame(verticalFrames: 19, frameY: frameY);
                    var sleepingOrigin = sleepingFrame.Size() / 2f;
                    spriteBatch.Draw(sleepingTexture.Value, NPC.Center + new Vector2(NPC.direction * 3f, -5f) - screenPos, sleepingFrame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, sleepingOrigin, NPC.scale, NPC.spriteDirection.ToSpriteEffect(), 0f);
                    spriteBatch.Draw(sleepingGlow.Value, NPC.Center + new Vector2(NPC.direction * 3f, -5f) - screenPos, sleepingFrame, Color.White, NPC.rotation, sleepingOrigin, NPC.scale, NPC.spriteDirection.ToSpriteEffect(), 0f);
                }
                return false;
            }
            if (NPC.frame.Y >= NPC.frame.Height * 23) {
                NPC.frameCounter = 0;
                NPC.frame.Y = NPC.frame.Height * 21;
            }
            NPC.GetDrawInfo(out var t, out var off, out var frame, out var orig, out int _);
            off.Y += NPC.gfxOffY - 4f;
            spriteBatch.Draw(t, NPC.position + off - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, orig, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            if (ModContent.RequestIfExists<Texture2D>($"{Texture}_Glow", out var glowmask, AssetRequestMode.ImmediateLoad)) {
                spriteBatch.Draw(glowmask.Value, NPC.position + off - screenPos, frame, NPC.GetNPCColorTintedByBuffs(Color.White), NPC.rotation, orig, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            }
            if ((int)NPC.ai[0] == 14) {
                var bloomFrame = AequusTextures.Bloom0.Frame(verticalFrames: 2);
                spriteBatch.Draw(AequusTextures.Bloom0, NPC.position + off - screenPos + new Vector2(2f * -NPC.spriteDirection, NPC.height / 2f + 6f).RotatedBy(NPC.rotation),
                    bloomFrame, Color.BlueViolet * 0.5f, NPC.rotation, AequusTextures.Bloom0.Size() / 2f, NPC.scale * 0.5f, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
                var auraFrame = TextureAssets.Extra[51].Value.Frame(verticalFrames: 4, frameY: (int)(Main.GlobalTimeWrappedHourly * 9f) % 4);
                spriteBatch.Draw(TextureAssets.Extra[51].Value, NPC.position + off - screenPos + new Vector2(4f * -NPC.spriteDirection, NPC.height / 2f + 8f).RotatedBy(NPC.rotation),
                    auraFrame, Color.BlueViolet * 0.7f, NPC.rotation, new Vector2(auraFrame.Width / 2f, auraFrame.Height), NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            }
            return false;
        }

        public void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
            Helper.ReplaceTextWithStringArgs(ref settings.HappinessReport, "[HateBiomeQuote]|",
                $"Mods.Aequus.TownNPCMood.Occultist.HateBiome_{(player.ZoneSnow ? "Snow" : "Evils")}", (s) => new { BiomeName = s[1], });
            Helper.ReplaceTextWithStringArgs(ref settings.HappinessReport, "[LikeNPCQuote]|",
                $"Mods.Aequus.TownNPCMood.Occultist.LikeNPC_{(player.isNearNPC(NPCID.Demolitionist) ? "Demolitionist" : "Clothier")}", (s) => new { NPCName = s[1], });
        }
    }

    public class OccultistHostile : Occultist {
        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("{$Mods.Aequus.NPCName.Occultist}");
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
                Hide = true,
            });
        }

        public override void SetDefaults() {
            base.SetDefaults();
            NPC.lifeMax = 5000;
            NPC.friendly = false;
            NPC.rarity = 1;
            NPC.townNPC = false;
            NPC.dontTakeDamage = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
            return false;
        }

        public override bool PreAI() {
            if (Main.netMode != NetmodeID.Server && Main.GameUpdateCount % 180 == 0) {
                for (int i = 0; i < 50; i++) {
                    var p = NPC.Center + new Vector2(NPC.direction * -50, -30f) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(15f, 60f);
                    if (Collision.SolidCollision(new Vector2(p.X - 8f, p.Y - 8f), 16, 16)) {
                        continue;
                    }
                    ParticleSystem.New<OccultistParticle>(ParticleLayer.BehindAllNPCs).Setup(p, Vector2.UnitY * -0.1f);
                    break;
                }
            }

            int dir = Math.Sign(((int)NPC.ai[0] + 1) * 16 + 8 - NPC.Center.X);
            if (NPC.direction != dir) {
                NPC.direction = dir;
            }
            if (AequusWorld.downedEventDemon) {
                NPC.ai[0] = 0f;
                NPC.ai[1] = 0f;
                NPC.Transform(ModContent.NPCType<Occultist>());
            }
            return false;
        }

        public static string RollChat(string dontRoll) {
            var chat = new SelectableChatHelper("Mods.Aequus.Chat.Occultist.Hostile.");

            for (int i = 0; i <= 3; i++) {
                chat.Add(i.ToString());
            }

            if (!WorldGen.crimson) {
                chat.Add("LightsBane");
                chat.Add("DemonBow");
                chat.Add("Vilethorn");
                chat.Add("CorruptPot");
                chat.Add("Corruption");
            }
            else {
                chat.Add("BloodButcherer");
                chat.Add("TendonBow");
                chat.Add("CrimsonRod");
                chat.Add("Mindfungus");
                chat.Add("Crimson");
            }

            AddNonDefaultChats(chat);

            if (string.IsNullOrEmpty(dontRoll)) {
                for (int i = 0; i < 25; i++) {
                    string t = chat.Get();
                    if (t != dontRoll)
                        return t;
                }
            }
            return chat.Get();
        }

        public static void AddNonDefaultChats(SelectableChatHelper chat) {
            CheckItem(ItemID.LightsBane, "LightsBane", chat);
            CheckItem(ItemID.DemonBow, "DemonBow", chat);
            CheckItem(ItemID.Vilethorn, "Vilethorn", chat);
            CheckItem(ModContent.ItemType<CorruptPot>(), "CorruptPot", chat);
            CheckItem(ItemID.BloodButcherer, "BloodButcherer", chat);
            CheckItem(ItemID.TendonBow, "TendonBow", chat);
            CheckItem(ItemID.CrimsonRod, "CrimsonRod", chat);
            CheckItem(ModContent.ItemType<MindfungusStaff>(), "CorruptPot", chat);
            var bny = Main.LocalPlayer.FindItem((i) => !i.IsAir && ItemID.Search.TryGetName(i.type, out string name) && name.Contains("Bunny"));
            if (bny != null || Main.rand.NextBool(3)) {
                chat.Add("Bunny");
            }
        }
        public static void CheckItem(int itemType, string text, SelectableChatHelper chat) {
            if (Main.LocalPlayer.HasItem(itemType)) {
                chat.Add(text);
            }
        }

        public override bool CanChat() {
            return true;
        }

        public override string GetChat() {
            return RollChat(null);
        }

        public override void SetChatButtons(ref string button, ref string button2) {
            button = TextHelper.GetTextValue("Chat.Occultist.ListenButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
            if (firstButton) {
                Main.npcChatText = RollChat(Main.npcChatText);
            }
        }

        public override bool CanGoToStatue(bool toKingStatue) {
            return false;
        }

        public static void CheckSpawn(int x, int y, int plr) {
            if (!AequusWorld.downedEventDemon && Main.player[plr].Distance(new Vector2(x * 16f, y * 16f)) > 800f && !Main.hardMode && !NPC.AnyNPCs(ModContent.NPCType<OccultistHostile>())) {
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    var p = Aequus.GetPacket(PacketType.SpawnHostileOccultist);
                    p.Write(x);
                    p.Write(y);
                    p.Write(plr);
                    p.Send();
                }
                else {
                    int spawnX = (x + 1) * 16 + 8;
                    int spawnY = (y + 1) * 16 + 8 + 24;
                    int dir = Math.Sign(Main.player[plr].Center.X - spawnX);
                    spawnX -= 48 * dir;
                    int middleX = x + 1;
                    dir = -dir;
                    for (int k = 0; k < 3; k++) {
                        int m = middleX + dir * 2 + k * dir;
                        int n = y + 3;
                        var t = Main.tile[m, n];
                        if (!t.IsFullySolid()) {
                            WorldGen.PlaceTile(m, n, TileID.Ash);
                        }
                        t.Slope = SlopeType.Solid;
                        t.IsHalfBlock = false;
                        for (int l = 0; l < 4; l++) {
                            if (Main.tile[m, n - l - 1].IsFullySolid()) {
                                WorldGen.KillTile(m, n - l - 1, noItem: true);
                            }
                        }
                    }
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendTileSquare(-1, middleX - 5, y - 5, 10, 10);
                    NPC.NewNPC(new EntitySource_SpawnNPC(), spawnX, spawnY, ModContent.NPCType<OccultistHostile>(), ai0: x, ai1: y);
                }
            }
        }

        public override void AI() {
            base.AI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            var texture = ModContent.Request<Texture2D>($"{this.GetPath()}_Sit", AssetRequestMode.ImmediateLoad).Value;
            var glow = ModContent.Request<Texture2D>($"{this.GetPath()}_Sit_Glow", AssetRequestMode.ImmediateLoad).Value;
            var frame = texture.Frame(verticalFrames: 5, frameY: (int)Main.GameUpdateCount / 5 % 4 + 1);
            var origin = frame.Size() / 2f;
            var drawCoords = NPC.Center - screenPos + new Vector2(0f, -6f);
            spriteBatch.Draw(texture, drawCoords, frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            spriteBatch.Draw(glow, drawCoords, frame, Color.White, NPC.rotation, origin, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            return false;
        }
    }

    public class OccultistParticle : BaseParticle<OccultistParticle> {
        public float t;
        public float opacity;
        public float scale;

        public override OccultistParticle CreateInstance() {
            return new OccultistParticle();
        }

        protected override void SetDefaults() {
            var tex = ModContent.Request<Texture2D>($"{Helper.GetPath<Occultist>()}Rune", AssetRequestMode.ImmediateLoad);
            SetTexture(new SpriteInfo(tex, 3, 14, new Vector2(tex.Value.Width / 6f, tex.Value.Height / 16f)), 14);
            t = Main.rand.Next(100);
            opacity = 0f;
            scale = Scale;
        }

        public override void Update(ref ParticleRendererSettings settings) {
            if (t > 400f) {
                opacity -= 0.001f + opacity * 0.09f;
            }
            else {
                if (opacity < 1f) {
                    opacity += 0.09f;
                    if (opacity > 1f)
                        opacity = 1f;
                }
                if (Scale < scale) {
                    Scale += 0.015f;
                    if (Scale > scale)
                        Scale = scale;
                }
            }
            Velocity *= 0.9f;
            if (Scale <= 0.1f || opacity <= 0f) {
                ShouldBeRemovedFromRenderer = true;
                return;
            }
            Position += Velocity;
            Position.Y += Helper.Wave(t * -0.02f, -0.1f, 0.1f);
            Lighting.AddLight(Position, new Vector3(0.6f, 0.1f, 0.05f));
            t++;
        }

        public override Color GetParticleColor(ref ParticleRendererSettings settings) {
            return Color.White * opacity;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
            spritebatch.Draw(texture, Position - Main.screenPosition,
                frame.Frame(frameX: 2, frameY: 0), Color.OrangeRed.UseA(0) * opacity * Helper.Wave(t * 0.1f, 0.5f, 1.1f), Rotation, origin, Scale, SpriteEffects.None, 0f);

            base.Draw(ref settings, spritebatch);
        }
    }
}