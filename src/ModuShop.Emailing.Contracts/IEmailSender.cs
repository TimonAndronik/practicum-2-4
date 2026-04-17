using System.Threading;
using System.Threading.Tasks;

namespace ModuShop.Emailing.Contracts;

public interface IEmailSender
{
    Task SendOrderConfirmationAsync(
        string toEmail,
        int orderId,
        decimal totalAmount,
        CancellationToken cancellationToken = default);
}