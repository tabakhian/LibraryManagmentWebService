using System.Collections.Generic;
namespace LibraryManagmentWS.Domain.Entities
{
	public class Library 
	{
		public Library()
		{
			Id = System.Guid.NewGuid();
			RegisterDate = System.DateTime.UtcNow;
			ConnectedUsers = new System.Collections.Generic.HashSet<User>();
		}

		public System.Guid Id { get; set; }
		public System.DateTime RegisterDate { get; set; }

		public string AuthCode { get; set; }
		public string Name { get; set; }

		public virtual System.Collections.Generic.ICollection<User> ConnectedUsers { get; set; }

	}
}
