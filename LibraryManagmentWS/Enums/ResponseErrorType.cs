
namespace LibraryManagmentWS.Domain.Enums
{
	public enum ResponseErrorType : int
	{

		NoneError = 0,
		Credential = 1,
		UnexpectedError = 2,
		Unregistered = 3,
		WrongInputInformation = 4,
		ThisNameIsAlreadyExist = 40,
		ThisUsernameIsAlreadyExist = 50,
		AccessDenied = 100,
	}
}
