using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SancScan.Models;
using Serilog;

namespace SancScan.Controllers
{
    public class BorrowController : Controller
    {
        //Veritabanı ve oturum bağlamı
        private readonly DatabaseContext _context;
        private ISession _session;

        public BorrowController(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            //Bağlamlara değer atanması...
            _context = context;
            this._session = httpContextAccessor.HttpContext.Session;
        }

        // GET: Borrow
        public async Task<IActionResult> Index()
        {
            //Giriş yapılarak atanması gereken string değer yoksa bu sayfada da işi olmasın...
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            //Veritabanı bağlamındaki kitapların değişkene aktarılması...
            var databaseContext = _context.Borrow.Include(b => b.Book);
            //Aktarılan kitapların, görünüme parametre olarak verilip, kitap listesinin görünüme yazılması. 
            return View(await databaseContext.ToListAsync());
        }

        // GET: Borrow/Details/5
        /*
        public async Task<IActionResult> Details(Guid? id)
        {
            
            if (id == null || _context.Borrow == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
            
        }
        */

        // GET: Borrow/Create
        public IActionResult Create()
        {
            //Klasik kontrol... Oturum açmazsa, kitap oluşturmasının anlamı yok. Tabii işin ehli hackerlar ne derece aşabilir, merak ediyorum.
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            try
            {
                //Tüm kitapların listesi, değeri id, görünümü kitap ismi olan select box'a yansıtılması.
                ViewData["BookId"] = new SelectList(_context.Book, "BookId", "BookName");
            }
            catch(Exception ex)
            {
                //Hangi türden istisnai durum olursa olsun,belirgin özelliklerin loglanması. Loglar logs klasörü altında .log uzantılı dosyaya yazılıyor.
                Log.Warning("\nMessage ---\n{0}", ex.Message);
                Log.Warning(
                    "\nHelpLink ---\n{0}", ex.HelpLink);
                Log.Information("\nSource ---\n{0}", ex.Source);
                Log.Information(
                    "\nStackTrace ---\n{0}", ex.StackTrace);
                Log.Information(
                    "\nTargetSite ---\n{0}", ex.TargetSite);
            }
            return View();
        }

        // POST: Borrow/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int BookId,string BorrowerName, DateTime DateToBeReturned, DateTime? BringBackDate)
        {
            //Klasik kontrol
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            try
            {
                //Parametre olarak gönderilen id ile eşleşen kitap kaydı kontrolü
                var checkedBook = _context.Book.Find(BookId);
                //Aynı id ile eşleşen ödünç kaydı kontrolü, zira kitap id'si ödünç tablosunda foreign key, kitap tablosunda ise primary key. 
                var checkedBorrow = _context.Borrow.Find(BookId);
                //Kitap yoksa bulunamadığını yansıtma.
                if (checkedBook == null)
                {
                    Log.Warning(DateTime.Now.ToString() + " Ödünç kaydı oluşturma işlemi içerisinde: " + BookId + " id'siyle başvuruldu, ilgili id ile bağlantılı kitap bulunamadı.");
                    return NotFound();
                }
                //Kitap daha önce ödünç verilmiş mi kontrol edeyim...
                bool checkForBorrow = ((checkedBorrow != null) && (!checkedBorrow.IsBroughtBack));
                //Ödünç verilmişse tedbir...
                if (checkedBook.IsBorrowed || checkForBorrow)
                {
                    Log.Warning(DateTime.Now.ToString() + " Ödünç kaydı oluşturma işlemi içerisinde: " + checkedBook.BookName + " isimli kitap ikinci defa ödünç verilmeye çalışıldı.");
                    TempData["BookBorrowedYet"] = $"{checkedBook.BookName} isimli kitap zaten ödünç verilmiş.";
                    return RedirectToAction(nameof(Index));
                }
                //Tarihler arası kıyas. Gerçeğe mutabık olması için...
                if (BringBackDate != null && DateTime.Compare((DateTime)BringBackDate, DateTime.Now) < 0)
                {
                    Log.Warning(DateTime.Now.ToString() + " Ödünç kaydı oluşturma işlemi içerisinde: Geri getirilecek tarih girilirken, girilmeye çalışılan tarihin ödünç alma tarihinden önce olduğu tespit edildi.");
                    TempData["BringBackDateError"] = "Kitabın geri getirileceği tarih geçmişteki bir tarih olamaz.";
                    return RedirectToAction(nameof(Index));
                }
                //Boş ödünç kaydı örneklemesi ve parametre olarak gelen değerlerin ilgili örneğe atanması.
                Borrow borrow = new Borrow();
                borrow.BookId = BookId;
                borrow.BorrowerName = BorrowerName;
                borrow.BorrowDateTime = DateTime.Now;
                borrow.BringBackDate = BringBackDate == null ? null : BringBackDate;//Boş olabilir
                borrow.DateToBeReturned = DateTime.Now.AddDays(15);
                borrow.ReceivedBackTime = null;
                //Kitap ödünç olarak veirldiğinde, kitap tablosunda ödünç verilen kitabın
                //olmadığı ve ödünç verildiğini gösteren alanların düzenlenmesi...
                var updatedBook = _context.Book.SingleOrDefault(b => b.BookId == BookId);
                if (updatedBook != null)
                {
                    updatedBook.DoesExist = false;
                    updatedBook.IsBorrowed = true;
                    await _context.SaveChangesAsync();
                }
                //Kayıt.
                _context.Add(borrow);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                //Tür ayırt etmeksizin istisna yakalayıp loglama.
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
            //ViewData["BookId"] = new SelectList(_context.Book, "BookId", "BookId", borrow.BookId);
            //return View(borrow);

            //return Content($"{BookId} {BorrowerName} {BorrowDateTime} {BringBackDate}");
        }
            

            // GET: Borrow/Edit/5
            
            public async Task<IActionResult> Edit(int? id)
            {
            //Rutin kontrol
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");

            /*if (id == null || _context.Borrow == null)
            {
                Log.Warning(DateTime.Now.ToString() + " Ödünç kaydı düzenleme sayfası görüntülenirken: " + "null id girilmesi veya girilen id'ye tabii ödünç kaydının bulunmadığı tespit edildi.");
                return NotFound();
            }*/
                //Olur da gönderilen id'ye karşılık gelen kayıt olmazsa loglama ve yansıtım...
                var borrow = await _context.Borrow.FindAsync(id);
                if (borrow == null) 
                {
                    Log.Warning(DateTime.Now.ToString() + " Ödünç kaydı düzenleme sayfası görüntülenirken: " + " girilen id'ye tabii ödünç kaydının boş olduğu tespit edildi.");
                    return NotFound();
                }
                try
                {
                    //Tüm kitapların, ödünç verilmek üzere listelenmesini sağlayan selectboxa yansıtılması...
                    ViewData["BookId"] = new SelectList(_context.Book, "BookId", "BookName", borrow.BookId);
                }
                catch (Exception ex)
                {
                    //Rutin istisna yazma...
                    Log.Warning("\nMessage ---\n{0}", ex.Message);
                    Log.Warning(
                        "\nHelpLink ---\n{0}", ex.HelpLink);
                    Log.Information("\nSource ---\n{0}", ex.Source);
                    Log.Information(
                        "\nStackTrace ---\n{0}", ex.StackTrace);
                    Log.Information(
                        "\nTargetSite ---\n{0}", ex.TargetSite);
                }
            return View(borrow);
        
            }
            

            // POST: Borrow/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int BorrowId, int BookId, string BorrowerName, bool IsBroughtBack, DateTime? BringBackDate)
            {
            if (_session.GetString("username") == null)
                return RedirectToAction("Index", "Login");
            //Düzenlenecek ödünç kaydının boş oluğ olmadığının kontrolü
            var result = _context.Borrow.Find(BorrowId);
                
                    if (result == null) { 
                        return NotFound();
                    }
                    //Karmaşaya meydan vermemek namına, kitabın verildiğinin sadece bir kere onaylanması için,
                    //eğer kitap geri alındığında geri getirildi mi sorusunda değişiklik yapılamaması için
                    //çalışma... Sonuçta ciddi bir iş ve cevaplanırken düşünülmesi lazım bence...
                    else if(result.IsBroughtBack)
                    {
                        IsBroughtBack = true;
                    }
                    
                    try
                    {
                        //Güncellenecek kaydın boş örneği...
                        Borrow updatedBorrow = new Borrow();
                        //Getirilen kitap...
                        Book broughtBook = _context.Book.Find(BookId);
                        //Kitabın getirildiğinden emin olunması, eğer getirilmişse, kaydın girildiği an...
                        if(!result.IsBroughtBack && IsBroughtBack)
                        {
                            result.ReceivedBackTime = DateTime.Now;
                        }
                        result.BookId = BookId;
                        result.BorrowerName = BorrowerName;
                        //Tarihte tutarsızlığı önlemek amaçlı girişim.
                        if (BringBackDate != null && DateTime.Compare(result.BorrowDateTime,  (DateTime)BringBackDate) <= 0)
                        {
                            result.BringBackDate = BringBackDate;
                        }
                        else if (BringBackDate != null && DateTime.Compare(result.BorrowDateTime, (DateTime)BringBackDate) > 0) 
                        {
                            Log.Warning(DateTime.Now.ToString() + " Ödünç kaydı düzenlemesi akabinde kayıt ederken: " + " geri getirme tarihinin, ödünç alınan tarihten erken olduğu tespit edildi.");
                            TempData["DateIncompatibility"] = "Geri getirilen tarih, ödünç alınan tarihten erken.";
                            return RedirectToAction(nameof(Index));
                        }
                        //Bilgisayar işletmeni bir defa verilebilecek olan cevaptan eminse, kaydın güncellenmesi... 
                        if(IsBroughtBack) 
                        {
                            broughtBook.DoesExist = true;
                            broughtBook.IsBorrowed = false;
                            result.IsBroughtBack = IsBroughtBack;
                        }

                        await _context.SaveChangesAsync();

                    }
                    catch (Exception ex)
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

        /*
            private bool BorrowExists(Guid id)
            {
              return (_context.Borrow?.Any(e => e.BookId == id)).GetValueOrDefault();
            }
            */
        }
    }
