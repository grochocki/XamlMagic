using System.Xml.Linq;

namespace XamlMagic.Service.Reorder
{
    public interface IProcessElementService
    {
        void ProcessElement(XElement element);
    }
}