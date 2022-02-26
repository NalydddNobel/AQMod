using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    public class WorldDefeats : ModWorld
    {
        public static bool DownedStarite;
        public static bool DownedCrabson;
        public static bool DownedCurrents;

        public static bool DownedRedSprite;
        public static bool DownedSpaceSquid;

        public static bool DownedGlimmer;
        public static bool DownedCrabSeason;
        public static bool DownedDemonSiege;
        public static bool DownedGaleStreams;

        public static bool NoHitOmegaStarite { get; set; }

        public static bool ObtainedUltimateSword { get; set; }
        public static bool ObtainedMothmanMask { get; set; }
        public static bool ObtainedCatalystPainting { get; set; }

        public static bool HunterIntroduction { get; set; }
        public static bool PhysicistIntroduction { get; set; }
        public static bool OmegaStariteIntroduction { get; set; }

        public static bool TownNPCLavaImmunity { get; set; }
        public static bool TownNPCMoveAtNight { get; set; }

        public static bool AirMerchantHasBeenFound { get; set; }

        /// <summary>
        /// If WoF or Omega Starite have been defeated
        /// </summary>
        public static bool SudoHardmode => Main.hardMode || DownedStarite;

        public override void Initialize()
        {
            DownedGlimmer = false;
            DownedStarite = false;
            DownedCrabson = false;
            DownedDemonSiege = false;
            DownedCrabSeason = false;
            DownedGaleStreams = false;
            DownedCurrents = false;
            DownedRedSprite = false;
            DownedSpaceSquid = false;

            NoHitOmegaStarite = false;

            ObtainedUltimateSword = false;
            ObtainedCatalystPainting = false;
            ObtainedMothmanMask = false;

            HunterIntroduction = false;
            PhysicistIntroduction = false;
            OmegaStariteIntroduction = false;

            TownNPCLavaImmunity = false;
            TownNPCMoveAtNight = false;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["DownedGlimmer"] = DownedGlimmer,
                ["DownedStarite"] = DownedStarite,
                ["DownedCrabson"] = DownedCrabson,
                ["DownedDemonSiege"] = DownedDemonSiege,
                ["DownedCrabSeason"] = DownedCrabSeason,
                ["DownedGaleStreams"] = DownedGaleStreams,
                ["DownedCurrents"] = DownedCurrents,
                ["DownedRedSprite"] = DownedRedSprite,
                ["DownedSpaceSquid"] = DownedSpaceSquid,

                ["NoHitOmegaStarite"] = NoHitOmegaStarite,

                ["ObtainedUltimateSword"] = ObtainedUltimateSword,
                ["ObtainedMothmanMask"] = ObtainedMothmanMask,
                ["ObtainedCatalystPainting"] = ObtainedCatalystPainting,

                ["HunterIntroduction"] = HunterIntroduction,
                ["PhysicistIntroduction"] = PhysicistIntroduction,
                ["OmegaStariteIntroduction"] = OmegaStariteIntroduction,

                ["IWillBeBackLavaImmunity"] = TownNPCLavaImmunity,
                ["TownNPCMoveAtNight"] = TownNPCMoveAtNight,
            };
        }

        public override void Load(TagCompound tag)
        {
            DownedGlimmer = tag.GetBool("DownedGlimmer");
            DownedStarite = tag.GetBool("DownedStarite");
            DownedCrabson = tag.GetBool("DownedCrabson");
            DownedDemonSiege = tag.GetBool("DownedDemonSiege");
            DownedCrabSeason = tag.GetBool("DownedCrabSeason");
            DownedGaleStreams = tag.GetBool("DownedGaleStreams");
            DownedCurrents = tag.GetBool("DownedCurrents");
            DownedRedSprite = tag.GetBool("DownedRedSprite");
            DownedSpaceSquid = tag.GetBool("DownedSpaceSquid");

            NoHitOmegaStarite = tag.GetBool("NoHitOmegaStarite");

            ObtainedUltimateSword = tag.GetBool("ObtainedUltimateSword");
            ObtainedMothmanMask = tag.GetBool("ObtainedMothmanMask");
            ObtainedCatalystPainting = tag.GetBool("ObtainedCatalystPainting");

            HunterIntroduction = tag.GetBool("HunterIntroduction");
            PhysicistIntroduction = tag.GetBool("PhysicistIntroduction");
            OmegaStariteIntroduction = tag.GetBool("OmegaStariteIntroduction");

            TownNPCLavaImmunity = tag.GetBool("IWillBeBackLavaImmunity");
            TownNPCMoveAtNight = tag.GetBool("TownNPCMoveAtNight");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(DownedGlimmer);
            writer.Write(DownedStarite);
            writer.Write(DownedCrabson);
            writer.Write(DownedDemonSiege);
            writer.Write(DownedCrabSeason);
            writer.Write(DownedGaleStreams);
            writer.Write(DownedCurrents);
            writer.Write(DownedRedSprite);
            writer.Write(NoHitOmegaStarite);
            writer.Write(ObtainedUltimateSword);
            writer.Write(ObtainedCatalystPainting);
            writer.Write(ObtainedMothmanMask);
            writer.Write(DownedSpaceSquid);
            writer.Write(TownNPCLavaImmunity);
            writer.Write(TownNPCMoveAtNight);
            writer.Write(OmegaStariteIntroduction);
        }

        public override void NetReceive(BinaryReader reader)
        {
            DownedGlimmer = reader.ReadBoolean();
            DownedStarite = reader.ReadBoolean();
            DownedCrabson = reader.ReadBoolean();
            DownedDemonSiege = reader.ReadBoolean();
            DownedCrabSeason = reader.ReadBoolean();
            DownedGaleStreams = reader.ReadBoolean();
            DownedCurrents = reader.ReadBoolean();
            DownedRedSprite = reader.ReadBoolean();

            NoHitOmegaStarite = reader.ReadBoolean();

            ObtainedUltimateSword = reader.ReadBoolean();
            ObtainedCatalystPainting = reader.ReadBoolean();
            ObtainedMothmanMask = reader.ReadBoolean();

            DownedSpaceSquid = reader.ReadBoolean();

            TownNPCLavaImmunity = reader.ReadBoolean();
            TownNPCMoveAtNight = reader.ReadBoolean();
            OmegaStariteIntroduction = reader.ReadBoolean();
        }

        internal static bool AnyBossDefeated()
        {
            return AnyVanillaBossDefeated() || AnyAequusBossDefeated();
        }

        public static bool AnyAequusBossDefeated()
        {
            return DownedCrabson || DownedStarite;
        }

        internal static bool AnyVanillaBossDefeated()
        {
            return NPC.downedSlimeKing ||
                NPC.downedBoss1 ||
                NPC.downedBoss2 ||
                NPC.downedBoss3 ||
                NPC.downedQueenBee ||
                Main.hardMode;
        }
    }
}