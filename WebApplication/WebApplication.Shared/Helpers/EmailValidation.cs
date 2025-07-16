using System.Text.RegularExpressions;

namespace WebApplication.Shared.Helpers
{

	public static class EmailValidation
	{
		private static readonly Regex _emailRegex = new(
			@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static bool IsValid(string email)
		{
			return !string.IsNullOrWhiteSpace(email) && _emailRegex.IsMatch(email);
		}
	}
}
