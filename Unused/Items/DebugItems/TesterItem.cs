using Aequus.Common.DataSets;
using Aequus.Common.Effects;
using Aequus.Common.Items;
using Aequus.Common.World;
using Aequus.Content.Biomes.Aether;
using Aequus.Content.Biomes.Pyramid;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.NPCs.Monsters.Event.DemonSiege;
using Aequus.Tiles.CrabCrevice;
using Aequus.Tiles.Herbs.Manacle;
using Aequus.Tiles.Herbs.Mistral;
using Aequus.Tiles.Herbs.Moonflower;
using Aequus.Tiles.Herbs.Moray;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Unused.Items.DebugItems {
    internal class TesterItem : ModItem {
        #region Test Methods
        public void WriteBuffSet_DemonSiegeImmune(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.DemonSiegeImmune);
        }

        public void WriteBuffSet_PlayerDoTDebuff(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.PlayerDoTDebuff);
        }

        public void WriteBuffSet_PlayerStatusDebuff(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.PlayerStatusDebuff);
        }

        public void WriteBuffSet_PotionPrefixBlacklist(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.PotionPrefixBlacklist);
        }

        public void WriteBuffSet_DontChangeDuration(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.DontChangeDuration);
        }

        public void WriteBuffSet_ProbablyCooldownDebuff(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.ProbablyCooldownDebuff);
        }

        public void WriteBuffSet_NotTypicalDebuff(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.NotTypicalDebuff);
        }

        public void WriteBuffSet_ProbablyFireDebuff(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.ProbablyFireDebuff);
        }

        public void WriteBuffSet_ClearableDebuff(TestParameters parameters) {
            WriteBuffs(parameters, BuffSets.ClearableDebuff);
        }

        public void WriteBuffs(TestParameters parameters, ICollection<int> collection) {
            foreach (var npc in collection) {
                var color = Color.Red.HueAdd(npc / (float)BuffLoader.BuffCount);
                Main.NewText(Lang.GetBuffName(npc), color);
            }
        }

        public void WeirdWindFlag(TestParameters parameters) {
            Main.windPhysics = !Main.windPhysics;
            Main.NewText($"Wind Physics set to '{Main.windPhysics}'");
        }

        public void SetupTravelShop(TestParameters parameters) {
            Chest.SetupTravelShop();
        }

        public void ToggleHardmode(TestParameters parameters) {
            Main.hardMode = !Main.hardMode;
            Main.NewText($"Hardmode set to '{Main.hardMode}'");
        }

        public void TileCoords(TestParameters parameters) {
            Main.NewText(Main.tile[parameters.TileCoordinates_Point].TileFrameX);
            Main.NewText(Main.tile[parameters.TileCoordinates_Point].TileFrameY, Color.Yellow);
        }

        public void CompleteBestiary(TestParameters parameters) {
            foreach (var npc in ContentSamples.NpcsByNetId) {
                Main.BestiaryTracker.Kills.SetKillCountDirectly(npc.Value.GetBestiaryCreditId(), 9999);
                Main.BestiaryTracker.Sights.RegisterWasNearby(npc.Value);
                Main.BestiaryTracker.Chats.RegisterChatStartWith(npc.Value);
            }
        }

        public void WriteHallowNPCs(TestParameters parameters) {
            WriteNPCsInHashSet(parameters, NPCSets.IsHallow);
        }
        public void WriteCrimsonNPCs(TestParameters parameters) {
            WriteNPCsInHashSet(parameters, NPCSets.IsCrimson);
        }
        public void WriteCorruptNPCs(TestParameters parameters) {
            WriteNPCsInHashSet(parameters, NPCSets.IsCorrupt);
        }

        public void GeneratePyramid(TestParameters parameters) {
            ModContent.GetInstance<PyramidGenerator>().ForceGeneratePyramid();
        }

        public void GenerateSecretPyramidRoom(TestParameters parameters) {
            ModContent.GetInstance<PyramidGenerator>().GenerateSecretRoom(parameters.X, parameters.Y);
        }

        public void UnlockAllEntries(TestParameters parameters) {
            foreach (var npc in ContentSamples.NpcsByNetId) {
                Main.BestiaryTracker.Kills.SetKillCountDirectly(npc.Value.GetBestiaryCreditId(), 9999);
                Main.BestiaryTracker.Sights.RegisterWasNearby(npc.Value);
                Main.BestiaryTracker.Chats.RegisterChatStartWith(npc.Value);
            }
        }

        public void GenerateAetherWaterfall(TestParameters parameters) {
            UnlockAllEntries(parameters);
            ModContent.GetInstance<LegacyAetherCavesGenerator>().GenerateWaterfall(parameters.X, parameters.Y);
        }

        public void GenerateAether(TestParameters parameters) {
            ModContent.GetInstance<LegacyAetherCavesGenerator>().Generate();
        }

        public void GenerateAetheriumSpike(TestParameters parameters) {
            ModContent.GetInstance<LegacyAetherCavesGenerator>().GenerateAetheriumSpike(parameters.X, parameters.Y);
        }

        public void ShimmerBiomeReworkTunnels(TestParameters parameters) {
            ModContent.GetInstance<LegacyAetherCavesGenerator>().GenerateTunnel(parameters.X, parameters.Y, parameters.X, parameters.Y, 100, 100);
            //ModContent.GetInstance<AetherCavesGenerator>().GenerateFill(parameters.X, parameters.Y, parameters.X, parameters.Y, 100, 100);
        }

        public void SetCam(TestParameters parameters) {
            var camera = ModContent.GetInstance<CameraFocus>();
            if (camera.hold <= 10) {
                foreach (var g in Main.gore) {
                    g.active = false;
                }
                camera.SetTarget("Test", Main.MouseWorld, CameraPriority.VeryImportant, hold: int.MaxValue - 100);
            }
            else {
                camera.hold = 2;
            }
        }

        public void SpitTileObjectData(TestParameters parameters) {
            int style = 0;
            int alt = 0;
            TileObjectData.GetTileInfo(Main.tile[parameters.X, parameters.Y], ref style, ref alt);
            Main.NewText(style, Color.Orange);
            Main.NewText(alt, Color.Yellow);
        }

        public void ReforgeItems(TestParameters parameters, int pre) {
            var player = parameters.Player;
            for (int i = 0; i < Main.InventorySlotsTotal; i++) {
                int stack = player.inventory[i].stack;
                player.inventory[i].SetDefaults(player.inventory[i].type);
                player.inventory[i].stack = stack;
                player.inventory[i].Prefix(pre);
            }
        }

        public void RadonCavesTest(TestParameters parameters) {
            int x = parameters.X;
            int y = parameters.Y;
            if (AequusWorldGenerator.RadonCaves.ValidSpotForCave(x, y)) {
                AequusWorldGenerator.RadonCaves.CreateCave(x, y);
            }
        }
        public void RadonGrowStalactite(TestParameters parameters) {
            int x = parameters.X;
            int y = parameters.Y;
            AequusWorldGenerator.RadonCaves.GrowStalactite(x, y, AequusWorldGenerator.RadonCaves.MaxWidth, AequusWorldGenerator.RadonCaves.MaxHeight);
        }

        public void PlacePollenExamples(TestParameters parameters) {
            int x = parameters.X;
            int y = parameters.Y;
            WorldGen.PlaceTile(x, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x, y - 1, ModContent.TileType<MorayTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y - 1, ModContent.TileType<MorayTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x + 4, y - 1, ModContent.TileType<MorayTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x, y + 2, ModContent.TileType<ManacleTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x + 2, y + 2, ModContent.TileType<ManacleTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x + 4, y + 2, ModContent.TileType<ManacleTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x + 2, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x + 4, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 9, TileID.Cloud, forced: true);
            WorldGen.PlaceTile(x, y + 8, ModContent.TileType<MistralTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 9, TileID.RainCloud, forced: true);
            WorldGen.PlaceTile(x + 2, y + 8, ModContent.TileType<MistralTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 9, TileID.SnowCloud, forced: true);
            WorldGen.PlaceTile(x + 4, y + 8, ModContent.TileType<MistralTile>(), forced: true, style: 2);
        }

        public void SpawnPhysicalTestDummies(TestParameters parameters, int npcId) {
            var m = parameters.WorldCoordinates;
            for (int i = 0; i < 5; i++) {
                var n = NPC.NewNPCDirect(null, m + new Vector2(90f * (i - 2), 0f), npcId);
                n.Aequus().noAI = true;
                n.knockBackResist = 0f;
            }
        }

        public void SpawnGlimmer(TestParameters parameters) {
            GlimmerZone.TileLocation = Point.Zero;
            GlimmerSystem.BeginEvent();
            GlimmerZone.TileLocation = parameters.TileCoordinates_Point;
        }

        public void KillOfType(TestParameters parameters, int npcId) {
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && Main.npc[i].type == npcId) {
                    Main.npc[i].StrikeInstantKill();
                }
            }
        }
        public NPC NoAINPC(TestParameters parameters, int npcId) {
            var n = NPC.NewNPCDirect(null, Main.MouseWorld, npcId);
            n.Aequus().noAI = true;
            return n;
        }

        public void NoAITrapperImp(TestParameters parameters) {
            var n = NPC.NewNPCDirect(null, Main.MouseWorld.NumFloor(4), ModContent.NPCType<TrapperImp>());
            n.Aequus().noAI = true;

            foreach (var v in Helper.CircularVector(3, -MathHelper.PiOver2)) {
                var n2 = NPC.NewNPCDirect(n.GetSource_FromAI(), (Main.MouseWorld + v * 125f).NumFloor(4), ModContent.NPCType<Trapper>(), n.whoAmI, ai1: n.whoAmI + 1f);
                n2.Aequus().noAI = true;
                n2.rotation = v.ToRotation() + MathHelper.PiOver2;
            }
        }

        public void WriteNPCsInHashSet(TestParameters parameters, HashSet<int> hash) {
            foreach (var npc in hash) {
                var color = Color.Red.HueAdd(npc / (float)NPCLoader.NPCCount);
                Main.NewText(Lang.GetNPCNameValue(npc), color);
            }
        }
        #endregion

        #region Item Stuff
        public override string Texture => AequusTextures.Gamestar.Path;
        public override bool IsLoadingEnabled(Mod mod) {
            return Aequus.DevelopmentFeatures;
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.rare = ItemRarityID.Red;
            Item.width = 20;
            Item.height = 20;

            Item.color = Color.Red;
            _debugMethod = 0;
            _methods = null;
            try {
                var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                List<MethodInfo> validMethods = new();
                for (int i = 0; i < methods.Length; i++) {
                    var parameters = methods[i].GetParameters();
                    if (parameters.Length != 1 || parameters[0].ParameterType != typeof(TestParameters)) {
                        continue;
                    }

                    validMethods.Add(methods[i]);
                }
                _methods = validMethods.ToArray();
                Item.color = Main.OurFavoriteColor;
            }
            catch {
            }
        }

        private void NextDebugMethod() {
            if (Main.MouseScreen.Y > Main.screenWidth / 2f) {
                _debugMethod--;
                if (_debugMethod < 0) {
                    _debugMethod = _methods.Length - 1;
                }
            }
            else {
                _debugMethod++;
                if (_debugMethod >= _methods.Length) {
                    _debugMethod = 0;
                }
            }
        }
        public override bool? UseItem(Player player) {
            if (player.altFunctionUse == 2) {
                NextDebugMethod();
                Main.NewText($"Selected {_debugMethod}: {_methods[_debugMethod].Name}", Main.DiscoColor);
                return true;
            }

            TestParameters testParameters = new(Helper.MouseTileX, Helper.MouseTileY, player);
            _methods[_debugMethod]?.Invoke(this, new object[] { testParameters });
            return true;
        }

        public override void HoldItem(Player player) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            Main.instance.MouseText(_methods[_debugMethod].Name, 1);
            int x = Helper.MouseTileX;
            int y = Helper.MouseTileY;
            if (Main.GameUpdateCount % 10 == 0) {
                int dustType = DustID.Torch;
                if (Helper.InOuterX(x, y, 3)) {
                    dustType = DustID.CursedTorch;
                }
                if (Helper.InOuterX(x, y, 5)) {
                    dustType = DustID.IceTorch;
                }
                if (Helper.InOuterX(x, y, 8)) {
                    dustType = DustID.CrimsonTorch;
                }
                Helper.DebugDustRectangle(x, y, dustType);
            }
        }

        public override bool AltFunctionUse(Player player) {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            tooltips.Add(new(Mod, "DebugLine0", $"{_debugMethod}: {_methods[_debugMethod]}"));

            if (Helper.DebugKeyPressed)
                return;

            tooltips.Add(new(Mod, "DebugLine1", string.Join(", ", Helper.GetListOfActiveDifficulties())));
            tooltips.Add(new TooltipLine(Mod, "DebugLine2", string.Join(", ", Main.LocalPlayer.GetStringListOfBiomes().ConvertAll(Language.GetTextValue))));
        }
        #endregion

        #region Data Stuff
        public MethodInfo[] _methods;
        public int _debugMethod;

        public record struct TestParameters(int X, int Y, Player Player) {
            public Point TileCoordinates_Point => new(X, Y);
            public Vector2 TileCoordinates_Vector2 => new(X, Y);
            public Vector2 WorldCoordinates => new Vector2(X, Y).ToWorldCoordinates();
        }
        #endregion
    }
}