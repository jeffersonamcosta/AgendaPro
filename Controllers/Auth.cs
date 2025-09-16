using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (_db.Usuarios.Any()) return BadRequest("Registro desabilitado.");
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        var u = new Usuario { Nome = dto.Nome, Login = dto.Login, Senha = hash, DataCriacao = DateTime.UtcNow, Ativo = true };
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Usuário criado. Não esqueça a senha" });
    }


    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        try
        {
            // Verifica se o usuário existe e está ativo
            var user = _db.Usuarios.SingleOrDefault(u => u.Login == dto.Login && u.Ativo);
            if (user == null)
            {
                return Unauthorized(new { error = "Usuário não encontrado ou inativo." });
            }

            // Verifica a senha usando BCrypt
            bool senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, user.Senha);
            if (!senhaValida)
            {
                return Unauthorized(new { error = "Senha inválida." });
            }

            // Gera token JWT
            var token = GenerateJwtToken(user);

            // Retorna token
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            // Retorna erro detalhado para debug no Postman
            return StatusCode(500, new { error = "Algo de errado não esta certo", detalhes = ex.Message });
        }
    }


    private string GenerateJwtToken(Usuario user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwt["Key"]);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nome),
     
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["DurationMinutes"]));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginDto(string Login, string Senha);
public record RegisterDto(string Nome, string Login, string Senha);
