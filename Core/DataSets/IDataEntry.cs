namespace Aequus.Core.DataSets;

public interface IDataEntry {
    string Name { get; set; }

    bool ValidEntry { get; }
    bool VanillaEntry { get; }

    void Initialize();
}

public interface IDataEntry<T> : IDataEntry {
    T Id { get; set; }
}