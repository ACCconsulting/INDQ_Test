using Dominio;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistencia
{
   public class IndqContext: IdentityDbContext<User>
    {
        public IndqContext(DbContextOptions options) : base(options)
        {

        }

      public  DbSet<Events> Events { get; set; }
      public  DbSet<Attendance> Attendances { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Attendance>().HasOne(u => u.User).WithMany().HasForeignKey(f => f.UserId);
            //modelBuilder.Entity<Attendance>().HasOne(u => u.Events).WithMany().HasForeignKey(f => f.EventsId);

        }

    }
}
