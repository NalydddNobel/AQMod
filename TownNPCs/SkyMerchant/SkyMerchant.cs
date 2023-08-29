using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.TownNPCs.SkyMerchant;

[AutoloadHead]
public partial class SkyMerchant : ModNPC {
    public enum MovementState {
        Walking,
        Ballooning,
    }

    public MovementState state;

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

    public override void AI() {
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
        button2 = this.GetLocalizedValue("Interface.RenameButton");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        return true;
    }
}