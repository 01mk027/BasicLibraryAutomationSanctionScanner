using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SancScan.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Dynamic;
using System.Data;
using Serilog;
using Microsoft.AspNetCore.Http;

namespace SancScan.Controllers
{
    public class BookController : Controller
    {
        //Veritabanı bağlamı
        private readonly DatabaseContext _context;
        //private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        //Kitapların resminin eklenebilmesi için wwwroot klasörüne işaret eden nesne.
        private readonly IWebHostEnvironment _webHostEnvironment;
        //Logger
        private readonly ILogger<BookController> _logger;
        //Oturum bağlamı
        private ISession _session;
        public BookController(DatabaseContext context, IWebHostEnvironment webHostEnvironment, ILogger<BookController> logger, IHttpContextAccessor httpContextAccessor)
        {
            //Her çağrıldığında sadece bir kere örneklenmesi amacıyla dependency injection işlemine tabii nesnelerin 
            //uygulama içerisinde eşlenmesi...
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            this._session = httpContextAccessor.HttpContext.Session;
        }
        

        
        

        // GET: Book
        public async Task<IActionResult> Index()
        {

            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");

            //Kitapların liste haline getirilmesi
            List<Book> books = await _context.Book.ToListAsync();
            //Alfabetik sıralama
            List<Book> orderedBooks = books.OrderBy(b => b.BookName).ToList();
            //List<Borrow> borrows = await _context.Borrow.Where(b => b.IsBroughtBack == false).ToListAsync();
            //Görünüm içerisinde sadece bir model kullanmak mümkün olduğundan iki tablodan da veri tutacak ayrı bir modelin örneklenmesi
            List<BookViewModel> bookViewModels = new List<BookViewModel>();
            for (int i = 0; i < orderedBooks.Count; i++)
            {
                BookViewModel viewItem = new BookViewModel();
                try
                {
                    //Ödünç verildiyse listelenmesi amacıyla ödünç kayıtlarının sırasıyla kitaplarla eşleşen çiftleri
                    var borrow = _context.Borrow.FirstOrDefault(x => x.BookId == orderedBooks[i].BookId && x.IsBroughtBack == false);
                    //Ödünç verilmişse modelin oluşturulması
                    if ((orderedBooks[i].IsBorrowed))
                    {
                        viewItem.BookId = orderedBooks[i].BookId;
                        viewItem.BookName = orderedBooks[i].BookName;
                        viewItem.AuthorName = orderedBooks[i].AuthorName;
                        viewItem.DoesExist = orderedBooks[i].DoesExist;
                        viewItem.IsBorrowed = orderedBooks[i].IsBorrowed;
                        viewItem.ImageFullPath = orderedBooks[i].ImageFullPath;
                        viewItem.CreatedAt = orderedBooks[i].CreatedAt;
                        viewItem.BorrowerName = borrow.BorrowerName;
                        viewItem.BringBackDate = borrow.BringBackDate;
                    }
                    //Verilmemişse oluşturulması
                    else if (!orderedBooks[i].IsBorrowed)
                    {
                        viewItem.BookId = orderedBooks[i].BookId;
                        viewItem.BookName = orderedBooks[i].BookName;
                        viewItem.AuthorName = orderedBooks[i].AuthorName;
                        viewItem.DoesExist = orderedBooks[i].DoesExist;
                        viewItem.IsBorrowed = orderedBooks[i].IsBorrowed;
                        viewItem.ImageFullPath = orderedBooks[i].ImageFullPath;
                        viewItem.CreatedAt = orderedBooks[i].CreatedAt;
                        viewItem.BorrowerName = null;
                        viewItem.BringBackDate = null;
                    }
                    bookViewModels.Add(viewItem);
                }
                catch(Exception ex)
                {
                    Log.Warning("\nMessage ---\n{0}", ex.Message);
                    Log.Warning(
                        "\nHelpLink ---\n{0}", ex.HelpLink);
                    Log.Information("\nSource ---\n{0}", ex.Source);
                    Log.Information(
                        "\nStackTrace ---\n{0}", ex.StackTrace);
                    Log.Information(
                        "\nTargetSite ---\n{0}", ex.TargetSite);
                }
            }

            //Oluşturulan modelin görünüme transferi...
            return View(bookViewModels);
                          
        }

        // GET: Book/Details/5
 /*
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
 */
        //Converting IFormFile to byte array
        public static async Task<byte[]> GetBytes(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string BookName, string AuthorName, IFormFile Image)
        {
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            try
            {
                //Boş kitap örneği
                var book = new Book();
                //book.BookId = Guid.NewGuid();

                //_context.Add(book);
                //await _context.SaveChangesAsync();
                //Kitap resminin wwwroot/Files klasörüne kaydedilebilmesi için işlemler...
                string rootPath = _webHostEnvironment.WebRootPath;
                rootPath += "/Files";
                string folderName = "Files/";
                string filename = Path.GetFileName(Image.FileName);
                string fullPath = Path.Combine(rootPath, filename);
                string envpath = folderName + "/" + filename;
                using (Stream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }
                //Resimler byte dizisi olarak kaydediliyormuş, bunu öğrendiğim iyi oldu....
                byte[] bytes = await GetBytes(Image);
                book.Image = bytes; //Assigning value of byte array which represent our picture in bytes
                book.BookName = BookName;
                book.AuthorName = AuthorName;
                book.DoesExist = true;
                book.ImageName = filename;
                book.ImageSize = Image.Length;
                book.ImageFullPath = envpath;
                book.IsBorrowed = false;
                book.CreatedAt = DateTime.Now;
                _context.Add(book);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Log.Warning("\nMessage ---\n{0}", ex.Message);
                Log.Warning(
                    "\nHelpLink ---\n{0}", ex.HelpLink);
                Log.Information("\nSource ---\n{0}", ex.Source);
                Log.Information(
                    "\nStackTrace ---\n{0}", ex.StackTrace);
                Log.Information(
                    "\nTargetSite ---\n{0}", ex.TargetSite);
            }
            return RedirectToAction(nameof(Index));
            //var hexString = Convert.ToBase64String(bytes);
            //
            


        }

        private IFormFile GetByteArrayFromImage(object myImage)
        {
            throw new NotImplementedException();
        }

        // GET: Book/Edit/5
        // GET: BooksControllerr/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            if (id == null || _context.Book == null)
            {
                Log.Warning(DateTime.Now.ToString() + " Kitap düzenleme işlemi içerisinde: " + " girilen id'ye karşılık gelmesi gereken kitap kaydının boş olduğu tespit edildi.");
                return NotFound();
            }

            //Gönderilen id'ye karşılık gelen kaydın boş oluğ olmadığının kontrolü
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                Log.Warning(DateTime.Now.ToString() + " Kitap düzenleme işlemi içerisinde: " + " id ile aranan kitabın boş olduğu tespit edildi.");
                return NotFound();
            }
            try
            {
                //Geçerli resmin yolunun çekilmesi... 
                string rootPath = _webHostEnvironment.WebRootPath;
                string fullPath = $"{rootPath}\\{book.ImageName}";//silinecek olan resmin yolu olabilir.
                book.ImageFullPath = fullPath;
            }
            catch(Exception ex) 
            {
                Log.Warning("\nMessage ---\n{0}", ex.Message);
                Log.Warning(
                    "\nHelpLink ---\n{0}", ex.HelpLink);
                Log.Information("\nSource ---\n{0}", ex.Source);
                Log.Information(
                    "\nStackTrace ---\n{0}", ex.StackTrace);
                Log.Information(
                    "\nTargetSite ---\n{0}", ex.TargetSite);
            }
            return View(book);
        }

        // POST: BooksControllerr/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string BookName, string AuthorName, IFormFile? Image)
        {
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            //var bookToBeUpdate = await _context.Book.FindAsync(id);
            //return Content($"{id} {BookName} {AuthorName} {Image.FileName} {bookToBeUpdate.ImageName} {Image.Length}");
            //Dolu - boş kontrolü
            var bookToBeUpdate = await _context.Book.FindAsync(id);
            if (bookToBeUpdate == null)
            {
                Log.Warning(DateTime.Now.ToString() + " Kitap kaydı güncelleme işlemi içerisinde: " + " güncellenmek üzere id ile aranan kitap kaydına rastlanamadığı tespit edildi.");
                return NotFound();
            }
            try
            {
                //parametre olarak gelen değerlerin modele eşlenmesi...
                bookToBeUpdate.BookName = BookName;
                bookToBeUpdate.AuthorName = AuthorName;
                //Resim güncelleme işlemleri, en zoru buydu...
                if (Image != null && Image.Length > 0)
                {
                    string rootPathDelete = _webHostEnvironment.WebRootPath;
                    string fullPathDelete = $"{rootPathDelete}\\{bookToBeUpdate.ImageName}";//silinecek olan resmin yolu olabilir.
                    if (System.IO.File.Exists(fullPathDelete))
                    {
                        System.IO.File.Delete(fullPathDelete);
                    }
                    string rootPath = _webHostEnvironment.WebRootPath;
                    rootPath += "/Files";
                    string folderName = "Files/";
                    string filename = Path.GetFileName(Image.FileName);
                    string fullPath = Path.Combine(rootPath, filename);
                    string envpath = folderName + "/" + filename;
                    using (Stream fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                    bookToBeUpdate.ImageName = filename;
                    bookToBeUpdate.ImageSize = Image.Length;
                    bookToBeUpdate.ImageFullPath = envpath;
                }
                //string envpath = folderName + "/" + filename;


                //return Content("iboshow");

                _context.Update(bookToBeUpdate);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Log.Warning("\nMessage ---\n{0}", ex.Message);
                Log.Warning(
                    "\nHelpLink ---\n{0}", ex.HelpLink);
                Log.Information("\nSource ---\n{0}", ex.Source);
                Log.Information(
                    "\nStackTrace ---\n{0}", ex.StackTrace);
                Log.Information(
                    "\nTargetSite ---\n{0}", ex.TargetSite);
            }
            return RedirectToAction(nameof(Index));


        }

        // GET: Book/Delete/5


        


    }
}
