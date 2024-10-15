﻿using NominaWeb.Enum;
using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Models.Usuario
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public UserRole Role { get; set; }  // Asegúrate de tener un enum Role definidooo
    }
}
