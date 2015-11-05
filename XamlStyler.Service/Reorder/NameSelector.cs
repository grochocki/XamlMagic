using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XamlStyler.Service.Reorder
{
    public class NameSelector
    {
        private string name;
        private Regex nameRegex;
        private string namespaceName;
        private Regex namespaceRegex;

        [DisplayName("Name")]
        [Description("Match name by name. null/empty = all. 'DOS' Wildcards permitted.")]
        public string Name
        { 
            get { return this.name; }
            set
            {
                this.name = value;
                this.nameRegex = (this.name != null) ? new Wildcard(this.name) : null;
            } 
        }

        [DisplayName("Namespace")]
        [Description("Match name by namespace. null/empty = all. 'DOS' Wildcards permitted.")]
        public string Namespace
        { 
            get { return this.namespaceName; }
            set
            {
                this.namespaceName = value;
                this.namespaceRegex = (this.namespaceName != null) ? new Wildcard(this.namespaceName) : null;
            } 
        }

        public NameSelector() { }

        public NameSelector(string name)
        {
            this.Name = name;
        }

        public NameSelector(string name, string @namespace)
        {
            this.Name = name;
            this.Namespace = @namespace;
        }

        public bool IsMatch(XName name)
        {
            if (!this.nameRegex?.IsMatch(name.LocalName) ?? false)
            {
                return false;
            }

            if (!this.namespaceRegex?.IsMatch(name.Namespace.NamespaceName) ?? false)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return (this.Namespace != null ? $"{this.Namespace}:" : null) + this.Name;
        }
    }
}