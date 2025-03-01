﻿namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class UpdateSeatDto
    {
        public int SeatId { get; set; }
        public int EventId { get; set; }
        public string? Row { get; set; }
        public int Number { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
    }
}
