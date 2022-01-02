using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WallShop.Models
{
    public class Category
    {
        [Key]       
        public int Id { get; set; }
        [Required]                       //обязательное заполнение этого поля 
        public string Name { get; set; }

        [DisplayName("Display Order")]  //для разделения слова
        [Required]                       //обязательное заполнение этого поля 
        [Range(1,int.MaxValue,ErrorMessage ="Display order must be greater than 0")] //задаем что DisplayOrder не будет меньше 0
        public int DisplayOrder { get; set; }
    }
}
