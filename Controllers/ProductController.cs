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
        
        #region # ProductList #
        
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
        
        #endregion
        
        #region # ProductDetail #
        
        [MvcModule(route: "productDetail", displayName: "Frontend - Product Detail")]
        public ActionResult ProductDetail()
        {
            string idStr = Request.QueryString["id"] ?? "";
            int productId;
            if (int.TryParse(idStr, out productId))
            {
                ProductDetailModel model = LoadProductDetail(productId);

                if (model != null)
                    return View(model);
            }

            if (productId > 0)
                ViewBag.Message = $"Product with Id: {productId} not found.";
            else
                ViewBag.Message = "No Product Id provided.";
            return View("BasicError");
        }

        private ProductDetailModel LoadProductDetail(int productId)
        {
            if (productId <= 0)
                return null;

            string backUrl =
                Globals.NavigateURL(new TabController().GetTabByName("Products", ModuleContext.PortalId).TabID);

            return (from p in context.Products
                where p.ProductId == productId
                select new ProductDetailModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDesc = p.ProductDesc,
                    ProductRichDesc = p.ProductRichDesc,
                    Active = p.Active,
                    DateCreated = p.DateCreated,
                    BackUrl = backUrl
                }).FirstOrDefault();
        }
        
        public class ProductDetailModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductDesc { get; set; }
            public string ProductRichDesc { get; set; }
            public bool Active { get; set; }
            public DateTime DateCreated { get; set; }
            public string BackUrl { get; set; }
        }
        
        #endregion
    }
}