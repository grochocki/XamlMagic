using Microsoft.VisualStudio.Shell;
using XamlStyler.Service.Options;

namespace XamlStyler.Plugin
{
    public class PackageOptions : DialogPage
    {
        private readonly IStylerOptions _options;

        public PackageOptions()
        {
            _options = new StylerOptions();
        }

        public override object AutomationObject
        {
            get { return _options; }
        }
    }
}