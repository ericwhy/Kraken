namespace Koretech.Kraken.KamlBoGen
{
    internal class SqlType
    {
        #region Static Members

        public static readonly SqlType Boolean = new() { TypeName = "boolean", ClrTypeName = "bool" };
        public static readonly SqlType Bytes = new() { TypeName = "bytes", ClrTypeName = "byte[]" };
        public static readonly SqlType Character = new() { TypeName = "character", ClrTypeName = "string" };
        public static readonly SqlType Date = new() { TypeName = "date", ClrTypeName = "DateTime" };
        public static readonly SqlType DateTime = new() { TypeName = "datetime", ClrTypeName = "DateTime" };
        public static readonly SqlType Decimal = new() { TypeName = "decimal", ClrTypeName = "decimal" };
        public static readonly SqlType Double = new() { TypeName = "double", ClrTypeName = "double" };
        public static readonly SqlType Integer = new() { TypeName = "integer", ClrTypeName = "int" };
        public static readonly SqlType Money = new() { TypeName = "money", ClrTypeName = "decimal" };
        public static readonly SqlType UniqueIdentifier = new() { TypeName = "uniqueidentifier", ClrTypeName = "Guid" };
        public static readonly SqlType NCharacter = new() { TypeName = "ncharacter", ClrTypeName = "string" };
        public static readonly SqlType YesNo = new() { TypeName = "yesno", ClrTypeName = "char" };

        /// <param name="sqlTypeName">Name of a SQL data type</param>
        /// <returns>The SqlType instance with the given name</returns>
        public static SqlType GetSqlType(string sqlTypeName)
        {
            return SqlTypesByName[sqlTypeName];
        }

        /// <param name="sqlTypeName">Name of a SQL data type</param>
        /// <returns>The name of the CLR data type corresponding to the given SQL type</returns>
        public static string GetClrTypeName(string sqlTypeName)
        {
            return SqlTypesByName[sqlTypeName].ClrTypeName;
        }

        /// <summary>
        /// Allows lookup of SqlTypes by the name of a SQL data type.
        /// </summary>
        private static readonly Dictionary<string, SqlType> SqlTypesByName = new()
        {
            { Boolean.TypeName, Boolean },
            { Bytes.TypeName, Bytes },
            { Character.TypeName, Character },
            { Date.TypeName, Date },
            { DateTime.TypeName, DateTime },
            { Decimal.TypeName, Decimal },
            { Double.TypeName, Double },
            { Integer.TypeName, Integer },
            { Money.TypeName, Money },
            { NCharacter.TypeName, NCharacter },
            { UniqueIdentifier.TypeName, UniqueIdentifier },
            { YesNo.TypeName, YesNo }
        };

        #endregion Static Members

        /// <summary>
        /// Name of the SQL data type
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Name of the CLR data type associated with the SQL data type
        /// </summary>
        public string ClrTypeName { get; private set; }
    }
}
