<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="cadencaminhamento.aspx.cs" Inherits="publico_cadencaminhamento" Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- CSS de Plugins -->
    <link href="https://cdn.jsdelivr.net/npm/icheck@1.0.2/skins/flat/green.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />

    <!-- Estilo customizado -->
    <style>
        body {
            background-color: #f8f9fa;
            font-family: 'Segoe UI', sans-serif;
        }

        .x_panel {
            border-radius: 10px;
            padding: 20px;
            background-color: #fff;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
        }

        .x_title {
            font-weight: 600;
            font-size: 1.1rem;
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 15px;
        }

        .x_title i {
            color: #007bff;
        }

        .form-control {
            border-radius: 6px !important;
        }

        select.form-control {
            color: #6c757d;
            font-style: italic;
        }

        .btn-primary {
            border-radius: 6px;
            background-color: #007bff;
            border: none;
        }

        .chosen-container {
            width: 100% !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
  

   

    <!-- Título com ícone -->
    <div class="x_panel d-flex align-items-center" style="gap: 12px;">
        <i class="fa-solid fa-stethoscope fa-xl"></i>
        <h2 style="margin: 0;">Cadastro de Solicitação de Exame</h2>
    </div>

    <!-- Informações do Paciente -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-user"></i> Informações do Paciente</div>
        <div class="row">
            <div class="col-md-9">
                <label>Prontuário: *</label>
                <asp:TextBox ID="txbProntuario" CssClass="form-control numeric" runat="server" AutoPostBack="true" />
            </div>
            <div class="col-md-3 d-flex align-items-end">
                <asp:Button ID="SearchButton" Text="Pesquisar" runat="server" CssClass="btn btn-primary w-100" OnClick="btnPesquisapaciente_Click" />
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
                <asp:TextBox ID="txbOb" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"
                    Text="Retirado por:&#10;RG ou CPF:&#10;Data:       &#10;Hora:"></asp:TextBox>
            </div>
        </div>
    </div>

    <!-- Pré Operatório -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-plus-square"></i> Informações do Pré Operatório</div>
        <select id="select2" multiple class="form-control chosen-select" runat="server" clientidmode="Static" data-placeholder="Selecione uma ou mais opções"></select>
    </div>

    <!-- Ressonância -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-vials"></i> Informações de Ressonância</div>
        <select id="select1" multiple class="form-control chosen-select" runat="server" clientidmode="Static" data-placeholder="Selecione uma ou mais opções"></select>
    </div>

    <!-- Teleconsulta -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-phone"></i> Informações de Teleconsulta</div>
        <select id="select3" multiple class="form-control chosen-select" runat="server" clientidmode="Static" data-placeholder="Selecione uma ou mais opções"></select>
    </div>

    <!-- Exames Únicos -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-vial"></i> Informações de Exames Únicos</div>
        <select id="select4" multiple class="form-control chosen-select" runat="server" clientidmode="Static" data-placeholder="Selecione uma ou mais opções"></select>
    </div>

    <!-- Solicitante -->
    <div class="x_panel">
        <div class="x_title"><i class="fa fa-user-md"></i> Informações do Solicitante</div>
        <label>Médico/Profissional</label>
        <asp:TextBox ID="txbprofissional" runat="server" CssClass="form-control" placeholder="Digite o nome do profissional"></asp:TextBox>
    </div>

    <!-- Botão Gravar -->
    <div class="row text-center mb-5">
        <asp:Button ID="btnBravar" runat="server" Text="Gravar" CssClass="btn btn-primary btn-lg px-5" OnClick="btnGravar_Click" />
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
      <!-- Scripts -->
  <!-- Plugins (após jQuery da MasterPage) -->
<script src="https://cdn.jsdelivr.net/npm/icheck@1.0.2/icheck.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

<script type="text/javascript">
    $(document).ready(function () {
        // Flatpickr na textbox de data
        flatpickr("#<%= txbDtPedido.ClientID %>", {
            dateFormat: "d/m/Y",
            locale: {
                firstDayOfWeek: 1,
                weekdays: {
                    shorthand: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'],
                    longhand: ['Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sábado']
                },
                months: {
                    shorthand: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
                    longhand: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro']
                }
            }
        });

        // Desativa autocomplete em inputs
        $("input").attr("autocomplete", "off");

        // Ativa iCheck nos inputs que forem do tipo checkbox ou radio (caso existam)
        $('input[type="checkbox"], input[type="radio"]').iCheck({
            checkboxClass: 'icheckbox_flat-green',
            radioClass: 'iradio_flat-green',
            increaseArea: '20%' // optional
        });

        // Somente números
        $('.numeric').keyup(function () {
            $(this).val(this.value.replace(/\D/g, ''));
        });

        // Chosen nos selects múltiplos
        $(".chosen-select").chosen({
            no_results_text: "Nada encontrado!",
            placeholder_text_multiple: "Selecione uma ou mais opções"
        });

        // Fecha modal e recarrega a página
        $("#btnCloseModal").click(function () {
            location.href = 'cadencaminhamento.aspx';
        });
    });
</script>

</asp:Content>
