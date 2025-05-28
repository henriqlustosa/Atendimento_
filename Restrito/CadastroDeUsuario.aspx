<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="CadastroDeUsuario.aspx.cs" Inherits="Restrito_CadastroDeUsuario" Title="Call HSPM" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        .form-label { display: block; margin-top: 10px; font-weight: bold; }
        .form-control { width: 300px; padding: 5px; }
        .btn { margin-top: 15px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="container">
        <h2>Cadastro de Usuário</h2>
        <div>
            <label class="form-label">Login de Rede:</label>
            <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" />
            <asp:Button ID="btnBuscarAD" runat="server" Text="Pesquisar no AD" CssClass="btn btn-outline-primary" OnClick="btnBuscarAD_Click" />
        </div>

        <div>
            <label class="form-label">Nome:</label>
            <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
        </div>

        <div>
            <label class="form-label">Email:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
        </div>

        <div>
            <label class="form-label">Perfis de Acesso:</label>
            <asp:CheckBoxList ID="cblPerfis" runat="server" RepeatDirection="Vertical" />
        </div>

        <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn btn-outline-primary" OnClick="btnSalvar_Click" />
        <br /><br />

        <asp:Label ID="lblResultado" runat="server" ForeColor="Green" />

        <hr />
<h3>Usuários Cadastrados</h3>
<asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" 
    DataKeyNames="LoginRede" OnRowCommand="gvUsuarios_RowCommand" CssClass="table table-bordered">
    <Columns>
        <asp:BoundField DataField="LoginRede" HeaderText="Login" />
        <asp:BoundField DataField="NomeCompleto" HeaderText="Nome" />
        <asp:BoundField DataField="Email" HeaderText="Email" />

        <asp:TemplateField HeaderText="Ações">
            <ItemTemplate>
                <asp:Button ID="btnExcluir" runat="server" CommandName="Excluir" 
                    CommandArgument='<%# Eval("LoginRede") %>' Text="Excluir" 
                    OnClientClick="return confirm('Tem certeza que deseja excluir este usuário?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>


    </div>
</asp:Content>
