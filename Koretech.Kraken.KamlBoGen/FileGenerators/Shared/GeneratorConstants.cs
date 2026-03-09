namespace Koretech.Kraken.KamlBoGen.FileGenerators.Shared
{
    /// <summary>
    /// Shared constants for Kraken code generation helpers.
    /// </summary>
    internal static class GeneratorConstants
    {
        public static class SqlColumnTypes
        {
            public const string DateTime = "datetime";
            public const string Int = "int";
            public const string VarBinary = "varbinary";
            public const string UniqueIdentifier = "uniqueidentifier";
            public const string Decimal = "decimal";
            public const string Float = "float";
            public const string Byte = "byte";
        }

        public static class PropertyPatterns
        {
            public const string AutoProperty = "{ get; set; }";
            public const string RequiredInitializer = " = default!;";
            public const string EmptyStringInitializer = " = string.Empty;";
        }

        public static class CommonUsings
        {
            public const string System = "using System;";
            public const string SystemCollectionsGeneric = "using System.Collections.Generic;";
            public const string SystemLinq = "using System.Linq;";
            public const string SystemThreadingTasks = "using System.Threading.Tasks;";
        }
    }
}
