namespace TestingAdminProgram.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Answer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Answer()
        {
            TestResultAnswerTables = new HashSet<TestResultAnswerTable>();
        }

        public long Id { get; set; }

        public long? IdQuestion { get; set; }

        public string Content { get; set; }

        public byte[] ContentImage { get; set; }

        [StringLength(1)]
        public string FlagCorrectly { get; set; }

        public virtual Question Question { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResultAnswerTable> TestResultAnswerTables { get; set; }
    }
}
