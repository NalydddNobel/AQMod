using Aequu2.DataSets.Json;
using Aequu2.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequu2.Content.Elements;

public partial class Element {
    private class JSON(Element Element) : IJsonHolder {
        [JsonIgnore]
        private readonly Element _element = Element;

        [JsonProperty]
        public Dictionary<IDEntry<ItemID>, bool> Items => _element._manualItems;

        [JsonIgnore]
        public string FilePath => $"Elements/{_element.Name}";
    }
}
