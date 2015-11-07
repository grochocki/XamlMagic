using Microsoft.VisualStudio.Shell;
using XamlMagic.Service.Options;

namespace XamlMagic.Plugin
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