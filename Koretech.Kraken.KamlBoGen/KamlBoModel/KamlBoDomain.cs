using System.Xml.Linq;

namespace Koretech.Kraken.KamlBoGen.KamlBoModel
{

    /// <summary>
    /// Represents the domain configuration taken from a kamlbo file.
    /// </summary>
    public class KamlBoDomain
    {
        private List<KamlBoEntity> entities = new List<KamlBoEntity>();
        private KamlBoEntity? primaryEntity;

        public string Name { get; }
        public string PrimaryEntityName { get; }
        
        public List<KamlBoEntity> Entities => (entities.Count > 0) ? entities : throw new InvalidOperationException("KamlBoDomain.Entities not initialized.  Entities must be added before calling this property.");
        public KamlBoEntity PrimaryEntity => primaryEntity ?? throw new InvalidOperationException("KamlBoDomain.PrimaryEntity not initialized.  Entities must be added before calling this property.");

        public KamlBoDomain(string name, string primaryEntityName)
        {
            Name = name;
            PrimaryEntityName = primaryEntityName;
        }

        public void AddBoEntity(KamlBoEntity entity)
        {
            entities.Add(entity);
            if (entity.Name.Equals(PrimaryEntityName, StringComparison.OrdinalIgnoreCase))
            {
                primaryEntity = entity;
            }
        }

        public bool Validate()
        {
            if (primaryEntity == null)
            {  
                return false; 
            }
            foreach (KamlBoEntity entity in entities) 
            { 
                if (!entity.Validate())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Factory method.  Creates an instance from an XML element.
        /// </summary>
        /// <param name="domainEl">XML element representing the domain</param>
        /// <returns>a new instance</returns>
        static public KamlBoDomain ParseFromXElement(XElement domainEl)
        {
            string name = domainEl.Attribute("Name")?.Value ?? "?_?";
            string primaryEntityName = domainEl.Attribute("PrimaryObject")?.Value ?? name; // If no primary entity is specified, assume the domain name is the primary entity name.
            var domain = new KamlBoDomain(name, primaryEntityName);
            return domain;
        }
    }
}