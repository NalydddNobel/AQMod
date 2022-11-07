using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Debuffs.Necro;
using Aequus.Common;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Necromancy
{
    public class NecromancyNPC : GlobalNPC, IAddRecipes
    {
        public class ActiveZombieInfo
        {
            public int PlayerOwner;
            public int NPCTarget;

            public bool IsZombieRunning => PlayerOwner > -1;

            public ActiveZombieInfo()
            {
                PlayerOwner = -1;
                NPCTarget = -1;
            }

            public void Reset()
            {
                PlayerOwner = -1;
                NPCTarget = -1;
            }
        }

        public static byte CheckZombies;

        public static SoundStyle ZombieRecruitSound { get; private set; }

        public static ActiveZombieInfo Zombie { get; private set; }
        public static PlayerTargetHack TargetHack { get; set; }

        public int ghostDebuffDOT;
        public float pandoraBox;
        public int conversionChance;

        public bool isZombie;
        public int zombieOwner;
        public int zombieTimer;
        public int zombieTimerMax;
        public float zombieDebuffTier;
        public int hitCheckDelay;
        public int slotsConsumed;
        public int renderLayer;
        public int ghostDamage;
        public float ghostSpeed;

        public int netUpdateTimer;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            Zombie = new ActiveZombieInfo();
            On.Terraria.NPC.SetTargetTrackingValues += NPC_SetTargetTrackingValues;
            On.Terraria.NPC.FindFirstNPC += NPC_FindFirstNPC;
            On.Terraria.NPC.AnyNPCs += NPC_AnyNPCs;
            On.Terraria.NPC.CountNPCS += NPC_CountNPCS;
            if (!Main.dedServ)
            {
                ZombieRecruitSound = Aequus.GetSound("zombierecruit");
            }
        }

        private static int NPC_FindFirstNPC(On.Terraria.NPC.orig_FindFirstNPC orig, int Type)
        {
            if (CheckZombies == 0)
            {
                return orig(Type);
            }
            var npcs = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == Type && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie))
                {
                    if (zombie.isZombie)
                    {
                        npcs.Add(Main.npc[i]);
                        Main.npc[i].active = false;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            int val = orig(Type);
            foreach (var npc in npcs)
            {
                npc.active = true;
            }
            return val;
        }

        private static bool NPC_AnyNPCs(On.Terraria.NPC.orig_AnyNPCs orig, int Type)
        {
            if (CheckZombies == 0)
            {
                return orig(Type);
            }
            var npcs = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == Type && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie))
                {
                    if (zombie.isZombie)
                    {
                        npcs.Add(Main.npc[i]);
                        Main.npc[i].active = false;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            bool val = orig(Type);
            foreach (var npc in npcs)
            {
                npc.active = true;
            }
            return val;
        }

        private static int NPC_CountNPCS(On.Terraria.NPC.orig_CountNPCS orig, int Type)
        {
            if (CheckZombies == 0)
            {
                return orig(Type);
            }
            var npcs = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == Type && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie)
                {
                    npcs.Add(Main.npc[i]);
                    Main.npc[i].active = false;
                }
            }
            int amt = orig(Type);
            foreach (var npc in npcs)
            {
                npc.active = true;
            }
            return amt;
        }

        #region Hooks
        private static void NPC_SetTargetTrackingValues(On.Terraria.NPC.orig_SetTargetTrackingValues orig, NPC self, bool faceTarget, float realDist, int tankTarget)
        {
            if (Zombie.IsZombieRunning)
            {
                self.target = self.GetGlobalNPC<NecromancyNPC>().zombieOwner;
                if (Zombie.NPCTarget != -1)
                {
                    self.targetRect = Main.npc[Zombie.NPCTarget].getRect();
                }
                else
                {
                    self.targetRect = Main.player[self.target].getRect();
                }

                if (faceTarget)
                {
                    self.direction = self.targetRect.X + self.targetRect.Width / 2 < self.position.X + self.width / 2 ? -1 : 1;
                    self.directionY = self.targetRect.Y + self.targetRect.Height / 2 < self.position.Y + self.height / 2 ? -1 : 1;
                }

                return;
            }
            orig(self, faceTarget, realDist, tankTarget);
        }
        #endregion

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (AequusHelpers.HereditarySource(source, out var ent))
            {
                NPC parentNPC = null;
                if (ent is NPC)
                {
                    parentNPC = (NPC)ent;
                }
                if (ent is Projectile parentProjectile && parentProjectile.GetGlobalProjectile<NecromancyProj>().isZombie)
                {
                    parentNPC = Main.npc[parentProjectile.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner];
                }

                if (parentNPC != null && parentNPC.TryGetGlobalNPC<NecromancyNPC>(out var parentZombie) && npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie))
                {
                    var info = GhostSyncInfo.GetInfo(parentNPC);
                    if (parentZombie.isZombie)
                    {
                        info.SetZombieNPCInfo(npc, zombie);
                        zombie.ApplyStaticStats(npc);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            PacketSystem.SyncNecromancyOwner(npc.whoAmI, info.Player);
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (isZombie)
            {
                var color = Color.White;
                int index = GhostOutlineRenderer.TargetID(Main.player[zombieOwner], renderLayer);
                if (GhostOutlineRenderer.necromancyRenderers.Length > index && GhostOutlineRenderer.necromancyRenderers[index] != null)
                {
                    color = GhostOutlineRenderer.necromancyRenderers[index].DrawColor();
                    color.A = 100;
                }

                float wave = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
                color *= (wave + 1f) / 2f * 0.5f;
                drawColor.A = (byte)MathHelper.Clamp(drawColor.R + color.R / 2, byte.MinValue, byte.MaxValue);
                drawColor.G = (byte)MathHelper.Clamp(drawColor.G + color.G / 2, byte.MinValue, byte.MaxValue);
                drawColor.B = (byte)MathHelper.Clamp(drawColor.B + color.B / 2, byte.MinValue, byte.MaxValue);
                drawColor.A = (byte)MathHelper.Clamp(drawColor.A + wave * 50f, byte.MinValue, byte.MaxValue - 60);
                return drawColor;
            }
            return null;
        }

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff<PandorasCurse>())
            {
                pandoraBox = 1f;
            }
            conversionChance = 0;
            if (ghostDebuffDOT > 0)
            {
                ghostDebuffDOT--;
            }
            if (isZombie)
            {
                npc.DelBuff(0);
            }
            else
            {
                zombieDebuffTier = 0f;
            }
        }

        public override bool PreAI(NPC npc)
        {
            CheckZombies = 10;
            Zombie.Reset();
            if (isZombie)
            {
                var stats = NecromancyDatabase.TryGet(npc, out var g) ? g : default(GhostInfo);
                stats.Aggro?.OnPreAI(npc, this);
                if (ghostDamage > 0)
                {
                    npc.defDamage = ghostDamage;
                    npc.damage = ghostDamage;
                }
                npc.StatSpeed() += ghostSpeed;
                if (zombieTimer == 0)
                {
                    int time = Main.player[zombieOwner].Aequus().ghostLifespan;
                    if (stats.TimeLeftMultiplier.HasValue)
                    {
                        time = (int)(time * stats.TimeLeftMultiplier.Value);
                    }

                    zombieTimerMax = time;
                    zombieTimer = time;
                }
                zombieTimer--;

                if (ShouldDespawnZombie(npc))
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false;
                }
                else
                {
                    npc.life = (int)Math.Clamp(npc.lifeMax * (zombieTimer / (float)zombieTimerMax), 1f, npc.lifeMax); // Aggros slimes and stuff
                }

                RestoreTarget();

                Zombie.PlayerOwner = zombieOwner;

                npc.GivenName = Main.player[zombieOwner].name + "'s " + Lang.GetNPCName(npc.netID);
                npc.friendly = true;
                npc.boss = false;
                npc.alpha = Math.Max(npc.alpha, 60);
                npc.dontTakeDamage = true;
                npc.npcSlots = 0f;
                if (!Main.player[npc.target].active || Main.player[npc.target].dead || !Main.player[npc.target].hostile || Main.player[npc.target].team == Main.player[zombieOwner].team)
                {
                    float prioritizeMultiplier = stats.PrioritizePlayerMultiplier.GetValueOrDefault(npc.noGravity ? 2f : 1f);
                    int npcTarget = GetNPCTarget(npc, Main.player[zombieOwner], npc.netID, npc.type, prioritizeMultiplier);

                    if (npcTarget != -1)
                    {
                        TargetHack = new PlayerTargetHack(npc, Main.npc[npcTarget], Main.player[zombieOwner], Main.npc[npcTarget].Center);
                        TargetHack.Move();
                        Zombie.NPCTarget = npcTarget;
                    }
                    UpdateHitbox(npc);
                }
            }
            return true;
        }
        public bool ShouldDespawnZombie(NPC npc)
        {
            return zombieTimer <= 0 || !Main.player[zombieOwner].active || Main.player[zombieOwner].dead;
        }
        public void UpdateHitbox(NPC npc)
        {
            if (hitCheckDelay <= 0)
            {
                hitCheckDelay = 30;
                try
                {
                    if (Main.myPlayer == zombieOwner)
                    {
                        Zombie.PlayerOwner = -1;
                        try
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<GhostHitbox>() && Main.projectile[i].TryGetGlobalProjectile<NecromancyProj>(out var zombie))
                                {
                                    if (zombie.zombieNPCOwner == npc.whoAmI)
                                    {
                                        hitCheckDelay = 120;
                                        return;
                                    }
                                }
                            }
                            int damage = ghostDamage > 0 ? ghostDamage : (int)(npc.damage * GetDamageMultiplier(npc, npc.damage));
                            int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, Vector2.Normalize(npc.velocity) * 0.01f, ModContent.ProjectileType<GhostHitbox>(), damage, 3f, zombieOwner, npc.whoAmI, 64f);
                            Main.projectile[p].originalDamage = damage;
                        }
                        catch
                        {
                        }
                        Zombie.PlayerOwner = zombieOwner;
                    }
                }
                catch
                {

                }
            }
            else
            {
                hitCheckDelay--;
            }
        }

        public override void PostAI(NPC npc)
        {
            if (isZombie)
            {
                var stats = NecromancyDatabase.TryGet(npc, out var g) ? g : default(GhostInfo);
                stats.Aggro?.OnPostAI(npc, this);
                if (ghostDamage > 0)
                {
                    npc.defDamage = ghostDamage;
                    npc.damage = ghostDamage;
                }
                npc.target = zombieOwner;
                //Main.NewText($"{ghostDamage} | {npc.damage}");
                npc.dontTakeDamage = true;
                RestoreTarget();
                var player = Main.player[zombieOwner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                aequus.ghostSlots += slotsConsumed;

                if (isZombie && aequus.setGravetenderGhost == npc.whoAmI && !stats.DontModifyVelocity)
                {
                    npc.StatSpeed() *= 1.5f;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.rand.NextBool(6))
                    {
                        var color = new Color(50, 150, 255, 100);
                        int index = GhostOutlineRenderer.TargetID(Main.player[zombieOwner], renderLayer);
                        if (GhostOutlineRenderer.necromancyRenderers.Length > index && GhostOutlineRenderer.necromancyRenderers[index] != null)
                        {
                            color = GhostOutlineRenderer.necromancyRenderers[index].DrawColor();
                            color.A = 100;
                        }
                        var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), newColor: color);
                        d.velocity *= 0.2f;
                        d.velocity += npc.velocity * 0.4f;
                        d.scale *= npc.scale;
                        d.noGravity = true;
                    }
                    if (aequus.setGravetenderGhost == npc.whoAmI && Main.rand.NextBool(6))
                    {
                        var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(), newColor: new Color(200, 50, 128, 25));
                        d.velocity *= 0.5f;
                        d.velocity += -npc.velocity * 0.2f;
                        d.scale *= npc.scale;
                        d.fadeIn = d.scale + 0.5f;
                        d.noGravity = true;
                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                }
                else
                {
                    netUpdateTimer--;
                    if (netUpdateTimer <= -10 || npc.netUpdate)
                    {
                        npc.netUpdate = true;
                        netUpdateTimer = 30 + npc.netSpam * 5;
                        Sync(npc);
                        npc.netSpam++;
                    }
                }
            }
            Zombie.Reset();
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (ghostDebuffDOT > 0)
            {
                npc.AddRegen(-(int)(ghostDebuffDOT * pandoraBox));
                damage = Math.Max(damage, ghostDebuffDOT / 20);
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (isZombie && !GhostOutlineRenderer.RenderingNow && !npc.IsABestiaryIconDummy && npc.lifeMax > 1 && !NPCID.Sets.ProjectileNPC[npc.type])
            {
                GhostOutlineRenderer.Target(Main.player[zombieOwner], renderLayer).Add(npc.whoAmI);
            }
            return true;
        }
        public void DrawHealthbar(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (Main.HealthBarDrawSettings == 0 || npc.life < 0)
            {
                return;
            }
            float y = 0f;
            if (Main.HealthBarDrawSettings == 1)
            {
                y += Main.NPCAddHeight(npc) + npc.height;
            }
            else if (Main.HealthBarDrawSettings == 2)
            {
                y -= Main.NPCAddHeight(npc) / 2f;
            }
            var center = npc.Center;
            InnerDrawHealthbar(npc, spriteBatch, screenPos, center.X, npc.position.Y + y + npc.gfxOffY, npc.life, npc.lifeMax, Lighting.Brightness((int)(center.X / 16f), (int)((center.Y + npc.gfxOffY) / 16f)), 1f);
        }
        public void InnerDrawHealthbar(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, float x, float y, int life, int maxLife, float alpha, float scale)
        {
            var hb = TextureAssets.Hb1.Value;
            var hbBack = TextureAssets.Hb2.Value;

            float lifeRatio = MathHelper.Clamp(life / (float)maxLife, 0f, 1f);
            int scaleX = (int)MathHelper.Clamp(hb.Width * lifeRatio, 2f, hb.Width - 2f);

            x -= hb.Width / 2f * scale;
            y += hb.Height; //I kind of like how they're lower than the vanilla hb spots
            if (Main.LocalPlayer.gravDir == -1f)
            {
                y -= Main.screenPosition.Y;
                y = Main.screenPosition.Y + Main.screenHeight - y;
            }
            var color = DetermineHealthbarColor(npc, lifeRatio);

            spriteBatch.Draw(hb, new Vector2(x - screenPos.X, y - screenPos.Y), new Rectangle(0, 0, 2, hb.Height), color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(hb, new Vector2(x - screenPos.X + 2 * scale, y - screenPos.Y), new Rectangle(2, 0, scaleX, hb.Height), color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(hb, new Vector2(x - screenPos.X + scaleX * scale, y - screenPos.Y), new Rectangle(hb.Width - 2, 0, 2, hb.Height), color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(hbBack, new Vector2(x - screenPos.X + (scaleX + 2) * scale, y - screenPos.Y),
                new Rectangle(scaleX + 2, 0, hbBack.Width - scaleX - 2, hbBack.Height), color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
        }
        public Color DetermineHealthbarColor(NPC npc, float lifeRatio)
        {
            int team = Main.player[zombieOwner].team;
            Color color;
            if (zombieOwner == Main.myPlayer)
            {
                color = Color.White;
                int index = GhostOutlineRenderer.TargetID(Main.player[zombieOwner], renderLayer);
                if (GhostOutlineRenderer.necromancyRenderers.Length > index && GhostOutlineRenderer.necromancyRenderers[index] != null)
                {
                    color = GhostOutlineRenderer.necromancyRenderers[index].DrawColor();
                    color.A = 100;
                }
            }
            else
            {
                color = Main.teamColor[Main.player[zombieOwner].team];
            }
            return Color.Lerp(color, (color * 0.5f).UseA(255), 1f - lifeRatio);
        }

        public void SpawnZombie(NPC npc)
        {
            var myZombie = npc.GetGlobalNPC<NecromancyNPC>();
            myZombie.zombieTimer = 1;
            myZombie.zombieTimerMax = 1;
            int myPriority = myZombie.DespawnPriority(npc);
            var myAequus = Main.player[myZombie.zombieOwner].Aequus();
            myZombie.ApplyStaticStats(npc);
            int slotsToConsume = myZombie.slotsConsumed;
            if (myAequus.ghostSlotsOld + slotsToConsume >= myAequus.ghostSlotsMax)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie && zombie.zombieOwner == myZombie.zombieOwner && zombie.slotsConsumed > 0)
                    {
                        int priority = zombie.DespawnPriority(Main.npc[i]);
                        if (priority < myPriority)
                        {
                            slotsToConsume -= zombie.slotsConsumed;
                            if (slotsToConsume <= 0)
                                break;
                        }
                    }
                }
                if (slotsToConsume > 0)
                    return;
            }
            int n = NPC.NewNPC(npc.GetSource_Death("Aequus:Zombie"), (int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height / 2, npc.netID, npc.whoAmI + 1);
            if (n < 200)
            {
                Main.npc[n].whoAmI = n;
                SpawnZombie_SetZombieStats(Main.npc[n], npc.Center, npc.velocity, npc.direction, npc.spriteDirection, out bool playSound);
                if (playSound)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        PacketSystem.Send((p) => p.WriteVector2(npc.Center), PacketType.SyncZombieRecruitSound);
                    }
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        SoundEngine.PlaySound(ZombieRecruitSound);
                    }
                }
            }
        }
        public void SpawnZombie_SetZombieStats(NPC zombieNPC, Vector2 position, Vector2 velocity, int direction, int spriteDirection, out bool playSound)
        {
            var zombie = zombieNPC.GetGlobalNPC<NecromancyNPC>();
            zombie.isZombie = true;
            zombie.zombieOwner = zombieOwner;
            zombie.zombieDebuffTier = zombieDebuffTier;
            zombie.zombieTimer = zombie.zombieTimerMax = Main.player[zombieOwner].Aequus().ghostLifespan;
            zombie.renderLayer = renderLayer;
            zombie.ghostSpeed = ghostSpeed;
            zombie.ghostDamage = ghostDamage;
            zombie.ApplyStaticStats(zombieNPC);
            zombieNPC.Center = position;
            zombieNPC.velocity = velocity * 0.25f;
            zombieNPC.direction = direction;
            zombieNPC.spriteDirection = spriteDirection;
            zombieNPC.friendly = true;
            zombieNPC.extraValue = 0;
            zombieNPC.value = 0;
            zombieNPC.boss = false;
            zombieNPC.SpawnedFromStatue = true;
            if (zombieNPC.ModNPC != null)
            {
                zombieNPC.ModNPC.Music = -1;
                zombieNPC.ModNPC.SceneEffectPriority = SceneEffectPriority.None;
            }
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, zombieNPC.whoAmI);
            }
            playSound = true;
        }

        public void ApplyStaticStats(NPC npc)
        {
            slotsConsumed = NecromancyDatabase.TryGet(npc, out var g) ? g.slotsUsed.GetValueOrDefault(1) : 0;
        }

        public void RenderLayer(int layer)
        {
            if (renderLayer < layer)
            {
                renderLayer = layer;
            }
        }

        public void DebuffTier(float tier)
        {
            if (zombieDebuffTier < tier)
            {
                zombieDebuffTier = tier;
            }
        }

        public void ConversionChance(int amt)
        {
            if (conversionChance == 0)
            {
                conversionChance = amt;
            }
            else
            {
                conversionChance = Math.Min(amt, conversionChance);
            }
        }

        public static int GetNPCTarget(Entity entity, Player player, int netID, int npcType, float prioritizePlayerMultiplier = 1f)
        {
            int target = -1;
            float distance = NecromancyDatabase.TryGet(netID, out var g) ? g.ViewDistance : 2000f;
            if (distance < 800f)
            {
                distance = 800f;
            }
            int closestToPlayer = player.Aequus().closestEnemy;
            int minionTarget = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                minionTarget = player.MinionAttackTargetNPC;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(entity) &&
                    !NPCID.Sets.CountsAsCritter[Main.npc[i].type])
                {
                    float c = entity.Distance(Main.npc[i].Center);
                    if (i == closestToPlayer)
                    {
                        c /= 4f * prioritizePlayerMultiplier;
                    }
                    if (i == minionTarget)
                    {
                        c /= 6f * prioritizePlayerMultiplier;
                    }
                    if (c < distance)
                    {
                        target = i;
                        distance = c;
                    }
                }
            }
            return target;
        }

        public static float GetDamageMultiplier(NPC npc, int originalDamage)
        {
            float dmgMultiplier = 1f;
            if (npc.boss)
            {
                dmgMultiplier += 4;
            }

            float addMultiplier = 0.5f;
            float healthAdditions = npc.lifeMax / (float)(2500 + originalDamage * 5);
            while (healthAdditions > 0f)
            {
                if (healthAdditions < 1f)
                {
                    addMultiplier *= healthAdditions;
                }
                dmgMultiplier += addMultiplier;
                addMultiplier /= 2f;
                healthAdditions -= 1f;
            }

            return Math.Max(dmgMultiplier / 2f, !Main.expertMode ? 1.33f : 1f);
        }

        public int DespawnPriority(NPC npc)
        {
            float priority = 10000f;
            if (NecromancyDatabase.TryGet(npc, out var g))
            {
                priority = g.despawnPriority;
            }
            return (int)(zombieTimer / (float)zombieTimerMax * priority);
        }

        public void Send(BinaryWriter writer)
        {
            writer.Write(isZombie);
            if (isZombie)
            {
                writer.Write(zombieTimer);
                writer.Write(zombieTimerMax);
                writer.Write(slotsConsumed);
            }
            else
            {
                writer.Write(ghostDebuffDOT);
            }
            writer.Write(zombieOwner);
            writer.Write(zombieDebuffTier);
            writer.Write((byte)renderLayer);
        }

        public void Receive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                isZombie = true;
                zombieTimer = reader.ReadInt32();
                zombieTimerMax = reader.ReadInt32();
                slotsConsumed = reader.ReadInt32();
            }
            else
            {
                ghostDebuffDOT = reader.ReadInt32();
            }
            zombieOwner = reader.ReadInt32();
            zombieDebuffTier = reader.ReadSingle();
            renderLayer = reader.ReadByte();
        }

        public static void Sync(NPC npc)
        {
            if (npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie))
            {
                PacketSystem.Send((p) => { p.Write((byte)npc.whoAmI); zombie.Send(p); }, PacketType.SyncNecromancyNPC);
            }
        }
        public static void Sync(int npc)
        {
            Sync(Main.npc[npc]);
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            var buffList = new List<int>(NecromancyDatabase.NecromancyDebuffs);
            buffList.Remove(ModContent.BuffType<EnthrallingDebuff>());
            for (int i = NPCID.NegativeIDCount + 1; i < Main.maxNPCTypes; i++)
            {
                if (!NecromancyDatabase.TryGet(i, out var stats) || stats.PowerNeeded == GhostInfo.Invalid.PowerNeeded)
                {
                    AequusBuff.AddStaticImmunity(i, false, buffList.ToArray());
                }
            }
        }

        public static void RestoreTarget()
        {
            if (TargetHack.HasInfo)
            {
                TargetHack.Restore();
                TargetHack = PlayerTargetHack.None;
            }
        }
    }
}