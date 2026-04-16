using System;

namespace DAL.DTO   
{
    public enum Status { Active, Inactive, Pending }
    public enum Language { English }
    
    public class QuoteDTO
    {
        public int Id { get; private set; }
        public string Title { get;private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;

        public QuoteDTO(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
            