// auth.service.ts

import { tokenNotExpired } from 'angular2-jwt';
import { Injectable } from '@angular/core';
import { isBrowser } from 'angular2-universal';

@Injectable()
export class AuthService {
    loggedIn() { 
        return tokenNotExpired();
    }
}