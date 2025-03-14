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
    public class SolicitacaoUseCase : BaseUseCase, ISolicitacaoUseCase
    {
        private readonly ISolicitacaoRepository _solicitacaoRepository;
        private readonly IGrupoRepository _grupoRepository;
        private readonly IMunicipeRepository _municipeRepository;
        private readonly IAgenteRepository _agenteRepository;
        private readonly IMapper _mapper;
        public SolicitacaoUseCase(
            ISolicitacaoRepository solicitacaoRepository,
            IMapper mapper,
            IMunicipeRepository municipeRepository,
            IGrupoRepository grupoRepository,
            IAgenteRepository agenteRepository,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _solicitacaoRepository = solicitacaoRepository;
            _mapper = mapper;
            _municipeRepository = municipeRepository;
            _grupoRepository = grupoRepository;
            _agenteRepository = agenteRepository;
        }
        public async Task<SolicitacaoDto> AddSolicitacao(SolicitacaoDto solicitacaoDto)
        {
            if (solicitacaoDto == null)
                throw new ArgumentNullException(nameof(solicitacaoDto), "Solicitação não pode ser nula");

            var solicitacao = _mapper.Map<Solicitacao>(solicitacaoDto);
            // Verifica se algum dos ofícios já existe em outra solicitação
            foreach (var numeroOficio in solicitacaoDto.Oficios)
            {
                var solicitacaoExistenteId = await _solicitacaoRepository.GetSolicitacaoIdByOficio(numeroOficio);
                if (solicitacaoExistenteId != 0)
                {
                    throw new InvalidOperationException($"O ofício '{numeroOficio}' já existe na solicitação com ID {solicitacaoExistenteId.Value}");
                }
            }

            // Adiciona os Municipes e Grupos relacionados
            foreach (var dtoStr in solicitacaoDto.IdMunicipes)
            {
                var dto = int.Parse(dtoStr);

                if (dto != 0)
                {
                    var municipe = await _municipeRepository.GetById(dto)
                        ?? throw new KeyNotFoundException("Municipe não encontrado.");

                    municipe.Solicitacoes.Add(solicitacao);
                }
            }

            foreach (var dtoStr in solicitacaoDto.IdGrupos)
            {
                var dto = int.Parse(dtoStr);

                if (dto != 0)
                {

                    var grupo = await _grupoRepository.GetById(dto)
                        ?? throw new KeyNotFoundException("grupo não encontrado.");

                    grupo.Solicitacoes.Add(solicitacao);


                }
            }

            foreach (var dtoStr in solicitacaoDto.IdAgentes)
            {
                var dto = int.Parse(dtoStr);

                if (dto != 0)
                {

                    var agente = await _agenteRepository.GetById(dto)
                            ?? throw new KeyNotFoundException("grupo não encontrado.");
                    agente.Solicitacoes.Add(solicitacao);

                }
            }

            // Mapeia e adiciona os Ofícios
            solicitacao.Oficios = solicitacaoDto.Oficios.Select(numero => new Oficio
            {
                NumeroOficio = numero
            }).ToList();
            solicitacao.DataCadastro = DateTime.UtcNow;
            solicitacao.UsuarioCadastro = GetUsuarioLogado();
            await _solicitacaoRepository.Add(solicitacao);
            return solicitacaoDto;
        }

        public async Task<SolicitacaoDto> AtualizarSolicitacao(int id, SolicitacaoDto solicitacaoDto)
        {
            if (solicitacaoDto == null)
                throw new ArgumentNullException(nameof(solicitacaoDto), "Solicitação não pode ser nula");

            var solicitacao = await _solicitacaoRepository.GetById(id)
                              ?? throw new KeyNotFoundException("Solicitação não encontrada.");

            _mapper.Map(solicitacaoDto, solicitacao);

            var novosOficios = solicitacaoDto.Oficios?.Select(oficioNum => new Oficio
            {
                NumeroOficio = oficioNum,
                SolicitacaoId = id
            }).ToList();

            solicitacao.Oficios = novosOficios?.Union(solicitacao.Oficios)
                                 .DistinctBy(o => o.NumeroOficio)
                                 .ToList() ?? new List<Oficio>();

            var municipesExistentes = solicitacao.Municipes.Select(m => m.Id).ToHashSet();
            var novosMunicipesIds = solicitacaoDto.IdMunicipes.Select(i => int.Parse(i)).ToHashSet();


            var idsParaAdicionar = novosMunicipesIds.Except(municipesExistentes);
            foreach (var idMunicipe in idsParaAdicionar)
            {
                var municipesParaAdicionar = await _municipeRepository.GetById(idMunicipe);
                if (municipesParaAdicionar != null)
                    solicitacao.Municipes.Add(municipesParaAdicionar);
            }

            var idsParaRemover = municipesExistentes.Except(novosMunicipesIds);
            foreach (var idMunicipe in idsParaRemover)
            {
                var grupoParaRemover = solicitacao.Municipes.FirstOrDefault(g => g.Id == idMunicipe);
                if (grupoParaRemover != null)
                    solicitacao.Municipes.Remove(grupoParaRemover);
            }

            var gruposExistentes = solicitacao.Grupos.Select(m => m.Id).ToHashSet();
            var novosGruposIds = solicitacaoDto.IdGrupos.Select(i => int.Parse(i)).ToHashSet();

            var idsParaAdicionarGrupos = novosGruposIds.Except(gruposExistentes);
            foreach (var idGrupo in idsParaAdicionarGrupos)
            {
                var gruposParaAdicionar = await _grupoRepository.GetById(idGrupo);
                if (gruposParaAdicionar != null)
                    solicitacao.Grupos.Add(gruposParaAdicionar);
            }

            var idsParaRemoverGrupos = gruposExistentes.Except(novosGruposIds);
            foreach (var idGrupo in idsParaRemoverGrupos)
            {
                var grupoParaRemover = solicitacao.Grupos.FirstOrDefault(g => g.Id == idGrupo);
                if (grupoParaRemover != null)
                    solicitacao.Grupos.Remove(grupoParaRemover);
            }

            var agentesExistentes = solicitacao.Agentes.Select(a => a.Id).ToHashSet();
            var novosAgentesIds = solicitacaoDto.IdAgentes.Select(i => int.Parse(i)).ToHashSet();

            var idsParaAdicionarAgentes = novosAgentesIds.Except(agentesExistentes);
            foreach (var idAgente in idsParaAdicionarAgentes)
            {
                var agentesParaAdicionar = await _agenteRepository.GetById(idAgente);
                if (agentesParaAdicionar != null)
                    solicitacao.Agentes.Add(agentesParaAdicionar);
            }

            var idsParaRemoverAgentes = agentesExistentes.Except(novosAgentesIds);
            foreach (var idAgente in idsParaRemoverAgentes)
            {
                var agenteParaRemover = solicitacao.Agentes.FirstOrDefault(g => g.Id == idAgente);

                if (agenteParaRemover != null)
                    solicitacao.Agentes.Remove(agenteParaRemover);
            }

            solicitacao.DataAlteracao = DateTime.UtcNow;
            solicitacao.UsuarioAlteracao = GetUsuarioLogado();
            await _solicitacaoRepository.Update(solicitacao);
            return solicitacaoDto;
        }


        public async Task<SolicitacaoDto> BuscarPorId(int id)
        {
            var solicitacao = await _solicitacaoRepository.GetById(id)
                              ?? throw new KeyNotFoundException("Solicitação não encontrada.");

            var solicitacaoDto = _mapper.Map<SolicitacaoDto>(solicitacao);
            solicitacaoDto.Oficios = solicitacao.Oficios.Select(s => s.NumeroOficio).ToList();
            solicitacaoDto.IdMunicipes = solicitacao.Municipes?.Select(m => $"{m.Id}").ToList();
            solicitacaoDto.IdGrupos = solicitacao.Grupos?.Select(g => $"{g.Id}").ToList();
            solicitacaoDto.IdAgentes = solicitacao.Agentes?.Select(a => $"{a.Id}").ToList();

            return solicitacaoDto;
        }

        public async Task DeletarSolicitacao(int id)
        {
            var solicitacao = await _solicitacaoRepository.GetById(id)
                              ?? throw new KeyNotFoundException("Solicitação não encontrada.");
            await _solicitacaoRepository.Delete(solicitacao);
        }

        public async Task<ICollection<SolicitacaoDto>> ListarTodos(int pagina, int tamanhoPagina)
        {
            pagina = pagina == 0 ? 1 : pagina;
            var solicitacoes = await _solicitacaoRepository.List(pagina, tamanhoPagina);

            return solicitacoes.Select(s => new SolicitacaoDto
            {
                Id = s.Id,
                Tipo = s.Tipo,
                Descricao = s.Descricao,
                Observacao = s.Observacao,
                SEI = s.SEI,
                Status = s.Status,
                DataFinalizado = s.DataFinalizado,
                Oficios = s.Oficios?.Select(o => o.NumeroOficio).ToList(),
                IdMunicipes = s.Municipes?.Select(m => $"[{m.Id}] {m.Nome}").ToList(),
                IdGrupos = s.Grupos?.Select(g => $"[{g.Id}] {g.NomeGrupo}").ToList(),
                IdAgentes = s.Agentes?.Select(a => $"[{a.Id}] {a.AgenteSolucao}").ToList()
            }).ToList();
        }

        public async Task<List<SolicitacaoDto>> Filtrar(
                string? tipo,
                string? descricao,
                string? observacao,
                string? sei,
                string? status,
                DateTime? dataFinalizado,
                DateTime? dataFinalizadoInicio,
                DateTime? dataFinalizadoFim,
                DateTime? dataCadastroInicio,
                DateTime? dataCadastroFim,
                string? usuarioCadastro,
                string? usuarioAlteracao,
                string? grupo,
                string? agente,
                string? municipe,
                int pagina,
                int tamanhoPagina
                )
        {
            var solicitacoes = await _solicitacaoRepository.Filtrar(
                tipo, descricao, observacao, sei, status,
                dataFinalizado, dataFinalizadoInicio, dataFinalizadoFim,
                dataCadastroInicio, dataCadastroFim, usuarioCadastro, usuarioAlteracao,
                grupo, agente, municipe, pagina, tamanhoPagina);

            var solicitacaoDto = _mapper.Map<List<SolicitacaoDto>>(solicitacoes);


            var resultado =
                solicitacoes.Select(s => new SolicitacaoDto
                {
                    Id = s.Id,
                    Tipo = s.Tipo,
                    Descricao = s.Descricao,
                    Observacao = s.Observacao,
                    SEI = s.SEI,
                    Status = s.Status,
                    DataFinalizado = s.DataFinalizado,
                    Oficios = s.Oficios?.Select(o => o.NumeroOficio).ToList(),
                    IdMunicipes = s.Municipes?.Select(m => $"[{m.Id}] {m.Nome}").ToList(),
                    IdGrupos = s.Grupos?.Select(g => $"[{g.Id}] {g.NomeGrupo}").ToList(),
                    IdAgentes = s.Agentes?.Select(a => $"[{a.Id}] {a.AgenteSolucao}").ToList()
                }).ToList();
            return resultado;



        }
        public async Task<int> CountAll()
        {
            return await _solicitacaoRepository.CountAll();
        }

        public async Task<int> ImportarSolicitacoes(IFormFile arquivo)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (arquivo == null || arquivo.Length == 0)
            {
                return 0; // Nenhum arquivo enviado
            }

            var listaSolicitacoes = new List<Solicitacao>();

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

                        var tipo = worksheet.Cells[row, 2].Text?.Trim();
                        var descricao = worksheet.Cells[row, 3].Text?.Trim();
                        var observacao = worksheet.Cells[row, 4].Text?.Trim();
                        var sei = worksheet.Cells[row, 5].Text?.Trim();
                        var status = worksheet.Cells[row, 6].Text?.Trim();
                        var dataCadastroStr = worksheet.Cells[row, 7].Text?.Trim();
                        var usuarioCadastro = worksheet.Cells[row, 8].Text?.Trim();
                        var dataAlteracaoStr = worksheet.Cells[row, 9].Text?.Trim();
                        var usuarioAlteracao = worksheet.Cells[row, 10].Text?.Trim();

                        var grupos = worksheet.Cells[row, 11].Text?.Trim();
                        var municipes = worksheet.Cells[row, 12].Text?.Trim();

                        var agentes = worksheet.Cells[row, 13].Text?.Trim();
                        var dataFinalizadoStr = worksheet.Cells[row, 9].Text?.Trim();
                        var numOficio = worksheet.Cells[row, 14].Text?.Trim();
                        // Definir data mínima permitida no SQL Server
                        DateTime dataMinima = new DateTime(1753, 1, 1);

                        // Converter datas corretamente
                        DateTime? dataFinalizado = string.IsNullOrWhiteSpace(dataFinalizadoStr)
                            ? (DateTime?)null
                            : DateTime.TryParseExact(dataFinalizadoStr, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var finalizadoDt) && finalizadoDt >= dataMinima
                                ? finalizadoDt
                                : (DateTime?)null; // Se for inválido, assume nulo

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

                        // Buscar grupos, agentes e munícipes no banco de dados
                        List<Grupo> gruposEncontrados = new List<Grupo>();

                        if (!string.IsNullOrWhiteSpace(grupos))
                        {
                            var gruposList = grupos.Split(',').Select(g => int.Parse(g));
                            {
                                foreach (var g in gruposList)
                                {

                                    var grupoEncontrado = await _grupoRepository.GetById(g);
                                    if (grupoEncontrado != null)
                                    {
                                        gruposEncontrados.Add(grupoEncontrado);
                                    }
                                }
                            }
                        }


                        List<Agente> agentesEncontrados = new List<Agente>();

                        if (!string.IsNullOrWhiteSpace(agentes))
                        {
                            var agentesList = agentes?.Split(',').Select(a => int.Parse(a));

                            foreach (var a in agentesList)
                            {
                                var agenteEncontrado = await _agenteRepository.GetById(a);
                                if (agenteEncontrado != null)
                                {
                                    agentesEncontrados.Add(agenteEncontrado);
                                }
                            }
                        }

                        List<Municipe> municipesEncontrados = new List<Municipe>();

                        if (!string.IsNullOrWhiteSpace(municipes))
                        {

                            var municipesList = municipes?.Split(',').Select(m => int.Parse(m));

                            foreach (var m in municipesList)
                            {
                                var municipeEncontrado = await _municipeRepository.GetById(m);
                                if (municipeEncontrado != null)
                                {
                                    municipesEncontrados.Add(municipeEncontrado);
                                }
                            }
                        }

                        List<Oficio> oficios = new List<Oficio>();

                        if (!string.IsNullOrEmpty(numOficio))
                        {
                            var numerosOficios = numOficio.Split(',').Select(o => o.Trim()).Where(o => !string.IsNullOrEmpty(o)).ToList();
                            foreach (var numero in numerosOficios)
                            {
                                oficios.Add(new Oficio
                                {
                                    NumeroOficio = numero,
                                    SolicitacaoId = id
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(tipo)) // Validação mínima
                        {
                            var solicitacao = new Solicitacao
                            {
                                Id = id,
                                Tipo = tipo,
                                Descricao = descricao,
                                Observacao = observacao,
                                SEI = sei,
                                Status = status,
                                DataFinalizado = dataAlteracao,
                                UsuarioCadastro = usuarioCadastro,
                                DataCadastro = dataCadastro,
                                UsuarioAlteracao = usuarioAlteracao,
                                DataAlteracao = dataAlteracao,
                                Grupos = gruposEncontrados,
                                Agentes = agentesEncontrados,
                                Municipes = municipesEncontrados,
                                Oficios = oficios
                            };
                            listaSolicitacoes.Add(solicitacao);
                        }
                    }
                }
            }

            await _solicitacaoRepository.ImportarSolicitacoes(listaSolicitacoes);
            return listaSolicitacoes.Count;
        }
    }
}
