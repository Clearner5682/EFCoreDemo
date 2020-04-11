using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Security;
using Model.DTO;
using System.Reflection;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Utils;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration _configuration;
        private static readonly IEnumerable<UserInfo> _users = new List<UserInfo>
        {
            new UserInfo{ UserId="9999",UserName="admin",Password="1",Phone="99999999"},
            new UserInfo{ UserId="1001",UserName="hongyan",Password="1",Phone="13798165294"}
        };

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            var user = _users.FirstOrDefault(o => o.UserName == userName && o.Password == password);
            if (user != null)
            {
                return new JsonResult(new { ErrorCode = "0", Token = GenerateToken(user) });
            }

            return new JsonResult(new { ErrorCode = "9999", Message = "用户名或密码错误" });
        }

        [HttpPost]
        public IActionResult LoginWX(string code)// 微信登录
        {
            // 注意点：
            // 1、如果参数是放在body的，只能使用实体类、dynamic、JObject来接收，使用简单的值类型是接收不到的
            // 2、如果要用简单的值类型参数接收，则可以将参数放到查询字符串中
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new Exception("code不能为空");
            }
            string appid = _configuration.GetSection("WXSetting").GetValue<string>("AppID");
            string appsecret = _configuration.GetSection("WXSetting").GetValue<string>("AppSecret");
            string url = @$"https://api.weixin.qq.com/sns/jscode2session?appid={appid}&secret={appsecret}&js_code={code}&grant_type=authorization_code";
            WXLoginResult wxLoginResult = null;
            string token = "";


            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                Stream responseStream = httpWebResponse.GetResponseStream();
                using (StreamReader sr = new StreamReader(responseStream))
                {
                    string json = sr.ReadToEnd();
                    wxLoginResult = JsonSerializer.Deserialize<WXLoginResult>(json, new JsonSerializerOptions { AllowTrailingCommas = true, IgnoreNullValues = true });
                    if (wxLoginResult == null)
                    {
                        throw new Exception("调用微信登录接口失败");
                    }
                    if (wxLoginResult.errcode != 0)
                    {
                        throw new Exception("微信小程序登录失败");
                    }
                    // TODO：调用wx.getUserInfo获取用户信息
                    // TODO：将openid、电话、用户名、密码、收货地址等存到业务系统数据库
                    UserInfo wxUser = new UserInfo
                    {
                        UserId = wxLoginResult.openid,
                        UserName = "WX_USER",
                        Phone = "13798165294"
                    };
                    token = GenerateToken(wxUser);

                }
            }

            return new JsonResult(new { ErrorCode = "0", Token = token, WXLoginResult = wxLoginResult });
        }

        [HttpPost]
        public IActionResult CheckToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("token为空");
            }
            string[] tokenArray = token.Split('.');
            if (tokenArray == null || tokenArray.Length != 3)
            {
                throw new Exception("token格式错误");
            }
            var header = JsonSerializer.Deserialize<JWT_Header>(Base64CryptHelper.Decrypt(tokenArray[0], Encoding.UTF8));
            var payload = JsonSerializer.Deserialize<JWT_Payload>(Base64CryptHelper.Decrypt(tokenArray[1], Encoding.UTF8));
            //DateTime exp = (payload.exp*1000).ToDateTime().ToLocalTime();
            //if (exp<DateTime.Now)
            //{
            //    throw new Exception("token已过期");
            //}
            string signature = tokenArray[2];
            var hs256 = new HMACSHA256();
            hs256.Key = Convert.FromBase64String(Base64CryptHelper.Encrypt(_configuration.GetValue<string>("SecretKey"), Encoding.UTF8));
            byte[] hash = hs256.ComputeHash(Encoding.UTF8.GetBytes(tokenArray[0] + "." + tokenArray[1]));
            string newSignature = Convert.ToBase64String(hash).Replace("/", "_").Replace("=", "");

            return new JsonResult(new { IsValid = signature == newSignature });
        }

        private string GenerateToken(UserInfo user)
        {
            string issuer = _configuration.GetValue<string>("Issuer");
            string audience = _configuration.GetValue<string>("Audience");
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.Role,"User"),
                new Claim("UserId",user.UserId),
                new Claim("UserName",user.UserName),
                new Claim("Phone",user.Phone)
            };
            DateTime notBefore = DateTime.Now;
            DateTime expires = DateTime.Now.AddDays(1);
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("SecretKey")));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(issuer, audience, claims, notBefore, expires, signingCredentials);

            string token1 = jwtSecurityToken.ToString();
            string token2 = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token2;
        }
    }
}