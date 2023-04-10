using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.PDFGenerator.Abstract;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace BCMStrategy.PDFGenerator.Repository
{
	public class PDFOperationRepository : IPdfOperationRepository
	{
		#region GENERAL PROPERTIES

		private static readonly EventLogger<PDFOperationRepository> log = new EventLogger<PDFOperationRepository>();
		public static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;

		private static IAmazonS3 s3Client;

		private static IGlobalConfiguration _globalConfiguration;

		private static IGlobalConfiguration GlobalConfiguration
		{
			get
			{
				if (_globalConfiguration == null)
				{
					_globalConfiguration = new GlobalConfigurationRepository();
				}

				return _globalConfiguration;
			}
		}

		private IWebLink _webLink;

		private IWebLink WebLink
		{
			get
			{
				if (_webLink == null)
				{
					_webLink = new WebLinkRepository();
				}

				return _webLink;
			}
		}

    ////private static ILexicon _lexicon;

    ////private static ILexicon Lexicon
    ////{
    ////  get
    ////  {
    ////    if (_lexicon == null)
    ////    {
    ////      _lexicon = new LexiconRepository();
    ////    }

    ////    return _lexicon;
    ////  }
    ////}

		private static IDocumentStorage _documentStorage;

		private static IDocumentStorage DocumentStorage
		{
			get
			{
				if (_documentStorage == null)
				{
					_documentStorage = new DocumentStorageRepository();
				}
				return _documentStorage;
			}
		}

		private IProcessEvents _processEvents;

		private IProcessEvents ProcessEvents
		{
			get
			{
				if (_processEvents == null)
				{
					_processEvents = new ProcessEventsRepository();
				}

				return _processEvents;
			}
		}

		#endregion GENERAL PROPERTIES

		#region PUBLIC METHODS

		static PDFOperationRepository()
		{
			s3Client = new AmazonS3Client(bucketRegion);
		}

		public void GeneratePDF(int processId, int processInstanceId)
		{
			GetLexiconsFromSolr(processId, processInstanceId);
		}

		#endregion PUBLIC METHODS

		#region PRIVATE METHODS

		/// <summary>
		/// Read Lexicons From Solr
		/// </summary>
		/// <param name="processId">ProcessId</param>
		/// <param name="processInstanceId">Process Instance Id</param>
		private void GetLexiconsFromSolr(int processId, int processInstanceId)
		{
			try
			{
				int result = SaveScraperEvent(processId, processInstanceId);
				if (result > 0)
				{
					log.LogEntry("ReadLexiconFromSolr :", processId, processInstanceId);

					List<LoaderLinkQueue> listOfWebURLtoTagLexicons = WebLink.GetWebLinkForLexicons(processId, processInstanceId);

					foreach (LoaderLinkQueue loaderLinkQueue in listOfWebURLtoTagLexicons)
					{
						ProcessPageSource(loaderLinkQueue);
					}

					log.LogEntry("End Of ReadLexiconFromSolr ");
					UpdateScraperEvent(result, processId, listOfWebURLtoTagLexicons.Count);

          // Commenting the code of Future enhancements

          ////if (!ProcessEvents.CheckFullProcessCompleted(processId, processInstanceId) &&
          ////       ProcessEvents.GetMessageCount(Helper.SQSTypes.EmailGeneration.ToString(), (int)Helper.SQSTypes.EmailGeneration) == 0)
          ////{
          ////  ProcessEvents.SaveToSQS(Helper.SQSTypes.EmailGeneration.ToString(), (int)Helper.SQSTypes.EmailGeneration);
          ////}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ReadLexiconFromSolr: Exception is thrown in Main method", ex, null);
			}
		}

		/// <summary>
		/// Lexicon Search From Solr
		/// </summary>
		/// <param name="loaderLinkQueue"></param>
		private void ProcessPageSource(LoaderLinkQueue loaderLinkQueue)
		{
			try
			{
				log.LogEntry("Start of ProcessPageSource");

				SolrSearchParameters searchParameters = new SolrSearchParameters();

				searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"AND scrapperDetailId:\"" + loaderLinkQueue.Id + "\"";
				LexiconDetailsView lexiconDetailsView = GlobalConfiguration.GetLexicon(searchParameters);
				if (lexiconDetailsView.Products.Count > 0)
				{
					var pageSource = lexiconDetailsView.Products.Select(x => x).FirstOrDefault().PageSource[0];
					StringBuilder tidyHtml = new StringBuilder();
					using (TidyManaged.Document Htmldoc = TidyManaged.Document.FromString(pageSource))
					{
            ////doc.OutputBodyOnly = AutoBool.;
						Htmldoc.CoerceEndTags = true;
						Htmldoc.EncloseBlockText = true;
						Htmldoc.Quiet = true;
						Htmldoc.ForceOutput = true;
						Htmldoc.OutputXhtml = true;
						////Htmldoc.DropEmptyElements = true;
						Htmldoc.CleanAndRepair();
						tidyHtml.Append(Htmldoc.Save());
					}
					HtmlDocument htmlDoc = new HtmlDocument();
					htmlDoc.OptionFixNestedTags = true;
					htmlDoc.OptionAutoCloseOnEnd = true;
					htmlDoc.OptionWriteEmptyNodes = true;
					htmlDoc.OptionCheckSyntax = true;
					htmlDoc.LoadHtml(tidyHtml.ToString());

					UploadFileAsync(htmlDoc.DocumentNode.OuterHtml, loaderLinkQueue.GUID, loaderLinkQueue.Id, loaderLinkQueue.ProcessId, loaderLinkQueue.SiteURL, loaderLinkQueue.ParentURL).Wait();
				}
				log.LogEntry("End of ProcessPageSource");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ProcessLexiconSearch: Exception is thrown in Main method", ex, null);
			}
		}

		/// <summary>
		/// Get the list of Lexicons
		/// </summary>
		/// <returns></returns>
    ////private List<LexiconModel> LexiconList()
    ////{
    ////  return Lexicon.GetLexiconListForScraping();
    ////}

		private static async Task UploadFileAsync(string html, string GUID, int scanningDetailId, int processId, string siteURL, string parentURL)
		{
			try
			{
				// LicenseKey.LoadLicenseFile(Helper.GetItextSharpLicenseKeyPath());

				Uri siteUri = new Uri(siteURL);

				string hostName = siteUri.Host;

				using (StringWriter sw = new StringWriter())
				{
					using (HtmlTextWriter hw = new HtmlTextWriter(sw))
					{
						StringBuilder sb = new StringBuilder();
						sb.Append(html);
            ////string htmlHeader = "<!DOCTYPE html><html><head><style>#div1{text-align:center;background:#353b50;line-height:2px;font-size:2px;color:#fff;}</style></head><body style='padding:0;margin:0;'><div style='width:100%'><div style='width:94%;float:left;'><div style='line-height:8px;font-size:8px;'>&nbsp;</div><div id='div1'>&nbsp;</div></div><div style='font-size:12px;float:left;margin-left:5px;background:#353b50;line-height:18px;color:#fff;'>&nbsp;_value&nbsp;</div></div></body></html>";
						Document pdfDoc = new Document(PageSize.A4, 50f, 15f, 50f, 20f);
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream))
							{
								StringReader sr = new StringReader(sb.ToString());
								pdfDoc.Open();
								writer.PageEvent = new HtmlPageEventHelper() { HostName = hostName, ParentURL = parentURL };
								writer.CloseStream = false;

								XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);

								pdfDoc.Close();

								byte[] bytes = memoryStream.ToArray();
								memoryStream.Close();
								string bucketName = Helper.BucketName + "/" + processId;

                ////System.IO.File.WriteAllBytes(@"D:\Generated PDF\hello_" + GUID + "_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf", bytes);
								var fileTransferUtilityRequest = new PutObjectRequest
								{
									BucketName = bucketName,
									InputStream = new MemoryStream(bytes),
									ContentType = "application/pdf",
									StorageClass = S3StorageClass.Standard,
									Key = GUID,
									CannedACL = S3CannedACL.PublicRead
								};
								PutObjectResponse response = await s3Client.PutObjectAsync(fileTransferUtilityRequest);

								if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
								{
									PdfGeneratorModel pdfGeneratormodel = new PdfGeneratorModel();
									pdfGeneratormodel.ScanningLinkDetailId = scanningDetailId;
									pdfGeneratormodel.URL = Helper.StoringURL + processId + "/" + GUID;
									pdfGeneratormodel.FileName = GUID;
									DocumentStorage.SaveFileToStorage(pdfGeneratormodel).Wait();
								}
							}
						}
					}
				}
			}
			catch (AmazonS3Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "UploadFileAsync: AmazonS3Exception is thrown while uploading file to AWS :- \n ", ex, null);
			}
			catch (AmazonClientException ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "UploadFileAsync: AmazonClientException is thrown while uploading file to AWS :- \n ", ex, null);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "UploadFileAsync: Exception is thrown while uploading file to AWS for GUID " + GUID + " and Scrapper-detail Id " + scanningDetailId + ":- \n ", ex, null);
			}
		}

		private int SaveScraperEvent(int processId, int processInstanceId)
		{
			Events scraperEvents = new Events()
			{
				ProcessEventId = processId,
				StartDateTime = Helper.GetSystemCurrentDateTime(),
				ProcessTypeId = Convert.ToInt32(Helper.ProcessType.PDFGeneration),
				ProcessInstanceId = processInstanceId,
			};
			return ProcessEvents.InsertEvents(scraperEvents);
		}

		private bool UpdateScraperEvent(int eventSavedresult, int processId, int solrCount)
		{
			Events scraperEvents = new Events()
			{
				Id = eventSavedresult,
				ProcessEventId = processId,
				EndDateTime = Helper.GetSystemCurrentDateTime(),
				PagesProcessed = solrCount
			};
			return ProcessEvents.UpdateEvents(scraperEvents);
		}

		////private static void ReadPDFFile(string fileName)
		////{
		////  string file = @"E:\Projects\BCM\BCMStrategy_3_0_0\BCMStrategy.PDFGenerator\bin\Debug\Document\1-33-10523.pdf";
		////  if (!File.Exists(file))
		////    throw new FileNotFoundException("fileName");
		////  using (PdfReader reader = new PdfReader(file))
		////  {
		////    StringBuilder sb = new StringBuilder();
		////    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
		////    for (int page = 0; page < reader.NumberOfPages; page++)
		////    {
		////      string text = PdfTextExtractor.GetTextFromPage(reader, page + 1, strategy);
		////      if (!string.IsNullOrWhiteSpace(text))
		////      {
		////        sb.Append(Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text))));
		////      }
		////    }
		////    File.WriteAllText(@"E:\Projects\BCM\BCMStrategy_3_0_0\BCMStrategy.PDFGenerator\bin\Debug\Document\" + DateTime.Now.Ticks + ".txt", sb.ToString());
		////  }
		////}

		#endregion PRIVATE METHODS
	}

	public class HtmlPageEventHelper : PdfPageEventHelper
	{
		////	public static String HEADER =
		////	"<div style='padding:0;margin:0;'><div style='width:100%'><div style='width:94%;float:left;'><div style='line-height:8px;font-size:8px;'>&nbsp;</div><div id='div1'>&nbsp;</div></div><div style='font-size:12px;float:left;margin-left:5px;background:#353b50;line-height:18px;color:#fff;'>&nbsp;_value&nbsp;</div></div></div>";
		////	public override void OnEndPage(PdfWriter writer, Document document)
		////	{
		////		base.OnStartPage(writer, document);
		////		int pageN = writer.CurrentPageNumber;
		////		ColumnText ct = new ColumnText(writer.DirectContent);
		////		XMLWorkerHelper.GetInstance().ParseXHtml(new ColumnTextElementHandler(ct), new StringReader(HEADER.Replace("_value", pageN.ToString())));
		////		ct.SetSimpleColumn(document.Left, document.Top, document.Right, document.GetTop(-20), 10, Element.ALIGN_MIDDLE);
		////		ct.Go();
		////		//var header = XMLWorkerHelper.ParseToElementList(HEADER.Replace("_value", pageN.ToString()), "<style>#div1{text-align:center;background:#353b50;line-height:2px;font-size:2px;color:#fff;}</style>");
		////		//try
		////		//{
		////		//	ColumnText ct = new ColumnText(writer.DirectContent);
		////		//	foreach (IElement e in header)
		////		//	{
		////		//		ct.AddElement(e);
		////		//	}
		////		//	ct.SetSimpleColumn(document.Left, document.Top, document.Right, document.GetTop(-20), 10, Element.ALIGN_MIDDLE);
		////		//	ct.Go();
		////		//}
		////		//catch (DocumentException de)
		////		//{
		////		//	//throw new ExceptionConverter(de);
		////		//}
		////	}

		//// This is the contentbyte object of the writer
		
		#region Fields

		private string _header;

		#endregion Fields

		#region Properties

		public string Header
		{
			get { return _header; }
			set { _header = value; }
		}

		#endregion Properties

		public string HostName { get; set; }

		public string ParentURL { get; set; }

		public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
		{
      PdfContentByte cb;
      PdfTemplate headerTemplate, footerTemplate;

      BaseFont bf = null;
			base.OnEndPage(writer, document);

			bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
			cb = writer.DirectContent;
			headerTemplate = cb.CreateTemplate(50, 50);
			footerTemplate = cb.CreateTemplate(50, 50);

			////Create PdfTable object
			PdfPTable pdfTab = new PdfPTable(3);

			////Row 1
			PdfPCell pdfCell1 = new PdfPCell();
			////PdfPCell pdfCell2 = new PdfPCell(p1Header);
			PdfPCell pdfCell3 = new PdfPCell();
			String text = "Page " + writer.PageNumber;

			////Add paging to header
			{
				cb.BeginText();
				cb.SetFontAndSize(bf, 12);
				cb.SetTextMatrix(document.PageSize.GetRight(80), document.PageSize.GetTop(45));
				cb.ShowText(text);
				cb.EndText();

				cb.BeginText();
				cb.SetFontAndSize(bf, 12);
				cb.SetTextMatrix(document.PageSize.GetLeft(40), document.PageSize.GetTop(45));

				if (ParentURL.ToLower().IndexOf("thomsonreuters.com") != -1)
				{
					cb.ShowText("Publisher: Reuters");
				}
				else
				{
					cb.ShowText("Publisher: " + HostName);
				}

				cb.EndText();

				float len = bf.GetWidthPoint(text, 12);
				//Adds "12" in Page 1 of 12
				cb.AddTemplate(headerTemplate, document.PageSize.GetRight(80) + len, document.PageSize.GetTop(45));
			}

			////Row 3
			////PdfPCell pdfCell5 = new PdfPCell(new Phrase("Date:" + PrintTime.ToShortDateString(), baseFontBig));
			PdfPCell pdfCell6 = new PdfPCell();

			////set the alignment of all three cells and set border to 0
			pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
			////pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
			pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;
			////pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;
			////pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
			pdfCell6.HorizontalAlignment = Element.ALIGN_CENTER;

			////pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
			pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
			////pdfCell4.VerticalAlignment = Element.ALIGN_TOP;
			////pdfCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
			pdfCell6.VerticalAlignment = Element.ALIGN_MIDDLE;

			////pdfCell4.Colspan = 3;

			pdfCell1.Border = 0;
			////pdfCell2.Border = 0;
			pdfCell3.Border = 0;
			////pdfCell4.Border = 0;
			////pdfCell5.Border = 0;
			pdfCell6.Border = 0;

			////add all three cells into PdfTable
			pdfTab.AddCell(pdfCell1);
			////pdfTab.AddCell(pdfCell2);
			pdfTab.AddCell(pdfCell3);
			////pdfTab.AddCell(pdfCell4);
			////pdfTab.AddCell(pdfCell5);
			pdfTab.AddCell(pdfCell6);

			pdfTab.TotalWidth = document.PageSize.Width - 80f;
			pdfTab.WidthPercentage = 70;
			////pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

			////call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
			////first param is start row. -1 indicates there is no end row and all the rows to be included to write
			////Third and fourth param is x and y position to start writing
			pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
			////set pdfContent value

			////Move the pointer and draw line to separate header section from rest of page
			cb.MoveTo(40, document.PageSize.Height - 50);
			cb.LineTo(document.PageSize.Width - 20, document.PageSize.Height - 50);
			cb.Stroke();

			////Move the pointer and draw line to separate footer section from rest of page
			cb.MoveTo(40, document.PageSize.GetBottom(20));
			cb.LineTo(document.PageSize.Width - 20, document.PageSize.GetBottom(20));
			cb.Stroke();
		}

		////public override void OnCloseDocument(PdfWriter writer, Document document)
		////{
		////	base.OnCloseDocument(writer, document);

		////	headerTemplate.BeginText();
		////	headerTemplate.SetFontAndSize(bf, 12);
		////	headerTemplate.SetTextMatrix(0, 0);
		////	headerTemplate.ShowText((writer.PageNumber - 1).ToString());
		////	headerTemplate.EndText();

		////	footerTemplate.BeginText();
		////	footerTemplate.SetFontAndSize(bf, 12);
		////	footerTemplate.SetTextMatrix(0, 0);
		////	footerTemplate.ShowText((writer.PageNumber - 1).ToString());
		////	footerTemplate.EndText();
		////}
	}

	public class ColumnTextElementHandler : IElementHandler
	{
		public ColumnTextElementHandler(ColumnText ct)
		{
			this.ct = ct;
		}

		private ColumnText ct = null;

		public void Add(IWritable w)
		{
			if (w is WritableElement)
			{
				foreach (IElement e in ((WritableElement)w).Elements())
				{
					ct.AddElement(e);
				}
			}
		}
	}
}