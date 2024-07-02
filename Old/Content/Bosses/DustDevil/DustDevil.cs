using Aequus.Core;
using Aequus.Content.Events.GaleStreams;
using Aequus.Content.Items.Potions.Healing.Restoration;
using Aequus.Core.CodeGeneration;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Entities.Bestiary;
using Aequus.Old.Content.Bosses.DustDevil.Items;
using Aequus.Old.Content.Bosses.DustDevil.Projectiles;
using Aequus.Old.Content.Items.Materials.Energies;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Old.Content.Bosses.DustDevil;

[Gen.AequusSystem_WorldField<bool>("downedDustDevil")]
[BestiaryBiome<GaleStreamsZone>()]
[AutoloadBossHead]
public class DustDevil : LegacyAequusBoss {
    public const float BossProgression = 8.5f;

    #region States
    public const int ICE = 5;
    public const int FIREBALLS = 4;
    public const int SUCTIONENEMIES = 3;
    public const int SUCTIONTILES = 2;
    #endregion

    public float HPRatio => NPC.life / (float)NPC.lifeMax;
    public bool PhaseTwo => NPC.life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

    public int SecondaryState { get => (int)NPC.ai[2]; set => NPC.ai[2] = value; }
    public float DirectStateTimer { get => NPC.ai[3]; set => NPC.ai[3] = value; }

    public int effectsTimer;
    public int auraTimer;

    public DustDevil() : base(new BossParams() {
        ItemRarity = Commons.Rare.BossDustDevil
    }) { }

    public override void Load() {
        MaskItem = new DustDevilMask(Name);

        base.Load();

        Mod.AddContent(MaskItem);
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 2;

        NPCSets.MPAllowedEnemies[Type] = true;
        NPCSets.BossBestiaryPriority.Add(Type);
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Wet] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Suffocation] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Lovestruck] = true;
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
        NPC.npcSlots = 20f;

        if (Main.getGoodWorld) {
            NPC.height *= 10;
        }
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void BossLoot(ref string name, ref int potionType) {
        potionType = ModContent.ItemType<GreaterRestorationPotion>();
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        base.ModifyNPCLoot(npcLoot);

        //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.GetInstance<DustDevilPet>().PetItem.Type, chanceDenominator: 4));
        //npcLoot.AddBossDrop(ItemDropRule.Common(ModContent.ItemType<Stormcloak>());
        npcLoot.Add(new DropPerPlayerOnThePlayer(EnergyMaterial.Atmospheric.Type, chanceDenominator: 1, amountDroppedMinimum: 3, amountDroppedMaximum: 3, optionalCondition: null));
        npcLoot.AddBossDrop(ItemDropRule.OneFromOptions(1,
            ModContent.ItemType<Content.Items.Weapons.Melee.Phasebrink.Phasebrink>()
        ));
    }

    public override Color? GetAlpha(Color drawColor) {
        return PhaseTwo ? Color.Red : Color.White;
    }

    public override bool CanHitNPC(NPC target) {
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
        if (State != GOODBYE && !NPC.HasValidTarget) {
            if (!CheckTargets()) {
                return;
            }
        }

        Lighting.AddLight(new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y), new Vector3(1f, 0.66f, 0f));
        Lighting.AddLight(new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height), new Vector3(0f, 0.66f, 1f));

        if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(60)) {
            CheckOrbitingBlocks();
        }

        switch (State) {
            case ICE:
                Ice();
                break;

            case FIREBALLS:
                Fireballs();
                break;

            case SUCTIONENEMIES:
                SuctionEnemies();
                break;

            case SUCTIONTILES:
                SuctionTiles();
                break;

            case INTRO:
                Intro();
                break;

            case INIT:
                State = INTRO;
                break;

            case GOODBYE:
                Goodbye();
                break;
        }
    }

    public int CalcDamage(float mult = 1f) {
        return (int)((Main.masterMode ? NPC.damage / 3 : Main.expertMode ? NPC.damage / 2 : NPC.damage) * mult);
    }

    public void SetState(int phase) {
        int secondary = SecondaryState;
        ClearAI();
        if (secondary == 0) {
            SecondaryState = phase;
            State = Main.rand.NextBool() ? FIREBALLS : ICE;
        }
        else {
            State = phase;
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
            NPC.ai[0] = GOODBYE;
            return false;
        }
        return true;
    }

    public void Ice() {
        SetFrame(1);
        StateTimer++;
        if (StateTimer == 60 || StateTimer == 120) {
            NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 20f;
            NPC.netUpdate = true;
        }
        if (StateTimer == 90 || StateTimer == 180) {
            if (!CheckTargets()) {
                return;
            }
            int delay = Mode(15, 10, 5);
            if (StateTimer % delay == 0) {
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
        if (StateTimer >= 200) {
            SetState(SecondaryState);
        }
        if (NPC.Distance(Main.player[NPC.target].Center) < 2000f) {
            NPC.velocity *= 0.92f;
        }
    }

    public void Fireballs() {
        SetFrame(0);
        StateTimer++;
        if (StateTimer == 60 || StateTimer == 120) {
            NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 40f;
            NPC.netUpdate = true;
        }
        if (StateTimer >= 90 && StateTimer < 120 || StateTimer >= 150 && StateTimer < 180) {
            if (!CheckTargets()) {
                return;
            }
            int delay = Mode(15, 10, 5);
            if (StateTimer % delay == 0) {
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
        if (StateTimer >= 200) {
            SetState(SecondaryState);
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
        StateTimer++;
        if (StateTimer <= 120) {
            int delay = Mode(25, 50, 75);

            if (StateTimer % delay == 0) {
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
            if (StateTimer > 400) {
                SetState(SUCTIONTILES);
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * Mode(3f, 6f, 6f), 0.1f);
        }
    }
    public bool SuctionEnemies_EnsureYouWontBreakTheNPCCap() {
        return Main.npc.Count((n) => n.active) <= Main.maxNPCs - 20;
    }

    public void SuctionTiles() {
        if ((int)StateTimer == 0) {
            if (!CheckTargets()) {
                return;
            }
            NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 40f;
        }
        else if (NPC.Distance(Main.player[NPC.target].Center) < 2000f) {
            NPC.velocity *= 0.9f;
        }

        StateTimer++;
        if (StateTimer <= 150) {
            if (StateTimer == 150) {
                if (!CheckTargets()) {
                    return;
                }
            }
            else if (StateTimer < 80 && StateTimer > 20) {
                int delay = Mode(8, 3, 1);

                if ((StateTimer - 20) % delay == 0) {
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
            if (StateTimer > 240) {
                SetState(SUCTIONENEMIES);
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * Mode(3f, 6f, 6f), 0.1f);
        }
    }

    public void Intro() {
        if (StateTimer < 120f) {
            if ((int)StateTimer == 0) {
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
            if ((int)StateTimer == 0) {
                StateTimer++;
                if (!CheckTargets()) {
                    return;
                }
            }
            SetState(SUCTIONTILES);
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

    public override void OnKill() {
        NPC.SetEventFlagCleared(ref AequusSystem.downedOmegaStarite, -1);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        NPC.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);

        DrawHead(spriteBatch, texture, offset, screenPos, frame, origin);

        return false;
    }

    public void DrawHead(SpriteBatch spriteBatch, Texture2D texture, Vector2 offset, Vector2 screenPos, Rectangle frame, Vector2 origin) {
        IEnumerable<Vector2> circular = Helper.CircularVector(8, Main.GlobalTimeWrappedHourly);
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