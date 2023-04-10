using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
    /// <summary>
    /// Web Link View Model
    /// </summary>
    public class WebLinkViewModel
    {


        public WebLinkViewModel()
        {
            this.SearchKeywords = new List<string>();
            this.RSSFeedURLs = new List<string>();
            ////this.ActionT = new List<DropdownMaster>();
            this.ActivityTypeDDL = new List<DropdownMaster>();
            this.MetaDataProprietaryDDL = new List<DropdownMaster>();
        }

        public int WebLinkMasterId { get; set; }
        private string _webLinkMasterHashId { get; set; }

        public string WebLinkMasterHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_webLinkMasterHashId))
                {
                    _webLinkMasterHashId = this.WebLinkMasterId == 0 ? string.Empty : this.WebLinkMasterId.ToEncrypt();
                }
                return _webLinkMasterHashId;
            }
            set
            {
                _webLinkMasterHashId = value;
            }
        }

        /// <summary>
        /// DropDown For Page Type
        /// </summary>
        public List<DropdownMaster> PageTypeDDL { get; set; }

        /// <summary>
        /// DropDown For WebSiteType
        /// </summary>
        public List<DropdownMaster> WebSiteTypeDDL { get; set; }

        /// <summary>
        /// Is Active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Is Active String
        /// </summary>
        public string IsActiveString { get; set; }

        /// <summary>
        /// Is HardCoded String
        /// </summary>
        public string IsHardCodedString { get; set; }

        public bool IsEdit { get; set; }

        /// <summary>
        /// DropDown For ActivityType
        /// </summary>
        public List<DropdownMaster> ActivityTypeDDL { get; set; }

        /// <summary>
        /// Activity Type Hash Id
        /// </summary>
        public List<string> ActivityTypeHashIds { get; set; }

        /// <summary>
        /// DropDown For EntityFullName
        /// </summary>
        public List<DropdownMaster> EntityFullNameDDL { get; set; }

        private string _entityFullNameHashId { get; set; }
        /// <summary>
        /// Page Type Hash Id
        /// </summary>
        public string EntityFullNameHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_entityFullNameHashId))
                {
                    _entityFullNameHashId = this.EntityFullNameId == 0 ? string.Empty : this.EntityFullNameId.ToEncrypt();
                }
                return _entityFullNameHashId;
            }
            set
            {
                _entityFullNameHashId = value;
            }
        }

        /// <summary>
        /// DropDown For EntityName
        /// </summary>
        public List<DropdownMaster> EntityNameDDL { get; set; }

        /// <summary>
        /// Entity Name Hash Id
        /// </summary>
        public string EntityNameHashId { get; set; }

        /// <summary>
        /// DropDown For IndividualPerson
        /// </summary>
        public List<DropdownMaster> IndividualPersonDDL { get; set; }

        /// <summary>
        /// Individual Person Hash Id
        /// </summary>
        public string IndividualPersonHashIds { get; set; }

        /// <summary>
        /// DropDown For EntityType
        /// </summary>
        public List<DropdownMaster> EntityTypeDDL { get; set; }

        /// <summary>
        /// DropDown For Sector
        /// </summary>
        public List<DropdownMaster> SectorDDL { get; set; }

        /// <summary>
        /// DropDown For Proprietary tags
        /// </summary>
        public List<DropdownMaster> MetaDataProprietaryDDL { get; set; }

        /// <summary>
        /// Meta data Proprietary Hash id
        /// </summary>
        //[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        //[Display(Name = "LblWebLinkUrl", ResourceType = typeof(Resource))]
        //[ListHasElements(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        //[Display(Name = "LblProprietaryTag", ResourceType = typeof(Resource))]
        //[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        //[Display(Name = "LblWebLinkUrl", ResourceType = typeof(Resource))]
        [ListHasElementsAttribute(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        [Display(Name = "LblActionType", ResourceType = typeof(Resource))]
        public List<string> ProprietaryHashIds { get; set; }

        /// <summary>
        /// Sector Hash Id
        /// </summary>
        public List<string> SectorHashIds { get; set; }

        /// <summary>
        /// Web Link Url
        /// </summary>
        //[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        //[Url]

        //[Url]
        //[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        //[Display(Name = "LblWebLinkUrl", ResourceType = typeof(Resource))]
        public string WebLinkUrl { get; set; }

        /// <summary>
        /// Page Type
        /// </summary>
        public string PageType { get; set; }

        /// <summary>
        /// WebSite Type
        /// </summary>
        public string WebSiteType { get; set; }

        /// <summary>
        /// Entity Full Name
        /// </summary>
        public string EntityFullName { get; set; }

        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Entity Type
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Entity Sector
        /// </summary>
        public string Sector { get; set; }

        /// <summary>
        /// Countr yName
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Is Blocked
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Is Hared Coded
        /// </summary>
        public bool IsHardCoded { get; set; }

        /// <summary>
        /// Is DropDown Available
        /// </summary>
        public bool IsDropDownAvailable { get; set; }

        /// <summary>
        /// Is Search Functionality
        /// </summary>
        public string DropDownSearchRegEx { get; set; }

        /// <summary>
        /// Is Search Functionality
        /// </summary>
        public string DropDownDateWiseRegEx { get; set; }

        /// <summary>
        /// Is Search Functionality
        /// </summary>
        public bool IsSearchFunctionality { get; set; }

        /// <summary>
        /// Search KeyWord
        /// </summary>
        public string SearchKeyWord { get; set; }

        /// <summary>
        /// Is RSS FeedAvailable
        /// </summary>
        public bool IsRSSFeedAvailable { get; set; }

        /// <summary>
        /// RSS Feed Click RegEx
        /// </summary>
        public string RSSFeedClickRegEx { get; set; }

        /// <summary>
        /// RSS Feed URL
        /// </summary>
        public string RSSFeedURL { get; set; }

        /// <summary>
        /// Is HyperLink ClickedAvailable
        /// </summary>
        public bool IsHyperLinkClickedAvailable { get; set; }

        /// <summary>
        /// HyperLink Click RegEx
        /// </summary>
        public string HyperLinkClickRegEx { get; set; }

        /// <summary>
        /// HyperLink Document RegEx
        /// </summary>
        public string HyperLinkDocumentRegEx { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// WebSite Type Id
        /// </summary>
        public int WebSiteTypeId { get; set; }

        private string _webSiteTypeHashId { get; set; }


        /// <summary>
        /// Web Site Hash Id
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        [Display(Name = "LblWebSiteType", ResourceType = typeof(Resource))]
        public string WebSiteTypeHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_webSiteTypeHashId))
                {
                    _webSiteTypeHashId = this.WebSiteTypeId == 0 ? string.Empty : this.WebSiteTypeId.ToEncrypt();
                }
                return _webSiteTypeHashId;
            }
            set
            {
                _webSiteTypeHashId = value;
            }

        }

        public int EntityFullNameId { get; set; }

        /// <summary>
        /// entity Type Id
        /// </summary>
        public int EntityTypeId { get; set; }

        private string _entityTypeHashId { get; set; }

        /// <summary>
        /// Entity Type HashId
        /// </summary>
        public string EntityTypeHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_entityTypeHashId))
                {
                    _entityTypeHashId = this.EntityTypeId == 0 ? string.Empty : this.EntityTypeId.ToEncrypt();
                }
                return _entityTypeHashId;
            }
            set
            {
                _entityTypeHashId = value;
            }
        }

        /// <summary>
        /// Country id
        /// </summary>
        public int CountryId { get; set; }

        private string _countryHashId { get; set; }


        /// <summary>
        /// Entity Type HashId
        /// </summary>
        //[Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        [Display(Name = "LblCountry", ResourceType = typeof(Resource))]
        public string CountryHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_countryHashId))
                {
                    _countryHashId = this.CountryId == 0 ? string.Empty : this.CountryId.ToEncrypt();
                }
                return _countryHashId;
            }
            set
            {
                _countryHashId = value;
            }
        }

        public int pageTypeId { get; set; }

        private string _pageTypeHashId { get; set; }

        /// <summary>
        /// Page Type Hash Id
        /// </summary>
        ////[Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
        ////[Display(Name = "LblPageType", ResourceType = typeof(Resource))]
        public string PageTypeHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_pageTypeHashId))
                {
                    _pageTypeHashId = this.pageTypeId == 0 ? string.Empty : this.pageTypeId.ToEncrypt();
                }
                return _pageTypeHashId;
            }
            set
            {
                _pageTypeHashId = value;
            }
        }

        /// <summary>
        /// Individual Persons 
        /// </summary>
        public List<PolicyMakerModel> PolicyMakerModelList { get; set; }

        /// <summary>
        /// RSS Feed URLs
        /// </summary>
        public List<string> RSSFeedURLs { get; set; }

        /// <summary>
        /// Search Keywords
        /// </summary>
        public List<string> SearchKeywords { get; set; }
    }
}