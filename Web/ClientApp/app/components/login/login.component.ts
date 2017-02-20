import { Component } from '@angular/core';
import { AuthHttp } from 'angular2-jwt';
import { Observable } from 'rxjs/Observable';
import { Http, Response, Headers, RequestOptions } from '@angular/http';

@Component({
    selector: 'login',
    template: require('./login.component.html')
})
export class LoginComponent {

    public baseUrl = 'http://localhost:58172/api/';
    public controller = 'auth';


    email: string;
    pwd: string;

    constructor(protected authHttp: Http) {
    }


    protected extractData(res: Response) {
        let body = res.json() || {};
        return body.data || body || {};
    }

    protected handleError(error: Response | any) {

        // In a real world app, we might use a remote logging infrastructure
        let errMsg: string;
        if (error instanceof Response) {
            const body = error.json() || '';
            const err = body.error || JSON.stringify(body);
            errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
        } else {
            errMsg = error.message ? error.message : error.toString();
        }
        console.error(errMsg);
        return Observable.throw(errMsg);
    }

    onSubmit() {
        debugger;
        this.authHttp.post(this.baseUrl + 'Account/Login', {
            Email: this.email, Password: this.pwd, RememberMe: true
        }).subscribe(result => {
            debugger;
        });
    }
}
