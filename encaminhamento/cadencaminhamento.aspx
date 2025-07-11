<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="cadencaminhamento.aspx.cs" Inherits="publico_cadencaminhamento" Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../vendors/iCheck/skins/flat/green.css" rel="stylesheet" />
    <link href="../js/chosen.min.css" rel="stylesheet" />
    <style>
        .chosen-container { width: 100% !important; }
        .x_panel {
            padding: 15px;
            border: 1px solid #e5e5e5;
            border-radius: 8px;
            background-color: #f9f9f9;
            margin-bottom: 1.5rem;
        }
        .x_title h2 {
            font-size: 1.2em;
            font-weight: bold;
        }
        .form-group label {
            font-weight: 500;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="../vendors/jquery/dist/jquery.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script src="../js/chosen.jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input").attr("autocomplete", "off");

            $('input').iCheck({
                checkboxClass: 'icheckbox_flat-green',
                radioClass: 'iradio_flat-green',
                increaseArea: '20%'
            });

            $('.numeric').keyup(function () {
                $(this).val(this.value.replace(/\D/g, ''));
            });

            $(".chosen-select").chosen({
                no_results_text: "Nada encontrado!",
                placeholder_text_multiple: "Selecione uma ou mais opções"
            });


            $("#btnCloseModal").click(function () {
                $(location).attr('href', 'cadencaminhamento.aspx');
            });
        });
    </script>

    <h3><asp:Label ID="lbTitulo" runat="server" Text="Cadastro Solicitação de Exame"></asp:Label></h3>

    <div class="x_panel">
        <div class="x_title">
            <h2>Informações do Paciente <asp:Label ID="lbProntuario" runat="server" Text="" Style="color: Black"></asp:Label></h2>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <label>Prontuário: <span class="required">*</span></label>
                <asp:TextBox ID="txbProntuario" class="form-control numeric" runat="server" AutoPostBack="true" />
                <asp:RequiredFieldValidator ID="UsernameRequiredValidator" runat="server" ControlToValidate="txbProntuario" ForeColor="red" Display="Static" ErrorMessage="Required" />
            </div>
            <div class="col-md-6 mt-2">
                <asp:Button ID="SearchButton" Text="Pesquisar" runat="server" class="btn btn-primary" OnClick="btnPesquisapaciente_Click" />
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-6">
                <label>Nome</label>
                <asp:TextBox ID="txbNomePaciente" runat="server" Enabled="false" class="form-control"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="x_panel">
        <div class="x_title">
            <h2>Informações do Pedido</h2>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <label>Data do pedido</label>
                <asp:TextBox ID="txbDtPedido" runat="server" class="form-control" data-inputmask="'mask': '99/99/9999'"></asp:TextBox>
            </div>
            <div class="col-md-6">
                <label>Especialidade</label>
                <asp:DropDownList ID="ddlEspecialidade" runat="server" class="form-control"></asp:DropDownList>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-12">
                <asp:TextBox ID="txbOb" runat="server" class="form-control" TextMode="MultiLine" Rows="6" Text="Retirado por:&#10;RG ou CPF:&#10;Data:       &#10;Hora:"></asp:TextBox>
            </div>
        </div>
    </div>

    <!-- Repetição dos painéis com o mesmo padrão -->
    <div class="x_panel">
        <div class="x_title">
            <h2>Informações do Pré Operatório</h2>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <select id="select2" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
            </div>
        </div>
    </div>

    <div class="x_panel">
        <div class="x_title">
            <h2>Informações de Ressonância</h2>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <select id="select1" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
            </div>
        </div>
    </div>

    <div class="x_panel">
        <div class="x_title">
            <h2>Informações de Teleconsulta</h2>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <select id="select3" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
            </div>
        </div>
    </div>

    <div class="x_panel">
        <div class="x_title">
            <h2>Informações de Exames Únicos</h2>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <select id="select4" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
            </div>
        </div>
    </div>

    <div class="x_panel">
        <div class="x_title">
            <h2>Informações do Solicitante</h2>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <label>Médico/Profissional</label>
                <asp:TextBox ID="txbprofissional" runat="server" class="form-control"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="row text-center">
        <asp:Button ID="btnBravar" runat="server" Text="Gravar" class="btn btn-primary btn-lg" OnClick="btnGravar_Click" />
    </div>

    <!-- Modal -->
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-hidden="true" data-keyboard="false" data-backdrop="static">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Cadastro</h5>
                </div>
                <div class="modal-body text-center">
                    <h2>Pedido Cadastrado.</h2>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnCloseModal" class="btn btn-default" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
