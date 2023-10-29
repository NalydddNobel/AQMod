using Aequus.Common.UI;
using Aequus.Content.DataSets;
using Aequus.Content.Enemies.PollutedOcean.Scavenger.UI;
using Aequus.Content.Items.Equipment.Accessories.Inventory;
using Aequus.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public class ScavengerLootBag : ModNPC {
    public const int SmartCursorInteractionDistance = 54;
    public static int BackpackDropRate = 10;

    public Item[] drops;
    public int playerOpened;

    #region Initialization
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 3;

        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = true,
        };

        NPCSets.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        NPC.lifeMax = 250;
        NPC.width = 14;
        NPC.height = 20;
        NPC.damage = 0;
        NPC.defense = 8;
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit15;
        NPC.DeathSound = SoundID.NPCHit17;
        NPC.knockBackResist = 0.035f;
        drops = new Item[0];
        playerOpened = -1;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScavengerBag>(), ScavengerLootBag.BackpackDropRate));
    }
    #endregion

    public override bool CanChat() {
        return playerOpened == -1;
    }

    public override string GetChat() {
        return "";
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        if (NPC.life <= 0) {
            for (int i = 0; i < 8; i++) {
                var g = Gore.NewGoreDirect(NPC.GetSource_FromThis(), new Vector2(NPC.position.X, NPC.Center.Y - 10f), Vector2.Zero, 1218);
                g.velocity = new Vector2(Main.rand.Next(1, 10) * 0.3f * 2.5f * hit.HitDirection, 0f - (3f + Main.rand.Next(4) * 0.3f));
            }
        }
        else {
            for (int i = 0; i < 3; i++) {
                var g = Gore.NewGoreDirect(NPC.GetSource_FromThis(), new Vector2(NPC.position.X, NPC.Center.Y - 10f), Vector2.Zero, 1218);
                g.velocity = new Vector2(Main.rand.Next(1, 10) * 0.3f * 2f * hit.HitDirection, 0f - (2.5f + Main.rand.Next(4) * 0.3f));
            }
        }
    }

    public override void AI() {
        NPC.EncourageDespawn(600);

        if (NPC.velocity.Y == 0f) {
            NPC.velocity.X *= 0.85f;
        }
        if (NPC.velocity.X != 0f) {
            NPC.spriteDirection = Math.Sign(NPC.velocity.X);
        }

        playerOpened = -1;
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].talkNPC == NPC.whoAmI) {
                playerOpened = i;
                if (Main.myPlayer == i && UISystem.TalkInterface.CurrentState is not ScavengerLootBagUI) {
                    Main.playerInventory = true;
                    Main.npcChatText = "";
                    UISystem.TalkInterface.SetState(new ScavengerLootBagUI());
                }
                break;
            }
        }
        for (int i = playerOpened + 1; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].talkNPC == NPC.whoAmI) {
                Main.player[i].SetTalkNPC(-1, fromNet: Main.netMode == NetmodeID.MultiplayerClient);
                continue;
            }
        }
    }

    public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) {
        boundingBox.X -= 4;
        boundingBox.Width += 8;
    }

    public override void FindFrame(int frameHeight) {
        if (playerOpened > -1) {
            if (NPC.frame.Y < frameHeight * (Main.npcFrameCount[Type] - 1)) {
                NPC.frameCounter++;
                if (NPC.frameCounter < 0.0) {
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frameCounter > 5.0) {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y += frameHeight;
                }
            }
        }
        else {
            if (NPC.frame.Y > 0) {
                NPC.frameCounter--;
                if (NPC.frameCounter > 0.0) {
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frameCounter < -5.0) {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y -= frameHeight;
                }
            }
        }
    }

    public override void OnKill() {
        foreach (var i in drops) {
            int item = Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), i);
            if (item == Main.maxItems) {
                continue;
            }
            Main.item[item].Prefix(-1);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var drawCoordinates = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY - 1f);
        var rectangle = NPC.getRect();
        NPCLoader.ModifyHoverBoundingBox(NPC, ref rectangle);
        bool hovering = false;
        if (!NPC.IsABestiaryIconDummy) {
            if (Main.instance.currentNPCShowingChatBubble == NPC.whoAmI) {
                hovering = true;
                Main.instance.currentNPCShowingChatBubble = -1;
                if (Main.LocalPlayer.talkNPC == NPC.whoAmI) {
                    Main.playerInventory = true;
                }
            }
            else if (Main.SmartInteractShowingGenuine && Main.SmartInteractNPC == NPC.whoAmI) {
                hovering = true;
            }
        }

        spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        if (hovering) {
            spriteBatch.Draw(AequusTextures.ScavengerLootBag_Outline, drawCoordinates, NPC.frame, Lighting.GetColor(NPC.Center.ToTileCoordinates(), Main.OurFavoriteColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(drops.Length);
        for (int i = 0; i < drops.Length; i++) {
            if (drops[i] == null || drops[i].IsAir) {
                writer.Write(false);
                continue;
            }
            writer.Write(true);
            ItemIO.Send(drops[i], writer, writeStack: true);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        int length = reader.ReadInt32();
        if (drops.Length != length) {
            drops = new Item[length];
        }
        for (int i = 0; i < length; i++) {
            if (drops[i] == null) {
                drops[i] = new();
            }
            if (!reader.ReadBoolean()) {
                continue;
            }
            ItemIO.Receive(drops[i], reader, readStack: true);
        }
    }

    public static void AddDropsToList(NPC npc, List<Item> dropsRegisterList) {
        var dropAttemptInfo = new DropAttemptInfo() {
            item = 0,
            npc = npc,
            player = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)],
            rng = Main.rand,
            IsExpertMode = Main.expertMode,
            IsMasterMode = Main.masterMode,
            IsInSimulation = false,
        };

        NewItemCache.Begin();
        Main.ItemDropSolver.TryDropping(dropAttemptInfo);
        NewItemCache.End();

        foreach (var d in NewItemCache.DroppedItems) {
            if (d == null || d.IsAir) {
                continue;
            }

            dropsRegisterList.Add(d);
        }
    }
}