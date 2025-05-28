<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Atendimento HSPM</title>
    <!-- Bootstrap -->
    <link href="vendors/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <!-- Font Awesome -->
    <link href="vendors/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <!-- NProgress -->
    <link href="vendors/nprogress/nprogress.css" rel="stylesheet" type="text/css" />
    <!-- Animate.css -->
    <link href="vendors/animate.css/animate.min.css" rel="stylesheet" type="text/css" />
    <!-- Custom Theme Style -->
    <link href="build/css/custom.min.css" rel="stylesheet" type="text/css" />
</head>
<body class="login">
<body>
    <form id="form1" runat="server">
      <div class="login-container">
    <div class="login-box">
        <h2>Login</h2>
        <asp:Label ID="lblUsuario" runat="server" Text="Usuário:"></asp:Label><br />
        <asp:TextBox ID="txtUsuario" runat="server"></asp:TextBox><br />

        <asp:Label ID="lblSenha" runat="server" Text="Senha:"></asp:Label><br />
        <asp:TextBox ID="txtSenha" runat="server" TextMode="Password"></asp:TextBox><br /><br />

        <asp:Button ID="btnLogin" runat="server" Text="Entrar" OnClick="btnLogin_Click" /><br /><br />
        <asp:Label ID="lblMensagem" runat="server" ForeColor="Red"></asp:Label>
    </div>
</div>

           <div >
          
                  <h1>HSPM</h1>
                  <p>©2025 HSPM - Hospital do Servidor Público Municipal</p>
                </div>
           </form>
</body>
</html>
