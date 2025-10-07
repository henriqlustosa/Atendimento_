<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" 
    CodeFile="cadencaminhamento.aspx.cs" Inherits="publico_cadencaminhamento" Title="HSPM ATENDIMENTO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <!-- CSS de plugins (Bootstrap e FontAwesome já estão na Master) -->
  <link href="https://cdn.jsdelivr.net/npm/icheck@1.0.2/skins/flat/green.css" rel="stylesheet" />
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-select@1.14.0-beta3/dist/css/bootstrap-select.min.css" />

  <style>
    body{background:#f8f9fa;font-family:'Segoe UI',sans-serif}
    .page-compact .x_panel{padding:12px 14px;margin-bottom:10px;background:#fff;border-radius:10px;box-shadow:0 1px 2px rgba(0,0,0,.05)}
    .page-compact .x_title{font-weight:600;font-size:1rem;margin-bottom:8px;display:flex;align-items:center;gap:8px}
    .page-compact .x_title i,
    .page-compact .card-header i,
    .page-compact .btn-link i{color:#0d6efd;}
    .page-compact h2{font-size:1.4rem;margin:0}
    .page-compact .form-control{height:36px;padding:6px 10px;font-size:.92rem;border-radius:6px!important}
    .page-compact textarea.form-control{min-height:82px;resize:vertical}
    .page-compact .card{margin-bottom:10px;border:0;border-radius:10px;box-shadow:0 1px 2px rgba(0,0,0,.05)}
    .page-compact .card-header{background:#fff;border:0;border-radius:10px;padding:.55rem .9rem}
    .page-compact .btn-link{padding:0;text-decoration:none;color:#111;display:flex;align-items:center;gap:10px;width:100%;text-align:left}
    .page-compact .chev{margin-left:auto;transition:transform .15s}
    .page-compact .btn-link[aria-expanded="true"] .chev{transform:rotate(180deg)}
    .page-compact .bootstrap-select>.dropdown-toggle{height:36px;padding:6px 10px;font-size:.92rem;border-radius:6px}
    .page-compact .bootstrap-select .dropdown-menu .inner{max-height:200px!important;overflow-y:auto!important}
    .bootstrap-select.no-search .bs-searchbox{display:none!important}
    .alert.alert-danger{margin:8px 0;padding:.5rem .75rem}
    .action-bar{position:sticky;bottom:0;z-index:10;background:rgba(248,249,250,.95);backdrop-filter:saturate(150%) blur(6px);
                border-top:1px solid #e9ecef;padding:.75rem 0;box-shadow:0 -2px 6px rgba(0,0,0,.05)}
    .action-bar-inner{display:flex;justify-content:center;}
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
<asp:ScriptManager ID="sm" runat="server" EnablePartialRendering="true" />

<div class="page-compact">
  <!-- Cabeçalho -->
  <div class="x_panel d-flex align-items-center" style="gap:12px;">
    <i class="fa-solid fa-stethoscope fa-xl"></i><h2>Cadastro de Solicitação de Exame</h2>
  </div>

  <!-- Resumo de erros -->
  <asp:ValidationSummary ID="vsSalvar" runat="server" CssClass="alert alert-danger"
    HeaderText="Por favor, corrija os campos abaixo:" ValidationGroup="Salvar" />

  <!-- Paciente -->
  <asp:UpdatePanel ID="upPaciente" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
      <div class="x_panel">
        <div class="x_title"><i class="fa fa-user"></i> Informações do Paciente</div>
        <div class="row">
          <div class="col-md-9">
            <label>Prontuário: *</label>
            <asp:TextBox ID="txbProntuario" runat="server" CssClass="form-control numeric" />
            <asp:RequiredFieldValidator ID="rfvProntuario" runat="server"
              ControlToValidate="txbProntuario" ErrorMessage="Informe o prontuário."
              Display="None" ValidationGroup="Salvar" />
          </div>
          <div class="col-md-3 d-flex align-items-end">
            <asp:Button ID="SearchButton" runat="server" Text="Pesquisar" CssClass="btn btn-primary w-100"
              OnClick="btnPesquisapaciente_Click" UseSubmitBehavior="false" />
          </div>
        </div>
        <div class="row mt-2">
          <div class="col-md-12">
            <label>Nome</label>
            <asp:TextBox ID="txbNomePaciente" runat="server" ReadOnly="true" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvNome" runat="server"
              ControlToValidate="txbNomePaciente" ErrorMessage="Pesquise o paciente para preencher o nome."
              Display="None" ValidationGroup="Salvar" />
          </div>
        </div>
      </div>
    </ContentTemplate>
    <Triggers><asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" /></Triggers>
  </asp:UpdatePanel>

  <!-- Accordion -->
  <div class="accordion" id="accAtendimento">

    <!-- Pedido -->
    <div class="card">
      <div class="card-header" id="hPedido">
        <button class="btn btn-link collapsed" type="button"
                data-bs-toggle="collapse" data-bs-target="#cPedido"
                aria-expanded="false" aria-controls="cPedido">
          <i class="fa fa-file-text"></i> Informações do Pedido <i class="fa fa-chevron-down chev"></i>
        </button>
      </div>
      <div id="cPedido" class="collapse" data-bs-parent="#accAtendimento">
        <div class="card-body">
          <div class="row">
            <div class="col-md-6">
              <label>Data do pedido *</label>
              <asp:TextBox ID="txbDtPedido" runat="server" CssClass="form-control"></asp:TextBox>
              <asp:RequiredFieldValidator ID="rfvData" runat="server"
                ControlToValidate="txbDtPedido" ErrorMessage="Informe a data do pedido."
                Display="None" ValidationGroup="Salvar" />
            </div>
            <div class="col-md-6">
              <label>Especialidade *</label>
              <asp:DropDownList ID="ddlEspecialidade" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                <asp:ListItem Text="Selecione..." Value=""></asp:ListItem>
              </asp:DropDownList>
              <asp:RequiredFieldValidator ID="rfvEsp" runat="server"
                ControlToValidate="ddlEspecialidade" InitialValue=""
                ErrorMessage="Selecione a especialidade." Display="None" ValidationGroup="Salvar" />
            </div>
          </div>
          <div class="row mt-2">
            <div class="col-12">
              <asp:TextBox ID="txbOb" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Pré-Operatório -->
    <div class="card">
      <div class="card-header" id="hPre">
        <button class="btn btn-link collapsed" type="button"
                data-bs-toggle="collapse" data-bs-target="#cPre"
                aria-expanded="false" aria-controls="cPre">
          <i class="fa fa-plus-square"></i> Informações do Pré Operatório <i class="fa fa-chevron-down chev"></i>
        </button>
      </div>
      <div id="cPre" class="collapse" data-bs-parent="#accAtendimento">
        <div class="card-body">
          <select id="select2" multiple runat="server" clientidmode="Static"
                  class="selectpicker form-control exam-select" data-exam="true"
                  data-size="6" data-container="body" data-dropup-auto="false"
                  data-live-search="true" title="Selecione uma ou mais opções"></select>
        </div>
      </div>
    </div>

    <!-- Ressonância -->
    <div class="card">
      <div class="card-header" id="hRes">
        <button class="btn btn-link collapsed" type="button"
                data-bs-toggle="collapse" data-bs-target="#cRes"
                aria-expanded="false" aria-controls="cRes">
          <i class="fa fa-vials"></i> Informações de Ressonância <i class="fa fa-chevron-down chev"></i>
        </button>
      </div>
      <div id="cRes" class="collapse" data-bs-parent="#accAtendimento">
        <div class="card-body">
          <select id="select1" multiple runat="server" clientidmode="Static"
                  class="selectpicker form-control exam-select" data-exam="true"
                  data-size="6" data-container="body" data-dropup-auto="false"
                  data-live-search="true" title="Selecione uma ou mais opções"></select>
        </div>
      </div>
    </div>

    <!-- Teleconsulta -->
    <div class="card">
      <div class="card-header" id="hTele">
        <button class="btn btn-link collapsed" type="button"
                data-bs-toggle="collapse" data-bs-target="#cTele"
                aria-expanded="false" aria-controls="cTele">
          <i class="fa fa-phone"></i> Informações de Teleconsulta <i class="fa fa-chevron-down chev"></i>
        </button>
      </div>
      <div id="cTele" class="collapse" data-bs-parent="#accAtendimento">
        <div class="card-body">
          <select id="select3" multiple runat="server" clientidmode="Static"
                  class="selectpicker form-control exam-select" data-exam="true"
                  data-size="6" data-container="body" data-dropup-auto="false"
                  data-live-search="true" title="Selecione uma ou mais opções"></select>
        </div>
      </div>
    </div>

    <!-- Exames Únicos -->
    <div class="card">
      <div class="card-header" id="hEx">
        <button class="btn btn-link collapsed" type="button"
                data-bs-toggle="collapse" data-bs-target="#cEx"
                aria-expanded="false" aria-controls="cEx">
          <i class="fa fa-vial"></i> Informações de Exames Únicos <i class="fa fa-chevron-down chev"></i>
        </button>
      </div>
      <div id="cEx" class="collapse" data-bs-parent="#accAtendimento">
        <div class="card-body">
          <select id="select4" multiple runat="server" clientidmode="Static"
                  class="selectpicker form-control exam-select" data-exam="true"
                  data-size="6" data-container="body" data-dropup-auto="false"
                  data-live-search="true" title="Selecione uma ou mais opções"></select>
        </div>
      </div>
    </div>

    <!-- Validação: pelo menos um exame -->
    <asp:CustomValidator ID="cvAlgumExame" runat="server"
      ClientValidationFunction="validateAlgumExame"
      OnServerValidate="cvAlgumExame_ServerValidate"
      ErrorMessage="Selecione pelo menos um item em Pré-operatório, Ressonância, Teleconsulta ou Exames Únicos."
      Display="None" ValidationGroup="Salvar" ValidateEmptyText="true" />

    <!-- Solicitante -->
    <div class="card">
      <div class="card-header" id="hSol">
        <button class="btn btn-link collapsed" type="button"
                data-bs-toggle="collapse" data-bs-target="#cSol"
                aria-expanded="false" aria-controls="cSol">
          <i class="fa fa-user-md"></i> Informações do Solicitante <i class="fa fa-chevron-down chev"></i>
        </button>
      </div>
      <div id="cSol" class="collapse" data-bs-parent="#accAtendimento">
        <div class="card-body">
          <label>Médico/Profissional</label>
          <asp:TextBox ID="txbprofissional" runat="server" CssClass="form-control" placeholder="Digite o nome do profissional"></asp:TextBox>
        </div>
      </div>
    </div>

  </div><!-- /accordion -->

  <!-- BOTÃO GRAVAR FIXO -->
  <div class="action-bar">
    <div class="action-bar-inner">
      <asp:Button ID="btnGravar" runat="server" Text="Gravar"
        CssClass="btn btn-success btn-lg shadow-sm"
        ValidationGroup="Salvar" CausesValidation="true"
        OnClientClick="return validateBeforeSubmit();"
        OnClick="btnGravar_Click" />
    </div>
  </div>

  <!-- Modal sucesso (BS5) -->
  <div class="modal fade" id="myModal" tabindex="-1" aria-hidden="true" data-bs-keyboard="false" data-bs-backdrop="static">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">Cadastro</h5></div>
        <div class="modal-body text-center"><h2>Pedido Cadastrado.</h2></div>
        <div class="modal-footer"><button type="button" id="btnCloseModal" class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button></div>
      </div>
    </div>
  </div>
</div>

<!-- Scripts de plugins (jQuery/Bootstrap já vêm da Master) -->
<script src="https://cdn.jsdelivr.net/npm/icheck@1.0.2/icheck.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap-select@1.14.0-beta3/dist/js/bootstrap-select.min.js"></script>

<script>
    function getSelectedValue($el) {
        try { if ($el.data('selectpicker')) { var v = $el.selectpicker('val'); if (v != null) return v; } }
        catch (e) { }
        return $el.val();
    }
    function anyExamSelected() {
        var ok = false;
        $('[data-exam]').each(function () {
            var v = getSelectedValue($(this));
            if (v && ($.isArray(v) ? v.length > 0 : $.trim(v).length > 0)) { ok = true; return false; }
        });
        return ok;
    }
    function validateAlgumExame(sender, args) { args.IsValid = anyExamSelected(); }

    function validateBeforeSubmit() {
        if (!Page_ClientValidate('Salvar')) {
            var invalid = null;
            if (typeof (Page_Validators) !== 'undefined') {
                for (var i = 0; i < Page_Validators.length; i++) {
                    var v = Page_Validators[i];
                    if (v && v.validationGroup === 'Salvar' && v.isvalid === false) {
                        invalid = document.getElementById(v.controltovalidate); break;
                    }
                }
            }
            if (invalid) {
                var $col = $(invalid).closest('.collapse');
                if ($col.length && !$col.hasClass('show')) {
                    bootstrap.Collapse.getOrCreateInstance($col[0], { toggle: false }).show();
                }
                setTimeout(function () {
                    try { invalid.focus(); } catch (e) { }
                    $('html,body').animate({ scrollTop: $(invalid).offset().top - 120 }, 180);
                }, 80);
            }
            return false;
        }
        return true;
    }

    $(function () {
        // datepicker
        flatpickr("#<%= txbDtPedido.ClientID %>", {
          dateFormat: "d/m/Y",
          locale: {
              firstDayOfWeek: 1,
              weekdays: { shorthand: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'], longhand: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'] },
              months: { shorthand: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'], longhand: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'] }
          }
      });

      // apenas números
      $('.numeric').on('input', function () { this.value = this.value.replace(/\D/g, ''); });

      // iCheck
      $('input[type="checkbox"], input[type="radio"]').iCheck({
          checkboxClass: 'icheckbox_flat-green',
          radioClass: 'iradio_flat-green',
          increaseArea: '20%'
      });

      // bootstrap-select
      $('.selectpicker').each(function () {
          var $s = $(this);
          if (!$s.data('selectpicker')) $s.selectpicker({ dropupAuto: false, liveSearch: true });
          dedupeOptionsByText($s); toggleSearchBox($s); ensureUniqueSelection($s);
          $s.selectpicker('render');
      });

      // acordeão exclusivo (fecha outros quando um abre)
      $('#accAtendimento').on('show.bs.collapse', function (e) {
          $('#accAtendimento .collapse.show').not(e.target).each(function () {
              bootstrap.Collapse.getOrCreateInstance(this, { toggle: false }).hide();
          });
      }).on('shown.bs.collapse', function (e) {
          $(e.target).find('select.selectpicker').each(function () {
              var $s = $(this); toggleSearchBox($s); ensureUniqueSelection($s); $s.selectpicker('render');
          });
          $('html,body').animate({ scrollTop: $(e.target).closest('.card').offset().top - 16 }, 180);
      });

      // garantir seleção única no selectpicker
      $(document).on('changed.bs.select', 'select.selectpicker', function () {
          var $s = $(this); ensureUniqueSelection($s); $s.selectpicker('render');
      });

      function toggleSearchBox($s) { $s.parent('.bootstrap-select').toggleClass('no-search', $s.find('option').length < 8); }
      function ensureUniqueSelection($s) {
          var seen = {};
          $s.find('option:selected').each(function () {
              var k = $(this).text().trim().toLowerCase();
              if (seen[k]) this.selected = false; else seen[k] = true;
          });
      }
      function dedupeOptionsByText($s) {
          var map = {};
          $s.find('option').each(function () {
              var t = $(this).text().trim().toLowerCase();
              if (map[t]) $(this).remove(); else map[t] = true;
          });
      }

      $('#btnCloseModal').on('click', function () { location.href = 'cadencaminhamento.aspx'; });
  });
</script>
</asp:Content>
