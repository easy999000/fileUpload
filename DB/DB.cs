namespace DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DB : DbContext
    {
        public DB()
            : base("name=DB")
        {
        }

        public virtual DbSet<ConfigInfo> ConfigInfo { get; set; }
        public virtual DbSet<FileInfo> FileInfo { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<Log_Operat> Log_Operat { get; set; }
        public virtual DbSet<Log_Error> Log_Error { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
