import {Component, ViewEncapsulation, Input, EventEmitter} from '@angular/core'; 
import { Routes, RouterModule } from '@angular/router';

@Component({
	selector: 'pages',
	encapsulation: ViewEncapsulation.None,
	styles: [],
    template: ` 
<span>page-headers</span>
        <router-outlet></router-outlet>
<span>page-footers</span>
      `
})
export class PagesComponent {

	constructor( ) { 
	}
     
}
