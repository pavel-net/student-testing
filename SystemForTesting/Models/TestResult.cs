namespace SystemForTesting.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TestResult
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TestResult()
        {
            TestResultAnswerTables = new HashSet<TestResultAnswerTable>();
        }

        public int Id { get; set; }

        public int? IdTest { get; set; }

        public int? IdStudent { get; set; }

        public int TotalScore { get; set; }

        public DateTime? TestDate { get; set; }

        public int Duration { get; set; }

        [StringLength(4)]
        public string Rating1 { get; set; }

        public double Rating2 { get; set; }

        [StringLength(32)]
        public string Rating3 { get; set; }

        public virtual Student Student { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResultAnswerTable> TestResultAnswerTables { get; set; }

        public virtual Test Test { get; set; }
    }
}
