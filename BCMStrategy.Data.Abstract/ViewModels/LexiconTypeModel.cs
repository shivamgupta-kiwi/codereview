using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
   public class LexiconTypeModel
    {
        private string _lexiconTypeMasterHashId;

        
        public int LexiconTypeMasterId { get; set; }

        public string LexiconTypeMasterHashId
        {
            get
            {
                if (string.IsNullOrEmpty(_lexiconTypeMasterHashId))
                {
                    _lexiconTypeMasterHashId = this.LexiconTypeMasterId == 0 ? string.Empty : this.LexiconTypeMasterId.ToEncrypt();
                }
                return _lexiconTypeMasterHashId;
            }
            set
            {
                _lexiconTypeMasterHashId = value;
            }
        }

        public string LexiconType { get; set; }
    }
}
