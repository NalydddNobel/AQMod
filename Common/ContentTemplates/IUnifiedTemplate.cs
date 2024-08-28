using System.Collections.Generic;

namespace Aequus.Common.ContentTemplates;

internal interface IUnifiedTemplate : IModType {
    List<ModType> ToLoad { get; init; }
}
