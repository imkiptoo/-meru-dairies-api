using System;
using AfricasTalkingCS;

namespace API.Utilities
{
    public class Sms
    {
        const string username = "zyve";
        const string apikey = "467a2352bbd74f29e996c27e4f6ff6a6eb29bae002a902581f2eaaaf1738f000";

        public static void Send(string recipients, string msg)
        {
            var gateway = new AfricasTalkingGateway(username, apikey);
            recipients = "+"+Tools.SanitizePhoneNumber(recipients);
            try
            {
                var sms = gateway.SendMessage(recipients, msg);
                foreach (var res in sms["SMSMessageData"]["Recipients"])
                {
                    Console.WriteLine((string)res["number"] + ": ");
                    Console.WriteLine((string)res["status"] + ": ");
                    Console.WriteLine((string)res["messageId"] + ": ");
                    Console.WriteLine((string)res["cost"] + ": ");
                }

            }
            catch (AfricasTalkingGatewayException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}