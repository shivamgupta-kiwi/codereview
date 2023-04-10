using BCMStrategy.Common.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mime;

namespace BCMStrategy.Email
{
	public class EmailHelper : IEmailHelper
	{
		/// <summary>
		/// Gets or sets Email Configuration
		/// </summary>
		public EmailConfiguration Configuration { get; set; }

		/// <summary>
		/// Sends Email message to specified addresses
		/// </summary>
		/// <param name="toAddresses">To Address</param>
		/// <param name="ccAddresses">CC Address</param>
		/// <param name="subject">Email Subject</param>
		/// <param name="message">Email Message</param>
		/// <returns>Boolean value whether Email has been sent or not.</returns>
		public bool Send(List<string> toAddresses, List<string> ccAddresses, string subject, string message)
		{
			try
			{
				var mail = new MailMessage();

				if (toAddresses == null)
					throw new ArgumentNullException("toAddresses");

				foreach (var addr in toAddresses)
				{
					mail.To.Add(addr);
				}

				if (ccAddresses != null)
				{
					foreach (var addr in ccAddresses)
					{
						mail.CC.Add(addr);
					}
				}

				mail.From = new MailAddress(Configuration.FromAddress);
				mail.Sender = new MailAddress(Configuration.FromAddress);
				mail.Subject = subject;
				mail.Body = message;
				mail.IsBodyHtml = true;

				var client = new SmtpClient(Configuration.SmtpServer, Configuration.SmtpPort);
				client.Credentials = Configuration.Credentials;

				client.Send(mail);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Sends the specified to address.
		/// </summary>
		/// <param name="toAddresses">To address.</param>
		/// <param name="ccAddress">The cc address.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="message">The message.</param>
		/// <param name="htmlMessage">The HTML message.</param>
		/// <returns>Boolean value whether Email has been sent or not</returns>
		public bool Send(string toAddresses, string subject, string message)
		{
			try
			{

				var mail = new MailMessage();

				mail.To.Add(toAddresses);
				mail.From = new MailAddress(Configuration.FromAddress);
				mail.Sender = new MailAddress(Configuration.FromAddress);
				mail.Subject = subject;
				mail.Body = message;
				mail.IsBodyHtml = true;

				var client = new SmtpClient(Configuration.SmtpServer, Configuration.SmtpPort);
				client.EnableSsl = Configuration.UseSsl;

				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.UseDefaultCredentials = false;

				Configuration.Credentials.UserName = Configuration.UserName;

				client.Credentials = Configuration.Credentials;

				client.Send(mail);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool SendEmailWithEmbeddedImage(string emailSubject, string emailBody, string toAddress, byte[] imageBytes)
		{
			try
			{
				using (var smtp = new SmtpClient())
				{
					AlternateView htmlView = AlternateView.CreateAlternateViewFromString(emailBody, null, MediaTypeNames.Text.Html);
					// Create a LinkedResource object for each embedded image
					MemoryStream stream = new MemoryStream(imageBytes);
					LinkedResource dashboardChart = new LinkedResource(stream, MediaTypeNames.Image.Jpeg);
					dashboardChart.ContentId = "DashboardChartImage";
					htmlView.LinkedResources.Add(dashboardChart);

					var mail = new MailMessage();
					mail.AlternateViews.Add(htmlView);
					mail.To.Add(toAddress);
					mail.From = new MailAddress(Configuration.FromAddress);
					mail.Sender = new MailAddress(Configuration.FromAddress);
					mail.Subject = emailSubject;
					mail.Body = String.Format(emailBody, dashboardChart.ContentId);
					mail.IsBodyHtml = true;

					var client = new SmtpClient(Configuration.SmtpServer, Configuration.SmtpPort);
					client.EnableSsl = Configuration.UseSsl;
					client.DeliveryMethod = SmtpDeliveryMethod.Network;
					client.UseDefaultCredentials = false;
					Configuration.Credentials.UserName = Configuration.UserName;
					client.Credentials = Configuration.Credentials;

					client.Send(mail);
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
