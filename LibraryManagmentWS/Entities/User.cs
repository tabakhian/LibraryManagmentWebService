using System.Collections.Generic;
namespace LibraryManagmentWS.Domain.Entities
{
	public class User
	{
		public User()
		{
			Id = System.Guid.NewGuid();
			RegisterDate = System.DateTime.UtcNow;
			IsPhoneNumberVerified = false;
			IsEmailAddressVerified = false;
			Role = Enums.Role.User;
			Gender = Enums.Gender.Unspecified;
			UploadedBooks = new System.Collections.Generic.HashSet<Book>();
			Renteds = new System.Collections.Generic.HashSet<BookRent>();
			RentedRequests = new System.Collections.Generic.HashSet<BookRentRequest>();
		}

		public System.Guid Id { get; set; }
		public System.DateTime RegisterDate { get; set; }
		public string UserAuthCode { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string PhoneNumber { get; set; }
		public string PhoneNumberIsoCode { get; set; }

		public bool IsPhoneNumberVerified { get; set; }

		public System.DateTime? PhoneNumberVerificationDateTime { get; set; }

		public string EmailAddress { get; set; }

		public bool IsEmailAddressVerified { get; set; }

		public System.DateTime? EmailAddressVerificationDateTime { get; set; }

		public Domain.Enums.Gender Gender { get; set; }

		public Domain.Enums.Role Role { get; set; }

		public virtual System.Collections.Generic.ICollection<Book> UploadedBooks { get; set; }

		public virtual Library Library { get; set; }
		public System.Guid? LibraryId { get; set; }

		public virtual System.Collections.Generic.ICollection<BookRent> Renteds { get; set; }
		public virtual System.Collections.Generic.ICollection<BookRentRequest> RentedRequests { get; set; }

	}
}
