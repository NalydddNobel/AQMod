using Aequus.Common.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;

public class BlackJellyfish : AIJellyfish {
    public override void SetDefaults() {
        NPC.lifeMax = 10;
    }
}