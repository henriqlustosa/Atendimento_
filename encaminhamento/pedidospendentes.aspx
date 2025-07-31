<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="pedidospendentes.aspx.cs" Inherits="encaminhamento_pedidospendentes"
    Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- Bootstrap (já deve estar na MasterPage) -->
    <link href="https://cdn.datatables.net/1.13.6/css/dataTables.bootstrap4.min.css" rel="stylesheet" />

    <style>
        .hidden-xs { display: none; }
        @media (min-width: 576px) {
            .hidden-xs { display: table-cell !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container-fluid">
        <h3 class="mb-4 mt-3 font-weight-bold text-primary">
            <asp:Label ID="lbTitulo" runat="server" Text="Solicitações de Exames Cadastrados"></asp:Label>
        </h3>

        <div class="table-responsive">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                DataKeyNames="cod_pedido" OnRowCommand="grdMain_RowCommand"
                CssClass="table table-bordered table-striped" UseAccessibleHeader="true"
                GridLines="Horizontal" BorderColor="#e0ddd1" Width="100%">
                <RowStyle BackColor="#f7f6f3" ForeColor="#333333" />

                <Columns>
                    <asp:BoundField DataField="cod_pedido" HeaderText="Código do Pedido" SortExpression="cod_pedido"
                        ItemStyle-CssClass="hidden-xs" HeaderStyle-CssClass="hidden-xs" />
                    <asp:BoundField DataField="prontuario" HeaderText="Prontuário" SortExpression="prontuario"
                        ItemStyle-CssClass="hidden-xs" HeaderStyle-CssClass="hidden-xs" />
                    <asp:BoundField DataField="nome_paciente" HeaderText="Paciente" SortExpression="nome_paciente" />
                    <asp:BoundField DataField="data_pedido" HeaderText="Data Pedido" SortExpression="data_pedido" />
                    <asp:BoundField DataField="data_cadastro" HeaderText="Data Cadastro" SortExpression="data_cadastro" />
                    <asp:BoundField DataField="descricao_espec" HeaderText="Especialidade" SortExpression="descricao_espec" />
                    <asp:BoundField DataField="exames_solicitados" HeaderText="Exames Solicitados" SortExpression="exames_solicitados" />
                    <asp:BoundField DataField="outras_informacoes" HeaderText="Outras Informações" SortExpression="outras_informacoes" />
                    <asp:BoundField DataField="solicitante" HeaderText="Solicitante" SortExpression="solicitante" />
                    <asp:BoundField DataField="lista_exames" HeaderText="Exames" SortExpression="lista_exames" />
                    <asp:BoundField DataField="lista_ressonancia" HeaderText="Ressonância" SortExpression="lista_ressonancia" />
                    <asp:BoundField DataField="usuario" HeaderText="Usuário" SortExpression="usuario" />

                 
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="gvlnkFile" runat="server" CommandName="fileRecord"
                                CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'
                                CssClass="btn btn-success btn-sm mx-1" OnClientClick="return file();">
                                <i class="fa fa-file" title="Arquivar"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="gvlnkEdit" runat="server" CommandName="editRecord"
                                CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'
                                CssClass="btn btn-info btn-sm mx-1">
                                <i class="fa fa-pencil-alt" title="Editar"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="gvlnkDelete" runat="server" CommandName="deleteRecord"
                                CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'
                                CssClass="btn btn-danger btn-sm mx-1" OnClientClick="return confirmation();">
                                <i class="fa fa-trash" title="Excluir"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#ffffff" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
            </asp:GridView>
        </div>
    </div>

    <!-- Scripts -->
    
    <script src="https://cdn.datatables.net/1.13.6/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.6/js/dataTables.bootstrap4.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/luxon/3.4.4/luxon.min.js"></script>
    <script src="https://cdn.datatables.net/plug-ins/1.13.6/dataRender/datetime.js"></script>

    <script>
        $(document).ready(function () {
            var table = $('#<%= GridView1.ClientID %>');

            // Corrige o thead para DataTables funcionar
            var header = table.find("tr:first").clone();
            table.find("tr:first").remove();
            table.prepend($("<thead></thead>").append(header));

            // Inicializa o DataTables
            table.DataTable({
                responsive: true,
                language: {
                    processing: "Processando...",
                    search: "Buscar:",
                    lengthMenu: "Mostrar _MENU_ registros por página",
                    info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                    infoEmpty: "Mostrando 0 a 0 de 0 registros",
                    infoFiltered: "(filtrado de _MAX_ registros no total)",
                    infoPostFix: "",
                    loadingRecords: "Carregando...",
                    zeroRecords: "Nenhum registro encontrado",
                    emptyTable: "Nenhum dado disponível na tabela",
                    paginate: {
                        first: "Primeiro",
                        previous: "Anterior",
                        next: "Próximo",
                        last: "Último"
                    },
                    aria: {
                        sortAscending: ": ativar para ordenar a coluna em ordem crescente",
                        sortDescending: ": ativar para ordenar a coluna em ordem decrescente"
                    }
                },

                columnDefs: [
                    {
                        targets: [3, 4], // Colunas de data
                        render: $.fn.dataTable.render.datetime('dd/MM/yyyy', null, 'pt-BR')
                    }
                ],
                order: [[0, 'asc']]
            });
        });

        function confirmation() {
            return confirm("Você realmente quer deletar o registro?");
        }

        function file() {
            return confirm("Você realmente quer arquivar o registro?");
        }
    </script>
</asp:Content>
