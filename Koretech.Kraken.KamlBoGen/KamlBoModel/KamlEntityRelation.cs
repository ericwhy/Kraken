using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koretech.Kraken.KamlBoGen.KamlBoModel
{
    /// <summary>
    /// Represents a BO relationship configuration taken from a kamlbo file.
    /// </summary>
    public class KamlEntityRelation
    {
        public string Name { get; set; }
        public string TargetEntity { get; set; }
        public string? TargetDomain { get; set; }
        public Dictionary<string, string> KeyMap { get; private set; }
        public bool IsToMany { get; set; }
        public bool IsToOne { get; set; }
        public bool IsToOwnerMany { get; set; }
        public bool IsToOwnerOne { get; set; }

        public bool IsCrossDomain => !string.IsNullOrEmpty(TargetDomain);

        /// <summary>
        /// Indicates whether this is a relationship from an owned type to its owning type.
        /// </summary>
        public bool IsToOwner
        {
            get
            {
                return IsToOwnerMany || IsToOwnerOne;
            }
        }

        public KamlEntityRelation(string myName, string myTargetEntity)
        {
            Name = myName;
            TargetEntity = myTargetEntity;
            KeyMap = new Dictionary<string, string>();
            IsToMany = false;
            IsToOne = false;
            IsToOwnerMany = false;
            IsToOwnerOne = false;
        }
    }
}
