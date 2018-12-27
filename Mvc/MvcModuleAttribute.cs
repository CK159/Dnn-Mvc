using System;

namespace SampleMVC.Modules.SampleMVC.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]  
    public class MvcModuleAttribute : Attribute
    {
        public string Route { get; set; }
        public string DisplayName { get; set; }

        public MvcModuleAttribute(string route, string displayName)
        {
            Route = route;
            DisplayName = displayName;
        }
    }
}