using Aequu2.Content.Configuration;
using Aequu2.Old.Content.Enemies.Mimics;
using Aequu2.Old.Content.Items.Materials;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequu2.Old.Common.NPCs;

public class MimicEdits : GlobalNPC, IPreExtractBestiaryItemDrops {
    public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
        return npc.type == NPCID.Mimic || npc.type == NPCID.IceMimic;
    }

    public override void SetDefaults(NPC npc) {
        if ((npc.type == NPCID.Mimic || npc.type == NPCID.IceMimic) && !Main.remixWorld && GameplayConfig.Instance.PreHardmodeMimics) {
            npc.damage = 30;
            npc.defense = 12;
            npc.lifeMax = 300;
            npc.value = Item.buyPrice(gold: 2);
        }
    }

    public override void OnSpawn(NPC npc, IEntitySource source) {
        if (source is not EntitySource_SpawnNPC) {
            return;
        }

        if (npc.type == NPCID.IceMimic) {
            if (Main.hardMode && GameplayConfig.Instance.HardmodeMimics && !Main.rand.NextBool(3)) {
                npc.Transform(ModContent.NPCType<FrostMimic>());
            }
        }
        else if (npc.type == NPCID.Mimic) {
            if (NPC.downedBoss3 && GameplayConfig.Instance.ShadowMimics && npc.position.Y / 16f > Main.UnderworldLayer) {
                npc.Transform(ModContent.NPCType<ShadowMimic>());
            }
            if (Main.hardMode && GameplayConfig.Instance.HardmodeMimics && !Main.rand.NextBool(3)) {
                if (npc.position.Y > Main.worldSurface * 16f && npc.position.Y < Main.UnderworldLayer * 16f) {
                    npc.Transform(ModContent.NPCType<AdamantiteMimic>());
                }
            }
        }
    }

    public static void AddPHMMimics(IDictionary<int, float> pool, in NPCSpawnInfo spawnInfo) {
        if (Main.hardMode || Main.remixWorld || !GameplayConfig.Instance.PreHardmodeMimics
            || spawnInfo.SpawnTileY < ((int)Main.worldSurface + 100) || spawnInfo.Water) {
            return;
        }

        var tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
        if (TileID.Sets.IcesSnow[tile.TileType]) {
            pool[NPCID.IceMimic] = 0.05f;
            return;
        }

        if (spawnInfo.SpawnTileY >= Main.UnderworldLayer && !NPC.downedBoss3) {
            return;
        }

        if (TileID.Sets.Conversion.Stone[tile.TileType] || TileID.Sets.Ash[tile.TileType]) {
            pool[NPCID.Mimic] = spawnInfo.SpawnTileY >= Main.rockLayer ? 0.04f : 0.01f;
        }
    }

    public void PreExtractBestiaryItemDrops(Mod mod, BestiaryDatabase bestiaryDatabase, ItemDropDatabase database) {
        if (!GameplayConfig.Instance.PreHardmodeMimics) {
            return;
        }

        var rules = ExtendLoot.GetNPCLoot(NPCID.Mimic, database);
        rules.RemoveAll(i => i is not LeadingConditionRule conditionRule || conditionRule.condition is not Conditions.RemixSeedEasymode);
        LeadingConditionRule conditionRule = new LeadingConditionRule(new Conditions.NotRemixSeed());
        //conditionRule.OnSuccess(new OneFromOptionsDropRule(1, 1,
        //    ItemID.Spear, ItemID.Blowpipe, ItemID.WoodenBoomerang, ItemID.Aglet, ItemID.ClimbingClaws, ItemID.Radar, ItemID.WandofSparking, ItemID.PortableStool, ItemID.CordageGuide
        //));

        conditionRule.OnSuccess(new OneFromOptionsDropRule(1, 1,
            ItemID.BandofRegeneration, ItemID.MagicMirror, ItemID.CloudinaBottle, ItemID.HermesBoots, ItemID.Mace, ItemID.ShoeSpikes
        ));
        rules.Add(conditionRule);

        var flareGunRule = ItemDropRule.Common(ItemID.FlareGun, 8);
        flareGunRule.OnSuccess(ItemDropRule.Common(ItemID.Flare, minimumDropped: 25, maximumDropped: 50));
        rules.Add(flareGunRule);
        rules.Add(ItemDropRule.Common(ItemID.Extractinator, 8));
        rules.Add(ItemDropRule.Common(ItemID.AngelStatue, 20));

        rules = ExtendLoot.GetNPCLoot(NPCID.IceMimic, database);
        rules.RemoveAll(i => i is not LeadingConditionRule conditionRule || conditionRule.condition is not Conditions.RemixSeedEasymode);

        conditionRule = new LeadingConditionRule(new Conditions.NotRemixSeed());
        conditionRule.OnSuccess(new OneFromOptionsDropRule(1, 1,
            ItemID.IceBoomerang, ItemID.IceBlade, ItemID.IceSkates, ItemID.SnowballCannon, ItemID.BlizzardinaBottle, ItemID.FlurryBoots
        ));
        rules.Add(conditionRule);

        rules.Add(ItemDropRule.Common(ModContent.ItemType<FrozenTechnology>(), 2, 1, 2));
        rules.Add(ItemDropRule.Common(ItemID.Extractinator, 20));
        rules.Add(ItemDropRule.Common(ItemID.IceMachine, 20));
        rules.Add(ItemDropRule.Common(ItemID.Fish, 20));
    }

}
