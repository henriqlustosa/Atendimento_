﻿<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="arquivomarcado.aspx.cs" Inherits="encaminhamento_arquivomarcado" Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script src='<%= ResolveUrl("~/vendors/jquery/dist/jquery.js") %>'
        type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $("input").attr("autocomplete", "off");

            $('.numeric').keyup(function () {
                $(this).val(this.value.replace(/\D/g, ''));
            });

            function validaEspecialidade() {
                $(function () {
                    $('.valida').css("border", "1px solid red");
                });

            }
        });

        function validaCampo() {
            $(function () {
                $('.valida').css("border", "1px solid red");
            });
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <div class="x_panel">
        <div class="x_title">
            <h2>Cadastro
                <asp:Label ID="lbProntuario" runat="server" Text="" Style="color: Black"></asp:Label></h2>
            <div class="clearfix">
            </div>
        </div>
        <div class="container">
            <div class="body">
                <div class="form-group row">
                    <label for="txbNomePaciente" class="col-sm-2 col-form-label">
                        Nome Paciente:</label>
                    <div class="col-sm-5">
                        <asp:TextBox ID="txbNomePaciente" runat="server" Enabled="false" class="form-control"></asp:TextBox>
                    </div>
                    <label for="txbProntuario" class="col-sm-1 col-form-label">
                        Prontuário:</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txbProntuario" runat="server" Enabled="false" class="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group row">
                    <label for="lbCodPedido" class="col-sm-2 col-form-label">
                        Código Pedido:</label>
                    <div class="col-sm-10">
                        <asp:Label ID="lbCodPedido" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label for="lbStatus" class="col-sm-2 col-form-label">
                        Status:</label>
                    <div class="col-sm-10">
                        <asp:Label ID="lbStatus" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label for="txbdtPedido" class="col-sm-2 col-form-label">
                        Data do Pedido:</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txbdtPedido" runat="server" Enabled="false" class="form-control"></asp:TextBox>
                    </div>
                    <label for="txbdtCadastrado" class="col-form-label">
                        Data Cadastrada:</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txbdtCadastrado" runat="server" Enabled="false" class="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group row">
                    <label for="txbEspecialidade" class="col-sm-2 col-form-label">
                        Especialidade:</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txbEspecialidade" runat="server" Enabled="false" class="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group row">
                    <label for="txbSolicitante" class="col-sm-2 col-form-label">
                        Solicitante:</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txbSolicitante" runat="server" Enabled="false" class="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="Label2" for="txbExamesSolicitados" runat="server" Text="Exames Solicitados:"></asp:Label>
                    <asp:TextBox ID="txbExamesSolicitados" runat="server" Enabled="false" class="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label ID="Label1" for="txbOutrasInformacoes" runat="server" Text="Outras Informações:"></asp:Label>
                    <asp:TextBox ID="txbOutrasInformacoes" runat="server" Enabled="false" class="form-control"
                        TextMode="MultiLine" Rows="5" required></asp:TextBox>
                </div>
                <div class="x_panel">
                    <div class="x_title">
                        <h2>Informações dos Exames
                         <asp:Label ID="Label4" runat="server" Text="" Style="color: Black" ></asp:Label></h2>
                        <div class="clearfix">
                        </div>
                    </div>
                    <div class="w-30 p-3">

                        <div>
                            <div class="col-xs-9 col-xs-9 col-xs-12">
                                <asp:CheckBoxList ID="cblExame" CellSpacing="40" RepeatColumns="4" runat="server"  Enabled="false">
                                </asp:CheckBoxList>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
  
</asp:Content>
