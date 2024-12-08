using StreamShift.Domain.DatabaseEnums;

namespace StreamShift.Infrastructure.Services.TransferService.Abstract
{
    public interface ITransferService
    {
        Task TransferDatabase(string sourceConnectionString, eDatabase sourceType, string destinationConnectionString, eDatabase destinationType);
    }
}