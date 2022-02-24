using Models;
using System.Collections.Generic;
using VDS.RDF.Storage;
using VDS.RDF;
using VDS.RDF.Query;
using System;

namespace DataSource
{
    public class Ontologia
    {
        private static string baseURL = @"http://localhost:3030/vacunacion/data";
        public static List<RecursoRDF> resources;

        public Ontologia()
        {
            if (Ontologia.resources == null)
            {
                Ontologia.resources = new List<RecursoRDF>();
                Persona f = new Persona();

                f.status_vacunacion = "Vacunacion Incompleta";
                f.ciudad = "Salcedo";
                f.nombre = "Alejandro Salazar";
                f.dosis_aplicadas = "1";
                f.recurso = "Hombre_1";
                f.type = "Persona";

                Enfermedad m = new Enfermedad();
                m.recurso = "Diabetes";
                m.nombre = "Diabetes Mellitus";
                m.type = "Enfermedad";
                f.enfermedad = new List<Enfermedad>();
                f.enfermedad.Add(m);



                Vacuna v = new Vacuna();
                v.descripcion = "Edades recomendadas: 18 años o mas";
                v.recurso = "Jhonson&Jhonson";
                v.nombre = "Jhonson & Jhonson";
                v.vacunas_disponibles = "100";
                v.type = "Vacuna";
                f.vacuna = new List<Vacuna>();
                f.vacuna.Add(v);

                Ontologia.resources.Add(f);
                Ontologia.resources.Add(m);
                Ontologia.resources.Add(v);
            }
        }
        /// <summary>
        /// Proceso de Busqueda y filtrado de datos en ontología
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Persona> consulta(Persona param)
        {
            List<Persona> resultado = null;
            SparqlParameterizedString query = new SparqlParameterizedString();
            FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);
            query.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            query.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            query.Namespaces.AddNamespace("data", new Uri("http://localhost:3030/vacunacion/data#"));
            query.Namespaces.AddNamespace("xsd", new Uri("http://www.w3.org/2001/XMLSchema#"));

            query.CommandText = "SELECT ?persona WHERE { ";

            if (!string.IsNullOrEmpty(param.ciudad))
            {
                query.CommandText += " ?persona data:deCiudad \"" + param.ciudad + "\"^^ xsd:string . ";
            }

            if (!string.IsNullOrEmpty(param.status_vacunacion))
            {
                query.CommandText += " ?persona data:StatusVacunacion \"" + param.status_vacunacion + "\"^^ xsd:string . ";
            }

            if (param.enfermedad != null && param.enfermedad.Count > 0)
            {
                foreach (var item in param.enfermedad)
                {
                    query.CommandText += "   ?persona data:tiene_Enfermedad_Cronica data:" + item.recurso + ". ";
                }
            }

            if (param.vacuna != null && param.vacuna.Count > 0)
            {
                foreach (var item in param.vacuna)
                {
                    query.CommandText += "   ?persona data:se_aplica data:" + item.recurso + ". ";
                }
            }

            if (!string.IsNullOrEmpty(param.dosis_aplicadas))
            {
                query.CommandText += " ?persona data:NroDosisAplicadas \"" + param.dosis_aplicadas + "\"^^xsd:string . ";
            }


            query.CommandText += " }";

            object e = pst.ExecuteQuery(query.ToString());

            try
            {
                SparqlResultSet res1 = (SparqlResultSet)e;
                if (!res1.IsEmpty)
                {
                    resultado = new List<Persona>();
                    foreach (var item in res1.Results)
                    {
                        Persona curr = readFruta(getResourceString(item[0].ToString()));
                        if (curr != null)
                        {
                            resultado.Add(curr);
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return resultado;
        }
        /// <summary>
        /// Obtener el recurso especificado según el recurso en cadena de texto
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public RecursoRDF getRecurso(string resource)
        {
            FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);

            SparqlParameterizedString query = new SparqlParameterizedString();
            query.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            query.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            query.Namespaces.AddNamespace("data", new Uri("http://localhost:3030/vacunacion/data#"));

            query.CommandText = "SELECT DISTINCT  ?d WHERE { data:" + resource + " a ?class . ?class rdfs:subClassOf ?d } ";
            Console.Write(query.ToString());
            object e = pst.ExecuteQuery(query.ToString());


            if (e is SparqlResultSet)
            {
                SparqlResultSet res1 = (SparqlResultSet)e;
                string a;
                if (res1.IsEmpty)
                {
                    a = "Enfermedad";
                }
                else
                {
                    a = (string)res1[0][0].ToString();
                    a = getResourceString(a);
                }
                switch (a)
                {
                    case "Vacuna":
                        return readVitamina(resource);
                    case "Enfermedad":
                        return readMineral(resource);
                    case "Persona":
                        return readFruta(resource);
                }
            }
            return null;
        }

        public bool crearFruta(Persona res)
        {
            if (res != null)
            {
                Ontologia.resources.Add(res);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Proceso de eliminación de nodo RDF/OWL en el servidor Fuseki
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public bool deleteFruta(string res)
        {
            try
            {
                RecursoRDF ress = getRecurso(res);
                if (ress != null)
                {
                    FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
                    fuseki.DeleteGraph(new Uri(Ontologia.baseURL + "#" + res));
                    return true;
                }
            }
            catch (Exception e)
            {
            }
            return false;
        }
        /// <summary>
        /// Lectura de recurso de tipo Fruta
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private Persona readFruta(string resource)
        {
            Persona res = null;
            FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);

            SparqlParameterizedString query = new SparqlParameterizedString();
            query.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            query.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            query.Namespaces.AddNamespace("data", new Uri("http://localhost:3030/vacunacion/data#"));

            query.CommandText = "Select distinct ?nombre ?status_vacunacion ?dosis_aplicadas ?ciudad ?enfermedad ?vacuna " +
                                "Where {" +
                                "  data:" + resource + " data:nombrePersona ?nombre. " +
                                "  data:" + resource + " data:StatusVacunacion ?status_vacunacion. " +
                                "  data:" + resource + " data:NroDosisAplicadas ?dosis_aplicadas. " +
                                "  data:" + resource + " data:deCiudad ?ciudad. " +
                                "  OPTIONAL {data:" + resource + " data:tiene_Enfermedad_Cronica ?enfermedad. }" +
                                "  OPTIONAL {data:" + resource + " data:se_aplica ?vacuna. }" +
                                "}";
            Console.Write(query.ToString());
            object e = pst.ExecuteQuery(query.ToString());

            try
            {
                SparqlResultSet res1 = (SparqlResultSet)e;
                if (!res1.IsEmpty)
                {
                    res = new Persona();

                    var item = res1[0];
                    res.recurso = resource;
                    res.type = "Persona";
                    res.nombre = (string)item[0].ToString();
                    res.status_vacunacion = (string)item[1].ToString();
                    res.dosis_aplicadas = (string)item[2].ToString();
                    res.ciudad = (string)item[3].ToString();
                    res.enfermedad = getMinerales(resource);
                    res.vacuna = getVitaminas(resource);

                }
            }
            catch (Exception ex) { }

            return res;
        }
        /// <summary>
        /// Lectura de lista de Vitaminas para un recurso fruta dado
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        private List<Vacuna> getVitaminas(string resources)
        {
            List<Vacuna> res = null;
            FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);

            SparqlParameterizedString query = new SparqlParameterizedString();
            query.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            query.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            query.Namespaces.AddNamespace("data", new Uri("http://localhost:3030/vacunacion/data#"));

            query.CommandText = " Select ?vacuna Where { data:" + resources + " data:se_aplica ?vacuna. }";

            Console.Write(query.ToString());
            object e = pst.ExecuteQuery(query.ToString());

            try
            {
                SparqlResultSet res1 = (SparqlResultSet)e;
                if (!res1.IsEmpty)
                {
                    res = new List<Vacuna>();
                    foreach (var item in res1.Results)
                    {
                        Vacuna curr = readVitamina(getResourceString(item[0].ToString()));
                        if (curr != null)
                        {
                            res.Add(curr);
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return res;
        }
        /// <summary>
        /// Lectura de lista de minerales para un recurso fruta dado
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        private List<Enfermedad> getMinerales(string resources)
        {
            List<Enfermedad> res = null;
            FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);

            SparqlParameterizedString query = new SparqlParameterizedString();
            query.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            query.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            query.Namespaces.AddNamespace("data", new Uri("http://localhost:3030/vacunacion/data#"));

            query.CommandText = " Select ?enfermedad  Where { data:" + resources + " data:tiene_Enfermedad_Cronica ?enfermedad. }";

            Console.Write(query.ToString());
            object e = pst.ExecuteQuery(query.ToString());

            try
            {
                SparqlResultSet res1 = (SparqlResultSet)e;
                if (!res1.IsEmpty)
                {
                    res = new List<Enfermedad>();
                    foreach (var item in res1.Results)
                    {
                        Enfermedad curr = readMineral(getResourceString(item[0].ToString()));
                        if (curr != null)
                        {
                            res.Add(curr);
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return res;
        }
        /// <summary>
        /// Lectura de recurso de tipo Mineral
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private Enfermedad readMineral(string resource)
        {
            Enfermedad res = null;
            FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);

            SparqlParameterizedString query = new SparqlParameterizedString();
            query.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            query.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            query.Namespaces.AddNamespace("data", new Uri("http://localhost:3030/vacunacion/data#"));

            query.CommandText = "Select ?nombre " +
                                    "Where { " +
                                        "data:" + resource + " data:nombreEnfermedadCronica ?nombre. " +
                                    "}";

            Console.Write(query.ToString());
            object e = pst.ExecuteQuery(query.ToString());

            try
            {
                SparqlResultSet res1 = (SparqlResultSet)e;
                if (!res1.IsEmpty)
                {
                    res = new Enfermedad();
                    var item = res1[0];
                    res.type = "Enfermedad";
                    res.recurso = resource;
                    res.nombre = (string)item[0].ToString();
                }
            }
            catch (Exception ex) { }

            return res;
        }
        /// <summary>
        /// Lectura de recurso de tipo Vitamina
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private Vacuna readVitamina(string resource)
        {
            Vacuna res = null;
            FusekiConnector fuseki = new FusekiConnector(Ontologia.baseURL);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);

            SparqlParameterizedString query = new SparqlParameterizedString();
            query.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            query.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            query.Namespaces.AddNamespace("data", new Uri("http://localhost:3030/vacunacion/data#"));

            query.CommandText = "Select ?nombre ?descripcion ?vacunas_disponibles " +
                                    "Where { " +
                                    "  data:" + resource + " data:nombreVacuna ?nombre. " +
                                    "  data:" + resource + " data:descripcionVacuna ?descripcion. " +
                                    "  data:" + resource + " data:CantidadVacunasDisponibles ?vacunas_disponibles. " +
                                    "}";
            Console.Write(query.ToString());
            object e = pst.ExecuteQuery(query.ToString());

            try
            {
                SparqlResultSet res1 = (SparqlResultSet)e;
                if (!res1.IsEmpty)
                {
                    res = new Vacuna();

                    var item = res1[0];
                    res.recurso = resource;
                    res.type = "Vacuna";
                    res.nombre = (string)item[0].ToString();
                    res.descripcion = (string)item[1].ToString();
                    res.vacunas_disponibles = (string)item[2].ToString();

                }

            }
            catch (Exception ex) { }

            return res;
        }
        /// <summary>
        /// Obtiene el recurso sin URL/URI
        /// </summary>
        /// <param name="rawResource"></param>
        /// <returns></returns>
        public string getResourceString(string rawResource)
        {
            return rawResource.Substring(rawResource.IndexOf("#") + 1);
        }
    }
}