using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SampleMVC.Modules.SampleMVC.Controllers;

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
        [ModuleAction(ControlKey = "Edit", Title = "Settings")]
        public ActionResult Index()
        {
            string route = new MvcModuleSettings(ModuleContext).Route;
            
            if (route == null || route.IsEmpty())
            {
                ViewBag.message = "Module has no route specified";
                return View("BasicError");
            }

            List<MvcMethodInfo> infos = MvcModuleLoader.GetActionsForRoute(route);

            if (infos.Count > 1)
            {
                ViewBag.message = $"{infos.Count} actions found for {route}";
                return View("BasicError");
            }
            
            MvcMethodInfo info = infos.FirstOrDefault();

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

            return InvokeMvcAction(info);
        }

        //TODO: Guess this needs to be a generic action invoker with model binder since DNN does not appear to allow altering the default post action of MVC Modules...
        [HttpPost]
        public ActionResult Index(ProductController.AddToCartVm item)
        {
            //ViewBag.message = $"ProductId: {item.ProductId} Quantity: {item.Quantity}";
            //return View("BasicError");
            
            string controllerName = "product";
            string actionName = "productdetail";
            
            ProductController pc = ViewRenderer.CreateController<ProductController>();
            pc.ControllerContext.RouteData.Values["action"] = actionName;
            pc.DnnPage = DnnPage;
            pc.ModuleContext = ModuleContext;
            
            ControllerContext.RouteData.Values["controller"] = controllerName;
            ControllerContext.RouteData.Values["action"] = actionName;
            
            return pc.ProductDetail(item);
        }

        protected ActionResult InvokeMvcAction(MvcMethodInfo info)
        {
            //Get the controller
            DnnController instance = (DnnController)typeof(ViewRenderer)
                .GetMethod("CreateController")
                .MakeGenericMethod(info.Type)
                .Invoke(null, new object[]{null});

            string controllerName = (string)instance.RouteData.Values["controller"];
            string actionName = info.Method.Name;
            
            //Set up extra context
            instance.ControllerContext.RouteData.Values["action"] = actionName;
            //DNN-specific extra context
            instance.DnnPage = DnnPage;
            instance.ModuleContext = ModuleContext;

            //Execute action
            ViewResult result = (ViewResult)info.Method.Invoke(instance, null);
            
            //Update this controller's context data to that of the target
            //so that when the target's result is executed by this controller, the proper view can be found
            ControllerContext.RouteData.Values["controller"] = controllerName;
            ControllerContext.RouteData.Values["action"] = actionName;
            LocalResourceFile = String.Format("~/DesktopModules/MVC/{0}/{1}/{2}.resx",
                ModuleContext.Configuration.DesktopModule.FolderName,
                Localization.LocalResourceDirectory,
                controllerName);
            
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