using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetNuke.Web.Mvc.Framework.Controllers;

namespace SampleMVC.Modules.SampleMVC.Mvc
{
    public static class MvcModuleLoader
    {
        //TODO: Load from configs
        private static readonly List<string> MvcAssemblies = new List<string>
        {
            "SampleMVC"  
        };
        
        public static List<MvcMethodInfo> GetActionsForRoute(string route)
        {
            //TODO: Load target assembly namespaces from web.config
            IEnumerable<MvcMethodInfo> target = GetAllActions()
                .Where(a => a.Attribute.Route == route);

            return target.ToList();
        }

        public static List<MvcMethodInfo> GetAllActions()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(s => MvcAssemblies.Contains(s.GetName().Name));

            var attrMethods = assemblies
                .SelectMany(b => b.GetTypes().Where(t => t.IsSubclassOf(typeof(DnnController))))
                .SelectMany(t => t.GetMethods().Select(m => new MvcMethodInfo
                {
                    Type = t,
                    Method = m,
                    Attribute = (MvcModuleAttribute)m
                        .GetCustomAttributes(typeof(MvcModuleAttribute), false).FirstOrDefault()
                }))
                .Where(x => x.Attribute != null); //Only methods with MvcModuleAttribute

            return attrMethods.ToList();
        }
    }

    public class MvcMethodInfo
    {
        public Type Type { get; set; }
        public MethodInfo Method { get; set; }
        public MvcModuleAttribute Attribute { get; set; }
    }
}