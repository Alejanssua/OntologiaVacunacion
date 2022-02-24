import { stringify } from 'querystring';
import { Component, OnInit, Input,ElementRef,ViewChild } from '@angular/core'; 
import { Persona, ParametroBusqueda } from '../../model';
import { ApiService } from '../../Services/api.service';
import { ResultadosComponent } from '../resultados/resultados.component'; 
declare var jQuery: any;
@Component({
  selector: 'app-param-busqueda',
  templateUrl: './param-busqueda.component.html',
  styleUrls: ['./param-busqueda.component.css']
})
export class ParamBusquedaComponent implements OnInit {

  @Input() public editor = false;
  @ViewChild('consultaModal') myModal: ElementRef;

  public ciudad:  string;  
  public dosis_aplicadas:  string;  
  public status_vacunacion:  string;
  public vacuna:  string;  
  public enfermedad:  string;  

  @Input() res: ResultadosComponent;

  constructor(private api:ApiService) { }

  ngOnInit() {
  }

  consulta(){
    let f = new ParametroBusqueda();
    if(this.status_vacunacion != null && this.status_vacunacion != ""){
      f.status_vacunacion = this.status_vacunacion;
    }

    if(this.ciudad != null){
      f.ciudad = this.ciudad;
    }

    if(this.dosis_aplicadas != null){
      f.dosis_aplicadas = this.dosis_aplicadas;
    }

    if(this.enfermedad != null){
      f.enfermedad = this.enfermedad;
    }
    
    if(this.vacuna != null){
      f.vacuna = this.vacuna;
    }



    console.log(f);
    this.api.consulta(f).subscribe(response => {
       this.res.results = response.json();
       this.res.iniciado = true;       
       jQuery(this.myModal.nativeElement).modal('hide');
    }, error => {
      alert('Parametros de Simulación no pudieron ser obtenidos');
    }); 

  }

}