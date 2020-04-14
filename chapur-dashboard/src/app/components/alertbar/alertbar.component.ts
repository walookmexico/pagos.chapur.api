import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-alertbar',
  templateUrl: './alertbar.component.html',
  styleUrls: ['./alertbar.component.css']
})
export class AlertbarComponent implements OnInit {

  @Input() type: string;
  @Input() title: string;
  @Input() description: string;
  @Input() noButtons: boolean = false;
  @Input() dismissButton: boolean = true;
  @Input() actionButton: boolean = true;
  @Output() dismissClicked = new EventEmitter();
  @Output() actionClicked = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

}
