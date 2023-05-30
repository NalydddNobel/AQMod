using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Necro {
    public class NecromancyDebuff : ModBuff
    {
        public override string Texture => Aequus.PlaceholderDebuff;

        public virtual float Tier => 1f;
        public virtual int DamageSet => 20;
        public virtual float BaseSpeed => 0.25f;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            NecromancyDatabase.NecromancyDebuffs.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.ghostDebuffDOT = 16;
            zombie.ghostDamage = DamageSet;
            zombie.ghostSpeed = BaseSpeed;
            zombie.DebuffTier(Tier);
            zombie.RenderLayer(ColorTargetID.ZombieScepter);
        }

        public static void ReduceDamageForDebuffApplication<T>(float tier, NPC npc, ref NPC.HitModifiers modifiers) where T : NecromancyDebuff
        {
            if (tier <= 100f && !npc.buffImmune[ModContent.BuffType<T>()] && !npc.HasBuff<T>() && NecromancyDatabase.TryGet(npc, out var value) && value.EnoughPower(tier))
            {
                modifiers.SetMaxDamage(npc.life / 2);
            }
        }

        public static void ApplyDebuff<T>(NPC npc, int time, int player) where T : NecromancyDebuff
        {
            npc = npc.realLife == -1 ? npc : Main.npc[npc.realLife];

            float tier = ModContent.GetInstance<T>().Tier;
            bool cheat = tier >= 100;
            if (cheat)
            {
                npc.buffImmune[ModContent.BuffType<T>()] = false;
            }
            npc.AddBuff(ModContent.BuffType<T>(), time);
            npc.GetGlobalNPC<NecromancyNPC>().zombieOwner = player;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                PacketSystem.SyncNecromancyOwner(npc.whoAmI, player);
            }
        }
    }
}