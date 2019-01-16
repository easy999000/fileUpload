namespace DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ConfigInfo")]
    public partial class ConfigInfo
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Effect { get; set; }

        [StringLength(200)]
        public string Path { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }
    }
}
