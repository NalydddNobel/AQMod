using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs.Monsters.Jungle
{
    public abstract class BaseCore : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

        public class EnemyData : TagSerializable
        {
            public Point tileLoc;
            public int NPCId;
            public int life;
            public int currentlySpawnedEnemyIndex;
            public List<TendrilDrawInfo> ConnectedTendril;
            public bool dead;

            internal EnemyData()
            {
            }

            public EnemyData(Point pos, int npcId)
            {
                tileLoc = pos;
                NPCId = npcId;
            }

            public void Send(BinaryWriter writer)
            {
                var bb = new BitsByte(dead);
                writer.Write(tileLoc.X);
                writer.Write(tileLoc.Y);
                writer.Write(NPCId);
                writer.Write(life);
                writer.Write(currentlySpawnedEnemyIndex);
                writer.Write(bb);
            }

            public void Recieve(BinaryReader reader)
            {
                tileLoc.X = reader.ReadInt32();
                tileLoc.Y = reader.ReadInt32();
                NPCId = reader.ReadInt32();
                life = reader.ReadInt32();
                currentlySpawnedEnemyIndex = reader.ReadInt32();
                var bb = (BitsByte)reader.ReadByte();
                dead = bb[0];
            }

            public TagCompound SerializeData()
            {
                return new TagCompound()
                {
                    ["NPCId"] = NPCId >= Main.maxNPCTypes ? $"{NPCLoader.GetNPC(NPCId).Mod.Name}:{NPCLoader.GetNPC(NPCId).Name}" : $"{NPCId}",
                    ["TileLocX"] = tileLoc.X,
                    ["TileLocY"] = tileLoc.Y,
                    ["Life"] = life,
                };
            }

            public static EnemyData Deserialize(TagCompound tag)
            {
                var data = new EnemyData();
                string npcID = tag.Get<string>("NPCId");
                if (!int.TryParse(npcID, out data.NPCId))
                {
                    string[] split = npcID.Split(":");
                    if (!ModLoader.TryGetMod(split[0], out var mod) || !mod.TryFind<ModNPC>(split[1], out var foundNPC))
                    {
                        return null;
                    }
                    data.NPCId = foundNPC.Type;
                }
                data.tileLoc.X = tag.Get<int>("TileLocX");
                data.tileLoc.Y = tag.Get<int>("TileLocY");
                data.life = tag.Get<int>("Life");
                return data;
            }
        }
        public abstract class EnemySpawnData
        {
            public abstract EnemyData TrySpawnEnemy(int x, int y);
        }
        public class EnemySpawn_Any4Sides : EnemySpawnData
        {
            public int npcIDToSpawn;

            public EnemySpawn_Any4Sides(int npcID)
            {
                npcIDToSpawn = npcID;
            }

            public override EnemyData TrySpawnEnemy(int x, int y)
            {
                if (!WorldGen.InWorld(x, y, 100) || Main.tile[x, y].IsFullySolid())
                {
                    return null;
                }

                if (Main.tile[x + 1, y].IsFullySolid() || Main.tile[x - 1, y].IsFullySolid() || Main.tile[x, y + 1].IsFullySolid() || Main.tile[x, y - 1].IsFullySolid())
                {
                    return new EnemyData(new Point(x, y), npcIDToSpawn);
                }
                return null;
            }
        }
        public struct TendrilDrawInfo
        {
            public Vector2 drawLoc;
            public float rotation;
        }

        public List<EnemyData> Enemies;

        public abstract EnemySpawnData ChooseSpawnType(int index, int count);
        public virtual int EnemyCount => Main.getGoodWorld ? 12 : (Main.expertMode ? 6 : 3);

        protected override bool CloneNewInstances => true;

        public virtual void OnKilledMinion(NPC npc, int index)
        {
            Enemies[index].dead = true;
            Main.NewText($"god, {index} is so dead!");
            NPC.netUpdate = true;
        }

        public override ModNPC Clone(NPC newEntity)
        {
            var c = base.Clone(newEntity);
            if (Enemies != null)
                ((BaseCore)c).Enemies = new List<EnemyData>(Enemies);
            return c;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MustAlwaysDraw[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 220;
            NPC.height = 260;
            NPC.lifeMax = 20000;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
            Enemies = null;
        }

        public override void AI()
        {
            NPC.timeLeft = Math.Max(NPC.timeLeft, 3600);
            if ((int)NPC.ai[0] == -1)
            {
                NPC.localAI[0] += NPC.localAI[0] * 0.005f + 0.3f;
                if (NPC.localAI[0] > 60f)
                {
                    NPC.life = -1;
                    NPC.checkDead();
                    var p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileID.LunarFlare, 0, 0f, Main.myPlayer, ai1: -1f);
                    p.frameCounter = Main.rand.Next(-6, 6);
                    p.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                return;
            }
            if ((int)NPC.ai[0] == 0 || Enemies == null || Enemies.Count == 0)
            {
                ConstructEnemies(EnemyCount);
                NPC.ai[0]++;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active)
                    {
                        for (int j = 0; j < Enemies.Count; j++)
                        {
                            if (Enemies[j].currentlySpawnedEnemyIndex != -1 || Enemies[j].dead)
                                continue;

                            var worldLoc = Enemies[j].tileLoc.ToWorldCoordinates();
                            if (Vector2.Distance(worldLoc, Main.player[i].Center) < 1500f)
                            {
                                Enemies[j].currentlySpawnedEnemyIndex = NPC.NewNPC(NPC.GetSource_FromThis(), Enemies[j].tileLoc.X * 16 + 8, Enemies[j].tileLoc.Y * 16 + 8, Enemies[j].NPCId);
                                if (Enemies[j].currentlySpawnedEnemyIndex == 200 || Enemies[j].currentlySpawnedEnemyIndex == -1)
                                {
                                    Enemies.RemoveAt(j);
                                }
                                else
                                {
                                    Main.npc[Enemies[j].currentlySpawnedEnemyIndex].Aequus().jungleCoreInvasion = NPC.whoAmI + 1;
                                    Main.npc[Enemies[j].currentlySpawnedEnemyIndex].Aequus().jungleCoreInvasionIndex = j;
                                    Main.npc[Enemies[j].currentlySpawnedEnemyIndex].target = i;
                                    Main.npc[Enemies[j].currentlySpawnedEnemyIndex].netUpdate = true;
                                }
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].currentlySpawnedEnemyIndex != -1 && (!Main.npc[Enemies[i].currentlySpawnedEnemyIndex].active || Main.npc[Enemies[i].currentlySpawnedEnemyIndex].type != Enemies[i].NPCId))
                {
                    Enemies[i].currentlySpawnedEnemyIndex = -1;
                    NPC.netUpdate = true;
                }
            }

            if (Main.netMode == NetmodeID.Server)
                return;
            NPC.localAI[1] += Main.rand.NextFloat(0.0001f, 0.0025f);
            NPC.scale = 1f + ((float)Math.Sin(NPC.localAI[1]) + 1f) * 0.1f;
            CheckTendrils();
        }

        public override bool CheckActive()
        {
            return Enemies != null && Enemies.Count > 0;
        }

        public void ConstructEnemies(int count)
        {
            Enemies = new List<EnemyData>();
            for (int i = 0; i < count; i++)
            {
                var chosenSpawnType = ChooseSpawnType(i, count);
                var source = NPC.GetSource_FromThis();
                for (int j = 0; j < 1500; j++)
                {
                    float r = Main.rand.NextFloat(MathHelper.TwoPi / count * i, MathHelper.TwoPi / count * (i + 1));
                    var endPoint = Point.Zero;
                    float distance = Main.rand.NextFloat(320f, 1200f);
                    bool finished = false;
                    Utils.PlotTileLine(NPC.Center + r.ToRotationVector2() * distance, NPC.Center + r.ToRotationVector2() * Main.rand.NextFloat(distance, distance * 4f), 5, (x, y) =>
                    {
                        var val = chosenSpawnType.TrySpawnEnemy(x, y);
                        if (val != null)
                        {
                            Enemies.Add(val);
                            finished = true;
                        }
                        return val == null;
                    });
                    if (finished)
                        break;
                }
            }
        }

        public void CheckTendrils()
        {
            var macroPoints = new List<List<Point>>();
            for (int i = 0; i < Enemies.Count; i++)
            {
                macroPoints.Add(new List<Point>());
                if (Enemies[i].ConnectedTendril != null)
                    continue;

                if (Enemies[i].dead)
                {
                    Enemies[i].ConnectedTendril = new List<TendrilDrawInfo>();
                    return;
                }
                var anchorPoint = NPC.Center;
                var origVelocity = Vector2.Normalize(Enemies[i].tileLoc.ToWorldCoordinates() - NPC.Center);
                var velocity = origVelocity;
                float maxDistance = (Enemies[i].tileLoc.ToWorldCoordinates() - NPC.Center).Length();
                for (int j = 0; j < 50000; j++)
                {
                    float maxDist = 160f;
                    float maxRotation = 0.1f;
                    if (Collision.SolidCollision(anchorPoint, 16, 16))
                    {
                        maxDist = 36f;
                    }
                    anchorPoint += velocity.RotatedBy(Main.rand.NextFloat(-maxRotation, maxRotation)) * Main.rand.NextFloat(maxDist / 2f, maxDist);
                    velocity = (velocity + origVelocity) / 2f;
                    macroPoints[i].Add(anchorPoint.ToTileCoordinates());
                    if (Vector2.Distance(NPC.Center, anchorPoint) > maxDistance * 0.75f)
                    {
                        break;
                    }
                }
                macroPoints[i].Add(Enemies[i].tileLoc);
            }
            int fullyDead = 0;
            for (int k = 0; k < Enemies.Count; k++)
            {
                if (Enemies[k].dead)
                {
                    if (Enemies[k].ConnectedTendril.Count == 0)
                    {
                        fullyDead++;
                        continue;
                    }
                    if (Main.GameUpdateCount % 3 == 0)
                    {
                        var p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), Enemies[k].ConnectedTendril[0].drawLoc, Vector2.Zero, ProjectileID.LunarFlare, 0, 0f, Main.myPlayer, ai1: -1f);
                        p.frameCounter = Main.rand.Next(-6, 6);
                        p.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        SoundEngine.PlaySound(SoundID.Item14, Enemies[k].ConnectedTendril[0].drawLoc);
                        for (int i = 0; i < 4; i++)
                        {
                            if (Enemies[k].ConnectedTendril.Count <= 0)
                            {
                                continue;
                            }
                            Enemies[k].ConnectedTendril.RemoveAt(0);
                        }
                    }
                }
                if (Enemies[k].ConnectedTendril != null)
                    continue;

                Enemies[k].ConnectedTendril = new List<TendrilDrawInfo>();
                var drawPoint = macroPoints[k][^1].ToWorldCoordinates();
                var velocity = Vector2.Normalize(macroPoints[k][^2].ToWorldCoordinates() - macroPoints[k][^1].ToWorldCoordinates()) * (16f);
                for (int i = macroPoints[k].Count - 1; i >= 0; i--)
                {
                    var start = macroPoints[k][i].ToWorldCoordinates();

                    var end = i == 0 ? NPC.Center : macroPoints[k][i - 1].ToWorldCoordinates();
                    float endDistance = Math.Max((end - NPC.Center).Length().UnNaN(), 18f * 4f);
                    while ((drawPoint - NPC.Center).Length() > endDistance)
                    {
                        velocity = Vector2.Lerp(velocity, Vector2.Normalize(end - drawPoint) * 16f, 0.01f);
                        Enemies[k].ConnectedTendril.Add(new TendrilDrawInfo() { drawLoc = drawPoint, rotation = velocity.ToRotation(), });
                        drawPoint += velocity * 0.5f;
                    }
                }
            }
            if (fullyDead >= Enemies.Count)
            {
                NPC.ai[0] = -1f;
            }
        }

        public override bool NeedSaving()
        {
            return Enemies != null;
        }

        public override void SaveData(TagCompound tag)
        {
            var t = new List<TagCompound>();
            foreach (var item in Enemies)
            {
                t.Add(item.SerializeData());
            }
            tag["Enemies"] = t;
        }

        public override void LoadData(TagCompound tag)
        {
            Enemies = new List<EnemyData>();
            var t = tag.GetList<TagCompound>("Enemies");
            foreach (var item in t)
            {
                var value = EnemyData.Deserialize(item);
                if (value != null)
                    Enemies.Add(value);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Enemies == null)
                return;

            writer.Write(Enemies.Count);
            foreach (var e in Enemies)
            {
                e.Send(writer);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            if (Enemies == null)
                Enemies = new List<EnemyData>(count);

            for (int i = 0; i < count; i++)
            {
                if (Enemies[i] == null)
                    Enemies[i] = new EnemyData();
                Enemies[i].Recieve(reader);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);

            var chain = ModContent.Request<Texture2D>($"{Texture}_Chain", AssetRequestMode.ImmediateLoad).Value;
            var chainOrigin = chain.Size() / 2f;
            float wave = -NPC.localAI[0] - Main.GlobalTimeWrappedHourly * 1.5f;
            if (Enemies != null)
            {
                foreach (var e in Enemies)
                {
                    if (e.ConnectedTendril != null)
                    {
                        foreach (var tendril in e.ConnectedTendril)
                        {
                            float colorWave = ((float)Math.Sin(wave * 4.1f) + 1f + (float)Math.Sin(wave * 2.1f) + 1f + (float)Math.Sin(wave * 8.1f) + 1f) / 3f;
                            spriteBatch.Draw(chain, tendril.drawLoc - screenPos, null, new Color(160, 200, 255, 255) * (0.8f + colorWave * 0.6f), tendril.rotation + MathHelper.PiOver2, chainOrigin, 1f + (float)Math.Max(Math.Pow(colorWave * 0.4f, 2), 0.0), SpriteEffects.None, 0f);
                            wave += 0.1f;
                        }
                    }
                }
            }
            foreach (var v in AequusHelpers.CircularVector(8, Main.GlobalTimeWrappedHourly))
            {
                spriteBatch.Draw(t, NPC.position + off - screenPos + v * 16f, frame, new Color(160, 200, 255, 128) * 0.2f, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(t, NPC.position + off - screenPos, frame, new Color(160, 200, 255, 255), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            float heartBeat = (float)(Math.Pow(Math.Max(Math.Sin(Main.GlobalTimeWrappedHourly * 6f + NPC.localAI[0]), 0f), 15) * 0.1);
            spriteBatch.Draw(t, NPC.position + off - screenPos, frame, new Color(160, 200, 255, 128), NPC.rotation, origin, NPC.scale + heartBeat, SpriteEffects.None, 0f);
            spriteBatch.Draw(t, NPC.position + off - screenPos, frame, new Color(160, 200, 255, 128) * 0.4f, NPC.rotation, origin, NPC.scale + heartBeat * 2f, SpriteEffects.None, 0f);
            float d = (NPC.Center - Main.LocalPlayer.Center).Length();
            if (d < 1500f)
            {
                AequusEffects.Shake.Set(Math.Max(16f * (1f - d / 1500f) * heartBeat * 6f * (1f + NPC.localAI[0] / 25f), AequusEffects.Shake.Intensity), 0.95f - NPC.localAI[0] / 90f);
            }
            return false;
        }
    }
}