import { Router } from '@angular/router';
import { Component, OnInit, Input } from '@angular/core';
import { Persona } from '../../model';
import { ApiService } from '../../Services/api.service';


@Component({
  selector: 'app-resultados',
  templateUrl: './resultados.component.html',
  styleUrls: ['./resultados.component.css']
})
export class ResultadosComponent implements OnInit {

  @Input() public editorCheck: boolean;
  @Input() public results: Persona[];

  iniciado:boolean = false;
  constructor(private _router: Router, private api: ApiService) {
    this.editorCheck = false;
    
  }

  ngOnInit() {
  }

  redireccionToFrutas(model: Persona) {
    this._router.navigate(['recurso/' + model.recurso]);
  }

  eliminarFruta(model: Persona) {
    this.api.eliminarFruta(model.recurso).subscribe(response => {
      this.results.splice(this.results.indexOf(model), 1);
    }, error => {
      alert('El recurso no ha podido ser eliminado');
    }); 
  }

}
