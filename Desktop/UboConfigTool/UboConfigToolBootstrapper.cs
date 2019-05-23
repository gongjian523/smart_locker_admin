using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UboConfigTool.Controls;
using UboConfigTool.Modules.Logging;
using UboConfigTool.Modules.UBO;
using UboConfigTool.Modules.User;
using UboConfigTool.Modules.User.View;

namespace UboConfigTool
{
    public class UboConfigToolBootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(UboConfigToolBootstrapper).Assembly));
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Shell)this.Shell;
            Application.Current.MainWindow.Show();
        }
    }
}
