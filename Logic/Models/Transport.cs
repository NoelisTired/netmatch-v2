using System;

namespace Logic.Models;

public class Transport
{
    public int Id { get; private set; }
    public int DagId { get; private set; }
    public string TransportType { get; private set; }
    public string DeportLocation { get; private set; }
    public string ArrivalLocation { get; private set; }
    public decimal Price { get; private set; }
    public int TransportNumber { get; private set; }
    public string TransportCompany { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }

    public Transport(int id, int dagId, string transportType, string deportLocation, string arrivalLocation, decimal price, int transportNumber, string transportCompany)
    {
        this.Id = id;
        this.DagId = dagId;
        this.TransportType = transportType;
        this.DeportLocation = deportLocation;
        this.ArrivalLocation = arrivalLocation;
        this.Price = price;
        this.TransportNumber = transportNumber;
        this.TransportCompany = transportCompany;
    }
}