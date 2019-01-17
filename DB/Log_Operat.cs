namespace DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Log_Operat")]
    public partial class Log_Operat
    {
        public int ID { get; set; }

        public int? OperatorId { get; set; }

        [StringLength(50)]
        public string OperatorName { get; set; }

        [StringLength(200)]
        public string LogContent { get; set; }

        public DateTime? OperatTime { get; set; }


        [StringLength(200)]
        public string Remark { get; set; }
    }
}
