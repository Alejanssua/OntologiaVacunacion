import { Component, OnInit, Input } from '@angular/core';
import { Persona } from '../../model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-fruta',
  templateUrl: './fruta.component.html',
  styleUrls: ['./fruta.component.css']
})
export class FrutaComponent implements OnInit {

  @Input() public model: Persona;
  constructor(private _router: Router) { }

  ngOnInit() {
    console.log(this.model);
  }

  redirectToResource(res) {
    this._router.navigate(['recurso/'+res.recurso]);
  }

}
