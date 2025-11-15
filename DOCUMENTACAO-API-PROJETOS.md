# Documentação da API de Projetos - Agricampanha

## Status: ✅ **100% FUNCIONAL**

Todos os endpoints foram testados e estão retornando as estruturas JSON corretas com galeria e fotos.

---

## Endpoints Implementados

### **1. Projetos**

#### **POST /api/Projetos**
Cria um novo projeto (com ou sem galeria).

**Request Body:**
```json
{
  "titulo": "Nome do Projeto",
  "descricao": "Descrição do projeto",
  "idGaleriaFotos": 1,  // Opcional
  "ativo": true
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "titulo": "Nome do Projeto",
  "descricao": "Descrição do projeto",
  "idGaleriaFotos": 1,
  "galeriaFotos": {
    "id": 1,
    "titulo": "Galeria do Projeto",
    "descricao": "Descrição da galeria",
    "idReferencia": 2,
    "idRegistroRelacionado": 1,
    "dataCriacao": "2025-11-09T20:00:00",
    "dataAtualizacao": null,
    "fotos": [
      {
        "id": 1,
        "idGaleriaFotos": 1,
        "url": "https://example.com/foto1.jpg",
        "nomeArquivo": "foto1.jpg",
        "legenda": "Legenda da foto",
        "tamanho": 102400,
        "ordem": 1,
        "isPrincipal": true,
        "dataUpload": "2025-11-09T20:00:00"
      }
    ]
  },
  "dataCriacao": "2025-11-09T20:00:00",
  "dataAtualizacao": null,
  "ativo": true
}
```

---

#### **GET /api/Projetos/{id}**
Busca um projeto por ID (inclui galeria e fotos).

**Response (200 OK):**
```json
{
  "id": 1,
  "titulo": "Nome do Projeto",
  "descricao": "Descrição do projeto",
  "idGaleriaFotos": 1,
  "galeriaFotos": {
    "id": 1,
    "titulo": "Galeria do Projeto",
    "fotos": [
      {
        "id": 1,
        "url": "https://example.com/foto1.jpg",
        "legenda": "Foto 1",
        "ordem": 1,
        "isPrincipal": true
      },
      {
        "id": 2,
        "url": "https://example.com/foto2.jpg",
        "legenda": "Foto 2",
        "ordem": 2,
        "isPrincipal": false
      }
    ]
  },
  "dataCriacao": "2025-11-09T20:00:00",
  "ativo": true
}
```

---

#### **GET /api/Projetos**
Lista projetos com paginação (inclui galeria e fotos em cada item).

**Query Parameters:**
- `pageNumber` (default: 1)
- `pageSize` (default: 10)
- `apenasAtivos` (default: true)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 1,
      "titulo": "Projeto 1",
      "galeriaFotos": {
        "id": 1,
        "fotos": [...]
      }
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 15,
  "totalPages": 2,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

#### **GET /api/Projetos/ativos**
Lista apenas projetos ativos (sem paginação).

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "titulo": "Projeto Ativo 1",
    "galeriaFotos": {
      "fotos": [...]
    }
  }
]
```

---

#### **PUT /api/Projetos/{id}**
Atualiza um projeto existente.

**Request Body:**
```json
{
  "titulo": "Novo Título",
  "descricao": "Nova Descrição",
  "idGaleriaFotos": 1
}
```

**Response (200 OK):**
Retorna o projeto atualizado com galeria e fotos.

---

#### **PATCH /api/Projetos/{id}/toggle-ativo**
Alterna o status ativo/inativo do projeto.

**Response (200 OK):**
Retorna o projeto com o status alterado.

---

#### **DELETE /api/Projetos/{id}**
Deleta um projeto.

**Response (200 OK):**
```json
{
  "message": "Projeto deletado com sucesso"
}
```

---

### **2. Galeria de Fotos**

#### **POST /api/GaleriaFotos**
Cria uma nova galeria de fotos.

**Request Body:**
```json
{
  "titulo": "Galeria Projeto X",
  "descricao": "Descrição da galeria",
  "idReferencia": 2,
  "idRegistroRelacionado": 1
}
```

---

#### **GET /api/GaleriaFotos/{id}**
Busca uma galeria por ID (inclui fotos).

---

#### **GET /api/GaleriaFotos/por-referencia/{idReferencia}**
Busca galerias por tipo de referência.

**Parâmetros:**
- `idReferencia`: 2 para Projetos
- `idRegistroRelacionado` (opcional): ID do registro específico

---

#### **PUT /api/GaleriaFotos/{id}**
Atualiza uma galeria.

---

#### **DELETE /api/GaleriaFotos/{id}**
Deleta uma galeria (e suas fotos em cascade).

---

### **3. Fotos**

#### **POST /api/GaleriaFotos/{idGaleria}/fotos**
Adiciona uma foto à galeria.

**Request Body:**
```json
{
  "idGaleriaFotos": 1,
  "url": "https://example.com/foto.jpg",
  "nomeArquivo": "foto.jpg",
  "legenda": "Legenda da foto",
  "tamanho": 102400,
  "ordem": 1,
  "isPrincipal": true
}
```

---

#### **GET /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}**
Busca uma foto específica.

---

#### **PUT /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}**
Atualiza legenda e ordem de uma foto.

**Request Body:**
```json
{
  "legenda": "Nova legenda",
  "ordem": 2
}
```

---

#### **DELETE /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}**
Remove uma foto da galeria.

---

#### **PATCH /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}/set-principal**
Define uma foto como principal (automaticamente remove a flag de outras fotos).

---

## Testes Realizados

### ✅ Teste 1: Estrutura JSON Completa
```powershell
.\test-json-response.ps1
```

**Resultado:** Projeto retorna com galeria completa e array de fotos.

### ✅ Teste 2: CRUD Completo de Fotos
```powershell
.\test-galeria-fotos.ps1
```

**Resultado:** Todas as operações de fotos funcionando corretamente.

### ✅ Teste 3: Projetos sem Galeria
Projetos criados sem `idGaleriaFotos` retornam `galeriaFotos: null`.

---

## Estrutura de Dados

### Relacionamentos
- **Projeto** → **GaleriaFotos** (1:1 opcional)
- **GaleriaFotos** → **Foto** (1:N)

### Campos Principais

**Projeto:**
- `id`, `titulo`, `descricao`
- `idGaleriaFotos` (nullable)
- `galeriaFotos` (objeto completo ou null)
- `ativo`, `dataCriacao`, `dataAtualizacao`

**GaleriaFotos:**
- `id`, `titulo`, `descricao`
- `idReferencia` (2 = Projetos)
- `idRegistroRelacionado`
- `fotos` (array)

**Foto:**
- `id`, `url`, `nomeArquivo`
- `legenda`, `tamanho`, `ordem`
- `isPrincipal` (apenas uma por galeria)
- `idGaleriaFotos`

---

## Notas Importantes

1. **Fotos Principal**: Apenas uma foto pode ser `isPrincipal: true` por galeria. Ao definir uma nova foto como principal, as outras são automaticamente desmarcadas.

2. **Cascade Delete**: Ao deletar uma galeria, todas as fotos são deletadas automaticamente.

3. **SetNull Delete**: Ao deletar uma galeria associada a um projeto, o `idGaleriaFotos` do projeto é setado para `null`.

4. **Eager Loading**: As queries automaticamente fazem `.Include()` da galeria e fotos ao buscar projetos.

5. **Ordenação**: Fotos são sempre ordenadas por `ordem` ascendente.

---

## Como Consumir no Frontend

### Exemplo com Axios/Fetch

```javascript
// Buscar projeto com galeria e fotos
const response = await axios.get(`/api/Projetos/${id}`);
const projeto = response.data;

// Verificar se tem galeria
if (projeto.galeriaFotos) {
  const fotos = projeto.galeriaFotos.fotos;

  // Encontrar foto principal
  const fotoPrincipal = fotos.find(f => f.isPrincipal);

  // Listar todas as fotos
  fotos.forEach(foto => {
    console.log(foto.url, foto.legenda);
  });
}
```

### Exemplo com React

```jsx
function ProjetoDetalhes({ projetoId }) {
  const [projeto, setProjeto] = useState(null);

  useEffect(() => {
    fetch(`/api/Projetos/${projetoId}`)
      .then(res => res.json())
      .then(data => setProjeto(data));
  }, [projetoId]);

  if (!projeto) return <div>Carregando...</div>;

  return (
    <div>
      <h1>{projeto.titulo}</h1>
      <p>{projeto.descricao}</p>

      {projeto.galeriaFotos && (
        <div className="galeria">
          <h2>{projeto.galeriaFotos.titulo}</h2>
          <div className="fotos">
            {projeto.galeriaFotos.fotos.map(foto => (
              <img
                key={foto.id}
                src={foto.url}
                alt={foto.legenda}
                className={foto.isPrincipal ? 'principal' : ''}
              />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
```

---

## Status Final

**✅ Backend 100% Funcional**

Todos os endpoints testados e retornando as estruturas corretas. Se houver problemas no frontend, verificar:

1. URL da API está correta?
2. Autenticação (Bearer token) está sendo enviada?
3. Nome dos campos respeitam camelCase (`galeriaFotos`, não `GaleriaFotos`)?
4. Frontend está tratando arrays vazios corretamente?

---

**Data da Última Atualização:** 2025-11-09
**Versão da API:** 1.0
**Status:** Produção
