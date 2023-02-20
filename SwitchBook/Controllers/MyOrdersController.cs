using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SwitchBook.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwitchBook.Models;
using SwitchBook.ViewModels;

namespace SwitchBook.Controllers
{
    public class MyOrdersController : Controller
    {
        private ApplicationDbContext _db;
        public MyOrdersController(ApplicationDbContext context)
        {
            _db = context;
        }
        public async Task<IActionResult> Index()
        {
            var myBooks= _db.Books.Where(x=>x.OwnerId == _db.Users.First(p=>p.UserName==User.Identity.Name).Id);
            var orderRequest = await _db.Orders.Where(x=>myBooks.Select(b=>b.Id).Contains(x.FirstBookId) && x.IsConfirm ==false).ToListAsync();
            var b1 = new List<Book>();
            var b2 = new List<Book>();
            foreach (var order in orderRequest)
            {
                b1.Add(await _db.Books.FirstOrDefaultAsync(x => x.Id == order.FirstBookId));
                b2.Add(await _db.Books.FirstOrDefaultAsync(x => x.Id == order.LastBookId));

            }
            var orderMyRequest = await _db.Orders.Where(x => myBooks.Select(b => b.Id).Contains(x.LastBookId) && x.IsConfirm == false).ToListAsync();
            var b1M = new List<Book>();
            var b2M= new List<Book>();
            foreach (var order in orderMyRequest)
            {
                b1M.Add(await _db.Books.FirstOrDefaultAsync(x => x.Id == order.FirstBookId));
                b2M.Add(await _db.Books.FirstOrDefaultAsync(x => x.Id == order.LastBookId));

            }

            //var b1 = await _db.Books.Where(x=>orderRequest.Select(b=>b.FirstBookId).Contains(x.Id)).ToListAsync();
            //var b2 = await _db.Books.Where(x=>orderRequest.Select(b=>b.LastBookId).Contains(x.Id)).ToListAsync();
            var orderHistory = await _db.Orders.Where(x =>( myBooks.Select(b => b.Id).Contains(x.FirstBookId) || myBooks.Select(b => b.Id).Contains(x.LastBookId) )&& x.IsConfirm == true).ToListAsync();
            var b1H = new List<Book>();
            var b2H = new List<Book>();

            foreach (var order in orderHistory)
            {
                b1H.Add(await _db.Books.FirstOrDefaultAsync(x => x.Id == order.FirstBookId));
                b2H.Add(await _db.Books.FirstOrDefaultAsync(x => x.Id == order.LastBookId));

            }
            //var b1H = await _db.Books.Where(x => orderHistory.Select(b => b.FirstBookId).Contains(x.Id)).ToListAsync();
            //var b2H = await _db.Books.Where(x => orderHistory.Select(b => b.LastBookId).Contains(x.Id)).ToListAsync();
            MyOrdersViewModel viewModel = new MyOrdersViewModel()
            {
                Requests = new OrderInfo(){Books1 = b1, Books2 = b2, Orders = orderRequest},
                History = new OrderInfo(){Orders = orderHistory, Books1 = b1H, Books2 = b2H},
                MyRequests = new OrderInfo(){Orders = orderMyRequest, Books1 = b1M, Books2 = b2M}
                
            };
            

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int OrderId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);
            var book1 = await _db.Books.FirstOrDefaultAsync(x => x.Id == order.FirstBookId);
            var book2 = await _db.Books.FirstOrDefaultAsync(x => x.Id == order.LastBookId);
            if (order == null)
                return NotFound();
            ViewBag.OrderId = order.Id;
            ViewBag.Book1 = book1;
            ViewBag.Book2 = book2;
            ViewBag.Address1 = await _db.Address.FirstOrDefaultAsync(x => x.Id == order.FirstAddressId);
            ViewBag.Address2 = await _db.Address.FirstOrDefaultAsync(x => x.Id == order.LastAddressId);
            ViewBag.Order = order;
            ViewBag.Owner1 = await _db.Users.FirstOrDefaultAsync(x => x.Id == book1.OwnerId);
            ViewBag.Owner2 = await _db.Users.FirstOrDefaultAsync(x => x.Id == book2.OwnerId);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmRequest(int OrderId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);
            if (order == null)
                return NotFound();
            order.IsConfirm = true;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }



        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int OrderId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);
            if(order == null)
                return NotFound();
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
