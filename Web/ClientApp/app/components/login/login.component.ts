import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthHttp } from 'angular2-jwt';
import { Observable } from 'rxjs/Observable';
import { Http, Response, Headers, RequestOptions } from '@angular/http';

@Component({
    selector: 'login',
    template: require('./login.component.html')
})
export class LoginComponent { 
    email: string;
    pwd: string;

    constructor(protected authHttp: Http, private router: Router) {
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

    protected buildOptions(): RequestOptions {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        return new RequestOptions({
            headers: headers            
        });
    }


    onSubmit() { 
        let options = this.buildOptions();
        this.authHttp.post('/token', {
            email: this.email, password: this.pwd, rememberMe: true
        }, options).subscribe(result => {            
            if (result && result["_body"]) {
                var json = JSON.parse(result["_body"]); 
                localStorage.setItem('id_token', json['access_token']);
                this.router.navigate(['/']);
            }
        });
    }
}
