using System.Collections.Generic;
using SwitchBook.Models;

namespace SwitchBook.ViewModels
{
    public class OrderInfo
    {
        public List<Book> Books1 { get; set; }
        public List<Book> Books2 { get; set; }

        public List<Order> Orders { get; set; }
    }
    public class MyOrdersViewModel
    {
       public OrderInfo Requests { get; set; }
       public OrderInfo History { get; set; }
       public OrderInfo MyRequests { get; set; }
    }
}