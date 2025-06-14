<template>
  <form @submit.prevent="salvarProduto" class="grid gap-3 mb-6">
    <input
      v-model="form.nome"
      placeholder="Nome do Produto"
      class="border p-2 rounded"
      required
    />
    <input
      v-model.number="form.preco"
      type="number"
      placeholder="Preço"
      class="border p-2 rounded"
      required
    />
    <input
      v-model="form.codigoBarras"
      placeholder="Código de Barras"
      class="border p-2 rounded"
      required
    />
    <input
      type="file"
      @change="carregarImagem"
      accept="image/*"
      class="border p-2 rounded"
    />

    <button type="submit" class="bg-blue-600 text-white px-4 py-2 rounded">
      Salvar
    </button>
  </form>
</template>

<script>
export default {
  data() {
    return {
      form: {
        nome: '',
        preco: 0,
        codigoBarras: '',
        imagemBase64: ''
      }
    };
  },
  methods: {
    // Converte a imagem selecionada em base64
    carregarImagem(event) {
      const file = event.target.files[0];
      const reader = new FileReader();
      reader.onload = () => {
        this.form.imagemBase64 = reader.result.split(',')[1];
      };
      if (file) reader.readAsDataURL(file);
    },

    // Envia o produto para a API
    async salvarProduto() {
      await fetch('http://localhost:5091/api/produtos', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(this.form)
      });

      // Limpa os campos
      this.form = {
        nome: '',
        preco: 0,
        codigoBarras: '',
        imagemBase64: ''
      };

      // Emite evento para o App.vue recarregar a lista
      this.$emit('produto-criado');
    }
  }
};
</script>

<style scoped>
form input {
  width: 100%;
}
</style>
