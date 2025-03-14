using AutoMapper;
using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.DTOs.ViewDetails;
using EdirSalesBancoDeDados.Application.Interfaces;
using EdirSalesBancoDeDados.Application.UsuarioLogado;
using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Globalization;

namespace EdirSalesBancoDeDados.Application.UseCases
{
    public class GrupoUseCase : BaseUseCase, IGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IMunicipeRepository _municipeRepository;
        private readonly ISolicitacaoRepository _solicitacaoRepository;
        private readonly IMapper _mapper;

        public GrupoUseCase(IGrupoRepository grupoRepository, IMapper mapper, IMunicipeRepository municipeRepository, ISolicitacaoRepository solicitacaoRepository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _grupoRepository = grupoRepository ?? throw new ArgumentNullException(nameof(grupoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _municipeRepository = municipeRepository ?? throw new ArgumentNullException(nameof(municipeRepository));
            _solicitacaoRepository = solicitacaoRepository ?? throw new ArgumentNullException(nameof(solicitacaoRepository));
        }

        public async Task<GrupoDto> AddGrupo(GrupoDto grupoDto)
        {
            if (grupoDto == null)
                throw new ArgumentNullException("Dados do grupo são obrigatórios.");

            var grupo = _mapper.Map<Grupo>(grupoDto);
            grupo.DataCadastro = DateTime.Now;
            grupo.UsuarioCadastro = GetUsuarioLogado();
            await _grupoRepository.Add(grupo);
            return grupoDto;
        }


        public async Task<GrupoDto> AtualizarGrupo(int id, GrupoDto grupoDto)
        {
            if (id == 0)
                throw new ArgumentException("ID do grupo não pode ser zero.");
            if (grupoDto == null)
                throw new ArgumentNullException("Dados do grupo são obrigatórios para atualizar.");

            var grupo = await _grupoRepository.GetById(id);
            if (grupo == null)
                throw new ArgumentNullException("Dados do grupo são obrigatórios para atualizar.");

            _mapper.Map(grupoDto, grupo);

            grupo.DataAlteracao = DateTime.UtcNow;
            grupo.UsuarioAlteracao = GetUsuarioLogado();

            await _grupoRepository.Update(grupo);
            return grupoDto;
        }

        public async Task<DetalheGrupoDto> BuscarPorId(int id)
        {
            if (id == 0)
                throw new ArgumentException("ID do grupo não pode ser zero.");

            var result = await _grupoRepository.DetalhesDoGrupo(id);
            if (result == null)
                throw new ArgumentNullException("Grupo não encontrado");

            var details = new DetalheGrupoDto
            {
                IdGrupo = id,
                NomeGrupo = result.NomeGrupo,
                Municipes = result.Municipes
                .Select(m => new DetalheMunicipeGrupo
                {
                    IdMunicipe = m.Id,
                    NomeMunicipe = m.Nome
                }).ToList()
            };
            return details;
        }

        public async Task DeletarGrupo(int id)
        {
            if (id == 0)
                throw new ArgumentException("ID do grupo não pode ser zero.");

            // Obtém o grupo e verifica se ele existe
            var grupo = await _grupoRepository.GetById(id)
                ?? throw new KeyNotFoundException("Grupo não encontrado para exclusão.");

            // Municipes vinculados ao grupo que estão apenas neste grupo
            var municipesParaExcluir = grupo.Municipes
                                            .Where(m => m.Grupos.Count == 1)
                                            .ToList();

            // Solicitações vinculadas ao grupo que estão apenas neste grupo
            var solicitacoesGruposParaExcluir = grupo.Solicitacoes
                                                     .Where(s => s.Grupos.Count == 1 && s.Municipes.Count == 0)
                                                     .ToList();

            // Solicitações de municipes que contenha somente o municipe que será excluido
            var solicitacoesMunicipesParaExcluir = municipesParaExcluir
                    .SelectMany(m => m.Solicitacoes)
                    .Where(s => s.Municipes.Count == 1 && s.Grupos.Count == 0)
                    .ToList();

            //Remove solicitacoes que ficaram sem grupo e sem municipe
            await _solicitacaoRepository.DeleteRange(solicitacoesMunicipesParaExcluir);
            await _solicitacaoRepository.DeleteRange(solicitacoesGruposParaExcluir);
            //Deleta os municipes que ficaram sem grupos
            await _municipeRepository.DeleteRange(municipesParaExcluir);

            // Por fim, exclui o grupo
            await _grupoRepository.Delete(grupo);
        }


        public async Task<IEnumerable<GrupoDto>> ListarTodos(int pagina, int tamanhoPagina)
        {
            var grupos = await _grupoRepository.List(pagina, tamanhoPagina);
            if (grupos == null)
                throw new ArgumentNullException("Nenhum grupo encontrado.");

            return _mapper.Map<IEnumerable<GrupoDto>>(grupos);
        }
        public async Task<List<GrupoDto>> Filtrar(string? nome, int pagina = 1, int tamanhoPagina = 20)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentNullException("Nome ou id incorreto");
            var result = await _grupoRepository.Filtrar(nome, pagina, tamanhoPagina);
            if (result == null) throw new ArgumentNullException("Nenhum grupo encontrado");
            return _mapper.Map<List<GrupoDto>>(result);
        }

        public async Task<int> CountAll()
        {
            return await _grupoRepository.CountAll();
        }
        //importar municipes
        public async Task<int> ImportarGrupos(IFormFile arquivo)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (arquivo == null || arquivo.Length == 0)
            {
                return 0; // Nenhum arquivo enviado
            }

            var listaGrupos = new List<Grupo>();

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

                        var nomeGrupo = worksheet.Cells[row, 2].Text?.Trim();
                        var usuarioCadastro = worksheet.Cells[row, 3].Text?.Trim();
                        var dataCadastroStr = worksheet.Cells[row, 4].Text?.Trim();
                        var usuarioAlteracao = worksheet.Cells[row, 5].Text?.Trim();
                        var dataAlteracaoStr = worksheet.Cells[row, 6].Text?.Trim();

                        // Converter datas corretamente
                        DateTime dataCadastro = DateTime.TryParseExact(
                                                dataCadastroStr,
                                                "dd/MM/yyyy HH:mm:ss",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out var cadastroDt) ? cadastroDt : DateTime.Now;

                        DateTime? dataAlteracao = DateTime.TryParseExact(dataAlteracaoStr, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var alteracaoDt) ? alteracaoDt : (DateTime?)null;


                        if (!string.IsNullOrEmpty(nomeGrupo)) // Validação mínima
                        {
                            var grupo = new Grupo
                            {
                                Id = id,
                                NomeGrupo = nomeGrupo,
                                UsuarioCadastro = usuarioCadastro,
                                DataCadastro = dataCadastro,
                                UsuarioAlteracao = usuarioAlteracao,
                                DataAlteracao = dataAlteracao
                            };
                            listaGrupos.Add(grupo);
                        }
                    }
                }
            }
            await _grupoRepository.ImportarGrupos(listaGrupos);
            return listaGrupos.Count;
        }
    }
}
