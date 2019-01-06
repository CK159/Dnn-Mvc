using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
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
        [ModuleAction(ControlKey = "Edit", Title = "Settings")]
        public ActionResult Index()
        {
            MvcMethodInfo info = GetAndValidateInfo(ActionType.Default);

            //If there is no method info, error message will be set in ViewBag
            if (info == null)
                return View("BasicError");

            //Get the target controller
            DnnController instance = GetController(info);
            //Execute action
            ActionResult result = (ActionResult) info.Method.Invoke(instance, null);
            //Set up this controller with data from target controller
            PostConfigure(info, instance);
            return result;
        }

        /// <summary>
        /// Handles the postback action for all MVC modules
        /// Finds method tagged with both [HttpPost] attribute and
        /// MvcModule attribute matching module's configured route
        /// Operates like Index() method but also handles model binding for target action
        /// </summary>
        /// <param name="whatever"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(bool? whatever)
        {
            MvcMethodInfo info = GetAndValidateInfo(ActionType.Postback);

            //If there is no method info, error message will be set in ViewBag
            if (info == null)
                return View("BasicError");

            //Get the target controller
            DnnController instance = GetController(info);

            //Model binding
            Type modelType = info.Method.GetParameters().FirstOrDefault()?.ParameterType;
            List<object> actionArguments = new List<object>();

            if (modelType != null)
            {
                object model = Activator.CreateInstance(modelType);
                //Trick from https://stackoverflow.com/a/23407917
                TryUpdateModel((dynamic) model);
                //Push any model binding errors to the target controller 
                instance.ModelState.Merge(ModelState);
                actionArguments.Add(model);
            }

            //Execute action, passing in the model if one exists
            var arr = actionArguments.ToArray();
            ActionResult result = (ActionResult) info.Method.Invoke(instance, arr);
            //Set up this controller with data from target controller
            PostConfigure(info, instance);
            return result;
        }

        private MvcMethodInfo GetAndValidateInfo(ActionType type)
        {
            string route = new MvcModuleSettings(ModuleContext).Route;

            if (route == null || route.IsEmpty())
            {
                ViewBag.message = "Module has no route specified.";
                return null;
            }

            List<MvcMethodInfo> infos = MvcModuleLoader.GetActionsForRoute(route, type);

            if (infos.Count > 1)
            {
                ViewBag.message = $"{infos.Count} actions found for {route}.";
                return null;
            }

            MvcMethodInfo info = infos.FirstOrDefault();

            if (info == null)
            {
                string typeName = type == ActionType.Postback ? "postback" : "non-postback";
                ViewBag.message = $"No {typeName} action found for {route}.";
                return null;
            }

            if (info.Type != typeof(DnnController)
                && !info.Type.IsSubclassOf(typeof(DnnController)))
            {
                ViewBag.message =
                    $"Controller for route {route} must be of type DnnController (or a derivative type). Currently returning {info.Type.Name}.";
                return null;
            }

            if (info.Method.ReturnType != typeof(ActionResult)
                && !info.Method.ReturnType.IsSubclassOf(typeof(ActionResult)))
            {
                ViewBag.message =
                    $"Route {route} must return ActionResult (or a derivative type). Currently returning {info.Method.ReturnType.Name}.";
                return null;
            }

            return info;
        }

        //Instantiate and set up target controller
        private DnnController GetController(MvcMethodInfo info)
        {
            DnnController instance = (DnnController) typeof(ViewRenderer)
                .GetMethod("CreateController")
                .MakeGenericMethod(info.Type)
                .Invoke(null, new object[] {null});

            //Set up extra context
            instance.ControllerContext.RouteData.Values["action"] = info.Method.Name;
            //DNN-specific extra context
            instance.DnnPage = DnnPage;
            instance.ModuleContext = ModuleContext;

            return instance;
        }

        //Update this controller's context data to that of the target
        //so that when the target's result is executed by this controller, the proper view can be found
        private void PostConfigure(MvcMethodInfo info, DnnController instance)
        {
            string controllerName = (string) instance.RouteData.Values["controller"];

            ControllerContext.RouteData.Values["controller"] = controllerName;
            ControllerContext.RouteData.Values["action"] = info.Method.Name;
            LocalResourceFile = String.Format("~/DesktopModules/MVC/{0}/{1}/{2}.resx",
                ModuleContext.Configuration.DesktopModule.FolderName,
                Localization.LocalResourceDirectory,
                controllerName);
        }

        public ActionResult Settings()
        {
            MvcModuleSettings settings = new MvcModuleSettings(ModuleContext);
            List<SelectListItem> routes = MvcModuleLoader.GetAllActions()
                .Where(r => !r.isPost) //Postback routes are not primary routes
                .Select(a => new SelectListItem
                {
                    Text = a.Attribute.DisplayName == ""
                        ? "Unlabeled Route: " + a.Attribute.Route
                        : a.Attribute.DisplayName,
                    Value = a.Attribute.Route
                })
                .OrderBy(s => s.Text)
                .ToList();

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