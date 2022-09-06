using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterBountyPlayer : ModPlayer
    {
        public List<string> CompletedBounties { get; private set; }

        public override void Initialize()
        {
            CompletedBounties = new List<string>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["CompletedBounties"] = CompletedBounties;
        }

        public override void LoadData(TagCompound tag)
        {
            CompletedBounties = tag.Get<List<string>>("CompletedBounties");
            if (CompletedBounties == null)
            {
                CompletedBounties = new List<string>();
            }
        }

        public override ModPlayer Clone(Player newEntity)
        {
            var clone = (CarpenterBountyPlayer)base.Clone(newEntity);
            clone.CompletedBounties = new List<string>(CompletedBounties);
            return clone;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (CarpenterBountyPlayer)clientClone;
            clone.CompletedBounties = new List<string>(CompletedBounties);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var client = (CarpenterBountyPlayer)clientPlayer;
            if (CompletedBounties.Count != client.CompletedBounties.Count)
            {
                PacketSystem.Send((p) =>
                {
                    p.Write(Player.whoAmI);
                    p.Write(CompletedBounties.Count);
                    for (int i = 0; i < CompletedBounties.Count; i++)
                    {
                        p.Write(CompletedBounties[i]);
                    }
                }, PacketType.CarpenterBountiesCompleted);
            }
        }

        public void RecieveClientChanges(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            CompletedBounties.Clear();
            for (int i = 0; i < CompletedBounties.Count; i++)
            {
                CompletedBounties.Add(reader.ReadString());
            }
        }
    }
}