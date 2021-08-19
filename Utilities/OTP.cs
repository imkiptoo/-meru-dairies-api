using System;
using System.Runtime.Serialization;
using AfricasTalkingCS;
using API.Utilities;
using Newtonsoft.Json.Linq;

namespace API.Utilities
{
    public class Otp
    {
        public static JObject GenerateMessage(int length, long ttl, string name)
        {
            var rVal = new JObject
            {
                ["sms"] = "",
                ["otp"] = "",
            };
            try
            {
                var oneTImePin = Tools.RandomDigits(length);

                rVal["otp"] = oneTImePin;
                
                const string simpleDateFormat = "dd-MMM-yyyy HH:mm:ss";
                
                var dtCurrentDateTime = DateTime.Now;
                var dtCurrentDateTimePlusTime = dtCurrentDateTime.Add(new TimeSpan(0,0, (int) (ttl * 60)));;
                
                var strTimeGenerated = dtCurrentDateTime.ToString(simpleDateFormat);
                var strExpiryDate = dtCurrentDateTimePlusTime.ToString(simpleDateFormat);


                var strMsg = "Dear "+(name == "" ? "user" : name)+",\n" + oneTImePin + " is your One Time Password(OTP) generated at " + strTimeGenerated + ". This OTP is valid up to " + strExpiryDate;
                rVal["sms"] = strMsg; 
            }
            catch (AfricasTalkingGatewayException exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            return rVal;
        }
    }
}