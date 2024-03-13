using Aequus.Common.NPCs.Bestiary;
using Aequus.Common.Renaming;
using Aequus.Content.DataSets;
using Aequus.Core;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Initialization;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC;

public class PhysicistPet : ModNPC, IAddRecipes {
    public int Owner { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }

    public override List<string> SetNPCNameList() {
        return new() {
            "Quimble",
            "Pimble",
            "Nimble",
            "Zimble",

            "Spinzie",
            "Pinzie",
            "Zinzie",
            "Xinzie",

            "Squondle",
            "Mondle",
            "Chondle",
            "Wandle",

            "Squizzer",
            "Chizzer",
            "Whizzer",
            "Fizzer",
            "Zizzer",
            "Tizzer",

            "Skeebler",
            "Beebler",
            "Zeebler",
            "Xeebler",
            "Teebler",
            "Weebler",
            "Meebler",

            "Whibbler",
            "Blipper",
            "Bleeper",
            "Blooper",
            "Zipper",
            "Zooper",

            "Pooper",
        };
    }

    public override void Load() {
        Mod.AddContent(new InstancedNPCEmote(this, EmoteID.Category.Town, () => WorldState.DownedCosmicBoss || WorldState.DownedTrueCosmicBoss));
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;
        NPCSets.HatOffsetY[Type] = 2;
        NPCSets.SpawnsWithCustomName[Type] = true;
        NPCSets.NoTownNPCHappiness[Type] = true;
        NPCSets.ActsLikeTownNPC[Type] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true;

        NPCMetadata.NameTagOverride[Type] = true;
    }

    public void AddRecipes(Aequus aequus) {
        BestiaryBuilder.ReSort(this, ContentSamples.NpcBestiarySortingId[ModContent.NPCType<Physicist>()] - 1);
    }

    public override void SetDefaults() {
        NPC.friendly = true;
        NPC.width = 16;
        NPC.height = 20;
        NPC.aiStyle = -1;
        NPC.damage = 0;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.noTileCollide = true;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 1f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.DesertBiome);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        int dustAmount = NPC.life > 0 ? 1 : 5;
        for (int k = 0; k < dustAmount; k++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
        }
    }

    public override bool CanChat() {
        return true;
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.92");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            PlayPetSound();
        }
    }
    public void PlayPetSound() {
        if (NPC.TryGetGlobalNPC(out RenameNPC renameNPC) && renameNPC.HasCustomName) {
            switch (renameNPC.CustomName.ToLower()) {
                case "little zumbo":
                case "littlezumbo":
                case "pooper":
                    SoundEngine.PlaySound(SoundID.Item16);
                    return;
            }
        }

        if (NPC.GivenName.ToLower() == "pooper") {
            SoundEngine.PlaySound(SoundID.Item16);
            return;
        }

        SoundEngine.PlaySound(AequusSounds.ZumboPettingSqueak with { Volume = 0.5f, });
    }

    public override string GetChat() {
        return this.GetDialogue(Main.rand.Next(2).ToString()).Value;
    }

    public override void AI() {
        int npcOwnerIndex = Owner;
        if (!Main.npc[npcOwnerIndex].active || Main.npc[npcOwnerIndex].type != ModContent.NPCType<Physicist>()) {
            npcOwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<Physicist>());
        }
        if (npcOwnerIndex > NPC.whoAmI || npcOwnerIndex == -1) // Ensures the npc is in a slot after their parent.
        {
            NPC.life = -1;
            NPC.HitEffect();
            NPC.active = false;
            return;
        }

        if ((int)Main.npc[Owner].ai[0] == 25) {
            NPC.Center = Main.npc[Owner].Center;
            return;
        }

        var npcOwner = Main.npc[npcOwnerIndex];
        var gotoLocation = npcOwner.Center + new Vector2(npcOwner.width * 1.5f * npcOwner.direction, -18f);

        NPC.direction = npcOwner.direction;
        NPC.spriteDirection = npcOwner.spriteDirection;
        var difference = gotoLocation - NPC.Center;
        if (difference.Length() < 15f) {
            if (NPC.velocity.Length() < 0.2f) {
                NPC.velocity += Main.rand.NextVector2Unit() * 0.1f;
            }
            if (NPC.velocity.Length() > 4f) {
                NPC.velocity *= 0.85f;
            }
            NPC.velocity *= 0.99f;
        }
        else {
            NPC.velocity += difference / 300f;
            if (NPC.velocity.Length() > 7f) {
                NPC.velocity.Normalize();
                NPC.velocity *= 7f;
            }
            if (difference.Length() > 900f) {
                NPC.Center = npcOwner.Center;
                NPC.netUpdate = true;
            }
        }
        if (NPC.position.Y < npcOwner.position.Y) {
            NPC.velocity.X *= 0.98f;
            NPC.velocity.Y -= 0.02f;
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) {
                NPC.velocity.X *= 0.95f;
                NPC.velocity.Y -= 0.05f;
            }
        }
        var rect = NPC.getRect();
        if (npcOwner.getRect().Intersects(rect)) {
            NPC.velocity += npcOwner.DirectionTo(NPC.Center) * 0.075f;
        }
        NPC.rotation = MathHelper.Clamp(NPC.velocity.X * 0.05f, -1f, 1f);
    }

    public override void FindFrame(int frameHeight) {
        NPC.frameCounter++;
        if (NPC.frameCounter > 6.0) {
            NPC.frame.Y += frameHeight;
            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type]) {
                NPC.frame.Y = 0;
            }

            NPC.frameCounter = 0.0;
        }
    }

    public override bool NeedSaving() {
        return true;
    }

    public override void SaveData(TagCompound tag) {
        if (!string.IsNullOrEmpty(NPC.GivenName)) {
            tag["Name"] = NPC.GivenName;
        }

        int physIndex = -1;
        int myPhysWhoAmI = (int)NPC.ai[0];
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Physicist>()) {
                physIndex++;
            }
            if (i >= myPhysWhoAmI) {
                break;
            }
        }
        tag["ParentApparentID"] = physIndex;
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet<string>("Name", out var value)) {
            NPC.GivenName = value;
        }

        if (tag.TryGet("ParentApparentID", out int findPhys)) {
            int physIndex = -1;
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Physicist>()) {
                    physIndex++;
                }
                if (physIndex >= findPhys) {
                    NPC.ai[0] = i;
                    break;
                }
            }
        }
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(NPC.GivenName ?? "Pooper");
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        NPC.GivenName = reader.ReadString();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = TextureAssets.Npc[Type].Value;
        var glowTexture = AequusTextures.PhysicistPet_Glow.Value;
        var frame = NPC.frame;
        float opacity = 1f;
        bool shimmer = NPC.IsShimmerVariant;

        if (NPC.IsABestiaryIconDummy) {
            opacity *= Main.npc[Owner].Opacity * (1f - Main.npc[Owner].shimmerTransparency);
            shimmer |= Main.npc[Owner].IsShimmerVariant;
        }

        if (shimmer) {
            texture = AequusTextures.PhysicistPet_Shimmer.Value;
            glowTexture = AequusTextures.PhysicistPet_Shimmer_Glow.Value;
            int frameY = frame.Y / frame.Height;
            frame = texture.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: Math.Clamp(frameY, 0, Main.npcFrameCount[Type] - 1));
        }

        var drawCoords = NPC.Center - screenPos;
        var origin = NPC.frame.Size() / 2f;
        var spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(texture, drawCoords, frame, NPC.GetNPCColorTintedByBuffs(drawColor) * opacity, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
        spriteBatch.Draw(glowTexture, drawCoords, frame, Color.White * opacity, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
        return false;
    }
}