
export class Persona {
  ciudad: string;
  nombre: string;
  dosis_aplicadas: string;
  status_vacunacion: string;
  vacuna: Vacuna[];
  enfermedad: Enfermedad[];
  recurso: string;
  clases: string[];
  type: string;

  constructor(){
    this.ciudad = "";
    this.nombre = "";
    this.dosis_aplicadas = "";
    this.status_vacunacion = "";;
    this.vacuna = new Array();
    this.enfermedad = new Array();
    this.recurso = "";
    this.clases = new Array();
    this.type = "";
  }
}

export class Vacuna {
  nombre: string;
  vacunas_disponibles: string;
  descripcion: string;
  recurso: string;
  clases: string[];
  type: string;
}


export class Enfermedad {
  nombre: string;
  recurso: string;
  clases: string[];
  type: string;
}

export class ParametroBusqueda {
  ciudad: string;
  dosis_aplicadas: string;
  status_vacunacion: string;
  vacuna: string;
  enfermedad: string;

  constructor(){
    this.ciudad = "";
    this.dosis_aplicadas = "";
    this.status_vacunacion = "";
    this.vacuna = "";
    this.enfermedad = "";
  }

}
