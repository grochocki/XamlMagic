using System.Xml.Linq;

namespace XamlStyler.Service.Reorder
{
    public interface IProcessElementService
    {
        void ProcessElement(XElement element);
    }
}