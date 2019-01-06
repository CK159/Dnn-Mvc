using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetNuke.Web.Mvc.Framework.Controllers;

namespace SampleMVC.Modules.SampleMVC.Mvc
{
    public static class MvcModuleLoader
    {
        public static List<MvcMethodInfo> GetActionsForRoute(string route)
        {
            IEnumerable<MvcMethodInfo> target = GetAllActions()
                .Where(a => a.Attribute.Route == route);

            return target.ToList();
        }

        public static List<MvcMethodInfo> GetAllActions()
        {
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<string> cfgAssemblies = MvcModuleConfig.GetAssembliesList();
            
            var activeAssemblies = allAssemblies.Where(s => cfgAssemblies.Contains(s.GetName().Name));

            var attrMethods = activeAssemblies
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