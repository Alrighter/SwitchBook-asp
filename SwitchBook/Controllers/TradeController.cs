using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwitchBook.Data;
using SwitchBook.Models;
using SwitchBook.ViewModels;

namespace SwitchBook.Controllers;

[Authorize]
public class TradeController : Controller
{
    public ApplicationDbContext _db;

    public TradeController(ApplicationDbContext context)
    {
        _db = context;
    }

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

        var firstBookOwnerId = await _db.Books.FirstOrDefaultAsync(x => x.Id == bookId);
            
        var secondBookOwnerId = await _db.Books.FirstOrDefaultAsync(x => x.Id == myBookId);

        var firstAddress = await _db.Address.FirstOrDefaultAsync(x => x.UserId == firstBookOwnerId.OwnerId);

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
}