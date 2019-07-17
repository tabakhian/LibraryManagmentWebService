namespace LibraryManagmentWS.Domain.Entities
{
	public class Book
	{

		public Book()
		{
			Id = System.Guid.NewGuid();
			RegisterDate = System.DateTime.UtcNow;
			HasImage = false;
			BookRents = new System.Collections.Generic.HashSet<BookRent>();
			BookRentRequests = new System.Collections.Generic.HashSet<BookRentRequest>();

		}

		public System.Guid Id { get; set; }
		public System.DateTime RegisterDate { get; set; }

		public string Name { get; set; }

		public string Author { get; set; }

		public string Translator { get; set; }

		public string Publisher { get; set; }

		public int PublishedDate { get; set; }

		public string Circulation { get; set; }

		public double Price { get; set; }

		public string ISBN { get; set; }

		public bool HasImage { get; set; }

		public virtual User UploaderUser { get; set; }
		public System.Guid? UploaderUserId { get; set; }

		public virtual System.Collections.Generic.ICollection<BookRent> BookRents { get; set; }
		public virtual System.Collections.Generic.ICollection<BookRentRequest> BookRentRequests { get; set; }

	}
}