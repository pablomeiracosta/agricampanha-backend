# Relatório de Compatibilidade Frontend x Backend API

**Data da Análise**: 19/11/2025
**Frontend**: React TypeScript
**Backend API**: ASP.NET Core 7.0
**Base URL**: http://website-website-agricampanha.yse2j4.easypanel.host:5000

---

## ❌ INCOMPATIBILIDADES CRÍTICAS IDENTIFICADAS

### 1. NOTÍCIAS - Endpoints Incompatíveis

#### ❌ Frontend usa: `/Noticias?page={}&pageSize={}`
- **Linha 181**: `${API_BASE_URL}/Noticias?page=${page}&pageSize=${pageSize}`
- **Linha 189**: `${API_BASE_URL}/Noticias?page=1&pageSize=${limit}`

#### ✅ Backend possui: `/api/Noticias/paginadas?pageNumber={}&pageSize={}`
- **Status**: ❌ **INCOMPATÍVEL**
- **Impacto**: CRÍTICO - Lista de notícias não funciona
- **Solução**: Alterar frontend para usar `/Noticias/paginadas` com parâmetros `pageNumber` e `pageSize`

---

#### ❌ Frontend usa: `/Noticias/categoria/{id}`
- **Linha 213**: `${API_BASE_URL}/Noticias/categoria/${categoryId}?page=${page}&pageSize=${pageSize}`

#### ✅ Backend possui: `/api/Noticias/por-categoria/{id}`
- **Status**: ❌ **INCOMPATÍVEL**
- **Impacto**: CRÍTICO - Filtro por categoria não funciona
- **Solução**: Alterar frontend para usar `/Noticias/por-categoria/`

---

#### ❌ Frontend usa: `/Noticias/buscar?query={}`
- **Linha 219**: `${API_BASE_URL}/Noticias/buscar?query=${encodeURIComponent(query)}&page=${page}&pageSize=${pageSize}`

#### ✅ Backend possui: Endpoint não existe na API
- **Status**: ❌ **NÃO IMPLEMENTADO**
- **Impacto**: MÉDIO - Busca de notícias não funciona
- **Solução**: Implementar endpoint de busca no backend OU remover do frontend

---

#### ❌ Frontend usa: `/Noticias/upload-imagem`
- **Linha 259**: `${API_BASE_URL}/Noticias/upload-imagem`

#### ✅ Backend possui: `/api/Fotos/upload`
- **Status**: ❌ **INCOMPATÍVEL**
- **Impacto**: CRÍTICO - Upload de imagens de notícias não funciona
- **Solução**: Alterar frontend para usar `/Fotos/upload`

---

### 2. PROJETOS - Endpoints Incompatíveis

#### ❌ Frontend usa: `/Projetos` (sem autenticação pública)
- **Linha 277**: `${API_BASE_URL}/Projetos` com token público

#### ✅ Backend possui: `/api/Projetos` (requer autenticação admin) e `/api/Projetos/publicos` (público)
- **Status**: ❌ **INCOMPATÍVEL**
- **Impacto**: CRÍTICO - Lista de projetos não funciona
- **Solução**: Alterar frontend para usar `/Projetos/publicos` para acesso público

---

### 3. GALERIA - Endpoints Totalmente Incompatíveis

#### ❌ Frontend usa endpoints `/Galerias/*`:
- **Linha 334**: `${API_BASE_URL}/Galerias/${id}`
- **Linha 340**: `${API_BASE_URL}/Galerias`
- **Linha 350**: `${API_BASE_URL}/Galerias/${id}`
- **Linha 362**: `${API_BASE_URL}/Galerias/${id}`
- **Linha 371**: `${API_BASE_URL}/Galerias/${galleryId}/fotos`
- **Linha 382**: `${API_BASE_URL}/Galerias/${galleryId}/fotos/${photoId}`
- **Linha 393**: `${API_BASE_URL}/Galerias/${galleryId}/fotos/${photoId}`
- **Linha 405**: `${API_BASE_URL}/Galerias/upload-foto`

#### ✅ Backend possui endpoints `/api/GaleriaFotos/*`:
- `/api/GaleriaFotos`
- `/api/GaleriaFotos/{id}`
- `/api/GaleriaFotos/por-referencia/{idReferencia}`
- `/api/Fotos/upload` (para upload)

- **Status**: ❌ **TOTALMENTE INCOMPATÍVEL**
- **Impacto**: CRÍTICO - Todo o módulo de galeria não funciona
- **Solução**: Refatorar completamente o módulo de galeria no frontend

---

## ✅ COMPATIBILIDADES CORRETAS

### 1. AUTENTICAÇÃO ✓
- **Frontend**: `POST /Auth/login`
- **Backend**: `POST /api/Auth/login`
- **Status**: ✅ COMPATÍVEL

### 2. CATEGORIAS ✓
- **Frontend**:
  - `GET /Categorias` ✅
  - `GET /Categorias/ativas` ✅
  - `GET /Categorias/{id}` ✅
  - `POST /Categorias` ✅
  - `PUT /Categorias/{id}` ✅
  - `DELETE /Categorias/{id}` ✅
- **Backend**: Todos os endpoints existem
- **Status**: ✅ TOTALMENTE COMPATÍVEL

### 3. NOTÍCIAS - Endpoints Básicos ✓
- **Frontend**:
  - `GET /Noticias/{id}` ✅
  - `POST /Noticias` ✅
  - `PUT /Noticias/{id}` ✅
  - `DELETE /Noticias/{id}` ✅
- **Backend**: Todos os endpoints existem
- **Status**: ✅ COMPATÍVEL (endpoints básicos de CRUD)

---

## 📋 RESUMO DE INCOMPATIBILIDADES

| Módulo | Endpoint Frontend | Endpoint Backend | Status | Prioridade |
|--------|------------------|------------------|--------|------------|
| **Notícias** | `/Noticias?page=` | `/Noticias/paginadas?pageNumber=` | ❌ Incompatível | 🔴 CRÍTICA |
| **Notícias** | `/Noticias/categoria/{id}` | `/Noticias/por-categoria/{id}` | ❌ Incompatível | 🔴 CRÍTICA |
| **Notícias** | `/Noticias/buscar` | ❌ Não existe | ❌ Faltando | 🟡 MÉDIA |
| **Notícias** | `/Noticias/upload-imagem` | `/Fotos/upload` | ❌ Incompatível | 🔴 CRÍTICA |
| **Projetos** | `/Projetos` | `/Projetos/publicos` | ❌ Incompatível | 🔴 CRÍTICA |
| **Galeria** | `/Galerias/*` | `/GaleriaFotos/*` | ❌ Totalmente incompatível | 🔴 CRÍTICA |
| **Galeria** | `/Galerias/upload-foto` | `/Fotos/upload` | ❌ Incompatível | 🔴 CRÍTICA |

---

## 🔧 CORREÇÕES NECESSÁRIAS NO FRONTEND

### Arquivo: `src/services/api.ts`

### 1. Corrigir endpoint de listagem de notícias
```typescript
// ANTES (Linha 181)
getAll: async (page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<News>> => {
  const response = await fetch(`${API_BASE_URL}/Noticias?page=${page}&pageSize=${pageSize}`);
  ...
},

// DEPOIS
getAll: async (page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<News>> => {
  const response = await fetch(`${API_BASE_URL}/Noticias/paginadas?pageNumber=${page}&pageSize=${pageSize}`);
  ...
},
```

### 2. Corrigir endpoint de notícias mais recentes
```typescript
// ANTES (Linha 189)
getLatest: async (limit: number = 4): Promise<News[]> => {
  const response = await fetch(`${API_BASE_URL}/Noticias?page=1&pageSize=${limit}`, {
    headers
  });
  ...
},

// DEPOIS
getLatest: async (limit: number = 4): Promise<News[]> => {
  const response = await fetch(`${API_BASE_URL}/Noticias/recentes/${limit}`, {
    headers
  });
  ...
},
```

### 3. Corrigir endpoint de notícias por categoria
```typescript
// ANTES (Linha 213)
getByCategory: async (categoryId: number, page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<News>> => {
  const response = await fetch(`${API_BASE_URL}/Noticias/categoria/${categoryId}?page=${page}&pageSize=${pageSize}`);
  ...
},

// DEPOIS
getByCategory: async (categoryId: number, page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<News>> => {
  const response = await fetch(`${API_BASE_URL}/Noticias/por-categoria/${categoryId}?pageNumber=${page}&pageSize=${pageSize}`);
  ...
},
```

### 4. Remover ou implementar endpoint de busca
```typescript
// OPÇÃO 1: Remover do frontend
// Comentar ou remover a função search (linhas 218-222)

// OPÇÃO 2: Implementar no backend
// Adicionar endpoint POST /api/Noticias/buscar no backend
```

### 5. Corrigir endpoint de upload de imagem
```typescript
// ANTES (Linha 259)
uploadImage: async (file: File): Promise<{ url: string }> => {
  const formData = new FormData();
  formData.append('file', file);

  const response = await fetch(`${API_BASE_URL}/Noticias/upload-imagem`, {
    ...
  });
  ...
},

// DEPOIS
uploadImage: async (file: File): Promise<{ url: string }> => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('folder', 'noticias');

  const response = await fetch(`${API_BASE_URL}/Fotos/upload`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${getAuthToken()}`,
    },
    body: formData,
  });

  if (!response.ok) throw new Error('Failed to upload image');
  const data = await response.json();
  return { url: data.url }; // Ajustar conforme resposta do backend
},
```

### 6. Corrigir endpoint de projetos públicos
```typescript
// ANTES (Linha 277)
getAll: async (): Promise<Project[]> => {
  try {
    const headers = await createPublicHeaders();
    const response = await fetch(`${API_BASE_URL}/Projetos`, {
      headers
    });
    ...
  }
},

// DEPOIS
getAll: async (): Promise<Project[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/Projetos/publicos`);

    if (!response.ok) {
      console.error('Failed to fetch projects:', response.status);
      return [];
    }

    return await response.json();
  } catch (error) {
    console.error('Error fetching projects:', error);
    return [];
  }
},
```

### 7. Refatorar completamente módulo de galeria
```typescript
// ANTES: galleryApi usa endpoints /Galerias/*
// DEPOIS: Reescrever para usar /GaleriaFotos/* e /Fotos/upload

export const galleryApi = {
  getAll: async (page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<any>> => {
    const response = await fetch(`${API_BASE_URL}/GaleriaFotos?pageNumber=${page}&pageSize=${pageSize}`);
    if (!response.ok) throw new Error('Failed to fetch gallery');
    return await response.json();
  },

  getById: async (id: number): Promise<any> => {
    const response = await fetch(`${API_BASE_URL}/GaleriaFotos/${id}`);
    if (!response.ok) throw new Error('Gallery not found');
    return await response.json();
  },

  getByReference: async (idReferencia: number, tipo: string): Promise<any[]> => {
    const response = await fetch(`${API_BASE_URL}/GaleriaFotos/por-referencia/${idReferencia}?tipo=${tipo}`);
    if (!response.ok) throw new Error('Failed to fetch gallery by reference');
    return await response.json();
  },

  create: async (gallery: any): Promise<any> => {
    const response = await fetch(`${API_BASE_URL}/GaleriaFotos`, {
      method: 'POST',
      headers: createHeaders(true),
      body: JSON.stringify(gallery),
    });
    if (!response.ok) throw new Error('Failed to create gallery');
    return await response.json();
  },

  update: async (id: number, gallery: any): Promise<any> => {
    const response = await fetch(`${API_BASE_URL}/GaleriaFotos/${id}`, {
      method: 'PUT',
      headers: createHeaders(true),
      body: JSON.stringify(gallery),
    });
    if (!response.ok) throw new Error('Failed to update gallery');
    return await response.json();
  },

  delete: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/GaleriaFotos/${id}`, {
      method: 'DELETE',
      headers: createHeaders(true),
    });
    if (!response.ok) throw new Error('Failed to delete gallery');
  },

  uploadPhoto: async (file: File, folder: string = 'galeria'): Promise<{ url: string }> => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('folder', folder);

    const response = await fetch(`${API_BASE_URL}/Fotos/upload`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`,
      },
      body: formData,
    });

    if (!response.ok) throw new Error('Failed to upload photo');
    return await response.json();
  },
};
```

---

## 🎯 PRIORIDADES DE CORREÇÃO

### 🔴 PRIORIDADE CRÍTICA (Corrigir Imediatamente)
1. ✅ **Listagem de notícias paginadas** - Página inicial quebrada
2. ✅ **Projetos públicos** - Módulo de projetos não funciona
3. ✅ **Upload de fotos** - Funcionalidade administrativa quebrada
4. ✅ **Notícias por categoria** - Filtro não funciona

### 🟡 PRIORIDADE MÉDIA (Corrigir em Sprint)
5. ✅ **Módulo de galeria completo** - Refatoração necessária
6. ✅ **Busca de notícias** - Implementar ou remover

---

## 📊 ESTATÍSTICAS

- **Total de Endpoints no Frontend**: ~25
- **Endpoints Compatíveis**: 10 (40%)
- **Endpoints Incompatíveis**: 7 (28%)
- **Endpoints Faltando no Backend**: 1 (4%)
- **Endpoints com Refatoração Necessária**: 7 (28%)

---

## ✅ CHECKLIST DE CORREÇÕES

- [ ] Corrigir `/Noticias` para `/Noticias/paginadas`
- [ ] Corrigir parâmetros `page` para `pageNumber`
- [ ] Corrigir `/Noticias/categoria/` para `/Noticias/por-categoria/`
- [ ] Implementar ou remover `/Noticias/buscar`
- [ ] Corrigir `/Noticias/upload-imagem` para `/Fotos/upload`
- [ ] Corrigir `/Projetos` para `/Projetos/publicos`
- [ ] Refatorar completamente módulo `/Galerias/` para `/GaleriaFotos/`
- [ ] Ajustar resposta de upload de fotos
- [ ] Testar todos os endpoints após correções
- [ ] Atualizar documentação do frontend

---

## 🔗 Referências

- **Swagger Backend**: http://website-website-agricampanha.yse2j4.easypanel.host:5000/swagger/index.html
- **API Test Report**: [API_TEST_REPORT.md](./API_TEST_REPORT.md)
- **Código Frontend**: [src/services/api.ts](../site/src/services/api.ts)

---

**Relatório gerado por**: Claude Code
**Data**: 19/11/2025
**Status**: ❌ **FRONTEND NÃO ESTÁ COMPATÍVEL COM O BACKEND**
**Ação Requerida**: CORREÇÕES URGENTES NECESSÁRIAS
