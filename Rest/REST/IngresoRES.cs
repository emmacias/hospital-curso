using Comun.ViewModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rest.REST
{
    public class IngresoRES
    {
        private const string url = "https://minegocioefectivo.com/hospital/api";

        /// <summary>
        /// Obtiene una sala y una cama disponibles en el hospital
        /// </summary>
        /// <returns></returns>
        public static SalaCamaVMR ObtenerSalaYCama()
        {
            SalaCamaVMR result = null;

            RestClient rest = new RestClient(url);

            var request = new RestRequest("/hospital", Method.Get);

            var response = rest.Execute(request);

            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Error al obtener respuesta del servicio.");
            }

            try
            {
                var salaCama = JsonConvert.DeserializeObject<RespuestaVMR<SalaCamaVMR>>(response.Content);

                if (salaCama.codigo != HttpStatusCode.OK)
                {
                    throw new Exception(string.Join(", ", salaCama.mensajes));
                }

                result = salaCama.datos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al deserializar. [{response.Content}]", ex);
            }

            return result;
        }
    }
}
