# 🧪 Avaliação Técnica Vue + .NET — ALPHA

Projeto criado como parte da avaliação técnica para vaga de **Desenvolvedor JR**.  
Desenvolvido por **Augusto** — Analista e Desenvolvedor de Sistemas 👨‍💻

---

## 🚀 Tecnologias Utilizadas

- **Backend:** ASP.NET Core Web API (.NET 7)
- **Frontend:** Vue.js 3 + Vite
- **Estilização:** TailwindCSS
- **Banco de dados:** SQL Server + Entity Framework Core
- **Integração externa:** Fake Store API
- **Documentação:** Swagger

---

## 📁 Estrutura de Pastas

```
/alpha
├── ProdutoApi/        → Projeto .NET (API)
├── Frontend/          → Projeto Vue.js (interface)
├── README.md
├── .gitignore
```

---

## ⚙️ Como Executar o Projeto

### ✅ Pré-requisitos

- .NET SDK 7
- SQL Server em execução
- Node.js + NPM
- Visual Studio / VS Code
- Navegador atualizado

---

### 🔧 Backend (.NET API)

1. Clone o repositório:
   ```bash
   git clone https://github.com/SEU-USUARIO/avaliacao-vue-dotnet.git
   cd ProdutoApi
   ```

2. Crie o banco de dados e aplique as migrations:
   ```bash
   dotnet ef database update
   ```

3. Execute a API:
   ```bash
   dotnet run
   ```

A API estará disponível em: `http://localhost:5091`

---

### 🎨 Frontend (Vue 3)

1. Acesse a pasta do frontend:
   ```bash
   cd ../Frontend
   npm install
   npm run dev
   ```

2. Acesse a aplicação em:  
   [http://localhost:5173](http://localhost:5173)

> ⚠️ A API precisa estar rodando em `http://localhost:5091/api/produtos`

---

## 🔁 Integração com Fake Store

- Ao **criar, editar ou excluir** um produto:
  - Ele é **replicado na Fake Store API**
  - A imagem enviada como **base64** é convertida para `.png` e salva no disco
  - Uma **URL pública** (ex: `http://localhost:5091/imagens/xyz.png`) é gerada e enviada para a Fake Store

---

## 🧾 Endpoints da API

Documentação completa disponível em `/swagger`

### Exemplos:

- `GET /api/produtos`  
  Filtros opcionais: `?nome=`, `?codigoBarras=`, `?ordenarPor=preco&ordem=asc|desc&page=1&pageSize=10`

- `POST /api/produtos`  
  Corpo:
  ```json
  {
    "nome": "Camisa",
    "preco": 129.90,
    "codigoBarras": "1234567890001",
    "imagemBase64": "iVBORw0KGgoAAAANSUhEUgAAA..."
  }
  ```

- `PUT /api/produtos/{id}`
- `DELETE /api/produtos/{id}`

---

## 💻 Funcionalidades do Frontend

- ✅ Cadastro de produtos (nome, preço, código de barras, imagem base64)
- ✅ Listagem de produtos com visual em cards
- ✅ Filtros por nome e código de barras
- ✅ Ordenação por preço (asc/desc)
- ✅ Edição e exclusão com modal reutilizável
- ✅ Modal dinâmico para criar e editar
- ✅ Atualização automática da lista ao salvar

---

## 🧱 Estrutura de Componentes Vue

| Componente         | Função                                                                 |
|--------------------|------------------------------------------------------------------------|
| `App.vue`          | Estrutura principal que orquestra lista e formulário                   |
| `ProdutoLista.vue` | Lista todos os produtos com filtros, ordenação e botões de ação        |
| `ProdutoForm.vue`  | Formulário modal para criação e edição de produto                      |

---

## 💡 Soluções Adotadas

- Fake Store exige `categoria` → enviado como `"geral"` por padrão
- Fake Store exige imagem como URL → convertida de base64 e salva localmente

---

## 🖼️ Prints do Sistema

> 📌 Coloque suas imagens na pasta `/screenshots` e atualize os links abaixo:

- 🧾 Formulário de Produto  
- 📋 Lista com Filtros e Ações  
- ✏️ Modal de Edição  

---

## ✅ Conclusão

Todos os requisitos da avaliação foram atendidos:

- CRUD completo com frontend e backend integrados
- Upload e conversão de imagem base64
- Comunicação com API externa (Fake Store)
- Interface moderna, responsiva e funcional
- Organização de código, logs, filtros e paginação
