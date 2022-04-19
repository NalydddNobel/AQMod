using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Players.StatData
{
    public abstract class PlayerStat : ModType, INetBinary, ICloneable
    {
        public int Type { get; private set; }

        protected sealed override void Register()
        {
            Type = CustomStatsManager.RegisterStat(this);
        }
        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public virtual PlayerStat GetNewInstance()
        {
            return (PlayerStat)Clone();
        }

        public virtual void Initalize(Player player, AequusPlayer aequus)
        {
        }

        public virtual void ResetEffects(Player player, AequusPlayer aequus)
        {
            Clear();
        }
        public abstract void Clear();

        public virtual void UpdateDead(Player player, AequusPlayer aequus)
        {
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void Add(PlayerStat playerStat)
        {
        }

        public virtual void Send(BinaryWriter writer)
        {
        }

        public virtual void Receive(BinaryReader reader)
        {
        }
    }
}