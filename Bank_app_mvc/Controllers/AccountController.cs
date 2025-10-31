using System.Security.Claims;
using Bank_app_mvc.Data;
using Bank_app_mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace Bank_app_mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if(User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(AccountExists(userId) == false)
            {
                return RedirectToAction("NoAccount","Customers");
            }
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.CustomerId == userId);


            return View(account);
        }

        public IActionResult Transaction()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new Transaction();
            return View(model);
        }

        public IActionResult TransactionSuccess()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMoney(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp =>
                        $"{kvp.Key}: {string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}"
                    );

                TempData["ModelErrors"] = string.Join(" | ", errors);
                // optional: also log to console
                foreach (var e in errors) Console.WriteLine(e);

                return View("Transaction", transaction);
            }
            // validate input
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var sender = await _context.Accounts.
                    SingleOrDefaultAsync(a => a.CustomerId == userId);

                // check if sender has enough funds

                if (sender.Balance < transaction.Amount)
                {
                    ModelState.AddModelError("Amount", "Insufficient balance.");
                    return View("Transaction", transaction);
                }

                // if sending to peak bank
                if (transaction.Bank == "PeakBank")
                {
                    // get the reciever and sender bank details
                    var receiver = await _context.Accounts.
                        SingleOrDefaultAsync(a => a.AccountNumber == transaction.ToAccountNumber);

                    if (receiver == null)
                    {
                        ModelState.AddModelError("", "Receiver account not found in PeakBank.");
                        return View("Transaction", transaction);
                    }
                    
                    // deduct and increment from their balance
                    sender.Balance -= transaction.Amount;
                    receiver.Balance += transaction.Amount;

                    var newTransaction = new Transaction
                    {
                        Amount = transaction.Amount,
                        TransactionType = transaction.TransactionType,
                        FromAccountNumber = sender.AccountNumber,
                        ToAccountNumber = receiver.AccountNumber,
                        Bank = "Peak Bank",
                        AccountType = transaction.AccountType
                    };

                    // save to account
                    _context.Update(sender);
                    _context.Update(receiver);
                    _context.Transactions.Add(newTransaction);
                    // update to database
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"₦{transaction.Amount:N0} successfully sent to {receiver.AccountName}.";
                    return View("TransactionSuccess");
                }
                else
                {
                    // For external bank transactions (non-PeakBank)
                     var newTransaction = new Transaction
                    {
                        Amount = transaction.Amount,
                        TransactionType = "Transfer",
                        FromAccountNumber = sender.AccountNumber,
                        ToAccountNumber = transaction.ToAccountNumber,
                        Bank = transaction.Bank,
                        AccountType = transaction.AccountType
                    };
                    sender.Balance -= transaction.Amount;

                    // save to account
                    _context.Update(sender);
                  
                    _context.Transactions.Add(newTransaction);
                    // update to database
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"₦{transaction.Amount:N0} successfully sent to {transaction.Bank} account {transaction.ToAccountNumber}.";
                    return View("TransactionSuccess");
                }
                
            }
            // If validation failed, redisplay the form with validation messages
            return View("Transaction", transaction);
        }

        public async Task<IActionResult> TransactionHistory()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.CustomerId == userId);
            if (account == null)
            {
                return RedirectToAction("NoAccount", "Customers");
            }

            var transactions = await _context.Transactions
                .Where(t => t.FromAccountNumber == account.AccountNumber)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
            return View(transactions);
        }
        private bool AccountExists(string id)
        {
            return _context.Accounts.Any(e => e.CustomerId == id);
        }
        

    }
}
