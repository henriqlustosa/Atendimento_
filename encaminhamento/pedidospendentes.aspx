<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="pedidospendentes.aspx.cs" Inherits="encaminhamento_pedidospendentes"
    Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <!-- DataTables CSS (CDN) -->
  <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />

  <style>
    .page-title { font-weight:600; margin:10px 0 18px; }
    .card{ background:#fff; border:1px solid #e5e7eb; border-radius:10px; box-shadow:0 1px 2px rgba(0,0,0,.04); }
    .card-body{ padding:18px; }
    .toolbar{ display:flex; gap:12px; align-items:center; flex-wrap:wrap; }
    .toolbar .form-inline{ display:flex; align-items:center; gap:10px; flex-wrap:wrap; }
    .form-control{ height:36px; }
    .btn{ height:36px; line-height:1; }
    table.dataTable thead th{ background:#334155; color:#fff; font-weight:600; border-color:#334155; }
    table.dataTable tbody td{ vertical-align:top; }
    .actions-col{ white-space:nowrap; text-align:center; width:160px; }
    .btn-icon{ width:32px; height:32px; padding:0; display:inline-flex; align-items:center; justify-content:center; }
    .btn-icon i{ font-size:14px; }
    .dataTables_length label{ font-weight:500; }
    .dataTables_filter input{ width:220px; }
    @media (max-width:576px){ .dataTables_filter input{ width:160px; } }

    /* Garantia de z-index correto do modal com DataTables */
    .modal-backdrop{ z-index:1050 !important; }
    .modal{ z-index:1055 !important; }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

  <h3 class="page-title">
    <asp:Label ID="lbTitulo" runat="server" Text="Solicitações de Exames Cadastrados (Pendentes)"></asp:Label>
  </h3>

  <div class="card">
    <div class="card-body">

      <!-- Toolbar (filtro por RH) -->
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

            <asp:TemplateField HeaderText="Carga Geral" SortExpression="carga_geral">
              <ItemTemplate>
                <%# FormatCargaGeral(Eval("carga_geral")) %>
              </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="usuario" HeaderText="Usuário" SortExpression="usuario" />

            <asp:TemplateField HeaderText=" " ItemStyle-CssClass="actions-col" HeaderStyle-CssClass="sorting_disabled">
              <ItemTemplate>
                <!-- Arquivar via modal (novo) -->
                <a href="#" class="btn btn-success btn-icon js-arquivar"
                   data-id='<%# Eval("cod_pedido") %>' title="Arquivar">
                  <i class="fa fa-file"></i>
                </a>

                <!-- Editar (mantido) -->
                <asp:LinkButton ID="gvlnkEdit" runat="server" CommandName="editRecord"
                    CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'
                    CssClass="btn btn-info btn-icon" ToolTip="Editar" CausesValidation="false">
                  <i class="fa fa-pen"></i>
                </asp:LinkButton>

                <!-- Excluir (mantido) -->
                <asp:LinkButton ID="gvlnkDelete" runat="server" CommandName="deleteRecord"
                    CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'
                    CssClass="btn btn-danger btn-icon" OnClientClick="return confirmation();"
                    ToolTip="Excluir" CausesValidation="false">
                  <i class="fa fa-trash"></i>
                </asp:LinkButton>
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
        </asp:GridView>
      </div>
    </div>
  </div>

  <!-- HiddenField para o ID selecionado -->
  <asp:HiddenField ID="hfPedidoId" runat="server" />

  <!-- MODAL Arquivamento -->
  <div class="modal fade" id="modalArquivo" tabindex="-1" aria-labelledby="lblModalArquivo" aria-hidden="true">
    <div class="modal-dialog modal-md">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="lblModalArquivo">Arquivar Pedido</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
        </div>

        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">Retirado por:</label>
            <asp:TextBox ID="txtRetiradoPor" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRetiradoPor"
                Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
          </div>

          <div class="mb-3">
            <label class="form-label">RG ou CPF:</label>
            <asp:TextBox ID="txtRgCpf" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRgCpf"
                Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
          </div>

          <div class="row">
            <div class="col-sm-6 mb-3">
              <label class="form-label">Data:</label>
              <asp:TextBox ID="txtData" runat="server" CssClass="form-control" />
              <asp:RequiredFieldValidator runat="server" ControlToValidate="txtData"
                  Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
            </div>
            <div class="col-sm-6 mb-3">
              <label class="form-label">Hora:</label>
              <asp:TextBox ID="txtHora" runat="server" CssClass="form-control" />
              <asp:RequiredFieldValidator runat="server" ControlToValidate="txtHora"
                  Display="Dynamic" ForeColor="red" ErrorMessage="Obrigatório" />
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
          <asp:Button ID="btnConfirmarArquivo" runat="server" CssClass="btn btn-success"
              Text="Salvar e Arquivar" OnClick="btnConfirmarArquivo_Click" />
        </div>
      </div>
    </div>
  </div>

  <!-- jQuery fallback (DataTables depende de jQuery) -->
  <script>
      if (typeof window.jQuery === 'undefined') {
          document.write('<script src="https://code.jquery.com/jquery-3.7.1.min.js" crossorigin="anonymous"><\/script>');
      }
  </script>

  <!-- DataTables (core) -->
  <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>

  <!-- Moment + locale pt-br (necessário para render.moment) -->
  <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
  <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/locale/pt-br.min.js"></script>

  <!-- DataTables datetime renderer (usa Moment internamente) -->
  <script src="https://cdn.datatables.net/plug-ins/1.13.6/dataRender/datetime.js"></script>

  <script type="text/javascript">
      (function () {
          "use strict";

          // --- Apenas números no RH
          (function onlyNumbersOnRH() {
              var tb = document.getElementById('<%= txbProntuario.ClientID %>');
        if (!tb) return;
        tb.addEventListener('keypress', function (e) {
          var ch = e.which || e.keyCode;
          if (ch === 8 || ch === 9 || ch === 13) return;
          if (ch < 48 || ch > 57) e.preventDefault();
        });
      })();

      // --- DataTables
      function ensureThead($tbl) {
        if ($tbl.find('thead').length === 0) {
          var $first = $tbl.find('tr:first');
          if ($first.length) $tbl.prepend($('<thead/>').append($first));
        }
      }

      function initDT() {
        var $tbl = $('#<%= GridView1.ClientID %>');
        if (!$tbl.length || !$.fn || !$.fn.DataTable) return;
        if ($tbl.find('tbody tr').length === 0) return;

        ensureThead($tbl);
        if ($.fn.DataTable.isDataTable($tbl[0])) $tbl.DataTable().clear().destroy();

        moment.locale('pt-br');

        $tbl.DataTable({
          paging: true,
          pageLength: 10,
          ordering: true,
          stateSave: true,
          responsive: true,
          autoWidth: false,
          dom: '<"top d-flex justify-content-between align-items-center"lfr>t<"bottom d-flex justify-content-between align-items-center"ip>',
          columnDefs: [
            {
              // 3: Data do Pedido — exibir só a DATA (ordenando corretamente)
              targets: 3,
              render: $.fn.dataTable.render.moment(
                ['DD/MM/YYYY HH:mm:ss', 'DD/MM/YYYY', 'YYYY-MM-DD', 'YYYY-MM-DD HH:mm:ss', 'YYYY-MM-DD[T]HH:mm:ss'],
                'DD/MM/YYYY',
                'pt-br'
              )
            },
            {
              // 4: Data de Cadastro — exibir DATA + HORA (ordenando corretamente)
              targets: 4,
              render: $.fn.dataTable.render.moment(
                ['DD/MM/YYYY HH:mm:ss', 'DD/MM/YYYY', 'YYYY-MM-DD', 'YYYY-MM-DD HH:mm:ss', 'YYYY-MM-DD[T]HH:mm:ss'],
                'DD/MM/YYYY HH:mm:ss',
                'pt-br'
              )
            }
          ],
          // order: [[3, 'desc']],
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

      // --- Modal Arquivar: abre/limpa e foca
      function registerOpenArquivoModal() {
        window.openArquivoModal = function (id) {
          try {
            const hf = document.getElementById('<%= hfPedidoId.ClientID %>');
            const ret = document.getElementById('<%= txtRetiradoPor.ClientID %>');
            const rg  = document.getElementById('<%= txtRgCpf.ClientID %>');
            const dt  = document.getElementById('<%= txtData.ClientID %>');
            const hr  = document.getElementById('<%= txtHora.ClientID %>');
                      const el = document.getElementById('modalArquivo');

                      if (!el) { console.error('Modal não encontrado'); return false; }

                      if (hf) hf.value = id;
                      if (ret) ret.value = "";
                      if (rg) rg.value = "";

                      const now = new Date();
                      if (dt) dt.value = now.toISOString().slice(0, 10); // yyyy-mm-dd
                      if (hr) hr.value = now.toTimeString().slice(0, 5);  // HH:mm

                      if (window.bootstrap && typeof bootstrap.Modal === 'function') {
                          const m = bootstrap.Modal.getOrCreateInstance(el, { backdrop: true, keyboard: true });
                          m.show();
                      } else {
                          // fallback se Bootstrap JS não estiver disponível
                          el.classList.add('show'); el.style.display = 'block'; document.body.classList.add('modal-open');
                      }

                      setTimeout(() => { if (ret) ret.focus(); }, 120);
                  } catch (err) {
                      console.error(err); alert('Erro ao abrir o modal: ' + (err?.message || err));
                  }
                  return false;
              };
          }

          // Delegação de clique para o botão Arquivar no grid
          function registerArquivoClickHandler() {
              $(document).off('click.arquivar', '.js-arquivar')
                  .on('click.arquivar', '.js-arquivar', function (e) {
                      e.preventDefault();
                      const id = $(this).data('id');
                      return window.openArquivoModal(id);
                  });
          }

          // Confirmações existentes
          function registerConfirmations() {
              window.confirmation = function () { return confirm("Você realmente quer deletar o registro?"); };
          }

          // Hooks
          $(function () {
              registerOpenArquivoModal();
              registerArquivoClickHandler();
              registerConfirmations();
              initDT();
          });

          // Re-inicializa após postback parcial (UpdatePanel)
          if (window.Sys && Sys.Application) {
              Sys.Application.add_load(function () {
                  registerOpenArquivoModal();
                  registerArquivoClickHandler();
                  registerConfirmations();
                  setTimeout(initDT, 0);
              });
          }
      })();
  </script>
</asp:Content>
