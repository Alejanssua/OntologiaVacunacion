import { Component, OnInit, Input } from '@angular/core';
import { Enfermedad } from '../../model';

@Component({
  selector: 'app-mineral',
  templateUrl: './mineral.component.html',
  styleUrls: ['./mineral.component.css']
})
export class MineralComponent implements OnInit {

  constructor() { }
  @Input() public model:Enfermedad;
  ngOnInit() {
  }

}
