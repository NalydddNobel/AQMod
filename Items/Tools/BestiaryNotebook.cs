using Aequus.Common.Networking;
using Aequus.NPCs;
using Aequus.UI.Elements;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Tools
{
    public class BestiaryNotebook : ModItem
    {
        internal static NPC dummyNPC;

        private static UIElement Hook_AddBookElement(On.Terraria.GameContent.Bestiary.NPCPortraitInfoElement.orig_ProvideUIElement orig, NPCPortraitInfoElement self, BestiaryUICollectionInfo info)
        {
            var element = orig(self, info);
            BestiaryNotebookElement.InNotebookPage = false;
            //if (Main.LocalPlayer.Aequus().bestiaryBook)
            {
                int npc = info.OwnerEntry.TryGetNPCNetID();
                if (npc != 0 && (npc < 0 || !NPCID.Sets.BelongsToInvasionOldOnesArmy[npc]) && AequusNPC.BestiarySpawnerInfo.TryGetValue(npc, out var spawnInfo) && spawnInfo.Enabled)
                {
                    var book = new BestiaryNotebookElement(npc, spawnInfo)
                    {
                        HAlign = 0.88f,
                        Width = new StyleDimension(26f, 0f),
                        Height = new StyleDimension(26f, 0f)
                    };
                    book.Top.Set(9, 0f);
                    element.Append(book);
                }
            }
            return element;
        }

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item4;
            Item.value = Item.sellPrice(gold: 2);
        }

        //public override bool CanUseItem(Player player)
        //{
        //    return player.Aequus().bestiaryBookSelection != 0;
        //}

        //public override bool? UseItem(Player player)
        //{
        //    var aequus = player.Aequus();
        //    if (Main.myPlayer == player.whoAmI && aequus.bestiaryBookSelection != 0)
        //    {
        //        if (Main.netMode == NetmodeID.MultiplayerClient)
        //        {
        //            PacketHandler.SendProcedure(PacketType.ClientNPCSpawn, "BestiaryBook", aequus.bestiaryBookSelection, Main.MouseWorld);
        //        }
        //        else
        //        {
        //            NPC.NewNPCDirect(player.GetSource_ItemUse(Item), Main.MouseWorld, aequus.bestiaryBookSelection);
        //        }
        //    }
        //    return false;
        //}

        //public override void UpdateInventory(Player player)
        //{
        //    var aequus = player.Aequus();
        //    aequus.bestiaryBook = true;
        //    if (aequus.bestiaryBookSelection != 0)
        //    {
        //        if (dummyNPC == null || dummyNPC.type != aequus.bestiaryBookSelection)
        //        {
        //            dummyNPC = new NPC();
        //            dummyNPC.SetDefaults(aequus.bestiaryBookSelection);
        //            dummyNPC.IsABestiaryIconDummy = true;
        //            dummyNPC.direction = 1;
        //            dummyNPC.active = true;
        //            dummyNPC.hide = false;
        //            dummyNPC.alpha = 0;
        //            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(dummyNPC.netID, out var offset))
        //            {
        //                dummyNPC.velocity.X = offset.Velocity;
        //            }
        //        }

        //        dummyNPC.FindFrame();
        //    }
        //}
    }
}