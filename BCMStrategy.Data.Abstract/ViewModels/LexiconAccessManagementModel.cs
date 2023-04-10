using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
    /// <summary>
    /// CustomerModel
    /// </summary>
    public class LexiconAccessManagementModel
    {
        public int LexiconPrivilegeId { get; set; }

        private string _lexiconPrivilegeHashId { get; set; }

        /// <summary>
        /// Customer Master HashId
        /// </summary>
        public string LexiconPrivilegeHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_lexiconPrivilegeHashId))
                {
                    _lexiconPrivilegeHashId = this.LexiconPrivilegeId == 0 ? string.Empty : this.LexiconPrivilegeId.ToEncrypt();
                }
                return _lexiconPrivilegeHashId;
            }
            set
            {
                _lexiconPrivilegeHashId = value;
            }
        }

        public int CustomerMasterId { get; set; }
        private string _customerMasterHashId { get; set; }

        /// <summary>
        /// Customer Master HashId
        /// </summary>
        public string CustomerMasterHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_customerMasterHashId))
                {
                    _customerMasterHashId = this.CustomerMasterId == 0 ? string.Empty : this.CustomerMasterId.ToEncrypt();
                }
                return _customerMasterHashId;
            }
            set
            {
                _customerMasterHashId = value;
            }
        }

        /// <summary>
        ///  Comapny Name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Designation
        /// </summary>
        public string Designation { get; set; }

        public int LexiconTypeId { get; set; }
        private string _lexiconTypeHashId { get; set; }

        /// <summary>
        /// Customer Master HashId
        /// </summary>
        public string LexiconTypeHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_lexiconTypeHashId))
                {
                    _lexiconTypeHashId = this.LexiconTypeId == 0 ? string.Empty : this.LexiconTypeId.ToEncrypt();
                }
                return _lexiconTypeHashId;
            }
            set
            {
                _lexiconTypeHashId = value;
            }
        }

        /// <summary>
        /// Customer First Name
        /// </summary>
        public string CustomerFirstName { get; set; }

        /// <summary>
        /// Customer Middle Name
        /// </summary>
        public string CustomerMiddleName { get; set; }

        /// <summary>
        /// Customer Last Name
        /// </summary>
        public string CustomerLastName { get; set; }

        public DateTime? Created { get; set; }

        /// <summary>
        /// Customer List
        /// </summary>
        public List<string> SelectedCustomerHashIds { get; set; }

        /// <summary>
        /// SelectedLexiconHashIds
        /// </summary>
        public List<string> SelectedLexiconHashIds { get; set; }

        public int LexiconeIssueId { get; set; }
        private string _lexiconHashId { get; set; }

        /// <summary>
        /// Lexicon Hash Id
        /// </summary>
        public string LexiconeIssueMasterHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_lexiconHashId))
                {
                    _lexiconHashId = this.LexiconeIssueId == 0 ? string.Empty : this.LexiconeIssueId.ToEncrypt();
                }
                return _lexiconHashId;
            }
            set
            {
                _lexiconHashId = value;
            }
        }

        public DateTime? Modified { get; set; }

         /// <summary>
         /// AccessLexiconTypes
        /// </summary>
        public List<string> AccessLexiconTypes { get; set; }

        public string AccessLexiconTypesString { get; set; }
    }
}
