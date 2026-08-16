using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.EmailTemplates
{
    public class OrderConfirmationTemplate
    {
        public static string Render(
            int orderId,
            decimal totalPrice,
            string address,
            string phone)
        {
            return $"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8">
                    <title>Order Confirmation</title>
                </head>

                <body style="margin:0; padding:0; background-color:#f4f4f4; font-family:Arial,sans-serif;">

                    <div style="max-width:600px; margin:40px auto; background:#ffffff; padding:30px;">

                        <h2 style="margin-top:0;">
                            Thank you for your order!
                        </h2>

                        <p>
                            Your order has been placed successfully.
                        </p>

                        <hr>

                        <p>
                            <strong>Order ID:</strong> #{orderId}
                        </p>

                        <p>
                            <strong>Total:</strong> {totalPrice:C}
                        </p>

                        <p>
                            <strong>Address:</strong> {address}
                        </p>

                        <p>
                            <strong>Phone:</strong> {phone}
                        </p>

                        <hr>

                        <p>
                            Thank you for shopping with MyShop.
                        </p>

                    </div>

                </body>
                </html>
                """;
        }

}
}
