<template>
  <div>
    <!-- AVISO fixo no canto superior direito -->
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

    <!-- Modal -->
    <div v-if="aberto" class="fixed inset-0 z-50 bg-black bg-opacity-40 flex items-center justify-center p-4">
      <div class="bg-white rounded-xl shadow-xl p-6 w-full max-w-lg relative">
        <!-- Fechar -->
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
              v-model="form.preco"
              @keypress="permitirSomenteNumerosVirgulaPonto"
              class="input"
              placeholder="Ex: 19,99"
              :class="{ 'border-red-500': erros.preco }"
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
    // Produto recebido para edição
    produtoEdicao: Object
  },
  data() {
    // Estado do modal e formulário
    return {
      aberto: false,             
      modoEdicao: false,         
      mensagemSucesso: '',       
      form: {
        nome: '',                
        preco: '',               
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
  watch: {
    // Detecta se foi passado um produto para edição
    produtoEdicao: {
      immediate: true,
      handler(produto) {
        if (produto) {
          this.form = { ...produto }; // Preenche o formulário
          this.form.preco = produto.preco.toString().replace('.', ','); // Formata o preço
          this.modoEdicao = true;
          this.aberto = true;
        }
      }
    }
  },
  methods: {
    // Abre o modal e reseta o formulário
    abrirModal() {
      this.form = {
        nome: '',
        preco: '',
        codigoBarras: '',
        imagemBase64: '',
        id: null
      };
      this.modoEdicao = false;
      this.aberto = true;
      this.limparErros();
    },

    // Fecha o modal
    fecharModal() {
      this.aberto = false;
    },

    // Limpa mensagens de erro do formulário
    limparErros() {
      this.erros = {
        nome: '',
        preco: '',
        codigoBarras: '',
        imagem: ''
      };
    },

    // Bloqueia caracteres inválidos no campo de preço (permite apenas números, vírgula e ponto)
    permitirSomenteNumerosVirgulaPonto(event) {
      const char = String.fromCharCode(event.keyCode || event.which);
      const regex = /[0-9.,]/; // Aceita apenas números, ponto e vírgula
      if (!regex.test(char)) {
        event.preventDefault(); // Bloqueia digitação
      }
    },

    // Quando o usuário escolhe uma imagem, ela é compactada e convertida em base64
    async carregarImagem(event) {
      const file = event.target.files[0];
      this.erros.imagem = '';
      if (!file) return;

      const tiposAceitos = ['image/jpeg', 'image/png', 'image/jpg'];
      if (!tiposAceitos.includes(file.type)) {
        this.erros.imagem = 'Apenas imagens JPEG ou PNG são permitidas.';
        return;
      }

      // Chama método que comprime e converte a imagem para base64
      const imagemCompactada = await this.comprimirImagem(file, 800, 0.7);
      this.form.imagemBase64 = imagemCompactada;
    },

    // Compacta e converte imagem em base64 com largura máxima e qualidade ajustável
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

            // Redimensiona a imagem se for maior que o limite
            if (largura > maxLargura) {
              largura = maxLargura;
              altura = Math.round(largura / ratio);
            }

            // Desenha a imagem no canvas
            canvas.width = largura;
            canvas.height = altura;

            const ctx = canvas.getContext('2d');
            ctx.fillStyle = '#ffffff';
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            ctx.drawImage(img, 0, 0, largura, altura);

            // Converte imagem para base64 e remove o prefixo
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

    // Valida os campos e envia o produto para a API (criação ou atualização)
    async salvarProduto() {
      this.limparErros();

      if (!this.form.nome.trim()) this.erros.nome = 'O nome é obrigatório.';
      if (!this.form.preco || parseFloat(this.form.preco.replace(',', '.')) <= 0)
        this.erros.preco = 'Informe um preço válido.';
      if (!this.form.codigoBarras.trim()) this.erros.codigoBarras = 'O código de barras é obrigatório.';
      if (!this.modoEdicao && !this.form.imagemBase64) this.erros.imagem = 'Selecione uma imagem.';

      // Se houver erros, não envia
      if (Object.values(this.erros).some(msg => msg)) return;

      // Monta URL e método da requisição
      const url = this.modoEdicao
        ? `http://localhost:5091/api/produtos/${this.form.id}`
        : 'http://localhost:5091/api/produtos';
      const method = this.modoEdicao ? 'PUT' : 'POST';

      // Prepara o payload
      const payload = { ...this.form };
      payload.preco = parseFloat(this.form.preco.replace(',', '.')); // Converte preço para float

      if (!this.modoEdicao) delete payload.id;

      // Envia para API
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

      // Fecha modal e mostra AVISO de sucesso
      this.fecharModal();
      this.mensagemSucesso = this.modoEdicao
        ? 'Produto atualizado com sucesso!'
        : 'Produto cadastrado com sucesso!';

      // Esconde AVISO após 5 segundos
      setTimeout(() => {
        this.mensagemSucesso = '';
      }, 5000);

      // Notifica componente pai para recarregar lista
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
