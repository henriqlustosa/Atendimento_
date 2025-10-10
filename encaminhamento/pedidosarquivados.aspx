<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="pedidosarquivados.aspx.cs" Inherits="encaminhamento_pedidosarquivados"
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
    .actions-col{ white-space:nowrap; text-align:center; width:140px; }
    .btn-icon{ width:32px; height:32px; padding:0; display:inline-flex; align-items:center; justify-content:center; }
    .btn-icon i{ font-size:14px; }
    .dataTables_length label{ font-weight:500; }
    .dataTables_filter input{ width:220px; }
    @media (max-width:576px){ .dataTables_filter input{ width:160px; } }
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

  <h3 class="page-title">
    <asp:Label ID="lbTitulo" runat="server" Text="Solicitações de Exames Arquivadas"></asp:Label>
  </h3>

  <div class="card">
    <div class="card-body">

     <!-- Toolbar (filtro por RH) -->
<div class="toolbar">
  <div class="form-inline">
    <label for="<%= txbProntuario.ClientID %>" class="mb-0">Prontuário:</label>

    <asp:TextBox ID="txbProntuario" runat="server" CssClass="form-control"
                 placeholder="Digite o RH (opcional)" />
    <!-- Sem RequiredFieldValidator; botão sem validação -->
    <asp:Button ID="btnPesquisar" runat="server" CssClass="btn btn-primary mt-2"
                Text="Pesquisar" OnClick="btnPesquisar_Click" CausesValidation="false" />
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

         
            <asp:BoundField DataField="retirado_informacoes" HeaderText="Informações de Arquivamento"
                            SortExpression="retirado_informacoes" HtmlEncode="false" />
            <asp:BoundField DataField="usuario_baixa" HeaderText="Arquivado por" SortExpression="usuario_baixa" />

            <asp:TemplateField HeaderText=" " ItemStyle-CssClass="actions-col" HeaderStyle-CssClass="sorting_disabled">
              <ItemTemplate>
                <asp:LinkButton ID="gvlnkPrint" CommandName="printRecord"
                    CommandArgument='<%#((GridViewRow)Container).RowIndex%>'
                    CssClass="btn btn-success btn-icon" runat="server" ToolTip="Imprimir">
                  <i class="fa fa-print"></i>
                </asp:LinkButton>

                <asp:LinkButton ID="gvlnkView" CommandName="viewRecord"
                    CommandArgument='<%#((GridViewRow)Container).RowIndex%>'
                    CssClass="btn btn-info btn-icon" runat="server" ToolTip="Visualizar">
                  <i class="fa fa-eye"></i>
                </asp:LinkButton>
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
        </asp:GridView>
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

      // apenas números no RH
      (function onlyNumbersOnRH() {
        var tb = document.getElementById('<%= txbProntuario.ClientID %>');
        if (!tb) return;
        tb.addEventListener('keypress', function (e) {
          var ch = e.which || e.keyCode;
          if (ch === 8 || ch === 9 || ch === 13) return;
          if (ch < 48 || ch > 57) e.preventDefault();
        });
      })();

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

              // Locale do moment
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
                          // 3: Data do Pedido — exibir só a DATA
                          targets: 3,
                          render: $.fn.dataTable.render.moment(
                              ['DD/MM/YYYY HH:mm:ss', 'DD/MM/YYYY', 'YYYY-MM-DD', 'YYYY-MM-DD HH:mm:ss', 'YYYY-MM-DD[T]HH:mm:ss'],
                              'DD/MM/YYYY',
                              'pt-br'
                          )
                      },
                      {
                          // 4: Data de Cadastro — exibir DATA + HORA
                          targets: 4,
                          render: $.fn.dataTable.render.moment(
                              ['DD/MM/YYYY HH:mm:ss', 'DD/MM/YYYY', 'YYYY-MM-DD', 'YYYY-MM-DD HH:mm:ss', 'YYYY-MM-DD[T]HH:mm:ss'],
                              'DD/MM/YYYY HH:mm:ss',
                              'pt-br'
                          )
                      }
                  ],
                  // Ex.: ordenar por Data do Pedido (coluna 3) desc → mais recentes primeiro
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

          $(function () { initDT(); });

          // re-inicializa após postback parcial (UpdatePanel)
          if (window.Sys && Sys.Application) {
              Sys.Application.add_load(function () { setTimeout(initDT, 0); });
          }
      })();
  </script>
</asp:Content>
