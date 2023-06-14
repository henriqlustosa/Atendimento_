using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net.Mail;

public partial class publico_EnviarEmail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void sendMessage_Click(object sender, EventArgs e)
    {
        string nome = "Sr. Luciano";
        string email = "lbito@gmail.com";
        string mensagem = "Envio de email via asp.net/c#";


        System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
        client.Host = "smtp.gmail.com";
        client.EnableSsl = true;
        client.Credentials = new System.Net.NetworkCredential("seu email", "sua senha");
        MailMessage mail = new MailMessage();
        mail.Sender = new System.Net.Mail.MailAddress("email que vai enviar", "ENVIADOR");
        mail.From = new MailAddress("de quem", "ENVIADOR");
        mail.To.Add(new MailAddress("paraquem", "RECEBEDOR"));
        mail.Subject = "Contato";
        //mail.Body = " Mensagem do site:<br/> Nome:  " + senderName.Text + "<br/> Email : " + senderEmail.Text + " <br/> Mensagem : " + message.Text;
        mail.IsBodyHtml = true;
        mail.Priority = MailPriority.High;
        try
        {
            client.Send(mail);
        }
        catch (System.Exception erro)
        {
            //trata erro
        }
        finally
        {
            mail = null;
        }
    }
}
