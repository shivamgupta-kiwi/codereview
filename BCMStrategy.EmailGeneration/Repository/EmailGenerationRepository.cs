using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.EmailGeneration.Abstract;
using BCMStrategy.Logger;
using BCMStrategy.Resources;

namespace BCMStrategy.EmailGeneration.Repository
{
	public class EmailGenerationRepository : IEmailGeneration
	{
		private static readonly EventLogger<EmailGenerationRepository> log = new EventLogger<EmailGenerationRepository>();

		#region General Variables

		private readonly string DayOfDataRetrieval = ConfigurationManager.AppSettings["DayOfDataRetrieval"];

		private readonly string EmailDurationInHours = ConfigurationManager.AppSettings["EmailDurationInHours"];

		private readonly string DashboardLink = "BCMStrategy/Dashboard?refHashId={0}&date={1}&key={2}";

		private static IUserMaster _userMaster;

		private static IUserMaster UserMaster
		{
			get
			{
				if (_userMaster == null)
				{
					_userMaster = new UserMasterRepository();
				}

				return _userMaster;
			}
		}

		private static IDashboard _dashboard;

		private static IDashboard Dashboard
		{
			get
			{
				if (_dashboard == null)
				{
					_dashboard = new DashboardRepository();
				}

				return _dashboard;
			}
		}

		private static IEmailGenerate _emailGenerate;

		private static IEmailGenerate EmailGenerate
		{
			get
			{
				if (_emailGenerate == null)
				{
					_emailGenerate = new EmailGenerateRepository();
				}

				return _emailGenerate;
			}
		}

		private static ICommonRepository _commonRepository;

		private static ICommonRepository CommonRepository
		{
			get
			{
				if (_commonRepository == null)
				{
					_commonRepository = new CommonRepository();
				}

				return _commonRepository;
			}
		}

		#endregion General Variables

		#region Helper Methods

		public void EmailGenerationAndSave()
		{
			try
			{
				List<UserModel> allCustomerList = UserMaster.GetAllCustomerList(Enums.UserType.CUSTOMER);
				GenerateChart();
				foreach (var user in allCustomerList)
				{
					DateTime reportDate = Helper.GetCurrentDateTime();
					DateTime dateOfDataRetrieval = reportDate.AddDays(-Convert.ToInt32(DayOfDataRetrieval));
					ReportViewModel rvm = new ReportViewModel()
					{
						IsAggregateDisplay = true,
						SelectedDate = dateOfDataRetrieval.ToString("MM/dd/yyyy"),
						RefHashId = user.UserMasterHashId
					};
					List<ReportViewModel> lexiconTypeList = Dashboard.GetChartLexiconValues(rvm);
					SaveEmailGeneration(user, reportDate.ToString("yyyyMMdd"), lexiconTypeList.Count, dateOfDataRetrieval);
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "EmailGenerationAndSave: Exception is thrown in Main method", ex, null);
			}
		}

		private async Task SaveEmailGeneration(UserModel userModel, string date, int lexiconCount, DateTime dateOfDataRetrieval)
		{
			try
			{
				EmailTemplateModel emailTemplate = EmailGenerate.GetEmailTemplate(Helper.EMailTemplateName.REPORT_TEMPLATE.ToString());
				string emailBody = string.Empty;
				var mailFormat_Body = emailTemplate.EmailBody;
				string webApplicationURL = await CommonRepository.GetWebApplicationBasePath();
				var subject = string.Format(emailTemplate.EmailSubject, dateOfDataRetrieval.ToString("dd MMMM yyyy"));

				string validationKey = Helper.GuidString();
				string link = string.Format("{0}{1}", webApplicationURL, string.Format(DashboardLink, userModel.UserMasterHashId, date, validationKey));

				emailBody += mailFormat_Body.Replace("{%UserName%}", string.Format("{0}", userModel.FirstName))
																		.Replace("{%DataGeneratedDate%}", dateOfDataRetrieval.ToString("dd MMMM yyyy"))
																		.Replace("{%Logo%}", string.Format("{0}{1}", webApplicationURL, "/Content/img/email-logo.png"))
																		.Replace("{%Link%}", link);

				EmailGenerationModel emailServiceModel = new EmailGenerationModel()
				{
					UserId = userModel.UserId,
					CreatedAt = Helper.GetCurrentDateTime(),
					SendAfterTime = Helper.GetCurrentDateTime(),
					SendBeforeTime = Helper.GetCurrentDateTime().AddHours(Convert.ToInt32(EmailDurationInHours)),
					EmailBody = emailBody,
					EmailSubject = subject,
					EmailTemplateId = emailTemplate.EmailTemplateId,
					Status = lexiconCount > 0 ? Helper.EmailServiceStatus.PENDING.ToString() : Helper.EmailServiceStatus.NA.ToString(),
					ValidationKey = validationKey
				};

				EmailGenerate.SaveEmailGenerationStatus(emailServiceModel);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "SaveEmailGeneration: Exception is thrown in Main method", ex, null);
			}
		}

		private void GenerateChart()
		{
			List<UserModel> allAdminList = UserMaster.GetAllCustomerList(Enums.UserType.ADMIN);
			DateTime reportDate = Helper.GetCurrentDateTime();
			DateTime dateOfDataRetrieval = reportDate.AddDays(-Convert.ToInt32(DayOfDataRetrieval));
			string userHashId = allAdminList.Select(x => x.UserMasterHashId).FirstOrDefault();
			ReportViewModel rvm = new ReportViewModel()
			{
				IsAggregateDisplay = true,
				SelectedDate = dateOfDataRetrieval.ToString("MM/dd/yyyy"),
				RefHashId = userHashId
			};

			List<ReportViewModel> lexiconTypeList = Dashboard.GetChartLexiconValues(rvm);
			Chart chart = new Chart() { Width = 550 };

			chart.Legends.Add(new Legend() { Name = "Legend" });
			chart.Legends[0].Docking = Docking.Bottom;
			chart.Legends[0].Alignment = StringAlignment.Center;
			ChartArea chartArea = new ChartArea() { Name = "ChartArea" };
			//Remove X-axis grid lines
			chartArea.AxisX.MajorGrid.LineWidth = 0;
			//Remove Y-axis grid lines
			chartArea.AxisY.MajorGrid.LineWidth = 1;
			//Chart Area Back Color
			chartArea.BackColor = Color.FromName("White");
			chart.ChartAreas.Add(chartArea);
			chart.Palette = ChartColorPalette.BrightPastel;
			string series = string.Empty;
			//create series and add data points to the series

			Title title = new Title();
			title.Text = string.Format(Resources.Resource.LblChartMainTitle, dateOfDataRetrieval.ToString("MM/dd/yyyy"));
			title.Font = new Font("sans-serif", 12, FontStyle.Bold);
			chart.Titles.Add(title);

			string[] Proprietary = lexiconTypeList.SelectMany(x => x.ActionType.Select(y => y.Name)).Distinct().ToArray();
			//a series to the chart
			List<ChartColors> chartColorList = Helper.GetProprietaryColorList();

			foreach (string dc in Proprietary)
			{
				string[] strLexicons = lexiconTypeList.Select(x => x.LexiconType).ToArray();
				foreach (string dr in strLexicons)
				{
					double[] dataPoint = lexiconTypeList.Where(x => x.LexiconType == dr).SelectMany(x => x.ActionType.Where(y => y.Name == dc).Select(y => (double)y.Value)).ToArray();

					if (chart.Series.FindByName(dc) == null)
					{
						series = dc;
						chart.Series.Add(series);
						chart.Series[series].ChartType = SeriesChartType.StackedColumn;
						chart.Series[series].Color = ColorTranslator.FromHtml(chartColorList.Where(x => x.ProprietaryName == dc).Select(x => x.ProprietaryColor).FirstOrDefault());
					}

					DataPoint objDataPoint = new DataPoint()
					{
						AxisLabel = dr,
						YValues = dataPoint,
						LegendText = dr,
						Color = ColorTranslator.FromHtml(chartColorList.Where(x => x.ProprietaryName == dc).Select(x => x.ProprietaryColor).FirstOrDefault())
					};

					chart.Series[series].Points.Add(objDataPoint);
				}
			}

			if (lexiconTypeList.Any())
			{
				using (MemoryStream imageStream = new MemoryStream())
				{
					chart.SaveImage(imageStream, ImageFormat.Jpeg);
					EmailGenerationChartImage emailGenerationChartImage = new EmailGenerationChartImage()
					{
						EmailGenerationDate = Helper.GetCurrentDateTime(),
						EmailGenerationImage = imageStream.ToArray(),
						EmailGenerationUpdateDate = Helper.GetCurrentDateTime()
					};
					EmailGenerate.SaveEmailGenerationChartImage(emailGenerationChartImage);
					////System.IO.File.WriteAllBytes(@"D:\Generated PDF\hello_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".png", str.ToArray());
				}
			}
		}

		////void sendEmai1l(byte[] data)
		////{
		////	string htmlBody = "<h1>Picture</h1><br><a href='https://google.com'><img src=\"cid:DashboardChartImage\"></a>";
		////	AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
		////	// Create a LinkedResource object for each embedded image
		////	MemoryStream str = new MemoryStream(data);
		////	LinkedResource dashboardChart = new LinkedResource(str, MediaTypeNames.Image.Jpeg);
		////	dashboardChart.ContentId = "DashboardChartImage";
		////	avHtml.LinkedResources.Add(dashboardChart);

		////	// Add the alternate views instead of using MailMessage.Body
		////	MailMessage mail = new MailMessage();
		////	mail.AlternateViews.Add(avHtml);

		////	MailAddress mad = new MailAddress("sandeepp@imail.iz");
		////	mail.From = mad;
		////	mail.To.Add("sandeepp@imail.iz");
		////	mail.Subject = "Client: Has Sent You A Screenshot fasdfas";

		////	mail.Body = String.Format(htmlBody, dashboardChart.ContentId);

		////	mail.IsBodyHtml = true;

		////	SmtpClient SMTP = new SmtpClient("192.168.1.81", 587);
		////	SMTP.EnableSsl = false;

		////	SMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
		////	SMTP.UseDefaultCredentials = false;

		////	NetworkCredential credentials = new NetworkCredential();
		////	credentials.UserName = "sandeepp@imail.iz";
		////	credentials.Password = "1";
		////	SMTP.Credentials = credentials;
		////	SMTP.Send(mail);
		////}

		#endregion Helper Methods
	}
}