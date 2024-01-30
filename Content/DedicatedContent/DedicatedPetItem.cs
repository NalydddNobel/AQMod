using Aequus.Content.Pets;

namespace Aequus.Content.DedicatedContent;

[Autoload(false)]
internal class DedicatedPetItem : InstancedPetItem, IDedicatedItem {
    private readonly System.String _dedicateeName;
    private readonly System.String _displayedDedicateeName;
    private readonly Color _dedicateeColor;

    public DedicatedPetItem(ModPet modPet, System.String dedicateeName, Color dedicateeColor, System.Boolean nameHidden = false, System.Int32 itemRarity = 3, System.Int32 value = 50000, Color? alphaOverride = null) : base(modPet, itemRarity, value, alphaOverride) {
        _dedicateeName = dedicateeName;
        _displayedDedicateeName = nameHidden ? null : dedicateeName;
        _dedicateeColor = dedicateeColor;
    }

    public System.String DedicateeName => _dedicateeName;
    public System.String DisplayedDedicateeName => _displayedDedicateeName;
    public Color TextColor => _dedicateeColor;
}
