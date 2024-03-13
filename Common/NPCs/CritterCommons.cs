using Aequus.Core.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.NPCs;

public class CritterCommons : GlobalNPC {
    public interface ICritter {
        bool IsGolden { get; }
    }
    [AttributeUsage(AttributeTargets.Class)]
    internal class AutoloadCatchItem : AutoloadXAttribute {
        private readonly int _value;
        private readonly int _rarity;
        private readonly int _bait;
        private readonly bool _lavaBait;

        public AutoloadCatchItem(int value = 0, int rarity = 0, int baitPower = 0, bool lavaBait = false) {
            _value = value;
            _rarity = rarity;
            _bait = baitPower;
            _lavaBait = lavaBait;
        }

        internal override void Load(ModType modType) {
            if (modType is not ModNPC modNPC) {
                return;
            }

            InstancedModNPCItem catchItem = new InstancedModNPCItem(modNPC, _value, _rarity, _bait, _lavaBait);

            modType.Mod.AddContent(catchItem);
        }
    }

    private static readonly Dictionary<int, int> CatchItems = new();

    public override void SetStaticDefaults() {
        foreach (var npc in Mod.GetContent<ModNPC>().Where(n => n is ICritter)) {
            NPCID.Sets.CountsAsCritter[npc.Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[npc.Type] = true;
            NPCID.Sets.TownCritter[npc.Type] = true;
            ICritter critter = (npc as ICritter);
            if (critter.IsGolden) {
                NPCID.Sets.GoldCrittersCollection.Add(npc.Type);
                if (Mod.TryFind(npc.Name, out ModItem catchItem)) {
                    Main.npcCatchable[npc.Type] = true;
                    CatchItems[npc.Type] = catchItem.Type;
                }
            }
        }
    }

    public override void Unload() {
        CatchItems.Clear();
    }

    public override void SetDefaults(NPC entity) {
        if (CatchItems.TryGetValue(entity.type, out int catchItem)) {
            entity.catchItem = catchItem;
        }
    }

    public class GoldenCritterCommons : GlobalNPC {
        public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
            return npc.ModNPC is ICritter critter && critter.IsGolden;
        }

        public override void SetDefaults(NPC npc) {
            npc.rarity = 3;
        }

        public override void PostAI(NPC npc) {
            npc.position += npc.netOffset;
            Color color = ExtendLight.Get(npc.Center);
            int sparkleChance = Math.Max(Math.Max(color.R, color.G), color.B);
            if (sparkleChance > 30) {
                sparkleChance /= 30;
                if (Main.rand.Next(300) < sparkleChance) {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TintableDustLighted, Alpha: 254, newColor: new Color(255, 255, 0), Scale: 0.5f);
                    dust.velocity *= 0f;
                }
            }
            npc.position -= npc.netOffset;
        }
    }
}
