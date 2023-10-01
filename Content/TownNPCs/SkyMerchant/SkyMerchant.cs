using Aequus.Common.NPCs;
using Aequus.Content.TownNPCs.SkyMerchant.Emote;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.TownNPCs.SkyMerchant;

[AutoloadHead]
public partial class SkyMerchant : AequusTownNPC<SkyMerchant> {
    public enum MovementState {
        Init,
        Walking,
        Ballooning,
    }

    public MovementState state;
    public float balloonOpacity;
    public int target;

    public override void SetDefaults() {
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
        NPC.rarity = 2;
        AnimationType = NPCID.Merchant;
        balloonOpacity = 1f;
    }

    #region Initialization
    public override void Load() {
        if (!Main.dedServ) {
            LoadDrawSets();
        }
    }

    public override void Unload() {
        UnloadDrawSets();
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 90;
        NPCID.Sets.AttackAverageChance[Type] = 50;
        NPCID.Sets.HatOffsetY[Type] = 0;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
        NPCID.Sets.ActsLikeTownNPC[Type] = true;
        NPCID.Sets.SpawnsWithCustomName[Type] = true;
        NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<SkyMerchantEmote>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.SkyBiome)
            .AddSpawn(BestiaryBuilder.DayTime);
    }
    #endregion

    #region AI
    private bool NearStoppingPoint() {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && !Main.player[i].DeadOrGhost && NPC.Distance(Main.player[i].Center) < 200f) {
                return true;
            }
        }
        return false;
    }

    private void BalloonMovement() {
        Vector2 gotoPosition = new Vector2(SkyMerchantSystem.SkyMerchantX * 16f, 2000f);
        var wantedVelocity = NPC.DirectionTo(gotoPosition);
        NPC.direction = 1;
        NPC.spriteDirection = 1;
        if (wantedVelocity.X < 0f) {
            NPC.velocity.X *= 0.95f;
        }
        else {
            NPC.velocity = Vector2.Lerp(NPC.velocity, wantedVelocity * 3f, 0.01f);
        }
    }

    public override bool PreAI() {
        SkyMerchantSystem.SpawnCheck = 0;
        NPC.aiStyle = NPCAIStyleID.Passive;
        target = -1;

        drawSet = !NPC.IsShimmerVariant ? DefaultDrawSet : ShimmerDrawSet;

        if (state == MovementState.Init) {
            state = MovementState.Ballooning;
            // Setup instanced shop here
            return false;
        }
        if (NPC.ai[0] == 25f) {
            balloonOpacity = 0f;
            return true;
        }
        if (balloonOpacity < 1f) {
            balloonOpacity += 0.025f;
            if (balloonOpacity > 1f) {
                balloonOpacity = 1f;
            }
        }

        if (state == MovementState.Ballooning) {
            NPC.noGravity = true;
            DrawOffsetY = MathF.Sin(Main.GlobalTimeWrappedHourly) * 4f;
            if (NPC.shimmerWet || NPC.shimmering) {
                balloonOpacity = Math.Min(1f - NPC.shimmerTransparency, balloonOpacity);
                NPC.velocity *= 0.95f;
                return true;
            }
            if (balloonOpacity < 1f) {
                NPC.velocity.Y += (1f - balloonOpacity) * 0.3f;
            }

            int attackRange = NPCID.Sets.DangerDetectRange[Type] == -1 ? 200 : NPCID.Sets.DangerDetectRange[Type];
            float closestDistance = attackRange;
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].damage > 0 && (Main.npc[i].noTileCollide || Collision.CanHit(NPC.Center, 0, 0, Main.npc[i].Center, 0, 0)) && NPCLoader.CanHitNPC(Main.npc[i], NPC)) {
                    float distance = NPC.Distance(Main.npc[i].Center);
                    if (distance > attackRange) {
                        continue;
                    }
                    target = i;
                    closestDistance = distance;
                }
            }

            if (target != -1) {
                if (closestDistance < Math.Max(Main.npc[target].Size.Length() * 3f, 100f)) {
                    NPC.velocity += NPC.DirectionFrom(Main.npc[target].Center) * 0.033f;
                }
                else {
                    NPC.velocity *= 0.966f;
                }
                return false;
            }

            if (NearStoppingPoint()) {
                NPC.velocity *= 0.95f;
                return false;
            }

            BalloonMovement();
            return false;
        }

        DrawOffsetY = 0f;
        NPC.noGravity = false;
        return true;
    }

    public override void AI() {
    }
    #endregion

    #region Chat
    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
        button2 = this.GetLocalizedValue("Interface.RenameButton");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
        }
    }

    public override bool CanChat() {
        return true;
    }

    public override string GetChat() {
        string key = Main.rand.Next(5).ToString();
        if (!Main.dayTime && Main.rand.NextBool(3)) {
            key = "Night";
        }
        if (Main.LocalPlayer.ZoneGraveyard && Main.rand.NextBool(3)) {
            key = "Graveyard";
        }
        if (Main.IsItStorming && Main.rand.NextBool(3)) {
            key = "Thunderstorm";
        }
        if (Main.bloodMoon && Main.rand.NextBool(3)) {
            key = "BloodMoon";
        }
        if (Main.LocalPlayer.ZoneGlimmer() && Main.rand.NextBool(3)) {
            key = "Glimmer";
        }
        if (Main.eclipse && Main.rand.NextBool(3)) {
            key = "Eclipse";
        }
        if (NPC.AnyNPCs(NPCID.Merchant) && Main.rand.NextBool(5)) {
            key = "Merchant";
        }
        if (NPC.AnyNPCs(NPCID.Pirate) && Main.rand.NextBool(5)) {
            key = "Pirate";
        }
        if (NPC.AnyNPCs(NPCID.Steampunker) && Main.rand.NextBool(5)) {
            key = "Steampunker";
        }
        if (NPC.AnyNPCs(NPCID.TravellingMerchant) && Main.rand.NextBool(3)) {
            key = "TravellingMerchant";
        }
        if (NPC.AnyNPCs(NPCID.Demolitionist) && Main.rand.NextBool(5)) {
            key = "Demolitionist";
        }
        return this.GetLocalization("Dialogue." + key).FormatWith(Lang.CreateDialogSubstitutionObject(NPC));
    }
    #endregion

    #region Names
    public override List<string> SetNPCNameList() {
        return new() {
            "Link",
            "Buddy",
            "Dobby",
            "Hermey",
            "Calcelmo",
            "Ancano",
            "Nurelion",
            "Vingalmo",
            "Faendal",
            "Malborn",
            "Niruin",
            "Enthir",
            "Araena",
            "Ienith",
            "Brand-Shei",
            "Erandur",
            "Neloth",
            "Gelebor",
            "Vyrthur",
        };
    }
    #endregion
}