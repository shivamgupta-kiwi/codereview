using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BCMStrategy.Data.Abstract.Abstract
{
    public interface ILexiconType
    {
        Task<ApiOutput> GetDDLLexiconTypeList();

        Task<ApiOutput> GetAllLexiconTypeList(GridParameters parametersJson);

    }
}
