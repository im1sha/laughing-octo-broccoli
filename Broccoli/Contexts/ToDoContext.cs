using Microsoft.EntityFrameworkCore;
using Broccoli.Entities;

namespace Broccoli.Contexts
{
    public class ToDoContext : DbContext
    {
        public DbSet<ToDo> ToDos { get; set; }
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {
        }
    }
}
