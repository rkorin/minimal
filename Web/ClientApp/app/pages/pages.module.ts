import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
//import { routing } from './pages.routing';

import { PagesComponent } from './pages.component';

import { AdminPageComponent } from './admin-page/admin-page.component';
import { NormalPageComponent } from './normal-page/normal-page.component';
import { PowerPageComponent } from './power-page/power-page.component';

@NgModule({
    imports: [CommonModule, FormsModule, RouterModule /*, routing*/],
    declarations: [PagesComponent, NormalPageComponent, PowerPageComponent, AdminPageComponent],
    providers: [],
    exports: []
})
export class PagesModule {
}