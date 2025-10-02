<%@ Page Title="Cadastro de Usuário" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="CadastroDeUsuario.aspx.cs" Inherits="restrito_CadastroDeUsuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <!-- DataTables + Bootstrap 5 (CSS) -->
  <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/dataTables.bootstrap5.min.css" />

  <!-- DataTables (JS) – usamos defer para carregar depois do HTML;
       jQuery e Bootstrap 5 já vêm da MasterPage -->
  <script defer src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
  <script defer src="https://cdn.datatables.net/1.13.8/js/dataTables.bootstrap5.min.js"></script>

  <style>
      /* Mantém os checkboxes em linha */
.cbl-perfis input[type="checkbox"] {
  margin-right: 4px; /* espaço entre o checkbox e o texto */
}

.cbl-perfis label {
  display: inline-block;
  margin-right: 15px; /* espaço entre cada item */
  white-space: nowrap; /* evita quebra de linha dentro do item */
}

/* Garante que todos fiquem na linha abaixo do label */
.cbl-perfis {
  display: flex;
  flex-wrap: wrap; /* se a tela for pequena, quebra automaticamente */
  gap: 10px; /* espaço entre itens */
  margin-top: .5rem; /* espaço entre o label e a lista */
}

    /* Espaçamento mais justo entre os itens do CheckBoxList */
    .cbl-perfis span{
      display:inline-flex; align-items:center; gap:.35rem;
      margin-right:.85rem; margin-bottom:.25rem;
      white-space:nowrap;
    }
    .cbl-perfis input{ margin:0; }

    .form-label{ margin-bottom:.35rem; }
    .row + .row{ margin-top:.65rem; }

    /* Aparência das tabelas (GridView) no estilo Bootstrap */
    .table { --bs-table-bg:#fff; }
    .dataTables_filter input { margin-left:.5rem; }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
  <div class="container py-2">
    <h2 class="text-center mb-3">Cadastro de Usuário</h2>

    <!-- Linha de campos principais -->
    <div class="row g-3 align-items-end">
      <div class="col-12 col-sm-2">
        <label for="<%= txtLogin.ClientID %>" class="form-label">Login de Rede</label>
        <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" />
      </div>

      <div class="col-12 col-sm-auto">
        <asp:Button ID="btnBuscarAD" runat="server" Text="Pesquisar" CssClass="btn btn-primary"
            OnClick="btnBuscarAD_Click" />
      </div>

      <div class="col-12 col-md-5">
        <label for="<%= txtNome.ClientID %>" class="form-label">Nome</label>
        <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
      </div>

      <div class="col-12 col-md-4">
        <label for="<%= txtEmail.ClientID %>" class="form-label">Email</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
      </div>
    </div>

    <!-- Perfis -->
    <div class="mt-3">
      <label class="form-label">Perfis de Acesso: </label>
      <asp:CheckBoxList ID="cblPerfis" runat="server"
          RepeatDirection="Horizontal" RepeatLayout="Flow"
          CssClass="cbl-perfis" />
    </div>

    <!-- Botão salvar -->
    <div class="text-center mt-3">
      <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn btn-success"
          OnClick="btnSalvar_Click" ValidationGroup="Cadastro" />
    </div>

    <div class="mt-2">
      <asp:Label ID="lblResultado" runat="server" ForeColor="Green" />
    </div>

    <!-- Usuários Ativos -->
    <div class="mt-4">
      <h3 class="h4">Usuários Cadastrados</h3>
      <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False"
          DataKeyNames="Id" OnRowCommand="gvUsuarios_RowCommand"
          CssClass="table table-striped table-hover align-middle w-100">
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" />
          <asp:BoundField DataField="LoginRede" HeaderText="Login" />
          <asp:BoundField DataField="NomeCompleto" HeaderText="Nome" />
          <asp:BoundField DataField="Email" HeaderText="Email" />
          <asp:TemplateField HeaderText="Ações">
            <ItemTemplate>
              <asp:Button ID="btnExcluir" runat="server" CssClass="btn btn-outline-danger btn-sm"
                  CommandName="Inativar" CommandArgument='<%# Eval("Id") %>' Text="Inativar"
                  OnClientClick="return confirm('Tem certeza que deseja inativar este usuário?');" />
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
      </asp:GridView>
    </div>

    <!-- Usuários Inativos -->
    <div class="mt-4">
      <h3 class="h4">Usuários Inativos</h3>
      <asp:GridView ID="gvUsuariosInativos" runat="server" AutoGenerateColumns="False"
          DataKeyNames="Id" OnRowCommand="gvUsuariosInativos_RowCommand"
          CssClass="table table-striped table-hover align-middle w-100">
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" />
          <asp:BoundField DataField="LoginRede" HeaderText="Login" />
          <asp:BoundField DataField="NomeCompleto" HeaderText="Nome" />
          <asp:BoundField DataField="Email" HeaderText="Email" />
          <asp:TemplateField HeaderText="Ações">
            <ItemTemplate>
              <asp:Button ID="btnAtivar" runat="server" CssClass="btn btn-outline-secondary btn-sm"
                  CommandName="Ativar" CommandArgument='<%# Eval("Id") %>' Text="Ativar"
                  OnClientClick="return confirm('Tem certeza que deseja ativar este usuário?');" />
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
      </asp:GridView>
    </div>
  </div>

  <!-- DataTables: PT-BR + THEAD gerado para GridView -->
  <script type="text/javascript">
    (function () {
      // Espera jQuery e DataTables estarem disponíveis
      function ready(fn) {
        if (document.readyState !== 'loading') fn();
        else document.addEventListener('DOMContentLoaded', fn);
      }

      ready(function () {
        if (!window.jQuery || !$.fn || !$.fn.dataTable) return;

        // Idioma padrão pt-BR
        $.extend(true, $.fn.dataTable.defaults, {
          language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/pt-BR.json" },
          pagingType: "simple_numbers",
          pageLength: 10
        });

        function wireGridView(id) {
          var $tbl = $('#' + id);
          // transforma a primeira linha em THEAD (padrão do GridView WebForms)
          var $firstRow = $tbl.find('tbody tr:first');
          if ($firstRow.length && $tbl.find('thead').length === 0) {
            $tbl.prepend($("<thead></thead>").append($firstRow));
          }
          // inicializa DataTable (tema Bootstrap 5 por causa do JS/CSS importados)
          $tbl.DataTable();
        }

        wireGridView('<%= gvUsuarios.ClientID %>');
        wireGridView('<%= gvUsuariosInativos.ClientID %>');
      });
      })();
  </script>
</asp:Content>
