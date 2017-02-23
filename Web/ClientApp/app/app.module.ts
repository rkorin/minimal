import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { LoginComponent } from './components/login/login.component';
import { AccountsComponent } from './components/accounts/accounts.component';
import { RolesComponent } from './components/roles/roles.component';

import { AuthModule, AuthService, AuthGuard } from './common';
import { CommonModule }  from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PagesModule, PagesComponent, AdminPageComponent, NormalPageComponent, PowerPageComponent } from './pages'

@NgModule({
    bootstrap: [AppComponent],
    declarations: [
        AppComponent,
        NavMenuComponent,
        DashboardComponent,
        LoginComponent,

        AccountsComponent, RolesComponent
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        CommonModule, FormsModule, ReactiveFormsModule,
        AuthModule,
        PagesModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'dashboard', pathMatch: 'full', canActivate: [AuthGuard] },
            { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
            { path: 'login', component: LoginComponent },
            { path: 'accounts', component: AccountsComponent, canActivate: [AuthGuard] },
            { path: 'roles', component: RolesComponent, canActivate: [AuthGuard] },

            { path: 'pages/normal', component: NormalPageComponent, canActivate: [AuthGuard] },
            { path: 'pages/power', component: PowerPageComponent, canActivate: [AuthGuard] },
            { path: 'pages/admin', component: AdminPageComponent, canActivate: [AuthGuard] },

            //{
            //    path: 'pages', component: PagesComponent, canActivate: [AuthGuard],
            //    children: [
            //        {
            //            path: '',
            //            canActivateChild: [AuthGuard],
            //            children: [
            //                { path: 'normal', component: NormalPageComponent },
            //                { path: 'power', component: PowerPageComponent },
            //                { path: 'admin', component: AdminPageComponent },
            //            ]
            //        }]
            //},

            { path: '**', redirectTo: 'dashboard' }
        ], { useHash: true })
    ]
})
export class AppModule {
}
