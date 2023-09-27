using Aequus.Common.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.TownNPCs.SkyMerchant;

[AutoloadHead]
public partial class SkyMerchant : ModNPC {
    public enum MovementState {
        Init,
        Walking,
        Ballooning,
    }

    public MovementState state;

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
    }

    #region Initialization
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 25;
        NPCID.Sets.ExtraFramesCount[Type] = 9;
        NPCID.Sets.AttackFrameCount[Type] = 4;
        NPCID.Sets.DangerDetectRange[Type] = 700;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 90;
        NPCID.Sets.AttackAverageChance[Type] = 50;
        NPCID.Sets.HatOffsetY[Type] = 0;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
        NPCID.Sets.ActsLikeTownNPC[Type] = true;
        NPCID.Sets.SpawnsWithCustomName[Type] = true;

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new(0) {
            Velocity = -1f,
            Direction = -1
        });
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

    public override bool PreAI() {
        if (state == MovementState.Init) {
            state = MovementState.Ballooning;
            // Setup instanced shop here
            return false;
        }
        if (state == MovementState.Ballooning) {
            NPC.noGravity = true;
            DrawOffsetY = MathF.Sin(Main.GlobalTimeWrappedHourly) * 4f;

            if (NearStoppingPoint()) {
                NPC.velocity *= 0.95f;
                return false;
            }
            
            Vector2 gotoPosition;
            if (Main.dayTime) {
                gotoPosition = new Vector2((float)(Main.maxTilesX * (Main.time / Main.dayLength) * 16.0), 2000f);
            }
            else {
                gotoPosition = new Vector2(Main.maxTilesX * 16f + 200f, 0f);
            }
            var wantedVelocity = NPC.DirectionTo(gotoPosition);
            NPC.direction = 1;
            NPC.spriteDirection = 1;
            if (wantedVelocity.X < 0f) {
                NPC.velocity.X *= 0.95f;
            }
            else {
                NPC.velocity = Vector2.Lerp(NPC.velocity, wantedVelocity * 3f, 0.01f);
            }
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

    // mom is vaccuming im now restricted to no mic :(
    // :(

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

    #region Drawing
    private void DrawAnchored(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 offset, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth) {
        spriteBatch.Draw(texture, position + offset.RotatedBy(NPC.rotation) * NPC.scale, frame, color, rotation, origin, scale, spriteEffects, 0f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var drawCoordinates = NPC.Center - screenPos + new Vector2(0f, DrawOffsetY);
        var texture = TextureAssets.Npc[Type].Value;
        if (state == MovementState.Ballooning) {
            DrawAnchored(spriteBatch, AequusTextures.Balloon_SkyMerchant, drawCoordinates, new Vector2(0f, -96f), null, drawColor, NPC.rotation, AequusTextures.Balloon_SkyMerchant.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            DrawAnchored(spriteBatch, texture, drawCoordinates, new Vector2(0f, -14f), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            spriteBatch.Draw(AequusTextures.Basket_SkyMerchant, drawCoordinates, null, drawColor, NPC.rotation, AequusTextures.Basket_SkyMerchant.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
        return true;
    }
    #endregion
}