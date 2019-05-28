using System.Threading.Tasks;
using Backend.Helpers;
using Backend.Models;

namespace Backend.Data
{
    public interface IWpmsRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();

         Task<User> GetUser(int id, bool isCurrentUser);

        Task<bool> TrelloBoardExist(string name);

        Task<string> CreateTrelloBoard(string name, int orgId); 

        TrelloBoard ConvertJsonToTrelloBoard(string json);
        Task<string> AddUserToTrelloBoard(string boardId, string userMail, int orgId);
        Task<string> DeleteUserFromTrelloBoard(int orgId,string boardId, string trelloMemberId);
    }
}