//import { Routes, RouterModule } from '@angular/router';
//import { PagesComponent } from './pages.component';
//import { AuthGuard } from '../common';

//import { AdminPageComponent } from './admin-page/admin-page.component';
//import { NormalPageComponent } from './normal-page/normal-page.component';
//import { PowerPageComponent } from './power-page/power-page.component';

//// noinspection TypeScriptValidateTypes
//const routes: Routes = [
//    {
//        path: 'pages',
//        component: PagesComponent,
//        canActivate: [AuthGuard],
//        children: [
//            {
//                path: '',
//                canActivateChild: [AuthGuard],
//                children: [
//                    { path: 'normal', component: NormalPageComponent }, 
//                    { path: 'power', component: PowerPageComponent }, 
//                    { path: 'admin', component: AdminPageComponent }, 
//                ]
//            }]
//    }
//];

//export const routing = RouterModule.forChild(routes);

