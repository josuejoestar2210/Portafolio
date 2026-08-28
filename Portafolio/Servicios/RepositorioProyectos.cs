using Portafolio.Models;

namespace Portafolio.Servicios
{
    public interface IRepositorioProyectos
    {
        List<Proyecto> ObtenerProyectos();
    }
    public class RepositorioProyectos : IRepositorioProyectos
    {
        public List<Proyecto> ObtenerProyectos()
        {
            return new List<Proyecto>
            {
                   new Proyecto
                   {
                       Titulo = "JoJoApp Anime",
                       Descripcion = "Aplicación para explorar anime, desarrollada con TypeScript",
                       Link = "https://jojoapp-anime.netlify.app",
                       ImagenURL = "/imagenes/jojoAnimeApp.png"
                   }
            };
        }
    }
}
