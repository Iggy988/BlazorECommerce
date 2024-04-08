using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorECommerce.Shared;
public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImgUrl { get; set; } = string.Empty;

    public Category? Category { get; set; }
    //if we want to specify id name by ourselfs -foreign key, if we want to seed data
    public int CategoryId { get; set; }
    public bool Featured { get; set; } = false;
    public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}
