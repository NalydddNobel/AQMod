using Aequu2.Core.Entities.Bestiary;
using Aequu2.DataSets;
using Aequu2.Old.Content.TownNPCs.PhysicistNPC;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequu2.Old.Content.DronePylons.NPCs;

public abstract class TownDroneBase : ModNPC, IAddRecipes {
    public Point pylonSpot;
    public int spawnInAnimation;

    public Vector2 movementPoint;

    public PylonDronePoint PylonManager => DroneSystem.Drones[pylonSpot];

    protected float SpawnInOpacity => spawnInAnimation == 0 ? 1f : -spawnInAnimation / 60f;

    public abstract DroneSlot Slot { get; }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .PreferBackground(BestiaryBuilder.SkyBiome);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(new CommonDrop(Slot.DroneItem.Type, chanceDenominator: 4, chanceNumerator: 5));
    }

    public virtual void AddRecipes(Mod mod) {
        BestiaryBuilder.ReSort(this, ModContent.NPCType<Physicist>());
    }

    public void DefaultMovement() {
        NPC.ai[3] += 1 / 60f;
        if (NPC.localAI[3]++ > 60f) {
            NPC.localAI[3] = 0f;
            NPC.netUpdate = true;
        }
        var tileCoords = NPC.Center.ToTileCoordinates();
        int topY = tileCoords.Y;
        int bottomY = tileCoords.Y;
        for (; bottomY < Main.maxTilesY - 45 && !Main.tile[tileCoords.X, bottomY].IsFullySolid(); bottomY++) {
        }
        for (int k = 0; k < 10 && topY > 45 && !Main.tile[tileCoords.X, topY].IsFullySolid();) {
            k++;
            topY--;
        }
        if (topY == bottomY) {
            NPC.velocity.Y += Main.rand.NextFloat(-0.2f, 0.2f);
        }
        float wave = Helper.Oscillate(NPC.ai[3] * 0.1f, -1f, 1f);
        if (NPC.velocity.X < -1f || NPC.velocity.X > 1f) {
            NPC.velocity.X *= 0.98f;
        }
        else {
            NPC.velocity.X = NPC.velocity.X + Main.rand.NextFloat(-0.05f + wave * 0.05f, 0.05f + wave * 0.05f);
        }

        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, Math.Sign((topY + bottomY) * 8f + 8f - NPC.Center.Y + Helper.Oscillate(NPC.ai[3] * 0.5f, -16f, 16f)), 0.01f);
    }

    public override void AI() {
        Main.BestiaryTracker.Kills.SetKillCountDirectly(NPC.GetBestiaryCreditId(), 999);
        NPC.direction = Math.Sign(NPC.velocity.X);
        if (spawnInAnimation < 0) {
            spawnInAnimation--;
            if (spawnInAnimation < -60) {
                spawnInAnimation = 0;
            }
        }
        if (pylonSpot == Point.Zero) {
            float closestPylon = 1000f;
            foreach (var p in DroneSystem.Drones.Keys) {
                float d = Vector2.Distance(NPC.Center, p.ToWorldCoordinates());
                if (d < closestPylon) {
                    closestPylon = d;
                    pylonSpot = p;
                }
            }
            if (pylonSpot == Point.Zero) {
                NPC.localAI[0] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.KillEffects();
                NPC.netUpdate = true;
                return;
            }
            NPC.netUpdate = true;
            NPC.ai[3] = Main.rand.NextFloat(100f);
            var townNPCs = PylonManager.GetNearbyTownNPCs();
            int div = townNPCs.Count;
            foreach (var n in townNPCs) {
                NPC.damage += n.defense;
            }
            if (div != 0)
                NPC.damage /= div;
            NPC.defDamage = NPC.damage;
        }
        if (pylonSpot == Point.Zero || !DroneSystem.ValidSpot(pylonSpot.X, pylonSpot.Y)) {
            NPC.localAI[0] = 0f;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.Kill();
            return;
        }
        if (!DroneSystem.Drones.TryGetValue(pylonSpot, out var drone)) {
            NPC.localAI[0] = 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.Kill();
            return;
        }
        if (!drone.isActive) {
            NPC.localAI[0] = 2f;
            NPC.active = false;
            return;
        }
        NPC.localAI[0] = 3f;
        NPC.localAI[1] = 0f;
        //NPC.timeLeft = 16;
    }

    public Color GetPylonColor() {
        if (NPC.IsABestiaryIconDummy) {
            return Color.Cyan;
        }

        if (TileDataSet.PylonColors.TryGetValue(Main.tile[pylonSpot].TileType, out Dictionary<int, Color> styleToColorDictionary)
            && styleToColorDictionary.TryGetValue(Main.tile[pylonSpot].TileFrameX / 54, out Color rgb)) {
            return rgb;
        }

        return Color.White;
    }

    public override void OnKill() {
        if ((int)NPC.localAI[0] == 0) {
            OnDeath();
        }
    }

    public virtual void OnDeath() {
        SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = 0.25f }, NPC.Center);
        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

        for (int i = 0; i < 10; i++) {
            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Electric);
            d.noGravity = true;
        }
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write((int)NPC.localAI[0]);
        writer.Write(pylonSpot.X);
        writer.Write(pylonSpot.Y);
        writer.Write(movementPoint.X);
        writer.Write(movementPoint.Y);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        NPC.localAI[0] = reader.ReadInt32();
        pylonSpot.X = reader.ReadInt32();
        pylonSpot.Y = reader.ReadInt32();
        movementPoint.X = reader.ReadSingle();
        movementPoint.X = reader.ReadSingle();
    }
}