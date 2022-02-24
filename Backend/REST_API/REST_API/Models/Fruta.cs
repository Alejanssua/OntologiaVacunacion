// using System.Collections.Generic;

// namespace Models
// {
//     public class Fruta : RecursoRDF
//     {
//         public string nombre_cientifico;
//         public string nombre_Comun;
//         public List<string> colores;
//         public float agua;
//         public List<Minerales> mineral;
//         public List<string> region;
//         public string textura;
//         public string sabor; 
//         public List<Vitamina> vitamina; 
//     }
// }

using System.Collections.Generic;

namespace Models
{
    public class Persona : RecursoRDF
    {
        public string nombre;
        public string status_vacunacion;
        public string dosis_aplicadas;
        public string ciudad;
        public List<Enfermedad> enfermedad;
        public List<Vacuna> vacuna; 
    }
}