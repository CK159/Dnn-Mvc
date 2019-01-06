using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Framework.Controllers;

namespace SampleMVC.Modules.SampleMVC.Mvc
{
    public class MvcMethodInfo
    {
        /// <summary>
        /// The type of the controller
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// The type of the action
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// The MvcModuleAttribute of the action
        /// </summary>
        public MvcModuleAttribute Attribute { get; set; }
        /// <summary>
        /// Whether this is a postback action (with [HttpPost] attribute) or not
        /// </summary>
        public bool isPost { get; set; }
    }

    public enum ActionType
    {
        All,
        Default, //Default route - non-postback, no [HttpPost] attribute
        Postback //Postback action for a route - add [HttpPost] attribute to method
    }
    
    public static class MvcModuleLoader
    {
        public static List<MvcMethodInfo> GetActionsForRoute(string route, ActionType actionType)
        {
            IEnumerable<MvcMethodInfo> target = GetAllActions()
                .Where(a => a.Attribute.Route == route);

            if (actionType == ActionType.Default)
                target = target.Where(t => !t.isPost);
            else if (actionType == ActionType.Postback)
                target = target.Where(t => t.isPost);

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
                        .GetCustomAttributes(typeof(MvcModuleAttribute), false).FirstOrDefault(),
                    isPost = m.GetCustomAttributes(typeof(HttpPostAttribute), false).Any()
                }))
                .Where(x => x.Attribute != null); //Only methods with MvcModuleAttribute

            return attrMethods.ToList();
        }
    }
}