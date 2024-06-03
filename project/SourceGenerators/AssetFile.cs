namespace SourceGenerators;

public class AssetFile {
    public string Name;
    public string Path;

    public AssetFile(string path) { 
        Path = path;
        Name = path.Substring(path.LastIndexOf('/') + 1);
    }
}