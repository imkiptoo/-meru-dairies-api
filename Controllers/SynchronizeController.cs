using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using API.Helpers;
using API.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [ApiController]
    [Route("api/synchronize")]
    [Authorize]
    public class SynchronizeController : ControllerBase
    {
        private readonly ILogger<SynchronizeController> _logger;

        public SynchronizeController(ILogger<SynchronizeController> logger)
        {
            _logger = logger;
            Tools._logger = logger;
        }
        
        [HttpPost]
        public IActionResult Post(SynchronizeRequestPayload synchronizeRequestPayload)
        {
            bool blRVal = false;
            try
            {
                Print.PrettyLog(theTopOnly: true);
                Print.PrettyLog("Received Synchronization Request");
                Print.PrettyLog(theBottomOnly: true);

                var strRequestPayload = synchronizeRequestPayload.Payload;

                strRequestPayload = Crypto.Decrypt(strRequestPayload);

                var joDataJson = JObject.Parse(strRequestPayload ?? string.Empty);

                var strData = joDataJson["data"]?.ToString();
                var strLongTableName = joDataJson["long_table_name"]?.ToString();
                var strShortTableName = joDataJson["short_table_name"]?.ToString();

                strData = Crypto.Decrypt(strData);
                strLongTableName = Crypto.Decrypt(strLongTableName);
                strShortTableName = Crypto.Decrypt(strShortTableName);

                var joData = JObject.Parse(strData ?? string.Empty);
                
                if (Tools.service!.MainCompanySyncEnabled)
                {
                    strLongTableName = $"[{Tools.service!.MainCompanyName}${strShortTableName}]";
                    blRVal = SynchronizeData(strLongTableName, strShortTableName, joData);
                }
                else
                {
                    blRVal = SynchronizeData(strLongTableName, strShortTableName, joData);
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
                Console.Write(e.StackTrace);
                return Problem(title: "Error Occured While Processing the Request", statusCode: 500);
            }
        }

        public bool SynchronizeData(string strLongTableName, string strShortTableName, JObject joData)
        {
            bool blRVal = false;
            try
            {
                Print.PrettyLog($"Table to synchronize: {strLongTableName}");
                Print.PrettyLog(theBottomOnly: true);
                
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
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return blRVal;
        }
    }

    public class SynchronizeRequestPayload
    {
        [Required] public string Payload { get; set; }
    }
}