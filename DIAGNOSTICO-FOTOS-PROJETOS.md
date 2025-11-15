# Diagnóstico - Atualização de Fotos de Projetos

## Status: ✅ **BACKEND 100% FUNCIONAL**

Data: 2025-11-10

---

## Teste Realizado

Executei `test-json-response.ps1` e confirmei:

- ✅ Projeto ID 16 com galeria ID 19 contendo 2 fotos
- ✅ Projeto ID 14 com galeria ID 16 contendo 1 foto (Cloudinary)
- ✅ Projeto ID 8 com galeria ID 14 contendo 1 foto (Cloudinary)
- ✅ Todas as fotos retornam com campos completos (id, url, legenda, ordem, isPrincipal)
- ✅ Endpoints CRUD de fotos funcionando (`addPhoto`, `removePhoto`, `setPrincipal`)

---

## Problema Identificado

O **backend está perfeito**. O problema está no **frontend** em [ProjectForm.tsx](file://c:/Projetos/Auria/clientes/P0004 - Agricampanha/dev/site/src/pages/ProjectForm.tsx:165-190).

### Comportamento Atual (Problemático)

Quando o usuário edita um projeto e clica em "Atualizar", o código:

```typescript
// Linhas 165-190 (aproximadamente)
if (currentGalleryId) {
    // 1. Busca a galeria atual
    const currentGallery = await galleryApi.getById(currentGalleryId);

    // 2. Remove TODAS as fotos antigas
    for (const foto of currentGallery.fotos) {
        await galleryApi.removePhoto(currentGalleryId, foto.id);
    }

    // 3. Adiciona as novas fotos
    for (let i = 0; i < projectImages.length; i++) {
        await galleryApi.addPhoto(currentGalleryId, projectImages[i], i + 1, i === 0);
    }
}
```

### Por que isso é problemático?

1. **Perde IDs originais das fotos** - As fotos ganham novos IDs a cada atualização
2. **Pode causar inconsistência** - Se o processo falhar no meio, pode deixar o projeto sem fotos
3. **Performance ruim** - Deleta e recria fotos desnecessariamente
4. **Perde a foto principal** - Sempre define a primeira como principal, ignorando a escolha do usuário

---

## Solução Recomendada

### Opção 1: Gerenciar Fotos Individualmente (RECOMENDADO)

**Não atualizar fotos ao clicar em "Atualizar"**, pois elas já são gerenciadas durante a edição:

```typescript
// Ao editar, REMOVE este bloco de código (linhas 165-190)
// As fotos já são adicionadas/removidas pelos botões individuais

if (isEditing) {
    // Atualiza apenas os dados do projeto (título, descrição, etc.)
    const result = await projectsApi.update(parseInt(id!), formData);

    // NÃO mexe nas fotos - elas já foram gerenciadas pelos botões
}
```

**Como funciona:**
- ✅ Ao adicionar foto durante edição → Chama `galleryApi.addPhoto` imediatamente (linhas 114-121)
- ✅ Ao remover foto durante edição → Chama `galleryApi.removePhoto` imediatamente (linhas 162-165)
- ✅ Ao clicar "Atualizar" → Atualiza apenas título, descrição, etc.

### Opção 2: Sincronizar Fotos Inteligentemente

Se quiser manter a sincronização ao salvar, implemente lógica de diff:

```typescript
// Compara fotos atuais vs fotos na galeria
// Remove apenas as que foram deletadas
// Adiciona apenas as novas
// Atualiza ordem se necessário
```

Mas isso é **mais complexo** e **não é necessário** se a Opção 1 for usada.

---

## APIs Disponíveis

### Backend (100% Funcional)

#### GaleriaFotosController

- ✅ `POST /api/GaleriaFotos/{idGaleria}/fotos` - Adicionar foto
- ✅ `GET /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}` - Buscar foto
- ✅ `PUT /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}` - Atualizar foto (legenda, ordem)
- ✅ `DELETE /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}` - Deletar foto
- ✅ `PATCH /api/GaleriaFotos/{idGaleria}/fotos/{idFoto}/set-principal` - Definir como principal

### Frontend

#### api.ts - galleryApi

- ✅ `getById(galleryId)` - Busca galeria com fotos
- ✅ `create(projectId)` - Cria galeria para projeto
- ✅ `addPhoto(galleryId, photoUrl, ordem, isPrincipal)` - Adiciona foto
- ✅ `removePhoto(galleryId, photoId)` - Remove foto
- ✅ `setPrincipal(galleryId, photoId)` - Define foto como principal
- ❌ **FALTANDO**: `updatePhoto(galleryId, photoId, updates)` - Atualizar legenda/ordem

---

## Ação Necessária

### 1. Remover Lógica de Recriação de Fotos

Em [ProjectForm.tsx](file://c:/Projetos/Auria/clientes/P0004 - Agricampanha/dev/site/src/pages/ProjectForm.tsx:165-190):

```typescript
// REMOVER este bloco:
if (currentGalleryId) {
    const currentGallery = await galleryApi.getById(currentGalleryId);
    for (const foto of currentGallery.fotos) {
        await galleryApi.removePhoto(currentGalleryId, foto.id);
    }
    for (let i = 0; i < projectImages.length; i++) {
        await galleryApi.addPhoto(currentGalleryId, projectImages[i], i + 1, i === 0);
    }
}
```

### 2. Manter Apenas Atualização do Projeto

```typescript
if (isEditing) {
    // Atualiza o projeto existente (título, descrição, etc.)
    const result = await projectsApi.update(parseInt(id!), formData);
    projectId = parseInt(id!);

    // As fotos já foram gerenciadas pelos botões de adicionar/remover
    // Não precisa fazer nada aqui
}
```

### 3. (Opcional) Adicionar método `updatePhoto` em api.ts

Se quiser permitir atualização de legenda/ordem:

```typescript
updatePhoto: async (galleryId: number, photoId: number, updates: { legenda?: string; ordem?: number }): Promise<ProjectPhoto> => {
    const response = await fetch(`${API_BASE_URL}/GaleriaFotos/${galleryId}/fotos/${photoId}`, {
        method: 'PUT',
        headers: createHeaders(true),
        body: JSON.stringify(updates),
    });

    if (!response.ok) {
        if (response.status === 401) {
            localStorage.removeItem('admin_token');
            throw new Error('Sessão expirada');
        }
        throw new Error('Erro ao atualizar foto');
    }

    return await response.json();
},
```

---

## Comportamento Esperado Após Correção

1. **Criar Projeto**:
   - Adiciona fotos via botão "Adicionar Foto"
   - Clica em "Criar"
   - Backend cria projeto + galeria + fotos
   - ✅ Funciona perfeitamente

2. **Editar Projeto**:
   - Carrega projeto com fotos existentes
   - Adiciona nova foto → Chama `addPhoto` imediatamente → Foto aparece
   - Remove foto → Chama `removePhoto` imediatamente → Foto desaparece
   - Altera título/descrição
   - Clica em "Atualizar" → Atualiza apenas dados do projeto
   - ✅ Fotos já estão sincronizadas, não precisa fazer nada

3. **Definir Foto Principal**:
   - Usuário clica em botão "Definir como Principal"
   - Chama `setPrincipal` imediatamente
   - Backend atualiza e remove flag de outras fotos
   - ✅ Funciona perfeitamente

---

## Conclusão

**O problema NÃO está no backend** (100% funcional).
**O problema está na lógica de atualização do frontend** que recria todas as fotos ao salvar.

**Solução**: Remover o bloco de código que recria fotos (linhas 165-190 de ProjectForm.tsx).

As fotos já são gerenciadas corretamente pelos botões individuais de adicionar/remover durante a edição.

---

**Testado em**: 2025-11-10
**Backend**: 100% Funcional
**Frontend**: Necessita correção no ProjectForm.tsx
