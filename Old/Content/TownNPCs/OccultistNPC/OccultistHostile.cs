using Aequus.Core;
using Aequus.Core.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Old.Content.TownNPCs.OccultistNPC;

public class OccultistHostile : Occultist {
    public override LocalizedText DisplayName => ModContent.GetInstance<Occultist>().DisplayName;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 25;
        NPCID.Sets.ActsLikeTownNPC[Type] = true;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
        NPCID.Sets.SpawnsWithCustomName[Type] = false;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() {
            Hide = true,
        });
    }

    public override void SetDefaults() {
        base.SetDefaults();
        NPC.lifeMax = 5000;
        NPC.friendly = false;
        NPC.rarity = 1;
        NPC.townNPC = false;
        NPC.dontTakeDamage = true;
    }

    public override Boolean CanHitPlayer(Player target, ref Int32 cooldownSlot) {
        return false;
    }

    public override Boolean PreAI() {
        if (Main.netMode != NetmodeID.Server && Main.GameUpdateCount % 180 == 0) {
            for (Int32 i = 0; i < 50; i++) {
                var p = NPC.Center + new Vector2(NPC.direction * -50, -30f) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(15f, 60f);
                if (Collision.SolidCollision(new Vector2(p.X - 8f, p.Y - 8f), 16, 16)) {
                    continue;
                }
                //ParticleSystem.New<OccultistParticle>(ParticleLayer.BehindAllNPCs).Setup(p, Vector2.UnitY * -0.1f);
                break;
            }
        }

        Int32 dir = Math.Sign(((Int32)NPC.ai[0] + 1) * 16 + 8 - NPC.Center.X);
        if (NPC.direction != dir) {
            NPC.direction = dir;
        }

        if (World.DownedDemonSiegeT1) {
            NPC.ai[0] = 0f;
            NPC.ai[1] = 0f;
            NPC.Transform(ModContent.NPCType<Occultist>());
        }
        return false;
    }

    public String RollUniqueChat(String textValueWeDontWant) {
        LocalizedText[] arr = GetSelectableChat().ToArray();
        for (Int32 i = 0; i < 25; i++) {
            String t = Main.rand.Next(arr).Value;
            if (t != textValueWeDontWant) {
                return t;
            }
        }

        return Main.rand.Next(arr).Value;
    }

    public IEnumerable<LocalizedText> GetSelectableChat() {
        for (Int32 i = 0; i <= 3; i++) {
            yield return this.GetDialogue(i.ToString());
        }

        if (WorldGen.crimson || Main.drunkWorld) {
            yield return this.GetDialogue("BloodButcherer");
            yield return this.GetDialogue("TendonBow");
            yield return this.GetDialogue("CrimsonRod");
            yield return this.GetDialogue("Crimson");
        }
        if (!WorldGen.crimson || Main.drunkWorld) {
            yield return this.GetDialogue("LightsBane");
            yield return this.GetDialogue("DemonBow");
            yield return this.GetDialogue("Vilethorn");
            yield return this.GetDialogue("Corruption");
        }

        if (Main.LocalPlayer.HasItem(ItemID.Bunny)) {
            yield return this.GetDialogue("Bunny");
        }
    }

    public override Boolean CanChat() {
        return true;
    }

    public override String GetChat() {
        return Main.rand.Next(GetSelectableChat().ToArray()).Value;
    }

    public override void SetChatButtons(ref String button, ref String button2) {
        button = this.GetLocalization("ListenButton").Value;
    }

    public override void OnChatButtonClicked(Boolean firstButton, ref String shopName) {
        if (firstButton) {
            Main.npcChatText = RollUniqueChat(Main.npcChatText);
        }
    }

    public override Boolean CanGoToStatue(Boolean toKingStatue) {
        return false;
    }

    public override void AI() {
        base.AI();
    }

    public override Boolean PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = AequusTextures.OccultistHostile_Sit;
        var glow = AequusTextures.OccultistHostile_Sit_Glow;
        var frame = texture.Frame(verticalFrames: 5, frameY: (Int32)Main.GameUpdateCount / 5 % 4 + 1);
        var origin = frame.Size() / 2f;
        var drawCoords = NPC.Center - screenPos + new Vector2(0f, -6f);
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(texture, drawCoords, frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
        spriteBatch.Draw(glow, drawCoords, frame, Color.White, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
        return false;
    }

    public static void CheckSpawn(Int32 x, Int32 y, Int32 plr) {
        if (World.DownedDemonSiegeT1 || Main.player[plr].Distance(new Vector2(x * 16f, y * 16f)) <= 800f || Main.hardMode || NPC.AnyNPCs(ModContent.NPCType<OccultistHostile>())) {
            return;
        }

        if (Main.netMode == NetmodeID.MultiplayerClient) {
            Aequus.GetPacket<OccultistRitualPacket>().Send(x, y, plr);
        }
        else {
            Int32 spawnX = (x + 1) * 16 + 8;
            Int32 spawnY = (y + 1) * 16 + 8 + 24;
            Int32 dir = Math.Sign(Main.player[plr].Center.X - spawnX);
            spawnX -= 48 * dir;
            Int32 middleX = x + 1;
            dir = -dir;
            for (Int32 k = 0; k < 3; k++) {
                Int32 m = middleX + dir * 2 + k * dir;
                Int32 n = y + 3;
                var t = Main.tile[m, n];
                if (!t.IsFullySolid()) {
                    WorldGen.PlaceTile(m, n, TileID.Ash);
                }
                t.Slope = SlopeType.Solid;
                t.IsHalfBlock = false;
                for (Int32 l = 0; l < 4; l++) {
                    if (Main.tile[m, n - l - 1].IsFullySolid()) {
                        WorldGen.KillTile(m, n - l - 1, noItem: true);
                    }
                }
            }
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, middleX - 5, y - 5, 10, 10);
            }

            NPC.NewNPC(new EntitySource_SpawnNPC(), spawnX, spawnY, ModContent.NPCType<OccultistHostile>(), ai0: x, ai1: y);
        }
    }

    public class OccultistRitualPacket : PacketHandler {
        public void Send(Int32 x, Int32 y, Int32 player) {
            ModPacket p = GetPacket();
            p.Write((UInt16)x);
            p.Write((UInt16)y);
            p.Write((Byte)player);
            p.Send();
        }

        public override void Receive(BinaryReader reader, Int32 sender) {
            UInt16 X = reader.ReadUInt16();
            UInt16 Y = reader.ReadUInt16();
            Byte Player = reader.ReadByte();

            CheckSpawn(X, Y, Player);
        }
    }
}
