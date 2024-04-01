using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorECommerce.Shared;
public class ProductVariant
{
    //JsonIgnore- to not get circular reference => ProductVariant will have Product, and Products will have list ProductVariant and so on
    // dont fill Product when wi recive ProductVariant
    [JsonIgnore] 
    public Product Product { get; set; }
    public int ProductId { get; set; }
    public ProductType ProductType { get; set; }
    public int ProductTypeId { get; set; }

    [Column(TypeName ="decimal(18,2)")]
    public decimal Price { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalPrice { get; set; }

}
