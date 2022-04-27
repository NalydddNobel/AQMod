using Aequus.Effects;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public sealed class DeathEffects : GlobalNPC
    {
        public enum Context
        {
            None = 0,
            Snowgrave,
        }

        public override bool InstancePerEntity => true;

        public Context context;
        public int timer;

        public bool HasDeathContext => timer > 0 && context != Context.None;

        public void SetContext(Context context, int time)
        {
            if (timer < time)
            {
                this.context = context;
                timer = time;
            }
        }

        public void ClearContext()
        {
            context = Context.None;
            timer = 0;
        }

        public override void Load()
        {
            On.Terraria.NPC.VanillaHitEffect += NPC_VanillaHitEffect;
        }

        private void NPC_VanillaHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            try
            {
                var deathEffects = self.GetGlobalNPC<DeathEffects>();
                if (deathEffects.HasDeathContext && Main.netMode != NetmodeID.Server)
                {
                    if (deathEffects.context == Context.Snowgrave && self.life <= 0 && FrozenNPC.CanFreezeNPC(self))
                    {
                        SoundID.Item30?.PlaySound(self.Center);
                        return;
                    }
                }
            }
            catch
            {

            }
            orig(self, hitDirection, dmg);
        }

        public override void AI(NPC npc)
        {
            if (timer > 0)
                timer--;
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (HasDeathContext)
            {
                if (context == Context.Snowgrave)
                {
                    DeathEffect_SnowgraveFreeze(npc);
                }
            }
            return false;
        }
        private void DeathEffect_SnowgraveFreeze(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && FrozenNPC.CanFreezeNPC(npc))
            {
                EffectsSystem.BehindProjs.Add(new FrozenNPC(npc.Center, npc));
            }
        }
    }
}