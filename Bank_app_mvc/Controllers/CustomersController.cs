using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bank_app_mvc.Data;
using Bank_app_mvc.Models;
using System.Security.Claims;

namespace Bank_app_mvc.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.                
                Include(c => c.User)
                .FirstOrDefaultAsync(d => d.CustomerId == userId); 
            if(customer == null)
            {
                return RedirectToAction("NoAccount","Customers");
            }
            return View(customer);
        }

        // GET: NoAccount
        public IActionResult NoAccount()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        
        // GET: Customers/Create
        public IActionResult Create()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Phone,Address,NIN,CustomerId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                var NewAccountNumber = GenerateRandomAccountNumber();

                var account = new Account
                {
                    AccountNumber = NewAccountNumber,
                    AccountName = $"{customer.FirstName} {customer.LastName}",
                    CustomerId = customer.CustomerId,
                    Customer = customer
                };
                _context.Add(account);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Customers");
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "Id", customer.CustomerId);
            return View(customer);
        }
        

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        private long GenerateRandomAccountNumber()
        {
            Random random = new Random();
            long accountNumber;
            bool accountNumberExists;

            do
            {
                var randomDigits = random.Next(10_000_000, 99_999_999);
                var accountNumberString = $"10{randomDigits}";

                accountNumber = long.Parse(accountNumberString);


                accountNumberExists =  _context.Accounts.Any(a => a.AccountNumber == accountNumber);
            }
            while (accountNumberExists); 

            return accountNumber;
        }
    }
}
