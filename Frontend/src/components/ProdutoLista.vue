<template>
  <div>
    <!-- Filtros e ordenação -->
    <div class="flex gap-3 mb-4">
      <input
        v-model="filtroNome"
        placeholder="Filtrar por nome"
        class="border p-2 rounded flex-1"
        @input="filtrar"
      />
      <input
        v-model="filtroCodigo"
        placeholder="Código de Barras"
        class="border p-2 rounded"
        @input="filtrar"
      />
      <select v-model="ordemPreco" class="border p-2 rounded" @change="filtrar">
        <option value="">Ordenar preço</option>
        <option value="asc">Mais barato</option>
        <option value="desc">Mais caro</option>
      </select>
    </div>

    <!-- Lista -->
    <div v-for="produto in produtosFiltrados" :key="produto.id" class="border-b py-4">
      <div class="flex items-center gap-4">
        <img
          v-if="produto.imagemBase64"
          :src="'data:image/png;base64,' + produto.imagemBase64"
          alt="Imagem"
          class="w-20 h-20 object-cover rounded border"
        />
        <div class="flex-1">
          <h2 class="font-bold text-lg">{{ produto.nome }}</h2>
          <p>R$ {{ produto.preco.toFixed(2) }}</p>
          <p class="text-sm text-gray-600">Código: {{ produto.codigoBarras }}</p>
        </div>
        <button
          @click="$emit('excluir', produto.id)"
          class="bg-red-600 text-white px-3 py-1 rounded"
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
select,
input {
  min-width: 140px;
}
</style>
