<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="buscaNaTabela.aspx.cs" Inherits="publico_buscaNaTabela" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">

 <table id="table_id" class="display">
        <thead>
            <tr>
                <th>Nome</th>
                <th>Sobrenome</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Jose</td>
                <td>da Silva</td>
            </tr>
            <tr>
                <td>Mané</td>
                <td>Medeiros</td>
            </tr>
        </tbody>
    </table>
 <script src='<%= ResolveUrl("https://cdnjs.cloudflare.com/ajax/libs/jquery/2.2.4/jquery.min.js") %>'
        type="text/javascript"></script>
  <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.20/css/jquery.dataTables.css" />
  <script src='<%= ResolveUrl("https://cdn.datatables.net/1.10.20/js/jquery.dataTables.js") %>' type="text/javascript"></script>



<script type="text/javascript"> 
   $(document).ready(function() {
        $.noConflict();
        var table = $('#table_id').DataTable();
    });
 </script>


</asp:Content>

