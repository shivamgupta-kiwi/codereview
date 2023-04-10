using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
    public class InstitutionTypesModel
    {
        private string _institutionTypesHashId;
        [JsonIgnore]
        public int InstitutionTypesId { get; set; }

        public string InstitutionTypesHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_institutionTypesHashId))
                {
                    _institutionTypesHashId = this.InstitutionTypesId == 0 ? string.Empty : this.InstitutionTypesId.ToEncrypt();
                }
                return _institutionTypesHashId;
            }
            set
            {
                _institutionTypesHashId = value;
            }
        }

        [IsInstitutionTypesExistAttribute(ErrorMessageResourceName = "ValidateInstitutionTypesExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        [RegularExpression(@"^[a-zA-Z0-9-\s]{2,250}$", ErrorMessageResourceName = "ValidationLength_2_250_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        [Display(Name = "LblInstitutionTypesName", ResourceType = typeof(Resource))]
        public string InstitutionTypesName { get; set; }
    }
}
