using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public string ProductRichDesc { get; set; }
        //public virtual ProductType Type { get; set; }
        public bool Active { get; set; }
        [Required, Column(TypeName = "datetime2"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }
        //public virtual ICollection<ProductResource> ProductResources { get; set; }
    }
}