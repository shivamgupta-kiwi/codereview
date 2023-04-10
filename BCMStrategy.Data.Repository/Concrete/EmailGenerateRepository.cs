using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Repository.Concrete
{
	public class EmailGenerateRepository : IEmailGenerate
	{
		public EmailTemplateModel GetEmailTemplate(string emailTemplateName)
		{
			EmailTemplateModel emailTemplate;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				emailTemplate = db.emailtemplate.Where(x => x.TemplateName == emailTemplateName && x.IsTemplateActive).Select(x => new EmailTemplateModel()
				{
					EmailTemplateId = x.Id,
					EmailBody = x.BodyHtml,
					EmailSubject = x.Subject
				}).FirstOrDefault();
			}
			return emailTemplate;
		}

		public void SaveEmailGenerationStatus(EmailGenerationModel emailGeneration)
		{

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				emailgenerationstatus emailgeneration = new emailgenerationstatus()
				{
					UserId = emailGeneration.UserId,
					TemplateId = emailGeneration.EmailTemplateId,
					CreatedAt = emailGeneration.CreatedAt,
					SendAfterTime = emailGeneration.SendAfterTime,
					SendBeforeTime = emailGeneration.SendBeforeTime,
					EmailBody = emailGeneration.HtmlBody,
					EmailSubject = emailGeneration.EmailSubject,
					Status = emailGeneration.Status,
					ValidationKey = emailGeneration.ValidationKey
				};
				db.emailgenerationstatus.Add(emailgeneration);
				db.SaveChanges();
			}
		}

		public void SaveEmailGenerationChartImage(EmailGenerationChartImage emailGenerationChartImageModel)
		{

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				var getData = db.emailgenerationchartimage.Where(x => x.GenerationDate.Value.Year == emailGenerationChartImageModel.EmailGenerationDate.Value.Year &&
																															 x.GenerationDate.Value.Month == emailGenerationChartImageModel.EmailGenerationDate.Value.Month &&
																															 x.GenerationDate.Value.Day == emailGenerationChartImageModel.EmailGenerationDate.Value.Day).FirstOrDefault();
				if (getData != null)
				{
					getData.ChartImage = emailGenerationChartImageModel.EmailGenerationImage;
					getData.UpdationDate = emailGenerationChartImageModel.EmailGenerationUpdateDate;
				}
				else
				{
					emailgenerationchartimage emailGenerationChartImage = new emailgenerationchartimage()
					{
						GenerationDate = emailGenerationChartImageModel.EmailGenerationDate,
						ChartImage = emailGenerationChartImageModel.EmailGenerationImage,
						UpdationDate = emailGenerationChartImageModel.EmailGenerationUpdateDate
					};
					db.emailgenerationchartimage.Add(emailGenerationChartImage);
				}
				db.SaveChanges();
			}
		}
	}
}
