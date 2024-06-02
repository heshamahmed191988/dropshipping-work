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


        public async Task<ActionResult> DisplayTransactions(string searchString, int pageNumber = 1, int pageSize = 30, DateTime? startDate = null, DateTime? endDate = null)
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
                    pageSize = 30; // Default page size
                }

                // Fetch transactions from the service with date filtration
                var allTransactions = await _userService.GetTransactionsWithPaginationAsync(0, int.MaxValue, startDate, endDate);

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
                ViewBag.StartDate = startDate; // Pass startDate to the view
                ViewBag.EndDate = endDate; // Pass endDate to the view

                return View(transactions);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                TempData["ErrorMessage"] = "An error occurred while retrieving transactions: " + ex.Message;
                return RedirectToAction(nameof(DisplayTransactions));
            }
        }



        [HttpPost]
        public async Task<IActionResult> UpdateTransactionStatus(int transactionId, string status)
        {
            try
            {
                await _userService.UpdateTransactionStatusAsync(transactionId, status);
                TempData["SuccessMessage"] = "Transaction status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the transaction status: " + ex.Message;
            }

            return RedirectToAction(nameof(DisplayTransactions));
        }



    }
}

