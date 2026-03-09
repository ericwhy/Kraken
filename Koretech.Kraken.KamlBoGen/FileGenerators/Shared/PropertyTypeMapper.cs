using Koretech.Kraken.KamlBoModel.Model;

namespace Koretech.Kraken.KamlBoGen.FileGenerators.Shared
{
    /// <summary>
    /// Maps KAML BO property metadata to CLR and SQL generation types.
    /// </summary>
    internal class PropertyTypeMapper
    {
        public string GetClrTypeName(KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            if (string.IsNullOrWhiteSpace(property.DataType))
            {
                throw new InvalidOperationException($"Property '{property.Name}' has no DataType defined.");
            }

            string dataType = property.DataType.ToLowerInvariant();

            if ((dataType == "integer" || dataType == "int") && property.Length > 10)
            {
                return "long";
            }

            return dataType switch
            {
                "boolean" or "bool" => "bool",
                "byte" or "tinyint" => "byte",
                "bytes" or "varbinary" => "byte[]",
                "character" or "char" or "ncharacter" or "varchar" => "string",
                "date" or "datetime" => "DateTime",
                "decimal" or "numeric" or "money" => "decimal",
                "double" or "float" => dataType == "float" ? "float" : "double",
                "integer" or "int" => "int",
                "smallint" => "short",
                "uniqueidentifier" or "guid" => "Guid",
                "yesno" => "char",
                _ => SqlType.GetClrTypeName(property.DataType)
            };
        }

        public string GetNullableSuffix(KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);
            return property.IsRequired ? string.Empty : "?";
        }

        public string GetFullClrType(KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);
            return $"{GetClrTypeName(property)}{GetNullableSuffix(property)}";
        }

        public string GetDefaultInitializer(KamlBoEntityProperty property, string? defaultValue = null)
        {
            ArgumentNullException.ThrowIfNull(property);

            if (defaultValue != null)
            {
                return IsStringType(property)
                    ? $" = \"{defaultValue}\";"
                    : $" = {defaultValue};";
            }

            if (!property.IsRequired)
            {
                return string.Empty;
            }

            string dataType = property.DataType?.ToLowerInvariant() ?? string.Empty;

            if (IsStringType(property))
            {
                return GeneratorConstants.PropertyPatterns.EmptyStringInitializer;
            }

            if (dataType == "date" || dataType == "datetime")
            {
                return " = DateTime.Now;";
            }

            if (dataType == "bytes" || dataType == "varbinary")
            {
                int length = property.Length > 0 ? property.Length : 1;
                return $" = new byte[{length}];";
            }

            return GeneratorConstants.PropertyPatterns.RequiredInitializer;
        }

        public bool IsStringType(KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            string dataType = property.DataType?.ToLowerInvariant() ?? string.Empty;
            return dataType is "character" or "char" or "ncharacter" or "varchar";
        }

        public bool IsNumericType(KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            string dataType = property.DataType?.ToLowerInvariant() ?? string.Empty;
            return dataType is "integer" or "int" or "smallint" or "decimal" or "numeric" or "money" or "float" or "double" or "byte" or "tinyint";
        }

        public string? GetEfColumnType(KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            string dataType = property.DataType?.ToLowerInvariant() ?? string.Empty;

            if (dataType is "date" or "datetime")
            {
                return GeneratorConstants.SqlColumnTypes.DateTime;
            }

            if (dataType is "integer" or "int")
            {
                return property.Length > 10 ? "bigint" : GeneratorConstants.SqlColumnTypes.Int;
            }

            if (dataType == "smallint")
            {
                return GeneratorConstants.SqlColumnTypes.Int;
            }

            if (dataType is "uniqueidentifier" or "guid")
            {
                return GeneratorConstants.SqlColumnTypes.UniqueIdentifier;
            }

            if (dataType is "bytes" or "varbinary")
            {
                return GeneratorConstants.SqlColumnTypes.VarBinary;
            }

            if (dataType is "byte" or "tinyint")
            {
                return GeneratorConstants.SqlColumnTypes.Byte;
            }

            if (dataType is "float" or "double")
            {
                return GeneratorConstants.SqlColumnTypes.Float;
            }

            if (dataType is "decimal" or "numeric" or "money")
            {
                return GeneratorConstants.SqlColumnTypes.Decimal;
            }

            return null;
        }

        public string GetValueProvidedCondition(string clrType, string valueExpression)
        {
            if (string.IsNullOrWhiteSpace(clrType))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(clrType));
            }

            if (string.IsNullOrWhiteSpace(valueExpression))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(valueExpression));
            }

            if (clrType.EndsWith("?", StringComparison.Ordinal))
            {
                return $"{valueExpression} != null";
            }

            return clrType switch
            {
                "int" or "Int32" or "long" or "Int64" or "short" or "Int16" or "byte" or "Byte" => $"{valueExpression} > 0",
                "decimal" or "Decimal" or "double" or "Double" or "float" or "Single" => $"{valueExpression} > 0",
                "string" or "String" => $"!string.IsNullOrEmpty({valueExpression})",
                "bool" or "Boolean" => "true",
                "Guid" => $"{valueExpression} != Guid.Empty",
                _ => $"{valueExpression} != null"
            };
        }
    }
}
