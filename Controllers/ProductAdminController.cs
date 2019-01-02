using System.Collections.Generic;
using System.Web.Mvc;
using Db;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SampleMVC.Modules.SampleMVC.Mvc;

namespace SampleMVC.Modules.SampleMVC.Controllers
{
    [DnnHandleError]
    public class ProductAdminController : DnnController
    {
        private Dbc context = new Dbc();

        [MvcModule(route: "productAdmin", displayName: "Admin - Products")]
        public ActionResult Index()
        {
            return View();
        }

        public class ProductAdminModel
        {
            public List<Product> Products { get; set; }
            public int SelectedProductId { get; set; }
        }

        public class EditProductModel
        {
            public int ProductId { get; set; }
        }
    }
}