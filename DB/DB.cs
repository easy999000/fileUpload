namespace DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DB : DbContext
    {
        public DB()
            : base("name = DB")//"name =DB"
        {
           // base.Database.Connection.ConnectionString = "data source=192.168.1.182;initial catalog=Test;persist security info=True;user id=sa;password=ABCabc123;MultipleActiveResultSets=True;App=EntityFramework";
        }

        public virtual DbSet<UserInfo> UserInfo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
