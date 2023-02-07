using Aequus.Common;
using Aequus.Common.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Debuff
{
    [AutoloadEquip(EquipType.Waist)]
    public class BlackPhial : ModItem
    {
        public static List<int> DebuffsAfflicted { get; private set; }

        public override void Load()
        {
            DebuffsAfflicted = new List<int>()
            {
                BuffID.Poisoned,
                BuffID.OnFire3,
                BuffID.Frostburn2,
                BuffID.CursedInferno,
                BuffID.Ichor,
                BuffID.ShadowFlame,
            };
        }

        public override void Unload()
        {
            DebuffsAfflicted?.Clear();
            DebuffsAfflicted = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accBlackPhial++;
            aequus.DebuffsInfliction.OverallTimeMultiplier += 0.5f;
        }

        public static void OnHitEffects(AequusPlayer aequus, Entity target, int damage, float knockback, bool crit)
        {
            int buffCount = 0;
            var entity = new EntityCommons(target);
            for (int i = 0; i < entity.maxBuffs; i++)
            {
                if (entity.buffType[i] > 0 && Main.debuff[entity.buffType[i]])
                {
                    buffCount++;
                }
            }
            if (Main.rand.NextBool(Math.Max(4 / aequus.accBlackPhial + aequus.cdBlackPhial / 5 + buffCount * 2, 1)))
            {
                var buffsToInflict = new List<int>(DebuffsAfflicted);
                for (int i = 0; i < NPC.maxBuffs; i++)
                {
                    if (entity.buffTime[i] > 0 && entity.buffType[i] > 0 && buffsToInflict.Contains(entity.buffType[i]))
                    {
                        buffsToInflict.Remove(entity.buffType[i]);
                    }
                }
                if (buffsToInflict.Count <= 0)
                    return;

                int buff = Main.rand.Next(buffsToInflict);
                if (!entity.ImmuneToBuff(buff))
                {
                    aequus.cdBlackPhial += 30 / aequus.accBlackPhial;
                    entity.AddBuff(buff, 150);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        PacketSystem.SyncSound(SoundPacket.BlackPhial, target.Center);
                    }
                    else
                    {
                        EmitSound(target.Center);
                    }
                    var size = target.Size;
                    int amt = (int)(size.Length() / 4f);
                    for (int i = 0; i < amt; i++)
                    {
                        var v = Main.rand.NextVector2CircularEdge(size.X, size.Y) * 0.5f;
                        var d = Dust.NewDustPerfect(target.Center + v, DustID.BatScepter, Vector2.Normalize(-v) * Main.rand.NextFloat(4f) + target.velocity, Scale: Main.rand.NextFloat(1f, 1.6f));
                        d.fadeIn = d.scale + 0.5f;
                        d.noGravity = true;
                    }
                }
            }
        }

        public static void EmitSound(Vector2 loc)
        {
            SoundEngine.PlaySound(Aequus.GetSound("concoction1").WithVolume(0.7f), loc);
        }
    }
}