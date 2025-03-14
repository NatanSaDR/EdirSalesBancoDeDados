using AutoMapper;
using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.Interfaces;
using EdirSalesBancoDeDados.Application.UsuarioLogado;
using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Globalization;

namespace EdirSalesBancoDeDados.Application.UseCases
{
    public class AgenteUseCase : BaseUseCase, IAgenteUseCase
    {
        private readonly IAgenteRepository _agenteRepository;
        private readonly IMapper _mapper;
        public AgenteUseCase(IAgenteRepository agenteRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _agenteRepository = agenteRepository;
            _mapper = mapper;
        }

        public async Task<AgenteDto> AddAgente(AgenteDto agenteDto)
        {
            if (agenteDto == null)
                throw new ArgumentNullException(nameof(agenteDto), "Agente não pode ser nulo");


            var agente = _mapper.Map<Agente>(agenteDto);

            agente.Contatos = agenteDto.Contatos.Select(
                c => new ContatoAgente
                {
                    Contato = c,
                    DataCadastro = DateTime.UtcNow,
                    UsuarioCadastro = GetUsuarioLogado()
                }
              ).ToList();

            agente.DataCadastro = DateTime.UtcNow;
            agente.UsuarioCadastro = GetUsuarioLogado();
            await _agenteRepository.Add(agente);
            return agenteDto;
        }

        public async Task<AgenteDto> AtualizarAgente(int id, AgenteDto agenteDto)
        {
            if (agenteDto == null || id == 0)
                throw new ArgumentNullException("Id ou Agente não pode ser nulos");

            var agente = await _agenteRepository.GetById(id);

            if (agente == null)
                throw new ArgumentNullException("Id não encontrado");

            var contatosNovos = agenteDto.Contatos?.ToList() ?? new List<string>();
            var contatosAntigos = agente.Contatos?.ToList() ?? new List<ContatoAgente>();

            // Identificando contatos que devem ser removidos
            var contatosRemovidos = contatosAntigos.Where(c => !contatosNovos.Contains(c.Contato)).ToList();
            foreach (var contato in contatosRemovidos)
            {
                agente.Contatos.Remove(contato);
            }

            // Identificando contatos que devem ser adicionados ou atualizados
            foreach (var contatoStr in contatosNovos)
            {
                var contatoExistente = contatosAntigos.FirstOrDefault(c => c.Contato == contatoStr);

                if (contatoExistente == null)
                {
                    // Novo contato: adiciona ao agente
                    var novoContato = new ContatoAgente
                    {
                        Contato = contatoStr,
                        AgenteId = id,
                        DataCadastro = DateTime.UtcNow,
                        UsuarioCadastro = GetUsuarioLogado()
                    };
                    agente.Contatos.Add(novoContato);
                }
                else
                {
                    // Contato já existe: apenas atualiza a DataAlteracao
                    contatoExistente.DataAlteracao = DateTime.UtcNow;
                    contatoExistente.UsuarioAlteracao = GetUsuarioLogado();
                }
            }

            // Atualizando o agente sem sobrescrever os contatos
            _mapper.Map(agenteDto, agente);

            agente.DataAlteracao = DateTime.UtcNow;
            agente.UsuarioAlteracao = GetUsuarioLogado();

            await _agenteRepository.Update(agente);
            return agenteDto;
        }


        public async Task<int> CountAll()
        {
            return await _agenteRepository.CountAll();
        }

        public async Task<AgenteDto> BuscarPorId(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id invalido");

            var agente = await _agenteRepository.GetById(id);

            if (agente == null)
                throw new ArgumentNullException("Agente não econtrado");

            var agenteDto = _mapper.Map<AgenteDto>(agente);
            return agenteDto;
        }

        public async Task DeletarAgente(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id ínválido");

            var agente = await _agenteRepository.GetById(id);
            if (agente == null)
                throw new ArgumentNullException("Agente não encontrado na base");
          
            await _agenteRepository.Delete(agente);
        }

        public async Task<ICollection<AgenteDto>> ListarTodos(int pagina, int tamanhoPagina)
        {
            var result = await _agenteRepository.List(pagina, tamanhoPagina);

            if (result == null)
                throw new ArgumentException("Sem Resultados");

            return _mapper.Map<ICollection<AgenteDto>>(result);
        }

        public async Task<int> ImportarAgentes(IFormFile arquivo)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (arquivo == null || arquivo.Length == 0)
            {
                return 0; // Nenhum arquivo enviado
            }

            var listaAgentes = new List<Agente>();

            using (var stream = new MemoryStream())
            {
                await arquivo.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Pega a primeira aba
                    int rowCount = worksheet.Dimension.Rows; // Conta as linhas do Excel

                    for (int row = 2; row <= rowCount; row++) // Começa da linha 2 (ignorando cabeçalhos)
                    {
                        var idStr = worksheet.Cells[row, 1].Text.Trim();
                        int id = int.TryParse(idStr, out var parsedId) ? parsedId : 0; // Evita erro de conversão

                        var nomeAgente = worksheet.Cells[row, 2].Text?.Trim();
                        var usuarioCadastro = worksheet.Cells[row, 3].Text?.Trim();
                        var dataCadastroStr = worksheet.Cells[row, 4].Text?.Trim();
                        var usuarioAlteracao = worksheet.Cells[row, 5].Text?.Trim();
                        var dataAlteracaoStr = worksheet.Cells[row, 6].Text?.Trim();

                        // Definir data mínima permitida no SQL Server
                        DateTime dataMinima = new DateTime(1753, 1, 1);

                        // Converter datas corretamente
                        DateTime dataCadastro = DateTime.TryParseExact(
                            dataCadastroStr,
                            "dd/MM/yyyy HH:mm:ss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var cadastroDt) && cadastroDt >= dataMinima
                            ? cadastroDt
                            : DateTime.Now; // Se for inválido, assume data atual

                        DateTime? dataAlteracao = string.IsNullOrWhiteSpace(dataAlteracaoStr)
                            ? (DateTime?)null
                            : DateTime.TryParseExact(dataAlteracaoStr, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var alteracaoDt) && alteracaoDt >= dataMinima
                                ? alteracaoDt
                                : (DateTime?)null; // Se for inválido, assume nulo

                        if (!string.IsNullOrEmpty(nomeAgente)) // Validação mínima
                        {
                            var agente = new Agente
                            {
                                Id = id,
                                AgenteSolucao = nomeAgente,
                                UsuarioCadastro = usuarioCadastro,
                                DataCadastro = dataCadastro,
                                UsuarioAlteracao = usuarioAlteracao,
                                DataAlteracao = dataAlteracao
                            };

                            listaAgentes.Add(agente);
                        }
                    }
                }
            }

            await _agenteRepository.ImportarAgentes(listaAgentes);
            return listaAgentes.Count;
        }
    }
}
