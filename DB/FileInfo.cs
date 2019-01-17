namespace DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FileInfo")]
    public partial class FileInfo
    {
        public int ID { get; set; }

        public int? UserId { get; set; }

        [StringLength(200)]
        public string FilePath { get; set; }

        [StringLength(200)]
        public string FileName { get; set; }

        public DateTime? UpLoadTime { get; set; }

        public int Download { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }
    }
}
