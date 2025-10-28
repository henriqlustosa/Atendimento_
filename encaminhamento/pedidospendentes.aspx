<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="pedidospendentes.aspx.cs" Inherits="encaminhamento_pedidospendentes"
    Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css"/>
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

  <asp:ScriptManager ID="sm" runat="server" EnablePartialRendering="true" />

  <h3 class="page-title">
    <asp:Label ID="lbTitulo" runat="server" Text="Solicitações de Exames Cadastrados (Pendentes)"></asp:Label>
  </h3>

  <asp:UpdatePanel ID="updMain" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

      <!-- Toolbar -->
      <div class="card mb-3">
        <div class="card-body">
          <div class="toolbar">
            <div>
              <label for="<%= txbProntuario.ClientID %>" class="mb-1">Prontuário (RH)</label>
              <asp:TextBox ID="txbProntuario" runat="server"
                           CssClass="form-control"
                           placeholder="Digite o RH ou deixe vazio para todos"
                           MaxLength="20" />
            </div>
            <div>
              <label class="mb-1">&nbsp;</label><br />
              <asp:Button ID="btnPesquisar" runat="server" CssClass="btn btn-primary"
                          Text="Pesquisar" OnClick="btnPesquisar_Click" CausesValidation="false" />
            </div>
          </div>
        </div>
      </div>

      <!-- Grid -->
      <div class="card">
        <div class="card-body">

          <asp:GridView ID="GridView1" runat="server"
                        AutoGenerateColumns="False"
                        AllowPaging="true"
                        PageSize="10"
                        AllowSorting="false"
                        DataKeyNames="cod_pedido"
                        DataSourceID="odsPendentes"
                        CssClass="table table-striped table-bordered"
                        GridLines="Horizontal"
                        ShowHeaderWhenEmpty="true"
                        OnRowCommand="grdMain_RowCommand"
                        OnPreRender="GridView1_PreRender"
                        OnDataBound="GridView1_DataBound">

            <Columns>
              <asp:BoundField DataField="cod_pedido" HeaderText="Código do Pedido" />
              <asp:BoundField DataField="prontuario" HeaderText="Prontuário" />
              <asp:BoundField DataField="nome_paciente" HeaderText="Paciente" />

              <asp:BoundField DataField="data_pedido" HeaderText="Data do Pedido"
                              DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
              <asp:BoundField DataField="data_cadastro" HeaderText="Data de Cadastro"
                              DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false" />

              <asp:BoundField DataField="descricao_espec" HeaderText="Especialidade" />
              <asp:BoundField DataField="exames_solicitados" HeaderText="Exames Solicitados" />
              <asp:BoundField DataField="outras_informacoes" HeaderText="Outras Informações" />

              <asp:TemplateField HeaderText="Carga Geral">
                <ItemTemplate><%# FormatCargaGeral(Eval("carga_geral")) %></ItemTemplate>
              </asp:TemplateField>

              <asp:BoundField DataField="usuario" HeaderText="Usuário" />

              <asp:TemplateField HeaderText=" " ItemStyle-CssClass="actions-col">
                <ItemTemplate>
               <a href="#" class="btn btn-success btn-icon js-arquivar"
   data-id='<%# Eval("cod_pedido") %>' title="Arquivar">
  <i class="fa fa-file"></i>
</a>
                  <asp:LinkButton ID="gvlnkEdit" runat="server" CommandName="editRecord"
                      CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'
                      CssClass="btn btn-info btn-icon" ToolTip="Editar">
                    <i class="fa fa-pen"></i>
                  </asp:LinkButton>
                  <asp:LinkButton ID="gvlnkDelete" runat="server" CommandName="deleteRecord"
                      CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'
                      CssClass="btn btn-danger btn-icon" ToolTip="Excluir"
                      OnClientClick="return confirm('Confirma exclusão?');">
                    <i class="fa fa-trash"></i>
                  </asp:LinkButton>
                </ItemTemplate>
              </asp:TemplateField>
            </Columns>
          </asp:GridView>

          <!-- PAGER EXTERNO -->
          <div class="d-flex justify-content-between align-items-center mt-2">
            <div class="btn-group" role="group" aria-label="Pager">
              <asp:LinkButton ID="lnkFirst" runat="server" CssClass="btn btn-light btn-sm"
                  OnClick="lnkFirst_Click">««</asp:LinkButton>
              <asp:LinkButton ID="lnkPrev" runat="server" CssClass="btn btn-light btn-sm"
                  OnClick="lnkPrev_Click">«</asp:LinkButton>

              <asp:Repeater ID="rptPages" runat="server"
                  OnItemCommand="rptPages_ItemCommand"
                  OnItemDataBound="rptPages_ItemDataBound">
                <ItemTemplate>
                  <asp:LinkButton ID="lnkPage" runat="server" CssClass="btn btn-light btn-sm"
                      CommandName="Page" CommandArgument='<%# Eval("Index") %>' />
                  <asp:Label ID="lblEllipsis" runat="server" Text="..." CssClass="btn btn-light btn-sm disabled" />
                </ItemTemplate>
              </asp:Repeater>

              <asp:LinkButton ID="lnkNext" runat="server" CssClass="btn btn-light btn-sm"
                  OnClick="lnkNext_Click">»</asp:LinkButton>
              <asp:LinkButton ID="lnkLast" runat="server" CssClass="btn btn-light btn-sm"
                  OnClick="lnkLast_Click">»»</asp:LinkButton>
            </div>

            <asp:Label ID="lblPageInfo" runat="server" CssClass="text-muted"></asp:Label>
          </div>

          <asp:ObjectDataSource ID="odsPendentes" runat="server"
            TypeName="PedidoDAO"
            SelectMethod="SelectPendentes"
            SelectCountMethod="SelectPendentesCount"
            EnablePaging="true"
            StartRowIndexParameterName="startRowIndex"
            MaximumRowsParameterName="maximumRows"
            SortParameterName="sortExpression"
            OnSelecting="odsPendentes_Selecting">

            <SelectParameters>
              <asp:ControlParameter Name="rh"
                ControlID="txbProntuario" PropertyName="Text"
                Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
              <asp:Parameter Name="sortExpression" Type="String" DefaultValue="cod_pedido DESC" />
            </SelectParameters>
          </asp:ObjectDataSource>

        </div>
      </div>

      <!-- Modal Arquivo (Bootstrap 5) -->
<asp:HiddenField ID="hfPedidoId" runat="server" />
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
        </div>
        <div class="mb-3">
          <label class="form-label">RG ou CPF:</label>
          <asp:TextBox ID="txtRgCpf" runat="server" CssClass="form-control" />
        </div>
        <div class="row g-3">
          <div class="col-sm-6">
            <label class="form-label">Data:</label>
            <asp:TextBox ID="txtData" runat="server" CssClass="form-control" />
          </div>
          <div class="col-sm-6">
            <label class="form-label">Hora:</label>
            <asp:TextBox ID="txtHora" runat="server" CssClass="form-control" />
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
</div>

    </ContentTemplate>
  </asp:UpdatePanel>

  <!-- JS libs -->
  <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
  <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
  <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
  <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/locale/pt-br.min.js"></script>

  <!-- Inicialização -->
  <script>
  (function () {
    "use strict";

    function normalizeColumnCount($tbl) {
      var thCount = $tbl.find('thead th').length;
      $tbl.find('tbody tr').each(function () {
        var $tr = $(this);
        var $tds = $tr.children('td');
        if ($tds.length === 1 && $tds.attr('colspan')) {
          $tr.remove();
          return;
        }
        var tdCount = $tds.length;
        for (var i = tdCount; i < thCount; i++) $tr.append('<td></td>');
      });
    }

    window.initDT = function initDT(tries) {
      tries = typeof tries === "number" ? tries : 10;

      var $tbl = $('#<%= GridView1.ClientID %>');
              if (!$tbl.length || !$.fn || !$.fn.DataTable) return;

              var th = $tbl.find('thead th').length;
              var $firstTds = $tbl.find('tbody tr:first td');

              if ($firstTds.length === 0 || ($firstTds.length === 1 && $firstTds.attr('colspan'))) {
                  if (tries > 0) return setTimeout(function () { initDT(tries - 1); }, 120);
                  return;
              }
              if ($firstTds.length !== th) {
                  normalizeColumnCount($tbl);
                  $firstTds = $tbl.find('tbody tr:first td');
                  if ($firstTds.length !== th) {
                      if (tries > 0) return setTimeout(function () { initDT(tries - 1); }, 120);
                      return;
                  }
              }

              if ($.fn.DataTable.isDataTable($tbl[0])) {
                  $tbl.DataTable().clear().destroy();
                  $tbl.find('thead').show();
              }

              if (typeof moment !== 'undefined') moment.locale('pt-br');

              $tbl.DataTable({
                  paging: false,          // paginação é do servidor
                  searching: true,
                  info: false,
                  lengthChange: false,
                  ordering: true,
                  language: { search: "Pesquisar:", emptyTable: "Nenhum registro encontrado" },
                  columnDefs: [
                      {
                          targets: [3, 4],
                          render: function (data, type) {
                              var m = moment(data, [
                                  'DD/MM/YYYY HH:mm:ss', 'DD/MM/YYYY', 'YYYY-MM-DD HH:mm:ss', 'YYYY-MM-DD', 'YYYY-MM-DD[T]HH:mm:ss'
                              ], true);
                              if ((type === 'sort' || type === 'type') && m.isValid())
                                  return m.format('YYYY-MM-DD HH:mm:ss');
                              return m.isValid() ? m.format('DD/MM/YYYY HH:mm:ss') : (data || '');
                          }
                      },
                      { targets: -1, orderable: false, searchable: false }
                  ],
                  order: [[0, 'desc']]
              });
          };

          // ---- Debounce para garantir apenas UMA inicialização por ciclo ----
          var initTimer = null;
          function debounceInit() {
              if (initTimer) clearTimeout(initTimer);
              initTimer = setTimeout(function () { initDT(10); }, 250);
          }

          $(document).ready(function () { debounceInit(); });

          if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
              Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                  debounceInit();
              });
          }
      })();
   
          (function () {
              "use strict";

          function abrirModalArquivar(id) {
              // grava o id no hidden
              document.getElementById("<%= hfPedidoId.ClientID %>").value = id;

          // preenche data/hora atuais (opcional)
          var now = new Date();
          var dd = String(now.getDate()).padStart(2, '0');
          var mm = String(now.getMonth() + 1).padStart(2, '0');
          var yyyy = now.getFullYear();
          var hh = String(now.getHours()).padStart(2, '0');
          var mi = String(now.getMinutes()).padStart(2, '0');

    document.getElementById("<%= txtData.ClientID %>").value = dd + '/' + mm + '/' + yyyy;
    document.getElementById("<%= txtHora.ClientID %>").value = hh + ':' + mi;

          // Bootstrap 5: cria/recupera e abre o modal
          var modalEl = document.getElementById('modalArquivo');
          var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
          modal.show();
  }

          function wireModalHandlers() {
              // Desregistra anteriores para evitar duplicidade após UpdatePanel
              $(document).off('click.hspm', '.js-arquivar');

          $(document).on('click.hspm', '.js-arquivar', function (e) {
              e.preventDefault();
          var id = this.getAttribute('data-id');
          abrirModalArquivar(id);
    });
  }

          // 1x no load
          $(document).ready(wireModalHandlers);

          // Após qualquer partial postback
          if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
              Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                  wireModalHandlers();
              });
  }
})();
  </script>



</asp:Content>
