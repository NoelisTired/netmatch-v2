using DAL.DTO;

namespace Interface.DataInterfaces;

public interface IQuoteRepository
{
    QuoteDTO? GetById(int id);
    QuoteDTO Add(QuoteDTO DTO);
    void UpdateById(int id, string title, string taal, string status);
    void RemoveById(int id);
}