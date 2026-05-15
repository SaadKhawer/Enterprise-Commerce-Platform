using System.Net;
using System.Net.Mail;
using ShopWaveBlazor.Web.ECommerceAPI.Models;

namespace ShopWaveBlazor.Web.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOrderConfirmationEmailAsync(string toEmail, Order order)
    {
        try
        {
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUser = "shopwavep@gmail.com"; // Replace with your Gmail
            var smtpPass = "exww govo kzja dnxi";    // Replace with your App Password

            var fromAddress = new MailAddress(smtpUser, "ShopWave Store");
            var toAddress = new MailAddress(toEmail);

            string subject = $"Order Confirmation - #{order.OrderID}";
            
            string body = $@"
                <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h1 style='color: #5b8dee; margin: 0;'>ShopWave</h1>
                        <p style='color: #888; font-size: 14px;'>Premium E-Commerce Experience</p>
                    </div>
                    
                    <h2 style='border-bottom: 2px solid #5b8dee; padding-bottom: 10px;'>Order Confirmed!</h2>
                    <p>Hi,</p>
                    <p>Thank you for shopping with us. Your order <strong>#{order.OrderID}</strong> has been placed successfully.</p>
                    
                    <div style='background: #f9f9f9; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                        <h3 style='margin-top: 0;'>Order Summary</h3>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <thead>
                                <tr style='border-bottom: 1px solid #ddd;'>
                                    <th style='text-align: left; padding: 8px;'>Item</th>
                                    <th style='text-align: center; padding: 8px;'>Qty</th>
                                    <th style='text-align: right; padding: 8px;'>Price</th>
                                </tr>
                            </thead>
                            <tbody>";

            foreach (var item in order.OrderDetails)
            {
                body += $@"
                                <tr>
                                    <td style='padding: 8px;'>{item.Product?.ProductName}</td>
                                    <td style='padding: 8px; text-align: center;'>{item.Quantity}</td>
                                    <td style='padding: 8px; text-align: right;'>Rs. {item.UnitPrice:N0}</td>
                                </tr>";
            }

            body += $@"
                            </tbody>
                        </table>
                        
                        <div style='margin-top: 20px; border-top: 2px solid #eee; padding-top: 10px; text-align: right;'>
                            <p style='margin: 5px 0;'>Subtotal: Rs. {(order.TotalAmount - order.DeliveryCharges):N0}</p>
                            <p style='margin: 5px 0;'>Delivery: Rs. {order.DeliveryCharges:N0}</p>
                            <h3 style='margin: 5px 0; color: #5b8dee;'>Total: Rs. {order.TotalAmount:N0}</h3>
                        </div>
                    </div>

                    <div style='margin: 20px 0;'>
                        <strong>Shipping Address:</strong><br/>
                        {order.ShippingAddress}<br/>
                        <strong>Phone:</strong> {order.PhoneNumber}
                    </div>

                    <p style='font-size: 12px; color: #888; text-align: center; margin-top: 40px;'>
                        This is an automated email. Please do not reply.
                    </p>
                </div>";

            using var smtp = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Detailed Logging for debugging
            Console.WriteLine("CRITICAL: Email sending failed!");
            Console.WriteLine($"Error Message: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            
            // You can also see this in Visual Studio Output Window
            System.Diagnostics.Debug.WriteLine($"EMAIL ERROR: {ex.Message}");
        }
    }
}
