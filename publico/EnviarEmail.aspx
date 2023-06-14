<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EnviarEmail.aspx.cs" Inherits="publico_EnviarEmail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="sendMessage" runat="server" Text="Enviar Email" 
            onclick="sendMessage_Click" />
    </div>
    </form>
</body>
</html>
