using System.Globalization;
using AutoMapper;
using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.DTOs.ViewDetailsMunicipe;
using EdirSalesBancoDeDados.Application.Interfaces;
using EdirSalesBancoDeDados.Application.UsuarioLogado;
using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace EdirSalesBancoDeDados.Application.UseCases
{
    public class MunicipeUseCase : BaseUseCase, IMunicipeUseCase
    {
        private readonly IMunicipeRepository _municipeRepository;
        private readonly IGrupoRepository _grupoRepository;
        private readonly IMapper _mapper;
        private readonly ISolicitacaoRepository _solicitacaoRepository;

        public MunicipeUseCase(IMunicipeRepository municipeRepository,
            IMapper mapper,
            IGrupoRepository grupoRepository,
            ISolicitacaoRepository solicitacaoRepository,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _municipeRepository = municipeRepository;
            _mapper = mapper;
            _grupoRepository = grupoRepository;
            _solicitacaoRepository = solicitacaoRepository;
        }

        public async Task<MunicipeDto> AddMunicipe(MunicipeDto municipeDto)
        {
            if (municipeDto == null || municipeDto.Grupos == null)
                throw new ArgumentNullException("Municipe não pode ser nulo ou precisa ter um grupo");

            var municipe = _mapper.Map<Municipe>(municipeDto);

            foreach (var grupo in municipeDto.Grupos)
            {
                var grupoMunicipe = await _grupoRepository.GetById(grupo);
                if (grupoMunicipe != null)
                    municipe.Grupos.Add(grupoMunicipe);
            }

            municipe.DataCadastro = DateTime.UtcNow;
            municipe.UsuarioCadastro = GetUsuarioLogado();

            await _municipeRepository.Add(municipe);
            return municipeDto;
        }

        public async Task<MunicipeDto> AtualizarMunicipe(int id, MunicipeDto municipeDto)
        {
            if (id == 0)
                throw new ArgumentException("ID inválido para atualização.");

            if (municipeDto.Grupos.Count == 0)
                throw new ArgumentNullException("Munícipe precisa ter um grupo");

            var municipe = await _municipeRepository.GetById(id);
            if (municipe == null)
                throw new ArgumentNullException("Munícipe não encontrado.");

            // 🔹 1. Atualizar GRUPOS manualmente
            var gruposExistentesIds = municipe.Grupos.Select(g => g.Id).ToHashSet();
            var novosGruposIds = municipeDto.Grupos.ToHashSet();

            var idsParaAdicionar = novosGruposIds.Except(gruposExistentesIds);
            foreach (var idGrupo in idsParaAdicionar)
            {
                var grupoParaAdicionar = await _grupoRepository.GetById(idGrupo);
                if (grupoParaAdicionar != null)
                    municipe.Grupos.Add(grupoParaAdicionar);
            }

            var idsParaRemover = gruposExistentesIds.Except(novosGruposIds);
            foreach (var idGrupo in idsParaRemover)
            {
                var grupoParaRemover = municipe.Grupos.FirstOrDefault(g => g.Id == idGrupo);
                if (grupoParaRemover != null)
                    municipe.Grupos.Remove(grupoParaRemover);
            }

            // 🔹 2. Atualizar TELEFONES manualmente
            var telefonesExistentes = municipe.Telefones.Select(t => t.Numero).ToHashSet();
            var novosTelefones = municipeDto.Telefones.Select(t => t.Numero).ToHashSet();

            var numerosParaAdicionar = novosTelefones.Except(telefonesExistentes);
            foreach (var numero in numerosParaAdicionar)
            {
                municipe.Telefones.Add(new Telefone
                {
                    Numero = numero,
                    MunicipeId = id,
                    Tipo = municipeDto.Telefones
                        .Where(t => t.Numero == numero)
                        .Select(t => t.Tipo).FirstOrDefault() ?? string.Empty
                });
            }

            var numerosParaRemover = telefonesExistentes.Except(novosTelefones);
            foreach (var numero in numerosParaRemover)
            {
                var telefoneParaRemover = municipe.Telefones.FirstOrDefault(t => t.Numero == numero);
                if (telefoneParaRemover != null)
                    municipe.Telefones.Remove(telefoneParaRemover);
            }

            // 🔹 3. Usar o `_mapper.Map()` IGNORANDO os GRUPOS e TELEFONES
            _mapper.Map(municipeDto, municipe);

            // 🔹 4. Atualizar DataAlteracao e UsuarioAlteracao
            municipe.DataAlteracao = DateTime.UtcNow;
            municipe.UsuarioAlteracao = GetUsuarioLogado();

            // 🔹 5. Chamar o UPDATE no repositório
            await _municipeRepository.Update(municipe);

            return municipeDto;
        }


        public async Task<DetalheMunicipe> BuscarPorId(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id não encontrado");

            var m = await _municipeRepository.GetById(id);
            if (m == null)
                throw new ArgumentNullException("Municipe não pode ser nulo");

            var result = new DetalheMunicipe
            {
                Id = m.Id,
                Nome = m.Nome,
                Sexo = m.Sexo,
                Aniversario = m.Aniversario?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Logradouro = m.Logradouro,
                Numero = m.Numero,
                Complemento = m.Complemento,
                Bairro = m.Bairro,
                Cidade = m.Cidade,
                Estado = m.Estado,
                CEP = m.CEP,
                Observacao = m.Observacao,
                Email = m.Email,
                Telefones = m.Telefones.Select(t => new TelefoneDto
                {
                    Id = t.Id,
                    Numero = t.Numero,
                    Tipo = t.Tipo,
                    Observacao = t.Observacao
                }).ToList(),
                Grupos = m.Grupos.Select(g => $"[{g.Id}] {g.NomeGrupo}").ToList(),
                UsuarioCadastro = m.UsuarioCadastro,
                DataCadastro = m.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                UsuarioAlteracao = m.UsuarioAlteracao,
                DataAlteracao = m.DataAlteracao?.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            };

            foreach (var solicitacao in m.Solicitacoes)
            {
                result.Solicitacoes.Add($"{solicitacao.Id} - {solicitacao.Descricao}");
            }

            return result;
        }

        public async Task DeletarMunicipe(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id não encontrado");

            var municipe = await _municipeRepository.GetById(id)
                ?? throw new KeyNotFoundException("Munícipe não encontrado.");

            var solicitacoesParaExcluir = municipe.Solicitacoes
                .Where(s => s.Municipes.Count == 1 && s.Grupos.Count == 0)
                .ToList();

            await _solicitacaoRepository.DeleteRange(solicitacoesParaExcluir);
            await _municipeRepository.Delete(municipe);
        }

        public async Task<ICollection<DetalheMunicipe>> ListarTodos(int pagina, int tamanhoPagina)
        {
            var municipes = await _municipeRepository.List(pagina, tamanhoPagina);

            if (municipes == null)
                throw new ArgumentNullException("Não existe municipes");

            var municipesMap = _mapper.Map<ICollection<DetalheMunicipe>>(municipes);
            foreach (var municipeMap in municipesMap)
            {
                municipeMap.Grupos = municipes.FirstOrDefault(m => m.Id == municipeMap.Id)?.Grupos.Select(g => $"[{g.Id}] {g.NomeGrupo}").ToList();
            }
            foreach (var municipe in municipesMap)
            {
                if (municipe.Aniversario != null)
                {
                    municipe.Aniversario = DateTime.Parse(municipe.Aniversario).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (municipe.DataCadastro != null)
                {
                    municipe.DataCadastro = DateTime.Parse(municipe.DataCadastro).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                if (municipe.DataAlteracao != null)
                {
                    municipe.DataAlteracao = DateTime.Parse(municipe.DataAlteracao).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
            }
            return municipesMap;
        }

        public async Task<int> CountAll()
        {
            return await _municipeRepository.CountAll();
        }

        public async Task<int> ImportarMunicipes(IFormFile arquivo)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (arquivo == null || arquivo.Length == 0)
            {
                return 0; // Nenhum arquivo enviado
            }

            var listaMunicipes = new List<Municipe>();

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

                        var nome = worksheet.Cells[row, 2].Text?.Trim();
                        var sexo = worksheet.Cells[row, 3].Text?.Trim();
                        var aniversarioStr = worksheet.Cells[row, 4].Text?.Trim();
                        var logradouro = worksheet.Cells[row, 5].Text?.Trim();
                        var numero = worksheet.Cells[row, 6].Text?.Trim();
                        var complemento = worksheet.Cells[row, 7].Text?.Trim();
                        var bairro = worksheet.Cells[row, 8].Text?.Trim();
                        var cidade = worksheet.Cells[row, 9].Text?.Trim();
                        var estado = worksheet.Cells[row, 10].Text?.Trim();
                        var cep = worksheet.Cells[row, 11].Text?.Trim();
                        var observacao = worksheet.Cells[row, 12].Text?.Trim();
                        var email = worksheet.Cells[row, 13].Text?.Trim();
                        var grupo = worksheet.Cells[row, 14].Text?.Trim();
                        var telefones = worksheet.Cells[row, 15].Text?.Trim();
                        var usuarioCadastro = worksheet.Cells[row, 16].Text?.Trim();
                        var dataCadastroStr = worksheet.Cells[row, 17].Text?.Trim();
                        var usuarioAlteracao = worksheet.Cells[row, 18].Text?.Trim();
                        var dataAlteracaoStr = worksheet.Cells[row, 19].Text?.Trim();

                        // Converter datas corretamente
                        DateTime dataMinima = new DateTime(1753, 1, 1);

                        DateTime? aniversario = string.IsNullOrWhiteSpace(aniversarioStr)
                            ? (DateTime?)null
                            : DateTime.TryParseExact(aniversarioStr, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var aniversarioDt) && aniversarioDt >= dataMinima
                                ? aniversarioDt
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

                        if (!string.IsNullOrEmpty(nome)) // Validação mínima
                        {
                            // Buscar todos os grupos de uma vez (evita erro de concorrência)
                            List<Grupo> gruposEncontrados = new List<Grupo>();
                            var gruposList = grupo.Split(',').Select(g =>
                            {
                                if (g == null || string.IsNullOrWhiteSpace(g))
                                {
                                    return 1822;
                                }
                                return int.Parse(g);
                            });

                            foreach (var g in gruposList)
                            {
                                var grupoEncontrado = await _grupoRepository.GetById(g);
                                if (grupoEncontrado != null)
                                {
                                    gruposEncontrados.Add(grupoEncontrado);
                                }
                            }

                            var municipe = new Municipe
                            {
                                Id = id,
                                Nome = nome,
                                Sexo = sexo,
                                Aniversario = aniversario,
                                Logradouro = logradouro,
                                Numero = numero,
                                Complemento = complemento,
                                Bairro = bairro,
                                Cidade = cidade,
                                Estado = estado,
                                CEP = cep,
                                Observacao = observacao,
                                Email = email,
                                Grupos = gruposEncontrados,
                                Telefones = !string.IsNullOrEmpty(telefones)
                                    ? telefones.Split(',')
                                        .Select(numero => numero.Trim()) // Remove espaços desnecessários
                                        .Where(numero => !string.IsNullOrEmpty(numero)) // Ignora valores vazios
                                        .Select(numero => new Telefone
                                        {
                                            Tipo = "Celular", // Tipo fixo
                                            Numero = numero, // Número vindo do Excel
                                            Observacao = "", // Observação vazia
                                            MunicipeId = id // Apenas a chave estrangeira, sem referenciar Municipe
                                        })
                                        .ToList()
                                    : new List<Telefone>(), // Evita null no caso de não haver telefones
                                UsuarioCadastro = usuarioCadastro,
                                DataCadastro = dataCadastro,
                                UsuarioAlteracao = usuarioAlteracao,
                                DataAlteracao = dataAlteracao
                            };

                            listaMunicipes.Add(municipe);
                        }
                    }
                }
            }
            await _municipeRepository.ImportarMunicipes(listaMunicipes);
            return listaMunicipes.Count;
        }

        public async Task<object> Filtrar(
                int? id,
                string? nome,
                string? sexo,
                string? aniversario,
                string? aniversarioInicio,
                string? aniversarioFim,
                string? logradouro,
                string? numero,
                string? complemento,
                string? bairro,
                string? cidade,
                string? estado,
                string? cep,
                string? observacao,
                string? email,
                string? telefone,
                string? grupo,
                string? dataCadastro,
                string? dataCadInicio,
                string? dataCadFim,
                string? dataAlteracao,
                string? dataAltInicio,
                string? dataAltFim,
                string? usuarioCadastro,
                string? usuarioAlteracao,
                int pagina,
                int tamanhoPagina)
        {
            var dataCad = DateTime.TryParseExact(dataCadastro, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataCadastroDt)
                ? dataCadastroDt
                : (DateTime?)null;

            var dataAlt = DateTime.TryParseExact(dataAlteracao, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataAlteracaoDt)
                ? dataAlteracaoDt
                : (DateTime?)null;

            var aniv = DateTime.TryParseExact(aniversario, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var aniversarioDt)
                ? aniversarioDt
                : (DateTime?)null;

            var anivInicio = DateTime.TryParseExact(aniversarioInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var aniversarioInicioDt)
                ? aniversarioInicioDt
                : (DateTime?)null;

            var anivFim = DateTime.TryParseExact(aniversarioFim, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var aniversarioFimDt)
                ? aniversarioFimDt
                : (DateTime?)null;

            var dataInicioCad = DateTime.TryParseExact(dataCadInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataInicioDt)
                ? dataInicioDt
                : (DateTime?)null;

            var dataFimCad = DateTime.TryParseExact(dataCadFim, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataFimDt)
                ? dataFimDt
                : (DateTime?)null;

            var dataInicioAlt = DateTime.TryParseExact(dataAltInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataAltInicioDt)
                ? dataAltInicioDt
                : (DateTime?)null;

            var dataFimAlt = DateTime.TryParseExact(dataAltFim, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataAltFimDt)
                ? dataAltFimDt
                : (DateTime?)null;

            var filtro = await _municipeRepository.Filtrar(
                id, nome, sexo, aniv, anivInicio, anivFim,
                logradouro, numero, complemento, bairro, cidade, estado, cep,
                observacao, email, telefone, grupo, dataCad, dataInicioCad, dataFimCad, dataAlt, dataInicioAlt, dataFimAlt,
                usuarioCadastro, usuarioAlteracao, pagina, tamanhoPagina
            );

            var result = filtro.dados.Select(m => new MunicipeDtoFilter
            {
                Id = m.Id,
                Nome = m.Nome,
                Sexo = m.Sexo,
                //passo o aniversario tratado sem a hora em formato string para o DTO
                Aniversario = m.Aniversario?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Logradouro = m.Logradouro,
                Numero = m.Numero,
                Complemento = m.Complemento,
                Bairro = m.Bairro,
                Cidade = m.Cidade,
                Estado = m.Estado,
                CEP = m.CEP,
                Observacao = m.Observacao,
                Email = m.Email,
                Telefones = m.Telefones.Select(t => new TelefoneDto
                {
                    Id = t.Id,
                    Numero = t.Numero,
                    Tipo = t.Tipo,
                    Observacao = t.Observacao
                }).ToList(),
                Grupos = m.Grupos.Select(g => $"[{g.Id}] {g.NomeGrupo}").ToList(),
                UsuarioCadastro = m.UsuarioCadastro,
                DataCadastro = m.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                UsuarioAlteracao = m.UsuarioAlteracao,
                DataAlteracao = m.DataAlteracao?.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            }).ToList();

            // Retornar um objeto diretamente, sem envolver uma lista
            return new
            {
                totalRegistros = filtro.totalRegistros,
                municipes = result
            };
        }
    }
}
