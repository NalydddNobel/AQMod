using Aequus.Content.Bosses;
using Aequus.Core.Assets;
using ReLogic.Utilities;
using System;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.NPCs;

[CloneByReference]
public abstract class AequusBoss : ModNPC {
    private InstancedBossBag _treasureBag;
    private ModItem _trophy;
    private ModItem _relic;
    private ModItem _bossMask;

    public abstract bool PreHardmode { get; }

    public abstract int ItemRarity { get; }

    internal string ItemPath(string suffix) {
        return $"{this.NamespaceFilePath()}/Items/{Name}{suffix}";
    }

    public sealed override void Load() {
        _treasureBag = new InstancedBossBag(this);
        Mod.AddContent(_treasureBag);

        ModTile trophyTile = new InstancedTrophyTile(this);
        _trophy = new InstancedTrophyItem(trophyTile);

        Mod.AddContent(trophyTile);
        Mod.AddContent(_trophy);

        RequestCache<Texture2D> relicTexture = new RequestCache<Texture2D>($"{this.NamespaceFilePath()}/Items/{Name}Relic");
        ModTile relicTile = new InstancedRelicTile(Name, GetRelicRenderer(relicTexture));
        _relic = new InstancedRelicItem(relicTile);

        Mod.AddContent(relicTile);
        Mod.AddContent(_relic);

        Type type = GetType();
        if (type.GetAttribute<AutoloadBossMaskAttribute>() != null) {
            _bossMask = new InstancedBossMask(this);
            Mod.AddContent(_bossMask);
        }
    }

    protected virtual IRelicRenderer GetRelicRenderer(RequestCache<Texture2D> texture) {
        return new BasicRelicRenderer(texture);
    }

    protected override bool CloneNewInstances => true;

    protected NPCLoot npcLoot;
    public sealed override void ModifyNPCLoot(NPCLoot npcLoot) {
        this.npcLoot = npcLoot;
        npcLoot.Add(ItemDropRule.Common(_trophy.Type, 10));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), _treasureBag.Type));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), _relic.Type));
        // Pet goes here
        if (_bossMask != null) {
            AddBossDrop(ItemDropRule.Common(_bossMask.Type, 7));
        }
    }
    public virtual void ModifyNPCLoot() { }

    protected void AddTreasureBagItem(params IItemDropRule[] dropRules) {
        _treasureBag._rulesToRegister.AddRange(dropRules);
    }
    protected void AddNonExpertItem(params IItemDropRule[] dropRules) {
        var notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
        foreach (var dropRule in dropRules) {
            notExpertRule.OnSuccess(dropRule);
        }
        npcLoot.Add(notExpertRule);
    }

    /// <summary>
    /// Adds all of the listed rules to the boss's Classic/Journey mode loot pool, and to the boss's Treasure Bag's loot pool.
    /// </summary>
    /// <param name="npcLoot"></param>
    /// <param name="dropRules"></param>
    protected void AddBossDrop(params IItemDropRule[] dropRules) {
        AddNonExpertItem(dropRules);
        AddTreasureBagItem(dropRules);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AutoloadBossMaskAttribute : Attribute { }
}