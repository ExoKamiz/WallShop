using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallShop.Data;
using WallShop.Models;

namespace WallShop.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db; //дает доступ к базе данных в обекте _db
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category; //создаем список типа IEnumerable обьектов Category и присваем обьект для получения катигории из нашей бд
            return View(objList);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj) //тут получаем обьект который нужно добавить в бд
        {
            _db.Category.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index"); //перенаправление исполнение кода в метод Index
        }
    }
}
