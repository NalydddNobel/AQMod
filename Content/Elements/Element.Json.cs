using AequusRemake.DataSets.Json;
using AequusRemake.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AequusRemake.Content.Elements;

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
