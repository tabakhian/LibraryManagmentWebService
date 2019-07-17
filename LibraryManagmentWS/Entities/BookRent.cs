namespace LibraryManagmentWS.Domain.Entities
{
	public class BookRent
	{

		public BookRent()
		{
			Id = System.Guid.NewGuid();
			RegisterDate = System.DateTime.UtcNow;
		}

		public System.Guid Id { get; set; }
		public System.DateTime RegisterDate { get; set; }


		public virtual Book Book { get; set; }
		public System.Guid? BookId { get; set; }

		public virtual User RenterUser { get; set; }
		public System.Guid? RenterUserId { get; set; }

	}
}