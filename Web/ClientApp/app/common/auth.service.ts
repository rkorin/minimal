// auth.service.ts

import { tokenNotExpired, JwtHelper } from 'angular2-jwt';
import { Injectable } from '@angular/core';
import { isBrowser } from 'angular2-universal';

@Injectable()
export class AuthService {
    loggedIn() {
        if (!this.Roles || !this.Claims) {
            let token = localStorage.getItem('id_token'),
                jwtHelper: JwtHelper = new JwtHelper(),
                dtoken = jwtHelper.decodeToken(token);
            this.update_token_info(dtoken);
        }
        return tokenNotExpired();
    }

    private login: string;
    private passwd: string;
    public Roles: any = null;
    public Claims: any = null;

    public clear() {
        this.login = '';
        this.passwd = '';
    }

    protected update_token_info(result: any) {
        this.Claims = {};
        this.Roles = {};
        result = result || {};
        for (var n in result || {}) {
            if (result.hasOwnProperty(n)) {
                let name: string = n.toString().toLowerCase();
                if (name.length > 4 && name.substring(0, 4) == 'sec_') {
                    let prefix = name.substring(0, 4);
                    let body = name.substring(4);
                    this.Claims[body] = this.parse_claims(result[n]);
                }
                if (name == 'role_list')
                    this.parse_roles(result[n]);
            }
        }
    }

    public update(eml: string, pwd: string, result: any) {
        this.login = eml;
        this.passwd = pwd;
        this.update_token_info(result);
    }

    public parse_roles(v: string): any {
        v = v || '';
        let roles = v.split(',');
        for (var i = 0; i < roles.length; ++i) {
            if (roles[i])
                this.Roles[roles[i]] = true;
        }
    }

    public parse_claims(v: string): any {
        var t = {};
        t[v] = true;
        return t;
    }
}