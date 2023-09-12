using ReLogic.Reflection;

namespace Aequus.Common.DataSets {
    public struct ContentFileInfo {
        public readonly bool HasContentFile;
        public readonly IdDictionary idDictionary;

        public ContentFileInfo(IdDictionary idDictionary) {
            HasContentFile = true;
            this.idDictionary = idDictionary;
        }
    }
}
