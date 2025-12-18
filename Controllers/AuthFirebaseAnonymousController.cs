using Microsoft.AspNetCore.Mvc;
using Firebase.Auth;
using Microsoft.Extensions.Configuration; // ДОБАВЬ эту строку

namespace ZaMasterServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthFirebaseAnonymosController : ControllerBase
    {
        private readonly IConfiguration _config; // ДОБАВЬ поле
        private FirebaseAuthLink _authLink;

        // ДОБАВЬ конструктор
        public AuthFirebaseAnonymosController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("auth-anonim")]
        public async Task<IActionResult> AuthFirebaseAnonim()
        {
            try
            {
                // ПОЛУЧАЕМ ключ из конфига
                string firebaseApiKey = _config["FirebaseApiKey"];

                // ИСПОЛЬЗУЕМ его
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));
                _authLink = await authProvider.SignInAnonymouslyAsync();
                string[] _authslinks = new string[] { _authLink.RefreshToken, _authLink.FirebaseToken, _authLink.User.LocalId };

                return Ok(new { success = true, response = "Аутификация прошла успешна ", authLinks = _authslinks });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost("GetIdToken")]
        public async Task<IActionResult> GetIdToken([FromBody] FirebaseAuthAnonim authin)
        {
            try
            {
                var auth = new FirebaseAuth
                {
                    RefreshToken = authin.RefreshToken,
                    FirebaseToken = authin.FirebaseToken,
                    User = new User { LocalId = authin.UserId }
                };

                if (authin.RefreshToken != null && authin.UserId != null)
                {
                    // ТУТ ТОЖЕ получаем ключ из конфига
                    string firebaseApiKey = _config["FirebaseApiKey"];
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));
                    _authLink = await authProvider.RefreshAuthAsync(auth);
                    return Ok(new { success = true, FirebaseToken = _authLink.FirebaseToken });
                }
                else
                {
                    return BadRequest(new { success = false, error = "Ошибка нулл" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }

    public class FirebaseAuthAnonim
    {
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public string FirebaseToken { get; set; }
    }
}