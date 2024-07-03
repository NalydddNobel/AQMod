using Aequu2.Core.Particles;
using Aequu2.DataSets;
using System;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequu2.Old.Content.Items.Weapons.Magic.Gamestar;

public class GamestarDebuff : ModBuff {
    public static readonly byte LagTicks = 7;
    internal static bool _loop;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;

        BuffSets.GrantImmunityWith[Type].Add(BuffID.Slow);
        BuffDataSet.PlayerStatusDebuff.Add(Type);
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC(out GamestarNPC gamestar)) {
            gamestar.gamestarDebuffMaxTime = Math.Max(gamestar.gamestarDebuffMaxTime, LagTicks);
        }
    }
}

public class GamestarNPC : GlobalNPC {
    public byte gamestarDebuffMaxTime;
    public byte gamestarDebuffTimer;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<GamestarDebuff>()];
    }

    public override void ResetEffects(NPC npc) {
        if (gamestarDebuffMaxTime == 0 || GamestarDebuff._loop) {
            return;
        }

        gamestarDebuffTimer++;
        gamestarDebuffMaxTime = 0;
    }

    public override Color? GetAlpha(NPC npc, Color drawColor) {
        return gamestarDebuffMaxTime > 0 ? Color.White : drawColor;
    }

    public override void PostAI(NPC npc) {
        if (gamestarDebuffMaxTime > 0 && Main.netMode != NetmodeID.Server) {
            int amt = Math.Max((npc.width + npc.height) / 30, gamestarDebuffMaxTime);
            foreach (var t in Particle<GamestarBits.Particle>.NewMultipleReduced(amt, gamestarDebuffMaxTime)) {
                Vector2 spawnCoords = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width, npc.width), Main.rand.NextFloat(-npc.height, npc.height));
                t.Setup(spawnCoords, 20);
            }
        }
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
        bitWriter.WriteBit(gamestarDebuffMaxTime > 0);
        if (gamestarDebuffMaxTime > 0) {
            binaryWriter.Write(gamestarDebuffTimer);
        }
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
        if (bitReader.ReadBit()) {
            gamestarDebuffTimer = binaryReader.ReadByte();
        }
    }

    public void UpdateLaggyNPC(NPC npc, int i) {
        GamestarDebuff._loop = true;
        try {
            for (int k = 0; k < gamestarDebuffMaxTime - 1; k++) {
                npc.UpdateNPC(i);
            }
        }
        catch (Exception ex) {
            Log.Error(ex);
        }
        finally {
            GamestarDebuff._loop = false;
        }
    }
}