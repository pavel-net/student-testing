using System.Text.Json.Serialization;

namespace SystemForTesting.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        [JsonIgnore]
        public long Id { get; set; }

        [JsonIgnore]
        public int? IdTopic { get; set; }

        public string Content { get; set; }

        public byte[] ContentImage { get; set; }

        public int Duration { get; set; }

        public int Score { get; set; }

        [StringLength(512)]
        public string Hint { get; set; }

        [StringLength(512)]
        public string Hint2 { get; set; }

        [StringLength(512)]
        public string Hint3 { get; set; }

        public byte[] HintImage { get; set; }

        public byte[] Hint2Image { get; set; }

        public byte[] Hint3Image { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }
        
        [JsonIgnore]
        public virtual Topic Topic { get; set; }
    }
}
