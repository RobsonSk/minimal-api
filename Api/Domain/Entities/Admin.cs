using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using minimal_api.Infra;

namespace minimal_api.Domain.Entities
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Profile { get; set; } = default!;
    }
}