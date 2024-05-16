using Aequus.Common.NPCs;
using Aequus.Content.Dusts;
using Aequus.Core.Initialization;
using Aequus.DataSets;
using Aequus.Old.Content.Necromancy.Networking;
using Aequus.Old.Content.Necromancy.Rendering;
using Aequus.Old.Content.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Old.Content.Necromancy;

public class NecromancyNPC : GlobalNPC, IAddRecipes {
    public class ActiveZombieInfo {
        public int PlayerOwner;
        public int NPCTarget;

        public bool IsZombieRunning => PlayerOwner > -1;

        public ActiveZombieInfo() {
            PlayerOwner = -1;
            NPCTarget = -1;
        }

        public void Reset() {
            PlayerOwner = -1;
            NPCTarget = -1;
        }
    }

    public static byte CheckZombies;

    public static ActiveZombieInfo Zombie { get; private set; }
    public static PlayerTargetHack TargetHack { get; set; }

    public int ghostDebuffDOT;
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
    public bool statFreezeLifespan;
    public int shadowDashTimer;
    public int ghostChainsNPC;
    public int ghostChainsTime;
    public bool hasSupportEffects;

    public int netUpdateTimer;

    public float ZombieLifespanPercentage => (float)(zombieTimer / (float)zombieTimerMax);

    public override bool InstancePerEntity => true;

    public NecromancyNPC() {
        ghostChainsNPC = -1;
    }

    public override void Load() {
        Zombie = new ActiveZombieInfo();
        Terraria.On_NPC.SetTargetTrackingValues += NPC_SetTargetTrackingValues;
        Terraria.On_NPC.FindFirstNPC += NPC_FindFirstNPC;
        Terraria.On_NPC.AnyNPCs += NPC_AnyNPCs;
        Terraria.On_NPC.CountNPCS += NPC_CountNPCS;
    }

    private static int NPC_FindFirstNPC(Terraria.On_NPC.orig_FindFirstNPC orig, int Type) {
        if (CheckZombies == 0) {
            return orig(Type);
        }
        var npcs = new List<NPC>();
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].type == Type && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie)) {
                if (zombie.isZombie == !Zombie.IsZombieRunning) {
                    npcs.Add(Main.npc[i]);
                    Main.npc[i].active = false;
                }
                else {
                    break;
                }
            }
        }
        int val = orig(Type);
        foreach (var npc in npcs) {
            npc.active = true;
        }
        return val;
    }

    private static bool NPC_AnyNPCs(Terraria.On_NPC.orig_AnyNPCs orig, int Type) {
        if (CheckZombies == 0) {
            return orig(Type);
        }
        var npcs = new List<NPC>();
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].type == Type && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie)) {
                if (zombie.isZombie == !Zombie.IsZombieRunning) {
                    npcs.Add(Main.npc[i]);
                    Main.npc[i].active = false;
                }
                else {
                    break;
                }
            }
        }
        bool val = orig(Type);
        foreach (var npc in npcs) {
            npc.active = true;
        }
        return val;
    }

    private static int NPC_CountNPCS(Terraria.On_NPC.orig_CountNPCS orig, int Type) {
        if (CheckZombies == 0) {
            return orig(Type);
        }
        var npcs = new List<NPC>();
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].type == Type && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie == !Zombie.IsZombieRunning) {
                npcs.Add(Main.npc[i]);
                Main.npc[i].active = false;
            }
        }
        int amt = orig(Type);
        foreach (var npc in npcs) {
            npc.active = true;
        }
        return amt;
    }

    #region Hooks
    private static void NPC_SetTargetTrackingValues(Terraria.On_NPC.orig_SetTargetTrackingValues orig, NPC self, bool faceTarget, float realDist, int tankTarget) {
        if (Zombie.IsZombieRunning) {
            self.target = self.GetGlobalNPC<NecromancyNPC>().zombieOwner;
            if (Zombie.NPCTarget != -1) {
                self.targetRect = Main.npc[Zombie.NPCTarget].getRect();
            }
            else {
                self.targetRect = Main.player[self.target].getRect();
            }

            if (faceTarget) {
                self.direction = self.targetRect.X + self.targetRect.Width / 2 < self.position.X + self.width / 2 ? -1 : 1;
                self.directionY = self.targetRect.Y + self.targetRect.Height / 2 < self.position.Y + self.height / 2 ? -1 : 1;
            }

            return;
        }
        orig(self, faceTarget, realDist, tankTarget);
    }
    #endregion

    public override void SetDefaults(NPC npc) {
        ghostChainsNPC = -1;
    }

    public static void InheritFromParent(NPC npc, Entity ent) {
        NPC parentNPC = null;
        if (ent is NPC) {
            parentNPC = (NPC)ent;
        }
        if (ent is Projectile parentProjectile && parentProjectile.GetGlobalProjectile<NecromancyProj>().isZombie) {
            parentNPC = Main.npc[parentProjectile.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner];
        }

        if (parentNPC != null && parentNPC.TryGetGlobalNPC<NecromancyNPC>(out var parentZombie) && npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie)) {
            var info = GhostSyncInfo.GetInfo(parentNPC);
            if (parentZombie.isZombie) {
                info.SetZombieNPCInfo(npc, zombie);
                zombie.ApplyStaticStats(npc);
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    ExtendedMod.GetPacket<SyncNecromancyOwnerPacket>().Send(npc.whoAmI, info.Player);
                }
            }
        }
    }
    public override void OnSpawn(NPC npc, IEntitySource source) {
        if (source is EntitySource_Parent parentSource) {
            InheritFromParent(npc, parentSource.Entity);
        }
    }

    public override Color? GetAlpha(NPC npc, Color drawColor) {
        if (isZombie && Main.netMode != NetmodeID.Server) {
            var color = GhostRenderer.GetColorTarget(Main.player[zombieOwner], renderLayer).getDrawColor() with { A = 100 };

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

    public override void ResetEffects(NPC npc) {
        statFreezeLifespan = false;
        conversionChance = 0;
        if (ghostDebuffDOT > 0) {
            ghostDebuffDOT--;
        }
        if (isZombie) {
            npc.DelBuff(0);
        }
        else {
            zombieDebuffTier = 0f;
        }

        if (ghostChainsTime > 0) {
            ghostChainsTime--;
            if (ghostChainsTime <= 0 || !Main.npc[ghostChainsNPC].active || Main.npc[ghostChainsNPC].friendly || Main.npc[ghostChainsNPC].dontTakeDamage) {
                ghostChainsTime = 0;
                ghostChainsNPC = -1;
            }
        }
    }

    public override bool PreAI(NPC npc) {
        CheckZombies = 10;
        Zombie.Reset();
        if (isZombie) {
            var player = Main.player[zombieOwner];
            var aequus = player.GetModPlayer<AequusPlayer>();
            if (ghostDamage > 0) {
                npc.defDamage = ghostDamage;
                npc.damage = ghostDamage;
            }
            if (zombieTimer == 0) {
                int time = (int)Main.player[zombieOwner].GetModPlayer<AequusPlayer>().ghostLifespan.ApplyTo(NecromancySystem.DefaultLifespan);

                zombieTimerMax = time;
                zombieTimer = time;
            }
            AequusNPC aequusNPC = npc.GetGlobalNPC<AequusNPC>();
            aequusNPC.statSpeedX += ghostSpeed;
            aequusNPC.statSpeedY += ghostSpeed;
            aequus.ghostSlots += slotsConsumed;
            if (aequus.gravetenderGhost == npc.whoAmI) {
                aequusNPC.statSpeedX *= 1.33f;
                aequusNPC.statSpeedY *= 1.33f;
                statFreezeLifespan = true;
            }
            if (!statFreezeLifespan) {
                zombieTimer--;
            }

            if (ShouldDespawnZombie(npc)) {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
            }
            else if (statFreezeLifespan) {
                npc.life = npc.lifeMax;
            }
            else {
                npc.life = (int)Math.Clamp(npc.lifeMax * (zombieTimer / (float)zombieTimerMax), 1f, npc.lifeMax); // Aggros slimes and stuff
            }

            RestoreTarget();

            Zombie.PlayerOwner = zombieOwner;

            npc.GivenName = Main.player[zombieOwner].name + "'s " + Lang.GetNPCName(npc.netID);
            npc.friendly = true;
            npc.boss = false;
            npc.alpha = Math.Max(npc.alpha, 60);
            npc.dontTakeDamage = true;
            npc.SpawnedFromStatue = true;
            npc.npcSlots = 0f;
            if (!Main.player[npc.target].active || Main.player[npc.target].dead || !Main.player[npc.target].hostile || Main.player[npc.target].team == Main.player[zombieOwner].team) {
                float prioritizeMultiplier = npc.noGravity ? 2f : 1f;
                int npcTarget = GetNPCTarget(npc, Main.player[zombieOwner], npc.netID, npc.type, prioritizeMultiplier);

                if (npcTarget != -1) {
                    TargetHack = new PlayerTargetHack(npc, Main.npc[npcTarget], Main.player[zombieOwner], Main.npc[npcTarget].Center);
                    TargetHack.Move();
                    Zombie.NPCTarget = npcTarget;
                }
                UpdateHitbox(npc);
            }
        }
        return true;
    }
    public bool ShouldDespawnZombie(NPC npc) {
        return zombieTimer <= 0 || !Main.player[zombieOwner].active || Main.player[zombieOwner].dead;
    }
    public void UpdateHitbox(NPC npc) {
        if (hitCheckDelay <= 0) {
            hitCheckDelay = 30;
            try {
                if (Main.myPlayer == zombieOwner) {
                    Zombie.PlayerOwner = -1;
                    try {
                        for (int i = 0; i < Main.maxProjectiles; i++) {
                            if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<GhostHitbox>() && Main.projectile[i].TryGetGlobalProjectile<NecromancyProj>(out var zombie)) {
                                if (zombie.zombieNPCOwner == npc.whoAmI) {
                                    hitCheckDelay = 120;
                                    return;
                                }
                            }
                        }
                        int damage = ghostDamage > 0 ? ghostDamage : (int)(npc.damage * GetDamageMultiplier(npc, npc.damage));
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, Vector2.Normalize(npc.velocity) * 0.01f, ModContent.ProjectileType<GhostHitbox>(), damage, 3f, zombieOwner, npc.whoAmI, 64f);
                        Main.projectile[p].originalDamage = damage;
                    }
                    catch {
                    }
                    Zombie.PlayerOwner = zombieOwner;
                }
            }
            catch {

            }
        }
        else {
            hitCheckDelay--;
        }
    }

    public override void PostAI(NPC npc) {
        if (isZombie) {
            if (ghostDamage > 0) {
                npc.defDamage = ghostDamage;
                npc.damage = ghostDamage;
            }
            var player = Main.player[zombieOwner];
            var aequus = player.GetModPlayer<AequusPlayer>();

            if (aequus.ghostShadowDash > 0 && Zombie != null && Zombie.NPCTarget > -1 && Zombie.NPCTarget == player.MinionAttackTargetNPC) {
                shadowDashTimer++;
                if (shadowDashTimer < 200) {
                    if (Math.Sign(npc.velocity.X) == Math.Sign(Main.npc[Zombie.NPCTarget].Center.X - npc.Center.X)) {
                        shadowDashTimer += 20;
                    }
                    if (Math.Sign(npc.velocity.Y) == Math.Sign(Main.npc[Zombie.NPCTarget].Center.Y - npc.Center.Y)) {
                        shadowDashTimer++;
                    }
                }
                if (shadowDashTimer > 240) {
                    shadowDashTimer = 0;
                }
            }
            else if (shadowDashTimer > 0) {
                shadowDashTimer--;
            }

            npc.target = zombieOwner;
            npc.dontTakeDamage = true;
            RestoreTarget();
            if (ghostChainsNPC > 0 && Main.npc[ghostChainsNPC].active) {
                var npcCenter = Main.npc[ghostChainsNPC].Center;
                float minDistance = npc.noGravity ? 200f : 100f;
                float distance = npc.Distance(npcCenter);
                if (distance > minDistance) {
                    var v = npc.DirectionTo(npcCenter) * ((distance - minDistance / 2f) / minDistance);
                    npc.velocity += v * 0.4f;
                    npc.position += v;
                    npc.netUpdate = true;
                }
            }

            if (Main.netMode != NetmodeID.Server) {
                if (Main.rand.NextBool(6)) {
                    var color = GhostRenderer.GetColorTarget(Main.player[zombieOwner], renderLayer).getDrawColor() with { A = 100 };
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), newColor: color);
                    d.velocity *= 0.2f;
                    d.velocity += npc.velocity * 0.4f;
                    d.scale *= npc.scale;
                    d.noGravity = true;
                }
                if (aequus.gravetenderGhost == npc.whoAmI && Main.rand.NextBool(6)) {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.SilverFlame, newColor: new Color(200, 50, 128, 25));
                    d.velocity *= 0.5f;
                    d.velocity += -npc.velocity * 0.2f;
                    d.scale *= npc.scale;
                    d.fadeIn = d.scale + 0.5f;
                    d.noGravity = true;
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
            }
            else {
                netUpdateTimer--;
                if (netUpdateTimer <= -10 || npc.netUpdate) {
                    npc.netUpdate = true;
                    netUpdateTimer = 30 + npc.netSpam * 5;
                    Sync(npc);
                    npc.netSpam++;
                }
            }
        }
        else if (ghostDebuffDOT > 0 && zombieOwner == Main.myPlayer) {
            if (Main.netMode != NetmodeID.Server && (Main.GameUpdateCount % 14 == 0 || Main.rand.NextBool(14))) {
                if (CheckCanConvertIntoGhost(npc) && CanSpawnZombie(npc, justChecking: true)) {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<GhostDrainDust>(), newColor: GhostRenderer.GetColorTarget(Main.player[zombieOwner], renderLayer).getDrawColor() with { A = 25 } * 0.8f, Scale: Main.rand.NextFloat(0.75f, 1f));
                    d.customData = Main.player[zombieOwner];
                }
            }
        }
        else {
            renderLayer = 0;
        }

        Zombie.Reset();
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage) {
        if (ghostDebuffDOT > 0) {
            float multiplier = 1f;
            if (zombieOwner > 0 && zombieOwner < Main.maxPlayers && Main.player[zombieOwner].active) {
                multiplier += Main.player[zombieOwner].GetModPlayer<AequusPlayer>().zombieDebuffMultiplier;
            }

            if (npc.lifeRegen > 0) {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen -= (int)(ghostDebuffDOT * multiplier);
            damage = Math.Max(damage, ghostDebuffDOT / 20);
        }
    }

    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (isZombie && !npc.IsABestiaryIconDummy && npc.lifeMax > 1 && !NPCSets.ProjectileNPC[npc.type]) {
            if (ghostChainsNPC > 0) {
                GhostRenderer.ChainedUpNPCs.Add((npc, Main.npc[ghostChainsNPC]));
            }
            if (!GhostRenderer.Rendering) {
                GhostRenderer.Requested = true;
                ModContent.GetInstance<NecromancyInterface>().Activate();
                GhostRenderer.GetColorTarget(Main.player[zombieOwner], renderLayer).NPCs.Add(npc);
            }
        }
        return true;
    }

    public void DrawHealthbar(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos) {
        if (Main.HealthBarDrawSettings == 0 || npc.life < 0) {
            return;
        }
        float y = 0f;
        if (Main.HealthBarDrawSettings == 1) {
            y += Main.NPCAddHeight(npc) + npc.height;
        }
        else if (Main.HealthBarDrawSettings == 2) {
            y -= Main.NPCAddHeight(npc) / 2f;
        }
        var center = npc.Center;
        var zombie = npc.GetGlobalNPC<NecromancyNPC>();
        InnerDrawHealthbar(npc, spriteBatch, screenPos, center.X, npc.position.Y + y + npc.gfxOffY, zombie.zombieTimer, zombie.zombieTimerMax, Lighting.Brightness((int)(center.X / 16f), (int)((center.Y + npc.gfxOffY) / 16f)), 1f);
    }
    public void InnerDrawHealthbar(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, float x, float y, int life, int maxLife, float alpha, float scale) {
        var hb = TextureAssets.Hb1.Value;
        var hbBack = TextureAssets.Hb2.Value;

        float lifeRatio = MathHelper.Clamp(life / (float)maxLife, 0f, 1f);
        int scaleX = (int)MathHelper.Clamp(hb.Width * lifeRatio, 2f, hb.Width - 2f);

        x -= hb.Width / 2f * scale;
        y += hb.Height; //I kind of like how they're lower than the vanilla hb spots
        if (Main.LocalPlayer.gravDir == -1f) {
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
    public Color DetermineHealthbarColor(NPC npc, float lifeRatio) {
        int team = Main.player[zombieOwner].team;
        var color = GhostRenderer.GetColorTarget(Main.player[zombieOwner], renderLayer).getDrawColor() with { A = 100 };
        return Color.Lerp(color, (color * 0.5f) with { A = 255 }, 1f - lifeRatio);
    }

    public override void OnKill(NPC npc) {
        if (npc.SpawnedFromStatue || npc.friendly || npc.lifeMax < 5) {
            return;
        }

        CreateGhost(npc);
    }

    public bool CheckCanConvertIntoGhost(NPC npc) {
        if (npc.boss || NPCDataSet.Unfriendable.Contains(npc.netID) || zombieOwner < 0 || zombieOwner > Main.maxPlayers || !Main.player[zombieOwner].active || Main.player[zombieOwner].dead || Main.player[zombieOwner].ghost) {
            return false;
        }

        if (conversionChance > 0 && Main.rand.NextBool(conversionChance)) {
            return true;
        }

        return zombieDebuffTier > 0;
    }

    public void CreateGhost(NPC npc) {
        if (CheckCanConvertIntoGhost(npc)) {
            SpawnZombie(npc);
        }
    }

    public bool MakeRoomForMe(NPC npc, out int killMinion) {
        float lowestPriority = float.MaxValue;
        killMinion = -1;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie && zombie.zombieOwner == zombieOwner && zombie.slotsConsumed == 1) {
                int priority = zombie.DespawnPriority(Main.npc[i]);
                if (priority < lowestPriority) {
                    killMinion = i;
                    lowestPriority = priority;
                }
            }
        }
        return killMinion != -1;
    }
    public bool CanSpawnZombie(NPC npc, bool justChecking = true) {
        ApplyStaticStats(npc);
        bool forceSpawn = false;
        var aequus = Main.player[zombieOwner].GetModPlayer<AequusPlayer>();
        if (Main.player[zombieOwner].HasMinionAttackTargetNPC && Main.player[zombieOwner].MinionAttackTargetNPC == npc.whoAmI) {
            if (aequus.ghostSlots + slotsConsumed > aequus.ghostSlotsMax) {
                MakeRoomForMe(npc, out int killMinion);
                if (!justChecking && killMinion != -1) {
                    forceSpawn = true;
                    Main.npc[killMinion].life = -1;
                    Main.npc[killMinion].HitEffect();
                    Main.npc[killMinion].active = false;
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, killMinion, 9999 + Main.npc[killMinion].lifeMax * 2 + Main.npc[killMinion].defense * 2);
                    }
                }
            }
        }
        if (!forceSpawn) {
            int slotsToConsume = slotsConsumed;
            if (aequus.ghostSlotsOld + slotsToConsume > aequus.ghostSlotsMax) {
                int myPriority = DespawnPriority(npc);
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie)
                        && zombie.isZombie && zombie.zombieOwner == zombieOwner && zombie.slotsConsumed > 0) {
                        int priority = zombie.DespawnPriority(Main.npc[i]);
                        if (priority < myPriority) {
                            slotsToConsume -= zombie.slotsConsumed;
                            if (slotsToConsume <= 0)
                                break;
                        }
                    }
                }
                if (slotsToConsume > 0)
                    return false;
            }
        }
        return true;
    }

    public void SpawnZombie(NPC npc) {
        var myZombie = npc.GetGlobalNPC<NecromancyNPC>();
        myZombie.zombieTimer = 1;
        myZombie.zombieTimerMax = 1;
        if (!myZombie.CanSpawnZombie(npc, justChecking: false)) {
            return;
        }
        var myAequus = Main.player[myZombie.zombieOwner].GetModPlayer<AequusPlayer>();
        int n = NPC.NewNPC(new EntitySource_Misc("Aequus:Zombie"), (int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height / 2, npc.netID, npc.whoAmI + 1);
        if (n < 200) {
            Main.npc[n].whoAmI = n;
            SpawnZombie_SetZombieStats(Main.npc[n], npc.Center, npc.velocity, npc.direction, npc.spriteDirection, out bool playSound);
            if (playSound) {
                if (Main.netMode == NetmodeID.Server) {
                    ExtendedMod.GetPacket<ZombieConvertEffectsPacket>().Send(npc, zombieOwner, renderLayer);
                }
                else {
                    ConvertEffects(npc.position, npc.width, npc.height, zombieOwner, renderLayer);
                }
            }
        }
    }
    public static void ConvertEffects(Vector2 position, int width, int height, int player, int renderLayer) {
        var clr = GhostRenderer.GetColorTarget(Main.player[player], renderLayer).getDrawColor() with { A = 25 } * 0.8f;
        for (int i = 0; i < 50; i++) {
            var d = Dust.NewDustDirect(position, width, height, ModContent.DustType<GhostDrainDust>(), newColor: clr.HueAdd(Main.rand.NextFloat(-0.02f, 0.02f)) with { A = 25 }, Scale: Main.rand.NextFloat(0.75f, 1.75f));
            d.customData = Main.player[player];
            d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f);
        }
        SoundEngine.PlaySound(AequusSounds.RecruitZombie, position);
    }
    public void SpawnZombie_SetZombieStats(NPC zombieNPC, Vector2 position, Vector2 velocity, int direction, int spriteDirection, out bool playSound) {
        var zombie = zombieNPC.GetGlobalNPC<NecromancyNPC>();
        zombie.isZombie = true;
        zombie.zombieOwner = zombieOwner;
        zombie.zombieDebuffTier = zombieDebuffTier;
        zombie.zombieTimer = zombie.zombieTimerMax = (int)Main.player[zombieOwner].GetModPlayer<AequusPlayer>().ghostLifespan.ApplyTo(NecromancySystem.DefaultLifespan);
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
        if (zombieNPC.ModNPC != null) {
            zombieNPC.ModNPC.Music = -1;
            zombieNPC.ModNPC.SceneEffectPriority = SceneEffectPriority.None;
        }
        if (Main.netMode == NetmodeID.Server) {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, zombieNPC.whoAmI);
        }
        playSound = true;
    }

    public void ApplyStaticStats(NPC npc) {
        slotsConsumed = 1;
    }

    public void RenderLayer(int layer) {
        if (renderLayer < layer) {
            renderLayer = layer;
        }
    }

    public void DebuffTier(float tier) {
        if (zombieDebuffTier < tier) {
            zombieDebuffTier = tier;
        }
    }

    public void ConversionChance(int amt) {
        if (conversionChance == 0) {
            conversionChance = amt;
        }
        else {
            conversionChance = Math.Min(amt, conversionChance);
        }
    }

    public static int GetNPCTarget(Entity entity, Player player, int netID, int npcType, float prioritizePlayerMultiplier = 1f) {
        int target = -1;
        float distance = 2000f;
        if (distance < 800f) {
            distance = 800f;
        }
        int closestToPlayer = player.GetModPlayer<AequusPlayer>().closestEnemy;
        int minionTarget = -1;
        if (player.HasMinionAttackTargetNPC) {
            minionTarget = player.MinionAttackTargetNPC;
        }
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(entity) &&
                !NPCSets.CountsAsCritter[Main.npc[i].type]) {
                float c = entity.Distance(Main.npc[i].Center);
                if (i == closestToPlayer) {
                    c /= 4f * prioritizePlayerMultiplier;
                }
                if (i == minionTarget) {
                    c /= 12f * prioritizePlayerMultiplier;
                }
                if (c < distance) {
                    target = i;
                    distance = c;
                }
            }
        }
        return target;
    }

    public static float GetDamageMultiplier(NPC npc, int originalDamage) {
        float dmgMultiplier = 1f;
        if (npc.boss) {
            dmgMultiplier += 4;
        }

        float addMultiplier = 0.5f;
        float healthAdditions = npc.lifeMax / (float)(2500 + originalDamage * 5);
        while (healthAdditions > 0f) {
            if (healthAdditions < 1f) {
                addMultiplier *= healthAdditions;
            }
            dmgMultiplier += addMultiplier;
            addMultiplier /= 2f;
            healthAdditions -= 1f;
        }

        return Math.Max(dmgMultiplier / 2f, !Main.expertMode ? 1.33f : 1f);
    }

    public int DespawnPriority(NPC npc) {
        float priority = 10000f;

        if (npc.damage < 10) {
            return npc.damage;
        }

        float priorityMultiplier = zombieTimer / (float)zombieTimerMax;

        float fallOff = 1200f;
        float distance = npc.Distance(Main.player[zombieOwner].Center);

        if (distance > fallOff) {
            priorityMultiplier *= Math.Max((distance - fallOff) / fallOff, 0.1f);
        }

        return (int)(priorityMultiplier * priority);
    }

    public void Send(BinaryWriter writer) {
        var bb = new BitsByte(isZombie, shadowDashTimer > 0, ghostChainsNPC > -1, zombieOwner > -1);
        writer.Write(bb);
        if (isZombie) {
            writer.Write(zombieTimer);
            writer.Write(zombieTimerMax);
            writer.Write(ghostDamage);
            writer.Write(ghostSpeed);
            writer.Write(slotsConsumed);
            if (bb[1]) {
                writer.Write(shadowDashTimer);
            }
            if (bb[2]) {
                writer.Write(ghostChainsNPC);
                writer.Write(ghostChainsTime);
            }
        }
        else {
            writer.Write(ghostDebuffDOT);
        }
        if (bb[3]) {
            writer.Write(zombieOwner);
            writer.Write(zombieDebuffTier);
            writer.Write((byte)renderLayer);
        }
    }

    public void Receive(BinaryReader reader) {
        var bb = (BitsByte)reader.ReadByte();
        if (bb[0]) {
            isZombie = true;
            zombieTimer = reader.ReadInt32();
            zombieTimerMax = reader.ReadInt32();
            ghostDamage = reader.ReadInt32();
            ghostSpeed = reader.ReadSingle();
            slotsConsumed = reader.ReadInt32();
            if (bb[1]) {
                shadowDashTimer = reader.ReadInt32();
            }
            if (bb[2]) {
                ghostChainsNPC = reader.ReadInt32();
                ghostChainsTime = reader.ReadInt32();
            }
        }
        else {
            ghostDebuffDOT = reader.ReadInt32();
        }
        if (bb[3]) {
            zombieOwner = reader.ReadInt32();
            zombieDebuffTier = reader.ReadSingle();
            renderLayer = reader.ReadByte();
        }
    }

    public static void Sync(NPC npc) {
        if (Main.netMode != NetmodeID.SinglePlayer) {
            ExtendedMod.GetPacket<SyncNecromancyNPCPacket>().Send(npc);
        }
    }
    public static void Sync(int npc) {
        Sync(Main.npc[npc]);
    }

    void IAddRecipes.AddRecipes(Mod mod) {
    }

    public static void RestoreTarget() {
        if (TargetHack.HasInfo) {
            TargetHack.Restore();
            TargetHack = PlayerTargetHack.None;
        }
    }
}