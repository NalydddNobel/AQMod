using Aequus.Common.Projectiles.SentryChip;
using Aequus.Projectiles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Common.Items.SentryChip {
    public class SentryAccessoriesDatabase : ILoadable {
        private static readonly Dictionary<int, List<SentryInteraction>> Interactions = new();

        public static void Register<T>(int itemId) where T : SentryInteraction {
            Register(itemId, ModContent.GetInstance<T>().CreateRegisterInstance(itemId));
        }

        internal static void Register(int itemId, SentryInteraction interaction) {
            (CollectionsMarshal.GetValueRefOrAddDefault(Interactions, itemId, out bool exists) ??= new()).Add(interaction);
        }

        public static int RemoveAll(int itemId, Predicate<SentryInteraction> predicate) {
            if (!TryGetInteractions(itemId, out var list)) {
                return 0;
            }

            return list.RemoveAll(predicate);
        }

        public static bool Remove(int itemId, SentryInteraction interaction) {
            if (!TryGetInteractions(itemId, out var list)) {
                return false;
            }

            return list.Remove(interaction);
        }

        public static bool TryGetInteractions(int itemId, out List<SentryInteraction> interaction) {
            return Interactions.TryGetValue(itemId, out interaction);
        }

        public static List<SentryInteraction> GetInteraction(int itemId) {
            return Interactions[itemId];
        }

        public void Load(Mod mod) {
        }

        public void Unload() {
            Interactions.Clear();
        }

        public static void ApplyOnAIInteractions(Projectile projectile, SentryAccessoriesGlobalProj sentryAccessoriesGlobalProj) {
            foreach (var accessory in AequusPlayer.GetEquips(Main.player[projectile.owner], armor: false, sentrySlot: true)) {
                if (!TryGetInteractions(accessory.type, out var interactions)) {
                    continue;
                }

                SentryAccessoryInfo info = new(projectile, sentryAccessoriesGlobalProj, accessory);
                foreach (var interaction in interactions) {
                    interaction.OnSentryAI(info);
                }
            }
        }

        public static void ApplyOnShootInteractions(EntitySource_Parent source, Projectile projectile, AequusProjectile newAequusProjectile, Projectile parentProjectile) {
            if (parentProjectile.owner != Main.myPlayer || parentProjectile.hostile || !parentProjectile.sentry || !Main.player[projectile.owner].active || Main.player[parentProjectile.owner].Aequus().accSentryInheritence == null) {
                return;
            }

            var parentSentryAccessoriesGlobalProj = parentProjectile.GetGlobalProjectile<SentryAccessoriesGlobalProj>();
            AequusProjectile.pWhoAmI = projectile.whoAmI;
            AequusProjectile.pIdentity = projectile.identity;

            try {
                foreach (var accessory in AequusPlayer.GetEquips(Main.player[projectile.owner], armor: false, sentrySlot: true)) {
                    if (!TryGetInteractions(accessory.type, out var interactions)) {
                        continue;
                    }

                    SentryAccessoryInfo info = new(parentProjectile, parentSentryAccessoriesGlobalProj, accessory);
                    foreach (var interaction in interactions) {
                        interaction.OnSentryCreateProjectile(source, projectile, newAequusProjectile, info);
                    }
                }
            }
            catch {
            }

            AequusProjectile.pIdentity = -1;
            AequusProjectile.pWhoAmI = -1;
        }

        #region Mod Calls
        public static SoftSupportSentryInteraction GetSoftSupportInteraction(int itemId) {
            var list = CollectionsMarshal.GetValueRefOrAddDefault(Interactions, itemId, out _) ??= new();
            if (list.Find(i => i is SoftSupportSentryInteraction) is not SoftSupportSentryInteraction interaction) {
                interaction = new();
                list.Add(interaction);
            }
            return interaction;
        }
        #endregion
    }
}