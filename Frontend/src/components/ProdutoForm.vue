<template>
  <div>
    <!-- Toast fixo no canto superior direito -->
    <div
      v-if="mensagemSucesso"
      class="fixed top-6 right-6 bg-green-500 text-white px-4 py-3 rounded-lg shadow-lg flex items-center gap-3 z-50 animate-fade"
    >
      <span>{{ mensagemSucesso }}</span>
      <button @click="mensagemSucesso = ''" class="text-white hover:text-gray-200 font-bold text-lg leading-none">
        ✖
      </button>
    </div>

    <!-- Botão para abrir o modal de novo produto -->
    <button @click="abrirModal" class="bg-orange-500 hover:bg-orange-600 text-white px-6 py-2 rounded shadow">
      Novo Produto
    </button>

    <!-- Modal para criação/edição de produto -->
    <div v-if="aberto" class="fixed inset-0 z-50 bg-black bg-opacity-40 flex items-center justify-center p-4">
      <div class="bg-white rounded-xl shadow-xl p-6 w-full max-w-lg relative">
        <!-- Botão fechar -->
        <button @click="fecharModal" class="absolute top-3 right-4 text-gray-500 hover:text-gray-800 text-xl">
          ✖
        </button>

        <!-- Título -->
        <h2 class="text-2xl font-bold mb-6 text-center text-orange-500">
          {{ modoEdicao ? 'Editar Produto' : 'Novo Produto' }}
        </h2>

        <!-- Formulário -->
        <form @submit.prevent="salvarProduto" class="grid gap-5">
          <div>
            <label class="block text-sm font-medium mb-1">Nome</label>
            <input v-model="form.nome" class="input" :class="{ 'border-red-500': erros.nome }" />
            <p v-if="erros.nome" class="text-red-500 text-sm mt-1">{{ erros.nome }}</p>
          </div>

          <div>
            <label class="block text-sm font-medium mb-1">Preço</label>
            <input
              class="input"
              :class="{ 'border-red-500': erros.preco }"
              :value="precoBruto"
              @input="handlePrecoInput"
              @blur="formatarPreco"
              @focus="removerFormatacao"
            />
            <p v-if="erros.preco" class="text-red-500 text-sm mt-1">{{ erros.preco }}</p>
          </div>

          <div>
            <label class="block text-sm font-medium mb-1">Código de Barras</label>
            <input v-model="form.codigoBarras" class="input" :class="{ 'border-red-500': erros.codigoBarras }" />
            <p v-if="erros.codigoBarras" class="text-red-500 text-sm mt-1">{{ erros.codigoBarras }}</p>
          </div>

          <div>
            <label class="block text-sm font-medium mb-1">Imagem</label>
            <input type="file" @change="carregarImagem" class="input" accept="image/*" />
            <p v-if="erros.imagem" class="text-red-500 text-sm mt-1">{{ erros.imagem }}</p>
          </div>

          <button type="submit" class="bg-orange-500 hover:bg-orange-600 text-white px-6 py-3 rounded-lg shadow">
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
      precoBruto: '',
      mensagemSucesso: '',
      form: {
        nome: '',
        preco: 0,
        codigoBarras: '',
        imagemBase64: '',
        id: null
      },
      erros: {
        nome: '',
        preco: '',
        codigoBarras: '',
        imagem: ''
      }
    };
  },
  computed: {
    precoDisplay() {
      return this.precoBruto;
    }
  },
  watch: {
    produtoEdicao: {
      immediate: true,
      handler(produto) {
        if (produto) {
          this.form = { ...produto };
          this.precoBruto = produto.preco.toString().replace('.', ',');
          this.modoEdicao = true;
          this.aberto = true;
        }
      }
    }
  },
  methods: {
    abrirModal() {
      this.form = { nome: '', preco: 0, codigoBarras: '', imagemBase64: '', id: null };
      this.precoBruto = '';
      this.modoEdicao = false;
      this.aberto = true;
      this.limparErros();
    },
    fecharModal() {
      this.aberto = false;
    },
    limparErros() {
      this.erros = {
        nome: '',
        preco: '',
        codigoBarras: '',
        imagem: ''
      };
    },
    handlePrecoInput(event) {
      let valor = event.target.value;

      // Remove tudo que não é número ou vírgula
      valor = valor.replace(/[^\d,]/g, '');

      // Garante no máximo uma vírgula
      const partes = valor.split(',');
      if (partes.length > 2) {
        valor = partes[0] + ',' + partes[1];
      }

      this.precoBruto = valor;
    },

    formatarPreco() {
      if (!this.precoBruto) {
        this.form.preco = 0;
        this.precoBruto = '';
        return;
      }

      const limpo = this.precoBruto.replace(/[^\d,]/g, '').replace(',', '.');
      const valor = parseFloat(limpo);

      if (!isNaN(valor) && valor > 0) {
        this.form.preco = valor;
        this.precoBruto = valor.toLocaleString('pt-BR', {
          style: 'currency',
          currency: 'BRL'
        });
      } else {
        this.form.preco = 0;
        this.precoBruto = '';
      }
    },
    removerFormatacao() {
      if (this.form.preco) {
        this.precoBruto = String(this.form.preco).replace('.', ',');
      }
    },
    async carregarImagem(event) {
      const file = event.target.files[0];
      this.erros.imagem = '';

      if (!file) return;

      const tiposAceitos = ['image/jpeg', 'image/png', 'image/jpg'];
      if (!tiposAceitos.includes(file.type)) {
        this.erros.imagem = 'Apenas imagens JPEG ou PNG são permitidas.';
        return;
      }

      const imagemCompactada = await this.comprimirImagem(file, 800, 0.7);
      this.form.imagemBase64 = imagemCompactada;
    },
    async comprimirImagem(file, maxLargura, qualidade = 0.7) {
      return new Promise((resolve, reject) => {
        const reader = new FileReader();

        reader.onload = (e) => {
          const img = new Image();
          img.onload = () => {
            const canvas = document.createElement('canvas');
            const ratio = img.width / img.height;

            let largura = img.width;
            let altura = img.height;

            if (largura > maxLargura) {
              largura = maxLargura;
              altura = Math.round(largura / ratio);
            }

            canvas.width = largura;
            canvas.height = altura;

            const ctx = canvas.getContext('2d');
            ctx.fillStyle = '#ffffff';
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            ctx.drawImage(img, 0, 0, largura, altura);

            const base64 = canvas.toDataURL('image/jpeg', qualidade);
            resolve(base64.split(',')[1]);
          };

          img.onerror = reject;
          img.src = e.target.result;
        };

        reader.onerror = reject;
        reader.readAsDataURL(file);
      });
    },
    async salvarProduto() {
      this.limparErros();

      if (!this.form.nome.trim()) this.erros.nome = 'O nome é obrigatório.';
      if (!this.form.preco || this.form.preco <= 0) this.erros.preco = 'Informe um preço válido.';
      if (!this.form.codigoBarras.trim()) this.erros.codigoBarras = 'O código de barras é obrigatório.';
      if (!this.modoEdicao && !this.form.imagemBase64) this.erros.imagem = 'Selecione uma imagem.';

      if (Object.values(this.erros).some(msg => msg)) return;

      const url = this.modoEdicao
        ? `http://localhost:5091/api/produtos/${this.form.id}`
        : 'http://localhost:5091/api/produtos';
      const method = this.modoEdicao ? 'PUT' : 'POST';

      const payload = { ...this.form };
      if (!this.modoEdicao) delete payload.id;

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const error = await response.text();
        alert("Erro ao salvar produto: " + error);
        return;
      }

      this.fecharModal();
      this.mensagemSucesso = this.modoEdicao
        ? 'Produto atualizado com sucesso!'
        : 'Produto cadastrado com sucesso!';

      setTimeout(() => {
        this.mensagemSucesso = '';
      }, 5000); // 5 segundos

      this.$emit('produto-criado');
    }
  }
};
</script>

<style scoped>
.input {
  @apply border border-gray-300 p-2 rounded w-full focus:outline-none focus:ring-2 focus:ring-indigo-400;
}

@keyframes fade {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade {
  animation: fade 0.3s ease-out;
}
</style>
