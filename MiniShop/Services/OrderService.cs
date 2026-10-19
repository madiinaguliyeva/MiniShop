using Microsoft.EntityFrameworkCore;
using MiniShop.Data;
using MiniShop.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiniShop.Services
{
    public class OrderService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly object _locker = new object();
        public int SuccessFullOrders = 0;

        public async Task BuyAsync(string customer, int ticketId)
        {
            using var db = new ShopDBContext();
            var ticket = await db.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket == null || ticket.Stock == 0)
            {
                Console.WriteLine($"Stockda bilet qalmadigi ucun, {customer} bilet ala bilmir...");
                return;
            }
            Console.WriteLine($"Bilet sayi: {ticket.Stock} \t{customer} bilet ucun odenise kecir....");
            await Task.Delay(5000);
            ticket.Stock--;

            var order = new Order { CustomerName = customer, CreatedTime = DateTime.Now };
            order.Items.Add(new OrderItem { TicketId = ticketId, Quantity = 1, Price = ticket.Price });
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            lock (_locker)
            {
                SuccessFullOrders++;
            }
            Console.WriteLine($"{customer} ugurla bilet aldi...");
        }

        public async Task SafeBuyAsync(string customer, int ticketId)
        {
            try
            {
                await _semaphore.WaitAsync();
                await BuyAsync(customer, ticketId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}