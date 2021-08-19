using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.Utilities;
using Newtonsoft.Json;
using SkyCrypto;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;


        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(AuthenticateRequestPayload authenticateRequestPayload)
        {
            var strPayload = authenticateRequestPayload.Payload;

            var authenticateRequest = JsonConvert.DeserializeObject<AuthenticateRequest>(strPayload);

            if (authenticateRequest != null)
            {
                authenticateRequest.Username = Crypto.decrypt(Tools.ENCRYPTION_KEY, authenticateRequest.Username);
                authenticateRequest.Password = Crypto.decrypt(Tools.ENCRYPTION_KEY, authenticateRequest.Password);

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
                        response = Crypto.encrypt(Tools.ENCRYPTION_KEY, responseString)
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
