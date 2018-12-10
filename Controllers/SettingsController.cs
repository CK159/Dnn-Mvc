/*
' Copyright (c) 2018 Sample
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Collections;
using System.Web.Mvc;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;

namespace SampleMVC.Modules.SampleMVC.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Settings()
        {
            var settings = new Models.Settings();
            settings.Namespace = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("SampleMVC_MVC_Namespace", "SampleMVC.Modules.SampleMVC.Controllers");
            settings.Controller = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("SampleMVC_MVC_Controller", "ItemController");
            settings.Method = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("SampleMVC_MVC_Method", "Index");

            return View(settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportsTokens"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Models.Settings settings)
        {
            ModuleContext.Configuration.ModuleSettings["SampleMVC_MVC_Namespace"] = settings.Namespace;
            ModuleContext.Configuration.ModuleSettings["SampleMVC_MVC_Controller"] = settings.Controller;
            ModuleContext.Configuration.ModuleSettings["SampleMVC_MVC_Method"] = settings.Method;

            return RedirectToDefaultRoute();
        }
    }
}