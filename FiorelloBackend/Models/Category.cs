using FiorelloBackend.Helpers.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiorelloBackend.Models
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = ValidationMessages.InputRequired)]
        [MaxLength(10)]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
