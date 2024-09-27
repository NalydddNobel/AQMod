using Aequus.Common;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Bestiary;
using Aequus.Common.NPCs;
using Aequus.Common.Utilities;
using Aequus.Common.Utilities.Helpers;
using Aequus.Content.Biomes.PollutedOcean;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Content.Monsters.PollutedOcean.Conehead;

public class ConeheadZombieLoader : LoadedType {
    protected override void Load() {
        Mod.AddContent(new InstancedConeheadZombie("Basic", NPCID.Zombie));
        Mod.AddContent(new InstancedConeheadZombie("Bald", NPCID.BaldZombie));
        Mod.AddContent(new InstancedConeheadZombie("Pincushion", NPCID.PincushionZombie));
        Mod.AddContent(new InstancedConeheadZombie("Slimed", NPCID.SlimedZombie));
        Mod.AddContent(new InstancedConeheadZombie("Swamp", NPCID.SwampZombie));
        Mod.AddContent(new InstancedConeheadZombie("Twiggy", NPCID.TwiggyZombie));
        Mod.AddContent(new InstancedConeheadZombie("Female", NPCID.FemaleZombie));
    }
}

[BestiaryBiome<PollutedOceanSurface>()]
internal class InstancedConeheadZombie : InstancedNPC, IPostPopulateItemDropDatabase {
    private readonly int _zombieClone;

    private readonly string _realName;

    public InstancedConeheadZombie(string name, string texture, int zombieClone) : base($"ConeheadZombie{name}", texture) {
        _realName = name;
        _zombieClone = zombieClone;
    }

    internal InstancedConeheadZombie(string name, int zombieClone) : this($"ConeheadZombie{name}", $"{typeof(InstancedConeheadZombie).NamespacePath()}/ConeheadZombie{name}", zombieClone) {
        _realName = name;
        _zombieClone = zombieClone;
    }

    public override LocalizedText DisplayName => this.GetCategoryText("ConeheadZombie.DisplayName");

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[_zombieClone];

        if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(_zombieClone, out var bestiaryDrawInfo)) {
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = bestiaryDrawInfo;
        }

        LegacyPushableEntities.NPCIDs.Add(Type);
#if ELEMENTS
        NPCDataSet.NoDropElementInheritence.Add(Type);
#endif
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(_zombieClone);

        NPC.damage = 14;
        NPC.defense = 12;
        NPC.lifeMax = 120;
        NPC.knockBackResist = 0.5f;
        NPC.value = Item.silver;

        AIType = _zombieClone;
        AnimationType = _zombieClone;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        string key = $"Mods.Aequus.NPCs.ConeheadZombie.{_realName}_Bestiary";

        // Use the default key if there is not one specified for this zombie type.
        if (!ALanguage.ContainsKey(key)) {
            key = "Mods.Aequus.NPCs.ConeheadZombie.Bestiary";
        }

        this.CreateEntry(key, database, bestiaryEntry)
            .AddSpawn(BestiaryBuilder.NightTime);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.GetInstance<Items.Armor.Conehead.ConeHelmet>().Items[0].Type, chanceDenominator: 50));
    }

    public void PostPopulateItemDropDatabase(ItemDropDatabase database) {
        LootUtils.InheritDropRules(_zombieClone, Type, database);
    }

    public override bool? CanFallThroughPlatforms() {
        return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.Bottom.Y;
    }
}
