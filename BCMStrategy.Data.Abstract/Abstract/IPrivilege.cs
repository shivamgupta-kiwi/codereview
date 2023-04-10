using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
    public interface IPrivilege
    {
        /// <summary>
        ///  Used to get All Customer
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        Task<ApiOutput> GetAllCustomer(GridParameters parameters);

        /// <summary>
        /// Get All Lexicon List
        /// </summary>
        /// <param name="lexiconTypeHashId"></param>
        /// <returns></returns>
        Task<List<LexiconModel>> GetLexiconTermHashIdsBasedOnLexiconType(string lexiconTypeHashId,GridParameters gridParameter);

        /// <summary>
        /// Update Lexicon Access Privilege
        /// </summary>
        /// <param name="lexiconAccessManagementModel"></param>
        /// <returns></returns>
        Task<bool> UpdateLexiconAccessPrivilege(LexiconAccessManagementModel lexiconAccessManagementModel);

        /// <summary>
        /// Get All Lexicon Access Customer
        /// </summary>
        /// <param name="parametersJson"></param>
        /// <returns></returns>
        Task<ApiOutput> GetAllLexiconAccessCustomer(GridParameters parametersJson);

        /// <summary>
        /// Get LexiconIds Based On HashId
        /// </summary>
        /// <param name="webLinkHashId"></param>
        /// <returns></returns>
        Task<List<LexiconAccessManagementModel>> GetLexiconIdsBasedOnCustomer(string customerHashId);

    }
}
