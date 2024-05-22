using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ViewModel.User
{
    public class TransactionDto
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePlaced { get; set; }
        public string? Status { get; set; }
        public string? WithdrawalMethod { get; set; }


    }
}
            