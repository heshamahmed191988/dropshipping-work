using Jumia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                // Handle error, maybe show an error message
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> DisplayTransactions(string searchString, int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                // Validate page and pageSize values
                if (pageNumber < 1)
                {
                    pageNumber = 1;
                }

                if (pageSize < 1)
                {
                    pageSize = 3; // Default page size
                }

                // Fetch transactions from the service without applying pagination yet
                var allTransactions = await _userService.GetTransactionsWithPaginationAsync(0, int.MaxValue);

                // If a search string is provided, filter transactions based on it
                var filteredTransactions = string.IsNullOrEmpty(searchString) ?
                    allTransactions :
                    allTransactions.Where(t => t.userName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                // Calculate total transactions count
                var totalTransactions = filteredTransactions.Count;

                // Calculate total pages
                var totalPages = (int)Math.Ceiling((double)totalTransactions / pageSize);

                // Apply pagination after filtering
                var transactions = filteredTransactions.Skip((pageNumber - 1) * pageSize)
                                                      .Take(pageSize)
                                                      .ToList();

                ViewBag.PageNumber = pageNumber; // Pass pageNumber to the view
                ViewBag.TotalPages = totalPages; // Pass totalPages to the view
                ViewBag.SearchString = searchString; // Pass searchString to the view

                return View(transactions);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                TempData["ErrorMessage"] = "An error occurred while retrieving transactions: " + ex.Message;
                return RedirectToAction(nameof(DisplayTransactions));
            }
        }






    }
}

