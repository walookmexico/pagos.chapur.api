import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-loader-process',
  templateUrl: './loader-process.component.html',
  styleUrls: ['./loader-process.component.css']
})
export class LoaderProcessComponent implements OnInit {

  @Input() showLoading: boolean;

  constructor() {
    this.showLoading = false;
  }

  ngOnInit() {
  }

}
