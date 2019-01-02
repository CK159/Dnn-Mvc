using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Db;
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using SampleMVC.Modules.SampleMVC.Components;
using SampleMVC.Modules.SampleMVC.Mvc;

namespace SampleMVC.Modules.SampleMVC.Controllers
{
    public class CartController : DnnController
    {
        private Dbc context = new Dbc();
        
        [MvcModule(route: "cart", displayName: "Frontend - Cart")]
        public ActionResult Index()
        {
            TabController tc = new TabController();
            
            List<CartItemVm> cart = (from c in Cart.Get(Session)
                join Product p in context.Products on c.ProductId equals p.ProductId
                select new CartItemVm
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Quantity = c.Quantity,
                    ProductDesc = p.ProductDesc,
                    ProductDetailUrl = Globals.NavigateURL(tc.GetTabByName("Product Details", ModuleContext.PortalId).TabID,"",  $"id={c.ProductId}")
                }).ToList();
            
            CartVm model = new CartVm
            {
                Cart = cart,
                ProductListUrl = Globals.NavigateURL(tc.GetTabByName("Products", ModuleContext.PortalId).TabID)
            };
            
            return View(model);
        }

        public class CartVm
        {
            public List<CartItemVm> Cart { get; set; }
            public string ProductListUrl { get; set; }
        }

        public class CartItemVm
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductDesc { get; set; }
            public int Quantity { get; set; }
            public string ProductDetailUrl { get; set; }
        }
    }
}