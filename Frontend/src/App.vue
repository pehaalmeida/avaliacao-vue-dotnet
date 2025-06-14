<template>
  <div class="p-6 max-w-5xl mx-auto">
    <h1 class="text-2xl font-bold mb-6">🛒 Gerenciador de Produtos</h1>

    <!-- Componente de formulário -->
    <ProdutoForm @produto-criado="carregarProdutos" />

    <hr class="my-6" />

    <!-- Componente de listagem -->
    <ProdutoLista
      :produtos="produtos"
      @excluir="excluirProduto"
    />
  </div>
</template>

<script>
import ProdutoForm from './components/ProdutoForm.vue'
import ProdutoLista from './components/ProdutoLista.vue'

export default {
  components: { ProdutoForm, ProdutoLista },
  data() {
    return {
      produtos: []
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
    }
  },
  mounted() {
    this.carregarProdutos();
  }
};
</script>
