using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagmentWS
{

	public class DatabaseContext : DbContext
	{
		public DbSet<Domain.Entities.User> Users { get; set; }
		public DbSet<Domain.Entities.Book> Books { get; set; }
		public DbSet<Domain.Entities.Library> Libraries { get; set; }
		public DbSet<Domain.Entities.BookRent> BookRents { get; set; }
		public DbSet<Domain.Entities.BookRentRequest> BookRentRequests { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder
				.UseLazyLoadingProxies()
				.UseSqlite("Data Source=LibraryWS.db");
		}
	}
}