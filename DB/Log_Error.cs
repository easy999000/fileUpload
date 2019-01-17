namespace DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Log_Error")]
    public partial class Log_Error
    {
        public int ID { get; set; }

        public int? OperatorId { get; set; }

        [StringLength(50)]
        public string Account { get; set; }

        [StringLength(200)]
        public string ErrorContent { get; set; }

        public DateTime? ErrorTime { get; set; }


        [StringLength(200)]
        public string Remark { get; set; }
    }
}
