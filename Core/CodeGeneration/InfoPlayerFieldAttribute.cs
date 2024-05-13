using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aequus.Core.CodeGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal class InfoPlayerFieldAttribute(string Name, string Type) : Attribute {
}
