using Aequus.Buffs.Debuffs;
using Aequus.Common.Networking;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Weapons.Summon.Candles;
using Aequus.NPCs.Monsters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public sealed class AequusNPC : GlobalNPC
    {
        public static HashSet<int> HeatDamage { get; private set; }
        public override bool InstancePerEntity => true;

        public bool heatDamage;
        public bool noHitEffect;

        public override void Load()
        {
            HeatDamage = new HashSet<int>()
            {
                NPCID.Lavabat,
                NPCID.LavaSlime,
                NPCID.FireImp,
                NPCID.MeteorHead,
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.BlazingWheel,
            };

            On.Terraria.NPC.UpdateCollision += NPC_UpdateCollision;
            On.Terraria.NPC.VanillaHitEffect += Hook_PreHitEffect;
        }
        private static void NPC_UpdateCollision(On.Terraria.NPC.orig_UpdateCollision orig, NPC self)
        {
            float velocityBoost = VelocityBoost(self);

            if (velocityBoost != 0f)
            {
                self.velocity *= 1f + velocityBoost;
            }
            orig(self);
            if (velocityBoost != 0f)
            {
                self.velocity /= 1f + velocityBoost;
            }
        }
        public static float VelocityBoost(NPC npc)
        {
            float velocityBoost = 0f;
            if (npc.TryGetGlobalNPC<NecromancyNPC>(out var z) && z.isZombie)
            {
                velocityBoost += z.DetermineVelocityBoost(npc, Main.player[z.zombieOwner], Main.player[z.zombieOwner].Aequus());
            }
            if (npc.HasBuff<Weakness>())
            {
                velocityBoost -= 0.25f;
            }
            return velocityBoost;
        }
        private static void Hook_PreHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            try
            {
                if (self.TryGetGlobalNPC<AequusNPC>(out var aequus))
                {
                    if (aequus.noHitEffect)
                    {
                        return;
                    }
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    goto Orig;
                }

                if (self.HasBuff<Bleeding>())
                {
                    int amt = (int)Math.Min(4.0 + dmg / 20.0, 20.0);
                    for (int i = 0; i < amt; i++)
                    {
                        bool foodParticle = Main.rand.NextBool();
                        var d = Dust.NewDustDirect(self.position, self.width, self.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                        d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 5f);
                        d.velocity += self.velocity * 0.5f;
                        if (Main.rand.NextBool(3))
                        {
                            d.noGravity = true;
                        }
                    }
                }

                if (self.life <= 0 && self.HasBuff<SnowgraveDebuff>()
                    && SnowgraveCorpse.CanFreezeNPC(self))
                {
                    SoundEngine.PlaySound(SoundID.Item30, self.Center);
                    return;
                }
            }
            catch
            {

            }
        Orig:
            orig(self, hitDirection, dmg);
        }

        public override void SetDefaults(NPC npc)
        {
            if (HeatDamage.Contains(npc.type))
            {
                heatDamage = true;
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.DungeonSpirit && AequusHelpers.HereditarySource(source, out var ent) && ent is NPC parent)
            {
                if (Heckto.Spawnable.Contains(parent.type))
                {
                    npc.Transform(ModContent.NPCType<Heckto>());
                }
            }
        }

        public override void PostAI(NPC npc)
        {
            if (npc.noTileCollide)
            {
                float velocityBoost = VelocityBoost(npc);
                if (velocityBoost > 0f)
                {
                    npc.position += npc.velocity * velocityBoost;
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<Weakness>())
            {
                byte a = drawColor.A;
                drawColor = (drawColor * 0.9f).UseA(a);
                if (npc.life >= 0 && Main.rand.NextBool(20))
                {
                    npc.HitEffect(0, 10);
                }
            }
            if (npc.life >= 0 && npc.HasBuff<Bleeding>())
            {
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool(5))
                        npc.HitEffect(0, 4);
                    bool foodParticle = Main.rand.NextBool();
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                    d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2f);
                    d.velocity += npc.velocity * 0.5f;
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff<Bleeding>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 4;
                damage += 4;
            }
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && npc.HasBuff<SnowgraveDebuff>())
            {
                DeathEffect_SnowgraveFreeze(npc);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient
                || npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
            {
                return false;
            }

            var players = GetCloseEnoughPlayers(npc);
            if (npc.HasBuff<SoulStolen>())
            {
                CheckSouls(players);
            }
            if (NecromancyDatabase.TryGetByNetID(npc, out var info))
            {
                var zombie = npc.GetGlobalNPC<NecromancyNPC>();
                if ((info.PowerNeeded != 0f || zombie.zombieDebuffTier >= 100f) && GhostKill(npc, zombie, info, players))
                {
                    zombie.SpawnZombie(npc);
                }
            }

            return false;
        }
        public void CheckSouls(List<(Player, AequusPlayer, float)> players)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                foreach (var p in players)
                {
                    if (p.Item2.candleSouls < p.Item2.soulCandleLimit)
                    {
                        p.Item2.candleSouls++;
                    }
                }
            }
            else
            {
                List<int> candlePlayers = new List<int>();
                foreach (var p in players)
                {
                    if (p.Item2.candleSouls < p.Item2.soulCandleLimit)
                    {
                        candlePlayers.Add(p.Item1.whoAmI);
                    }
                }

                if (candlePlayers.Count > 0)
                {
                    PacketHandler.Send((p) =>
                    {
                        p.Write(candlePlayers.Count);
                        for (int i = 0; i < candlePlayers.Count; i++)
                        {
                            p.Write(candlePlayers[i]);
                        }
                    }, PacketType.GiveoutEnemySouls);
                }
            }
        }
        public bool GhostKill(NPC npc, NecromancyNPC zombie, GhostInfo info, List<(Player, AequusPlayer, float)> players)
        {
            if (zombie.zombieDrain > 0 && info.PowerNeeded <= zombie.zombieDebuffTier)
            {
                return true;
            }
            if (zombie.conversionChance > 0 && Main.rand.NextBool(zombie.conversionChance))
            {
                return true;
            }
            //for (int i = 0; i < players.Count; i++)
            //{
            //    if (players[i].Aequus().dreamMask && Main.rand.NextBool(4))
            //    {
            //        zombie.zombieOwner = players[i].whoAmI;
            //        zombie.zombieDebuffTier = info.PowerNeeded;
            //        return true;
            //    }
            //}
            return false;
        }
        public void DeathEffect_SnowgraveFreeze(NPC npc)
        {
            if (SnowgraveCorpse.CanFreezeNPC(npc))
            {
                AequusEffects.BehindProjs.Add(new SnowgraveCorpse(npc.Center, npc));
            }
        }
        public List<(Player, AequusPlayer, float)> GetCloseEnoughPlayers(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return new List<(Player, AequusPlayer, float)>() { (Main.LocalPlayer, Main.LocalPlayer.Aequus(), npc.Distance(Main.LocalPlayer.Center)), };
            }
            var list = new List<(Player, AequusPlayer, float)>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    float d = npc.Distance(Main.player[i].Center);
                    if (d < 2000f)
                    {
                        list.Add((Main.player[i], Main.player[i].Aequus(), d));
                    }
                }
            }
            return list;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.Pixie:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PixieCandle>(), 100));
                    break;

                case NPCID.BloodZombie:
                case NPCID.Drippler:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodZombieCandle>(), 100));
                    break;

                case NPCID.DevourerHead:
                case NPCID.GiantWormHead:
                case NPCID.BoneSerpentHead:
                case NPCID.TombCrawlerHead:
                case NPCID.DiggerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.SeekerHead:
                case NPCID.BloodEelHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 25));
                    break;
            }
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.type == NPCID.Angler)
            {
                if (firstButton)
                {
                    var inv = Main.LocalPlayer.inventory;
                    for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                    {
                        if (AequusItem.LegendaryFish.Contains(inv[i].type))
                        {
                            Main.LocalPlayer.Aequus().LegendaryFishRewards(npc, inv[i], i);
                            inv[i].stack--;
                            if (inv[i].stack <= 0)
                            {
                                inv[i].TurnToAir();
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}