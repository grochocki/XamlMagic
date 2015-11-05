using System.Linq;
using XamlStyler.Service.Reorder;

namespace XamlStyler.Service.Model
{
    public sealed class AttributeOrderRule
    {
        public Wildcard Name { get; }
        public int Group { get; }
        public int Priority { get; }
        public int MatchScore { get; }

        public AttributeOrderRule(string name, int group, int priority)
        {
            this.Name = new Wildcard(name);
            this.Group = group;
            this.Priority = priority;

            // Calculate match score. 1=no wildcards 0:contains ? -1:contains *
            this.MatchScore = name.Any(_ => (_ == '*'))
                ? -1
                : name.Any(_ => _ == '?')
                    ? 0
                    : 1;
        }
    }
}