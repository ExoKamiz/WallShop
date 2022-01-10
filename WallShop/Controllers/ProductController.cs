using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallShop.Data;
using WallShop.Models;

namespace WallShop.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db; //дает доступ к базе данных в обекте _db
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product; //создаем список типа IEnumerable обьектов Product и присваем обьект для получения катигории из нашей бд

            foreach(var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId); //из всех имеющихся сущностей Product будет извлечена и присвоенна модель Category на основе этого условия;
                                                                                         //в этом условии(FirstOrDefault) будет работать принцип - сколько бы записей не излвлекалось, обьекту Category
                                                                                         //будет присвоенна только первая
            };

            return View(objList);
        }

        //GET - UPSERT
        public IActionResult Upsert(int? id)      //универсальный метод для создания и редактирования; если запрос на редактирование, то int, если на создание, то null(поэтому свойство может не иметь значения)
        {
            Product product = new Product();      //создаем новый обьект product типа Product 
            if(id == null)                        //и проверяем что, если id которое мы получили не имеет значения, то поступил запрос на создание новой сущности
            {
                return View(product);             //переход на представление со ссылкой на модель
            }
            else                                  //если id имеет значение 
            {
                product = _db.Product.Find(id);   //получаем Product из базы данных используя Find     
                if(id == null)
                {
                    return NotFound();
                }
                return View(product);
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category obj) //тут получаем обьект который нужно добавить в бд
        {
            if (ModelState.IsValid)               //проверяет выполнены ли все правила которые вы определили для своей модели; валидация со стороны сервера
            {
                _db.Category.Add(obj);
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
            var obj = _db.Category.Find(id);      //получаем обьект Category из базы данных используя Find(работает только с полем имеющим первичный ключ) для искомого id

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
            var obj = _db.Category.Find(id);
            if(obj == null)                      //тут не нужна валидация, а только проверка определен ли обьект
            {
                return NotFound();
            }
                _db.Category.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index"); //перенаправление исполнение кода в метод Index
        }
    }
}
