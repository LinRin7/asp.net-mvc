using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "商品名稱")]
        public string Name { get; set; }

        [Display(Name = "商品介紹")]
        public string Description { get; set; }

        //[DataType(DataType.Currency)]//貨幣
        //[Column(TypeName = "decimal(8, 2)")]
        [Display(Name = "金額")]
        public decimal Price { get; set; }

        [Display(Name = "類型")]
        public string Category { get; set; }

        [Display(Name = "發售日")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "品牌")]
        public string Brand { get; set; }

        public string Picture { get; set; }

        //除Id以外的值 賦值
        public void setValue(Product product)
        {
            this.Name = product.Name;
            this.Description = product.Description;
            this.Price = product.Price;
            this.Category = product.Category;
            this.ReleaseDate = product.ReleaseDate;
            this.Brand = product.Brand;
            this.Picture = product.Picture;
        }
    }
}
