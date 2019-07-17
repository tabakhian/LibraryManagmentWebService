using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace LibraryManagmentWS
{
	public class LibraryManagerService : LibraryManager.LibraryManagerBase
	{
		public DatabaseContext myDatabaseContext { get; set; }
		public LibraryManagerService(DatabaseContext databaseContext)
		{
			myDatabaseContext = databaseContext;
		}

		public override Task<HelloReply> ServerAvailable(HelloRequest request, ServerCallContext context)
		{
			return Task.FromResult(new HelloReply
			{
				IsSuccessfull = true
			});
		}
		public async override Task<LibraryRegisterReply> LibraryRegister(LibraryRegisterRequest request, ServerCallContext context)
		{
			if (string.IsNullOrEmpty(request.Name) || request.Name.Length < 2)
			{
				return (new LibraryRegisterReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					AuthCode = string.Empty
				});
			}

			Domain.Entities.Library oLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.Name.Trim().ToLower() == request.Name.Trim().ToLower())
				.FirstOrDefault();
			if (oLibrary != null)
			{
				return (new LibraryRegisterReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.ThisNameIsAlreadyExist,
					ErrorMessage = string.Empty,
					AuthCode = string.Empty
				});
			}

			oLibrary = new Domain.Entities.Library()
			{
				Name = request.Name,
				AuthCode = Convert.ToBase64String(System.Guid.NewGuid().ToByteArray()),
			};

			myDatabaseContext.Libraries.Add(oLibrary);
			await myDatabaseContext.SaveChangesAsync();

			return (new LibraryRegisterReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
				AuthCode = oLibrary.AuthCode,
			});
		}
		public async override Task<LibraryEditReply> LibraryEdit(LibraryEditRequest request, ServerCallContext context)
		{
			if (string.IsNullOrEmpty(request.AuthCode))
			{
				return (new LibraryEditReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.Library oLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.AuthCode == request.AuthCode)
				.FirstOrDefault();
			if (oLibrary == null)
			{
				return (new LibraryEditReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.Name) || request.Name.Length < 2)
			{
				return (new LibraryEditReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.Library oNameSearchLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.Name.Trim().ToLower() == request.Name.Trim().ToLower() && current.Id != oLibrary.Id)
				.FirstOrDefault();
			if (oNameSearchLibrary != null)
			{
				return (new LibraryEditReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.ThisNameIsAlreadyExist,
					ErrorMessage = string.Empty,
				});
			}

			oLibrary.Name = request.Name;
			myDatabaseContext.Entry(oLibrary).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			await myDatabaseContext.SaveChangesAsync();

			return (new LibraryEditReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			});
		}
		public async override Task<AddUserReply> AddUser(AddUserRequest request, ServerCallContext context)
		{
			if (string.IsNullOrEmpty(request.AuthCode))
			{
				return (new AddUserReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					UserAuthCode = string.Empty,
				});
			}

			Domain.Entities.Library oLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.AuthCode == request.AuthCode)
				.FirstOrDefault();
			if (oLibrary == null)
			{
				return (new AddUserReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
					UserAuthCode = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.Username) || request.Username.Length < 2)
			{
				return (new AddUserReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					UserAuthCode = string.Empty,
				});
			}

			Domain.Entities.User oUsernameSearch =
				myDatabaseContext.Users
				.Where(current => current.Username.Trim().ToLower() == request.Username.Trim().ToLower())
				.FirstOrDefault();
			if (oUsernameSearch != null)
			{
				return (new AddUserReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.ThisNameIsAlreadyExist,
					ErrorMessage = string.Empty,
					UserAuthCode = string.Empty,
				});
			}


			Domain.Entities.User oUser =
				new Domain.Entities.User()
				{
					FirstName = request.FirstName,
					LastName = request.LastName,
					Username = request.Username,
					Password = request.Password,
					PhoneNumberIsoCode = request.PhoneNumberIsoCode,
					PhoneNumber = request.PhoneNumber,
					EmailAddress = request.EmailAddress,
					Role = (Domain.Enums.Role)request.Role,
					Library = oLibrary,
					LibraryId = oLibrary.Id,
					UserAuthCode = (System.Guid.NewGuid().ToString() + System.Guid.NewGuid().ToString()),
				};

			myDatabaseContext.Users.Add(oUser);
			await myDatabaseContext.SaveChangesAsync();

			return (new AddUserReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
				UserAuthCode = oUser.UserAuthCode,
			});
		}
		public async override Task<AddBookReply> AddBook(AddBookRequest request, ServerCallContext context)
		{
			if (string.IsNullOrEmpty(request.AuthCode))
			{
				return (new AddBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					BookId = string.Empty,
				});
			}

			Domain.Entities.Library oLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.AuthCode == request.AuthCode)
				.FirstOrDefault();
			if (oLibrary == null)
			{
				return (new AddBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
					BookId = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new AddBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					BookId = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new AddBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
					BookId = string.Empty,
				});
			}

			Domain.Entities.Book oBookSearch =
				myDatabaseContext.Books
				.Where(current => current.Name.Trim().ToLower() == request.Name.Trim().ToLower())
				.FirstOrDefault();
			if (oBookSearch != null)
			{
				return (new AddBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.ThisNameIsAlreadyExist,
					ErrorMessage = string.Empty,
					BookId = string.Empty,
				});
			}


			Domain.Entities.Book oBook =
				new Domain.Entities.Book()
				{
					Name = request.Name,
					Author = request.Author,
					Translator = request.Translator,
					PublishedDate = request.PublishedDate,
					Publisher = request.Publisher,
					Circulation = request.Circulation,
					Price = request.Price,
					ISBN = request.ISBN,
					HasImage = request.HasImage,
					UploaderUser = oUser,
					UploaderUserId = oUser.Id,
				};


			string TextPath = "c:\\LibraryManagmentWebServiceData\\Book-" + oBook.Id.ToString();
			try
			{
				bool exists = Directory.Exists(TextPath);
				if (exists)
				{
					Directory.Delete(TextPath, true);
				}
				Directory.CreateDirectory(TextPath);
				StreamWriter Save = new StreamWriter(TextPath + "\\" + "Text.txt", true);
				Save.WriteLine(request.Text);
				Save.Close();
				if (request.HasImage)
				{
					MemoryStream oMemoryStream = new MemoryStream();
					request.ImageData.WriteTo(oMemoryStream);
					oMemoryStream.Seek(0, SeekOrigin.Begin);
					Image oImage = Image.FromStream(oMemoryStream);
					if (oImage == null)
					{
						return (new AddBookReply
						{
							IsSuccessfull = false,
							ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
							ErrorMessage = string.Empty,
							BookId = string.Empty,
						});
					}
					if (oImage.Height < 100 || oImage.Width < 100)
					{
						return (new AddBookReply
						{
							IsSuccessfull = false,
							ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
							ErrorMessage = string.Empty,
							BookId = string.Empty,
						});
					}
					if (oImage.Height > 2000 || oImage.Width > 2000)
					{
						return (new AddBookReply
						{
							IsSuccessfull = false,
							ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
							ErrorMessage = string.Empty,
							BookId = string.Empty,
						});
					}
					oImage.Save(TextPath + "\\" + "Image.jpg");
					oBook.HasImage = true;
				}
			}
			catch (Exception)
			{
				return (new AddBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					BookId = string.Empty,
				});
			}
		
			myDatabaseContext.Books.Add(oBook);
			await myDatabaseContext.SaveChangesAsync();

			return (new AddBookReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
				BookId = oBook.Id.ToString(),
			});
		}
		public async override Task<RemoveBookReply> RemoveBook(RemoveBookRequest request, ServerCallContext context)
		{
			if (string.IsNullOrEmpty(request.AuthCode))
			{
				return (new RemoveBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.Library oLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.AuthCode == request.AuthCode)
				.FirstOrDefault();
			if (oLibrary == null)
			{
				return (new RemoveBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new RemoveBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new RemoveBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			System.Guid oBookGuidCode;
			bool oBookGuidCodeParsedSuccessfuly;
			oBookGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.BookId, out oBookGuidCode);
			if (!oBookGuidCodeParsedSuccessfuly)
			{
				return (new RemoveBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.Book oBookSearch =
				myDatabaseContext.Books
				.Where(current => current.Id == oBookGuidCode)
				.FirstOrDefault();
			if (oBookSearch == null)
			{
				return (new RemoveBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			bool CanRemove =
				oUser.UploadedBooks
				.Where(current => current.Id == oBookSearch.Id)
				.Any();

			string TextPath = "c:\\LibraryManagmentWebServiceData\\Book-" + oBookSearch.Id.ToString();
			try
			{
				bool exists = Directory.Exists(TextPath);
				if (exists)
				{
					Directory.Delete(TextPath, true);
				}
			}
			catch (Exception)
			{

			}

			myDatabaseContext.Books.Remove(oBookSearch);
			await myDatabaseContext.SaveChangesAsync();
		
			return (new RemoveBookReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			});
		}
		public override Task<BookDetailReply> BookDetail(BookDetailRequest request, ServerCallContext context)
		{
		
			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (Task.FromResult(new BookDetailReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				}));
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (Task.FromResult(new BookDetailReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				}));
			}

			if (string.IsNullOrEmpty(request.BookId))
			{
				return (Task.FromResult(new BookDetailReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				}));
			}

			System.Guid oBookGuidCode;
			bool oBookGuidCodeParsedSuccessfuly;
			oBookGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.BookId, out oBookGuidCode);
			if (!oBookGuidCodeParsedSuccessfuly)
			{
				return (Task.FromResult(new BookDetailReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				}));
			}

			Domain.Entities.Book oBook =
			myDatabaseContext.Books
			.Where(current => current.Id == oBookGuidCode)
			.FirstOrDefault();
			if (oBook == null)
			{
				return (Task.FromResult(new BookDetailReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				}));
			}

			bool CanRead =
				oUser.Renteds
				.Where(current => current.BookId == oBook.Id)
				.Any();

			string BookText = string.Empty;
			if (CanRead)
			{
				string TextPath = "c:\\LibraryManagmentWebServiceData\\Book-" + oBook.Id.ToString();
				try
				{
					bool exists = Directory.Exists(TextPath);
					if (!exists)
					{
						Directory.CreateDirectory(TextPath);
					}
					string TextFullPath = TextPath + "\\" + "Text.txt";

					if (!File.Exists(TextFullPath))
					{
						File.CreateText(TextFullPath);
						BookText = "Book Text isnt Available! Write SomeThings...";
					}
					else
					{
						StreamReader read = new StreamReader(TextFullPath);
						BookText = read.ReadToEnd();
						read.Close();
					}
				}
				catch (Exception )
				{
					return (Task.FromResult(new BookDetailReply
					{
						IsSuccessfull = false,
						ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
						ErrorMessage = string.Empty,
					}));
				}

			}

			bool hostLibrary =
				oBook.UploaderUser.Library.Id == oUser.Library.Id;

			Google.Protobuf.ByteString imageData = null;

			System.IO.MemoryStream oMemoryStreamImage =
					ImageUtils.GetPhoto("c:\\LibraryManagmentWebServiceData\\Book-" + oBook.Id.ToString() + "\\" + "Image.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
			oMemoryStreamImage.Seek(0, SeekOrigin.Begin);

			imageData =
				Google.Protobuf.ByteString.FromStream(oMemoryStreamImage);

			
			bool RentedThisBook =
				oBook.BookRents.Where(current => current.RenterUserId == oUser.Id)
				.Any();
			string strRentId = string.Empty;
			if (RentedThisBook)
			{
				strRentId =
					oBook.BookRents.Where(current => current.RenterUserId == oUser.Id).OrderByDescending(current => current.RegisterDate).FirstOrDefault().Id.ToString();
			}

			bool RentedRequestPending =
				oBook.BookRentRequests.Where(current => current.RenterUserId == oUser.Id && current.Status == Domain.Enums.RentRequestStatus.Pending)
				.Any();
			string strRentRequestId = string.Empty;
			if (RentedRequestPending)
			{
				strRentRequestId =
				oBook.BookRentRequests.Where(current => current.RenterUserId == oUser.Id && current.Status == Domain.Enums.RentRequestStatus.Pending).OrderByDescending(current => current.RegisterDate).FirstOrDefault().Id.ToString();
			}

			string strLastRejectedResponse = string.Empty;
			if (oBook.BookRentRequests.Where(current => current.RenterUserId == oUser.Id && current.Status == Domain.Enums.RentRequestStatus.Rejected).Any())
			{
				strLastRejectedResponse =
					oBook.BookRentRequests.Where(current => current.RenterUserId == oUser.Id && current.Status == Domain.Enums.RentRequestStatus.Rejected).OrderByDescending(current => current.RegisterDate).FirstOrDefault().ResponseContext.ToString();
			}

			return (Task.FromResult(new BookDetailReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
				BookId = oBook.Id.ToString(),
				Name = oBook.Name,
				Translator = oBook.Translator,
				Author = oBook.Author,
				Publisher = oBook.Translator,
				PublishedDate = oBook.PublishedDate,
				Circulation = oBook.Circulation,
				ISBN = oBook.ISBN,
				Price = (Int64)oBook.Price,
				LibraryId = oBook.UploaderUser.LibraryId.ToString(),
				LibraryName = oBook.UploaderUser.Library.Name,
				HasImage = oBook.HasImage,
				CanRead = CanRead,
				HostLibrary = hostLibrary,
				ImageData = imageData,
				Text = BookText,
				RentedThisBook = RentedThisBook,
				RentId = strRentId,
				RentRequestedPendingNow = RentedRequestPending,
				RentRequestId = strRentRequestId,
				LastRentRequestedRejectedResponseContext = strLastRejectedResponse,
			}));
		}
		public override Task<BookListReply> BookList(BookListRequest request, ServerCallContext context)
		{

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (Task.FromResult(new BookListReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				}));
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (Task.FromResult(new BookListReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				}));
			}


			var booksList =
				myDatabaseContext.Books.ToList();

			System.Collections.Generic.List<BookDetailReply> ReturnBooksList =
					new List<BookDetailReply>();

			foreach (var book in booksList)
			{
				bool CanRead =
				oUser.Renteds
				.Where(current => current.BookId == book.Id)
				.Any();

				string BookText = string.Empty;
				if (CanRead)
				{
					string TextPath = "c:\\LibraryManagmentWebServiceData\\Book-" + book.Id.ToString();
					try
					{
						bool exists = Directory.Exists(TextPath);
						if (!exists)
						{
							Directory.CreateDirectory(TextPath);
						}
						string TextFullPath = TextPath + "\\" + "Text.txt";

						if (!File.Exists(TextFullPath))
						{
							File.CreateText(TextFullPath);
							BookText = "Book Text isnt Available! Write SomeThings...";
						}
						else
						{
							StreamReader read = new StreamReader(TextFullPath);
							BookText = read.ReadToEnd();
							read.Close();
						}
					}
					catch (Exception)
					{
						return (Task.FromResult(new BookListReply
						{
							IsSuccessfull = false,
							ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
							ErrorMessage = string.Empty,
						}));
					}

				}

				bool hostLibrary =
					book.UploaderUser.Library.Id == oUser.Library.Id;

				Google.Protobuf.ByteString imageData = null;

				System.IO.MemoryStream oMemoryStreamImage =
					ImageUtils.GetPhoto("c:\\LibraryManagmentWebServiceData\\Book-" + book.Id.ToString() + "\\" + "Image.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
				oMemoryStreamImage.Seek(0, SeekOrigin.Begin);

				imageData =
					Google.Protobuf.ByteString.FromStream(oMemoryStreamImage);

				
				ReturnBooksList.Add(new BookDetailReply
				{ 
					IsSuccessfull = true,
					ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
					ErrorMessage = string.Empty,
					BookId = book.Id.ToString(),
					Name = book.Name,
					Translator = book.Translator,
					Author = book.Author,
					Publisher = book.Translator,
					PublishedDate = book.PublishedDate,
					Circulation = book.Circulation,
					ISBN = book.ISBN,
					Price = (Int64)book.Price,
					LibraryId = book.UploaderUser.LibraryId.ToString(),
					LibraryName = book.UploaderUser.Library.Name,
					HasImage = book.HasImage,
					CanRead = CanRead,
					HostLibrary = hostLibrary,
					ImageData = imageData,
					Text = BookText,
				});
			}

			var RequestResult = new BookListReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			};
			RequestResult.Books.AddRange(ReturnBooksList);

			return (Task.FromResult(RequestResult));
		}
		public override Task<BookRentingSystemListReply> BookRentingSystemList(BookRentingSystemListRequest request, ServerCallContext context)
		{
			if (string.IsNullOrEmpty(request.AuthCode))
			{
				return (Task.FromResult(new BookRentingSystemListReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				}));
			}

			Domain.Entities.Library oLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.AuthCode == request.AuthCode)
				.FirstOrDefault();
			if (oLibrary == null)
			{
				return (Task.FromResult(new BookRentingSystemListReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				}));
			}

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (Task.FromResult(new BookRentingSystemListReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				}));
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (Task.FromResult(new BookRentingSystemListReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				}));
			}

			Google.Protobuf.ByteString GetImageByteString(string strUrl)
			{
				Google.Protobuf.ByteString imageData = null;

				System.IO.MemoryStream oMemoryStreamImage =
					ImageUtils.GetPhoto(strUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
				oMemoryStreamImage.Seek(0, SeekOrigin.Begin);

				imageData =
					Google.Protobuf.ByteString.FromStream(oMemoryStreamImage);

				return (imageData);
			}

			var RentRequests =
				myDatabaseContext.BookRentRequests
				.Where(current => current.Book.UploaderUserId == oUser.Id || (current.Book.UploaderUser.LibraryId == oUser.LibraryId && oUser.Role >= Domain.Enums.Role.Administrator))
				.Where(current => current.Status == Domain.Enums.RentRequestStatus.Pending)
				.ToList()
				.Select(current => new BookNewRentRequestList
				{
					RentRequestCode = current.Id.ToString(),
					BookId = current.BookId.ToString(),
					BookName = current.Book.Name,
					Context = current.Context,
					Time = current.RegisterDate.ToString(),
					UserFullName = current.RenterUser.FirstName + " " + current.RenterUser.LastName,
					UserId = current.RenterUserId.ToString(),
					UserLibraryId = current.RenterUser.LibraryId.ToString(),
					UserLibraryName = current.RenterUser.Library.Name,
					BookImageData = GetImageByteString("c:\\LibraryManagmentWebServiceData\\Book-" + current.BookId.ToString() + "\\" + "Image.jpg"),
				})
				.ToList();

			var BookRenteds =
				myDatabaseContext.BookRents
				.Where(current => current.Book.UploaderUserId == oUser.Id || (current.Book.UploaderUser.LibraryId == oUser.LibraryId && oUser.Role >= Domain.Enums.Role.Administrator))
				.ToList()
				.Select(current => new BookRentedList
				{
					RentCode = current.Id.ToString(),
					BookId = current.BookId.ToString(),
					BookName = current.Book.Name,
					Time = current.RegisterDate.ToString(),
					UserFullName = current.RenterUser.FirstName + " " + current.RenterUser.LastName,
					UserId = current.RenterUserId.ToString(),
					UserLibraryId = current.RenterUser.LibraryId.ToString(),
					UserLibraryName = current.RenterUser.Library.Name,
					BookImageData = GetImageByteString("c:\\LibraryManagmentWebServiceData\\Book-" + current.BookId.ToString() + "\\" + "Image.jpg"),
				})
				.ToList();

			var RentedBooks =
				myDatabaseContext.BookRents
				.Where(current => current.RenterUserId == oUser.Id)
				.ToList()
				.Select(current => new RentedBookList
				{
					RentCode = current.Id.ToString(),
					BookId = current.BookId.ToString(),
					BookName = current.Book.Name,
					Time = current.RegisterDate.ToString(),
					BookLibraryId = current.Book.UploaderUser.LibraryId.ToString(),
					BookLibraryName = current.Book.UploaderUser.Library.Name,
					BookImageData = GetImageByteString("c:\\LibraryManagmentWebServiceData\\Book-" + current.BookId.ToString() + "\\" + "Image.jpg"),
				})
				.ToList();

			var RequestResult = new BookRentingSystemListReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,			
			};
			RequestResult.NewRentRequestList.AddRange(RentRequests);
			RequestResult.BookRentedList.AddRange(BookRenteds);
			RequestResult.RentedBookList.AddRange(RentedBooks);

			return (Task.FromResult(RequestResult));
		}
		public override Task<BookRentingSystemNotificationCountReply> BookRentingSystemNotificationCount(BookRentingSystemNotificationCountRequest request, ServerCallContext context)
		{
			if (string.IsNullOrEmpty(request.AuthCode))
			{
				return (Task.FromResult(new BookRentingSystemNotificationCountReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					NewRentRequestCount = 0,
				}));
			}

			Domain.Entities.Library oLibrary =
				myDatabaseContext.Libraries
				.Where(current => current.AuthCode == request.AuthCode)
				.FirstOrDefault();
			if (oLibrary == null)
			{
				return (Task.FromResult(new BookRentingSystemNotificationCountReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
					NewRentRequestCount = 0,
				}));
			}

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (Task.FromResult(new BookRentingSystemNotificationCountReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
					NewRentRequestCount = 0,
				}));
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (Task.FromResult(new BookRentingSystemNotificationCountReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
					NewRentRequestCount = 0,
				}));
			}

			int RentRequestsCount =
				myDatabaseContext.BookRentRequests
				.Where(current => current.Book.UploaderUserId == oUser.Id || (current.Book.UploaderUser.LibraryId == oUser.LibraryId && oUser.Role >= Domain.Enums.Role.Administrator))		
				.Where(current=> current.Status == Domain.Enums.RentRequestStatus.Pending)
				.Count();

			var RequestResult = new BookRentingSystemNotificationCountReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
				NewRentRequestCount = RentRequestsCount,
			};

			return (Task.FromResult(RequestResult));
		}
		public async override Task<RentABookReply> RentABook(RentABookRequest request, ServerCallContext context)
		{
		
			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new RentABookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new RentABookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.BookId))
			{
				return (new RentABookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			System.Guid oBookGuidCode;
			bool oBookGuidCodeParsedSuccessfuly;
			oBookGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.BookId, out oBookGuidCode);
			if (!oBookGuidCodeParsedSuccessfuly)
			{
				return (new RentABookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}


			Domain.Entities.Book oBook =
			myDatabaseContext.Books
			.Where(current => current.Id == oBookGuidCode)
			.FirstOrDefault();
			if (oBook == null)
			{
				return (new RentABookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.BookRentRequest oRentRequest =
				new Domain.Entities.BookRentRequest()
				{
					Book = oBook,
					BookId = oBook.Id,
					Context = request.Context,
					RenterUser = oUser,
					RenterUserId = oUser.Id,
					Status = Domain.Enums.RentRequestStatus.Pending,
				};
		
			myDatabaseContext.BookRentRequests.Add(oRentRequest);
			await myDatabaseContext.SaveChangesAsync();

			return (new RentABookReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
				RentRequestId = oRentRequest.Id.ToString(),
			});
		}
		public async override Task<AcceptRentRequestReply> AcceptRentRequest(AcceptRentRequestRequest request, ServerCallContext context)
		{

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new AcceptRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new AcceptRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.RentRequestId))
			{
				return (new AcceptRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			System.Guid oRentRequestGuidCode;
			bool oRentRequestGuidCodeParsedSuccessfuly;
			oRentRequestGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.RentRequestId, out oRentRequestGuidCode);
			if (!oRentRequestGuidCodeParsedSuccessfuly)
			{
				return (new AcceptRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}


			Domain.Entities.BookRentRequest oRentRequest =
				myDatabaseContext.BookRentRequests
			.Where(current => current.Id == oRentRequestGuidCode)
			.FirstOrDefault();
			if (oRentRequest == null)
			{
				return (new AcceptRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			oRentRequest.Status = Domain.Enums.RentRequestStatus.Accepted;
			myDatabaseContext.Entry(oRentRequest).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

			Domain.Entities.BookRent oBookRent =
				new Domain.Entities.BookRent()
				{
					Book = oRentRequest.Book,
					BookId = oRentRequest.BookId,
					RenterUser = oRentRequest.RenterUser,
					RenterUserId = oRentRequest.RenterUserId,
				};

			myDatabaseContext.BookRents.Add(oBookRent);
			await myDatabaseContext.SaveChangesAsync();

			return (new AcceptRentRequestReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			});
		}
		public async override Task<RejectRentRequestReply> RejectRentRequest(RejectRentRequestRequest request, ServerCallContext context)
		{

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new RejectRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new RejectRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.RentRequestId))
			{
				return (new RejectRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			System.Guid oRentRequestGuidCode;
			bool oRentRequestGuidCodeParsedSuccessfuly;
			oRentRequestGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.RentRequestId, out oRentRequestGuidCode);
			if (!oRentRequestGuidCodeParsedSuccessfuly)
			{
				return (new RejectRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}


			Domain.Entities.BookRentRequest oRentRequest =
				myDatabaseContext.BookRentRequests
			.Where(current => current.Id == oRentRequestGuidCode)
			.FirstOrDefault();
			if (oRentRequest == null)
			{
				return (new RejectRentRequestReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}
			oRentRequest.Status = Domain.Enums.RentRequestStatus.Rejected;
			oRentRequest.ResponseContext = request.ResponseContext;

			myDatabaseContext.Entry(oRentRequest).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			await myDatabaseContext.SaveChangesAsync();

			return (new RejectRentRequestReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			});
		}
		public async override Task<CancelRentRequestBookReply> CancelRentRequestBook(CancelRentRequestBookRequest request, ServerCallContext context)
		{

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new CancelRentRequestBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new CancelRentRequestBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.RentRequestId))
			{
				return (new CancelRentRequestBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			System.Guid oRentRequestGuidCode;
			bool oRentRequestGuidCodeParsedSuccessfuly;
			oRentRequestGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.RentRequestId, out oRentRequestGuidCode);
			if (!oRentRequestGuidCodeParsedSuccessfuly)
			{
				return (new CancelRentRequestBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}


			Domain.Entities.BookRentRequest oRentRequest =
				myDatabaseContext.BookRentRequests
			.Where(current => current.Id == oRentRequestGuidCode)
			.FirstOrDefault();
			if (oRentRequest == null)
			{
				return (new CancelRentRequestBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			myDatabaseContext.BookRentRequests.Remove(oRentRequest);
			await myDatabaseContext.SaveChangesAsync();

			return (new CancelRentRequestBookReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			});
		}
		public async override Task<CancelRentBookReply> CancelRentBook(CancelRentBookRequest request, ServerCallContext context)
		{

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new CancelRentBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new CancelRentBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.RentId))
			{
				return (new CancelRentBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			System.Guid oRentGuidCode;
			bool oRentGuidCodeParsedSuccessfuly;
			oRentGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.RentId, out oRentGuidCode);
			if (!oRentGuidCodeParsedSuccessfuly)
			{
				return (new CancelRentBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.BookRent oRent =
			myDatabaseContext.BookRents
			.Where(current => current.Id == oRentGuidCode)
			.FirstOrDefault();
			if (oRent == null)
			{
				return (new CancelRentBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			myDatabaseContext.BookRents.Remove(oRent);
			await myDatabaseContext.SaveChangesAsync();

			return (new CancelRentBookReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			});
		}
		public async override Task<ReturnRentedBookReply> ReturnRentedBook(ReturnRentedBookRequest request, ServerCallContext context)
		{

			if (string.IsNullOrEmpty(request.UserAuthCode))
			{
				return (new ReturnRentedBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.User oUser =
				myDatabaseContext.Users
				.Where(current => current.UserAuthCode == request.UserAuthCode)
				.FirstOrDefault();
			if (oUser == null)
			{
				return (new ReturnRentedBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.Credential,
					ErrorMessage = string.Empty,
				});
			}

			if (string.IsNullOrEmpty(request.RentId))
			{
				return (new ReturnRentedBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			System.Guid oRentGuidCode;
			bool oRentGuidCodeParsedSuccessfuly;
			oRentGuidCodeParsedSuccessfuly = System.Guid.TryParse(request.RentId, out oRentGuidCode);
			if (!oRentGuidCodeParsedSuccessfuly)
			{
				return (new ReturnRentedBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			Domain.Entities.BookRent oRent =
			myDatabaseContext.BookRents
			.Where(current => current.Id == oRentGuidCode)
			.FirstOrDefault();
			if (oRent == null)
			{
				return (new ReturnRentedBookReply
				{
					IsSuccessfull = false,
					ErrorType = (int)Domain.Enums.ResponseErrorType.WrongInputInformation,
					ErrorMessage = string.Empty,
				});
			}

			myDatabaseContext.BookRents.Remove(oRent);
			await myDatabaseContext.SaveChangesAsync();

			return (new ReturnRentedBookReply
			{
				IsSuccessfull = true,
				ErrorType = (int)Domain.Enums.ResponseErrorType.NoneError,
				ErrorMessage = string.Empty,
			});
		}
	}
}
