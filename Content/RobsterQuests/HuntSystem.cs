using AQMod.Common.IO;
using AQMod.NPCs.Friendly.Town;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.RobsterQuests
{
    public class HuntSystem : ModWorld
    {
        public static RobsterHunt Hunt { get; set; }
        public static ushort QuestsCompleted { get; set; }
        public static int TargetNPC { get; private set; }
        internal static int _targetNPCType = -1;

        public override void Initialize()
        {
            Hunt = null;
            TargetNPC = -1;
            _targetNPCType = -1;
            Robster.Initialize();
        }

        public override TagCompound Save()
        {
            if (Hunt != null)
            {
                var tag = Hunt.Save();
                if (tag == null)
                    return null;
                tag["Hunt"] = Hunt.GetKey();
                tag["QuestsCompleted"] = (int)QuestsCompleted;
                if (_targetNPCType != -1)
                    tag["TargetNPC"] = new ModNPCIO().GetKey(_targetNPCType);
                return tag;
            }
            return null;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("Hunt"))
            {
                string key = tag.GetString("Hunt");
                Hunt = AQMod.RobsterHunts.GetContent(key);
                QuestsCompleted = (ushort)tag.GetInt("QuestsCompleted");
                if (tag.ContainsKey("TargetNPC"))
                    _targetNPCType = tag.GetInt("TargetNPC");
            }
        }

        public override void PostUpdate()
        {
            if (_targetNPCType >= 0 && TargetNPC == -1)
                SetNPCTarget(_targetNPCType);
        }

        public static bool QuitHunt(Player player)
        {
            if (Hunt != null)
                Hunt.RemoveHunt();
            return RandomizeHunt(player);
        }

        public static void SetNPCTarget(int value, bool npcID = true)
        {
            if (npcID)
                TargetNPC = NPC.FindFirstNPC(value);
            else
                TargetNPC = value;
            if (TargetNPC != -1)
            {
                if (npcID)
                    _targetNPCType = value;
                else
                    _targetNPCType = Main.npc[value].type;
            }
            else
            {
                _targetNPCType = -1;
            }
        }

        /// <summary>
        /// Randomizes the hunt. Add a player instance so the hunt can possibly interact with player data. Some hunts cannot start without player data.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool RandomizeHunt(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                var hunt = AQMod.RobsterHunts.GetContent(Main.rand.Next(AQMod.RobsterHunts.Count));
                if (hunt.CanStart(player))
                {
                    Hunt = hunt;
                    Hunt.OnStart(player);
                    return true;
                }
            }
            return false;
        }

        public static bool? SpecialHuntTileDestroyed(int tileX, int tileY, bool alsoResetQuest = true)
        {
            var worldPosition = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f);
            var closestPlayer = Main.player[Player.FindClosest(new Vector2(tileX * 16f, tileY * 16f), 16, 16)];
            float seeingDistance = 1000f;
            if (closestPlayer.dead || Vector2.Distance(closestPlayer.Center, worldPosition) > seeingDistance)
            {
                if (alsoResetQuest)
                {
                    AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterFailAccident", Robster.RobsterBroadcastMessageColor);
                    QuitHunt(closestPlayer);
                }
                return false;
            }
            if (closestPlayer.invis)
                seeingDistance /= 5f;
            bool foundOut = false;
            int robsterNPCID = ModContent.NPCType<Robster>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                float npcSeeingDistance = seeingDistance;
                int directionToTile = worldPosition.X < Main.npc[i].position.X + Main.npc[i].width / 2f ? -1 : 1;
                if (-Main.npc[i].direction != directionToTile)
                    npcSeeingDistance /= 2;
                if (Main.npc[i].active && Main.npc[i].townNPC && Main.npc[i].type != robsterNPCID && Vector2.Distance(Main.npc[i].Center, worldPosition) < npcSeeingDistance && Collision.CanHitLine(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, new Vector2(tileX * 16f, tileY * 16f), 16, 16))
                {
                    foundOut = true;
                    Main.npc[i].ai[0] = 0f;
                    Main.npc[i].ai[1] = 1500f;
                    Main.npc[i].direction = directionToTile;
                    Main.npc[i].spriteDirection = directionToTile;
                    Main.npc[i].netUpdate = true;


                    int[] emoteChoices = new int[] { EmoteID.WeatherLightning, EmoteID.ItemSword, EmoteID.MiscFire };


                    int choice = Main.rand.Next(emoteChoices.Length);
                    EmoteBubble.NewBubble(emoteChoices[choice], new WorldUIAnchor(Main.npc[i]), 480);
                }
            }
            if (foundOut && alsoResetQuest)
            {
                AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterFail", Robster.RobsterBroadcastMessageColor);
                QuitHunt(closestPlayer);
            }
            return foundOut;
        }

        internal static void Unload() { Hunt = null; }
    }
}