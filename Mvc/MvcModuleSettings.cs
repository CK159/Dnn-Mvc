using DotNetNuke.Collections;
using DotNetNuke.UI.Modules;

namespace SampleMVC.Modules.SampleMVC.Mvc
{
    public class MvcModuleSettings
    {
        public string Route { get; set; } = "";

        public MvcModuleSettings()
        {
        }

        public MvcModuleSettings(ModuleInstanceContext context)
        {
            LoadSettings(context);
        }

        public virtual void LoadSettings(ModuleInstanceContext context)
        {
            Route = context.Configuration.ModuleSettings
                .GetValueOrDefault("SampleMVC_MvcModule_Route", "");
        }
    }
}