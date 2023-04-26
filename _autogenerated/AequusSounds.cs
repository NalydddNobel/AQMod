using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

#if DEBUG
#else
namespace Aequus {
    /// <summary>
    /// (Amt Sounds: 124)
    /// </summary>
    [CompilerGenerated]
    public class AequusSounds : ILoadable {
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((SoundAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/boowomp
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset boowomp = new SoundAsset("Aequus/Assets/Sounds/boowomp", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/boundBowRecharge
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset boundBowRecharge = new SoundAsset("Aequus/Assets/Sounds/Item/boundBowRecharge", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/chargeUp
        /// <para>Num Variants: 6</para>
        /// </summary>
        public static readonly SoundAsset chargeUp = new SoundAsset("Aequus/Assets/Sounds/Crabson/chargeUp", 6);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/chargeUp0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset chargeUp0 = new SoundAsset("Aequus/Assets/Sounds/Crabson/chargeUp0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/chargeUp1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset chargeUp1 = new SoundAsset("Aequus/Assets/Sounds/Crabson/chargeUp1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/chargeUp2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset chargeUp2 = new SoundAsset("Aequus/Assets/Sounds/Crabson/chargeUp2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/chargeUp3
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset chargeUp3 = new SoundAsset("Aequus/Assets/Sounds/Crabson/chargeUp3", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/chargeUp4
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset chargeUp4 = new SoundAsset("Aequus/Assets/Sounds/Crabson/chargeUp4", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/chargeUp5
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset chargeUp5 = new SoundAsset("Aequus/Assets/Sounds/Crabson/chargeUp5", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/coinCrit
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset coinCrit = new SoundAsset("Aequus/Assets/Sounds/Item/coinCrit", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/coinHit
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset coinHit = new SoundAsset("Aequus/Assets/Sounds/Item/coinHit", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/coinHit0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset coinHit0 = new SoundAsset("Aequus/Assets/Sounds/Item/coinHit0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/coinHit1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset coinHit1 = new SoundAsset("Aequus/Assets/Sounds/Item/coinHit1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/concoction
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset concoction = new SoundAsset("Aequus/Assets/Sounds/concoction", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/concoction0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset concoction0 = new SoundAsset("Aequus/Assets/Sounds/concoction0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/concoction1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset concoction1 = new SoundAsset("Aequus/Assets/Sounds/concoction1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/crownOfBlood
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset crownOfBlood = new SoundAsset("Aequus/Assets/Sounds/Item/crownOfBlood", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/dagger
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset dagger = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/dagger", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/daggerHit
        /// <para>Num Variants: 3</para>
        /// </summary>
        public static readonly SoundAsset daggerHit = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/daggerHit", 3);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/daggerHit0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset daggerHit0 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/daggerHit0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/daggerHit1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset daggerHit1 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/daggerHit1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/daggerHit2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset daggerHit2 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/daggerHit2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/PossessedShard/dash
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset dash = new SoundAsset("Aequus/Assets/Sounds/Item/PossessedShard/dash", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/OmegaStarite/dead
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset dead = new SoundAsset("Aequus/Assets/Sounds/OmegaStarite/dead", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/death
        /// <para>Num Variants: 3</para>
        /// </summary>
        public static readonly SoundAsset death = new SoundAsset("Aequus/Assets/Sounds/Crabson/death", 3);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/death0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset death0 = new SoundAsset("Aequus/Assets/Sounds/Crabson/death0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/death1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset death1 = new SoundAsset("Aequus/Assets/Sounds/Crabson/death1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/death2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset death2 = new SoundAsset("Aequus/Assets/Sounds/Crabson/death2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/SpaceSquid/deathray
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset deathray = new SoundAsset("Aequus/Assets/Sounds/SpaceSquid/deathray", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/destroy
        /// <para>Num Variants: 4</para>
        /// </summary>
        public static readonly SoundAsset destroy = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/destroy", 4);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/destroy0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset destroy0 = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/destroy0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/destroy1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset destroy1 = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/destroy1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/destroy2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset destroy2 = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/destroy2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/destroy3
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset destroy3 = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/destroy3", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/doomShotgun
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset doomShotgun = new SoundAsset("Aequus/Assets/Sounds/Item/doomShotgun", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/electricHit
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset electricHit = new SoundAsset("Aequus/Assets/Sounds/Item/electricHit", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/electricHit0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset electricHit0 = new SoundAsset("Aequus/Assets/Sounds/Item/electricHit0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/electricHit1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset electricHit1 = new SoundAsset("Aequus/Assets/Sounds/Item/electricHit1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/evilConvert
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset evilConvert = new SoundAsset("Aequus/Assets/Sounds/Item/evilConvert", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/OmegaStarite/explosion
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset explosion = new SoundAsset("Aequus/Assets/Sounds/OmegaStarite/explosion", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/OmegaStarite/hit
        /// <para>Num Variants: 3</para>
        /// </summary>
        public static readonly SoundAsset hit_OmegaStarite = new SoundAsset("Aequus/Assets/Sounds/OmegaStarite/hit", 3);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/hit
        /// <para>Num Variants: 4</para>
        /// </summary>
        public static readonly SoundAsset hit_Sword = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/hit", 4);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/OmegaStarite/hit0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset hit0_OmegaStarite = new SoundAsset("Aequus/Assets/Sounds/OmegaStarite/hit0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/hit0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset hit0_Sword = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/hit0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/OmegaStarite/hit1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset hit1_OmegaStarite = new SoundAsset("Aequus/Assets/Sounds/OmegaStarite/hit1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/hit1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset hit1_Sword = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/hit1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/OmegaStarite/hit2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset hit2_OmegaStarite = new SoundAsset("Aequus/Assets/Sounds/OmegaStarite/hit2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/hit2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset hit2_Sword = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/hit2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/hit3
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset hit3 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/hit3", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/inflictaetherfire
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset inflictaetherfire = new SoundAsset("Aequus/Assets/Sounds/inflictaetherfire", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/inflictBlood
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset inflictBlood = new SoundAsset("Aequus/Assets/Sounds/inflictBlood", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/inflictFire
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset inflictFire = new SoundAsset("Aequus/Assets/Sounds/inflictFire", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/inflictweakness
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset inflictweakness = new SoundAsset("Aequus/Assets/Sounds/inflictweakness", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/jump
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset jump = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/jump", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/jump0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset jump0 = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/jump0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/jump1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset jump1 = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/jump1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Misc/jumpshroom
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset jumpshroom = new SoundAsset("Aequus/Assets/Sounds/Misc/jumpshroom", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/largeSlam
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset largeSlam = new SoundAsset("Aequus/Assets/Sounds/Crabson/largeSlam", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/largeSlash
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset largeSlash = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/largeSlash", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/largeSlash0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset largeSlash0 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/largeSlash0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/largeSlash1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset largeSlash1 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/largeSlash1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Meathook/meathook
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset meathook = new SoundAsset("Aequus/Assets/Sounds/Item/Meathook/meathook", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Meathook/meathook0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset meathook0 = new SoundAsset("Aequus/Assets/Sounds/Item/Meathook/meathook0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Meathook/meathook1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset meathook1 = new SoundAsset("Aequus/Assets/Sounds/Item/Meathook/meathook1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Meathook/meathookConnect
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset meathookConnect = new SoundAsset("Aequus/Assets/Sounds/Item/Meathook/meathookConnect", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Meathook/meathookPull
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset meathookPull = new SoundAsset("Aequus/Assets/Sounds/Item/Meathook/meathookPull", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/moonflower
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset moonflower = new SoundAsset("Aequus/Assets/Sounds/moonflower", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/necromancySpawn
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset necromancySpawn = new SoundAsset("Aequus/Assets/Sounds/Item/necromancySpawn", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/neonCharge
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset neonCharge = new SoundAsset("Aequus/Assets/Sounds/neonCharge", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/neonShoot
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset neonShoot = new SoundAsset("Aequus/Assets/Sounds/neonShoot", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/pearlShoot
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset pearlShoot = new SoundAsset("Aequus/Assets/Sounds/Crabson/pearlShoot", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/photobookopen
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset photobookopen = new SoundAsset("Aequus/Assets/Sounds/photobookopen", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/photobookturn
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset photobookturn = new SoundAsset("Aequus/Assets/Sounds/photobookturn", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Nightfall/pushDown
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset pushDown = new SoundAsset("Aequus/Assets/Sounds/Item/Nightfall/pushDown", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Nightfall/pushUp
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset pushUp = new SoundAsset("Aequus/Assets/Sounds/Item/Nightfall/pushUp", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/raygun
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset raygun = new SoundAsset("Aequus/Assets/Sounds/Item/raygun", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/recruitZombie
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset recruitZombie = new SoundAsset("Aequus/Assets/Sounds/recruitZombie", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/savingGraceCast
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset savingGraceCast = new SoundAsset("Aequus/Assets/Sounds/Item/savingGraceCast", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/savingGraceHeal
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset savingGraceHeal = new SoundAsset("Aequus/Assets/Sounds/Item/savingGraceHeal", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/select
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset select = new SoundAsset("Aequus/Assets/Sounds/select", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/PossessedShard/shardHit
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset shardHit = new SoundAsset("Aequus/Assets/Sounds/Item/PossessedShard/shardHit", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/PossessedShard/shardHit0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shardHit0 = new SoundAsset("Aequus/Assets/Sounds/Item/PossessedShard/shardHit0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/PossessedShard/shardHit1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shardHit1 = new SoundAsset("Aequus/Assets/Sounds/Item/PossessedShard/shardHit1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/JunkJet/shoot
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset shoot_JunkJet = new SoundAsset("Aequus/Assets/Sounds/Item/JunkJet/shoot", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Slingshot/shoot
        /// <para>Num Variants: 3</para>
        /// </summary>
        public static readonly SoundAsset shoot_Slingshot = new SoundAsset("Aequus/Assets/Sounds/Item/Slingshot/shoot", 3);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/shoot
        /// <para>Num Variants: 3</para>
        /// </summary>
        public static readonly SoundAsset shoot_Umystick = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/shoot", 3);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/JunkJet/shoot0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot0_JunkJet = new SoundAsset("Aequus/Assets/Sounds/Item/JunkJet/shoot0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Slingshot/shoot0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot0_Slingshot = new SoundAsset("Aequus/Assets/Sounds/Item/Slingshot/shoot0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/shoot0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot0_Umystick = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/shoot0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/JunkJet/shoot1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot1_JunkJet = new SoundAsset("Aequus/Assets/Sounds/Item/JunkJet/shoot1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Slingshot/shoot1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot1_Slingshot = new SoundAsset("Aequus/Assets/Sounds/Item/Slingshot/shoot1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/shoot1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot1_Umystick = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/shoot1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Slingshot/shoot2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot2_Slingshot = new SoundAsset("Aequus/Assets/Sounds/Item/Slingshot/shoot2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Umystick/shoot2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset shoot2_Umystick = new SoundAsset("Aequus/Assets/Sounds/Item/Umystick/shoot2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/sizzle
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset sizzle = new SoundAsset("Aequus/Assets/Sounds/sizzle", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Misc/skeletonTrapMake
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset skeletonTrapMake = new SoundAsset("Aequus/Assets/Sounds/Misc/skeletonTrapMake", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/slam
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset slam = new SoundAsset("Aequus/Assets/Sounds/Crabson/slam", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/slam0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset slam0 = new SoundAsset("Aequus/Assets/Sounds/Crabson/slam0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/slam1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset slam1 = new SoundAsset("Aequus/Assets/Sounds/Crabson/slam1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/slideWhistle
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset slideWhistle = new SoundAsset("Aequus/Assets/Sounds/slideWhistle", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/slotMachine
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset slotMachine = new SoundAsset("Aequus/Assets/Sounds/Item/slotMachine", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/SpaceSquid/snowflakeShoot
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset snowflakeShoot = new SoundAsset("Aequus/Assets/Sounds/SpaceSquid/snowflakeShoot", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/snowgrave
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset snowgrave = new SoundAsset("Aequus/Assets/Sounds/Item/snowgrave", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/sonicMeteor
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset sonicMeteor = new SoundAsset("Aequus/Assets/Sounds/sonicMeteor", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/SpaceSquid/spaceGun
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset spaceGun = new SoundAsset("Aequus/Assets/Sounds/SpaceSquid/spaceGun", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/squeak
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset squeak = new SoundAsset("Aequus/Assets/Sounds/squeak", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/OmegaStarite/starBullets
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset starBullets = new SoundAsset("Aequus/Assets/Sounds/OmegaStarite/starBullets", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Slingshot/stretch
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset stretch = new SoundAsset("Aequus/Assets/Sounds/Item/Slingshot/stretch", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/superAttack
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset superAttack = new SoundAsset("Aequus/Assets/Sounds/Crabson/superAttack", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/superJump
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset superJump = new SoundAsset("Aequus/Assets/Sounds/Crabson/superJump", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordPowerReady
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset swordPowerReady = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordPowerReady", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordSwoosh
        /// <para>Num Variants: 6</para>
        /// </summary>
        public static readonly SoundAsset swordSwoosh = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordSwoosh", 6);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordSwoosh0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset swordSwoosh0 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordSwoosh0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordSwoosh1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset swordSwoosh1 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordSwoosh1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordSwoosh2
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset swordSwoosh2 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordSwoosh2", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordSwoosh3
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset swordSwoosh3 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordSwoosh3", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordSwoosh4
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset swordSwoosh4 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordSwoosh4", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/Sword/swordSwoosh5
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset swordSwoosh5 = new SoundAsset("Aequus/Assets/Sounds/Item/Sword/swordSwoosh5", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/RedSprite/thunderClap
        /// <para>Num Variants: 2</para>
        /// </summary>
        public static readonly SoundAsset thunderClap = new SoundAsset("Aequus/Assets/Sounds/RedSprite/thunderClap", 2);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/RedSprite/thunderClap0
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset thunderClap0 = new SoundAsset("Aequus/Assets/Sounds/RedSprite/thunderClap0", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/RedSprite/thunderClap1
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset thunderClap1 = new SoundAsset("Aequus/Assets/Sounds/RedSprite/thunderClap1", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/touhouShoot
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset touhouShoot = new SoundAsset("Aequus/Assets/Sounds/touhouShoot", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Crabson/walk
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset walk = new SoundAsset("Aequus/Assets/Sounds/Crabson/walk", 1);
        /// <summary>
        /// Full Path: Aequus/Assets/Sounds/Item/warhorn
        /// <para>Num Variants: 1</para>
        /// </summary>
        public static readonly SoundAsset warhorn = new SoundAsset("Aequus/Assets/Sounds/Item/warhorn", 1);
    }
}
#endif