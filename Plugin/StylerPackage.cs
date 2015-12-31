using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XamlMagic.Service;
using XamlMagic.Service.Options;

namespace XamlMagic.Plugin
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(PackageOptions), "XAML Magic", "General", 101, 106, true)]
    [ProvideProfile(typeof(PackageOptions), "XAML Magic", "XAML Magic Settings", 106, 107, true, DescriptionResourceID = 108)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid(GuidList.XamlMagicPackagePackageString)]
    public sealed class StylerPackage : Package
    {
        private DTE dte;
        private Events events;
        private CommandEvents fileSaveAll;
        private CommandEvents fileSaveSelectedItems;
        private IVsUIShell uiShell;

        public StylerPackage()
        {
            // Put any initialization code that does not require VS service
            // At this point, the package is created but not inside VS environment
            // Put all other initialization in Initialize()
            Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
        }

        protected override void Initialize()
        {
            /// Initialization of the package; this method is called right after the package is sited, so this is the place
            /// where you can put all the initialization code that rely on services provided by VisualStudio.
            Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
            base.Initialize();

            this.dte = GetService(typeof (DTE)) as DTE;

            if (this.dte == null)
            {
                throw new NullReferenceException("DTE is null");
            }

            this.uiShell = GetService(typeof (IVsUIShell)) as IVsUIShell;

            // Initialize command events listeners
            this.events = this.dte.Events;

            // File.SaveSelectedItems command
            this.fileSaveSelectedItems
                = this.events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 331];
            this.fileSaveSelectedItems.BeforeExecute += this.OnFileSaveSelectedItemsBeforeExecute;

            // File.SaveAll command
            this.fileSaveAll = this.events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 224];
            this.fileSaveAll.BeforeExecute += this.OnFileSaveAllBeforeExecute;

            //Initialize menu command
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var menuCommandService = this.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;

            if (menuCommandService != null)
            {
                // Create the command for the menu item.
                var menuCommandId = new CommandID(GuidList.XamlMagicPackageCommandSet,
                    (int) PkgCmdIDList.FormatXamlCommand);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandId);
                menuCommandService.AddCommand(menuItem);
            }
        }

        private bool IsFormatableDocument(Document document)
        {
            bool isFormatableDocument;
            isFormatableDocument = (!document.ReadOnly && (document.Language == "XAML"));

            if (!isFormatableDocument)
            {
                //xamarin
                isFormatableDocument = (document.Language == "XML")
                    && document.FullName.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase);
            }

            return isFormatableDocument;
        }

        private void OnFileSaveSelectedItemsBeforeExecute(
            string guid,
            int id,
            object customIn,
            object customOut,
            ref bool cancelDefault)
        {
            Document document = this.dte.ActiveDocument;

            if (this.IsFormatableDocument(document))
            {
                var options = GetDialogPage(typeof (PackageOptions)).AutomationObject as IStylerOptions;

                if (options.BeautifyOnSave)
                {
                    this.Execute(document);
                }
            }
        }

        private void OnFileSaveAllBeforeExecute(
            string guid,
            int id,
            object customIn,
            object customOut,
            ref bool cancelDefault)
        {
            // use parallel processing, but only on the documents that are formatable 
            // (to avoid the overhead of Task creating when it's not necessary)

            List<Document> docs = new List<Document>();
            foreach (Document document in this.dte.Documents)
            {
                if (this.IsFormatableDocument(document))
                {
                    docs.Add(document);
                }
            }

            Parallel.ForEach(docs, document =>
            {
                var options = GetDialogPage(typeof(PackageOptions)).AutomationObject as IStylerOptions;
                if (options.BeautifyOnSave)
                {
                    this.Execute(document);
                }
            });
        }

        private void Execute(Document document)
        {
            if (!this.IsFormatableDocument(document))
            {
                return;
            }

            Properties xamlEditorProps = this.dte.Properties["TextEditor", "XAML"];

            var stylerOptions = GetDialogPage(typeof (PackageOptions)).AutomationObject as IStylerOptions;

            stylerOptions.IndentSize = Int32.Parse(xamlEditorProps.Item("IndentSize").Value.ToString());
            stylerOptions.IndentWithTabs = (bool) xamlEditorProps.Item("InsertTabs").Value;

            StylerService styler = StylerService.CreateInstance(stylerOptions);

            var textDocument = (TextDocument) document.Object("TextDocument");

            TextPoint currentPoint = textDocument.Selection.ActivePoint;
            int originalLine = currentPoint.Line;
            int originalOffset = currentPoint.LineCharOffset;

            EditPoint startPoint = textDocument.StartPoint.CreateEditPoint();
            EditPoint endPoint = textDocument.EndPoint.CreateEditPoint();

            string xamlSource = startPoint.GetText(endPoint);
            xamlSource = styler.ManipulateTreeAndFormatInput(xamlSource);

            startPoint.ReplaceText(endPoint, xamlSource, 0);

            if (originalLine <= textDocument.EndPoint.Line)
            {
                textDocument.Selection.MoveToLineAndOffset(originalLine, originalOffset);
            }
            else
            {
                textDocument.Selection.GotoLine(textDocument.EndPoint.Line);
            }
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                this.uiShell.SetWaitCursor();
                Document document = this.dte.ActiveDocument;

                if (this.IsFormatableDocument(document))
                {
                    this.Execute(document);
                }
            }
            catch (Exception ex)
            {
                var title = $"Error in {GetType().Name}:";
                var message = String.Format(
                    CultureInfo.CurrentCulture,
                    "{0}\r\n\r\nIf this deems a malfunctioning of styler, please kindly submit an issue at https://github.com/grochocki/XamlMagic.",
                    ex.Message);

                this.ShowMessageBox(title, message);
            }
        }

        private void ShowMessageBox(string title, string message)
        {
            Guid clsid = Guid.Empty;
            int result;

            this.uiShell.ShowMessageBox(
                0,
                ref clsid,
                title,
                message,
                String.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0, // false
                out result);
        }
    }
}