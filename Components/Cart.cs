using System;
using System.Collections.Generic;
using System.Web;

namespace SampleMVC.Modules.SampleMVC.Components
{
    public class Cart : List<CartItem>
    {
        private HttpSessionStateBase _session;
        
        public static Cart Get(HttpSessionStateBase session)
        {
            Cart c = (Cart) session["SampleMVC_Cart"] ?? new Cart();
            c._session = session;
            return c;
        }

        public void Save()
        {
            _session["SampleMVC_Cart"] = this;
        }
    }

    public class CartItem
    {
        public Guid CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public CartItem()
        {
            CartId = Guid.NewGuid();
        }
    }
}