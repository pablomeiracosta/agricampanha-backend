using AutoMapper;
using Auria.Bll.Services.Interfaces;
using Auria.Data.Entities;
using Auria.Data.Repositories.Interfaces;
using Auria.Dto.Cotacoes;
using Auria.Structure;
using Serilog;

namespace Auria.Bll.Services;

public class CotacaoService : ICotacaoService
{
    private readonly ICotacaoRepository _cotacaoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CotacaoService(
        ICotacaoRepository cotacaoRepository,
        AuriaContext context)
    {
        _cotacaoRepository = cotacaoRepository;
        _mapper = context.Mapper ?? throw new InvalidOperationException("Mapper não configurado");
        _logger = context.Log;
    }

    public async Task<IEnumerable<CotacaoDto>> GetAllAsync()
    {
        try
        {
            var cotacoes = await _cotacaoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CotacaoDto>>(cotacoes);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar todas as cotações");
            throw;
        }
    }

    public async Task<CotacaoDto?> GetByIdAsync(int id)
    {
        try
        {
            var cotacao = await _cotacaoRepository.GetByIdAsync(id);
            return cotacao == null ? null : _mapper.Map<CotacaoDto>(cotacao);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar cotação por ID: {Id}", id);
            throw;
        }
    }

    public async Task<CotacaoComTendenciaDto?> GetMostRecentWithTrendAsync()
    {
        try
        {
            _logger.Information("Buscando cotação mais recente com tendências");

            // Busca as duas cotações mais recentes
            var cotacoesRecentes = await _cotacaoRepository.GetMostRecentAsync(2);
            var listaCotacoes = cotacoesRecentes.ToList();

            if (!listaCotacoes.Any())
            {
                _logger.Warning("Nenhuma cotação encontrada");
                return null;
            }

            var cotacaoMaisRecente = listaCotacoes.First();
            var cotacaoComTendencia = _mapper.Map<CotacaoComTendenciaDto>(cotacaoMaisRecente);

            // Se houver uma segunda cotação, calcular as tendências
            if (listaCotacoes.Count > 1)
            {
                var cotacaoAnterior = listaCotacoes[1];

                cotacaoComTendencia.TendenciaSoja = CalcularTendencia(
                    cotacaoMaisRecente.Soja,
                    cotacaoAnterior.Soja);

                cotacaoComTendencia.TendenciaArroz = CalcularTendencia(
                    cotacaoMaisRecente.Arroz,
                    cotacaoAnterior.Arroz);

                cotacaoComTendencia.TendenciaMilho = CalcularTendencia(
                    cotacaoMaisRecente.Milho,
                    cotacaoAnterior.Milho);

                _logger.Information(
                    "Tendências calculadas - Soja: {TendenciaSoja}, Arroz: {TendenciaArroz}, Milho: {TendenciaMilho}",
                    cotacaoComTendencia.TendenciaSoja,
                    cotacaoComTendencia.TendenciaArroz,
                    cotacaoComTendencia.TendenciaMilho);
            }
            else
            {
                _logger.Information("Apenas uma cotação encontrada, tendências mantidas como 'stable'");
            }

            return cotacaoComTendencia;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao buscar cotação mais recente com tendências");
            throw;
        }
    }

    public async Task<CotacaoDto> CreateAsync(CotacaoCreateDto cotacaoDto)
    {
        try
        {
            _logger.Information("Criando nova cotação - Soja: {Soja}, Arroz: {Arroz}, Milho: {Milho}",
                cotacaoDto.Soja, cotacaoDto.Arroz, cotacaoDto.Milho);

            var cotacao = _mapper.Map<Cotacao>(cotacaoDto);
            cotacao.DataCadastro = DateTime.Now;

            var cotacaoCriada = await _cotacaoRepository.AddAsync(cotacao);
            _logger.Information("Cotação criada com sucesso: ID {Id}", cotacaoCriada.IdCotacao);

            return _mapper.Map<CotacaoDto>(cotacaoCriada);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao criar cotação");
            throw;
        }
    }

    public async Task<CotacaoDto> UpdateAsync(int id, CotacaoUpdateDto cotacaoDto)
    {
        try
        {
            _logger.Information("Atualizando cotação: ID {Id}", id);

            var cotacaoExistente = await _cotacaoRepository.GetByIdAsync(id);
            if (cotacaoExistente == null)
            {
                throw new Exception($"Cotação com ID {id} não encontrada");
            }

            // Atualiza os valores
            cotacaoExistente.Soja = cotacaoDto.Soja;
            cotacaoExistente.Arroz = cotacaoDto.Arroz;
            cotacaoExistente.Milho = cotacaoDto.Milho;

            await _cotacaoRepository.UpdateAsync(cotacaoExistente);
            _logger.Information("Cotação atualizada com sucesso: ID {Id}", id);

            return _mapper.Map<CotacaoDto>(cotacaoExistente);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao atualizar cotação: ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            _logger.Information("Deletando cotação: ID {Id}", id);

            var cotacao = await _cotacaoRepository.GetByIdAsync(id);
            if (cotacao == null)
            {
                _logger.Warning("Cotação não encontrada: ID {Id}", id);
                return false;
            }

            await _cotacaoRepository.DeleteAsync(id);
            _logger.Information("Cotação deletada com sucesso: ID {Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao deletar cotação: ID {Id}", id);
            throw;
        }
    }

    private string CalcularTendencia(decimal valorAtual, decimal valorAnterior)
    {
        if (valorAtual > valorAnterior)
            return "up";
        else if (valorAtual < valorAnterior)
            return "down";
        else
            return "stable";
    }
}
