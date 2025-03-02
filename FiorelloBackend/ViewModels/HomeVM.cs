using FiorelloBackend.Models;
using System.Collections.Generic;

namespace FiorelloBackend.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<BlogVM> Blogs { get; set; }
        public IEnumerable<CategoryVM> Categories { get; set; }
        public IEnumerable<ProductVM> Products { get; set; }
        

    }
}
