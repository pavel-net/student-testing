namespace TestingAdminProgram.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Teacher
    {
        public int Id { get; set; }

        [StringLength(256)]
        public string Fio { get; set; }

        [StringLength(128)]
        public string Post { get; set; }

        [StringLength(128)]
        public string Department { get; set; }

        [Required]
        [StringLength(32)]
        public string Login { get; set; }

        [StringLength(32)]
        public string Password { get; set; }
    }
}
