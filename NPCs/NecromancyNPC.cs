using Aequus.Buffs.Debuffs;
using Aequus.Common.Catalogues;
using Aequus.Common.Networking;
using Aequus.Graphics;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class NecromancyNPC : GlobalNPC, IEntityNetworker
    {
        public static bool AI_IsZombie { get; set; }
        public static int AI_ZombiePlayerOwner { get; set; }
        public static int AI_NPCTarget { get; set; }
        public static Vector2 AI_ReturnPlayerLocation { get; set; }

        public int zombieDrain;
        public bool isZombie;
        public int zombieOwner;
        public int zombieTimer;
        public int zombieTimerMax;
        public float zombieDebuffTier;
        public int hitCheckDelay;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            On.Terraria.NPC.Transform += NPC_Transform;
            On.Terraria.NPC.SetTargetTrackingValues += NPC_SetTargetTrackingValues;
        }

        private static void NPC_Transform(On.Terraria.NPC.orig_Transform orig, NPC self, int newType)
        {
            bool isZombieOld = self.GetGlobalNPC<NecromancyNPC>().isZombie;
            int owner = -1;
            int timer = -1;
            float tier = -1f;
            if (isZombieOld)
            {
                var zombie = self.GetGlobalNPC<NecromancyNPC>();
                owner = zombie.zombieOwner;
                timer = zombie.zombieTimer;
                tier = zombie.zombieDebuffTier;
            }

            orig(self, newType);

            if (isZombieOld)
            {
                var zombie = self.GetGlobalNPC<NecromancyNPC>();
                zombie.isZombie = true;
                zombie.zombieOwner = owner;
                zombie.zombieTimer = timer;
                zombie.zombieDebuffTier = tier;
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

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!AI_IsZombie)
            {
                return;
            }
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
            if (entity is NPC npc2 && npc2.GetGlobalNPC<NecromancyNPC>().isZombie)
            {
                npc.boss = false;
                npc.friendly = true;
                npc.damage *= 5;
                player = npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = npc2.GetGlobalNPC<NecromancyNPC>().zombieOwner;
                npc.GetGlobalNPC<NecromancyNPC>().zombieTimer = npc2.GetGlobalNPC<NecromancyNPC>().zombieTimer;
                npc.GetGlobalNPC<NecromancyNPC>().zombieTimerMax = npc2.GetGlobalNPC<NecromancyNPC>().zombieTimerMax;
                tier = npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = npc2.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier;
                npc.GetGlobalNPC<NecromancyNPC>().isZombie = true;
                sendPacket = true;
            }
            else if (entity is Projectile proj && proj.GetGlobalProjectile<NecromancyProj>().isZombie)
            {
                npc.boss = false;
                npc.friendly = true;
                npc.damage *= 5;
                player = npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = proj.owner;
                npc.GetGlobalNPC<NecromancyNPC>().zombieTimer = Main.npc[proj.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner].GetGlobalNPC<NecromancyNPC>().zombieTimer;
                npc.GetGlobalNPC<NecromancyNPC>().zombieTimerMax = Main.npc[proj.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner].GetGlobalNPC<NecromancyNPC>().zombieTimerMax;
                tier = npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = proj.GetGlobalProjectile<NecromancyProj>().zombieDebuffTier;
                npc.GetGlobalNPC<NecromancyNPC>().isZombie = true;
                sendPacket = true;
            }

            if (sendPacket)
            {
                PacketSender.SyncNecromancyOwnerTier(npc.whoAmI, player, tier);
            }
        }

        public override bool? CanHitNPC(NPC npc, NPC target)
        {
            return isZombie ? (target.CanBeChasedBy(npc) ? true : null) : null;
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
                if (zombieTimer == 0)
                {
                    zombieTimerMax = zombieTimer = Main.player[zombieOwner].Aequus().necromancyTime;
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
                npc.target = zombieOwner;
                npc.alpha = Math.Max(npc.alpha, 60);
                npc.dontTakeDamage = true;
                npc.npcSlots = 0f;
                int npcTarget = GetNPCTarget(npc, npc.type);

                if (npcTarget != -1)
                {
                    AI_ReturnPlayerLocation = Main.player[zombieOwner].position;
                    AI_NPCTarget = npcTarget;
                    Main.player[zombieOwner].Center = Main.npc[npcTarget].Center;

                    if (hitCheckDelay <= 0)
                    {
                        hitCheckDelay = 4;
                        try
                        {
                            if (Main.myPlayer == zombieOwner)
                            {
                                ZombieHurtNPCsCheck(npc);
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
            }
            return true;
        }
        public bool ShouldDespawnZombie(NPC npc)
        {
            return zombieTimer <= 0 || !Main.player[zombieOwner].active || Main.player[zombieOwner].dead;
        }
        public void ZombieHurtNPCsCheck(NPC npc)
        {
            var myRect = npc.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var n = Main.npc[i];
                if (n.dontTakeDamage || n.dontTakeDamageFromHostiles || n.immortal || n.immune[255] > 0 || n.friendly)
                {
                    continue;
                }

                bool? modCanHit = NPCLoader.CanHitNPC(npc, Main.npc[i]);
                if (modCanHit != false && myRect.Intersects(n.getRect()) && npc.type != NPCID.Gnome)
                {
                    ZombieHurtNPC(npc, n);
                }
            }
        }
        public void ZombieHurtNPC(NPC npc, NPC target)
        {
            int immuneTime = 30;
            int damage = Main.DamageVar(GetNPCDamage(npc, target));
            float knockBack = 6f;
            int hitDirection = (!(npc.Center.X > target.Center.X)) ? 1 : (-1);
            bool crit = false;
            NPCLoader.ModifyHitNPC(npc, target, ref damage, ref knockBack, ref crit);
            Main.player[npc.GetGlobalNPC<NecromancyNPC>().zombieOwner].ApplyDamageToNPC(target, damage, knockBack, hitDirection, crit);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, target.whoAmI, damage, knockBack, hitDirection);
            }
            target.netUpdate = true;
            target.immune[255] = immuneTime;
            NPCLoader.OnHitNPC(npc, target, damage, knockBack, crit);
        }
        public int GetNPCDamage(NPC npc, NPC target)
        {
            double damage = ContentSamples.NpcsByNetId[npc.netID].damage;
            if (Main.masterMode)
            {
                damage /= 3f;
            }
            else if (Main.expertMode)
            {
                damage /= 2f;
            }
            return (int)damage * GetDamageMultiplier(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (isZombie)
            {
                npc.dontTakeDamage = true;
                if (AI_ReturnPlayerLocation != Vector2.Zero)
                {
                    Main.player[zombieOwner].position = AI_ReturnPlayerLocation;
                    AI_ReturnPlayerLocation = Vector2.Zero;
                }
                var aequus = Main.player[zombieOwner].GetModPlayer<AequusPlayer>();
                aequus.necromancySlotUsed++;
                if (Main.rand.NextBool(6))
                {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), newColor: new Color(50, 150, 255, 100));
                    d.velocity *= 0.3f;
                    d.velocity += npc.velocity * 0.2f;
                    d.scale *= npc.scale;
                    d.noGravity = true;
                }
            }
            AI_IsZombie = false;
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

        public override void OnKill(NPC npc)
        {
            if (zombieDrain > 0 && CanBeTurnedIntoZombie(npc))
            {
                SpawnZombie(npc);
            }
        }   
        public bool CanBeTurnedIntoZombie(NPC npc)
        {
            if (npc.type == NPCID.DungeonGuardian || npc.SpawnedFromStatue)
            {
                return false;
            }
            return NecromancyTypes.NPCs.GetOrDefault(npc.type, NecromancyTypes.NecroStats.None).PowerNeeded <= zombieDebuffTier;
        }
        public void SpawnZombie(NPC npc)
        {
            int n = NPC.NewNPC(npc.GetSource_Death("Aequus:Zombie"), (int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height / 2, npc.netID, npc.whoAmI + 1);
            if (n < 200)
            {
                Main.npc[n].GetGlobalNPC<NecromancyNPC>().isZombie = true;
                Main.npc[n].GetGlobalNPC<NecromancyNPC>().zombieOwner = zombieOwner;
                Main.npc[n].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = zombieDebuffTier;
                Main.npc[n].Center = npc.Center;
                Main.npc[n].velocity = npc.velocity * 0.25f;
                Main.npc[n].direction = npc.direction;
                Main.npc[n].spriteDirection = npc.spriteDirection;
                Main.npc[n].friendly = true;
                Main.npc[n].extraValue = 0;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }
        }

        public static int GetNPCTarget(Entity entity, int npcType)
        {
            int target = -1;
            float distance = NecromancyTypes.NPCs.GetOrDefault(npcType, NecromancyTypes.NecroStats.None).ViewDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(entity) &&
                    !NPCID.Sets.CountsAsCritter[Main.npc[i].type])
                {
                    float c = entity.Distance(Main.npc[i].Center);
                    if (c < distance)
                    {
                        target = i;
                        distance = c;
                    }
                }
            }
            return target;
        }

        public static int GetDamageMultiplier(NPC npc)
        {
            int dmgMultiplier = 1;
            if (npc.boss)
            {
                dmgMultiplier = 5;
            }
            dmgMultiplier += npc.lifeMax / 2500;
            return dmgMultiplier;
        }

        internal static void SetupBuffImmunities()
        {
            var buffList = new List<int>(NecromancyTypes.NecromancyDebuffs);
            buffList.Remove(ModContent.BuffType<EnthrallingDebuff>());
            for (int i = 0; i < Main.maxNPCTypes; i++)
            {
                if (!NecromancyTypes.NPCs.TryGetValue(i, out var stats) || stats == NecromancyTypes.NecroStats.None)
                {
                    if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(i, out var value))
                    {
                        NPCID.Sets.DebuffImmunitySets.Add(i, new NPCDebuffImmunityData() { SpecificallyImmuneTo = buffList.ToArray() });
                        continue;
                    }
                    if (value == null)
                    {
                        value = NPCID.Sets.DebuffImmunitySets[i] = new NPCDebuffImmunityData();
                    }
                    if (value.SpecificallyImmuneTo == null)
                    {
                        value.SpecificallyImmuneTo = buffList.ToArray();
                        continue;
                    }
                    Array.Resize(ref value.SpecificallyImmuneTo, value.SpecificallyImmuneTo.Length + buffList.Count);
                    int k = 0;
                    for (int j = value.SpecificallyImmuneTo.Length - buffList.Count; j < value.SpecificallyImmuneTo.Length; j++)
                    {
                        value.SpecificallyImmuneTo[j] = buffList[k];
                        k++;
                    }
                }
            }
        }

        void IEntityNetworker.Send(int whoAmI, BinaryWriter writer)
        {
            writer.Write(Main.npc[whoAmI].active);
            if (Main.npc[whoAmI].active)
            {
                writer.Write(isZombie);
                if (isZombie)
                {
                    writer.Write(zombieTimer);
                    writer.Write(zombieTimerMax);
                }
                else
                {
                    writer.Write(zombieDrain);
                }
                writer.Write(zombieOwner);
                writer.Write(zombieDebuffTier);
            }
        }

        void IEntityNetworker.Receive(int whoAmI, BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                if (reader.ReadBoolean())
                {
                    isZombie = true;
                    zombieTimer = reader.ReadInt32();
                    zombieTimerMax = reader.ReadInt32();
                }
                else
                {
                    zombieDrain = reader.ReadInt32();
                }
                zombieOwner = reader.ReadInt32();
                zombieDebuffTier = reader.ReadSingle();
                Main.NewText(Lang.GetNPCName(Main.npc[whoAmI].type) + ", WhoAmI: " + whoAmI + ", Tier: " + zombieDebuffTier, Main.DiscoColor);
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (isZombie && !NecromancyScreenTarget.RenderingNow && !npc.IsABestiaryIconDummy && npc.lifeMax > 1 && !NPCID.Sets.ProjectileNPC[npc.type])
            {
                NecromancyScreenTarget.Add(npc.whoAmI);
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

            //if (scaleX < 34)
            //{
            //    if (scaleX < 36)
            //    {
            //        spriteBatch.Draw(TextureAssets.Hb2.Value, new Vector2(x - screenPos.X + (float)y * scale, y - screenPos.Y), (Rectangle?)new Rectangle(2, 0, 2, TextureAssets.Hb2.Height()), color, 0f, new Vector2(0f, 0f), scale, (SpriteEffects)0, 0f);
            //    }
            //    if (scaleX < 34)
            //    {
            //        spriteBatch.Draw(TextureAssets.Hb2.Value, new Vector2(x - screenPos.X + (float)(y + 2) * scale, y - screenPos.Y), (Rectangle?)new Rectangle(scaleX + 2, 0, 36 - scaleX - 2, TextureAssets.Hb2.Height()), color, 0f, new Vector2(0f, 0f), scale, (SpriteEffects)0, 0f);
            //    }
            //    if (scaleX > 2)
            //    {
            //        spriteBatch.Draw(TextureAssets.Hb1.Value, new Vector2(x - screenPos.X, y - screenPos.Y), (Rectangle?)new Rectangle(0, 0, scaleX - 2, TextureAssets.Hb1.Height()), color, 0f, new Vector2(0f, 0f), scale, (SpriteEffects)0, 0f);
            //    }
            //    spriteBatch.Draw(TextureAssets.Hb1.Value, new Vector2(x - screenPos.X + (float)(scaleX - 2) * scale, y - screenPos.Y), (Rectangle?)new Rectangle(32, 0, 2, TextureAssets.Hb1.Height()), color, 0f, new Vector2(0f, 0f), scale, (SpriteEffects)0, 0f);
            //}
            //else
            //{
            //    if (scaleX < 36)
            //    {
            //        spriteBatch.Draw(TextureAssets.Hb2.Value, new Vector2(x - screenPos.X + (float)scaleX * scale, y - screenPos.Y), (Rectangle?)new Rectangle(scaleX, 0, 36 - scaleX, TextureAssets.Hb2.Height()), color, 0f, new Vector2(0f, 0f), scale, (SpriteEffects)0, 0f);
            //    }
            //    spriteBatch.Draw(TextureAssets.Hb1.Value, new Vector2(x - screenPos.X, y - screenPos.Y), (Rectangle?)new Rectangle(0, 0, scaleX, TextureAssets.Hb1.Value.Height), color, 0f, new Vector2(0f, 0f), scale, (SpriteEffects)0, 0f);
            //}
        }
        public Color DetermineHealthbarColor(NPC npc, float lifeRatio)
        {
            return Color.Lerp(Color.Cyan, Color.Blue, 1f - lifeRatio);
        }
    }
    public class NecromancyProj : GlobalProjectile, IEntityNetworker
    {
        public bool isZombie;
        public int zombieNPCOwner;
        public float zombieDebuffTier;
        private int netUpdate;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
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
            if (!NecromancyNPC.AI_IsZombie)
            {
                return;
            }
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
        }
        public void ZombieCheck(Entity entity, Projectile projectile)
        {
            if (entity is Projectile proj && proj.GetGlobalProjectile<NecromancyProj>().isZombie)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.owner = NecromancyNPC.AI_ZombiePlayerOwner;
                isZombie = true;
                zombieNPCOwner = proj.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner;
                zombieDebuffTier = proj.GetGlobalProjectile<NecromancyProj>().zombieDebuffTier;
                projectile.timeLeft = Math.Min(projectile.timeLeft, proj.timeLeft);
            }
            else if (entity is NPC npc && npc.GetGlobalNPC<NecromancyNPC>().isZombie)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.owner = NecromancyNPC.AI_ZombiePlayerOwner;
                isZombie = true;
                zombieNPCOwner = entity.whoAmI;
                zombieDebuffTier = npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier;
                projectile.timeLeft = Math.Min(projectile.timeLeft, npc.GetGlobalNPC<NecromancyNPC>().zombieTimer);
            }
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
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    if (netUpdate <= 0)
                    {
                        PacketSender.SendNecromancyProjectile(-1, -1, projectile.identity);
                        netUpdate = 120 + projectile.netSpam * 5;
                    }
                    else
                    {
                        netUpdate--;
                    }
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
                int npcTarget = NecromancyNPC.GetNPCTarget(projectile, zombieNPCOwner);

                if (npcTarget != -1)
                {
                    NecromancyNPC.AI_ReturnPlayerLocation = Main.player[projectile.owner].position;
                    NecromancyNPC.AI_NPCTarget = npcTarget;
                    Main.player[projectile.owner].Center = Main.npc[npcTarget].Center;
                }
            }
            return true;
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
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(50, 150, 255, 100));
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
                damage *= NecromancyNPC.GetDamageMultiplier(Main.npc[zombieNPCOwner]);
            }
        }

        void IEntityNetworker.Send(int whoAmI, BinaryWriter writer)
        {
            writer.Write(isZombie);
            if (isZombie)
            {
                writer.Write(zombieNPCOwner);
                writer.Write(zombieDebuffTier);
            }
        }

        void IEntityNetworker.Receive(int whoAmI, BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                isZombie = true;
                zombieNPCOwner = reader.ReadInt32();
                zombieDebuffTier = reader.ReadSingle();
            }
        }
    }
}