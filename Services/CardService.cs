using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;
using Bank_Project.Data;

namespace Bank_Project.Services
{
    public class CardService
    {
        private readonly PasswordHasher<Cards> _cardHasher = new();
        private readonly AppDbContext _context;

        public CardService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Cards> CreateCardAsync(Accounts account, CreateCardDto dto)
        {
            var card = new Cards
            {
                CardNumber = Cards.GenerateCardNumber(),
                CardType = dto.CardType,
                ExpiryDate = DateTime.Now.AddYears(5),
                CVV = Cards.GenerateCVV(),
                Account = account
            };

            card.PasswordHash = _cardHasher.HashPassword(card, dto.CardPassword);

            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();

            return card;
        }
    }
}