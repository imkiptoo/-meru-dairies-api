using API.Helpers;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("service/login")]
        public IActionResult ServiceLogin(AuthenticateRequestPayload authenticateRequestPayload)
        {
            var strPayload = authenticateRequestPayload.Payload;

            var authenticateRequest = JsonConvert.DeserializeObject<AuthenticateRequest>(strPayload);

            if (authenticateRequest != null)
            {
                authenticateRequest.Username = Crypto.Decrypt(authenticateRequest.Username);
                authenticateRequest.Password = Crypto.Decrypt(authenticateRequest.Password);

                var response = _userService.Authenticate(authenticateRequest);

                if (response == null)
                {
                    return Ok(new
                    {
                        status = 401,
                        message = "Username or password is incorrect. Please try again",
                    });
                }
                else
                {
                    var responseString = JsonConvert.SerializeObject(response);


                    return Ok(new
                    {
                        status = 200,
                        response = Crypto.Encrypt(responseString)
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = 401,
                    message = "Username or password is incorrect. Please try again",
                });
            }
        }
        
        [HttpPost]
        [Route("login")]
        public IActionResult Login(AuthenticateRequest authenticateRequest)
        {
            if (authenticateRequest != null)
            {
                /*authenticateRequest.Username = Crypto.Decrypt(authenticateRequest.Username);
                authenticateRequest.Password = Crypto.Decrypt(authenticateRequest.Password);*/

                var response = _userService.Authenticate(authenticateRequest);

                if (response == null)
                {
                    return Ok(new
                    {
                        status = 401,
                        message = "Username or password is incorrect. Please try again",
                    });
                }
                else
                {
                    var responseString = JsonConvert.SerializeObject(response);


                    return Ok(new
                    {
                        status = 200,
                        response = Crypto.Encrypt(responseString)
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = 401,
                    message = "Username or password is incorrect. Please try again",
                });
            }
        }
    }
}
