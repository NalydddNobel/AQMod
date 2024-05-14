using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.Items.Armor.MiscHelmets;
using Aequus.Core.ContentGeneration;
using Aequus.DataSets;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Content.Enemies.PollutedOcean.Conehead;

public class ConeheadZombieLoader : ILoad {
    public readonly List<ModNPC> Types = [];

    public void Load(Mod mod) {
        mod.AddContent(new InstancedConeheadZombie("Basic", NPCID.Zombie));
        mod.AddContent(new InstancedConeheadZombie("Bald", NPCID.BaldZombie));
        mod.AddContent(new InstancedConeheadZombie("Pincushion", NPCID.PincushionZombie));
        mod.AddContent(new InstancedConeheadZombie("Slimed", NPCID.SlimedZombie));
        mod.AddContent(new InstancedConeheadZombie("Swamp", NPCID.SwampZombie));
        mod.AddContent(new InstancedConeheadZombie("Twiggy", NPCID.TwiggyZombie));
        mod.AddContent(new InstancedConeheadZombie("Female", NPCID.FemaleZombie));
    }

    public void Unload() { }
}

[ModBiomes(typeof(PollutedOceanBiomeSurface))]
internal class InstancedConeheadZombie : InstancedModNPC, IPostPopulateItemDropDatabase {
    private readonly int _zombieClone;

    private readonly string _realName;

    public InstancedConeheadZombie(string name, int zombieClone) : base($"ConeheadZombie{name}", $"{typeof(InstancedConeheadZombie).NamespaceFilePath()}/ConeheadZombie{name}") {
        _realName = name;
        _zombieClone = zombieClone;
    }

    public override LocalizedText DisplayName => Language.GetOrRegister("Mods.Aequus.NPCs.ConeheadZombie.DisplayName");

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = Main.npcFrameCount[_zombieClone];

        if (NPCSets.NPCBestiaryDrawOffset.TryGetValue(_zombieClone, out var bestiaryDrawInfo)) {
            NPCSets.NPCBestiaryDrawOffset[Type] = bestiaryDrawInfo;
        }

        NPCDataSet.PushableByTypeId.Add(Type);
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
        if (!ExtendLanguage.ContainsKey(key)) {
            key = "Mods.Aequus.NPCs.ConeheadZombie.Bestiary";
        }

        this.CreateEntry(key, database, bestiaryEntry)
            .AddSpawn(BestiaryTimeTag.NightTime);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.GetInstance<ConeHelmet>().Items[0].Type, chanceDenominator: 50));
    }

    public void PostPopulateItemDropDatabase(Mod mod, ItemDropDatabase database) {
        ExtendLoot.InheritDropRules(_zombieClone, Type, database);
    }

    public override bool? CanFallThroughPlatforms() {
        return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.Bottom.Y;
    }
}
