using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using DotNetNuke.Web.Mvc.Framework.Controllers;

namespace SampleMVC.Modules.SampleMVC.Mvc
{
    /// <summary>
    /// Base controller of configurable MVC module
    /// </summary>
    public class MvcModuleController : DnnController
    {
        /// <summary>
        /// Default route for all MVC modules point here.
        /// Finds method tagged with MvcModule attribute matching module's configured route.
        /// Creates controller and invokes action.
        /// Reconfigures this controller to have target controller's context data.
        /// Reconfigured context data allows the resultant ActionResult to be executed/rendered by framework as normal.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string route = new MvcModuleSettings(ModuleContext).Route;
            
            if (route == null || route.IsEmpty())
            {
                ViewBag.message = "Module has no route specified";
                return View("BasicError");
            }

            MvcMethodInfo info = MvcModuleLoader.GetAction(route);

            if (info == null)
            {
                ViewBag.message = $"No action found for {route}";
                return View("BasicError");
            }

            if (info.Type != typeof(DnnController)
                && !info.Type.IsSubclassOf(typeof(DnnController)))
            {
                ViewBag.message = $"Controller for route {route} must be of type DnnController (or a derivative type). Currently returning {info.Type.Name}";
                return View("BasicError");
            }

            if (info.Method.ReturnType != typeof(ActionResult) 
                && !info.Method.ReturnType.IsSubclassOf(typeof(ActionResult)))
            {
                ViewBag.message = $"Route {route} must return ActionResult (or a derivative type). Currently returning {info.Method.ReturnType.Name}";
                return View("BasicError");
            }

            return Process(info);
        }

        protected ActionResult Process(MvcMethodInfo info)
        {
            //Get the controller
            DnnController instance = (DnnController)typeof(ViewRenderer)
                .GetMethod("CreateController")
                .MakeGenericMethod(info.Type)
                .Invoke(null, new object[]{null});

            //Set up extra context
            instance.ControllerContext.RouteData.Values["action"] = info.Method.Name;
            //DNN-specific context
            instance.ModuleContext = ModuleContext;

            //Execute action
            ViewResult result = (ViewResult)info.Method.Invoke(instance, null);
            
            //Update this controller's context data to that of the target
            //so that when the target's result is executed by this controller, the proper view can be found
            ControllerContext.RouteData.Values["controller"] = instance.RouteData.Values["controller"];
            ControllerContext.RouteData.Values["action"] = instance.RouteData.Values["action"];
            
            return result;
        }
        
        public ActionResult Settings()
        {
            MvcModuleSettings settings = new MvcModuleSettings(ModuleContext);
            List<SelectListItem> routes = MvcModuleLoader.GetAllActions().Select(a => new SelectListItem
            {
                Text = a.Attribute.DisplayName,
                Value = a.Attribute.Route
            }).OrderBy(s => s.Text).ToList();
            
            //Unknown or no longer available route selected. Preserve selection.
            if (settings.Route != "" && routes.All(r => r.Value != settings.Route))
                routes.Insert(0, new SelectListItem
                {
                    Text = $"Unknown Route: {settings.Route}",
                    Value = settings.Route
                });
            
            MvcSettingsModel model = new MvcSettingsModel
            {
                Routes = routes,
                Settings = new MvcModuleSettings(ModuleContext)
            };
            return View("BaseSettings", model);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(MvcModuleSettings settings)
        {
            //TODO: Generic saving method that creates settings object appropriate for target route
            //Uses shared settings view with partial view containing controller-specific values
            ModuleContext.Configuration.ModuleSettings["SampleMVC_MvcModule_Route"] = settings.Route;

            return RedirectToRoute("Settings");
        }
    }

    public class MvcSettingsModel
    {
        public List<SelectListItem> Routes { get; set; }
        public MvcModuleSettings Settings { get; set; }
    }
}