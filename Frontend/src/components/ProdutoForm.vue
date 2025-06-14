<template>
  <div>
    <!-- Botão abrir modal -->
    <button @click="abrirModal" class="bg-primary text-white px-4 py-2 rounded hover:bg-indigo-700 mb-4">
      Novo Produto
    </button>

    <!-- Modal -->
    <div v-if="aberto" class="fixed inset-0 z-50 bg-black bg-opacity-50 flex items-center justify-center">
      <div class="bg-white rounded shadow p-6 w-full max-w-md relative">
        <button @click="fecharModal" class="absolute top-2 right-2 text-gray-600 hover:text-black">✖</button>
        <h2 class="text-xl font-semibold mb-4">{{ modoEdicao ? 'Editar' : 'Novo' }} Produto</h2>

        <form @submit.prevent="salvarProduto" class="grid gap-4">
          <div class="grid gap-1">
            <label>Nome</label>
            <input v-model="form.nome" class="input" required />
          </div>
          <div class="grid gap-1">
            <label>Preço</label>
            <input v-model.number="form.preco" type="number" class="input" required />
          </div>
          <div class="grid gap-1">
            <label>Código de Barras</label>
            <input v-model="form.codigoBarras" class="input" required />
          </div>
          <div class="grid gap-1">
            <label>Imagem</label>
            <input type="file" @change="carregarImagem" class="input" accept="image/*" />
          </div>

          <button type="submit" class="bg-primary text-white px-4 py-2 rounded hover:bg-indigo-700">
            {{ modoEdicao ? 'Atualizar' : 'Salvar' }} Produto
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    produtoEdicao: Object
  },
  data() {
    return {
      aberto: false,
      modoEdicao: false,
      form: {
        nome: '',
        preco: 0,
        codigoBarras: '',
        imagemBase64: '',
        id: null
      }
    };
  },
  watch: {
    produtoEdicao: {
      immediate: true,
      handler(produto) {
        if (produto) {
          this.form = { ...produto };
          this.modoEdicao = true;
          this.aberto = true;
        }
      }
    }
  },
  methods: {
    abrirModal() {
      this.form = { nome: '', preco: 0, codigoBarras: '', imagemBase64: '', id: null };
      this.modoEdicao = false;
      this.aberto = true;
    },
    fecharModal() {
      this.aberto = false;
    },
    carregarImagem(event) {
      const file = event.target.files[0];
      const reader = new FileReader();
      reader.onload = () => {
        this.form.imagemBase64 = reader.result.split(',')[1];
      };
      if (file) reader.readAsDataURL(file);
    },
    async salvarProduto() {
      const url = this.modoEdicao
        ? `http://localhost:5091/api/produtos/${this.form.id}`
        : 'http://localhost:5091/api/produtos';
      const method = this.modoEdicao ? 'PUT' : 'POST';

      await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(this.form)
      });

      this.fecharModal();
      this.$emit('produto-criado');
    }
  }
};
</script>

<style scoped>
.input {
  @apply border p-2 rounded w-full;
}
</style>
