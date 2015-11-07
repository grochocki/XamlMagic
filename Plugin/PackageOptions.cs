using Microsoft.VisualStudio.Shell;
using XamlMagic.Service.Options;

namespace XamlMagic.Plugin
{
    public class PackageOptions : DialogPage
    {
        private readonly IStylerOptions options;

        public PackageOptions()
        {
            this.options = new StylerOptions();
        }

        public override object AutomationObject
        {
            get { return this.options; }
        }
    }
}