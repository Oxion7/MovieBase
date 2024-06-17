using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MovieBase.ViewModel
{
    public class ChangeRoleViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public string Role { get; set; }
    }
}