﻿namespace Aequus;
public static partial class Helper {
    public static int GoreType(string name) {
        return Mod.Find<ModGore>(name).Type;
    }

    public static Gore DeathGore(this NPC npc, int goreID, Vector2 offset = default(Vector2), Vector2? velocity = null, float Scale = 1f) {
        var g = Gore.NewGoreDirect(npc.GetSource_Death(), npc.Center + offset, velocity ?? npc.velocity, goreID, Scale);
        g.position.X -= g.Width / 2f;
        g.position.Y -= g.Height / 2f;
        return g;
    }

    public static Gore DeathGore(this NPC npc, string name, Vector2 offset = default(Vector2), Vector2? velocity = null, float Scale = 1f) {
        return DeathGore(npc, GoreType(name), offset, velocity, Scale);
    }
}