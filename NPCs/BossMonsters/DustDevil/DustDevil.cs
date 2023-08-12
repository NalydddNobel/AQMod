using Aequus;
using Aequus.Common;
using Aequus.Common.Effects;
using Aequus.Common.Items.DropRules;
using Aequus.Common.NPCs;
using Aequus.Common.Utilities;
using Aequus.Content.Events.GaleStreams;
using Aequus.Items.Equipment.Accessories.Combat.Passive.Stormcloak;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Misc.GrabBags.TreasureBags;
using Aequus.Items.Potions.Healing.Restoration;
using Aequus.Items.Weapons.Magic.GaleStreams.WindFan;
using Aequus.Items.Weapons.Melee.Misc.PhaseDisc;
using Aequus.NPCs.BossMonsters.DustDevil.Projectiles;
using Aequus.Particles;
using Aequus.Tiles.Furniture.Boss.Relics;
using Aequus.Tiles.Furniture.Boss.Trophies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.DustDevil {
    [AutoloadBossHead]
    public class DustDevil : AequusBoss {
        public const float BossProgression = 8.5f;

        public const int ACTION_ICE = 5;
        public const int ACTION_FIREBALLS = 4;
        public const int ACTION_SUCTIONENEMIES = 3;
        public const int ACTION_SUCTIONTILES = 2;

        internal static LegacyDrawList LegacyDrawBack;
        internal static LegacyDrawList LegacyDrawFront;
        public static ConfiguredMusicData music { get; private set; }

        public float HPRatio => NPC.life / (float)NPC.lifeMax;
        public bool PhaseTwo => NPC.life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

        public int SecondaryAction { get => (int)NPC.ai[2]; set => NPC.ai[2] = value; }
        public float DirectActionTimer { get => NPC.ai[3]; set => NPC.ai[3] = value; }

        public TornadoManipulator Tornado;

        public int effectsTimer;
        public int auraTimer;

        public override void Load() {
            if (!Main.dedServ) {
                music = new ConfiguredMusicData(MusicID.Boss2, MusicID.OtherworldlyBoss2);
                LegacyDrawBack = new LegacyDrawList();
                LegacyDrawFront = new LegacyDrawList();
            }
        }

        public override void Unload() {
            music = null;
            LegacyDrawBack?.Clear();
            LegacyDrawBack = null;
            LegacyDrawFront?.Clear();
            LegacyDrawFront = null;
        }

        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 2;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData() {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Suffocation,
                    BuffID.Lovestruck,
                }
            });

            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void SetDefaults() {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 18000;
            NPC.damage = 15;
            NPC.defense = 12;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 10);
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
            NPC.Aequus().noGravityDrops = true;

            if (Main.getGoodWorld) {
                NPC.height *= 10;
            }

            this.SetBiome<GaleStreamsZone>();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void BossLoot(ref string name, ref int potionType) {
            potionType = ModContent.ItemType<GreaterRestorationPotion>();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            this.CreateLoot(npcLoot)
                .AddRelic<DustDevilRelic>()
                .Add(new GuaranteedFlawlesslyRule(ModContent.ItemType<DustDevilTrophy>(), 10))
                .AddBossBag<DustDevilBag>()
                .ExpertDropForCrossModReasons<Stormcloak>()
                .AddPerPlayer<AtmosphericEnergy>(stack: 3)
                .SetCondition(new Conditions.NotExpert())
                //.Add<DustDevilMask>(chance: 7, stack: 1)
                .AddOptions(chance: 1, ModContent.ItemType<PhaseDisc>(), ModContent.ItemType<WindFan>())
                .RegisterCondition();
        }

        public override Color? GetAlpha(Color drawColor) {
            return PhaseTwo ? Color.Red : Color.White;
        }

        public override bool CanHitNPC(NPC target)/* tModPorter Suggestion: Return true instead of null */
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            int count = 1;
            for (int i = 0; i < count; i++) {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, Main.rand.NextBool() ? DustID.Torch : DustID.Frost);
                d.velocity = (d.position - NPC.Center) / 8f;
                if (Main.rand.NextBool(3)) {
                    d.velocity *= 2f;
                    d.scale *= 1.75f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                    d.noGravity = true;
                }
            }
        }

        public override void AI() {
            if (Main.netMode != NetmodeID.Server) {
                if (Tornado == null || !Tornado.IsActive()) {
                    Tornado = new TornadoManipulator() {
                        DustDevil = NPC,
                        range = 2000f,
                        pull = 0.001f,
                    };

                    DustDevilParticleSystem.Manipulators.Add(Tornado);
                }
                Tornado.Position = new Vector3(NPC.Center, 0f);
                Tornado.timeLeft = 4;
                Tornado.pull = 0.6f;

                for (int i = 0; i < 1; i++) {
                    var loc = new Vector3(NPC.Center + new Vector2(Main.rand.NextFloat(-1200, 1200), Main.rand.NextFloat(-1200, 1200)), Main.rand.NextFloat(-NPC.width * 2, NPC.width * 2));
                    if (Main.rand.Next(120) < 118 && !Collision.CanHitLine(new Vector2(loc.X, loc.Y), 2, 2, NPC.position, NPC.width, NPC.height)) {
                        continue;
                    }
                    var p = new DustParticle(loc, new Vector3(Main.rand.NextVector2Square(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), Color.White * 0.8f, Main.rand.NextFloat(0.75f, 1.8f), Main.rand.NextFloat(MathHelper.TwoPi));
                    DustDevilParticleSystem.AddParticle(p);

                    if (Main.rand.NextBool(12)) {
                        loc = new Vector3(NPC.Center + new Vector2(Main.rand.NextFloat(-10, 10), 100f + Main.rand.NextFloat(-NPC.width, NPC.width * 2)), Main.rand.NextFloat(-NPC.width * 2, NPC.width * 2));
                        if (Main.rand.Next(120) < 118 && !Collision.CanHitLine(new Vector2(loc.X, loc.Y), 2, 2, NPC.position, NPC.width, NPC.height)) {
                            continue;
                        }
                        var v = new Vector3(Main.rand.NextVector2Square(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                        for (int j = 0; j < 4; j++) {
                            p = new DustParticle(loc - v * j * 0.2f, v, Color.White, Main.rand.NextFloat(0.75f, 1.5f), Main.rand.NextFloat(MathHelper.TwoPi));
                            p.frame.X = 0;
                            p.OnAdd();
                            DustDevilParticleSystem.AddParticle(p);
                        }
                    }
                }
            }

            AequusNPC.ForceZen(NPC);

            if (Action != ACTION_GOODBYE && !NPC.HasValidTarget) {
                if (!CheckTargets()) {
                    return;
                }
            }

            Lighting.AddLight(new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y), new Vector3(1f, 0.66f, 0f));
            Lighting.AddLight(new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height), new Vector3(0f, 0.66f, 1f));

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(60)) {
                CheckOrbitingBlocks();
            }

            switch (Action) {
                case ACTION_ICE:
                    Ice();
                    break;

                case ACTION_FIREBALLS:
                    Fireballs();
                    break;

                case ACTION_SUCTIONENEMIES:
                    SuctionEnemies();
                    break;

                case ACTION_SUCTIONTILES:
                    SuctionTiles();
                    break;

                case ACTION_INTRO:
                    Intro();
                    break;

                case ACTION_INIT:
                    Action = ACTION_INTRO;
                    break;

                case ACTION_GOODBYE:
                    Goodbye();
                    break;
            }
        }

        public int CalcDamage(float mult = 1f) {
            return (int)((Main.masterMode ? NPC.damage / 3 : Main.expertMode ? NPC.damage / 2 : NPC.damage) * mult);
        }

        public void SetAction(int phase) {
            int secondary = SecondaryAction;
            ClearAI();
            if (secondary == 0) {
                SecondaryAction = phase;
                Action = Main.rand.NextBool() ? ACTION_FIREBALLS : ACTION_ICE;
            }
            else {
                Action = phase;
            }
            NPC.netUpdate = true;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                CheckOrbitingBlocks();
        }

        public void CheckOrbitingBlocks() {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DustDevilTileProj>()) {
                    count++;
                }
            }
            var source = NPC.GetSource_FromAI();
            for (int i = count; i < 200; i++) {
                var v = Main.rand.NextVector2Unit() * Main.rand.NextFloat(200f, 1600f);
                var t = (NPC.Center + v).ToTileCoordinates();
                if (Main.rand.Next(60) < 58 && (!WorldGen.InWorld(t.X, t.Y, 10) || !Main.tile[t].IsFullySolid() || Main.tileFrameImportant[Main.tile[t].TileType])) {
                    if (Main.rand.NextBool(500))
                        i--;
                    continue;
                }
                Projectile.NewProjectile(source, NPC.Center + v, Vector2.Zero, ModContent.ProjectileType<DustDevilTileProj>(), CalcDamage(), 1f, Main.myPlayer, 0f, NPC.whoAmI);
            }
        }

        public bool CheckTargets(float checkDistance = 4000f) {
            NPC.TargetClosest(faceTarget: false);
            NPC.netUpdate = true;
            if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > checkDistance) {
                ClearAI();
                ClearLocalAI();
                NPC.ai[0] = ACTION_GOODBYE;
                return false;
            }
            return true;
        }

        public void Ice() {
            SetFrame(1);
            ActionTimer++;
            if (ActionTimer == 60 || ActionTimer == 120) {
                NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 20f;
                NPC.netUpdate = true;
            }
            if (ActionTimer == 90 || ActionTimer == 180) {
                if (!CheckTargets()) {
                    return;
                }
                int delay = Mode(15, 10, 5);
                if (ActionTimer % delay == 0) {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        float speed = Mode(12f, 20f, 48f);
                        var toPlayer = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);

                        var source = NPC.GetSource_FromAI();
                        int damage = CalcDamage(1.25f);
                        foreach (var r in Helper.Circular(Mode(6, 15, 50), Main.rand.NextFloat(MathHelper.TwoPi))) {
                            Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(r) * speed, ModContent.ProjectileType<DustDevilFrostball>(), damage, 1f, Main.myPlayer, ai0: 1f);
                            Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(r) * speed, ModContent.ProjectileType<DustDevilFrostball>(), damage, 1f, Main.myPlayer, ai0: -1f);
                        }
                    }
                }
                NPC.netUpdate = true;
            }
            if (ActionTimer >= 200) {
                SetAction(SecondaryAction);
            }
            if (NPC.Distance(Main.player[NPC.target].Center) < 2000f) {
                NPC.velocity *= 0.92f;
            }
        }

        public void Fireballs() {
            SetFrame(0);
            ActionTimer++;
            if (ActionTimer == 60 || ActionTimer == 120) {
                NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 40f;
                NPC.netUpdate = true;
            }
            if (ActionTimer >= 90 && ActionTimer < 120 || ActionTimer >= 150 && ActionTimer < 180) {
                if (!CheckTargets()) {
                    return;
                }
                int delay = Mode(15, 10, 5);
                if (ActionTimer % delay == 0) {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        float speed = Mode(12f, 20f, 48f);
                        var toPlayer = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);

                        var source = NPC.GetSource_FromAI();
                        int amt = Mode(1, 3, 7);
                        int damage = CalcDamage(1.25f);
                        float r = Main.rand.NextFloat(-0.5f, 0.5f);
                        for (int i = 0; i < amt; i++) {
                            Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(MathHelper.PiOver4 / 2f * i + r) * speed, ModContent.ProjectileType<DustDevilFireball>(), damage, 1f, Main.myPlayer, ai0: 1f);
                            if (i != 0)
                                Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(MathHelper.PiOver4 / 2f * -i + r) * speed, ModContent.ProjectileType<DustDevilFireball>(), damage, 1f, Main.myPlayer, ai0: -1f);
                        }
                    }
                }
                NPC.netUpdate = true;
            }
            if (ActionTimer >= 200) {
                SetAction(SecondaryAction);
            }
            if (NPC.Distance(Main.player[NPC.target].Center) < 2000f) {
                NPC.velocity *= 0.92f;
            }
        }

        public Point ScanTilePoint(Vector2 dir) {
            var center = NPC.Center;
            dir = Vector2.Normalize(dir) * 16f;
            int scanTiles = 75;
            for (int i = 0; i < scanTiles; i++) {
                var point = (center + dir * i).ToTileCoordinates();
                if (!WorldGen.InWorld(point.X, point.Y, 10) || Main.tile[point].IsSolid()) {
                    return point;
                }
            }
            return (center + dir * scanTiles).ToTileCoordinates();
        }

        public void SuctionEnemies() {
            ActionTimer++;
            if (ActionTimer <= 120) {
                int delay = Mode(25, 50, 75);

                if (ActionTimer % delay == 0) {
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        if (SuctionEnemies_EnsureYouWontBreakTheNPCCap()) {
                            var point = ScanTilePoint(Main.rand.NextVector2Unit()).ToWorldCoordinates() + new Vector2(8f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), point, Vector2.Normalize(NPC.Center - point),
                                ModContent.ProjectileType<SuctionedEnemy>(), CalcDamage(), 1f, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }
                }
                NPC.velocity = -Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
            }
            else {
                if (ActionTimer > 400) {
                    SetAction(ACTION_SUCTIONTILES);
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * Mode(3f, 6f, 6f), 0.1f);
            }
        }
        public bool SuctionEnemies_EnsureYouWontBreakTheNPCCap() {
            return Main.npc.Count((n) => n.active) <= Main.maxNPCs - 20;
        }

        public void SuctionTiles() {
            if ((int)ActionTimer == 0) {
                if (!CheckTargets()) {
                    return;
                }
                NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 40f;
            }
            else if (NPC.Distance(Main.player[NPC.target].Center) < 2000f) {
                NPC.velocity *= 0.9f;
            }

            ActionTimer++;
            if (ActionTimer <= 150) {
                if (ActionTimer == 150) {
                    if (!CheckTargets()) {
                        return;
                    }
                }
                else if (ActionTimer < 80 && ActionTimer > 20) {
                    int delay = Mode(8, 3, 1);

                    if ((ActionTimer - 20) % delay == 0) {
                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            for (int i = 0; i < Main.maxProjectiles; i++) {
                                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DustDevilTileProj>() && Main.projectile[i].ai[0] == DustDevilTileProj.STATE_ORBITING) {
                                    if (Main.rand.NextBool(10)) {
                                        Main.projectile[i].ai[0] = DustDevilTileProj.STATE_BEINGPULLED;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else {
                if (ActionTimer > 240) {
                    SetAction(ACTION_SUCTIONENEMIES);
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * Mode(3f, 6f, 6f), 0.1f);
            }
        }

        public void Intro() {
            if (ActionTimer < 120f) {
                if ((int)ActionTimer == 0) {
                    if (!CheckTargets()) {
                        return;
                    }
                    NPC.velocity.X = 0f;
                    NPC.velocity.Y = 30f;
                }
                else {
                    NPC.velocity.Y *= 0.975f;
                    if (NPC.velocity.Y < 0.1f) {
                        NPC.velocity.Y = 0f;
                    }
                }
                NPC.ai[1]++;
            }
            else {
                if ((int)ActionTimer == 0) {
                    ActionTimer++;
                    if (!CheckTargets()) {
                        return;
                    }
                }
                SetAction(ACTION_SUCTIONTILES);
            }
        }

        public void Goodbye() {
            if (NPC.ai[1] < 60f) {
                NPC.alpha += 5;
                NPC.velocity.X *= 0.8f;
                if ((int)NPC.ai[1] == 1) {
                    NPC.velocity.Y = -6f;
                }
                else {
                    NPC.velocity.Y *= 0.985f;
                }
                NPC.ai[1]++;
            }
            else {
                NPC.alpha = 255;
                NPC.timeLeft = 0;
                NPC.active = false;
                NPC.life = -1;
            }
        }

        public static void AddLegacyDraw(int proj, float z) {
            if (z > 0f) {
                LegacyDrawBack.Add(proj);
                return;
            }
            LegacyDrawFront.Add(proj);
        }

        public static bool CurrentlyLegacyDrawing(float z) {
            return z > 0f ? LegacyDrawBack.RenderingNow : LegacyDrawFront.RenderingNow;
        }

        public override void OnKill() {
            AequusWorld.MarkAsDefeated(ref AequusWorld.downedDustDevil, Type);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            NPC.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);

            DrawHead(spriteBatch, texture, offset, screenPos, frame, origin);

            return false;
        }

        public void DrawHead(SpriteBatch spriteBatch, Texture2D texture, Vector2 offset, Vector2 screenPos, Rectangle frame, Vector2 origin) {
            var circular = Helper.CircularVector(8, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < 2; i++) {
                float time = (auraTimer / 20f + 10f * i) % 20f;
                foreach (var v in circular) {
                    spriteBatch.Draw(texture, NPC.position + offset + v * time - screenPos, frame, Color.White.UseA(0) * (1f - time / 20f) * 0.33f,
                        NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }
            spriteBatch.Draw(texture, NPC.position + offset - screenPos, frame, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }

        public static void GetLegacyTornadoInfo(float npcHeight, float y, out float start, out float end, out float progress) {
            start = npcHeight * 1.5f;
            end = -start;
            progress = (-y + start) / (start * 2f);
        }
    }
}