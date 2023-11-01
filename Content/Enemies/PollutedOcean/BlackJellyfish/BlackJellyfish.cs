using Aequus.Common.NPCs;
using Aequus.Content.DataSets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;

public class BlackJellyfish : AIJellyfish {
    #region Initialization
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.BlueJellyfish];
        NPCSets.PushableByTypeId.Add(Type);
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
        NPC.alpha = 20;
        AnimationType = NPCID.BlueJellyfish;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.CavernsBiome)
            .AddSpawn(BestiaryBuilder.OceanBiome);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ItemID.Glowstick, minimumDropped: 1, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.JellyfishNecklace, 100));
    }
    #endregion

    public static float AttackRange => 60f;

    public override void AI() {
        if (NPC.ai[2] > 0f || (NPC.direction != 0 && NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < shockRampUpDistance)) {
            NPC.ai[2]++;
            if (NPC.justHit) {
                NPC.ai[2] -= 30f;
                NPC.ai[2] *= 0.75f;
            }
            if (NPC.ai[2] > shockAttackLength && Main.netMode != NetmodeID.MultiplayerClient) {
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

    public override bool CanShock() {
        return false;
    }

    public override Color? GetAlpha(Color drawColor) {
        return drawColor * Math.Clamp(drawColor.ToVector3().Length() / 2f, 0.5f, 1f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.ai[2] > 0f) {

        }
        return true;
    }
}