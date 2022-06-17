using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Debuffs.Necro;
using Aequus.Common;
using Aequus.Common.Networking;
using Aequus.Graphics;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModGlobalsNet;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Necromancy
{
    public class NecromancyNPC : GlobalNPC, IGlobalsNetworker, IAddRecipes
    {
        public static HashSet<int> BlacklistVelocityBoost { get; private set; }

        public static bool AI_IsZombie;
        public static int AI_ZombiePlayerOwner;
        public static int AI_NPCTarget;
        public static Vector2 AI_ReturnPlayerLocation;

        public static SoundStyle ZombieRecruitSound { get; private set; }

        public int zombieDrain;
        public bool isZombie;
        public int zombieOwner;
        public int zombieTimer;
        public int zombieTimerMax;
        public float zombieDebuffTier;
        public int hitCheckDelay;
        public int slotsConsumed;
        public int conversionChance;
        public int renderLayer;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            BlacklistVelocityBoost = new HashSet<int>();
            On.Terraria.NPC.Transform += NPC_Transform;
            On.Terraria.NPC.SetTargetTrackingValues += NPC_SetTargetTrackingValues;
            On.Terraria.NPC.UpdateCollision += NPC_UpdateCollision;
            if (!Main.dedServ)
            {
                ZombieRecruitSound = Aequus.GetSound("zombierecruit");
            }
        }

        #region Hooks
        private static void NPC_UpdateCollision(On.Terraria.NPC.orig_UpdateCollision orig, NPC self)
        {
            if (self.TryGetGlobalNPC<NecromancyNPC>(out var z) && z.isZombie)
            {
                float velocityBoost = z.DetermineVelocityBoost(self, Main.player[z.zombieOwner], Main.player[z.zombieOwner].Aequus());
                if (velocityBoost > 0f)
                {
                    self.velocity *= 1f + velocityBoost;
                }
                orig(self);
                if (velocityBoost > 0f)
                {
                    self.velocity /= 1f + velocityBoost;
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void NPC_Transform(On.Terraria.NPC.orig_Transform orig, NPC self, int newType)
        {
            bool isZombieOld = self.GetGlobalNPC<NecromancyNPC>().isZombie;
            int owner = -1;
            int timer = -1;
            float tier = -1f;
            int renderLayer = -1;
            if (isZombieOld)
            {
                var zombie = self.GetGlobalNPC<NecromancyNPC>();
                owner = zombie.zombieOwner;
                timer = zombie.zombieTimer;
                tier = zombie.zombieDebuffTier;
                renderLayer = zombie.renderLayer;
            }

            orig(self, newType);

            if (isZombieOld)
            {
                var zombie = self.GetGlobalNPC<NecromancyNPC>();
                zombie.isZombie = true;
                zombie.zombieOwner = owner;
                zombie.zombieTimer = timer;
                zombie.zombieDebuffTier = tier;
                zombie.renderLayer = renderLayer;
            }
        }

        private static void NPC_SetTargetTrackingValues(On.Terraria.NPC.orig_SetTargetTrackingValues orig, NPC self, bool faceTarget, float realDist, int tankTarget)
        {
            if (AI_IsZombie)
            {
                self.target = self.GetGlobalNPC<NecromancyNPC>().zombieOwner;
                if (AI_NPCTarget != -1)
                {
                    self.targetRect = Main.npc[AI_NPCTarget].getRect();
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
            if (source is EntitySource_OnHit onHit)
            {
                ZombieCheck(onHit.EntityStruck, npc);
            }
            else if (source is EntitySource_Parent parent)
            {
                ZombieCheck(parent.Entity, npc);
            }
            else if (source is EntitySource_HitEffect hitEffect)
            {
                ZombieCheck(hitEffect.Entity, npc);
            }
            else if (source is EntitySource_Death death)
            {
                ZombieCheck(death.Entity, npc);
            }
        }
        public void ZombieCheck(Entity entity, NPC npc)
        {
            bool sendPacket = false;
            int player = 0;
            float tier = 0f;
            if (entity is NPC parentNPC && parentNPC.GetGlobalNPC<NecromancyNPC>().isZombie)
            {
                player = parentNPC.GetGlobalNPC<NecromancyNPC>().zombieOwner;
                tier = parentNPC.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier;
                ZombifyChild(npc, parentNPC, player, tier);
                sendPacket = true;
            }
            else if (entity is Projectile parentProjectile && parentProjectile.GetGlobalProjectile<NecromancyProj>().isZombie)
            {
                player = parentProjectile.owner;
                tier = parentProjectile.GetGlobalProjectile<NecromancyProj>().zombieDebuffTier;
                ZombifyChild(npc, Main.npc[parentProjectile.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner], player, tier);
                sendPacket = true;
            }

            if (sendPacket && Main.netMode != NetmodeID.SinglePlayer)
            {
                PacketHandler.SyncNecromancyOwner(npc.whoAmI, player, tier);
            }
        }
        public void ZombifyChild(NPC npc, NPC parentNPC, int player, float tier)
        {
            npc.boss = false;
            npc.friendly = true;
            npc.SpawnedFromStatue = true;
            npc.extraValue = 0;
            npc.value = 0;
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            var parentZombie = parentNPC.GetGlobalNPC<NecromancyNPC>();
            zombie.zombieOwner = player;
            zombie.zombieTimer = parentZombie.zombieTimer;
            zombie.zombieTimerMax = parentZombie.zombieTimerMax;
            zombie.zombieDebuffTier = tier;
            zombie.renderLayer = parentZombie.renderLayer;
            zombie.isZombie = true;
            zombie.OnSpawnZombie(npc);
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (isZombie)
            {
                float wave = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
                drawColor.A = (byte)MathHelper.Clamp(drawColor.R - 100, byte.MinValue, byte.MaxValue);
                drawColor.G = (byte)MathHelper.Clamp(drawColor.G - (50 + (int)(Math.Max(0f, wave) * 10f)), drawColor.R, byte.MaxValue);
                drawColor.B = (byte)MathHelper.Clamp(drawColor.B + 100, drawColor.G, byte.MaxValue);
                drawColor.A = (byte)MathHelper.Clamp(drawColor.A + wave * 50f, byte.MinValue, byte.MaxValue - 60);
                return drawColor;
            }
            return null;
        }

        public override void ResetEffects(NPC npc)
        {
            if (!isZombie)
            {
                zombieDebuffTier = 0f;
            }
            conversionChance = 0;
            if (zombieDrain > 0)
            {
                zombieDrain--;
            }
            if (isZombie)
            {
                npc.DelBuff(0);
            }
        }

        public override bool PreAI(NPC npc)
        {
            AI_IsZombie = isZombie;
            AI_NPCTarget = -1;
            if (isZombie)
            {
                var stats = NecromancyDatabase.GetByNetID(npc.netID, npc.type);
                stats.Aggro?.OnPreAI(npc, this);
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

                if (AI_ReturnPlayerLocation != Vector2.Zero)
                {
                    Main.player[zombieOwner].position = AI_ReturnPlayerLocation;
                    AI_ReturnPlayerLocation = Vector2.Zero;
                }

                AI_ZombiePlayerOwner = zombieOwner;

                npc.GivenName = Main.player[zombieOwner].name + "'s " + Lang.GetNPCName(npc.netID);
                npc.friendly = true;
                npc.boss = false;
                npc.alpha = Math.Max(npc.alpha, 60);
                npc.dontTakeDamage = true;
                npc.npcSlots = 0f;
                if (!Main.player[npc.target].active || Main.player[npc.target].dead || !Main.player[npc.target].hostile || Main.player[npc.target].team == Main.player[zombieOwner].team)
                {
                    npc.target = zombieOwner;
                    float prioritizeMultiplier = stats.PrioritizePlayerMultiplier.GetValueOrDefault(npc.noGravity ? 2f : 1f);
                    int npcTarget = GetNPCTarget(npc, Main.player[zombieOwner], npc.netID, npc.type, prioritizeMultiplier);

                    if (npcTarget != -1)
                    {
                        AI_ReturnPlayerLocation = Main.player[zombieOwner].position;
                        AI_NPCTarget = npcTarget;
                        Main.player[zombieOwner].Center = Main.npc[npcTarget].Center;
                        UpdateHitbox(npc);
                    }
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
                        AI_IsZombie = false;
                        try
                        {
                            float multiplier = GetDamageMultiplier(npc, npc.damage);
                            int noSummonBoostDamage = (int)(npc.damage * multiplier);
                            int summonDamage = (int)(noSummonBoostDamage * Main.player[zombieOwner].GetTotalDamage(Aequus.NecromancyDamage).Multiplicative);
                            int p = Projectile.NewProjectile(npc.GetSource_FromThis("Aequus:NecromancyNPCHitbox"), npc.position, Vector2.Normalize(npc.velocity) * 0.01f, ModContent.ProjectileType<GhostHitbox>(), summonDamage, 1f, zombieOwner, npc.whoAmI);
                            Main.projectile[p].width = npc.width;
                            Main.projectile[p].height = npc.height;
                            Main.projectile[p].position = npc.position;
                            Main.projectile[p].originalDamage = noSummonBoostDamage;
                        }
                        catch
                        {

                        }
                        AI_IsZombie = true;
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
                var stats = NecromancyDatabase.GetByNetID(npc);
                stats.Aggro?.OnPostAI(npc, this);

                npc.dontTakeDamage = true;
                if (AI_ReturnPlayerLocation != Vector2.Zero)
                {
                    Main.player[zombieOwner].position = AI_ReturnPlayerLocation;
                    AI_ReturnPlayerLocation = Vector2.Zero;
                }
                var player = Main.player[zombieOwner];
                var aequus = player.GetModPlayer<AequusPlayer>();
                aequus.ghostSlots += slotsConsumed;

                if (zombieOwner == Main.myPlayer && aequus.pandorasBoxItem != null)
                {
                    UsePandorasBox(npc, aequus, aequus.pandorasBoxItem, AI_NPCTarget);
                }

                if (npc.noTileCollide)
                {
                    float velocityBoost = DetermineVelocityBoost(npc, player, aequus);
                    if (velocityBoost > 0f)
                    {
                        npc.position += npc.velocity * velocityBoost;
                    }
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.rand.NextBool(6))
                    {
                        var color = new Color(50, 150, 255, 100);
                        int index = GhostOutlineTarget.GetScreenTargetIndex(Main.player[zombieOwner], renderLayer);
                        if (AequusEffects.necromancyRenderers.Length > index && AequusEffects.necromancyRenderers[index] != null)
                        {
                            color = AequusEffects.necromancyRenderers[index].DrawColor();
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
                        var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(), newColor: new Color(200, 50, 128, 100));
                        d.velocity *= 0.5f;
                        d.velocity += -npc.velocity * 0.2f;
                        d.scale *= npc.scale;
                        d.fadeIn = d.scale + 0.5f;
                        d.noGravity = true;
                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                }
            }
            AI_IsZombie = false;
        }

        public float DetermineVelocityBoost(NPC npc, Player player, AequusPlayer aequus)
        {
            float value = 0f;
            if (aequus.setGravetenderGhost == npc.whoAmI)
            {
                value += 0.5f;
            }
            return value;
        }

        public void UsePandorasBox(NPC npc, AequusPlayer aequus, Item pandorasBox, int target)
        {
            if (target == -1)
            {
                return;
            }

            if (Main.rand.NextBool(aequus.pandorasBoxSpawnChance))
            {
                PandorasBox_SpawnProjectile(npc, aequus, pandorasBox, target, Main.rand.Next(PandorasBox.ProjectileTypesShot));
            }
        }

        public void PandorasBox_SpawnProjectile(NPC npc, AequusPlayer aequus, Item pandorasBox, int target, int projectileType)
        {
            var to = npc.DirectionTo(Main.npc[target].Center);
            float speed = 8f;
            if (projectileType == ProjectileID.DemonScythe)
            {
                speed = 0.2f;
            }
            int damage = 40;
            damage += aequus.Player.GetWeaponDamage(pandorasBox);
            Projectile.NewProjectileDirect(npc.GetSource_Accessory(pandorasBox), npc.Center, to * speed, projectileType, damage, 1f, Main.myPlayer);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (zombieDrain > 0)
            {
                int dot = zombieDrain / AequusHelpers.NPCREGEN;
                npc.AddRegen(-dot);
                if (damage < dot)
                    damage = dot;
            }
        }

        public void SpawnZombie(NPC npc)
        {
            int n = NPC.NewNPC(npc.GetSource_Death("Aequus:Zombie"), (int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height / 2, npc.netID, npc.whoAmI + 1);
            if (n < 200)
            {
                Main.npc[n].whoAmI = n;
                SpawnZombie_SetZombieStats(Main.npc[n], npc.Center, npc.velocity, npc.direction, npc.spriteDirection, out bool playSound);
                if (playSound)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        SoundHelpers.SendSound(SoundHelpers.NetSoundID.ZombieRecruit, npc.Center);
                    }
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        SoundEngine.PlaySound(ZombieRecruitSound);
                    }
                }
                PacketHandler.SyncNPC(Main.npc[n]);
            }
        }
        public void SpawnZombie_SetZombieStats(NPC zombieNPC, Vector2 position, Vector2 velocity, int direction, int spriteDirection, out bool playSound)
        {
            zombieNPC.GetGlobalNPC<NecromancyNPC>().isZombie = true;
            zombieNPC.GetGlobalNPC<NecromancyNPC>().zombieOwner = zombieOwner;
            zombieNPC.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = zombieDebuffTier;
            zombieNPC.GetGlobalNPC<NecromancyNPC>().zombieTimer = zombieNPC.GetGlobalNPC<NecromancyNPC>().zombieTimerMax =
                Main.player[zombieOwner].Aequus().ghostLifespan;
            zombieNPC.GetGlobalNPC<NecromancyNPC>().renderLayer = renderLayer;
            zombieNPC.GetGlobalNPC<NecromancyNPC>().OnSpawnZombie(zombieNPC);
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

        public void OnSpawnZombie(NPC npc)
        {
            slotsConsumed = NecromancyDatabase.GetByNetID(npc).slotsUsed.GetValueOrDefault(1);
        }

        public static int GetNPCTarget(Entity entity, Player player, int netID, int npcType, float prioritizePlayerMultiplier = 1f)
        {
            int target = -1;
            float distance = NecromancyDatabase.GetByNetID(netID, npcType).ViewDistance;
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

            return dmgMultiplier / 2f;
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

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (isZombie && !GhostOutlineTarget.RenderingNow && !npc.IsABestiaryIconDummy && npc.lifeMax > 1 && !NPCID.Sets.ProjectileNPC[npc.type])
            {
                int index = GhostOutlineTarget.GetScreenTargetIndex(Main.player[zombieOwner], renderLayer);
                if (AequusEffects.necromancyRenderers.Length <= index)
                {
                    Array.Resize(ref AequusEffects.necromancyRenderers, index + 1);
                }

                if (AequusEffects.necromancyRenderers[index] == null)
                {
                    int team = Main.player[zombieOwner].team;
                    AequusEffects.necromancyRenderers[index] = new GhostOutlineTarget(team, index, () => Main.teamColor[team]);
                }

                AequusEffects.necromancyRenderers[index].Add(npc.whoAmI);
                DrawHealthbar(npc, spriteBatch, screenPos);
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
                int index = GhostOutlineTarget.GetScreenTargetIndex(Main.player[zombieOwner], renderLayer);
                if (AequusEffects.necromancyRenderers.Length > index && AequusEffects.necromancyRenderers[index] != null)
                {
                    color = AequusEffects.necromancyRenderers[index].DrawColor();
                    color.A = 100;
                }
            }
            else
            {
                color = Main.teamColor[Main.player[zombieOwner].team];
            }
            return Color.Lerp(color, (color * 0.5f).UseA(255), 1f - lifeRatio);
        }

        public int DespawnPriority(NPC npc) => (int)(zombieTimer / (float)zombieTimerMax * NecromancyDatabase.GetOrDefault(npc.type).despawnPriority);

        void IGlobalsNetworker.Send(int whoAmI, BinaryWriter writer)
        {
            writer.Write(Main.npc[whoAmI].active);
            if (Main.npc[whoAmI].active)
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
                    writer.Write(zombieDrain);
                }
                writer.Write(zombieOwner);
                writer.Write(zombieDebuffTier);
                writer.Write((byte)renderLayer);
            }
        }

        void IGlobalsNetworker.Receive(int whoAmI, BinaryReader reader)
        {
            if (reader.ReadBoolean())
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
                    zombieDrain = reader.ReadInt32();
                }
                zombieOwner = reader.ReadInt32();
                zombieDebuffTier = reader.ReadSingle();
                renderLayer = reader.ReadByte();
            }
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            var buffList = new List<int>(NecromancyDatabase.NecromancyDebuffs);
            buffList.Remove(ModContent.BuffType<EnthrallingDebuff>());
            for (int i = NPCID.NegativeIDCount + 1; i < Main.maxNPCTypes; i++)
            {
                if (!NecromancyDatabase.TryGetByNetID(i, NPCID.FromNetId(i), out var stats) || stats.PowerNeeded == GhostInfo.Invalid.PowerNeeded)
                {
                    AequusBuff.AddStaticImmunity(i, false, buffList.ToArray());
                }
            }
        }
    }

    public class NecromancyProj : GlobalProjectile
    {
        public static HashSet<int> BlacklistExtraUpdates { get; private set; }

        public bool isZombie;
        public int zombieNPCOwner;
        public float zombieDebuffTier;
        public int renderLayer;
        public bool appliedEffects;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            BlacklistExtraUpdates = new HashSet<int>()
            {
                ProjectileID.SandnadoHostile,
            };
            On.Terraria.Projectile.Kill += Projectile_Kill;
        }

        private static void Projectile_Kill(On.Terraria.Projectile.orig_Kill orig, Projectile self)
        {
            NecromancyNPC.AI_IsZombie = self.GetGlobalProjectile<NecromancyProj>().isZombie;
            try
            {
                orig(self);
            }
            catch
            {
            }
            NecromancyNPC.AI_IsZombie = false;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_OnHit onHit)
            {
                ZombieCheck(onHit.EntityStruck, projectile);
            }
            else if (source is EntitySource_Parent parent)
            {
                ZombieCheck(parent.Entity, projectile);
            }
            else if (source is EntitySource_HitEffect hitEffect)
            {
                ZombieCheck(hitEffect.Entity, projectile);
            }
            else if (source is EntitySource_Death death)
            {
                ZombieCheck(death.Entity, projectile);
            }
            else if (source is EntitySource_ItemUse itemUse)
            {
                ZombieCheck(itemUse.Entity, projectile);
            }
        }
        public void ZombieCheck(Entity entity, Projectile projectile)
        {
            if (entity is Projectile proj && proj.GetGlobalProjectile<NecromancyProj>().isZombie)
            {
                ZombifyChild(projectile, proj.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner, proj.GetGlobalProjectile<NecromancyProj>().zombieDebuffTier, proj.timeLeft + 10, proj.GetGlobalProjectile<NecromancyProj>().renderLayer);
            }
            else if (entity is NPC npc && npc.GetGlobalNPC<NecromancyNPC>().isZombie)
            {
                ZombifyChild(projectile, entity.whoAmI, npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier, npc.GetGlobalNPC<NecromancyNPC>().zombieTimer, npc.GetGlobalNPC<NecromancyNPC>().renderLayer);
            }
        }
        public void ZombifyChild(Projectile projectile, int npc, float tier, int timeLeft, int renderLayer)
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.owner = NecromancyNPC.AI_ZombiePlayerOwner;
            projectile.DamageType = Aequus.NecromancyDamage;
            if (!projectile.usesLocalNPCImmunity)
            {
                projectile.usesIDStaticNPCImmunity = true;
                projectile.idStaticNPCHitCooldown = 10;
            }
            isZombie = true;
            zombieNPCOwner = npc;
            zombieDebuffTier = tier;
            this.renderLayer = renderLayer;
            projectile.timeLeft = Math.Max(Math.Min(projectile.timeLeft, timeLeft - 10), 2);
        }

        public override Color? GetAlpha(Projectile projectile, Color drawColor)
        {
            if (isZombie)
            {
                float wave = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
                drawColor.A = (byte)MathHelper.Clamp(drawColor.R - 100, byte.MinValue, byte.MaxValue);
                drawColor.G = (byte)MathHelper.Clamp(drawColor.G - (50 + (int)(Math.Max(0f, wave) * 10f)), drawColor.R, byte.MaxValue);
                drawColor.B = (byte)MathHelper.Clamp(drawColor.B + 100, drawColor.G, byte.MaxValue);
                drawColor.A = (byte)MathHelper.Clamp(drawColor.A + wave * 50f, byte.MinValue, byte.MaxValue - 60);
                return drawColor;
            }
            return null;
        }

        public override bool PreAI(Projectile projectile)
        {
            NecromancyNPC.AI_IsZombie = isZombie;
            NecromancyNPC.AI_NPCTarget = -1;
            if (isZombie)
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (!appliedEffects)
                {
                    if (aequus.ghostProjExtraUpdates > 0)
                    {
                        if (!BlacklistExtraUpdates.Contains(projectile.type))
                            projectile.extraUpdates = (projectile.extraUpdates + 1) * (aequus.ghostProjExtraUpdates + 1) - 1;
                    }
                    appliedEffects = true;
                }
                if (!Main.npc[zombieNPCOwner].active)
                {
                    projectile.Kill();
                    return true;
                }
                if (NecromancyNPC.AI_ReturnPlayerLocation != Vector2.Zero)
                {
                    Main.player[projectile.owner].position = NecromancyNPC.AI_ReturnPlayerLocation;
                    NecromancyNPC.AI_ReturnPlayerLocation = Vector2.Zero;
                }

                NecromancyNPC.AI_ZombiePlayerOwner = projectile.owner;

                projectile.hostile = false;
                projectile.friendly = true;
                projectile.alpha = Math.Max(projectile.alpha, 60);
                int npcTarget = NecromancyNPC.GetNPCTarget(projectile, Main.player[Main.npc[zombieNPCOwner].GetGlobalNPC<NecromancyNPC>().zombieOwner], Main.npc[zombieNPCOwner].netID, Main.npc[zombieNPCOwner].type);

                if (npcTarget != -1)
                {
                    NecromancyNPC.AI_ReturnPlayerLocation = Main.player[projectile.owner].position;
                    NecromancyNPC.AI_NPCTarget = npcTarget;
                    Main.player[projectile.owner].Center = Main.npc[npcTarget].Center;
                }

                SpecialProjecitleAI(projectile);
            }
            return true;
        }
        public void SpecialProjecitleAI(Projectile projectile)
        {
            if (projectile.type == ProjectileID.Spike || projectile.type == ProjectileID.FrostWave)
            {
                projectile.scale = 1f;
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (isZombie)
            {
                if (NecromancyNPC.AI_ReturnPlayerLocation != Vector2.Zero)
                {
                    Main.player[projectile.owner].position = NecromancyNPC.AI_ReturnPlayerLocation;
                    NecromancyNPC.AI_ReturnPlayerLocation = Vector2.Zero;
                }
                if (Main.rand.NextBool(6))
                {
                    int index = GhostOutlineTarget.GetScreenTargetIndex(Main.player[Main.npc[zombieNPCOwner].GetGlobalNPC<NecromancyNPC>().zombieOwner], renderLayer);
                    var color = new Color(50, 150, 255, 100);
                    if (AequusEffects.necromancyRenderers.Length > index && AequusEffects.necromancyRenderers[index] != null)
                    {
                        color = AequusEffects.necromancyRenderers[index].DrawColor();
                        color.A = 100;
                    }

                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), newColor: color);
                    d.velocity *= 0.3f;
                    d.velocity += projectile.velocity * 0.2f;
                    d.scale *= projectile.scale;
                    d.noGravity = true;
                }
            }
            NecromancyNPC.AI_IsZombie = false;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (isZombie)
            {
                float multiplier = NecromancyNPC.GetDamageMultiplier(Main.npc[zombieNPCOwner], projectile.damage);
                if (Main.masterMode)
                {
                    multiplier *= 3f;
                }
                else if (Main.expertMode)
                {
                    multiplier *= 2f;
                }
                damage = (int)(damage * multiplier);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (isZombie)
            {
                target.AddBuff(ModContent.BuffType<SoulStolen>(), 300);
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter writer)
        {
            bitWriter.WriteBit(isZombie);
            if (isZombie)
            {
                writer.Write(zombieNPCOwner);
                writer.Write(zombieDebuffTier);
                writer.Write((byte)renderLayer);
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader reader)
        {
            if (bitReader.ReadBit())
            {
                isZombie = true;
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.DamageType = Aequus.NecromancyDamage;
                if (!projectile.usesLocalNPCImmunity)
                {
                    projectile.usesIDStaticNPCImmunity = true;
                    projectile.idStaticNPCHitCooldown = 10;
                }
                zombieNPCOwner = reader.ReadInt32();
                zombieDebuffTier = reader.ReadSingle();
                renderLayer = reader.ReadByte();
            }
        }
    }
}