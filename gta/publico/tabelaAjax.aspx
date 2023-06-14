<%@ Page Language="C#" AutoEventWireup="true" CodeFile="tabelaAjax.aspx.cs" Inherits="publico_tabelaAjax" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link href="../build/css/bootstrap.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="statusB" class="table table-bordered table-condensed table-hover table-striped">
            <thead>
                <tr>
                    <th>
                        Descricao
                    </th>
                    <th>
                        Quantidade
                    </th>
                </tr>
            </thead>
            <tbody id="tdata">
            </tbody>
        </table>
    </div>
    </form>
    <!-- jQuery -->

    <script src='<%= ResolveUrl("~/vendors/jquery/dist/jquery.js") %>' type="text/javascript"></script>

    <!-- Bootstrap -->

    <script src='<%= ResolveUrl("~/vendors/bootstrap/dist/js/bootstrap431.js") %>' type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            dadosMesStatus();
        });

        function dadosMesStatus() {
            $.ajax({
                async: false
                                , url: '<%= ResolveUrl("~/publico/webservice.asmx/QuantidadeAtivosStatus") %>'
                                , data: '{mes : 01, ano: 2020}'
                                , type: 'POST'
                                , contentType: 'application/json; charset=utf-8'
                                , dataType: 'json'
                                , success: function(data) {
                                    var data = JSON.parse(data.d);
                                    data.forEach(function(dt) {
                                        $("#tdata").append("<tr>" +
                                        "<td>" + dt.descricao + "</td>" +
                                        "<td>" + dt.quantidade + "</td>"
                                        + "</tr>"
                                        );
                                    });

                                }
                                , error: function(xhr, er) {
                                    $("#lbMsg").html('<p> Erro ' + xhr.staus + ' - ' + xhr.statusText + ' - <br />Tipo de erro:  ' + er + '</p>');
                                }
            });
        }
         
    </script>

</body>
</html>
