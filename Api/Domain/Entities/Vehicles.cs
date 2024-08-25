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
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = default!;
        [Required]
        public int Year { get; set; } = default!;
    }
}