using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;

namespace Bank_Project.Services
{
    public class CardService
    {
        private readonly PasswordHasher<Cards> _cardHasher = new();

        public Cards CreateCard(Accounts account, CreateCardDto dto)
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

            return card;
        }
    }
}