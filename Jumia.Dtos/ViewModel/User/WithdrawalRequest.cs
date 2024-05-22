using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.User
{
    public class WithdrawalRequest
    {
        public string UserId { get; set; }
        public decimal RequestedAmount { get; set; }
        public string? WithdrawalMethod { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; } = "Pending";
        public decimal? NumberOfWithdrawl { get; set; }
    }
}
