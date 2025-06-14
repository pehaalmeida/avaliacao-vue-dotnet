<template>
  <div class="min-h-screen bg-gray-100 text-gray-900">
    <header class="bg-white shadow">
      <div class="max-w-6xl mx-auto px-4 py-6 flex items-center justify-between">
        <h1 class="text-2xl md:text-3xl font-bold text-indigo-600 flex items-center gap-2">
          🛒 Gerenciador de Produtos
        </h1>
      </div>
    </header>

    <main class="max-w-6xl mx-auto px-4 py-6">
      <!-- Formulário como modal -->
      <ProdutoForm
        :produto-edicao="produtoSelecionado"
        @produto-criado="carregarProdutos"
      />

      <hr class="my-8 border-gray-300" />

      <!-- Lista com edição -->
      <ProdutoLista
        :produtos="produtos"
        @excluir="excluirProduto"
        @editar="editarProduto"
      />
    </main>
  </div>
</template>

<script>
import ProdutoForm from './components/ProdutoForm.vue'
import ProdutoLista from './components/ProdutoLista.vue'

export default {
  components: { ProdutoForm, ProdutoLista },
  data() {
    return {
      produtos: [],
      produtoSelecionado: null
    };
  },
  methods: {
    async carregarProdutos() {
      const res = await fetch('http://localhost:5091/api/produtos');
      this.produtos = await res.json();
    },
    async excluirProduto(id) {
      await fetch(`http://localhost:5091/api/produtos/${id}`, { method: 'DELETE' });
      this.carregarProdutos();
    },
    editarProduto(produto) {
      this.produtoSelecionado = { ...produto };
    }
  },
  mounted() {
    this.carregarProdutos();
  }
};
</script>

<style>
body {
  background: #f3f4f6;
  font-family: 'Segoe UI', sans-serif;
}
</style>
