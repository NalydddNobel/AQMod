using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public class PlayerLifeCuts : ModPlayer
    {
        public struct HealthSlice
        {
            public int time;
            public int amtTaken;
            public bool physicallyHitPlayer = false;
            public PlayerDeathReason reason = null;

            public HealthSlice(int amtTaken, int time = 0, bool hitPlayer = false, PlayerDeathReason reason = null)
            {
                this.amtTaken = amtTaken;
                this.time = time;
                physicallyHitPlayer = hitPlayer;
                this.reason = reason;
            }
        }

        public List<HealthSlice> slices;

        public override void Initialize()
        {
            slices = new List<HealthSlice>();
        }

        public override void UpdateDead()
        {
            slices.Clear();
        }

        public override void PostUpdateEquips()
        {
            for (int i = 0; i < slices.Count; i++)
            {
                var s = slices[i];
                s.time--;
                if (s.time <= 0)
                {
                    var reason = s.reason ?? PlayerDeathReason.ByOther(4);
                    if (s.physicallyHitPlayer)
                    {
                        Player.Hurt(reason, s.amtTaken, -Player.direction);
                    }
                    else
                    {
                        Player.statLife -= s.amtTaken;
                        if (Player.statLife <= 0)
                        {
                            Player.KillMe(reason, s.amtTaken, -Player.direction);
                        }
                    }
                    slices.RemoveAt(i);
                    i--;
                    continue;
                }
                slices[i] = s;
            }
        }

        public void SacrificeLife(int amt, int frames = 1, int separation = 1, bool hitPlayer = false, PlayerDeathReason reason = null)
        {
            if (amt < frames || frames < 2)
            {
                slices.Add(new HealthSlice(amt, 0));
                return;
            }
            int lifeTaken = amt / frames;
            for (int i = 0; i < frames - 1; i++)
            {
                slices.Add(new HealthSlice(lifeTaken, i * separation, hitPlayer, reason));
            }
            slices.Add(new HealthSlice(lifeTaken + (amt - lifeTaken * frames), frames * separation, hitPlayer, reason));
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (PlayerLifeCuts)clientClone;
            clone.slices = new List<HealthSlice>();
            foreach (var l in slices)
            {
                clone.slices.Add(l);
            }
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
        }
    }
}