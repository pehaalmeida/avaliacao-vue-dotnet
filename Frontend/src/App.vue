<template>
  <div class="min-h-screen bg-light text-gray-900">
    <div class="max-w-5xl mx-auto p-6">
      <h1 class="text-3xl font-bold text-primary mb-6 flex items-center gap-2">
        🛒 Gerenciador de Produtos
      </h1>

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
    </div>
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
