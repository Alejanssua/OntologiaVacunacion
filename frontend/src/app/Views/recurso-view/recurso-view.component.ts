import { Component, OnInit } from '@angular/core';
import { ApiService } from "../../Services/api.service";
import { ActivatedRoute } from '@angular/router';
import { Params } from '@angular/router/src/shared';
import { Router } from '@angular/router';
 
@Component({
  selector: 'app-recurso-view',
  templateUrl: './recurso-view.component.html',
  styleUrls: ['./recurso-view.component.css']
})
export class RecursoViewComponent implements OnInit {

  constructor(private api: ApiService,
    private route: ActivatedRoute,private _router: Router) { }

  public model: any;
  public resourceName;
  viewNum = -1;

  ngOnInit() {
    const g = this.route.params.subscribe(
      (params: Params) => {
        this.resourceName = params['resource'];
        this.getResource();
      })
  }

  getResource() {
    this.api.getRecurso(this.resourceName).subscribe(response => {
      this.model = response.json();
      this.showModel();
    }, error => {
      //error
      this._router.navigate(['**']);

    });
  }

  showModel() {
    if (this.model != null) {
      if (this.model.type == "Persona") {
        this.viewNum = 0;
      } else if (this.model.type == "Enfermedad") {
        this.viewNum = 1;
      } else if (this.model.type == "Vacuna") {
        this.viewNum = 2;
      }
    } 
  }

}
