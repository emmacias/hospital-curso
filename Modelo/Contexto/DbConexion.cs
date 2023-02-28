using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    /// <summary>
    /// Clase extendida del contexto para personalizar propiedades y métodos de la conexión a la base de datos.
    /// </summary>
    public partial class DbConexion : DbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ContextName"></param>
        private DbConexion(string ContextName)
            : base(ContextName)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Database.CommandTimeout = 900;
        }

        /// <summary>
        /// Creación de un objeto de la conexión.
        /// </summary>
        /// <returns>DbConexion</returns>
        public static DbConexion Create()
        {
            return new DbConexion("name=DbConexion");
        }

        /// <summary>
        /// Personalización del método de persistencia para mejorar los mensajes de error.
        /// </summary>
        /// <returns>int</returns>
        /// <exception cref="DbEntityValidationException"></exception>
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Recuperar mensajes de error
                var errorMessages = ex.EntityValidationErrors
                                    .SelectMany(x => x.ValidationErrors)
                                    .Select(x => x.ErrorMessage);

                // Unir los mensajes de error en una sola cadena
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combinar el mensaje de excepción original con el nuevo
                var exceptionMessage = string.Concat(ex.Message, " Los errores de validación son: ", fullErrorMessage);

                // Lanzar una nueva DbEntityValidationException con el mensaje de excepción mejorado
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}
