using System;
using System.Collections.Generic;
using System.Linq;
using XamlMagic.Service.Options;

namespace XamlMagic.Service.Model
{
    public sealed class AttributeOrderRules
    {
        private readonly IList<AttributeOrderRule> Rules;

        public AttributeOrderRules(IStylerOptions options)
        {
            this.Rules = new List<AttributeOrderRule>();

            var groupIndex = 1;
            foreach (var @group in options.AttributeOrderingRuleGroups)
            {
                if (!String.IsNullOrWhiteSpace(@group))
                {
                    int priority = 1;

                    string[] names = @group.Split(',')
                        .Where(_ => !String.IsNullOrWhiteSpace(_))
                        .Select(_ => _.Trim())
                        .ToArray();

                    foreach (var name in names)
                    {
                        this.Rules.Add(new AttributeOrderRule(name, groupIndex, priority));
                        priority++;
                    }
                }
                groupIndex++;
            }

            // Add catch all group at the end ensuring we always get a match;
            this.Rules.Add(new AttributeOrderRule("*", groupIndex, 0));
        }

        public AttributeOrderRule GetRuleFor(string attributeName)
        {
            return this.Rules
                .Where(_ => _.Name.IsMatch(attributeName))
                .OrderByDescending(_ => _.MatchScore)
                .FirstOrDefault();
        }
    }
}