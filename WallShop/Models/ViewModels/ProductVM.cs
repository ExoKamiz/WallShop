using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WallShop.Models.ViewModels
{
    public class ProductVM
    {
        //создаем ProductVM исходя из ранее имеющихся в нашем контроллере полей Product product и IEnumerable<SelectListItem>
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}
