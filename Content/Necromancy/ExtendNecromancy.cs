namespace Aequus.Content.Necromancy;
public static class ExtendNecromancy {
    public static void DefaultToNecromancy(this Item item, int timeBetweenShots) {
        item.useTime = timeBetweenShots;
        item.useAnimation = timeBetweenShots;
        item.useStyle = ItemUseStyleID.Shoot;
        item.DamageType = Aequus.NecromancyClass;
        item.noMelee = true;
    }
}
