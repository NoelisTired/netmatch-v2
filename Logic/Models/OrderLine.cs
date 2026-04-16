using System;

namespace Logic.Models;

public class OrderLine
{
    public int Id { get; private set; }
    public int QuoteId { get; private set; }
    public string Description { get; private set; }
    public decimal Amount { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }

    public OrderLine(int id, int quoteId, string description, decimal amount, int quantity)
    {
        this.Id = id;
        this.QuoteId = quoteId;
        this.Description = description;
        this.Amount = amount;
        this.Quantity = quantity;
    }
}