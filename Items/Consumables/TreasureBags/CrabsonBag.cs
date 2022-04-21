using Aequus.Items.Accessories;
using Aequus.Items.Misc.Energies;
using Aequus.NPCs.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.TreasureBags
{
    public class CrabsonBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.Blue;
        public override int BossBagNPC => ModContent.NPCType<Crabson>();
        protected override bool PreHardmode => true;

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<AquaticEnergy>(), Main.rand.Next(4) + 5);
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<Mendshroom>());

            //AQMod.AequusDeveloperItems(player, hardmode: false);
            //player.QuickSpawnItem(ModContent.ItemType<Crabax>());
            //if (Main.rand.NextBool(7))
            //    player.QuickSpawnItem(ModContent.ItemType<CrabsonMask>());
            //player.QuickSpawnItem(ModContent.ItemType<BreakdownDye>());
            //player.QuickSpawnItem(ModContent.ItemType<CrustaciumBlob>(), Main.rand.NextVRand(120, 200));
            //var choices = new List<int>()
            //{
            //    ModContent.ItemType<Bubbler>(),
            //    ModContent.ItemType<CinnabarBow>(),
            //    ModContent.ItemType<JerryClawFlail>(),
            //    ModContent.ItemType<Crabsol>(),
            //};
            //int choice = Main.rand.Next(choices.Count);
            //player.QuickSpawnItem(choices[choice]);
            //choices.RemoveAt(choice);
            //choice = Main.rand.Next(choices.Count);
            //player.QuickSpawnItem(choices[choice]);
        }
    }
}