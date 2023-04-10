using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Resources;

namespace BCMStrategy.Data.Abstract.ViewModels
{
	public class GlobalSettingModel
	{
		[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
		[Display(Name = "LblSMTPDetails", ResourceType = typeof(Resource))]
		public string SMTPDetails { get; set; }
	}
}
