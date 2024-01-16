using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.DataSets;
using Aequus.Content.Tiles.Banners;
using Aequus.Core.Graphics;
using System;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;

[AutoloadBanner]
[ModBiomes(typeof(PollutedOceanBiome))]
public partial class BlackJellyfish : AIJellyfish {
    public static int AttackRange => 60;

    #region Initialization
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;
        NPCSets.PushableByTypeId.AddEntry(Type);
        DrawLayers.Instance.PostDrawLiquids += DrawExplodingJellyfishesLayer;
    }

    public override void SetDefaults() {
        NPC.noGravity = true;
        NPC.width = 26;
        NPC.height = 26;
        NPC.aiStyle = -1;
        NPC.damage = 25;
        NPC.defense = 4;
        NPC.lifeMax = 34;
        NPC.HitSound = SoundID.NPCHit25;
        NPC.DeathSound = SoundID.NPCDeath28;
        NPC.value = Item.silver;
        AnimationType = NPCID.BlueJellyfish;
        shockAttackLength = 70;
        dashSpeed = 8f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ItemID.Glowstick, minimumDropped: 1, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.JellyfishNecklace, 100));
    }
    #endregion

    public float GetLightingIntensity() {
        return GetLightingIntensity(LightHelper.GetLightColor(NPC.Center));
    }
    public float GetLightingIntensity(Color lightColor) {
        return Math.Clamp(MathF.Pow(lightColor.ToVector3().Length(), 4f), 0.45f, 1f);
    }

    public override void AI() {
        if (Main.netMode != NetmodeID.Server) {
            float lightingIntensity = GetLightingIntensity();
            if (lightingIntensity <= 0.75f) {
                NPC.ShowNameOnHover = false;
                NPC.nameOver = 0f;
            }
            else {
                NPC.ShowNameOnHover = true;
            }
        }
        if (NPC.ai[2] > 0f || (NPC.wet && NPC.direction != 0 && NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < shockRampUpDistance && NPC.velocity.Length() > (dashSpeed / 2f))) {
            NPC.ai[2]++;
            if (NPC.ai[2] <= shockAttackLength - 16) {
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            NPC.velocity *= 0.99f;
            if (!NPC.wet) {
                NPC.velocity *= 0.9f;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * NPC.velocity.Length(), 0.05f);
            if (NPC.justHit && NPC.ai[2] < shockAttackLength) {
                NPC.ai[2] -= 8f;
                NPC.ai[2] *= 0.75f;
            }
            if ((int)NPC.ai[2] == shockAttackLength) {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                for (int i = 0; i < 30; i++) {
                    var randomVector = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 1.8f));
                    d.rotation = 0f;
                    d.velocity = randomVector * Main.rand.NextFloat(4f);
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.8f);
                    d.noGravity = true;
                }
                for (int i = 0; i < 60; i++) {
                    var randomVector = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(NPC.Center + randomVector * Main.rand.NextFloat(0.8f, 1f) * NPC.ai[2] / shockAttackLength * AttackRange, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 2.5f));
                    d.rotation = 0f;
                    d.velocity += randomVector * 4f * Main.rand.NextFloat();
                    d.noGravity = true;
                }
                NPC.velocity = Vector2.Zero;
                NPC.netUpdate = true;
            }
            else {
                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.33f, 1f) * Math.Min(NPC.ai[2] / shockAttackLength, 1f) * AttackRange, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 1.8f));
                d.rotation = 0f;
                d.noGravity = true;
            }
            if (NPC.ai[2] >= shockAttackLength) {
                var center = NPC.Center;
                NPC.width = AttackRange * 2;
                NPC.height = AttackRange * 2;
                NPC.Center = center;
                NPC.noTileCollide = true;
                NPC.dontTakeDamage = true;
            }
            //if (NPC.ai[2] > shockAttackLength) {
            //    NPC.ai[2] -= 0.9f;
            //}
            if ((int)NPC.ai[2] > shockAttackLength + 14f && Main.netMode != NetmodeID.MultiplayerClient) {
                NPC.active = false;
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                }
            }
            return;
        }
        NPC.ai[2] = 0f;
        base.AI();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return NPC.ai[2] >= shockAttackLength && NPC.ai[2] < shockAttackLength + 4f;
    }

    public override bool CanShock() {
        return false;
    }
}