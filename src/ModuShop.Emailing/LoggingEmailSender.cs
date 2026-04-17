using System.Threading;
using System.Threading.Tasks;
using ModuShop.Emailing.Contracts;
using Serilog;

namespace ModuShop.Emailing;

public class LoggingEmailSender(ILogger logger) : IEmailSender
{
    public Task SendOrderConfirmationAsync(
        string toEmail,
        int orderId,
        decimal totalAmount,
        CancellationToken cancellationToken = default)
    {
        logger.Information(
            "Fake email sent to {Email}. Order #{OrderId}, total: {TotalAmount}",
            toEmail,
            orderId,
            totalAmount);

        return Task.CompletedTask;
    }
}