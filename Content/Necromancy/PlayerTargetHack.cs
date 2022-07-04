using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Content.Necromancy
{
    public readonly struct PlayerTargetHack
    {
        public static PlayerTargetHack None => default(PlayerTargetHack);

        public readonly NPC ZombieNPC;
        public readonly NPC TargetNPC;
        public readonly Player Player;
        public readonly Vector2 NewLocation;
        public readonly Vector2 OriginalLocation;
        public readonly bool OriginalWet;
        public readonly bool OriginalLavaWet;
        public readonly bool OriginalHoneyWet;

        public bool HasInfo => Player != null;

        public PlayerTargetHack(NPC zombie, NPC target, Player player, Vector2 newLocation)
        {
            ZombieNPC = zombie;
            TargetNPC = target;
            Player = player;
            NewLocation = newLocation;
            OriginalLocation = player.Center;
            OriginalWet = player.wet;
            OriginalLavaWet = player.lavaWet;
            OriginalHoneyWet = player.honeyWet;
        }

        public void Move()
        {
            Player.Center = TargetNPC.Center;
            Player.wet = Collision.WetCollision(TargetNPC.position, TargetNPC.width, TargetNPC.height);
            Player.lavaWet = Collision.LavaCollision(TargetNPC.position, TargetNPC.width, TargetNPC.height);
            Player.honeyWet = Collision.honey;
        }

        public void Restore()
        {
            Player.Center = OriginalLocation;
            Player.wet = OriginalWet;
            Player.lavaWet = OriginalLavaWet;
            Player.honeyWet = OriginalHoneyWet;
        }
    }
}