using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrivateChat.Api.DataModels;
using PrivateChat.Api.Entity.Configuration;
using entityUser = PrivateChat.Api.Entity.Models;

namespace PrivateChat.Api.DBContext
{
    public class AdminContext: IdentityDbContext<entityUser.User>
    {
        //private readonly DbContext _dbContext;
        public AdminContext(DbContextOptions<AdminContext> options): base(options)
        {
            //_dbContext = dbContext;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }


        public DbSet<User> User { get; set; }
        public DbSet<entityUser.User> UserEn { get; set; } 
        public DbSet<Chat> Chat { get; set; }
        public DbSet<Comments> Comments { get; set; }

        public DbSet<Message> Message { get; set; }
    }
}
