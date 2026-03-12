using System.Text;
using Koretech.Tools.KamlBoModel.Model;

namespace Koretech.Tools.KrakenGenerator.FileGenerators.Shared
{
    /// <summary>
    /// Provides reusable code-generation snippets for Kraken generators.
    /// </summary>
    internal class CodeSnippetBuilder
    {
        private readonly PropertyTypeMapper _typeMapper;

        public CodeSnippetBuilder(PropertyTypeMapper typeMapper)
        {
            _typeMapper = typeMapper ?? throw new ArgumentNullException(nameof(typeMapper));
        }

        public string PropertyDeclaration(
            KamlBoEntityProperty property,
            string? defaultValue = null,
            string? comment = null)
        {
            ArgumentNullException.ThrowIfNull(property);

            string fullType = _typeMapper.GetFullClrType(property);
            string initializer = _typeMapper.GetDefaultInitializer(property, defaultValue);

            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(comment))
            {
                builder.AppendLine($"\t\t// {comment}");
            }

            builder.Append($"\t\tpublic {fullType} {property.Name} {GeneratorConstants.PropertyPatterns.AutoProperty}{initializer}");
            return builder.ToString();
        }

        public string SimpleProperty(string type, string name, bool isRequired, string? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(type));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            string nullableSuffix = isRequired ? string.Empty : "?";
            string initializer = defaultValue ?? (isRequired ? GeneratorConstants.PropertyPatterns.RequiredInitializer : string.Empty);
            return $"\t\tpublic {type}{nullableSuffix} {name} {GeneratorConstants.PropertyPatterns.AutoProperty}{initializer}";
        }

        public string CollectionNavigation(string targetEntityName, string propertyName, string? comment = null)
        {
            if (string.IsNullOrWhiteSpace(targetEntityName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetEntityName));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));
            }

            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(comment))
            {
                builder.AppendLine($"\t\t// {comment}");
            }

            builder.Append($"\t\tpublic ICollection<{targetEntityName}> {propertyName} {{ get; set; }} = new List<{targetEntityName}>();");
            return builder.ToString();
        }

        public string ReferenceNavigation(string targetEntityName, string propertyName, string? comment = null)
        {
            if (string.IsNullOrWhiteSpace(targetEntityName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetEntityName));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));
            }

            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(comment))
            {
                builder.AppendLine($"\t\t// {comment}");
            }

            builder.Append($"\t\tpublic {targetEntityName}? {propertyName} {{ get; set; }} = null;");
            return builder.ToString();
        }

        public string ParameterList(IEnumerable<Parameter> parameters, bool includeOptionalDefaults = false)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            List<string> parameterList = new();
            foreach (Parameter parameter in parameters)
            {
                string parameterString = $"{parameter.Type} {parameter.Name}";
                if (includeOptionalDefaults && parameter.DefaultValue != null)
                {
                    parameterString += $" = {parameter.DefaultValue}";
                }

                parameterList.Add(parameterString);
            }

            return string.Join(", ", parameterList);
        }

        public string Field(string type, string name, bool isReadonly = true, string? initializer = null)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(type));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            string readonlyModifier = isReadonly ? "readonly " : string.Empty;
            string initialization = string.IsNullOrEmpty(initializer) ? string.Empty : $" = {initializer}";
            return $"\t\tprivate {readonlyModifier}{type} {name}{initialization};";
        }

        public string Using(string namespaceName)
        {
            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(namespaceName));
            }

            return $"using {namespaceName};";
        }

        public string Usings(params string[] namespaces)
        {
            var builder = new StringBuilder();
            foreach (string namespaceName in namespaces)
            {
                builder.AppendLine(Using(namespaceName));
            }

            return builder.ToString();
        }

        public string FieldAssignment(string fieldName, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldName));
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(parameterName));
            }

            return $"\t\t\t{fieldName} = {parameterName};";
        }

        public string NullCheck(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));
            }

            return $"\t\t\t_ = {variableName} ?? throw new ArgumentNullException(nameof({variableName}));";
        }

        public string XmlSummary(string summary, int indentLevel = 1)
        {
            if (string.IsNullOrWhiteSpace(summary))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(summary));
            }

            string indent = new('\t', indentLevel);
            var builder = new StringBuilder();
            builder.AppendLine($"{indent}/// <summary>");

            string[] lines = summary.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                builder.AppendLine($"{indent}/// {line.Trim()}");
            }

            builder.Append($"{indent}/// </summary>");
            return builder.ToString();
        }

        public string XmlParam(string paramName, string description, int indentLevel = 1)
        {
            if (string.IsNullOrWhiteSpace(paramName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(paramName));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));
            }

            string indent = new('\t', indentLevel);
            return $"{indent}/// <param name=\"{paramName}\">{description}</param>";
        }
    }

    internal class Parameter
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string? DefaultValue { get; set; }

        public Parameter(string type, string name, string? defaultValue = null)
        {
            Type = type;
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}
