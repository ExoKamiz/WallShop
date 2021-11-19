using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallShop.Data;
using WallShop.Models;

namespace WallShop.Controllers
{
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BasketController(ApplicationDbContext db)
        {
            _db = db; //дает доступ к базе данных в обекте _db
        }

        public IActionResult Index()
        {
            IEnumerable<Basket> objList = _db.Basket; //создаем список типа IEnumerable обьектов Category и присваем обьект для получения катигории из нашей бд
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
        public IActionResult Create(Basket obj) //тут получаем обьект который нужно добавить в бд
        {
            _db.Basket.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index"); //перенаправление исполнение кода в метод Index
        }
    }
}

