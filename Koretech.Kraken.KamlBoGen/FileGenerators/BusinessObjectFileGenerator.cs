using Koretech.Kraken.KamlBoGen.KamlBoModel;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class BusinessObjectFileGenerator : FileGenerator
    {
        private const string businessObjectsPath = "BusinessObjects";

        public BusinessObjectFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = businessObjectsPath;
        }

        /// <summary>
        /// Creates the subdirectory for storing BO files in a specific domain if it doesn't already exist.
        /// </summary>
        public void CreateDomainSubdirectory(KamlBoDomain domain)
        {
            outputRootDirectory.GetDirectories(businessObjectsPath).Single().CreateSubdirectory(domain.Name);
        }

        /// <summary>
        /// Generates the two classes that make up a BO based on a KAML BO specification.
        /// </summary>
        /// <param name="domain">root of the model of the KAML BO specification, i.e. the domain</param>
        protected override void DoGenerate(KamlBoDomain domain)
        {
            foreach (KamlBoEntity entity in domain.Entities)
            {
                string businessObjectName = entity.Name;
                string entityName = $"{entity.Name}Entity";
                string domainName = domain.Name;
                string entityFullPath = Path.Combine(outputRootDirectory.FullName, generatePath);
                string domainFullPath = Path.Combine(entityFullPath, domainName);

                CreateBaseClassFile(entity, domainName, entityName, businessObjectName, domainFullPath);
                CreatePartialClassFile(entity, domainName, entityName, businessObjectName, domainFullPath);
            }
        }

        private void CreateBaseClassFile(KamlBoEntity entity, string domainName, string entityName, string businessObjectName, string domainFullPath)
        {
            string domainPackageName = domainName + "s";
            string className = $"{businessObjectName}Base";
            string sourceFileName = Path.Combine(domainFullPath, $"{businessObjectName}Base_Gen.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();

            // Usings
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.Entities;");
            writer.WriteLine("using Koretech.Platform.BusinessObjects;");
            writer.WriteLine("using System.Collections;");
            if (entity.HasCrossDomainRelationship)
            {
                List<string> targetDomains = new();
                foreach (KamlEntityRelation relation in entity.Relations.Where(r => r.IsCrossDomain))
                {
                    if (relation.TargetDomain != null && !targetDomains.Contains(relation.TargetDomain))
                    {
                        string targetDomainPackageName = relation.TargetDomain + "s";
                        writer.WriteLine($"using Koretech.Domains.{targetDomainPackageName}.BusinessObjects;");
                        targetDomains.Add(relation.TargetDomain);
                    }
                }
                writer.WriteLine();
            }
            writer.WriteLine();

            writer.WriteLine($"namespace Koretech.Domains.{domainPackageName}.BusinessObjects");
            writer.WriteLine("{");
            writer.WriteLine("\t/// <summary>");
            writer.WriteLine($"\t/// This business object class wraps the domain entity {entityName} and provides access to the entity's data");
            writer.WriteLine("\t/// through accessor properties.  It also provides a place for business logic related to the domain entity.");
            writer.WriteLine("\t/// </summary>");
            writer.WriteLine($"\tpublic abstract class {className} : BusinessObject");
            writer.WriteLine("\t{");
            writer.WriteLine();
            writer.WriteLine($"\t\tprivate {entityName} _entity;");
            writer.WriteLine();
            writer.WriteLine("\t\t/// <summary>");
            writer.WriteLine("\t\t/// Constructor.  Protected to force use of the static factory method NewInstance().");
            writer.WriteLine("\t\t/// </summary>");
            writer.WriteLine("\t\t/// <param name=\"entity\">An entity that provides data for the business object</param>");
            writer.WriteLine($"\t\tprotected {className}({entityName} entity)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_entity = entity;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic {className}()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_entity = new();");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\t/// <summary>");
            writer.WriteLine("\t\t/// Initializes a new instance of the business object class.");
            writer.WriteLine($"\t\t/// Override when you need to do work in the {businessObjectName}(entity) constructor.");
            writer.WriteLine("\t\t/// </summary>");
            writer.WriteLine("\t\tprotected virtual void Initialize() { }");
            writer.WriteLine();
            writer.WriteLine($"\t\tinternal {entityName} Entity");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tget => _entity;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();

            // Properties
            writer.WriteLine("\t\t#region Entity Properties");
            writer.WriteLine();
            foreach (KamlEntityProperty property in entity.Properties)
            {
                string clrType = SqlType.GetClrTypeName(property.DataType);
                string nullableChar = property.IsRequired ? string.Empty : "?";
                writer.WriteLine($"\t\tpublic virtual {clrType}{nullableChar} {property.Name}");
                writer.WriteLine("\t\t{");
                writer.WriteLine($"\t\t\tget => _entity.{property.Name};");
                writer.WriteLine($"\t\t\tset => _entity.{property.Name} = value;");
                writer.WriteLine("\t\t}");
                writer.WriteLine();
            }
            writer.WriteLine("\t\t#endregion Entity Properties");
            writer.WriteLine();

            // Relationships
            writer.WriteLine("\t\t#region Relationships");
            writer.WriteLine();
            foreach (KamlEntityRelation relationship in entity.Relations)
            {
                if (relationship.IsToMany || relationship.IsToOwnerMany)
                {
                    writer.WriteLine($"\t\tpublic List<{relationship.TargetEntity}> {relationship.Name} = new();");
                }
                else if (relationship.IsToOne || relationship.IsToOwnerOne)
                {
                    writer.WriteLine($"\t\tpublic {relationship.TargetEntity}? {relationship.Name};");
                }
                writer.WriteLine();
            }
            writer.WriteLine("\t\t#endregion Relationships");

            writer.WriteLine();
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }

        /// <summary>
        /// Generates a partial BO class from a KAML BO specification.
        /// </summary>
        /// <param name="entity">KAML BO specification</param>
        public void CreatePartialClassFile(KamlBoEntity entity, string domainName, string entityName, string businessObjectName, string domainFullPath)
        {
            string domainPackageName = domainName + "s";
            string className = $"{businessObjectName}";
            string sourceFileName = Path.Combine(domainFullPath, $"{businessObjectName}_Gen.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();

            // Usings
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.Entities;");
            writer.WriteLine("using System.Collections;");
            if (entity.HasCrossDomainRelationship)
            {
                List<string> targetDomains = new();
                foreach (KamlEntityRelation relation in entity.Relations.Where(r => r.IsCrossDomain))
                {
                    if (relation.TargetDomain != null && !targetDomains.Contains(relation.TargetDomain))
                    {
                        string targetDomainPackageName = relation.TargetDomain + "s";
                        writer.WriteLine($"using Koretech.Domains.{targetDomainPackageName}.BusinessObjects;");
                        targetDomains.Add(relation.TargetDomain);
                    }
                }
                writer.WriteLine();
            }
            writer.WriteLine();

            writer.WriteLine($"namespace Koretech.Domains.{domainPackageName}.BusinessObjects");
            writer.WriteLine("{");
            writer.WriteLine("\t/// <summary>");
            writer.WriteLine($"\t/// This business object class wraps the domain entity {entityName} and provides access to the entity's data");
            writer.WriteLine("\t/// through accessor properties.  It also provides a place for business logic related to the domain entity.");
            writer.WriteLine("\t/// </summary>");
            writer.WriteLine($"\tpublic partial class {className} : {className}Base");
            writer.WriteLine("\t{");

            // Static methods
            writer.WriteLine("\t\t#region Static Methods");
            writer.WriteLine();
            writer.WriteLine("\t\t/// <summary>");
            writer.WriteLine("\t\t/// Creates a business object from an entity.  Any child entities");
            writer.WriteLine("\t\t/// will also have business objects created for them and attached to the parent business");
            writer.WriteLine("\t\t/// object.  This will result in a business object tree that mirrors the entity tree.");
            writer.WriteLine("\t\t/// </summary>");
            writer.WriteLine("\t\t/// <param name=\"entity\">The entity to create a business object from</param>");
            writer.WriteLine("\t\t/// <returns>A newly created business object wrapping the provided entity</returns>");
            writer.WriteLine($"\t\tpublic static {businessObjectName} NewInstance({entityName} entity)");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\t{businessObjectName} businessObject = new(entity);");
            writer.WriteLine();
            writer.WriteLine("\t\t\t// Recursively create business objects from the entities that have relationships with this one");
            writer.WriteLine("\t\t\t// and link to them through the relationship properties in this class.");
            foreach (KamlEntityRelation relationship in entity.Relations)
            {
                writer.WriteLine();
                writer.WriteLine($"\t\t\tif (entity.{relationship.Name} != null)");
                writer.WriteLine("\t\t\t{");
                string targetBOName = relationship.TargetEntity;
                string disambiguationNamespace = !relationship.IsCrossDomain ? "BusinessObjects." : string.Empty;
                if (relationship.IsToMany || relationship.IsToOwnerMany)
                {
                    writer.WriteLine($"\t\t\t\tIList<{targetBOName}> new{targetBOName} = {disambiguationNamespace}{targetBOName}.NewInstance(entity.{relationship.Name});");
                    writer.WriteLine($"\t\t\t\tbusinessObject.{relationship.Name}.AddRange(new{targetBOName});");
                }
                else if (relationship.IsToOne || relationship.IsToOwnerOne)
                {
                    writer.WriteLine($"\t\t\t\tbusinessObject.{relationship.Name} = {disambiguationNamespace}{targetBOName}.NewInstance(entity.{relationship.Name});");
                }
                writer.WriteLine("\t\t\t}");
            }
            writer.WriteLine();
            writer.WriteLine("\t\t\treturn businessObject;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\t/// <summary>");
            writer.WriteLine("\t\t/// Creates business objects from a set of entities by wrapping each entity.  Any child entities");
            writer.WriteLine("\t\t/// will also have business objects created for them and attached to the appropriate parent business");
            writer.WriteLine("\t\t/// object.  This will result in a business object tree that mirrors the entity tree.");
            writer.WriteLine("\t\t/// </summary>");
            writer.WriteLine("\t\t/// <param name=\"entities\">The entities to create business objects from</param>");
            writer.WriteLine("\t\t/// <returns>A newly created business object(s) wrapping the provided entities</returns>");
            writer.WriteLine($"\t\tpublic static IList<{businessObjectName}> NewInstance(IList<{entityName}> entities)");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\tList<{businessObjectName}> businessObjects = new();");
            writer.WriteLine();
            writer.WriteLine($"\t\t\tforeach ({entityName} entity in entities)");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine($"\t\t\t\t{businessObjectName} newBusinessObject = new(entity);");
            writer.WriteLine("\t\t\t\tbusinessObjects.Add(newBusinessObject);");
            writer.WriteLine();
            writer.WriteLine("\t\t\t\t// Recursively create business objects from the entities that have relationships with this one");
            writer.WriteLine("\t\t\t\t// and link to them through the relationship properties in this class.");
            foreach (KamlEntityRelation relationship in entity.Relations)
            {
                writer.WriteLine();
                writer.WriteLine($"\t\t\t\tif (entity.{relationship.Name} != null)");
                writer.WriteLine("\t\t\t\t{");
                string targetBOName = relationship.TargetEntity;
                string disambiguationNamespace = !relationship.IsCrossDomain ? "BusinessObjects." : string.Empty;
                if (relationship.IsToMany || relationship.IsToOwnerMany)
                {
                    writer.WriteLine($"\t\t\t\t\tIList<{targetBOName}> new{targetBOName} = {disambiguationNamespace}{targetBOName}.NewInstance(entity.{relationship.Name});");
                    writer.WriteLine($"\t\t\t\t\tnewBusinessObject.{relationship.Name}.AddRange(new{targetBOName});");
                }
                else if (relationship.IsToOne || relationship.IsToOwnerOne)
                {
                    writer.WriteLine($"\t\t\t\t\tnewBusinessObject.{relationship.Name} =  {disambiguationNamespace}{targetBOName}.NewInstance(entity.{relationship.Name});");
                }
                writer.WriteLine("\t\t\t\t}");
            }
            writer.WriteLine("\t\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\t\treturn businessObjects;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\t#endregion Static Methods");
            writer.WriteLine();
            writer.WriteLine("\t\t/// <summary>");
            writer.WriteLine("\t\t/// Create a new instance of this class by wrapping an entity.");
            writer.WriteLine("\t\t/// </summary>");
            writer.WriteLine("\t\t/// <param name=\"entity\">the entity to wrap</param>");
            writer.WriteLine($"\t\tprotected {businessObjectName}({entityName} entity) : base(entity)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tInitialize();");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }
    }
}
