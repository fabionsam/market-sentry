# 🛡️ Market Sentry

Aplicação para monitoramento de ativos da B3. Onde você pode configurar preços de compra/venda e receber alertas por e-mail. 

O projeto utiliza uma arquitetura **"All-in-One"**, onde uma API .NET roda em background como um Worker Service e, simultaneamente, serve o Frontend em Angular, permitindo que a aplicação final seja distribuída como um **único executável (.exe)** sem necessidade de instalação complexa.

---

## 🚀 Tecnologias Utilizadas

### Backend (.NET)
* **C# .NET 10**: Web API + Worker Service (Background Tasks).
* **Entity Framework Core**: ORM com banco de dados **SQLite** (portátil).
* **Refit**: Cliente HTTP tipado para consumo de APIs externas.
* **MailKit**: Envio de e-mails via SMTP com suporte a SSL/TLS.
* **Polly**: Resiliência e retentativas em chamadas HTTP.

### Frontend (Angular)
* **Angular 20+**: Standalone Components, Signals e Control Flow Syntax (`@if`, `@for`).
* **Angular Material**: UI Kit profissional (Dialogs, Expansion Panels, Inputs).
* **Chart.js (ng2-charts)**: Gráficos interativos com histórico de 24h.
* **RxJS**: Gerenciamento de estado reativo e Polling inteligente.

---

## ✨ Funcionalidades

* ✅ **Monitoramento Contínuo**: Verifica cotações a cada 60 segundos automaticamente.
* ✅ **Multi-API**: Suporte configurável para múltiplos provedores de dados (ex: Brapi, HgBrasil).
* ✅ **Alertas por E-mail**: Dispara notificações com template HTML personalizado quando o preço atinge o alvo.
* ✅ **Dashboard Interativo**:
    * Card expansível com gráfico de histórico.
    * Atualização em tempo real sem recarregar a página.
    * Feedback visual de status (Loading, Toasts de erro/sucesso).
* ✅ **Configuração Fácil**: Interface visual para configurar SMTP e adicionar novos ativos.
* ✅ **Instalação Simplificada**: Roda localmente na máquina do cliente sem depender de servidor web externo.

---

## 🛠️ Como Rodar (Desenvolvimento)

Pré-requisitos: **.NET SDK** e **Node.js** instalados.

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/fabionsam/market-sentry.git
    cd MarketSentry
    ```

2.  **Frontend (Angular):**
    ```bash
    cd MarketSentry.UI
    npm install
    npm start
    ```
    *O Angular rodará em `http://localhost:4200`.*

3.  **Backend (.NET):**
    Abra o projeto no Visual Studio ou VS Code e execute o projeto **MarketSentry.API**.
    *A API rodará (geralmente) em `http://localhost:5000` ou porta similar.*

    > **Nota:** Certifique-se de configurar a API para rodar simultaneamente ao Angular ou utilize o proxy configurado no projeto UI.

---

## 📦 Como Buildar (Produção / Executável)

Existem duas formas de gerar o executável final para o cliente:

### Opção 1: Via Linha de Comando (Manual)
1.  Na raiz da solução, execute:
    ```bash
    dotnet publish MarketSentry.API -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./BuildFinal
    ```
2.  Compile o Angular (`ng build`) e copie o conteúdo da pasta `dist` para dentro de `BuildFinal/wwwroot`.

### Opção 2: Via Script Automático (Windows) ✨
Para facilitar, incluímos um script que faz todo o trabalho sujo (compila o Angular, o .NET e organiza as pastas).

1.  Localize o arquivo **`GerarExecutavel.bat`** (ou `GerarInstalador.bat`) na raiz do projeto.
2.  Dê **dois cliques** nele.
3.  Aguarde o processo terminar.
4.  Uma pasta chamada `Instalador_MarketSentry` será criada contendo o `.exe` pronto para uso.
5.  Nessa pasta existirá um arquivo chamado `Iniciar_Sistema` para facilitar a inicialização do sistema.

---

## 📧 Testando o Envio de E-mail

Para testar os alertas sem utilizar sua conta de e-mail real, recomendo o uso do **Ethereal Email** ou **Papercut SMTP**.

### Usando o Ethereal (Online)
1.  Acesse [https://ethereal.email/](https://ethereal.email/).
2.  Clique em "Create Ethereal Account".
3.  No Market Sentry, vá em ⚙️ **Configurações** e preencha:
    * **Host:** `smtp.ethereal.email`
    * **Porta:** `587`
    * **Usuário/Senha:** (Os fornecidos pelo site)
    * **SSL:** Marcado (Sim)
4.  Quando o sistema disparar um alerta, verifique a caixa de entrada no site do Ethereal.

---

## 🔮 Possíveis Melhorias (Roadmap)

* [ ] **Autenticação:** Adicionar login para proteger as configurações.
* [ ] **Edição de ativos configurados:** Adicionar opção de editar ativos configurados.
* [ ] **Integração com Telegram/WhatsApp:** Enviar alertas via Bot além do e-mail.
* [ ] **Dockerização:** Criar um `Dockerfile` para rodar a aplicação em containers.
* [ ] **Gerenciamento de Logs:** Interface para visualizar os logs de erro/execução do Worker.
* [ ] **Minimizar para a Bandeja (System Tray):** Rodar a aplicação em segundo plano (ícone perto do relógio) em vez de uma janela de console, evitando fechamentos acidentais.

---

Desenvolvido com ☕ e <br/>
[![My Skills](https://skillicons.dev/icons?i=ts,cs,angular,html,css,sqlite,nodejs)](https://skillicons.dev)
