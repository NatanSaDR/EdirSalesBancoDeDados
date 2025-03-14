using EdirSalesBancoDeDados.Application.Interfaces;
using EdirSalesBancoDeDados.Application.UseCases;
using EdirSalesBancoDeDados.Domain.Interfaces;
using EdirSalesBancoDeDados.Infrastructure;
using EdirSalesBancoDeDados.Infrastructure.Repositories;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using EdirSalesBancoDeDados.Application.AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace EdirSalesBancoDeDados.Infra.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration, int n)
        {
            services.AddHttpContextAccessor();

            AddDbContext(services, configuration, n);
            AddUseCase(services);
            AddRepository(services);
            AddFluentMigrator(services, configuration, n);
            AddAutoMapper(services);
            AddJwt(services, configuration);
            return services;
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration, int n)
        {
            //criei uma extension de configuration pra acessar a string de conexao por metodo
            var connectionString = configuration.GetConnectionString(AlterarDB(n));
            services.AddDbContext<EdirSalesContext>(dbContextOptions =>
            {
                //passo a minha connectionstring pro metodo do sql
                dbContextOptions.UseSqlServer(connectionString);
            });
        }
        private static void AddAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
        }
        private static void AddFluentMigrator(IServiceCollection services, IConfiguration configuration, int n)
        {

            var connectionString = configuration.GetConnectionString(AlterarDB(n));

            //função do fluent
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("EdirSalesBancoDeDados.Infrastructure")).For.All();
            });
        }
        private static void AddRepository(IServiceCollection services)
        {
            services.AddScoped<IGrupoRepository, GrupoRepository>();
            services.AddScoped<IMunicipeRepository, MunicipeRepository>();
            services.AddScoped<ISolicitacaoRepository, SolicitacaoRepository>();
            services.AddScoped<IAgenteRepository, AgenteRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

        }
        private static void AddUseCase(IServiceCollection services)
        {
            services.AddScoped<IGrupoUseCase, GrupoUseCase>();
            services.AddScoped<IMunicipeUseCase, MunicipeUseCase>();
            services.AddScoped<ISolicitacaoUseCase, SolicitacaoUseCase>();
            services.AddScoped<IAgenteUseCase, AgenteUseCase>();
            services.AddScoped<IUserUseCase, UserUseCase>();
        }
        private static void AddJwt(IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("Erro: A chave secreta do JWT não foi carregada corretamente.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Secret"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Evita atrasos na expiração do token
                };
            });
        }
        private static string AlterarDB(int n)
        {
            if (n == 1)
                return "ConnectionSQL_Casa";


            return "ConnectionSQL_Escritorio";
            ;
        }
    }
}
