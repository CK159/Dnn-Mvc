using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Db;
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SampleMVC.Modules.SampleMVC.Mvc;

namespace SampleMVC.Modules.SampleMVC.Controllers
{
    [DnnHandleError]
    public class ProductController : DnnController
    {
        private Dbc context = new Dbc();
        
        [MvcModule(route: "productList", displayName: "Frontend - Product List")]
        public ActionResult ProductList()
        {
            ProductListModel model = new ProductListModel
            {
                Products = context.Products.OrderBy(p => p.ProductName).ToList(),
                Message = "Todo: show message",
                DetailTabId = new TabController().GetTabByName("Product Details", ModuleContext.PortalId).TabID
            };
            
            return View(model);
        }

        public class ProductListModel
        {
            public List<Product> Products { get; set; }
            public string Message { get; set; }
            public int DetailTabId { get; set; }
        }
        
        [MvcModule(route: "productDetail", displayName: "Frontend - Product Detail")]
        public ActionResult ProductDetail()
        {
            return View();
        }

        public class ProductDetailModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductDesc { get; set; }
            public string ProductRichDesc { get; set; }
            public bool Active { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}