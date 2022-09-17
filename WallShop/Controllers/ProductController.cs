using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WallShop.Data;
using WallShop.Models;
using WallShop.Models.ViewModels;

namespace WallShop.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;    
            //IHostingEnvironment этот интерфейс предлагает ряд свойств, с помощью которых мы можем получить информацию об окружении:
            //ApplicationName: возвращает имя приложения
            //EnvironmentName: возвращает описание среды, в которой хостируется приложение
            //ContentRootPath: возвращает путь к корневой папке приложения
            //WebRootPath: возвращает путь к папке, в которой хранится статический контент приложения, как правило, это папка wwwroot
            //ContentRootFileProvider: возвращает реализацию интерфейса Microsoft.AspNetCore.FileProviders.IFileProvider, которая может использоваться для чтения файлов из папки ContentRootPath
            //WebRootFileProvider: возвращает реализацию интерфейса Microsoft.AspNetCore.FileProviders.IFileProvider, которая может использоваться для чтения файлов из папки WebRootPath

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db; //дает доступ к базе данных в обекте _db
            _webHostEnvironment = webHostEnvironment;       //получаем webHostEnvironment с помощью внедрения зависимостей
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
            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem      //для передачи списка всех Category из контролера в представление(SelectListItem для раскрывающегося списка)
            //{                                                                                               //и выьираем имя и id категории и конвертируем в элементы специального типа для выбора
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            ////ViewBag.CategoryDropDown = CategoryDropDown;        //передаем CategoryDropDown нашему View c помощью ViewBag для вывода на экран
            //ViewData["CategoryDropDown"] = CategoryDropDown; 

            //Product product = new Product();      //создаем новый обьект product типа Product 

            ProductVM productVM = new ProductVM()   //загружаем ViewModel типа ProductVM
            {
                Product = new Product(),            //с добавлением пустого обьекта Product
                CategorySelectList = _db.Category.Select(i => new SelectListItem      //для передачи списка всех Category из контролера в представление(SelectListItem для раскрывающегося списка)
                {                                                                                               //и выбираем имя и id категории и конвертируем в элементы специального типа для выбора
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };


            if (id == null)                        //и проверяем что, если id которое мы получили не имеет значения, то поступил запрос на создание новой сущности
            {
                return View(productVM);             //переход на представление со ссылкой на модель
            }
            else                                  //если id имеет значение 
            {
                productVM.Product = _db.Product.Find(id);   //получаем productVM из базы данных используя Find     
                if(productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }                                      //теперь наше представление строго типизированно с помощью productVM
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM) //тут получаем обьект который нужно добавить в бд
        {
            if (ModelState.IsValid)               //проверяет выполнены ли все правила которые вы определили для своей модели; валидация со стороны сервера
            {   
                //если новое изображение загружено - нам нужно его получить
                var files = HttpContext.Request.Form.Files;     //сохраним загруженное нами изображение в files, извлекая его с помощью HttpContext
                                                                //класс HttpRequest позволяет ASP.NET считывать значения HTTP, отправляемые клиентом во время веб-запроса
                                                                //Form олучает коллекцию переменных формы, Files получает коллекцию загруженных с клиента файлов
                string webRootPath = _webHostEnvironment.WebRootPath;   //записывает в переменную путь к папке wwwroot

                if(productVM.Product.Id == 0)           //проверим эта функция для создания или обновления(есть ли изображение); если 0, то необъодим код для создания новой сущности  
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath; //получаем путь в папку в которой будут храниться файлы с картинками
                    string fileName = Guid.NewGuid().ToString();    //GUID — это 128-разрядное целое число (16 байт), которое можно использовать во всех компьютерах и сетях,
                                                                    //где требуется уникальный идентификатор. Такой идентификатор имеет очень низкую вероятность дублирования.
                    string extension = Path.GetExtension(files[0].FileName);    //получаем разрешение файла присваивая значения файла который был загружен;
                                                                                //Path - это строка, предоставляющая расположение файла или каталога, GetExtension - определяет, включает ли путь расширение имени файла
                    //скопируем файл в новое место которое определяется значением upload
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))    //добавлем встроенный класс, с экземпляром класса FileStream(исп. для управления файлами);
                                                                                                                            //Combine - объединяет две строки в путь; FileMode - указывает, каким образом операционная система должна открыть файл
                    {
                        files[0].CopyTo(fileStream);
                    }
                    //обновляем ссылку на Image внутри сущности Product указав новый путь для доступа
                    productVM.Product.Image = fileName + extension;

                    _db.Product.Add(productVM.Product); //добавляем новый товар

                }
                else
                {
                    //bool Func(Product u)          //лямда 
                    //{
                    //    if (u.Id == x)
                    //    {
                    //        return true;
                    //    }
                    //    return false;
                    //}
                    //Updating
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id); //получаем актуальную сущность Product из Db на основе Id
                                                                                                                  //добавляем AsNoTracking чтобы EF не путалось что обновляеть и не выдавало ошибки, так как он не может отслеживать одни и теже обьекты(у обоих одинаковые ключи)
                                                                                                                  //так как мы обновляем нашу бд в конце, а тут нам обновление не нужно, а просто получение из дб 
                    if (files.Count > 0)       //проверяем на наличие файла 
                    {
                        string upload = webRootPath + WC.ImagePath; 
                        string fileName = Guid.NewGuid().ToString();  
                        string extension = Path.GetExtension(files[0].FileName);
                                                                                        //прежде нам нужно удалить старый файл
                        var oldFile = Path.Combine(upload, objFromDb.Image);        //обьеденим путь и название старого файла 
                            
                        if (System.IO.File.Exists(oldFile))             //удалим старый файл 
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))    
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;     //после перемещения сохраним ссылку на изображение и добавим ссылку на св-во Image
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;      //если фото не менялось, оставим его таким же
                    }
                    _db.Product.Update(productVM.Product);      //обновляем дб
                }

                _db.SaveChanges();
                return RedirectToAction("Index"); //перенаправление исполнение кода в метод Index
            }
            return View();
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
