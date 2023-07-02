using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SancScan.Models;
using Serilog;

namespace SancScan.Controllers
{
    public class LoginController : Controller
    {
        //Entity framework ile MSSQL veritabanı arasındaki bağlam. 
        private readonly DatabaseContext _context;
        //Global oturum arayüzü
        private ISession _session;

        public LoginController(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            //Dependency Injection tatbik edilmesiyle bağlantı başı bir defa örneklenme burada sağlanmaktadır.
            _context = context;
            //_session isimli oturum yönetme arayüzü örneği 
            this._session = httpContextAccessor.HttpContext.Session;
        }

        //İlk başta otomatik olarak oluşturuldu.
        public async Task<IActionResult> Index()
        {
            //https://localhost:XXXX/Login adresine varıldığında görünüm döndürülür. Öntanımlı olarak get metodu kullanır.
            return View();
        }

        // GET: Login/Details/5
     

        // GET: Login/Create
        public IActionResult Create()
        {
            //https://localhost:XXXX/Login/Create adresine varıldığında görünüm döndürülür. Öntanımlı olarak get metodu kullanır.
            return View();
        }

        // POST: Login/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string username, string password)
        {
            //Kullanıcı oluşturulan fonksiyon. 
                //Boş kullanıcı adı ve şifre girildiğinde önce loglar, sonra indexe yönlendirir.
                if(username == null || password == null)
                {
                    Log.Warning(DateTime.Now.ToString() + " Kullanıcı kaydı oluşturulurken: " + " Boş kullanıcı adı veya şifre girildiği tespit edildi.");
                    return RedirectToAction(nameof(Index));
                }
                //Login modeli, düzenlenip kaydedilmek üzere örneklenir.
                Login newUser = new Login();
                //Üye özelliklerin kullanıcıdan alınan değerlerle eşleştirilmesi.
                newUser.Username = username;
                newUser.Password = password;
                //Veritabanı bağlamına eklenip kayıt yapılması.
                _context.Add(newUser);
                await _context.SaveChangesAsync();
                TempData["RegistrationSuccessful"] = "Kayıt başarılı. Lütfen giriş yapınız.";
                return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password)
        {
            //Kullanıcıdan gelen parametrelerin boş olup olmadığının kontrolü
            if(username == null || password == null)
            {
                return RedirectToAction(nameof(Index));
            }
            //Kullanıcıdan gelen parametrelere haiz kaydın olup olmadığının kontrolü
            var checkedUser = _context.Login.FirstOrDefault(x => x.Username == username && x.Password == password);
            //Yanlışsa uyarı verip, tekrar denenmesi için indexe yönlendirme
            if(checkedUser == null) 
            {
                TempData["wrongCredential"] = "Kullanıcı adı veya şifreniz yanlış";
                return RedirectToAction(nameof(Index));
            }
            //Eğer parametrelerle kayıt eşleşiyorsa oturum bağlamında geçerli stringin set edlimesi
            _session.SetString("username", username);
            //Direk kitapların listesinin olduğu sayfaya yönlendirdim.
            return RedirectToAction("Index", "Book", new { area = "" });
        }

        public async Task<IActionResult> Logout()
        {
            //Çıkış için bu metoda başvurulduğunda, set edilen oturum bağlamı stringinin kaldırılması. Sonra yeniden girememesi için... 
            if (_session.GetString("username") != null)
            {
                _session.Remove("username");
            }
            //Her halükarda indexe yönlendirilmesi. Yani giriş yapmadan https://localhost:XXXX/Login/Logout yazdığında da giriş sayfasına yönlensin.
            return _session.GetString("username") == null ? RedirectToAction("Index", "Login") : RedirectToAction("Index", "Login");
        }

    }
}
