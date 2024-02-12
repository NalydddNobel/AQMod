using System;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace Aequus.Old.Content.Critters;

[LegacyName("DwarfStariteCritter")]
public class DwarfStarite : ModNPC {
    //private TrailRenderer _trail;
    //private TrailRenderer _constellationTrail;
    //public float rotationSpeed;
    //public int constellation;
    //public int Constellation { get => constellation - 1; set => constellation = value + 1; }

    //public override void SetStaticDefaults() {
    //    Main.npcFrameCount[Type] = 2;
    //    Main.npcCatchable[Type] = true;
    //    NPCSets.CountsAsCritter[Type] = true;
    //    NPCSets.ShimmerTransformToNPC[Type] = NPCID.Shimmerfly;
    //    NPCSets.TrailingMode[Type] = 7;
    //    NPCSets.TrailCacheLength[Type] = 60;
    //    NPCSets.ImmuneToRegularBuffs[Type] = true;
    //}

    //public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
    //    this.CreateEntry(database, bestiaryEntry);
    //}

    //public override void SetDefaults() {
    //    NPC.width = 12;
    //    NPC.height = 12;
    //    NPC.aiStyle = -1;
    //    NPC.damage = 0;
    //    NPC.defense = 0;
    //    NPC.lifeMax = 5;
    //    NPC.HitSound = SoundID.NPCHit5;
    //    NPC.DeathSound = SoundID.NPCDeath55;
    //    NPC.npcSlots = 0.1f;
    //    NPC.noGravity = true;
    //    NPC.catchItem = (short)ModContent.ItemType<DwarfStariteItem>();

    //    this.SetBiome<GlimmerZone>();
    //}

    //public override Color? GetAlpha(Color drawColor) {
    //    return new Color(255, 255, 255, 0);
    //}

    //public override void HitEffect(NPC.HitInfo hit) {
    //    if (Main.netMode == NetmodeID.Server) {
    //        return;
    //    }

    //    float x = NPC.velocity.X.Abs() * hit.HitDirection;
    //    if (NPC.life <= 0) {
    //        if (NPC.life == -33333) {
    //            for (int i = 0; i < 60; i++) {
    //                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Main.rand.NextFloat(0.2f, 1f)).UseA(0));
    //                d.velocity *= 0.2f;
    //                d.velocity += d.position - NPC.Center;
    //            }
    //            for (int i = 0; i < 50; i++) {
    //                var b = ParticleSystem.Fetch<MonoBloomParticle>().Setup(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 12), Vector2.Zero, Color.White.UseA(0), new Color(25, 25, 40, 0), Main.rand.NextFloat(0.8f, 1.45f), 0.33f);
    //                b.Velocity += (b.Position - NPC.Center) / 2f;
    //                ParticleSystem.GetLayer(ParticleLayer.AboveDust).Add(b);
    //            }
    //            for (int i = 0; i < 20; i++) {
    //                var b = ParticleSystem.Fetch<MonoBloomParticle>().Setup(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(10, 42), Vector2.Zero, Color.White.UseA(0), new Color(25, 25, 40, 0), Main.rand.NextFloat(0.8f, 1.45f), 0.33f);
    //                b.Velocity += (b.Position - NPC.Center) / 3f;
    //                ParticleSystem.GetLayer(ParticleLayer.AboveDust).Add(b);
    //            }
    //            for (int i = 0; i < 25; i++) {
    //                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), DustID.Enchanted_Gold + Main.rand.Next(2), newColor: Color.White.UseA(0));
    //                d.velocity *= 0.1f;
    //                d.velocity += (d.position - NPC.Center) / 2f;
    //            }
    //        }
    //        for (int i = 0; i < 15; i++) {
    //            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
    //            d.velocity *= 0.1f;
    //            d.velocity += d.position - NPC.Center;
    //        }
    //        for (int i = 0; i < 3; i++) {
    //            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-2f, 2f) + x, Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
    //        }
    //    }
    //    else {
    //        for (int i = 0; i < 3; i++) {
    //            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie);
    //            Main.dust[d].velocity.X += x;
    //            Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
    //        }
    //        if (Main.rand.NextBool()) {
    //            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
    //            Main.dust[d].velocity.X += x;
    //            Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
    //        }
    //        if (Main.rand.NextBool())
    //            Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
    //    }
    //}

    //public override void AI() {
    //    var aequus = NPC.Aequus();
    //    if (Main.dayTime && aequus.lastHit > 60) {
    //        aequus.noOnKill = true;
    //    }
    //    else {
    //        aequus.noOnKill = false;
    //    }

    //    if (Constellation == -1) {
    //        for (int i = 0; i < Main.maxNPCs; i++) {
    //            if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].type == Type && NPC.Distance(Main.npc[i].Center) < 360f && NPC.CanHitLine(Main.npc[i])) {
    //                if (Main.npc[i].ModNPC is DwarfStarite dwarfStarite && dwarfStarite.Constellation == NPC.whoAmI) {
    //                    continue;
    //                }
    //                Constellation = i;
    //                break;
    //            }
    //        }
    //    }
    //    else if (!Main.npc[Constellation].active || Main.npc[Constellation].type != Type) {
    //        Constellation = -1;
    //    }
    //    else if (NPC.Distance(Main.npc[Constellation].Center) > 200f) {
    //        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Main.npc[Constellation].Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 10f, 0.01f);
    //        Main.npc[Constellation].velocity = Vector2.Lerp(Main.npc[Constellation].velocity, Main.npc[Constellation].DirectionTo(NPC.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 10f, 0.01f);
    //    }

    //    if ((int)NPC.ai[1] == -1) {
    //        if (NPC.ai[3] > 0f)
    //            NPC.ai[3] = 0f;
    //        NPC.ai[3] -= 0.66f;
    //        if (Main.rand.NextBool()) {
    //            var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[3] / 2f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Math.Min(Main.rand.NextFloat(0.5f, 1f) - NPC.ai[3] / 60f, 1f)).UseA(0));
    //            d.velocity *= 0.2f;
    //            d.velocity += (NPC.Center - d.position) / 16f;
    //        }
    //        if (NPC.ai[3] < -60f) {
    //            NPC.life = -33333;
    //            NPC.HitEffect();
    //            NPC.checkDead();
    //        }
    //        return;
    //    }
    //    int tileHeight = 0;
    //    int tileX = ((int)NPC.position.X + NPC.width) / 16;
    //    int tileY = ((int)NPC.position.Y + NPC.height) / 16;
    //    for (int i = 0; i < 10; i++) {
    //        if (WorldGen.InWorld(tileX, tileY + i, 10) && Main.tile[tileX, tileY + i].IsSolid()) {
    //            tileHeight = i + 1;
    //            break;
    //        }
    //    }
    //    if (tileHeight == 10) {
    //        NPC.ai[0] = 0.5f;
    //    }
    //    else {
    //        if ((int)NPC.ai[1] <= 0) {
    //            NPC.ai[0] = Main.rand.NextFloat(-1f, 1f);
    //            NPC.ai[1] = Main.rand.Next(20, 80);
    //        }
    //        else {
    //            NPC.ai[1]--;
    //            if (NPC.collideX) {
    //                NPC.ai[0] = -NPC.ai[0];
    //                NPC.velocity.Y = NPC.oldVelocity.Y * 0.8f;
    //            }
    //        }
    //    }
    //    if ((int)NPC.ai[3] <= 0) {
    //        NPC.ai[2] = Main.rand.NextFloat(-2f, 2f);
    //        NPC.ai[3] = Main.rand.Next(120, 600);
    //    }
    //    else {
    //        NPC.ai[3]--;
    //        if (NPC.collideX) {
    //            NPC.ai[2] = -NPC.ai[2];
    //            NPC.velocity.X = NPC.oldVelocity.X * 0.8f;
    //        }
    //    }
    //    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.ai[2] * 3f, 0.02f);
    //    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, NPC.ai[0] * 3f, 0.02f);
    //    NPC.rotation += NPC.velocity.X * 0.004f;
    //    if ((int)NPC.frameCounter == 0 && Main.rand.NextBool(400)) {
    //        rotationSpeed = 1f;
    //    }
    //    if (rotationSpeed <= 0.01f) {
    //        rotationSpeed = 0f;
    //    }
    //    else {
    //        rotationSpeed *= 0.95f;
    //        NPC.rotation += rotationSpeed * 0.1f;
    //    }
    //    if (rotationSpeed > 0.1f) {
    //        if (Main.rand.NextBool(10)) {
    //            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
    //            Main.dust[d].velocity = NPC.velocity * 0.01f;
    //        }
    //        if (Main.rand.NextBool(20)) {
    //            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
    //            Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
    //            Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
    //        }
    //        if (Main.rand.NextBool(20)) {
    //            int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
    //            Main.gore[g].scale *= 0.6f;
    //        }
    //    }
    //    else {
    //        if (Main.rand.NextBool(20)) {
    //            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
    //            Main.dust[d].velocity = NPC.velocity * 0.1f;
    //            Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
    //        }
    //        if (Main.rand.NextBool(120)) {
    //            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
    //            Main.dust[d].velocity.X = Main.rand.NextFloat(-1f, 1f);
    //            Main.dust[d].velocity.Y = Main.rand.NextFloat(-1f, 1f);
    //            Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
    //        }
    //        if (Main.rand.NextBool(120)) {
    //            int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), 16 + Main.rand.Next(2));
    //            Main.gore[g].scale *= 0.3f;
    //        }
    //    }
    //    Lighting.AddLight(NPC.Center, new Vector3(0.1f, 0.1f, 0.01f));
    //}

    //public override void UpdateLifeRegen(ref int damage) {
    //    if (Main.dayTime && !Helper.ShadedSpot(NPC.Center) && !Main.remixWorld) {
    //        NPC.lifeRegen = -1;
    //    }
    //}

    //public override void FindFrame(int frameHeight) {
    //    if ((int)NPC.ai[1] == -1) {
    //        return;
    //    }

    //    NPC.frameCounter++;
    //    if (NPC.frameCounter >= 10.0) {
    //        NPC.frameCounter = 0.0;
    //        NPC.frame.Y = (NPC.frame.Y + frameHeight) % (frameHeight * Main.npcFrameCount[Type]);
    //    }
    //}

    //public override float SpawnChance(NPCSpawnInfo spawnInfo) {
    //    return 0f;
    //}

    //public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
    //    var texture = TextureAssets.Npc[NPC.type].Value;
    //    var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
    //    var origin = NPC.frame.Size() / 2f;
    //    var drawPos = NPC.Center - screenPos;

    //    Main.spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
    //    Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(20, 20, 20, 0), NPC.rotation, origin, NPC.scale + 0.3f, SpriteEffects.None, 0f);

    //    var colorEnd = Main.tenthAnniversaryWorld ? Color.HotPink : Color.Blue;
    //    var colorStart = Main.tenthAnniversaryWorld ? Color.Pink : Color.Cyan;
    //    _trail ??= new(TrailTextures.Trail[2].Value, TrailRenderer.DefaultPass, f => new Vector2(4f + 2f * (1f - f)), f => Color.Lerp(colorStart, colorEnd, f) * (1f - f), drawOffset: NPC.Size / 2f);
    //    _trail.Draw(NPC.oldPos);
    //    if (constellation > 0 && !NPC.IsABestiaryIconDummy) {
    //        var constellationColorStart = Main.tenthAnniversaryWorld ? Color.Pink : Color.Blue;
    //        var constellationColorEnd = Main.tenthAnniversaryWorld ? Color.HotPink : Color.DeepSkyBlue;

    //        var coords = Helper.LinearInterpolationBetween(NPC.position, Main.npc[Constellation].position, 100);
    //        _constellationTrail ??= new(TrailTextures.Trail[0].Value, TrailRenderer.DefaultPass, f => new Vector2(10f), f => Color.Lerp(constellationColorStart, constellationColorEnd, MathF.Pow(MathF.Sin(f * MathHelper.Pi), 2f) * 0.6f) * MathF.Sin(f * MathHelper.Pi), drawOffset: NPC.Size / 2f);
    //        _constellationTrail.Draw(coords);
    //    }
    //    return false;
    //}
}

[LegacyName("DwarfStarite")]
public class DwarfStariteItem : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.DefaultToCapturedCritter((short)ModContent.NPCType<DwarfStarite>());
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 10);
    }
}