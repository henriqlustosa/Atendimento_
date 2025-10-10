<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="arquivomarcado.aspx.cs" Inherits="encaminhamento_arquivomarcado" Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- iCheck (estilo) -->
    <link href="../vendors/iCheck/skins/flat/green.css" rel="stylesheet" />
    <!-- Chosen -->
    <link href="../js/chosen.min.css" rel="stylesheet" type="text/css" />

    <style>
        /* grupo de radios com visual consistente */
        .rb-group{
            display:flex; align-items:center; gap:24px;
            background:#f9fafb; border:1px solid #e9ecef; border-radius:10px; padding:10px 12px;
        }
        .rb-item{ display:flex; align-items:center; gap:8px; font-weight:600; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="../vendors/jquery/dist/jquery.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <!-- Chosen -->
    <script src="../js/chosen.jquery.min.js" type="text/javascript"></script>
    <!-- iCheck (script) -->
    <script src="../vendors/iCheck/icheck.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $("input").attr("autocomplete", "off");

            $('.numeric').keyup(function () {
                $(this).val(this.value.replace(/\D/g, ''));
            });

            // Chosen (mantido como estava e desabilitado)
            $('#<%=select1.ClientID %>').attr('disabled', 'disabled');
            $('#<%=select2.ClientID %>').attr('disabled', 'disabled');
            $("#<%=select1.ClientID %>").chosen();
            $("#<%=select2.ClientID %>").chosen();

            // iCheck para radios / checkboxes
            $('input[type="checkbox"], input[type="radio"]').iCheck({
                checkboxClass: 'icheckbox_flat-green',
                radioClass: 'iradio_flat-green',
                increaseArea: '20%'
            });

            // garante "Não" como padrão se nada vier marcado do servidor
            var rbNao = $('#<%= rbCargaNao.ClientID %>');
            var rbSim = $('#<%= rbCargaSim.ClientID %>');
            if (!rbNao.is(':checked') && !rbSim.is(':checked')) {
                rbNao.iCheck('check');
            }
        });

        function validaEspecialidade() { $('.valida').css("border", "1px solid red"); }
        function validaCampo() { $('.valida').css("border", "1px solid red"); }
    </script>

    <div class="x_panel">
        <div class="x_title">
            <h2>Cadastro
                <asp:Label ID="lbProntuario" runat="server" Text="" Style="color: Black"></asp:Label>
            </h2>
            <div class="clearfix"></div>
        </div>

        <div class="container">
            <div class="body">

                <div class="form-group row">
                    <label for="txbNomePaciente" class="col-sm-2 col-form-label">Nome Paciente:</label>
                    <div class="col-sm-5">
                        <asp:TextBox ID="txbNomePaciente" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </div>
                    <label for="txbProntuario" class="col-sm-1 col-form-label">Prontuário:</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txbProntuario" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group row">
                    <label for="lbCodPedido" class="col-sm-2 col-form-label">Código Pedido:</label>
                    <div class="col-sm-10">
                        <asp:Label ID="lbCodPedido" runat="server" Text=""></asp:Label>
                    </div>
                </div>

                <div class="form-group row">
                    <label for="lbStatus" class="col-sm-2 col-form-label">Status:</label>
                    <div class="col-sm-10">
                        <asp:Label ID="lbStatus" runat="server" Text=""></asp:Label>
                    </div>
                </div>

                <div class="form-group row">
                    <label for="txbdtPedido" class="col-sm-2 col-form-label">Data do Pedido:</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txbdtPedido" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </div>
                    <label for="txbdtCadastrado" class="col-form-label">Data Cadastrada:</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txbdtCadastrado" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group row">
                    <label for="txbEspecialidade" class="col-sm-2 col-form-label">Especialidade:</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txbEspecialidade" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <!-- SUBSTITUI: bloco do 'Solicitante' por 'Informações de Cargas em geral' -->
                <div class="form-group row">
                    <label class="col-sm-2 col-form-label">Informações de Cargas em geral:</label>
                    <div class="col-sm-10">
                        <div class="mb-2">Este formulário é relacionado a <strong>carga</strong>?</div>

                        <div class="rb-group">
                            <label class="rb-item">
                                <asp:RadioButton ID="rbCargaSim" runat="server" GroupName="CargaRel" Text="Sim" Enabled="false" />
                            </label>
                            <label class="rb-item">
                                <!-- Padrão: Não -->
                                <asp:RadioButton ID="rbCargaNao" runat="server" GroupName="CargaRel" Text="Não" Enabled="false" Checked="true" />
                            </label>
                        </div>
                        <small class="form-text text-muted mt-1">
                            Em páginas de visualização, os campos ficam somente leitura.
                        </small>
                    </div>
                </div>
                <!-- FIM SUBSTITUIÇÃO -->

                <div class="form-group">
                    <asp:Label ID="Label2" for="txbExamesSolicitados" runat="server" Text="Exames Solicitados:"></asp:Label>
                    <asp:TextBox ID="txbExamesSolicitados" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="form-group">
                    <asp:Label ID="Label1" for="txbOutrasInformacoes" runat="server" Text="Outras Informações:"></asp:Label>
                    <asp:TextBox ID="txbOutrasInformacoes" runat="server" Enabled="false" CssClass="form-control"
                        TextMode="MultiLine" Rows="5" required></asp:TextBox>
                </div>

                <div class="x_panel">
                    <div class="x_title">
                        <h2>Informações dos Exames
                            <asp:Label ID="Label4" runat="server" Text="" Style="color: Black"></asp:Label>
                        </h2>
                        <div class="clearfix"></div>
                    </div>
                    <div class="w-30 p-3">
                        <div>
                            <div class="col-xs-9 col-xs-9 col-xs-12">
                                <select ID="select2" multiple style="width:750px" runat="server" ClientIDMode="Static"></select>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="x_panel">
                    <div class="x_title">
                        <h2>Informações da Ressonancia
                            <asp:Label ID="Label3" runat="server" Text="" Style="color: Black"></asp:Label>
                        </h2>
                        <div class="clearfix"></div>
                    </div>
                    <div class="w-30 p-3">
                        <div>
                            <div class="col-xs-9 col-xs-9 col-xs-12">
                                <select ID="select1" multiple style="width:750px" runat="server" ClientIDMode="Static"></select>
                            </div>
                        </div>
                    </div>
                </div>

            </div><!-- body -->
        </div><!-- container -->
    </div><!-- x_panel -->
</asp:Content>
