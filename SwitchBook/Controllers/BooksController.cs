using System.IO;
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
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string query)
    {
        var books = await _context.Books.Where(x =>
            EF.Functions.Like(x.Title, $"%{query}%") || EF.Functions.Like(x.Author, $"%{query}%") ||
            EF.Functions.Like(x.Description, $"%{query}%")).ToListAsync();

        var orders = await _context.Orders.ToListAsync();
        foreach (var order in orders)
        {
            var mybook = await _context.Books.FirstOrDefaultAsync(x => x.Id == order.FirstBookId);
            books.Remove(mybook);
            mybook = await _context.Books.FirstOrDefaultAsync(x => x.Id == order.LastBookId);
            books.Remove(mybook);
        }

        return View(books);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        var owner = await _context.Users.FirstOrDefaultAsync(x => x.Id == book.OwnerId);
        ViewBag.Owner = owner.UserName;
        if (book == null) return NotFound();

        return View(book);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookViewModel bookvm)
    {
        if (ModelState.IsValid)
        {
            var book = new Book
            {
                Title = bookvm.Title,
                Author = bookvm.Author,
                Description = bookvm.Description,
                OwnerId = _context.Users.First(x => x.UserName == User.Identity.Name).Id
            };
            if (bookvm.Image != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(bookvm.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)bookvm.Image.Length);
                }

                book.Image = imageData;
            }

            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,OwnerId,Description,Image")] Book book)
    {
        if (id != book.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null) return NotFound();

        return View(book);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}