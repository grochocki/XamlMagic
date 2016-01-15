using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XamlMagic.Service.Helpers;
using XamlMagic.Service.Model;
using XamlMagic.Service.Options;
using XamlMagic.Service.Parser;
using XamlMagic.Service.Reorder;

namespace XamlMagic.Service
{
    public class StylerService : XmlEscapingService
    {
        private readonly Stack<ElementProcessStatus> ElementProcessStatusStack
            = new Stack<ElementProcessStatus>();

        private IStylerOptions Options { get; set; }

        private IList<string> NewlineExemptionElementsList { get; set; }

        private IList<string> NoNewLineMarkupExtensionsList { get; set; }

        private AttributeOrderRules OrderRules { get; set; }

        private List<IProcessElementService> ProcessElementServices { get; set; }

        private void Initialize()
        {
            ProcessElementServices = new List<IProcessElementService>
            {
                new VSMReorderService() { Mode = this.Options.ReorderVSM },
                new FormatThicknessService(this.Options.ThicknessStyle, this.Options.ThicknessAttributes),
                this.GetReorderGridChildrenService(),
                this.GetReorderCanvasChildrenService(),
                this.GetReorderSettersService()
            };
        }

        private NodeReorderService GetReorderGridChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = this.Options.ReorderGridChildren };
            reorderService.ParentNodeNames.Add(new NameSelector("Grid", null));
            reorderService.ChildNodeNames.Add(new NameSelector(null, null));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Row", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Column", null, true));
            return reorderService;
        }

        private NodeReorderService GetReorderCanvasChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = this.Options.ReorderCanvasChildren };
            reorderService.ParentNodeNames.Add(new NameSelector("Canvas", null));
            reorderService.ChildNodeNames.Add(new NameSelector(null, null));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Left", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Top", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Right", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Bottom", null, true));
            return reorderService;
        }

        private NodeReorderService GetReorderSettersService()
        {
            var reorderService = new NodeReorderService();
            reorderService.ParentNodeNames.Add(new NameSelector("DataTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("MultiDataTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("MultiTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("Style", null));
            reorderService.ParentNodeNames.Add(new NameSelector("Trigger", null));
            reorderService.ChildNodeNames.Add(new NameSelector("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));

            switch (this.Options.ReorderSetters)
            {
                case ReorderSettersBy.None:
                    reorderService.IsEnabled = false;
                    break;
                case ReorderSettersBy.Property:
                    reorderService.SortByAttributes.Add(new SortBy("Property", null, false));
                    break;
                case ReorderSettersBy.TargetName:
                    reorderService.SortByAttributes.Add(new SortBy("TargetName", null, false));
                    break;
                case ReorderSettersBy.TargetNameThenProperty:
                    reorderService.SortByAttributes.Add(new SortBy("TargetName", null, false));
                    reorderService.SortByAttributes.Add(new SortBy("Property", null, false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return reorderService;
        }

        public static StylerService CreateInstance(IStylerOptions options)
        {
            var stylerServiceInstance = new StylerService { Options = options };
            stylerServiceInstance.NewlineExemptionElementsList = stylerServiceInstance.Options.NewlineExemptionElements.ToList();
            stylerServiceInstance.NoNewLineMarkupExtensionsList = stylerServiceInstance.Options.NoNewLineMarkupExtensions.ToList();
            stylerServiceInstance.OrderRules = new AttributeOrderRules(options);
            stylerServiceInstance.ElementProcessStatusStack.Clear();
            stylerServiceInstance.ElementProcessStatusStack.Push(new ElementProcessStatus());
            return stylerServiceInstance;
        }

        private string Format(string xamlSource)
        {
            StringBuilder output = new StringBuilder();

            using (var sourceReader = new StringReader(xamlSource))
            {
                // Not used
                // var settings = new XmlReaderSettings {IgnoreComments = false};
                using (XmlReader xmlReader = XmlReader.Create(sourceReader))
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.Mixed);

                                this.ElementProcessStatusStack.Push(
                                    new ElementProcessStatus
                                    {
                                        Parent = this.ElementProcessStatusStack.Peek(),
                                        Name = xmlReader.Name,
                                        ContentType = ContentTypeEnum.None,
                                        IsMultlineStartTag = false,
                                        IsSelfClosingElement = false,
                                        IsPreservingSpace = this.ElementProcessStatusStack.Peek().IsPreservingSpace
                                    });

                                this.ProcessElement(xmlReader, output);

                                if (this.ElementProcessStatusStack.Peek().IsSelfClosingElement)
                                {
                                    this.ElementProcessStatusStack.Pop();
                                }
                                break;
                            case XmlNodeType.Text:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.SingleLineTextOnly);
                                this.ProcessTextNode(xmlReader, output);
                                break;
                            case XmlNodeType.ProcessingInstruction:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.Mixed);
                                this.ProcessInstruction(xmlReader, output);
                                break;
                            case XmlNodeType.Comment:
                                this.UpdateParentElementProcessStatus(ContentTypeEnum.Mixed);
                                this.ProcessComment(xmlReader, output);
                                break;
                            case XmlNodeType.CDATA:
                                this.ProcessCDATA(xmlReader, output);
                                break;
                            case XmlNodeType.Whitespace:
                                this.ProcessWhitespace(xmlReader, output);
                                break;
                            case XmlNodeType.SignificantWhitespace:
                                this.ProcessSignificantWhitespace(xmlReader, output);
                                break;
                            case XmlNodeType.EndElement:
                                this.ProcessEndElement(xmlReader, output);
                                this.ElementProcessStatusStack.Pop();
                                break;
                            case XmlNodeType.XmlDeclaration:
                                //ignoring xml declarations for Xamarin support
                                this.ProcessXMLRoot(xmlReader, output);
                                break;
                            default:
                                Trace.WriteLine($"Unprocessed NodeType: {xmlReader.NodeType} Name: {xmlReader.Name} Value: {xmlReader.Value}");
                                break;
                        }
                    }
                }
            }

            return output.ToString();
        }

        private void ProcessCDATA(XmlReader xmlReader, StringBuilder output)
        {
            // If there is linefeed(s) between element and CDATA then treat CDATA as element and indent accordingly, otherwise treat as single line text
            if (output.IsNewLine())
            {
                this.UpdateParentElementProcessStatus(ContentTypeEnum.MultiLineTextOnly);
                if (!this.ElementProcessStatusStack.Peek().IsPreservingSpace)
                {
                    string currentIndentString = GetIndentString(xmlReader.Depth);
                    output.Append(currentIndentString);
                }
            }
            else
            {
                this.UpdateParentElementProcessStatus(ContentTypeEnum.SingleLineTextOnly);
            }

            // All newlines are returned by XmlReader as \n due to requirements in the XML Specification (http://www.w3.org/TR/2008/REC-xml-20081126/#sec-line-ends)
            // Change them back into the environment newline characters.
            output.Append("<![CDATA[").Append(xmlReader.Value.Replace("\n", Environment.NewLine)).Append("]]>");
        }

        /// <summary>
        /// Execute styling from string input
        /// </summary>
        /// <param name="xamlSource"></param>
        /// <returns></returns>
        public string ManipulateTreeAndFormatInput(string xamlSource)
        {
            this.Initialize();

            // parse XDocument
            var xDoc = XDocument.Parse(EscapeDocument(xamlSource), LoadOptions.PreserveWhitespace);

            // first, manipulate the tree; then, write it to a string
            return this.UnescapeDocument(this.Format(this.ManipulateTree(xDoc)));
        }

        private string ManipulateTree(XDocument xDoc)
        {
            var xmlDeclaration = xDoc.Declaration?.ToString() ?? String.Empty;
            var rootElement = xDoc.Root;

            if (rootElement != null)
            {
                this.HandleNode(rootElement);
            }

            return (xmlDeclaration + xDoc);
        }

        private void HandleNode(XNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    XElement element = node as XElement;

                    if (element?.Nodes().Any() ?? false)
                    {
                        // handle children first
                        foreach (var childNode in element.Nodes())
                        {
                            this.HandleNode(childNode);
                        }
                    }

                    if (element != null)
                    {
                        foreach (var elementService in ProcessElementServices)
                        {
                            elementService.ProcessElement(element);
                        }
                    }
                    break;
            }
        }

        private string GetIndentString(int depth)
        {
            if (depth < 0)
            {
                depth = 0;
            }

            if (this.Options.IndentWithTabs)
            {
                return new String('\t', depth);
            }

            return new String(' ', (depth * this.Options.IndentSize));
        }

        private bool IsNoLineBreakElement(string elementName)
        {
            return this.NewlineExemptionElementsList.Contains<string>(elementName);
        }

        private void ProcessXMLRoot(XmlReader xmlReader, StringBuilder output)
        {
            output.Append("<?xml ").Append(xmlReader.Value.Trim()).Append(" ?>");
        }

        private void ProcessComment(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            string content = xmlReader.Value;

            if (!output.IsNewLine())
            {
                output.Append(Environment.NewLine);
            }

            if (content.Contains("<") && content.Contains(">"))
            {
                output.Append(currentIndentString);
                output.Append("<!--");
                if (content.Contains("\n"))
                {
                    output.Append(String.Join(Environment.NewLine, content.GetLines().Select(_ => _.TrimEnd(' '))));
                    if (content.TrimEnd(' ').EndsWith("\n"))
                    {
                        output.Append(currentIndentString);
                    }
                }
                else
                {
                    output.Append(content);
                }

                output.Append("-->");
            }
            else if (content.Contains("#region") || content.Contains("#endregion"))
            {
                output.Append(currentIndentString).Append("<!--").Append(content.Trim()).Append("-->");
            }
            else if (content.Contains("\n"))
            {
                output.Append(currentIndentString).Append("<!--");

                var contentIndentString = this.GetIndentString(xmlReader.Depth + 1);
                foreach (var line in content.Trim().GetLines())
                {
                    output.Append(Environment.NewLine).Append(contentIndentString).Append(line.Trim());
                }

                output.Append(Environment.NewLine).Append(currentIndentString).Append("-->");
            }
            else
            {
                output.Append(currentIndentString)
                    .Append("<!--")
                    .Append(' ', Options.CommentSpaces)
                    .Append(content.Trim())
                    .Append(' ', Options.CommentSpaces)
                    .Append("-->");
            }
        }

        public int AttributeInfoComparison(AttributeInfo x, AttributeInfo y)
        {
            if (x.OrderRule.Group != y.OrderRule.Group)
            {
                return x.OrderRule.Group.CompareTo(y.OrderRule.Group);
            }

            if (x.OrderRule.Priority != y.OrderRule.Priority)
            {
                return x.OrderRule.Priority.CompareTo(y.OrderRule.Priority);
            }

            return Options.OrderAttributesByName
                ? String.Compare(x.Name, y.Name, StringComparison.Ordinal)
                : 0;
        }

        private void ProcessElement(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            string elementName = xmlReader.Name;

            // Calculate how element should be indented
            if (!this.ElementProcessStatusStack.Peek().IsPreservingSpace)
            {
                // "Run" get special treatment to try to preserve spacing. Use xml:space='preserve' to make sure!
                if (elementName.Equals("Run"))
                {
                    this.ElementProcessStatusStack.Peek().Parent.IsSignificantWhiteSpace = true;
                    if ((output.Length == 0) || output.IsNewLine())
                    {
                        output.Append(currentIndentString);
                    }
                }
                else
                {
                    this.ElementProcessStatusStack.Peek().Parent.IsSignificantWhiteSpace = false;
                    if ((output.Length == 0) || output.IsNewLine())
                    {
                        output.Append(currentIndentString);
                    }
                    else
                    {
                        output.Append(Environment.NewLine).Append(currentIndentString);
                    }
                }
            }

            // Output the element itself
            output.Append('<').Append(xmlReader.Name);

            bool isEmptyElement = xmlReader.IsEmptyElement;
            bool hasPutEndingBracketOnNewLine = false;
            var list = new List<AttributeInfo>(xmlReader.AttributeCount);

            if (xmlReader.HasAttributes)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    string attributeName = xmlReader.Name;
                    string attributeValue = xmlReader.Value;
                    AttributeOrderRule orderRule = OrderRules.GetRuleFor(attributeName);
                    list.Add(new AttributeInfo(attributeName, attributeValue, orderRule));

                    // Check for xml:space as defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-white-space
                    if (xmlReader.IsXmlSpaceAttribute())
                    {
                        this.ElementProcessStatusStack.Peek().IsPreservingSpace = (xmlReader.Value == "preserve");
                    }
                }

                if (this.Options.EnableAttributeReordering)
                {
                    list.Sort(AttributeInfoComparison);
                }

                currentIndentString = this.GetIndentString(xmlReader.Depth);

                var noLineBreakInAttributes = (list.Count <= this.Options.AttributesTolerance)
                    || this.IsNoLineBreakElement(elementName);
                var forceLineBreakInAttributes = false;

                // Root element?
                if (this.ElementProcessStatusStack.Count == 2)
                {
                    switch (this.Options.RootElementLineBreakRule)
                    {
                        case LineBreakRule.Default:
                            break;
                        case LineBreakRule.Always:
                            noLineBreakInAttributes = false;
                            forceLineBreakInAttributes = true;
                            break;
                        case LineBreakRule.Never:
                            noLineBreakInAttributes = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // No need to break attributes
                if (noLineBreakInAttributes)
                {
                    foreach (var attrInfo in list)
                    {
                        output.Append(' ').Append(attrInfo.ToSingleLineString());
                    }

                    this.ElementProcessStatusStack.Peek().IsMultlineStartTag = false;
                }
                // Need to break attributes
                else
                {
                    IList<String> attributeLines = new List<String>();

                    var currentLineBuffer = new StringBuilder();
                    int attributeCountInCurrentLineBuffer = 0;

                    AttributeInfo lastAttributeInfo = null;
                    foreach (AttributeInfo attrInfo in list)
                    {
                        // Attributes with markup extension, always put on new line
                        if (attrInfo.IsMarkupExtension && this.Options.FormatMarkupExtension)
                        {
                            string baseIndetationString;

                            if (!this.Options.KeepFirstAttributeOnSameLine)
                            {
                                baseIndetationString = this.GetIndentString(xmlReader.Depth);
                            }
                            else
                            {
                                baseIndetationString = this.GetIndentString(xmlReader.Depth - 1)
                                    + String.Empty.PadLeft(elementName.Length + 2, ' ');
                            }

                            string pendingAppend;

                            if (this.NoNewLineMarkupExtensionsList.Contains(attrInfo.MarkupExtension))
                            {
                                pendingAppend = $" {attrInfo.ToSingleLineString()}";
                            }
                            else
                            {
                                pendingAppend = attrInfo.ToMultiLineString(baseIndetationString);
                            }

                            if (currentLineBuffer.Length > 0)
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            attributeLines.Add(pendingAppend);
                        }
                        else
                        {
                            string pendingAppend = attrInfo.ToSingleLineString();

                            bool isAttributeCharLengthExceeded = (attributeCountInCurrentLineBuffer > 0)
                                && (Options.MaxAttributeCharatersPerLine > 0)
                                && ((currentLineBuffer.Length + pendingAppend.Length)
                                    > Options.MaxAttributeCharatersPerLine);

                            bool isAttributeCountExceeded = (Options.MaxAttributesPerLine > 0)
                                && ((attributeCountInCurrentLineBuffer + 1) > Options.MaxAttributesPerLine);

                            bool isAttributeRuleGroupChanged = Options.SeparateByGroups
                                && (lastAttributeInfo != null)
                                && (lastAttributeInfo.OrderRule.Group != attrInfo.OrderRule.Group);

                            if ((currentLineBuffer.Length > 0)
                                && (forceLineBreakInAttributes
                                    || isAttributeCharLengthExceeded
                                    || isAttributeCountExceeded
                                    || isAttributeRuleGroupChanged))
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            currentLineBuffer.AppendFormat("{0} ", pendingAppend);
                            attributeCountInCurrentLineBuffer++;
                        }

                        lastAttributeInfo = attrInfo;
                    }

                    if (currentLineBuffer.Length > 0)
                    {
                        attributeLines.Add(currentLineBuffer.ToString());
                    }

                    for (int i = 0; i < attributeLines.Count; i++)
                    {
                        if ((i == 0) && this.Options.KeepFirstAttributeOnSameLine)
                        {
                            output.Append(' ').Append(attributeLines[i].Trim());

                            // Align subsequent attributes with first attribute
                            currentIndentString = GetIndentString(xmlReader.Depth - 1)
                                + String.Empty.PadLeft(elementName.Length + 2, ' ');
                            continue;
                        }
                        output.Append(Environment.NewLine).Append(currentIndentString).Append(attributeLines[i].Trim());
                    }

                    this.ElementProcessStatusStack.Peek().IsMultlineStartTag = true;
                }

                // Determine if to put ending bracket on new line
                if (this.Options.PutEndingBracketOnNewLine
                    && this.ElementProcessStatusStack.Peek().IsMultlineStartTag)
                {
                    output.Append(Environment.NewLine).Append(currentIndentString);
                    hasPutEndingBracketOnNewLine = true;
                }
            }

            if (isEmptyElement)
            {
                if (!hasPutEndingBracketOnNewLine && this.Options.SpaceBeforeClosingSlash)
                {
                    output.Append(' ');
                }
                output.Append("/>");

                this.ElementProcessStatusStack.Peek().IsSelfClosingElement = true;
            }
            else
            {
                output.Append(">");
            }
        }

        private void ProcessEndElement(XmlReader xmlReader, StringBuilder output)
        {
            if (this.ElementProcessStatusStack.Peek().IsPreservingSpace)
            {
                output.Append("</").Append(xmlReader.Name).Append(">");
            }
            else if (this.ElementProcessStatusStack.Peek().IsSignificantWhiteSpace && !output.IsNewLine())
            {
                output.Append("</").Append(xmlReader.Name).Append(">");
            }
            // Shrink the current element, if it has no content.
            // E.g., <Element>  </Element> => <Element />
            else if ((ContentTypeEnum.None == this.ElementProcessStatusStack.Peek().ContentType)
                && Options.RemoveEndingTagOfEmptyElement)
            {
                #region shrink element with no content

                output = output.TrimEnd(' ', '\t', '\r', '\n');

                int bracketIndex = output.LastIndexOf('>');
                output.Insert(bracketIndex, '/');

                if ((output[bracketIndex - 1] != '\t')
                    && (output[bracketIndex - 1] != ' ')
                    && Options.SpaceBeforeClosingSlash)
                {
                    output.Insert(bracketIndex, ' ');
                }

                #endregion shrink element with no content
            }
            else if ((ContentTypeEnum.SingleLineTextOnly == this.ElementProcessStatusStack.Peek().ContentType)
                && !this.ElementProcessStatusStack.Peek().IsMultlineStartTag)
            {
                int bracketIndex = output.LastIndexOf('>');

                string text = output.Substring(bracketIndex + 1, output.Length - bracketIndex - 1).Trim();

                output.Length = bracketIndex + 1;
                output.Append(text).Append("</").Append(xmlReader.Name).Append(">");
            }
            else
            {
                string currentIndentString = this.GetIndentString(xmlReader.Depth);

                if (!output.IsNewLine())
                {
                    output.Append(Environment.NewLine);
                }

                output.Append(currentIndentString).Append("</").Append(xmlReader.Name).Append(">");
            }
        }

        private void ProcessInstruction(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = this.GetIndentString(xmlReader.Depth);

            if (!output.IsNewLine())
            {
                output.Append(Environment.NewLine);
            }

            output.Append(currentIndentString).Append("<?Mapping ").Append(xmlReader.Value).Append(" ?>");
        }

        private void ProcessTextNode(XmlReader xmlReader, StringBuilder output)
        {
            var xmlEncodedContent = xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true);
            if (this.ElementProcessStatusStack.Peek().IsPreservingSpace)
            {
                output.Append(xmlEncodedContent.Replace("\n", Environment.NewLine));
            }
            else
            {
                string currentIndentString = this.GetIndentString(xmlReader.Depth);
                IEnumerable<String> textLines = xmlEncodedContent.Trim()
                    .Split('\n')
                    .Where(_ => (_.Trim().Length > 0))
                    .ToList();

                foreach (var line in textLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.Length > 0)
                    {
                        output.Append(Environment.NewLine).Append(currentIndentString).Append(trimmedLine);
                    }
                }
            }

            if (xmlEncodedContent.Any(_ => (_ == '\n')))
            {
                this.UpdateParentElementProcessStatus(ContentTypeEnum.MultiLineTextOnly);
            }
        }

        private void ProcessWhitespace(XmlReader xmlReader, StringBuilder output)
        {
            var hasNewline = xmlReader.Value.Contains('\n');

            if (this.ElementProcessStatusStack.Peek().IsSignificantWhiteSpace && hasNewline)
            {
                this.ElementProcessStatusStack.Peek().IsSignificantWhiteSpace = false;
            }

            if (hasNewline && !this.ElementProcessStatusStack.Peek().IsPreservingSpace)
            {
                // For WhiteSpaces contain linefeed, trim all spaces/tab，
                // since the intent of this whitespace node is to break line,
                // and preserve the line feeds
                output.Append(xmlReader.Value
                    .Replace(" ", "")
                    .Replace("\t", "")
                    .Replace("\r", "")
                    .Replace("\n", Environment.NewLine));
            }
            else
            {
                // Preserve "pure" WhiteSpace between elements
                // e.g.,
                //   <TextBlock>
                //     <Run>A</Run> <Run>
                //      B
                //     </Run>
                //  </TextBlock>
                output.Append(xmlReader.Value.Replace("\n", Environment.NewLine));
            }
        }

        private void ProcessSignificantWhitespace(XmlReader xmlReader, StringBuilder output)
        {
            output.Append(xmlReader.Value.Replace("\n", Environment.NewLine));
        }

        private void UpdateParentElementProcessStatus(ContentTypeEnum contentType)
        {
            ElementProcessStatus parentElementProcessStatus = this.ElementProcessStatusStack.Peek();
            parentElementProcessStatus.ContentType |= contentType;
        }
    }
}