using System.ComponentModel.DataAnnotations;

namespace FiorelloBackend.Areas.Admin.ViewModels.Category
{
    public class CategoryEditVM
    {
        [Required]
        public string Name { get; set; }
    }
}
