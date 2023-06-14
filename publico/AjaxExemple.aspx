<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="AjaxExemple.aspx.cs" Inherits="publico_AjaxExemple" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">
     function GetData() {
         $.ajax({
             type: 'POST'
             //Caminho do WebService + / + nome do metodo
                , url: '<%= ResolveUrl("~/publico/webservice.asmx/RetornaChamadasDia") %>'
             //, url: '<%= ResolveUrl("~/publico/webservice.asmx/RetornaJSON") %>'
                , contentType: 'application/json; charset=utf-8'
                , dataType: 'json'
             //Adicionando a palavra e o número de repetições.
             //, data: "{word:'Cassiano', n:'5'}"

             //, data: "{word:'" + escape($("#palavra").val()) + "', n:'" + $("#n").val() + "'}"

                , success: function(data, status) {
                    //Tratando o retorno com parseJSON
                    var itens = $.parseJSON(data.d);
                    //Alert com o primeiro item
                    //alert(itens[0]);
                    //Respondendo na tela todos os itens
                    $('.resultado').text(data.d);

                }
                , error: function(xmlHttpRequest, status, err) {
                    //Caso ocorra algum erro:
                    $('.resultado').html('Ocorreu um erro');
                }
         });
     }
  
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    
    <div>
        <div>
            <strong>Palavra:</strong>
        </div>
        <div>
            <input type="text" id="palavra" />
        </div>
        <div>
            <strong>Número de repetições:</strong>
        </div>
        <div>
            <input type="text" id="n" />
        </div>
        <div>
            <input type="button" onclick="GetData();" value="Executar" />
        </div>
        <div>
            <strong>resultado:</strong><span class="resultado">Null</span>
        </div>
    </div>
</asp:Content>
