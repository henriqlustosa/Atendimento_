using System;
using System.IO;
using System.Net;
using Hspm.CadEncaminhamento.Domain;
using Newtonsoft.Json;

namespace Hspm.CadEncaminhamento.Infrastructure
{
    // Classe auxiliar que reflete o JSON da API
    internal sealed class PacienteApiResponse
    {
        public string Nm_nome { get; set; }
    }

    public sealed class PacienteGatewayApi : IPacienteGateway
    {
        private readonly string _baseUrl;

        public PacienteGatewayApi(string baseUrl)
        {
            _baseUrl = (baseUrl ?? string.Empty).TrimEnd('/');
        }

        public PacienteDto ObterPorProntuario(int prontuario)
        {
            string uri = _baseUrl + "/pacientes/paciente/" + prontuario;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Method = "GET";
                req.Timeout = 15000; // 15s (ajuste se quiser)

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    if (resp.StatusCode != HttpStatusCode.OK)
                        return null;

                    using (Stream s = resp.GetResponseStream())
                    using (StreamReader sr = new StreamReader(s))
                    {
                        string json = sr.ReadToEnd();

                        // Deserializa de forma tipada (sem dynamic)
                        PacienteApiResponse api = JsonConvert.DeserializeObject<PacienteApiResponse>(json);
                        if (api == null || string.IsNullOrEmpty(api.Nm_nome))
                            return null;

                        PacienteDto dto = new PacienteDto();
                        dto.Nome = api.Nm_nome;
                        return dto;
                    }
                }
            }
            catch (WebException)
            {
                // TODO: logar se necessário
                return null;
            }
            catch (Exception)
            {
                // TODO: logar se necessário
                return null;
            }
        }
    }
}
