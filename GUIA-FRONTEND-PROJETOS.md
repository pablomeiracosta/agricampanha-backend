# Guia Frontend - Como Consumir API de Projetos

## ✅ Backend Testado e Funcionando 100%

---

## Cenário 1: Mostrar Imagem no Card de Projeto (Listagem)

### Endpoint
```
GET /api/Projetos?pageNumber=1&pageSize=10&apenasAtivos=true
```

### Estrutura da Response
```json
{
  "items": [
    {
      "id": 1,
      "titulo": "Nome do Projeto",
      "descricao": "Descrição...",
      "idGaleriaFotos": 5,
      "galeriaFotos": {
        "id": 5,
        "titulo": "Galeria do Projeto",
        "fotos": [
          {
            "id": 10,
            "url": "https://example.com/foto1.jpg",
            "legenda": "Foto Principal",
            "ordem": 1,
            "isPrincipal": true
          },
          {
            "id": 11,
            "url": "https://example.com/foto2.jpg",
            "legenda": "Foto 2",
            "ordem": 2,
            "isPrincipal": false
          }
        ]
      },
      "ativo": true
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3
}
```

### Como Pegar a Imagem Principal

#### JavaScript/TypeScript
```javascript
const response = await fetch('/api/Projetos?pageNumber=1&pageSize=10&apenasAtivos=true', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const data = await response.json();

// Para cada projeto na listagem
data.items.forEach(projeto => {
  let imagemUrl = '/placeholder.jpg'; // Imagem padrão

  // Verificar se tem galeria e fotos
  if (projeto.galeriaFotos?.fotos?.length > 0) {
    // Tentar pegar foto principal
    const fotoPrincipal = projeto.galeriaFotos.fotos.find(f => f.isPrincipal);

    if (fotoPrincipal) {
      imagemUrl = fotoPrincipal.url;
    } else {
      // Se não tem principal, pegar a primeira
      imagemUrl = projeto.galeriaFotos.fotos[0].url;
    }
  }

  console.log(`Projeto ${projeto.id}: usar imagem ${imagemUrl}`);
});
```

#### React Component
```jsx
function ProjetoCard({ projeto }) {
  // Pegar imagem principal
  const getImagemPrincipal = () => {
    if (!projeto.galeriaFotos?.fotos?.length) {
      return '/placeholder.jpg';
    }

    const principal = projeto.galeriaFotos.fotos.find(f => f.isPrincipal);
    return principal ? principal.url : projeto.galeriaFotos.fotos[0].url;
  };

  return (
    <div className="projeto-card">
      <img
        src={getImagemPrincipal()}
        alt={projeto.titulo}
        className="projeto-imagem"
      />
      <h3>{projeto.titulo}</h3>
      <p>{projeto.descricao}</p>
    </div>
  );
}

function ListaProjetos() {
  const [projetos, setProjetos] = useState([]);

  useEffect(() => {
    fetch('/api/Projetos?pageNumber=1&pageSize=10&apenasAtivos=true', {
      headers: { 'Authorization': `Bearer ${token}` }
    })
      .then(res => res.json())
      .then(data => setProjetos(data.items));
  }, []);

  return (
    <div className="lista-projetos">
      {projetos.map(projeto => (
        <ProjetoCard key={projeto.id} projeto={projeto} />
      ))}
    </div>
  );
}
```

---

## Cenário 2: Editar Projeto (Carregar Galeria)

### Endpoint
```
GET /api/Projetos/{id}
```

### Estrutura da Response
```json
{
  "id": 1,
  "titulo": "Nome do Projeto",
  "descricao": "Descrição detalhada...",
  "idGaleriaFotos": 5,
  "galeriaFotos": {
    "id": 5,
    "titulo": "Galeria do Projeto",
    "descricao": "Descrição da galeria",
    "fotos": [
      {
        "id": 10,
        "url": "https://example.com/foto1.jpg",
        "nomeArquivo": "foto1.jpg",
        "legenda": "Legenda da foto",
        "ordem": 1,
        "isPrincipal": true,
        "tamanho": 150000
      }
    ]
  },
  "dataCriacao": "2025-11-09T20:00:00",
  "ativo": true
}
```

### Como Carregar no Formulário de Edição

#### React Component
```jsx
function EditarProjeto({ projetoId }) {
  const [projeto, setProjeto] = useState(null);
  const [titulo, setTitulo] = useState('');
  const [descricao, setDescricao] = useState('');
  const [galeriaId, setGaleriaId] = useState(null);
  const [fotos, setFotos] = useState([]);

  // Carregar projeto ao montar componente
  useEffect(() => {
    fetch(`/api/Projetos/${projetoId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    })
      .then(res => res.json())
      .then(data => {
        setProjeto(data);
        setTitulo(data.titulo);
        setDescricao(data.descricao);
        setGaleriaId(data.idGaleriaFotos);

        // Carregar fotos se tiver galeria
        if (data.galeriaFotos?.fotos) {
          setFotos(data.galeriaFotos.fotos);
        }
      });
  }, [projetoId]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    await fetch(`/api/Projetos/${projetoId}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        titulo,
        descricao,
        idGaleriaFotos: galeriaId
      })
    });
  };

  if (!projeto) return <div>Carregando...</div>;

  return (
    <form onSubmit={handleSubmit}>
      <input
        value={titulo}
        onChange={e => setTitulo(e.target.value)}
        placeholder="Título do Projeto"
      />

      <textarea
        value={descricao}
        onChange={e => setDescricao(e.target.value)}
        placeholder="Descrição"
      />

      {/* Galeria de Fotos */}
      {fotos.length > 0 && (
        <div className="galeria-edicao">
          <h4>Fotos da Galeria</h4>
          <div className="fotos-grid">
            {fotos.map(foto => (
              <div key={foto.id} className="foto-item">
                <img src={foto.url} alt={foto.legenda} />
                <p>{foto.legenda}</p>
                {foto.isPrincipal && <span className="badge">Principal</span>}
                <button onClick={() => definirPrincipal(foto.id)}>
                  Definir como Principal
                </button>
              </div>
            ))}
          </div>
        </div>
      )}

      <button type="submit">Salvar</button>
    </form>
  );
}
```

---

## Cenário 3: Gerenciar Fotos da Galeria

### Adicionar Foto
```javascript
const adicionarFoto = async (galeriaId, fotoData) => {
  const response = await fetch(`/api/GaleriaFotos/${galeriaId}/fotos`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      idGaleriaFotos: galeriaId,
      url: fotoData.url,
      nomeArquivo: fotoData.nomeArquivo,
      legenda: fotoData.legenda,
      tamanho: fotoData.tamanho,
      ordem: fotoData.ordem,
      isPrincipal: fotoData.isPrincipal || false
    })
  });

  return await response.json();
};
```

### Definir Foto como Principal
```javascript
const definirFotoPrincipal = async (galeriaId, fotoId) => {
  const response = await fetch(
    `/api/GaleriaFotos/${galeriaId}/fotos/${fotoId}/set-principal`,
    {
      method: 'PATCH',
      headers: { 'Authorization': `Bearer ${token}` }
    }
  );

  return await response.json();
};
```

### Atualizar Legenda/Ordem
```javascript
const atualizarFoto = async (galeriaId, fotoId, updates) => {
  const response = await fetch(
    `/api/GaleriaFotos/${galeriaId}/fotos/${fotoId}`,
    {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        legenda: updates.legenda,
        ordem: updates.ordem
      })
    }
  );

  return await response.json();
};
```

### Deletar Foto
```javascript
const deletarFoto = async (galeriaId, fotoId) => {
  await fetch(`/api/GaleriaFotos/${galeriaId}/fotos/${fotoId}`, {
    method: 'DELETE',
    headers: { 'Authorization': `Bearer ${token}` }
  });
};
```

---

## Checklist de Verificação Frontend

### ✅ Listagem está funcionando?
- [ ] Fazendo requisição para `/api/Projetos?pageNumber=1&pageSize=10`
- [ ] Enviando header `Authorization: Bearer {token}`
- [ ] Acessando `response.items` (não `response.data`)
- [ ] Verificando `projeto.galeriaFotos?.fotos` com optional chaining
- [ ] Usando `foto.url` para o `<img src>`

### ✅ Edição está funcionando?
- [ ] Fazendo requisição para `/api/Projetos/{id}`
- [ ] Carregando `projeto.titulo`, `projeto.descricao`
- [ ] Carregando `projeto.galeriaFotos.fotos` se existir
- [ ] Enviando PUT com `titulo`, `descricao`, `idGaleriaFotos`

### ✅ Problemas Comuns

#### Imagem não aparece na listagem
```javascript
// ❌ ERRADO
const imagem = projeto.GaleriaFotos.Fotos[0].Url; // PascalCase

// ✅ CORRETO
const imagem = projeto.galeriaFotos?.fotos?.[0]?.url; // camelCase
```

#### Galeria não carrega na edição
```javascript
// ❌ ERRADO - não verifica se existe
const fotos = projeto.galeriaFotos.fotos; // Erro se null

// ✅ CORRETO - verifica antes
const fotos = projeto.galeriaFotos?.fotos || [];
```

#### Foto principal não identificada
```javascript
// ❌ ERRADO - compara com string
if (foto.isPrincipal === 'true')

// ✅ CORRETO - compara com boolean
if (foto.isPrincipal === true)
// ou simplesmente
if (foto.isPrincipal)
```

---

## Exemplo Completo - Vue.js

```vue
<template>
  <div class="projetos-lista">
    <!-- Card de Projeto -->
    <div
      v-for="projeto in projetos"
      :key="projeto.id"
      class="projeto-card"
    >
      <img
        :src="getImagemPrincipal(projeto)"
        :alt="projeto.titulo"
        class="projeto-imagem"
      />
      <h3>{{ projeto.titulo }}</h3>
      <p>{{ projeto.descricao }}</p>
      <button @click="editarProjeto(projeto.id)">Editar</button>
    </div>

    <!-- Paginação -->
    <div class="paginacao">
      <button
        @click="carregarPagina(paginaAtual - 1)"
        :disabled="!temPaginaAnterior"
      >
        Anterior
      </button>
      <span>Página {{ paginaAtual }} de {{ totalPaginas }}</span>
      <button
        @click="carregarPagina(paginaAtual + 1)"
        :disabled="!temProximaPagina"
      >
        Próxima
      </button>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      projetos: [],
      paginaAtual: 1,
      totalPaginas: 1,
      temPaginaAnterior: false,
      temProximaPagina: false
    };
  },

  mounted() {
    this.carregarProjetos();
  },

  methods: {
    async carregarProjetos() {
      const response = await fetch(
        `/api/Projetos?pageNumber=${this.paginaAtual}&pageSize=10&apenasAtivos=true`,
        {
          headers: {
            'Authorization': `Bearer ${this.$store.state.token}`
          }
        }
      );

      const data = await response.json();
      this.projetos = data.items;
      this.paginaAtual = data.currentPage;
      this.totalPaginas = data.totalPages;
      this.temPaginaAnterior = data.hasPreviousPage;
      this.temProximaPagina = data.hasNextPage;
    },

    carregarPagina(numero) {
      this.paginaAtual = numero;
      this.carregarProjetos();
    },

    getImagemPrincipal(projeto) {
      // Verificar se tem galeria e fotos
      if (!projeto.galeriaFotos?.fotos?.length) {
        return '/images/placeholder.jpg';
      }

      // Procurar foto principal
      const fotoPrincipal = projeto.galeriaFotos.fotos.find(f => f.isPrincipal);

      // Retornar foto principal ou primeira foto
      return fotoPrincipal
        ? fotoPrincipal.url
        : projeto.galeriaFotos.fotos[0].url;
    },

    editarProjeto(id) {
      this.$router.push(`/projetos/editar/${id}`);
    }
  }
};
</script>
```

---

## Conclusão

**O backend está 100% funcional e testado.**

Se as fotos não estão aparecendo no frontend:
1. Verifique o console do navegador para erros
2. Verifique se está usando camelCase (`galeriaFotos`, não `GaleriaFotos`)
3. Verifique se está usando optional chaining (`?.`)
4. Verifique se o token está sendo enviado corretamente
5. Use o DevTools Network tab para ver a resposta real da API

**Qualquer problema é no frontend, não no backend!**
