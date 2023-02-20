using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwitchBook.Data;
using SwitchBook.Models;
using SwitchBook.ViewModels;

namespace SwitchBook.Controllers
{
    [Authorize]
    public class TradeController : Controller
    {
        public ApplicationDbContext _db;

        public TradeController(ApplicationDbContext context)
        {
            _db = context;
        }
        // GET: TradeController
        public async Task<ActionResult> Index(int bookId)
        {

            if (ModelState.IsValid)
            {
                ViewBag.bookId = bookId;
                
                var owner = await _db.Users.FirstAsync(x => x.UserName == User.Identity.Name);
               
                var myBooks = await _db.Books.Where(x => x.OwnerId == owner.Id).ToListAsync();
                var book = await _db.Books.FirstOrDefaultAsync(x => x.Id == bookId);
                TradeViewModel model = new TradeViewModel() {Book = book, MyBooks = myBooks, Owner = owner};
                return View(model);
            }
            return Redirect("/Home");
        }

        // GET: TradeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //// GET: TradeController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: TradeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int bookId, int myBookId)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/Home");
            }

            if (bookId == myBookId)
            {
                return RedirectToAction("Index", "Books");
            }

            //get first book owner id 
            var firstBookOwnerId = await _db.Books.FirstOrDefaultAsync(x => x.Id == bookId);
            //get second book owner id
            var secondBookOwnerId = await _db.Books.FirstOrDefaultAsync(x => x.Id == myBookId);

            //get first book owner address
            var firstAddress = await _db.Address.FirstOrDefaultAsync(x => x.UserId == firstBookOwnerId.OwnerId);
            //get second book owner address
            var secondAddress = await _db.Address.FirstOrDefaultAsync(x => x.UserId == secondBookOwnerId.OwnerId);

            Order newOrder = new Order()
            {
                FirstBookId = bookId,
                LastBookId = myBookId,
                
                FirstAddressId = firstAddress.Id,

                LastAddressId = secondAddress.Id,

                IsConfirm  = false

             };
            await _db.Orders.AddAsync(newOrder);
            await _db.SaveChangesAsync();
            return Redirect("/Books");
        }

        // GET: TradeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TradeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TradeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TradeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
