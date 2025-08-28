using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.UserManagement
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string? PasswordHash { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public AccountType AccountType { get; set; }
        public UserStatus UserStatus { get; set; }
        public Gender Gender { get; set; }
    }
}
