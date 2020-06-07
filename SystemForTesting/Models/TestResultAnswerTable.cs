namespace SystemForTesting.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestResultAnswerTable")]
    public partial class TestResultAnswerTable
    {
        public long Id { get; set; }

        public int? IdTestResult { get; set; }

        public long? IdAnswer { get; set; }

        public virtual Answer Answer { get; set; }

        public virtual TestResult TestResult { get; set; }
    }
}
