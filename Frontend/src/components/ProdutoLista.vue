<template>
  <div>
    <!-- Filtros de busca e ordenação para refinar a lista de produtos -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <input v-model="filtroNome" placeholder="Filtrar por nome" class="input" @input="filtrar" />
      <input v-model="filtroCodigo" placeholder="Código de Barras" class="input" @input="filtrar" />
      <select v-model="ordemPreco" class="input" @change="filtrar">
        <option value="">Organizar por valor</option>
        <option value="asc">Mais barato</option>
        <option value="desc">Mais caro</option>
      </select>
    </div>

    <!-- Lista paginada de produtos filtrados -->
    <div class="grid gap-4 sm:grid-cols-1 md:grid-cols-2 xl:grid-cols-3">
      <div
        v-for="produto in paginaAtualProdutos"
        :key="produto.id"
        class="bg-white rounded-lg shadow hover:shadow-md transition-all border border-gray-200 p-4 flex flex-col"
      >
        <!-- Exibição da imagem do produto (caso disponível) -->
        <div class="w-full flex justify-center mb-4">
          <img
            v-if="produto.imagemBase64"
            :src="'data:image/png;base64,' + produto.imagemBase64"
            alt="Imagem"
            class="w-full max-w-[160px] h-auto object-contain rounded"
          />
        </div>

        <!-- Informações principais do produto -->
        <h2 class="font-bold text-lg text-gray-800 mb-1">{{ produto.nome }}</h2>
        <p class="text-indigo-700 font-semibold mb-1">
          R$ {{ produto.preco.toLocaleString('pt-BR', { minimumFractionDigits: 2 }) }}
        </p>
        <p class="text-sm text-gray-500 mb-4">Código de Barras: {{ produto.codigoBarras }}</p>

        <!-- Ações disponíveis para o produto: editar ou excluir -->
        <div class="mt-auto flex justify-between gap-2">
          <button
            @click="$emit('editar', produto)"
            class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded w-full"
          >
            Editar
          </button>
          <button
            @click="abrirModalExclusao(produto.id)"
            class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded w-full"
          >
            Excluir
          </button>
        </div>
      </div>
    </div>

    <!-- Controles de navegação entre páginas -->
    <div v-if="totalPaginas > 1" class="flex justify-center items-center mt-6 gap-2">
      <button
        :disabled="paginaAtual === 1"
        @click="paginaAtual--"
        class="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
      >
        Anterior
      </button>
      <span class="px-2">Página {{ paginaAtual }} de {{ totalPaginas }}</span>
      <button
        :disabled="paginaAtual === totalPaginas"
        @click="paginaAtual++"
        class="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
      >
        Próxima
      </button>
    </div>

    <!-- Exibição de mensagem quando não houver produtos -->
    <div v-if="produtosFiltrados.length === 0" class="text-center text-gray-500 text-lg mt-10">
      Nenhum produto foi localizado.
    </div>

    <!-- Modal de confirmação de exclusão de produto -->
    <div v-if="mostrarModal" class="fixed inset-0 z-50 bg-black bg-opacity-40 flex items-center justify-center p-4">
      <div class="bg-white rounded-lg shadow-xl p-6 w-full max-w-md text-center">
        <h3 class="text-lg font-semibold mb-4 text-gray-800">Deseja realmente excluir este produto?</h3>
        <div class="flex justify-center gap-4 mt-6">
          <button @click="cancelarExclusao" class="px-4 py-2 bg-gray-300 hover:bg-gray-400 rounded">Cancelar</button>
          <button @click="confirmarExclusao" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded">Confirmar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  // Recebe a lista de produtos como prop do componente pai
  props: {
    produtos: Array
  },
  // Define o nome do componente
  data() {
    return {
      filtroNome: '',             
      filtroCodigo: '',          
      ordemPreco: '',              
      produtosFiltrados: [],       
      paginaAtual: 1,              
      itensPorPagina: 9,           
      mostrarModal: false,         
      idParaExcluir: null          
    };
  },
  computed: {
    // Calcula o número total de páginas com base nos itens filtrados
    totalPaginas() {
      return Math.ceil(this.produtosFiltrados.length / this.itensPorPagina);
    },
    // Obtém os produtos correspondentes à página atual
    paginaAtualProdutos() {
      const inicio = (this.paginaAtual - 1) * this.itensPorPagina;
      const fim = inicio + this.itensPorPagina;
      return this.produtosFiltrados.slice(inicio, fim);
    }
  },
  watch: {
    // Aplica os filtros automaticamente sempre que a lista de produtos for atualizada
    produtos: {
      immediate: true,
      handler() {
        this.filtrar();
      }
    }
  },
  methods: {
    // Aplica filtros por nome, código e ordena por preço
    filtrar() {
      let lista = [...this.produtos];

      if (this.filtroNome) {
        lista = lista.filter(p =>
          p.nome?.toLowerCase().includes(this.filtroNome.toLowerCase())
        );
      }

      if (this.filtroCodigo) {
        lista = lista.filter(p =>
          p.codigoBarras?.includes(this.filtroCodigo)
        );
      }

      if (this.ordemPreco === 'asc') {
        lista.sort((a, b) => a.preco - b.preco);
      } else if (this.ordemPreco === 'desc') {
        lista.sort((a, b) => b.preco - a.preco);
      }

      this.produtosFiltrados = lista;
      this.paginaAtual = 1; // Reinicia para a primeira página ao aplicar novo filtro
    },

    // Abre o modal e define o ID do produto a ser excluído
    abrirModalExclusao(id) {
      this.idParaExcluir = id;
      this.mostrarModal = true;
    },

    // Fecha o modal sem excluir
    cancelarExclusao() {
      this.idParaExcluir = null;
      this.mostrarModal = false;
    },

    // Confirma a exclusão emitindo um evento para o componente pai
    confirmarExclusao() {
      if (this.idParaExcluir !== null) {
        this.$emit('excluir', this.idParaExcluir);
        this.mostrarModal = false;
        this.idParaExcluir = null;
      }
    }
  }
};
</script>

<style scoped>
.input {
  @apply border p-2 rounded w-full;
}
</style>
