using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Components;
using Aequus.Common.UI;
using Aequus.Content.TownNPCs.SkyMerchant.Emote;
using Aequus.Content.TownNPCs.SkyMerchant.UI;
using Aequus.Content.Weapons.Ranged.Bows.SkyHunterCrossbow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace Aequus.Content.TownNPCs.SkyMerchant;

public partial class SkyMerchant : AequusTownNPC<SkyMerchant>, ICustomMapHead {
    public enum MovementState {
        Init,
        Walking,
        Ballooning,
    }

    public MovementState state;
    public float balloonOpacity;
    public float balloonBobbing;
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
        NPC.townNPC = true;

        TownNPCStayingHomeless = true;

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
        NPCID.Sets.AttackType[Type] = 1;
        NPCID.Sets.AttackTime[Type] = 60;
        NPCID.Sets.AttackAverageChance[Type] = 20;
        NPCID.Sets.DangerDetectRange[Type] = 632;
        NPCID.Sets.HatOffsetY[Type] = 0;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
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
        Vector2 gotoPosition = new Vector2(SkyMerchantSystem.SkyMerchantX * 16f, Helper.Oscillate((float)Main.time / 1000f, (float)Helper.ZoneSkyHeightY / 2f, (float)Helper.ZoneSkyHeightY * 16f - 200f));
        //Dust.NewDustPerfect(gotoPosition, DustID.Torch);
        var wantedVelocity = NPC.DirectionTo(gotoPosition);
        NPC.direction = 1;
        NPC.spriteDirection = 1;
        if (wantedVelocity.X < 0f) {
            NPC.velocity *= 0.95f;
        }
        else {
            NPC.velocity = Vector2.Lerp(NPC.velocity, wantedVelocity * 3f, 0.01f);
        }
    }

    private void BalloonAttack() {
        NPC.ai[1]++;
        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] > 0f && Main.rand.NextBool(NPCID.Sets.AttackAverageChance[Type])) {
            var velocity = NPC.DirectionTo(Main.npc[target].Center + Main.npc[target].velocity * 10f);
            NPC.velocity -= velocity * 1f;
            float speed = 18f;
            float gravCorrection = 0f;
            float randomOffset = 0f;
            int projectileType = 0;
            int cooldown = 0;
            int randomCooldown = 0;
            NPCLoader.TownNPCAttackCooldown(NPC, ref cooldown, ref randomCooldown);
            cooldown += Main.rand.Next(randomCooldown);
            int attackDelay = 0;
            NPCLoader.TownNPCAttackProj(NPC, ref projectileType, ref attackDelay);
            NPCLoader.TownNPCAttackProjSpeed(NPC, ref speed, ref gravCorrection, ref randomOffset);

            // Really stupid calculation but whatever
            float damageMult = 1f;
            int defense = NPC.defense;
            NPCLoader.BuffTownNPC(ref damageMult, ref defense);
            int damage = 0;
            float knockback = 0f;
            NPCLoader.TownNPCAttackStrength(NPC, ref damage, ref knockback);
            damage += (int)(NPC.damage * damageMult);
            if (Main.expertMode) {
                damage = (int)(damage * Main.GameModeInfo.TownNPCDamageMultiplier);
            }
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity * speed + new Vector2(0f, -gravCorrection) + Main.rand.NextVector2Square(-randomOffset, randomOffset), ModContent.ProjectileType<SkyMerchantProjectile>(), damage, knockback, Main.myPlayer);
            NPC.ai[1] = -cooldown - attackDelay;
            NPC.netUpdate = true;
        }
    }

    public override bool PreAI() {
        NPC.townNPC = false;
        SkyMerchantSystem.SpawnCheck = 0;
        NPC.aiStyle = NPCAIStyleID.Passive;
        target = -1;

        drawSet = !NPC.IsShimmerVariant ? DefaultDrawSet : ShimmerDrawSet;

        if (state == MovementState.Init) {
            state = MovementState.Ballooning;
            if (TileHelper.ScanDown(NPC.Center.ToTileCoordinates(), 60, out var result)) {
                for (int i = 0; i < Main.maxNPCs; i++) {
                    var npc = Main.npc[i];
                    if (npc.active && npc.townNPC && !npc.homeless && NPC.Distance(npc.Center) < 900f && npc?.ModNPC?.TownNPCStayingHomeless != true) {
                        state = MovementState.Walking;
                        NPC.Bottom = result.ToWorldCoordinates(8f, 0f);
                        break;
                    }
                }
            }
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

            if (!WorldGen.InWorld((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 30)) {
                NPC.active = false;
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                }
                return false;
            }

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
                    if (distance > closestDistance) {
                        continue;
                    }
                    target = i;
                    closestDistance = distance;
                }
            }

            if (target != -1) {
                BalloonAttack();
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

    public override void FindFrame(int frameHeight) {
        balloonBobbing += 1 / 60f;
        if (state == MovementState.Ballooning) {
            NPC.gfxOffY = MathF.Sin(balloonBobbing) * 4f + 4f;
        }
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
        if (state == MovementState.Ballooning) {
            damage = NPC.defense;
        }
        else {
            damage = 20;
        }
        knockback = 4f;
    }

    public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset) {
        Main.GetItemDrawFrame(ModContent.ItemType<SkyHunterCrossbow>(), out item, out itemFrame);
        horizontalHoldoutOffset = -18;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
        if (state == MovementState.Ballooning) {
            cooldown = 60;
        }
        else {
            cooldown = 30;
        }
        randExtraCooldown = 20;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
        projType = ModContent.ProjectileType<SkyMerchantProjectile>();
        attackDelay = 10;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
        multiplier = 18f;
        gravityCorrection = 1f;
        randomOffset = 1f;
    }

    #region Chat
    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
        button2 = this.GetLocalizedValue("Interface.RenameButton");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
        }
        else {
            Main.playerInventory = true;
            Main.npcChatText = "";
            UISystem.TalkInterface.SetState(new RenameItemUIState());
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