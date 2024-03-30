using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorECommerce.Shared;
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    //localhost:7118/products/category/video-game/
    public string Url { get; set; } = string.Empty;
}
