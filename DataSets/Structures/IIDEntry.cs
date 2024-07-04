namespace AequusRemake.DataSets.Structures;

public interface IIDEntry {
    /// <summary>Name for the content entry.</summary>
    public string Name { get; }
    /// <summary>ID for the content.</summary>
    public int Id { get; }
    /// <summary>Whether this is a valid entry. Entries for content from other mods will be marked as Invalid if the mod is not enabled.</summary>
    bool ValidEntry { get; }
    /// <summary>Whether this is a vanilla entry.</summary>
    bool VanillaEntry { get; }

    /// <summary>Set <see cref="Id"/> using <paramref name="name"/>. Called when loading the .json metadata files.</summary>
    /// <param name="name">Name of the content.</param>
    void SetName(string name);

    /// <summary>Set <see cref="Id"/> directly. Use this to update the name or other values if needed.</summary>
    /// <param name="id">The Content's Id.</param>
    void SetId(int id);
}
