using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using API.Helpers;
using API.Utilities;
using Newtonsoft.Json.Linq;
using SkyCrypto;

namespace API.Controllers
{
    [ApiController]
    [Route("api/synchronize")]
    [Authorize]
    public class SynchronizeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(SynchronizeRequestPayload synchronizeRequestPayload)
        {
            bool blRVal = false;
            try
            {
                Console.Write(synchronizeRequestPayload.Payload);

                var strRequestPayload = synchronizeRequestPayload.Payload;

                strRequestPayload = Crypto.decrypt(Tools.ENCRYPTION_KEY, strRequestPayload);

                var joDataJson = JObject.Parse(strRequestPayload ?? string.Empty);

                var strData = joDataJson["data"]?.ToString();
                var strLongTableName = joDataJson["long_table_name"]?.ToString();
                var strShortTableName = joDataJson["short_table_name"]?.ToString();

                strData = Crypto.decrypt(Tools.ENCRYPTION_KEY.Reverse().ToString(), strData);
                strLongTableName = Crypto.decrypt(Tools.ENCRYPTION_KEY.Reverse().ToString(), strLongTableName);
                strShortTableName = Crypto.decrypt(Tools.ENCRYPTION_KEY.Reverse().ToString(), strShortTableName);

                var joData = JObject.Parse(strData ?? string.Empty);

                switch (strShortTableName)
                {
                    case "Payment Header":
                    {
                        PaymentHeaderSynchronization paymentHeaderSynchronization = new PaymentHeaderSynchronization();
                        blRVal = paymentHeaderSynchronization.ProcessPaymentHeader(strLongTableName, joData);
                        break;
                    }
                    case "Document Line":
                    {
                        DocumentLineSynchronization documentLineSynchronization = new DocumentLineSynchronization();
                        blRVal = documentLineSynchronization.ProcessDocumentLine(strLongTableName, joData);
                        break;
                    }
                    case "Receipt Header":
                    {
                        PaymentHeaderSynchronization paymentHeaderSynchronization = new PaymentHeaderSynchronization();
                        blRVal = paymentHeaderSynchronization.ProcessPaymentHeader(strLongTableName, joData);
                        break;
                    }
                }

                if (blRVal)
                {
                    return Ok(new
                    {
                        status = 200,
                        message = "OK",
                    });
                }
                else
                {
                    return Problem(title: "Error Occured While Processing the Request", statusCode: 500);
                }
            }
            catch (Exception e)
            {
                return Problem(title: "Error Occured While Processing the Request", statusCode: 500);
            }
        }
    }

    public class SynchronizeRequestPayload
    {
        [Required] public string Payload { get; set; }
    }
}