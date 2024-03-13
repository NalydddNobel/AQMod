namespace Aequus.Old.Content.Necromancy.Aggression;

public interface IEnemyAggressor {
    public abstract void OnPreAI(NPC npc, NecromancyNPC necro);

    public abstract void OnPostAI(NPC npc, NecromancyNPC necro);
}