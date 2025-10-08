<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="TiposExame.aspx.cs" Inherits="restrito_TiposExame"
    Title="HSPM ATENDIMENTO - Tipos de Exame" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.8/css/jquery.dataTables.min.css" />
    <style>
        .page-title{font-weight:600;margin:10px 0 18px}
        .card{background:#fff;border:1px solid #e5e7eb;border-radius:10px;box-shadow:0 1px 2px rgba(0,0,0,.04)}
        .card-body{padding:18px}
        .badge-A{background:#e6f4ea;color:#137333;border:1px solid #c6e7cc;padding:.25rem .5rem;border-radius:.4rem}
        .badge-I{background:#fce8e6;color:#c5221f;border:1px solid #f8c7c3;padding:.25rem .5rem;border-radius:.4rem}
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- ScriptManager deve vir antes de qualquer UpdatePanel -->
    <asp:ScriptManager ID="sm" runat="server" EnablePartialRendering="true" />

    <div class="container-fluid">
        <h3 class="page-title text-primary">Cadastro de Tipos de Exame</h3>

        <!-- Abas (categorias) -->
        <ul class="nav nav-tabs mb-3">
            <li class="nav-item">
                <asp:LinkButton ID="tabExamesUnico" runat="server" CssClass="nav-link"
                    OnClick="Tab_Click" CommandArgument="exames_unico">Exames Únicos</asp:LinkButton>
            </li>
            <li class="nav-item">
                <asp:LinkButton ID="tabPreOp" runat="server" CssClass="nav-link"
                    OnClick="Tab_Click" CommandArgument="pre_operatorio">Pré-operatório</asp:LinkButton>
            </li>
            <li class="nav-item">
                <asp:LinkButton ID="tabRessonancia" runat="server" CssClass="nav-link"
                    OnClick="Tab_Click" CommandArgument="ressonancia">Ressonância</asp:LinkButton>
            </li>
            <li class="nav-item">
                <asp:LinkButton ID="tabTeleconsulta" runat="server" CssClass="nav-link"
                    OnClick="Tab_Click" CommandArgument="teleconsulta">Teleconsulta</asp:LinkButton>
            </li>
        </ul>

        <asp:HiddenField ID="hfCategoria" runat="server" />

        <div class="card">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div>
                        <asp:Label ID="lblOk" runat="server" CssClass="text-success font-weight-bold"></asp:Label>
                        <asp:Label ID="lblErr" runat="server" CssClass="text-danger font-weight-bold"></asp:Label>
                    </div>
                    <button type="button" class="btn btn-primary"
                            data-bs-toggle="modal" data-bs-target="#mdlNovo">
                        <i class="fa fa-plus"></i> Novo registro
                    </button>
                </div>

                <asp:UpdatePanel ID="updGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                         <asp:HiddenField ID="hfEditId" runat="server" />
                         <asp:HiddenField ID="hfDelId" runat="server" />
                        <asp:GridView ID="gvTipos" runat="server"
                            CssClass="table table-striped table-bordered"
                            AutoGenerateColumns="False" DataKeyNames="Id"
                            OnRowCommand="gvTipos_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="Código" />
                                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <span class='<%# "badge-" + Eval("StatusBadge") %>'>
                                            <%# Eval("StatusTexto") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ações">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server"
                                            CssClass="btn btn-sm btn-outline-primary me-1"
                                            CommandName="editRecord"
                                            CommandArgument="<%# Container.DataItemIndex %>">
                                            <i class="fa fa-edit"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDel" runat="server"
                                            CssClass="btn btn-sm btn-outline-danger"
                                            CommandName="deleteRecord"
                                            CommandArgument="<%# Container.DataItemIndex %>">
                                            <i class="fa fa-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- Modal NOVO -->
    <div class="modal fade" id="mdlNovo" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog" role="document"><div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Novo registro</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
            </div>
            <div class="modal-body">
                <div class="form-group mb-3">
                    <label>Descrição</label>
                    <asp:TextBox ID="txtDescricaoNovo" runat="server" CssClass="form-control" MaxLength="255" />
                </div>
                <div class="form-group">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatusNovo" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Ativo" Value="A" />
                        <asp:ListItem Text="Inativo" Value="I" />
                    </asp:DropDownList>
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSalvarNovo" runat="server" CssClass="btn btn-primary"
                            Text="Salvar" OnClick="btnSalvarNovo_Click" />
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
            </div>
        </div></div>
    </div>

    <!-- Modal EDITAR -->
    <div class="modal fade" id="mdlEdit" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog" role="document"><div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Editar registro</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
            </div>
            <div class="modal-body">
               
                <div class="form-group mb-3">
                    <label>Descrição</label>
                    <asp:TextBox ID="txtDescricaoEdit" runat="server" CssClass="form-control" MaxLength="255" />
                </div>
                <div class="form-group">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatusEdit" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Ativo" Value="A" />
                        <asp:ListItem Text="Inativo" Value="I" />
                    </asp:DropDownList>
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSalvarEdit" runat="server" CssClass="btn btn-primary"
                            Text="Atualizar" OnClick="btnSalvarEdit_Click" />
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
            </div>
        </div></div>
    </div>

    <!-- Modal EXCLUIR -->
    <div class="modal fade" id="mdlDel" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog" role="document"><div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Excluir registro</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
            </div>
            <div class="modal-body">
               
                <p>Confirma a exclusão do registro
                   <strong>#<asp:Literal ID="litDelId" runat="server" /></strong>?</p>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnConfirmDel" runat="server" CssClass="btn btn-danger"
                            Text="Excluir" OnClick="btnConfirmDel_Click" />
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
            </div>
        </div></div>
    </div>

    <!-- DataTables (requer jQuery já carregado na Master) -->
    <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript">
      (function ($) {

        // Se o GridView renderizar <tr> de header fora de <thead>, corrige aqui
        function ensureThead($t) {
          if ($t.find('> thead').length === 0) {
            var $firstTr = $t.find('> tr:first');
            if ($firstTr.length && $firstTr.find('th').length) {
              var $thead = $('<thead/>').append($firstTr.clone());
              $firstTr.remove();
              $t.prepend($thead);
            }
          }
        }

        function safeInitDataTable() {
          var $t = $('#<%= gvTipos.ClientID %>');
                if (!$t.length || !$.fn.DataTable) return;

                // garante cabeçalho dentro de THEAD
                ensureThead($t);

                // compara quantidade de colunas pra evitar o erro tn/18
                var headerCells = $t.find('> thead > tr:first > th').length;
                var bodyCells = $t.find('> tbody > tr:first > td').length;

                if (bodyCells && bodyCells !== headerCells) {
                    // Não inicializa quando há colspan/linhas “especiais”
                    return;
                }

                // destrói instância anterior (se existir) antes de recriar
                if ($.fn.DataTable.isDataTable($t[0])) {
                    $t.DataTable().clear().destroy();
                }

                $t.DataTable({
                    stateSave: true,
                    language: {
                        processing: "Processando...", search: "Buscar:", lengthMenu: "Mostrar _MENU_",
                        info: "Mostrando _START_ a _END_ de _TOTAL_", infoEmpty: "Sem registros",
                        infoFiltered: "(filtrado de _MAX_)", loadingRecords: "Carregando...",
                        zeroRecords: "Nada encontrado", emptyTable: "Sem dados",
                        paginate: { first: "Primeiro", previous: "Anterior", next: "Próximo", last: "Último" }
                    }
                });
            }

            // 1) primeira carga
            $(safeInitDataTable);

            // 2) reinicializa após qualquer AsyncPostBack do UpdatePanel
            if (window.Sys && Sys.WebForms) {
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                if (prm) prm.add_endRequest(function () { safeInitDataTable(); });
            }

            // helpers chamados pelo servidor
            window.openEdit = function () { $('#mdlEdit').modal('show'); };
            window.openDel = function () { $('#mdlDel').modal('show'); };

        })(jQuery);
    </script>
</asp:Content>
