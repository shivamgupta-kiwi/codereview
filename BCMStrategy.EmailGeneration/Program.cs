using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.EmailGeneration.Abstract;
using BCMStrategy.EmailGeneration.Repository;
using BCMStrategy.Logger;

namespace BCMStrategy.EmailGeneration
{
	public class Program
	{
		protected Program()
		{
		}
		private static readonly EventLogger<Program> log = new EventLogger<Program>();

		#region General Variables
		private static IEmailGeneration _emailGeneration;

		private static IEmailGeneration EmailGeneration
		{
			get
			{
				if (_emailGeneration == null)
				{
					_emailGeneration = new EmailGenerationRepository();
				}

				return _emailGeneration;
			}
		}
		#endregion

		static void Main(string[] args)
		{
			try
			{
				EmailGeneration.EmailGenerationAndSave();
			}
			catch (Exception ex) { log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Main method", ex, null); }
		}
	}
}
