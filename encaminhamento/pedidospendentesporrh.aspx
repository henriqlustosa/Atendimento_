<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="pedidospendentesporrh.aspx.cs" Inherits="encaminhamento_pedidospendentesporrh"
    Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <!-- DataTables CSS (CDN) -->
  <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />

  <style>
    .page-title { font-weight:600; margin:10px 0 18px; }
    .card{ background:#fff; border:1px solid #e5e7eb; border-radius:10px; box-shadow:0 1px 2px rgba(0,0,0,.04); }
    .card-body{ padding:18px; }
    .toolbar{ display:flex; gap:12px; align-items:center; justify-content:space-between; flex-wrap:wrap; }
    .toolbar .form-inline{ display:flex; align-items:center; gap:10px; }
    .form-control{ height:36px; }
    .btn{ height:36px; line-height:1; }
    table.dataTable thead th{ background:#334155; color:#fff; font-weight:600; border-color:#334155; }
    table.dataTable tbody td{ vertical-align:top; }
    .actions-col{ white-space:nowrap; width:120px; text-align:center; }
    .btn-icon{ width:32px; height:32px; padding:0; display:inline-flex; align-items:center; justify-content:center; }
    .btn-icon i{ font-size:14px; }
    .dataTables_length label{ font-weight:500; }
    .dataTables_filter input{ width:220px; }
    @media (max-width:576px){ .dataTables_filter input{ width:160px; } }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
  <h3 class="page-title">
    <asp:Label ID="lbTitulo" runat="server" Text="Solicitações de Exames Cadastrados"></asp:Label>
  </h3>

  <div class="card">
    <div class="card-body">
      <div class="toolbar">
        <div class="form-inline">
          <label for="<%= txbProntuario.ClientID %>" class="mb-0">Prontuário:</label>

          <asp:TextBox ID="txbProntuario" runat="server"
                       CssClass="form-control"
                       placeholder="Digite o RH"
                       MaxLength="9"
                       aria-label="Prontuário (RH)"></asp:TextBox>

          <asp:RequiredFieldValidator ID="rfvProntuario" runat="server"
              ControlToValidate="txbProntuario" ForeColor="red" Display="Dynamic"
              ErrorMessage="Obrigatório" ValidationGroup="pesquisa" />

          <asp:Button ID="btnPesquisar" runat="server" CssClass="btn btn-primary"
              Text="Pesquisar" ValidationGroup="pesquisa" OnClick="btnPesquisar_Click" />
        </div>
      </div>

      <div class="table-responsive" style="margin-top:14px;">
        <asp:GridView ID="GridView1" runat="server"
          AutoGenerateColumns="False"
          DataKeyNames="cod_pedido"
          OnRowCommand="grdMain_RowCommand"
          OnPreRender="GridView1_PreRender"
          CssClass="table table-striped table-bordered"
          GridLines="Horizontal" BorderColor="#e0ddd1" Width="100%"
          ShowHeaderWhenEmpty="true">
          <RowStyle BackColor="#f7f6f3" ForeColor="#333333" />
          <HeaderStyle CssClass="dt-header" />
          <Columns>
            <asp:BoundField DataField="cod_pedido" HeaderText="Código do Pedido" SortExpression="cod_pedido" />
            <asp:BoundField DataField="prontuario" HeaderText="Prontuário" SortExpression="prontuario" />
            <asp:BoundField DataField="nome_paciente" HeaderText="Paciente" SortExpression="nome_paciente" />
            <asp:BoundField DataField="data_pedido" HeaderText="Data do Pedido" SortExpression="data_pedido" />
            <asp:BoundField DataField="data_cadastro" HeaderText="Data de Cadastro" SortExpression="data_cadastro" />
            <asp:BoundField DataField="descricao_espec" HeaderText="Especialidade" SortExpression="descricao_espec" />
            <asp:BoundField DataField="exames_solicitados" HeaderText="Exames Solicitados" SortExpression="exames_solicitados" />
            <asp:BoundField DataField="outras_informacoes" HeaderText="Outras Informações" SortExpression="outras_informacoes" />
            <asp:BoundField DataField="solicitante" HeaderText="Solicitante" SortExpression="solicitante" />
            <asp:BoundField DataField="usuario" HeaderText="Usuário" SortExpression="usuario" />

            <asp:TemplateField HeaderText=" " ItemStyle-CssClass="actions-col" HeaderStyle-CssClass="sorting_disabled">
              <ItemTemplate>
                <asp:LinkButton ID="gvlnkFile"
                  CssClass="btn btn-success btn-icon" runat="server"
                  OnClientClick='return openArquivoModal("<%# Eval("cod_pedido") %>");'
                  ToolTip="Arquivar" CausesValidation="false">
                  <i class="fa fa-file"></i>
                </asp:LinkButton>

                <asp:LinkButton ID="gvlnkEdit" CommandName="editRecord" CommandArgument='<%#((GridViewRow)Container).RowIndex%>'
                  CssClass="btn btn-info btn-icon" runat="server" ToolTip="Editar" CausesValidation="false">
                  <i class="fa fa-pen"></i>
                </asp:LinkButton>

                <asp:LinkButton ID="gvlnkDelete" CommandName="deleteRecord" CommandArgument='<%#((GridViewRow)Container).RowIndex%>'
                  CssClass="btn btn-danger btn-icon" runat="server" OnClientClick="return confirmation();"
                  ToolTip="Excluir" CausesValidation="false">
                  <i class="fa fa-trash"></i>
                </asp:LinkButton>
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
        </asp:GridView>

        <!-- Hidden para levar o ID ao servidor -->
        <asp:HiddenField ID="hfPedidoId" runat="server" />

        <!-- MODAL Arquivamento -->
        <div class="modal fade" id="modalArquivo" tabindex="-1" role="dialog" aria-labelledby="lblModalArquivo" aria-hidden="true">
          <div class="modal-dialog modal-md" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="lblModalArquivo">Arquivar Pedido</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>

              <div class="modal-body">
                <div class="form-group">
                  <label>Retirado por:</label>
                  <asp:TextBox ID="txtRetiradoPor" runat="server" CssClass="form-control" />
                  <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRetiradoPor"
                      Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
                </div>

                <div class="form-group">
                  <label>RG ou CPF:</label>
                  <asp:TextBox ID="txtRgCpf" runat="server" CssClass="form-control" />
                  <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRgCpf"
                      Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
                </div>

                <div class="form-row">
                  <div class="form-group col-sm-6">
                    <label>Data:</label>
                    <asp:TextBox ID="txtData" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtData"
                        Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
                  </div>
                  <div class="form-group col-sm-6">
                    <label>Hora:</label>
                    <asp:TextBox ID="txtHora" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtHora"
                        Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
                  </div>
                </div>
              </div>

              <div class="modal-footer">
                <button type="button" class="btn btn-light" data-dismiss="modal">Cancelar</button>
                <asp:Button ID="btnConfirmarArquivo" runat="server" CssClass="btn btn-success"
                    Text="Salvar e Arquivar" OnClick="btnConfirmarArquivo_Click" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- jQuery fallback -->
  <script>
      if (typeof window.jQuery === 'undefined') {
          document.write('<script src="https://code.jquery.com/jquery-3.7.1.min.js" crossorigin="anonymous"><\/script>');
      }
  </script>

  <!-- DataTables (CDN) -->
  <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>

  <script type="text/javascript">
      // Permitir somente números no RH
      (function () {
          var tb = document.getElementById('<%= txbProntuario.ClientID %>');
          if (tb) {
              tb.addEventListener('keypress', function (e) {
                  var ch = e.which || e.keyCode;
                  if (ch === 8 || ch === 9 || ch === 13) return; // backspace, tab, enter
                  if (ch < 48 || ch > 57) e.preventDefault();
              });
          }
      })();

      // Abre modal e injeta o ID do pedido
      function openArquivoModal(id) {
          $('#<%= hfPedidoId.ClientID %>').val(id);
      var hoje = new Date();
      var d = hoje.toISOString().slice(0, 10);  // yyyy-mm-dd
      var h = hoje.toTimeString().slice(0, 5);  // HH:mm
      if (!$('#<%= txtData.ClientID %>').val()) $('#<%= txtData.ClientID %>').val(d);
      if (!$('#<%= txtHora.ClientID %>').val()) $('#<%= txtHora.ClientID %>').val(h);
      $('#modalArquivo').modal('show');
      return false; // evita postback
    }

    (function () {
      function ensureThead($tbl) {
        if ($tbl.find('thead').length === 0) {
          var $first = $tbl.find('tr:first');
          if ($first.length) $tbl.prepend($('<thead/>').append($first));
        }
      }

      function initDT() {
        var $tbl = $('#<%= GridView1.ClientID %>');
              if (!$tbl.length || !window.jQuery || !$.fn || !$.fn.DataTable) return;

              // só inicializa se houver linhas
              if ($tbl.find('tbody tr').length === 0) return;

              ensureThead($tbl);

              if ($.fn.DataTable.isDataTable($tbl[0])) {
                  $tbl.DataTable().clear().destroy();
              }

              $tbl.DataTable({
                  paging: true,
                  pageLength: 10,
                  lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
                  ordering: true,
                  stateSave: true,
                  responsive: true,
                  autoWidth: false,
                  dom: '<"top d-flex justify-content-between align-items-center"lfr>t<"bottom d-flex justify-content-between align-items-center"ip>',
                  columnDefs: [
                      { targets: -1, orderable: false, searchable: false },
                      {
                          targets: [3, 4],
                          render: function (data) {
                              if (!data) return "";
                              var s = String(data);
                              if (s.indexOf(' ') > -1) return s.split(' ')[0];
                              if (s.indexOf('T') > -1) return s.split('T')[0].split('-').reverse().join('/');
                              return s;
                          }
                      }
                  ],
                  language: {
                      processing: "Processando...",
                      search: "Buscar:",
                      lengthMenu: "Mostrar _MENU_ registros por página",
                      info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                      infoEmpty: "Mostrando 0 a 0 de 0 registros",
                      infoFiltered: "(filtrado de _MAX_ no total)",
                      loadingRecords: "Carregando...",
                      zeroRecords: "Nenhum registro encontrado",
                      emptyTable: "Nenhum dado disponível",
                      paginate: { first: "Primeiro", previous: "Anterior", next: "Próximo", last: "Último" }
                  }
              });
          }

          // 1) ao carregar
          $(document).ready(initDT);

          // 2) após postback parcial (UpdatePanel)
          if (window.Sys && Sys.Application) {
              Sys.Application.add_load(function () { setTimeout(initDT, 0); });
          }

          // Confirmações
          window.file = function () { return confirm("Você realmente quer arquivar o registro?"); };
          window.confirmation = function () { return confirm("Você realmente quer deletar o registro?"); };
      })();
  </script>
</asp:Content>
