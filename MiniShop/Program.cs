using MiniShop.Data;
using MiniShop.Models;
using MiniShop.Repositories;
using MiniShop.Services;
using System;
using System.Threading.Tasks;

namespace MiniShop
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Proqram basladi...\n");

            using var context = new ShopDBContext();
            var ticketRepo = new Repository<Ticket>(context);
            var orderRepo = new Repository<Order>(context);
            var orderService = new OrderService(); 

            Console.WriteLine("****** CRUD ******");

            var newTicket = new Ticket { Title = "Concert Ticket", Price = 50m, Stock = 5 };
            await ticketRepo.AddAsync(newTicket);
            Console.WriteLine($"[CREATE] Yeni bilet elave olundu: {newTicket.Title}");

            var tickets = await ticketRepo.GetAllAsync();
            Console.WriteLine("\n[READ] Bazadakı biletler:");
            foreach (var t in tickets)
            {
                Console.WriteLine($"- ID: {t.Id}, Ad: {t.Title}, Qiymet: {t.Price}, Stok: {t.Stock}");
            }

            newTicket.Price = 55m;
            await ticketRepo.UpdateAsync(newTicket);
            Console.WriteLine($"\n[UPDATE] Bilet qiymeti yenilendi. Yeni qiymet: {newTicket.Price}");

            Console.WriteLine("\n=== BİLET ALISI (CONCURRENCY) TESTİ ===");
            await Task.WhenAll(
                       orderService.SafeBuyAsync("Customer1", 2),
                       orderService.SafeBuyAsync("Customer2", 2)
                    );

            Console.WriteLine($"\nÜmumi ugurla satılan bilet sayi: {orderService.SuccessFullOrders}");

            Console.WriteLine("\nCixis ucun bir duymeye basin...");
            Console.ReadKey();
        }
    }
}