using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WallShop.Models
{
    public class Product
    {
        [Key]

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        public string Image { get; set; }

        //создаем внешний ключ с идентификатором сущности Category

        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }                         //внешний ключ между таблицами Product и Category, связывает их сущности
        [ForeignKey("CategoryId")]                                  //атрибут аннотации данных связывающий сущности; внешний ключ, как атрибут добляем к обьекту Category
        public virtual Category Category { get; set; }              // entity framefork создает автоматически связь между Product и Category
    }
}
