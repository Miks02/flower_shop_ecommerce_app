using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.LoyaltyTransactions;

public static class LoyaltyTransactionErrors
{
    public static Error InsufficientPoints() 
        => new Error("LoyaltyError_InsufficientPoints", "Minimalni broj potrebnih poena koje možete iskoristiti je 1000.");
}