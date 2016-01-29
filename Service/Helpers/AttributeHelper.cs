using System.Xml.Linq;

namespace XamlMagic.Service.Helpers
{
	internal static class AttributeHelper
	{
		/// <summary>
		/// Gets a properly configured XName object from an XAttribute.
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static XName GetName(XAttribute attribute)
		{
			if(attribute.Value.Contains(":"))
			{
				string[] parts = attribute.Value.Split(':');

				if (parts.Length == 2)
				{
					string prefix = parts[0];
					string value = parts[1];

					var ns = attribute.Parent.GetNamespaceOfPrefix(prefix);

					return XName.Get(value, ns.NamespaceName);
				}
			}

			return XName.Get(attribute.Value);
		}
	}
}
