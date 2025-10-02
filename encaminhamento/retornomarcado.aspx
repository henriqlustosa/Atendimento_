<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="retornomarcado.aspx.cs" Inherits="encaminhamento_retornomarcado" Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="https://cdn.jsdelivr.net/npm/icheck@1.0.2/skins/flat/green.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
    <style>
        body { background-color:#f8f9fa; font-family:'Segoe UI',sans-serif; }
        .x_panel { border-radius:10px; padding:20px; background:#fff; margin-bottom:20px; box-shadow:0 2px 4px rgba(0,0,0,.05); }
        .x_title { font-weight:600; font-size:1.1rem; display:flex; align-items:center; gap:8px; margin-bottom:15px; }
        .x_title i { color:#007bff; }
        .form-control { border-radius:6px !important; }
        .chosen-container { width:100% !important; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

    <!-- mantém o id do pedido na página -->
    <asp:HiddenField ID="hfPedidoId" runat="server" />

    <div class="x_panel d-flex align-items-center" style="gap: 12px;">
        <i class="fa-solid fa-stethoscope fa-xl"></i>
        <h2 style="margin: 0;">Edição de Solicitação de Exame</h2>
    </div>

    <!-- Informações do Paciente -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-user"></i> Informações do Paciente</div>
        <div class="row">
            <div class="col-md-9">
                <label>Prontuário:</label>
                <asp:TextBox ID="txbProntuario" CssClass="form-control numeric" runat="server" Enabled="false" />
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-6">
                <label>Nome</label>
                <asp:TextBox ID="txbNomePaciente" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
    </div>

    <!-- Informações do Pedido -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-file-text"></i> Informações do Pedido</div>
        <div class="row">
            <div class="col-md-6">
                <label>Data do pedido</label>
                <asp:TextBox ID="txbDtPedido" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-6">
                <label>Especialidade</label>
                <asp:DropDownList ID="ddlEspecialidade" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-12">
                <asp:TextBox ID="txbOb" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
            </div>
        </div>
    </div>

    <!-- Pré-Operatório -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-plus-square"></i> Informações do Pré-Operatório</div>
        <select id="select2" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
    </div>

    <!-- Ressonância -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-vials"></i> Informações de Ressonância</div>
        <select id="select1" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
    </div>

    <!-- Teleconsulta -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-phone"></i> Informações de Teleconsulta</div>
        <select id="select3" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
    </div>

    <!-- Exames Únicos -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-vial"></i> Informações de Exames Únicos</div>
        <select id="select4" multiple class="form-control chosen-select" runat="server" clientidmode="Static"></select>
    </div>

    <!-- Profissional -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-user-md"></i> Solicitante</div>
        <label>Médico/Profissional</label>
        <asp:TextBox ID="txbprofissional" runat="server" CssClass="form-control"></asp:TextBox>
    </div>

    <div class="row text-center mb-5">
        <asp:Button ID="btnGravar" runat="server" Text="Atualizar" CssClass="btn btn-primary btn-lg px-5" OnClick="btnGravar_Click" />
    </div>

    <!-- Modal Sucesso -->
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-hidden="true" data-keyboard="false" data-backdrop="static">
        <div class="modal-dialog modal-lg"><div class="modal-content">
            <div class="modal-header"><h5 class="modal-title">Confirmação</h5></div>
            <div class="modal-body text-center"><h2>Pedido Atualizado com Sucesso.</h2></div>
            <div class="modal-footer"><button type="button" id="btnCloseModal" class="btn btn-secondary" data-dismiss="modal">Fechar</button></div>
        </div></div>
    </div>

    <!-- Scripts -->
    <script src="https://cdn.jsdelivr.net/npm/icheck@1.0.2/icheck.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

    <script type="text/javascript">
        $(function () {
            flatpickr("#<%= txbDtPedido.ClientID %>", {
                dateFormat: "d/m/Y",
                locale: {
                    firstDayOfWeek: 1,
                    weekdays: { shorthand: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'], longhand: ['Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sábado'] },
                    months: { shorthand: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'], longhand: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'] }
                }
            });

            $("input").attr("autocomplete", "off");

            $('input[type="checkbox"], input[type="radio"]').iCheck({
                checkboxClass: 'icheckbox_flat-green',
                radioClass: 'iradio_flat-green'
            });

            $('.numeric').keyup(function () { $(this).val(this.value.replace(/\D/g, '')); });

            $(".chosen-select").chosen({
                no_results_text: "Nada encontrado!",
                placeholder_text_multiple: "Selecione uma ou mais opções",
                width: "100%"
            });

            $("#btnCloseModal").click(function () { location.href = 'pedidospendentesporrh.aspx'; });
        });
    </script>
</asp:Content>
