// auth.service.ts

import { tokenNotExpired, JwtHelper } from 'angular2-jwt';
import { Injectable } from '@angular/core';
import { isBrowser } from 'angular2-universal';

@Injectable()
export class AuthService {
    loggedIn() {
        return tokenNotExpired();
    }

    private login: string;
    private passwd: string;
    private Roles: any;
    private Claims: any;

    public clear() {
        this.login = '';
        this.passwd = '';
    }

    public update(eml: string,
        pwd: string,
        result: any) {
        debugger;
        this.login = eml;
        this.passwd = pwd;
    }
}