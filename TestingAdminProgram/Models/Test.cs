namespace TestingAdminProgram.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Test
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Test()
        {
            TestResults = new HashSet<TestResult>();
        }

        public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        public int? IdTopic { get; set; }

        public int? IdDiscipline { get; set; }

        public int CountQuestions { get; set; }

        public virtual Discipline Discipline { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResult> TestResults { get; set; }

        public virtual Topic Topic { get; set; }
    }
}
