using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using XamlMagic.Service.Options;

namespace XamlMagic.Service.Reorder
{
    public sealed class VSMReorderService : IProcessElementService
    {
        public readonly NameSelector VSMNode = new NameSelector("VisualStateManager.VisualStateGroups", null);

        public VisualStateManagerRule Mode { get; set; } = VisualStateManagerRule.None;

        public void ProcessElement(XElement element)
        {
            if (this.Mode == VisualStateManagerRule.None)
            {
                return;
            }

            if (!element.HasElements)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine(element.Name.ToString());

            if (this.VSMNode.IsMatch(element.Name))
            {
                if (element.Parent != null)
                {
                    // Reorder the parent of any element that contains a VSM.
                    this.ReorderChildNodes(element.Parent);
                }
            }
        }

        /// <summary>
        /// Move VSM to last child of element.
        /// </summary>
        /// <param name="element">Element that is getting its VSM moved to the end.</param>
        private void ReorderChildNodes(XElement element)
        {
            List<NodeCollection> nodeCollections = new List<NodeCollection>();
            NodeCollection vsmNodeCollection = new NodeCollection();

            var children = element.Nodes();
            bool hasCollectionBeenAdded = false;

            NodeCollection currentNodeCollection = null;

            foreach (var child in children)
            {
                if (currentNodeCollection == null)
                {
                    currentNodeCollection = new NodeCollection();
                }

                currentNodeCollection.Nodes.Add(child);

                if (child.NodeType == XmlNodeType.Element)
                {
                    if (this.VSMNode.IsMatch(((XElement)child).Name))
                    {
                        // Extract VSM for adding to end.
                        vsmNodeCollection = currentNodeCollection;
                        hasCollectionBeenAdded = true;
                    }
                    else if (!hasCollectionBeenAdded)
                    {
                        // Maintain all other nodes.
                        nodeCollections.Add(currentNodeCollection);
                        hasCollectionBeenAdded = true;
                    }

                    currentNodeCollection = null;
                    hasCollectionBeenAdded = false;
                }
            }

            var newNodes = (this.Mode == VisualStateManagerRule.Last)
                ? nodeCollections.SelectMany(_ => _.Nodes).Concat(vsmNodeCollection.Nodes)
                : vsmNodeCollection.Nodes.Concat(nodeCollections.SelectMany(_ => _.Nodes));

            element.ReplaceNodes(newNodes);
        }
    }
}
