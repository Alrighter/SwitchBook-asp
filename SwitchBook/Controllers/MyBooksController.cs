using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwitchBook.Data;
using SwitchBook.Models;

namespace SwitchBook.Controllers;

[Authorize]
public class MyBooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public MyBooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ownerId = await _context.Users.FirstAsync(x => x.UserName == User.Identity.Name);
        var mybooks = await _context.Books.Where(x => x.OwnerId == ownerId.Id).ToListAsync();

        //check if book was in order table and remove from it mybook variable by id if order was confirmed
        var orders = await _context.Orders.ToListAsync();
        foreach (var order in orders)
        {
            var mybook = await _context.Books.FirstOrDefaultAsync(x => x.Id == order.FirstBookId);
            mybooks.Remove(mybook);
            mybook = await _context.Books.FirstOrDefaultAsync(x => x.Id == order.LastBookId);
            mybooks.Remove(mybook);
        }

        return View(mybooks);
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
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,OwnerId,Description, Image")] Book book,
        IFormFile ImageEdit)
    {
        if (id != book.Id) return NotFound();

        if (!ModelState.IsValid) return View(book);

        try
        {
            if (ImageEdit != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(ImageEdit.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)ImageEdit.Length);
                }

                // установка массива байтов
                book.Image = imageData;
            }
            else
            {
                //find book no tracked
                var originalBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == book.Id);

                book.Image = originalBook.Image;
            }

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

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null) return NotFound();

        return View(book);
    }

    // POST: Books/Delete/5
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