using System.Collections.Generic;
using System.Threading.Tasks;

namespace BCMStrategy.Common.Email
{
	public interface IEmailHelper
	{
		/// <summary>
		/// Gets or sets the configuration.
		/// </summary>
		/// <value>
		/// The configuration.
		/// </value>
		EmailConfiguration Configuration { get; set; }

		/// <summary>
		/// Method to send Email
		/// </summary>
		/// <param name="toAddresses">to Addresses</param>
		/// <param name="ccAddresses">cc Addresses</param>
		/// <param name="subject">The subject</param>
		/// <param name="message">The message</param>
		/// <returns>Returns whether Email has been send or not</returns>
		bool Send(List<string> toAddresses, List<string> ccAddresses, string subject, string message);

		/// <summary>
		/// Sends the specified to addresses.
		/// </summary>
		/// <param name="toAddresses">To addresses.</param>
		/// <param name="ccAddresses">The cc addresses.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="message">The message.</param>
		/// <param name="htmlMessage">The HTML message.</param>
		/// <returns>Returns whether Email has been send or not</returns>
		bool Send(string toAddresses, string subject, string message);

		bool SendEmailWithEmbeddedImage(string emailSubject, string emailBody, string toAddress, byte[] imageBytes);
	}
}