using Aequus.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class ThoriumMod : ModSupport<ThoriumMod>
    {

        public override void SafeLoad(Mod mod)
        {
        }

        public override void AddRecipes()
        {
            foreach (var b in AequusBuff.PlayerDoTBuff)
            {
                if (b >= Main.maxBuffTypes && BuffLoader.GetBuff(b).Mod.Name == "Aequus")
                {
                    Call("AddPlayerDoTBuffID", b);
                }
            }
            foreach (var b in AequusBuff.PlayerStatusBuff)
            {
                if (b >= Main.maxBuffTypes && BuffLoader.GetBuff(b).Mod.Name == "Aequus")
                {
                    Call("AddPlayerStatusBuffID", b);
                }
            }
        }

        public override void SafeUnload()
        {
        }
    }
}