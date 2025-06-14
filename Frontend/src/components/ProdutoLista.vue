<template>
  <div>
    <!-- Filtros -->
    <div class="flex flex-col md:flex-row gap-4 mb-6 items-end">
      <input v-model="filtroNome" placeholder="Filtrar por nome" class="input" @input="filtrar" />
      <input v-model="filtroCodigo" placeholder="Código de Barras" class="input" @input="filtrar" />
      <select v-model="ordemPreco" class="input" @change="filtrar">
        <option value="">Ordenar preço</option>
        <option value="asc">Mais barato</option>
        <option value="desc">Mais caro</option>
      </select>
    </div>

    <!-- Lista de produtos -->
    <div
      v-for="produto in produtosFiltrados"
      :key="produto.id"
      class="flex flex-col md:flex-row items-start md:items-center gap-4 border border-gray-300 rounded-lg p-4 mb-4 bg-white shadow-sm"
    >
      <img
        v-if="produto.imagemBase64"
        :src="'data:image/png;base64,' + produto.imagemBase64"
        alt="Imagem"
        class="w-full max-w-[96px] h-auto object-cover rounded border"
      />
      <div class="flex-1">
        <h2 class="font-semibold text-lg">{{ produto.nome }}</h2>
        <p class="text-gray-700">R$ {{ produto.preco.toFixed(2) }}</p>
        <p class="text-sm text-gray-500">Código: {{ produto.codigoBarras }}</p>
      </div>
      <div class="flex gap-2">
        <button
          @click="$emit('editar', produto)"
          class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded"
        >
          Editar
        </button>
        <button
          @click="$emit('excluir', produto.id)"
          class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded"
        >
          Excluir
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    produtos: Array
  },
  data() {
    return {
      filtroNome: '',
      filtroCodigo: '',
      ordemPreco: '',
      produtosFiltrados: []
    };
  },
  watch: {
    produtos: {
      immediate: true,
      handler() {
        this.filtrar();
      }
    }
  },
  methods: {
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
    }
  }
};
</script>

<style scoped>
.input {
  @apply border p-2 rounded w-full;
}
</style>
