<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Relatorio1.aspx.cs" Inherits="publico_Relatorio1" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="well" style="overflow: auto">
                <div class="col-md-3">
                    <asp:DropDownList ID="ddl" runat="server" 
                        OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="True">
                    </asp:DropDownList>
                    <asp:Label ID="lbData" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
