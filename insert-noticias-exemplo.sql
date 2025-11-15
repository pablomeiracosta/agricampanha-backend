-- Script para inserir 5 notícias de exemplo sobre agronegócio
-- Categorias: 1=Agricultura, 2=Pecuária, 3=Tecnologia no Campo, 4=Sustentabilidade, 5=Mercado e Economia

-- Notícia 1 - Agricultura
INSERT INTO AGRICAMPANHA_NOTICIA (Titulo, Subtitulo, CategoriaId, DataNoticia, Fonte, Texto, ImagemUrl, DataCriacao)
VALUES (
    'Nova técnica de irrigação reduz consumo de água em 40%',
    'Método desenvolvido pela Embrapa promete revolucionar agricultura no semiárido',
    1,
    '2025-11-05',
    'Embrapa',
    'Pesquisadores da Empresa Brasileira de Pesquisa Agropecuária (Embrapa) desenvolveram uma técnica inovadora de irrigação por gotejamento que utiliza sensores inteligentes para monitorar a umidade do solo em tempo real. O sistema aplica água apenas quando necessário, resultando em uma economia de até 40% no consumo de água. Os testes realizados em lavouras de soja no Ceará demonstraram que, além da economia hídrica, a técnica mantém os mesmos níveis de produtividade. A tecnologia está sendo adaptada para uso em diversas culturas, incluindo milho, feijão e hortaliças.',
    'https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=800',
    GETDATE()
);

-- Notícia 2 - Pecuária
INSERT INTO AGRICAMPANHA_NOTICIA (Titulo, Subtitulo, CategoriaId, DataNoticia, Fonte, Texto, ImagemUrl, DataCriacao)
VALUES (
    'Pecuária de precisão aumenta produtividade e bem-estar animal',
    'Tecnologias de monitoramento permitem acompanhamento individual do rebanho',
    2,
    '2025-11-04',
    'Associação Brasileira de Pecuária',
    'A pecuária de precisão está transformando a criação de gado no Brasil. Com o uso de coleiras eletrônicas, sensores e análise de dados, os produtores conseguem monitorar individualmente cada animal do rebanho. Os dispositivos rastreiam temperatura corporal, padrões de movimento e alimentação, permitindo identificar precocemente sinais de doenças ou estresse. Fazendas que adotaram a tecnologia reportam aumento de 25% na produtividade leiteira e redução de 30% no uso de medicamentos veterinários. O sistema também contribui para o bem-estar animal, permitindo manejo mais adequado e individualizado.',
    'https://images.unsplash.com/photo-1560493676-04071c5f467b?w=800',
    GETDATE()
);

-- Notícia 3 - Tecnologia no Campo
INSERT INTO AGRICAMPANHA_NOTICIA (Titulo, Subtitulo, CategoriaId, DataNoticia, Fonte, Texto, ImagemUrl, DataCriacao)
VALUES (
    'Drones agrícolas revolucionam monitoramento de lavouras',
    'Equipamento identifica pragas e doenças antes dos sintomas visíveis',
    3,
    '2025-11-03',
    'AgroTech Brasil',
    'O uso de drones equipados com câmeras multiespectrais está transformando o monitoramento de lavouras no agronegócio brasileiro. A tecnologia permite identificar áreas afetadas por pragas, doenças e deficiências nutricionais antes mesmo que os sintomas sejam visíveis a olho nu. Agricultores que utilizam os drones conseguem detectar problemas até 15 dias antes do aparecimento de sinais visuais, permitindo intervenção precoce e mais eficaz. Segundo especialistas, a detecção antecipada pode aumentar a eficiência das aplicações de defensivos em até 60%, além de reduzir custos e impacto ambiental. A tecnologia já é usada em mais de 5 milhões de hectares no Brasil.',
    'https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=800',
    GETDATE()
);

-- Notícia 4 - Sustentabilidade
INSERT INTO AGRICAMPANHA_NOTICIA (Titulo, Subtitulo, CategoriaId, DataNoticia, Fonte, Texto, ImagemUrl, DataCriacao)
VALUES (
    'Agricultura regenerativa recupera solos degradados e aumenta rentabilidade',
    'Práticas sustentáveis mostram que é possível produzir mais com menor impacto ambiental',
    4,
    '2025-11-02',
    'Instituto de Agricultura Sustentável',
    'A agricultura regenerativa tem ganhado destaque como alternativa sustentável para recuperação de áreas degradadas. O método combina técnicas como plantio direto, rotação de culturas, integração lavoura-pecuária-floresta e uso de plantas de cobertura. Estudos conduzidos em propriedades no Mato Grosso mostram que, após três anos de práticas regenerativas, o solo recuperou 35% da matéria orgânica perdida e a retenção de água aumentou significativamente. Além dos benefícios ambientais, os produtores observaram aumento médio de 20% na rentabilidade, devido à redução no uso de insumos químicos e maior resiliência das lavouras a eventos climáticos extremos.',
    'https://images.unsplash.com/photo-1464226184884-fa280b87c399?w=800',
    GETDATE()
);

-- Notícia 5 - Mercado e Economia
INSERT INTO AGRICAMPANHA_NOTICIA (Titulo, Subtitulo, CategoriaId, DataNoticia, Fonte, Texto, ImagemUrl, DataCriacao)
VALUES (
    'Exportações do agronegócio brasileiro batem recorde histórico',
    'Setor ultrapassa marca de US$ 150 bilhões e lidera balança comercial do país',
    5,
    '2025-11-01',
    'Ministério da Agricultura',
    'O agronegócio brasileiro atingiu um marco histórico ao ultrapassar US$ 150 bilhões em exportações no acumulado do ano. O resultado representa crescimento de 15% em relação ao mesmo período do ano anterior e consolida o Brasil como um dos maiores exportadores mundiais de produtos agrícolas. Os principais produtos responsáveis pelo crescimento foram soja, milho, carne bovina e celulose. A China continua sendo o principal destino das exportações, seguida pela União Europeia e Estados Unidos. Especialistas atribuem o bom desempenho à maior produtividade, investimentos em tecnologia e abertura de novos mercados. O setor representa mais de 25% do PIB nacional e é responsável por 1 em cada 3 empregos no país.',
    'https://images.unsplash.com/photo-1500937386664-56d1dfef3854?w=800',
    GETDATE()
);

-- Verificar inserção
SELECT COUNT(*) AS TotalNoticias FROM AGRICAMPANHA_NOTICIA;
SELECT Id, Titulo, CategoriaId FROM AGRICAMPANHA_NOTICIA ORDER BY Id;

PRINT '========================================';
PRINT '5 notícias de exemplo inseridas com sucesso!';
PRINT '========================================';
