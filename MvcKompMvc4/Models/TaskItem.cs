using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcKompApp.Models
{
    // 1. Create model
    // 2. Create Data Context
    // 3. Add Controller using the model and context created

    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage="Task Title is required")]
        [StringLength(25)]
        public string Task { get; set; }

       // [Required(ErrorMessage = "Task Title is required")]
        [StringLength(256)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        
        
        public bool Completed  { get; set; }     
        
        [DataType(DataType.DateTime)]
        public DateTime EntryDate { get; set; }
    }

    /* There are two tables, one for the TaskItems entity set and then the EdmMetadata table.
     * The EdmMetadata table is used by the Entity Framework to determine when the model and the database are out of sync.
     
     * Entity Framework is smart and look up for the connection string with the same name of the Data Context, in this case TaskDbContext
     
     *By default the database is created and provisioned using SqlClient against localhost\SQLEXPRESS and has the same name as the derived context. This convention is configurable and is controlled by an AppDomain setting that can be tweaked or replaced. You can tweak the default SqlClient convention to connect to a different database, replace it with a SqlCe convention that we include or define your own convention by implementing the IDbConnectionFactory interface.
 
public interface IDbConnectionFactory
{
    DbConnection CreateConnection(string databaseName);
}
 
The active IDbConnectionFactory can be retrieved or set via the static property, Database.DefaultConnectionFactory.
 
     */
    public class TaskDBContext : DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>().HasKey<int>(t => t.Id);

            base.OnModelCreating(modelBuilder);
        }
    }

    public class TaskDbContextInitializer : DropCreateDatabaseIfModelChanges<TaskDBContext>
    {
        protected override void Seed(TaskDBContext context)
        {
            var bugghinaTask = new TaskItem { Completed = false, EntryDate = DateTime.Now, Description = "Love my pet", Task = "Bugghina" };
            context.Tasks.Add(bugghinaTask);
            base.Seed(context);
        }
    }
}