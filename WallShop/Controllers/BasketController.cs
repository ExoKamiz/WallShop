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
            if (ModelState.IsValid)
            {
                _db.Basket.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index"); //перенаправление исполнение кода в метод Index
            }
            return View(obj);
        }

        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)               //проверка есть ли значени в нашем id
            {
                return NotFound();
            }
            var obj = _db.Basket.Find(id);      //получаем Category из базы данных используя Find(работает только с полем имеющим первичный ключ) для искомого id

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Basket obj) //тут получаем обьект который нужно добавить в бд
        {
            if (ModelState.IsValid)               //проверяет выполнены ли все правила которые вы определили для своей модели; валидация со стороны сервера
            {
                _db.Basket.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index"); //перенаправление исполнение кода в метод Index
            }
            return View(obj);
        }


        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)               //проверка есть ли значени в нашем id
            {
                return NotFound();
            }
            var obj = _db.Basket.Find(id);      //получаем обьект Category из базы данных используя Find(работает только с полем имеющим первичный ключ) для искомого id

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id) //тут получаем обьект который нужно добавить в бд
        {
            var obj = _db.Basket.Find(id);
            if (obj == null)                      //тут не нужна валидация, а только проверка определен ли обьект
            {
                return NotFound();
            }
            _db.Basket.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index"); //перенаправление исполнение кода в метод Index
        }
    }
}


